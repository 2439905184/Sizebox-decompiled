using System.Collections.Generic;
using UnityEngine;

namespace UltimateFracturing
{
	public class MeshData
	{
		public class IncreasingSizeComparer : IComparer<MeshData>
		{
			private int nSplitAxis;

			public IncreasingSizeComparer(int nSplitAxis)
			{
				this.nSplitAxis = nSplitAxis;
			}

			public int Compare(MeshData a, MeshData b)
			{
				Vector3 v3Min = a.v3Min;
				Vector3 v3Max = a.v3Max;
				Vector3 v3Min2 = b.v3Min;
				Vector3 v3Max2 = b.v3Max;
				if (nSplitAxis == 0)
				{
					if (!(v3Max.x - v3Min.x - (v3Max2.x - v3Min2.x) < 0f))
					{
						return 1;
					}
					return -1;
				}
				if (nSplitAxis == 1)
				{
					if (!(v3Max.y - v3Min.y - (v3Max2.y - v3Min2.y) < 0f))
					{
						return 1;
					}
					return -1;
				}
				if (nSplitAxis == 2)
				{
					if (!(v3Max.z - v3Min.z - (v3Max2.z - v3Min2.z) < 0f))
					{
						return 1;
					}
					return -1;
				}
				if (!(Mathf.Max(v3Max.x - v3Min.x, v3Max.y - v3Min.y, v3Max.z - v3Min.z) - Mathf.Max(v3Max2.x - v3Min2.x, v3Max2.y - v3Min2.y, v3Max2.z - v3Min2.z) < 0f))
				{
					return 1;
				}
				return -1;
			}
		}

		public class DecreasingSizeComparer : IComparer<MeshData>
		{
			private int nSplitAxis;

			public DecreasingSizeComparer(int nSplitAxis)
			{
				this.nSplitAxis = nSplitAxis;
			}

			public int Compare(MeshData a, MeshData b)
			{
				Vector3 v3Min = a.v3Min;
				Vector3 v3Max = a.v3Max;
				Vector3 v3Min2 = b.v3Min;
				Vector3 v3Max2 = b.v3Max;
				if (nSplitAxis == 0)
				{
					if (!(v3Max.x - v3Min.x - (v3Max2.x - v3Min2.x) < 0f))
					{
						return -1;
					}
					return 1;
				}
				if (nSplitAxis == 1)
				{
					if (!(v3Max.y - v3Min.y - (v3Max2.y - v3Min2.y) < 0f))
					{
						return -1;
					}
					return 1;
				}
				if (nSplitAxis == 2)
				{
					if (!(v3Max.z - v3Min.z - (v3Max2.z - v3Min2.z) < 0f))
					{
						return -1;
					}
					return 1;
				}
				if (!(Mathf.Max(v3Max.x - v3Min.x, v3Max.y - v3Min.y, v3Max.z - v3Min.z) - Mathf.Max(v3Max2.x - v3Min2.x, v3Max2.y - v3Min2.y, v3Max2.z - v3Min2.z) < 0f))
				{
					return -1;
				}
				return 1;
			}
		}

		public int nSubMeshCount;

		public int[][] aaIndices;

		public int nSplitCloseSubMesh;

		public VertexData[] aVertexData;

		public Vector3 v3Position;

		public Quaternion qRotation;

		public Vector3 v3Scale;

		public Vector3 v3Min;

		public Vector3 v3Max;

		public int nCurrentVertexHash;

		public Material[] aMaterials;

		public MeshDataConnectivity meshDataConnectivity;

		private MeshData()
		{
			meshDataConnectivity = new MeshDataConnectivity();
			aMaterials = new Material[1];
			aMaterials[0] = null;
		}

		public MeshData(Material[] aMaterials, List<int>[] alistIndices, List<VertexData> listVertexData, int nSplitCloseSubMesh, Vector3 v3Position, Quaternion qRotation, Vector3 v3Scale, Matrix4x4 mtxTransformVertices, bool bUseTransform, bool bBuildVertexHashData)
		{
			nSubMeshCount = alistIndices.Length;
			aaIndices = new int[nSubMeshCount][];
			for (int i = 0; i < nSubMeshCount; i++)
			{
				aaIndices[i] = alistIndices[i].ToArray();
			}
			this.nSplitCloseSubMesh = nSplitCloseSubMesh;
			aVertexData = listVertexData.ToArray();
			if (bUseTransform)
			{
				for (int j = 0; j < aVertexData.Length; j++)
				{
					aVertexData[j].v3Vertex = mtxTransformVertices.MultiplyPoint3x4(aVertexData[j].v3Vertex);
				}
			}
			ComputeMinMax(aVertexData, ref v3Min, ref v3Max);
			this.v3Position = v3Position;
			this.qRotation = qRotation;
			this.v3Scale = v3Scale;
			meshDataConnectivity = new MeshDataConnectivity();
			if (bBuildVertexHashData)
			{
				BuildVertexHashData();
			}
			if (aMaterials != null)
			{
				this.aMaterials = new Material[aMaterials.Length];
				aMaterials.CopyTo(this.aMaterials, 0);
			}
			else
			{
				this.aMaterials = new Material[1];
				this.aMaterials[0] = null;
			}
		}

		public MeshData(Transform transform, Mesh mesh, Material[] aMaterials, Matrix4x4 mtxLocalToWorld, bool bTransformVerticesToWorld, int nSplitCloseSubMesh, bool bBuildVertexHashData)
			: this(transform.position, transform.rotation, transform.localScale, mesh, aMaterials, mtxLocalToWorld, bTransformVerticesToWorld, nSplitCloseSubMesh, bBuildVertexHashData)
		{
		}

		public MeshData(Vector3 v3Position, Quaternion qRotation, Vector3 v3Scale, Mesh mesh, Material[] aMaterials, Matrix4x4 mtxLocalToWorld, bool bTransformVerticesToWorld, int nSplitCloseSubMesh, bool bBuildVertexHashData)
		{
			nSubMeshCount = mesh.subMeshCount;
			aaIndices = new int[nSubMeshCount][];
			for (int i = 0; i < nSubMeshCount; i++)
			{
				aaIndices[i] = mesh.GetTriangles(i);
			}
			this.nSplitCloseSubMesh = nSplitCloseSubMesh;
			aVertexData = VertexData.BuildVertexDataArray(mesh, mtxLocalToWorld, bTransformVerticesToWorld);
			ComputeMinMax(aVertexData, ref v3Min, ref v3Max);
			this.v3Position = v3Position;
			this.qRotation = qRotation;
			this.v3Scale = v3Scale;
			if (bTransformVerticesToWorld)
			{
				v3Scale = Vector3.one;
			}
			meshDataConnectivity = new MeshDataConnectivity();
			if (bBuildVertexHashData)
			{
				BuildVertexHashData();
			}
			if (aMaterials != null)
			{
				this.aMaterials = new Material[aMaterials.Length];
				aMaterials.CopyTo(this.aMaterials, 0);
			}
			else
			{
				this.aMaterials = new Material[1];
				this.aMaterials[0] = null;
			}
		}

		public static MeshData CreateBoxMeshData(Vector3 v3Pos, Quaternion qRot, Vector3 v3Scale, Vector3 v3Min, Vector3 v3Max)
		{
			MeshData meshData = new MeshData();
			meshData.nSubMeshCount = 1;
			meshData.aaIndices = new int[1][];
			meshData.aaIndices[0] = new int[36]
			{
				1, 0, 3, 1, 3, 2, 4, 5, 7, 5,
				6, 7, 0, 4, 3, 4, 7, 3, 7, 2,
				3, 7, 6, 2, 5, 0, 1, 5, 4, 0,
				6, 1, 2, 6, 5, 1
			};
			meshData.nSplitCloseSubMesh = 0;
			Vector3[] array = new Vector3[8]
			{
				new Vector3(v3Min.x, v3Min.y, v3Min.z),
				new Vector3(v3Min.x, v3Min.y, v3Max.z),
				new Vector3(v3Max.x, v3Min.y, v3Max.z),
				new Vector3(v3Max.x, v3Min.y, v3Min.z),
				new Vector3(v3Min.x, v3Max.y, v3Min.z),
				new Vector3(v3Min.x, v3Max.y, v3Max.z),
				new Vector3(v3Max.x, v3Max.y, v3Max.z),
				new Vector3(v3Max.x, v3Max.y, v3Min.z)
			};
			meshData.aVertexData = new VertexData[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				meshData.aVertexData[i] = new VertexData(i);
				meshData.aVertexData[i].v3Vertex = array[i];
			}
			meshData.v3Position = v3Pos;
			meshData.qRotation = qRot;
			meshData.v3Scale = v3Scale;
			meshData.v3Min = v3Min;
			meshData.v3Max = v3Max;
			meshData.nCurrentVertexHash = 8;
			return meshData;
		}

		public MeshData GetDeepCopy()
		{
			MeshData meshData = new MeshData();
			meshData.nSubMeshCount = nSubMeshCount;
			meshData.aaIndices = new int[nSubMeshCount][];
			for (int i = 0; i < nSubMeshCount; i++)
			{
				meshData.aaIndices[i] = new int[aaIndices[i].Length];
				aaIndices[i].CopyTo(meshData.aaIndices[i], 0);
			}
			meshData.nSplitCloseSubMesh = nSplitCloseSubMesh;
			meshData.aVertexData = new VertexData[aVertexData.Length];
			aVertexData.CopyTo(meshData.aVertexData, 0);
			meshData.v3Position = v3Position;
			meshData.qRotation = qRotation;
			meshData.v3Scale = v3Scale;
			meshData.v3Min = v3Min;
			meshData.v3Max = v3Max;
			meshData.nCurrentVertexHash = nCurrentVertexHash;
			meshData.meshDataConnectivity = meshDataConnectivity.GetDeepCopy();
			meshData.aMaterials = new Material[aMaterials.Length];
			aMaterials.CopyTo(meshData.aMaterials, 0);
			return meshData;
		}

		public bool FillMeshFilter(MeshFilter meshFilter, bool bTransformVerticesToLocal)
		{
			meshFilter.transform.position = v3Position;
			meshFilter.transform.rotation = qRotation;
			meshFilter.transform.localScale = v3Scale;
			meshFilter.sharedMesh = new Mesh();
			Mesh sharedMesh = meshFilter.sharedMesh;
			VertexData.SetMeshDataFromVertexDataArray(meshFilter, this, bTransformVerticesToLocal);
			sharedMesh.subMeshCount = nSubMeshCount;
			for (int i = 0; i < nSubMeshCount; i++)
			{
				sharedMesh.SetTriangles(aaIndices[i], i);
			}
			sharedMesh.RecalculateBounds();
			return true;
		}

		public static void ComputeMinMax(IEnumerable<VertexData> VertexData, ref Vector3 v3Min, ref Vector3 v3Max)
		{
			v3Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			v3Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			foreach (VertexData VertexDatum in VertexData)
			{
				if (VertexDatum.v3Vertex.x < v3Min.x)
				{
					v3Min.x = VertexDatum.v3Vertex.x;
				}
				if (VertexDatum.v3Vertex.y < v3Min.y)
				{
					v3Min.y = VertexDatum.v3Vertex.y;
				}
				if (VertexDatum.v3Vertex.z < v3Min.z)
				{
					v3Min.z = VertexDatum.v3Vertex.z;
				}
				if (VertexDatum.v3Vertex.x > v3Max.x)
				{
					v3Max.x = VertexDatum.v3Vertex.x;
				}
				if (VertexDatum.v3Vertex.y > v3Max.y)
				{
					v3Max.y = VertexDatum.v3Vertex.y;
				}
				if (VertexDatum.v3Vertex.z > v3Max.z)
				{
					v3Max.z = VertexDatum.v3Vertex.z;
				}
			}
		}

		private void BuildVertexHashData()
		{
			List<Dictionary<int, Vector3>> list = new List<Dictionary<int, Vector3>>();
			List<float> list2 = new List<float>();
			int num = aVertexData.Length / Parameters.VERTICESSPACESUBDIVISION + 1;
			for (int i = 0; i < num; i++)
			{
				float item = v3Min.y + (float)(i + 1) / (float)num * (v3Max.y - v3Min.y);
				list.Add(new Dictionary<int, Vector3>());
				list2.Add(item);
			}
			int[] array = new int[3];
			nCurrentVertexHash = 0;
			int num2 = 0;
			for (int j = 0; j < aVertexData.Length; j++)
			{
				Vector3 v3Vertex = aVertexData[j].v3Vertex;
				int num3 = Mathf.FloorToInt((v3Vertex.y - v3Min.y) / (v3Max.y - v3Min.y) * (float)list2.Count);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num3 >= list2.Count)
				{
					num3 = list2.Count - 1;
				}
				array[0] = num3;
				array[1] = -1;
				array[2] = -1;
				int num4 = 1;
				if (Mathf.Abs(list2[num3] - v3Vertex.y) < Parameters.EPSILONDISTANCEPLANE && num3 < list2.Count - 1)
				{
					array[num4++] = num3 + 1;
				}
				if (num3 > 0 && Mathf.Abs(list2[num3 - 1] - v3Vertex.y) < Parameters.EPSILONDISTANCEPLANE)
				{
					array[num4++] = num3 - 1;
				}
				int num5 = -1;
				foreach (int num6 in array)
				{
					if (num6 == -1)
					{
						continue;
					}
					foreach (KeyValuePair<int, Vector3> item2 in list[num6])
					{
						if (Vector3.Distance(item2.Value, v3Vertex) < Parameters.EPSILONDISTANCEVERTEX)
						{
							num5 = item2.Key;
							break;
						}
					}
				}
				if (num5 == -1)
				{
					int num7 = nCurrentVertexHash++;
					list[num3].Add(num7, v3Vertex);
					aVertexData[j].nVertexHash = num7;
					num2++;
				}
				else
				{
					aVertexData[j].nVertexHash = num5;
				}
			}
		}

		public bool GetSharedFacesArea(FracturedObject fracturedComponent, MeshData meshData2, out float fSharedArea)
		{
			fSharedArea = 0f;
			bool result = false;
			foreach (int key in meshDataConnectivity.dicHash2FaceList.Keys)
			{
				if (!meshData2.meshDataConnectivity.dicHash2FaceList.ContainsKey(key))
				{
					continue;
				}
				foreach (MeshDataConnectivity.Face item in meshDataConnectivity.dicHash2FaceList[key])
				{
					Vector3 v3Vertex = aVertexData[aaIndices[item.nSubMesh][item.nFaceIndex * 3]].v3Vertex;
					Vector3 v3Vertex2 = aVertexData[aaIndices[item.nSubMesh][item.nFaceIndex * 3 + 1]].v3Vertex;
					float magnitude = Vector3.Cross(aVertexData[aaIndices[item.nSubMesh][item.nFaceIndex * 3 + 2]].v3Vertex - v3Vertex, v3Vertex2 - v3Vertex).magnitude;
					foreach (MeshDataConnectivity.Face item2 in meshData2.meshDataConnectivity.dicHash2FaceList[key])
					{
						Vector3 v3Vertex3 = meshData2.aVertexData[meshData2.aaIndices[item2.nSubMesh][item2.nFaceIndex * 3]].v3Vertex;
						Vector3 v3Vertex4 = meshData2.aVertexData[meshData2.aaIndices[item2.nSubMesh][item2.nFaceIndex * 3 + 1]].v3Vertex;
						float magnitude2 = Vector3.Cross(meshData2.aVertexData[meshData2.aaIndices[item2.nSubMesh][item2.nFaceIndex * 3 + 2]].v3Vertex - v3Vertex3, v3Vertex4 - v3Vertex3).magnitude;
						bool flag = false;
						if (Face2InsideFace1(fracturedComponent, this, meshData2, item, item2))
						{
							flag = true;
						}
						else if (Face2InsideFace1(fracturedComponent, meshData2, this, item2, item))
						{
							flag = true;
						}
						if (flag)
						{
							fSharedArea += Mathf.Min(magnitude, magnitude2);
							result = true;
						}
					}
				}
			}
			return result;
		}

		private static bool Face2InsideFace1(FracturedObject fracturedComponent, MeshData meshData1, MeshData meshData2, MeshDataConnectivity.Face face1, MeshDataConnectivity.Face face2)
		{
			if (fracturedComponent.FracturePattern == FracturedObject.EFracturePattern.BSP && !meshData1.meshDataConnectivity.dicFace2IsClipped.ContainsKey(face1) && !meshData2.meshDataConnectivity.dicFace2IsClipped.ContainsKey(face2))
			{
				return true;
			}
			Vector3 v3Vertex = meshData1.aVertexData[meshData1.aaIndices[face1.nSubMesh][face1.nFaceIndex * 3]].v3Vertex;
			Vector3 v3Vertex2 = meshData1.aVertexData[meshData1.aaIndices[face1.nSubMesh][face1.nFaceIndex * 3 + 1]].v3Vertex;
			Vector3 v3Vertex3 = meshData1.aVertexData[meshData1.aaIndices[face1.nSubMesh][face1.nFaceIndex * 3 + 2]].v3Vertex;
			Vector3 vector = -Vector3.Cross(v3Vertex2 - v3Vertex, v3Vertex3 - v3Vertex);
			if (vector.magnitude < Parameters.EPSILONCROSSPRODUCT)
			{
				return false;
			}
			Quaternion q = Quaternion.LookRotation(vector.normalized, (v3Vertex2 - v3Vertex).normalized);
			Matrix4x4 inverse = Matrix4x4.TRS(v3Vertex, q, Vector3.one).inverse;
			Vector3 v3Vertex4 = meshData2.aVertexData[meshData2.aaIndices[face2.nSubMesh][face2.nFaceIndex * 3]].v3Vertex;
			Vector3 v3Vertex5 = meshData2.aVertexData[meshData2.aaIndices[face2.nSubMesh][face2.nFaceIndex * 3 + 1]].v3Vertex;
			Vector3 v3Vertex6 = meshData2.aVertexData[meshData2.aaIndices[face2.nSubMesh][face2.nFaceIndex * 3 + 2]].v3Vertex;
			Vector3 point = (v3Vertex4 + v3Vertex5 + v3Vertex6) / 3f;
			point = inverse.MultiplyPoint3x4(point);
			Vector3 vector2 = inverse.MultiplyPoint3x4(v3Vertex);
			Vector3 vector3 = inverse.MultiplyPoint3x4(v3Vertex2);
			Vector3 vector4 = inverse.MultiplyPoint3x4(v3Vertex3);
			Vector3 lhs = vector4 - vector3;
			Vector3 lhs2 = vector2 - vector4;
			bool flag = false;
			if (point.x >= 0f && Vector3.Cross(lhs, point - vector3).z <= 0f && Vector3.Cross(lhs2, point - vector4).z <= 0f)
			{
				flag = true;
			}
			if (!flag)
			{
				Vector3 vector5 = inverse.MultiplyPoint3x4(v3Vertex4);
				Vector3 vector6 = inverse.MultiplyPoint3x4(v3Vertex5);
				Vector3 vector7 = inverse.MultiplyPoint3x4(v3Vertex6);
				if (!flag && Fracturer.IntersectEdges2D(vector5.x, vector5.y, vector6.x, vector6.y, vector2.x, vector2.y, vector3.x, vector3.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector5.x, vector5.y, vector6.x, vector6.y, vector3.x, vector3.y, vector4.x, vector4.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector5.x, vector5.y, vector6.x, vector6.y, vector4.x, vector4.y, vector2.x, vector2.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector6.x, vector6.y, vector7.x, vector7.y, vector2.x, vector2.y, vector3.x, vector3.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector6.x, vector6.y, vector7.x, vector7.y, vector3.x, vector3.y, vector4.x, vector4.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector6.x, vector6.y, vector7.x, vector7.y, vector4.x, vector4.y, vector2.x, vector2.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector7.x, vector7.y, vector5.x, vector5.y, vector2.x, vector2.y, vector3.x, vector3.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector7.x, vector7.y, vector5.x, vector5.y, vector3.x, vector3.y, vector4.x, vector4.y))
				{
					flag = true;
				}
				if (!flag && Fracturer.IntersectEdges2D(vector7.x, vector7.y, vector5.x, vector5.y, vector4.x, vector4.y, vector2.x, vector2.y))
				{
					flag = true;
				}
			}
			return flag;
		}

		public static List<MeshData> PostProcessConnectivity(MeshData meshDataSource, MeshFaceConnectivity connectivity, MeshDataConnectivity meshConnectivity, List<int>[] alistIndices, List<VertexData> listVertexData, int nSplitCloseSubMesh, int nCurrentVertexHash, bool bTransformToLocal)
		{
			List<MeshData> list = new List<MeshData>();
			List<int>[] array = new List<int>[alistIndices.Length];
			int[] array2 = new int[alistIndices.Length];
			int num = 0;
			List<int>[] array3 = new List<int>[alistIndices.Length];
			List<VertexData> list2 = new List<VertexData>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < alistIndices.Length; i++)
			{
				array[i] = new List<int>();
				for (int j = 0; j < alistIndices[i].Count / 3; j++)
				{
					array[i].Add(j);
				}
				array2[i] = num;
				num += alistIndices[i].Count / 3;
				array3[i] = new List<int>();
			}
			while (StillHasFacesToProcess(array))
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k].Count > 0)
					{
						dictionary.Clear();
						list2.Clear();
						MeshDataConnectivity meshConnectivityOut = new MeshDataConnectivity();
						for (int l = 0; l < alistIndices.Length; l++)
						{
							array3[l].Clear();
						}
						LookForClosedObjectRecursive(connectivity, meshConnectivity, k, array[k][0], alistIndices, listVertexData, array, array2, array3, list2, dictionary, meshConnectivityOut);
						Vector3 zero = Vector3.zero;
						Vector3 zero2 = Vector3.zero;
						ComputeMinMax(list2, ref zero, ref zero2);
						Vector3 pos = (zero + zero2) * 0.5f;
						Matrix4x4 mtxTransformVertices = Matrix4x4.TRS(pos, meshDataSource.qRotation, meshDataSource.v3Scale);
						MeshData meshData = new MeshData(meshDataSource.aMaterials, array3, list2, nSplitCloseSubMesh, pos, meshDataSource.qRotation, meshDataSource.v3Scale, mtxTransformVertices, bTransformToLocal, false);
						meshData.meshDataConnectivity = meshConnectivityOut;
						meshData.nCurrentVertexHash = nCurrentVertexHash;
						list.Add(meshData);
					}
				}
			}
			return list;
		}

		private static bool StillHasFacesToProcess(List<int>[] alistFacesRemaining)
		{
			for (int i = 0; i < alistFacesRemaining.Length; i++)
			{
				if (alistFacesRemaining[i].Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		private static void LookForClosedObjectRecursive(MeshFaceConnectivity connectivity, MeshDataConnectivity meshConnectivity, int nSubMeshStart, int nFaceSubMeshStart, List<int>[] alistIndicesIn, List<VertexData> listVertexDataIn, List<int>[] alistFacesRemainingInOut, int[] aLinearFaceIndexStart, List<int>[] alistIndicesOut, List<VertexData> listVertexDataOut, Dictionary<int, int> dicVertexRemap, MeshDataConnectivity meshConnectivityOut)
		{
			MeshFaceConnectivity.TriangleData triangleData = connectivity.listTriangles[aLinearFaceIndexStart[nSubMeshStart] + nFaceSubMeshStart];
			if (triangleData.bVisited)
			{
				return;
			}
			meshConnectivityOut.NotifyRemappedFace(meshConnectivity, nSubMeshStart, nFaceSubMeshStart, nSubMeshStart, alistIndicesOut[nSubMeshStart].Count / 3);
			for (int i = 0; i < 3; i++)
			{
				int num = alistIndicesIn[nSubMeshStart][nFaceSubMeshStart * 3 + i];
				if (dicVertexRemap.ContainsKey(num))
				{
					alistIndicesOut[nSubMeshStart].Add(dicVertexRemap[num]);
					continue;
				}
				int count = listVertexDataOut.Count;
				alistIndicesOut[nSubMeshStart].Add(count);
				listVertexDataOut.Add(listVertexDataIn[num].Copy());
				dicVertexRemap.Add(num, count);
			}
			alistFacesRemainingInOut[nSubMeshStart].Remove(nFaceSubMeshStart);
			triangleData.bVisited = true;
			for (int j = 0; j < 3; j++)
			{
				MeshFaceConnectivity.TriangleData triangleData2 = null;
				for (int k = 0; k < triangleData.alistNeighborSubMeshes[j].Count; k++)
				{
					if (triangleData.alistNeighborSubMeshes[j][k] != -1 && triangleData.alistNeighborTriangles[j][k] != -1)
					{
						triangleData2 = connectivity.listTriangles[aLinearFaceIndexStart[triangleData.alistNeighborSubMeshes[j][k]] + triangleData.alistNeighborTriangles[j][k]];
					}
					if (triangleData2 != null && !triangleData2.bVisited)
					{
						LookForClosedObjectRecursive(connectivity, meshConnectivity, triangleData2.nSubMesh, triangleData2.nTriangle, alistIndicesIn, listVertexDataIn, alistFacesRemainingInOut, aLinearFaceIndexStart, alistIndicesOut, listVertexDataOut, dicVertexRemap, meshConnectivityOut);
					}
				}
			}
		}
	}
}
