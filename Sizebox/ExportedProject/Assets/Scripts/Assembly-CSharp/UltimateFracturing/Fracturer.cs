using System;
using System.Collections.Generic;
using Poly2Tri;
using UnityEngine;

namespace UltimateFracturing
{
	public static class Fracturer
	{
		private class FracturingStats
		{
			public int nChunkCount;

			public int nTotalChunks;

			public int nSplitCount;

			public bool bCancelFracturing;

			public FracturingStats()
			{
				nChunkCount = 0;
				nTotalChunks = 0;
				nSplitCount = 0;
				bCancelFracturing = false;
			}
		}

		public delegate void ProgressDelegate(string strTitle, string message, float fT);

		private class SpaceTreeNode
		{
			private SpaceTreeNode nodeOneSide;

			private SpaceTreeNode nodeOtherSide;

			private int nLevel;

			private int nSplitsX;

			private int nSplitsY;

			private int nSplitsZ;

			private Vector3 v3Min;

			private Vector3 v3Max;

			private List<MeshData> listMeshDatasSpace;

			public SpaceTreeNode()
			{
				nodeOneSide = null;
				nodeOtherSide = null;
				listMeshDatasSpace = new List<MeshData>();
				nLevel = 0;
				nSplitsX = 0;
				nSplitsY = 0;
				nSplitsZ = 0;
			}

			public bool ContainsCompletely(Vector3 v3Min, Vector3 v3Max)
			{
				if (((this.v3Min.x <= v3Min.x && this.v3Max.x >= v3Max.x) || nSplitsX == 0) && ((this.v3Min.y <= v3Min.y && this.v3Max.y >= v3Max.y) || nSplitsY == 0) && ((this.v3Min.z <= v3Min.z && this.v3Max.z >= v3Max.z) || nSplitsZ == 0))
				{
					return true;
				}
				return false;
			}

			public static List<MeshData> GetSmallestPossibleMeshData(SpaceTreeNode root, Vector3 v3Min, Vector3 v3Max)
			{
				if (!root.ContainsCompletely(v3Min, v3Max))
				{
					return root.listMeshDatasSpace;
				}
				SpaceTreeNode spaceTreeNode = root;
				while (true)
				{
					if (spaceTreeNode.nodeOneSide != null && spaceTreeNode.nodeOneSide.ContainsCompletely(v3Min, v3Max))
					{
						spaceTreeNode = spaceTreeNode.nodeOneSide;
						continue;
					}
					if (spaceTreeNode.nodeOtherSide == null || !spaceTreeNode.nodeOtherSide.ContainsCompletely(v3Min, v3Max))
					{
						break;
					}
					spaceTreeNode = spaceTreeNode.nodeOtherSide;
				}
				return spaceTreeNode.listMeshDatasSpace;
			}

			public static SpaceTreeNode BuildSpaceTree(MeshData meshDataIn, int nSubdivisionLevels, FracturedObject fracturedComponent, ProgressDelegate progress = null)
			{
				if (nSubdivisionLevels < 1)
				{
					return null;
				}
				SplitOptions splitOptions = new SplitOptions();
				splitOptions.bForceNoIslandGeneration = true;
				splitOptions.bForceNoChunkConnectionInfo = true;
				splitOptions.bForceNoIslandConnectionInfo = true;
				splitOptions.bForceNoCap = false;
				splitOptions.bVerticesAreLocal = true;
				SpaceTreeNode spaceTreeNode = new SpaceTreeNode();
				spaceTreeNode.listMeshDatasSpace = new List<MeshData>();
				spaceTreeNode.listMeshDatasSpace.Add(meshDataIn);
				spaceTreeNode.nLevel = 0;
				spaceTreeNode.nSplitsX = 0;
				spaceTreeNode.nSplitsY = 0;
				spaceTreeNode.nSplitsZ = 0;
				spaceTreeNode.v3Min = meshDataIn.v3Min;
				spaceTreeNode.v3Max = meshDataIn.v3Max;
				Queue<SpaceTreeNode> queue = new Queue<SpaceTreeNode>();
				queue.Enqueue(spaceTreeNode);
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				for (int i = 0; i < nSubdivisionLevels; i++)
				{
					num += Mathf.RoundToInt(Mathf.Pow(2f, i));
				}
				while (queue.Count > 0)
				{
					SpaceTreeNode spaceTreeNode2 = queue.Dequeue();
					if (spaceTreeNode2.nLevel >= nSubdivisionLevels)
					{
						continue;
					}
					if (progress != null)
					{
						progress("Fracturing", string.Format("Pre computing space volume (split {0}/{1}, Depth {2})", num2 + 1, num, spaceTreeNode2.nLevel + 1), Mathf.Clamp01((float)num2 / (float)num));
					}
					if (IsFracturingCancelled())
					{
						return null;
					}
					Vector3 up = Vector3.up;
					Vector3 right = Vector3.right;
					Vector3 v3PlanePoint = (spaceTreeNode2.v3Min + spaceTreeNode2.v3Max) * 0.5f;
					float num6 = spaceTreeNode2.v3Max.x - spaceTreeNode2.v3Min.x;
					float num7 = spaceTreeNode2.v3Max.y - spaceTreeNode2.v3Min.y;
					float num8 = spaceTreeNode2.v3Max.z - spaceTreeNode2.v3Min.z;
					Vector3 vector = spaceTreeNode2.v3Min;
					Vector3 vector2 = spaceTreeNode2.v3Max;
					Vector3 vector3 = spaceTreeNode2.v3Min;
					Vector3 vector4 = spaceTreeNode2.v3Max;
					if (num6 >= num7 && num6 >= num8)
					{
						up = Vector3.right;
						right = Vector3.forward;
						vector2.x = v3PlanePoint.x;
						vector3.x = v3PlanePoint.x;
						num3++;
					}
					else if (num7 >= num6 && num7 >= num8)
					{
						up = Vector3.up;
						right = Vector3.right;
						vector2.y = v3PlanePoint.y;
						vector3.y = v3PlanePoint.y;
						num4++;
					}
					else
					{
						up = Vector3.forward;
						right = Vector3.right;
						vector2.z = v3PlanePoint.z;
						vector3.z = v3PlanePoint.z;
						num5++;
					}
					foreach (MeshData item in spaceTreeNode2.listMeshDatasSpace)
					{
						List<MeshData> listMeshDatasPosOut;
						List<MeshData> listMeshDatasNegOut;
						if (SplitMeshUsingPlane(item, fracturedComponent, splitOptions, up, right, v3PlanePoint, out listMeshDatasPosOut, out listMeshDatasNegOut, progress))
						{
							spaceTreeNode2.nodeOneSide = new SpaceTreeNode();
							spaceTreeNode2.nodeOneSide.listMeshDatasSpace = listMeshDatasNegOut;
							spaceTreeNode2.nodeOneSide.v3Min = vector;
							spaceTreeNode2.nodeOneSide.v3Max = vector2;
							spaceTreeNode2.nodeOneSide.nLevel = spaceTreeNode2.nLevel + 1;
							spaceTreeNode2.nodeOneSide.nSplitsX = num3;
							spaceTreeNode2.nodeOneSide.nSplitsY = num4;
							spaceTreeNode2.nodeOneSide.nSplitsZ = num5;
							queue.Enqueue(spaceTreeNode2.nodeOneSide);
							spaceTreeNode2.nodeOtherSide = new SpaceTreeNode();
							spaceTreeNode2.nodeOtherSide.listMeshDatasSpace = listMeshDatasPosOut;
							spaceTreeNode2.nodeOtherSide.v3Min = vector3;
							spaceTreeNode2.nodeOtherSide.v3Max = vector4;
							spaceTreeNode2.nodeOtherSide.nLevel = spaceTreeNode2.nLevel + 1;
							spaceTreeNode2.nodeOtherSide.nSplitsX = num3;
							spaceTreeNode2.nodeOtherSide.nSplitsY = num4;
							spaceTreeNode2.nodeOtherSide.nSplitsZ = num5;
							queue.Enqueue(spaceTreeNode2.nodeOtherSide);
						}
					}
					num2++;
				}
				return spaceTreeNode;
			}
		}

		public class SplitOptions
		{
			public static SplitOptions Default = new SplitOptions();

			public bool bForceNoProgressInfo;

			public bool bForceNoIslandGeneration;

			public bool bForceNoChunkConnectionInfo;

			public bool bForceNoIslandConnectionInfo;

			public bool bForceNoCap;

			public bool bForceCapVertexSoup;

			public bool bIgnoreNegativeSide;

			public bool bVerticesAreLocal;

			public int nForceMeshConnectivityHash;

			public SplitOptions()
			{
				bForceNoProgressInfo = false;
				bForceNoIslandGeneration = false;
				bForceNoChunkConnectionInfo = false;
				bForceNoIslandConnectionInfo = false;
				bForceNoCap = false;
				bForceCapVertexSoup = false;
				bIgnoreNegativeSide = false;
				bVerticesAreLocal = false;
				nForceMeshConnectivityHash = -1;
			}
		}

		private static FracturingStats s_FracturingStats = new FracturingStats();

		public static void CancelFracturing()
		{
			lock (s_FracturingStats)
			{
				s_FracturingStats.bCancelFracturing = true;
			}
		}

		public static bool IsFracturingCancelled()
		{
			bool flag = false;
			lock (s_FracturingStats)
			{
				return s_FracturingStats.bCancelFracturing;
			}
		}

		public static bool FractureToChunks(FracturedObject fracturedComponent, bool bPositionOnSourceAndHideOriginal, out List<GameObject> listGameObjectsOut, ProgressDelegate progress = null)
		{
			listGameObjectsOut = new List<GameObject>();
			bool flag = false;
			GameObject sourceObject = fracturedComponent.SourceObject;
			MeshFilter meshFilter = null;
			Mesh mesh = null;
			if ((bool)sourceObject)
			{
				meshFilter = fracturedComponent.SourceObject.GetComponent<MeshFilter>();
				if ((bool)meshFilter)
				{
					mesh = CopyMesh(meshFilter);
					mesh.name = string.Format("mesh_{0}{1}", fracturedComponent.SourceObject.gameObject.name, fracturedComponent.SourceObject.gameObject.GetInstanceID().ToString());
				}
			}
			if (fracturedComponent.FracturePattern == FracturedObject.EFracturePattern.BSP)
			{
				flag = FractureToChunksBSP(fracturedComponent, bPositionOnSourceAndHideOriginal, out listGameObjectsOut, progress);
			}
			if (fracturedComponent.SingleMeshObject != null)
			{
				UnityEngine.Object.DestroyImmediate(fracturedComponent.SingleMeshObject);
			}
			if (flag && !IsFracturingCancelled())
			{
				fracturedComponent.OnCreateFracturedObject();
				fracturedComponent.SingleMeshObject = UnityEngine.Object.Instantiate(fracturedComponent.SourceObject);
				fracturedComponent.SingleMeshObject.name = "@" + fracturedComponent.name + " (single mesh)";
				fracturedComponent.SingleMeshObject.transform.localPosition = fracturedComponent.transform.position;
				fracturedComponent.SingleMeshObject.transform.localRotation = fracturedComponent.transform.rotation;
				fracturedComponent.SingleMeshObject.transform.localScale = fracturedComponent.SourceObject.transform.localScale;
				fracturedComponent.SingleMeshObject.transform.parent = fracturedComponent.transform;
				Component[] components = fracturedComponent.SingleMeshObject.GetComponents<Component>();
				foreach (Component component in components)
				{
					if (component.GetType() != typeof(Transform) && component.GetType() != typeof(MeshRenderer) && component.GetType() != typeof(MeshFilter))
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
				}
				fracturedComponent.SingleMeshObject.GetComponent<MeshFilter>().sharedMesh = mesh;
				fracturedComponent.SingleMeshObject.SetActive(true);
			}
			return flag;
		}

		private static Mesh CopyMesh(MeshFilter meshfIn)
		{
			Mesh mesh = new Mesh();
			Vector3[] vertices = meshfIn.sharedMesh.vertices;
			Vector3[] normals = meshfIn.sharedMesh.normals;
			Vector4[] tangents = meshfIn.sharedMesh.tangents;
			Vector2[] uv = meshfIn.sharedMesh.uv;
			Vector2[] uv2 = meshfIn.sharedMesh.uv2;
			Color[] colors = meshfIn.sharedMesh.colors;
			Color32[] colors2 = meshfIn.sharedMesh.colors32;
			if (vertices != null && vertices.Length != 0)
			{
				Vector3[] array = new Vector3[vertices.Length];
				vertices.CopyTo(array, 0);
				mesh.vertices = array;
			}
			if (normals != null && normals.Length != 0)
			{
				Vector3[] array2 = new Vector3[normals.Length];
				normals.CopyTo(array2, 0);
				mesh.normals = array2;
			}
			if (tangents != null && tangents.Length != 0)
			{
				Vector4[] array3 = new Vector4[tangents.Length];
				tangents.CopyTo(array3, 0);
				mesh.tangents = array3;
			}
			if (uv != null && uv.Length != 0)
			{
				Vector2[] array4 = new Vector2[uv.Length];
				uv.CopyTo(array4, 0);
				mesh.uv = array4;
			}
			if (uv2 != null && uv2.Length != 0)
			{
				Vector2[] array5 = new Vector2[uv2.Length];
				uv2.CopyTo(array5, 0);
				mesh.uv2 = array5;
			}
			if (colors != null && colors.Length != 0)
			{
				Color[] array6 = new Color[colors.Length];
				colors.CopyTo(array6, 0);
				mesh.colors = array6;
			}
			if (colors2 != null && colors2.Length != 0)
			{
				Color32[] array7 = new Color32[colors2.Length];
				colors2.CopyTo(array7, 0);
				mesh.colors32 = array7;
			}
			mesh.subMeshCount = meshfIn.sharedMesh.subMeshCount;
			for (int i = 0; i < meshfIn.sharedMesh.subMeshCount; i++)
			{
				int[] triangles = meshfIn.sharedMesh.GetTriangles(i);
				int[] array8 = new int[triangles.Length];
				triangles.CopyTo(array8, 0);
				mesh.SetTriangles(array8, i);
			}
			return mesh;
		}

		private static bool FractureToChunksBSP(FracturedObject fracturedComponent, bool bPositionOnSourceAndHideOriginal, out List<GameObject> listGameObjectsOut, ProgressDelegate progress = null)
		{
			listGameObjectsOut = new List<GameObject>();
			MeshFilter component = fracturedComponent.SourceObject.GetComponent<MeshFilter>();
			if (component == null)
			{
				return false;
			}
			s_FracturingStats = new FracturingStats();
			s_FracturingStats.nTotalChunks = fracturedComponent.GenerateNumChunks;
			if (progress != null)
			{
				progress("Fracturing", "Initializing...", 0f);
			}
			foreach (FracturedChunk listFracturedChunk in fracturedComponent.ListFracturedChunks)
			{
				if (listFracturedChunk != null)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.gameObject);
				}
			}
			fracturedComponent.ListFracturedChunks.Clear();
			fracturedComponent.DecomposeRadius = (component.sharedMesh.bounds.max - component.sharedMesh.bounds.min).magnitude;
			UnityEngine.Random.InitState(fracturedComponent.RandomSeed);
			FracturedChunk component2 = fracturedComponent.gameObject.GetComponent<FracturedChunk>();
			int num = ((component2 != null) ? component2.SplitSubMeshIndex : (-1));
			if (num == -1 && (bool)fracturedComponent.SourceObject.GetComponent<Renderer>() && fracturedComponent.SourceObject.GetComponent<Renderer>().sharedMaterial == fracturedComponent.SplitMaterial)
			{
				num = 0;
			}
			SplitOptions @default = SplitOptions.Default;
			@default.bVerticesAreLocal = false;
			Vector3 position = fracturedComponent.transform.position;
			Quaternion rotation = fracturedComponent.transform.rotation;
			Vector3 position2 = fracturedComponent.SourceObject.transform.position;
			fracturedComponent.SourceObject.transform.position = Vector3.zero;
			fracturedComponent.transform.position = fracturedComponent.SourceObject.transform.position;
			fracturedComponent.transform.rotation = fracturedComponent.SourceObject.transform.rotation;
			Material[] aMaterials = (fracturedComponent.SourceObject.GetComponent<Renderer>() ? fracturedComponent.SourceObject.GetComponent<Renderer>().sharedMaterials : null);
			MeshData meshData = new MeshData(component.transform, component.sharedMesh, aMaterials, fracturedComponent.SourceObject.transform.localToWorldMatrix, true, num, true);
			Queue<MeshData> queue = new Queue<MeshData>();
			Queue<int> queue2 = new Queue<int>();
			if (fracturedComponent.GenerateIslands)
			{
				CombinedMesh component3 = fracturedComponent.SourceObject.GetComponent<CombinedMesh>();
				if (component3 != null)
				{
					component3.TransformObjInfoMeshVectorsToLocal(fracturedComponent.SourceObject.transform.transform);
					List<MeshData> list = new List<MeshData>();
					for (int i = 0; i < component3.GetObjectCount(); i++)
					{
						CombinedMesh.ObjectInfo objectInfo = component3.GetObjectInfo(i);
						foreach (MeshData item in ComputeMeshDataIslands(new MeshData(component.transform, objectInfo.mesh, objectInfo.aMaterials, fracturedComponent.transform.localToWorldMatrix * objectInfo.mtxLocal, true, -1, true), false, fracturedComponent, progress))
						{
							queue.Enqueue(item);
							queue2.Enqueue(0);
							list.Add(item);
						}
					}
					if (fracturedComponent.GenerateChunkConnectionInfo)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (progress != null)
							{
								progress("Fracturing", "Processing combined object chunks connectivity...", (float)j / (float)list.Count);
								if (IsFracturingCancelled())
								{
									return false;
								}
							}
							for (int k = 0; k < list.Count; k++)
							{
								if (j != k)
								{
									ComputeIslandsMeshDataConnectivity(fracturedComponent, false, list[j], list[k]);
								}
							}
						}
					}
				}
				else
				{
					foreach (MeshData item2 in ComputeMeshDataIslands(meshData, false, fracturedComponent, progress))
					{
						queue.Enqueue(item2);
						queue2.Enqueue(0);
					}
				}
			}
			else
			{
				queue.Enqueue(meshData);
				queue2.Enqueue(0);
			}
			s_FracturingStats.nChunkCount = 1;
			bool generateIslands = fracturedComponent.GenerateIslands;
			while (queue.Count < s_FracturingStats.nTotalChunks && !IsFracturingCancelled())
			{
				MeshData meshDataIn = queue.Dequeue();
				int num2 = queue2.Dequeue();
				if (progress != null)
				{
					progress("Fracturing", string.Format("Computing chunk {0}/{1} (Depth {2})", s_FracturingStats.nChunkCount + 1, s_FracturingStats.nTotalChunks, generateIslands ? ": size ordered traversal" : num2.ToString()), Mathf.Clamp01((float)s_FracturingStats.nChunkCount / (float)s_FracturingStats.nTotalChunks));
				}
				int nSplitAxisPerformed = -1;
				Matrix4x4 randomPlaneSplitMatrix = GetRandomPlaneSplitMatrix(meshDataIn, fracturedComponent, out nSplitAxisPerformed);
				List<MeshData> listMeshDatasPosOut;
				List<MeshData> listMeshDatasNegOut;
				if (SplitMeshUsingPlane(meshDataIn, fracturedComponent, @default, randomPlaneSplitMatrix.MultiplyVector(Vector3.up), randomPlaneSplitMatrix.MultiplyVector(Vector3.right), randomPlaneSplitMatrix.MultiplyPoint3x4(Vector3.zero), out listMeshDatasPosOut, out listMeshDatasNegOut, progress))
				{
					s_FracturingStats.nSplitCount++;
					foreach (MeshData item3 in listMeshDatasPosOut)
					{
						queue.Enqueue(item3);
					}
					queue2.Enqueue(num2 + 1);
					foreach (MeshData item4 in listMeshDatasNegOut)
					{
						queue.Enqueue(item4);
					}
					queue2.Enqueue(num2 + 1);
				}
				if (generateIslands)
				{
					List<MeshData> list2 = new List<MeshData>();
					while (queue.Count > 0)
					{
						list2.Add(queue.Dequeue());
					}
					list2.Sort(new MeshData.DecreasingSizeComparer(nSplitAxisPerformed));
					foreach (MeshData item5 in list2)
					{
						queue.Enqueue(item5);
					}
				}
				s_FracturingStats.nChunkCount = queue.Count;
			}
			MeshData[] array = queue.ToArray();
			if (!IsFracturingCancelled())
			{
				for (int l = 0; l < array.Length; l++)
				{
					GameObject gameObject = CreateNewSplitGameObject(fracturedComponent.SourceObject, fracturedComponent, fracturedComponent.SourceObject.name + (l + 1), true, array[l]);
					gameObject.AddComponent<Rigidbody>();
					gameObject.GetComponent<Rigidbody>().isKinematic = true;
					listGameObjectsOut.Add(gameObject);
				}
				if (fracturedComponent.GenerateChunkConnectionInfo)
				{
					ComputeChunkConnections(fracturedComponent, listGameObjectsOut, new List<MeshData>(array), progress);
				}
				fracturedComponent.ComputeChunksRelativeVolume();
				fracturedComponent.ComputeChunksMass(fracturedComponent.TotalMass);
				fracturedComponent.ComputeSupportPlaneIntersections();
			}
			if (fracturedComponent.AlwaysComputeColliders)
			{
				ComputeChunkColliders(fracturedComponent, progress);
			}
			bool num3 = IsFracturingCancelled();
			fracturedComponent.SourceObject.transform.position = position2;
			if (bPositionOnSourceAndHideOriginal)
			{
				fracturedComponent.gameObject.transform.position = fracturedComponent.SourceObject.transform.position;
				fracturedComponent.gameObject.transform.rotation = fracturedComponent.SourceObject.transform.rotation;
				fracturedComponent.SourceObject.SetActive(false);
			}
			else
			{
				fracturedComponent.transform.position = position;
				fracturedComponent.transform.rotation = rotation;
			}
			return !num3;
		}

		public static List<MeshData> ComputeMeshDataIslands(MeshData meshDataIn, bool bVerticesAreLocal, FracturedObject fracturedComponent, ProgressDelegate progress)
		{
			MeshFaceConnectivity meshFaceConnectivity = new MeshFaceConnectivity();
			MeshDataConnectivity meshDataConnectivity = new MeshDataConnectivity();
			List<int>[] array = new List<int>[meshDataIn.aaIndices.Length];
			List<VertexData> list = new List<VertexData>();
			for (int i = 0; i < meshDataIn.nSubMeshCount; i++)
			{
				array[i] = new List<int>();
				for (int j = 0; j < meshDataIn.aaIndices[i].Length / 3; j++)
				{
					int num = meshDataIn.aaIndices[i][j * 3];
					int num2 = meshDataIn.aaIndices[i][j * 3 + 1];
					int num3 = meshDataIn.aaIndices[i][j * 3 + 2];
					int nVertexHash = meshDataIn.aVertexData[num].nVertexHash;
					int nVertexHash2 = meshDataIn.aVertexData[num2].nVertexHash;
					int nVertexHash3 = meshDataIn.aVertexData[num3].nVertexHash;
					Vector3 v3Vertex = meshDataIn.aVertexData[num].v3Vertex;
					Vector3 v3Vertex2 = meshDataIn.aVertexData[num2].v3Vertex;
					Vector3 v3Vertex3 = meshDataIn.aVertexData[num3].v3Vertex;
					array[i].Add(num);
					array[i].Add(num2);
					array[i].Add(num3);
					if (fracturedComponent.GenerateChunkConnectionInfo)
					{
						meshDataConnectivity.NotifyNewClippedFace(meshDataIn, i, j, i, j);
					}
					meshFaceConnectivity.AddEdge(i, v3Vertex, v3Vertex2, nVertexHash, nVertexHash2, num, num2);
					meshFaceConnectivity.AddEdge(i, v3Vertex2, v3Vertex3, nVertexHash2, nVertexHash3, num2, num3);
					meshFaceConnectivity.AddEdge(i, v3Vertex3, v3Vertex, nVertexHash3, nVertexHash, num3, num);
				}
			}
			list.AddRange(meshDataIn.aVertexData);
			meshDataIn.meshDataConnectivity = meshDataConnectivity;
			List<MeshData> list2 = MeshData.PostProcessConnectivity(meshDataIn, meshFaceConnectivity, meshDataConnectivity, array, list, meshDataIn.nSplitCloseSubMesh, meshDataIn.nCurrentVertexHash, false);
			if (fracturedComponent.GenerateChunkConnectionInfo)
			{
				for (int k = 0; k < list2.Count; k++)
				{
					if (progress != null)
					{
						progress("Fracturing", "Processing initial island connectivity...", (float)k / (float)list2.Count);
						if (IsFracturingCancelled())
						{
							return new List<MeshData>();
						}
					}
					for (int l = 0; l < list2.Count; l++)
					{
						if (k != l)
						{
							ComputeIslandsMeshDataConnectivity(fracturedComponent, bVerticesAreLocal, list2[k], list2[l]);
						}
					}
				}
			}
			return list2;
		}

		public static void ComputeChunkColliders(FracturedObject fracturedComponent, ProgressDelegate progress)
		{
			int num = 0;
			int num2 = 0;
			s_FracturingStats = new FracturingStats();
			foreach (FracturedChunk listFracturedChunk in fracturedComponent.ListFracturedChunks)
			{
				if (IsFracturingCancelled())
				{
					break;
				}
				if (progress != null)
				{
					progress("Computing colliders", string.Format("Collider {0}/{1}", num + 1, fracturedComponent.ListFracturedChunks.Count), (float)num / (float)fracturedComponent.ListFracturedChunks.Count);
				}
				if (listFracturedChunk == null)
				{
					continue;
				}
				if (listFracturedChunk.GetComponent<Collider>() != null)
				{
					MeshCollider component = listFracturedChunk.GetComponent<MeshCollider>();
					if ((bool)component)
					{
						component.sharedMesh = null;
					}
					if ((bool)listFracturedChunk.GetComponent<Collider>())
					{
						UnityEngine.Object.DestroyImmediate(listFracturedChunk.GetComponent<Collider>());
					}
				}
				if (listFracturedChunk.GetComponent<Rigidbody>() != null)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.GetComponent<Rigidbody>());
				}
				while (listFracturedChunk.transform.childCount > 0)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.transform.GetChild(0).gameObject);
				}
				listFracturedChunk.HasConcaveCollider = false;
				bool flag = false;
				RemoveEmptySubmeshes(listFracturedChunk);
				if (listFracturedChunk.Volume > fracturedComponent.MinColliderVolumeForBox)
				{
					if (fracturedComponent.IntegrateWithConcaveCollider)
					{
						int num3 = ConcaveColliderInterface.ComputeHull(listFracturedChunk.gameObject, fracturedComponent);
						if (num3 > 0)
						{
							listFracturedChunk.HasConcaveCollider = true;
							flag = true;
							num2 += num3;
						}
					}
					else
					{
						MeshFilter component2 = listFracturedChunk.GetComponent<MeshFilter>();
						if (component2 != null && component2.sharedMesh != null && component2.sharedMesh.vertexCount >= 3)
						{
							listFracturedChunk.HasConcaveCollider = false;
							MeshCollider meshCollider = listFracturedChunk.gameObject.AddComponent<MeshCollider>();
							meshCollider.convex = true;
							flag = true;
							if ((bool)meshCollider.sharedMesh)
							{
								num2 += meshCollider.sharedMesh.triangles.Length / 3;
							}
							meshCollider.isTrigger = fracturedComponent.ChunkColliderType == FracturedObject.ColliderType.Trigger;
						}
					}
				}
				if (!flag)
				{
					BoxCollider boxCollider = listFracturedChunk.gameObject.AddComponent<BoxCollider>();
					num2 += 12;
					boxCollider.isTrigger = fracturedComponent.ChunkColliderType == FracturedObject.ColliderType.Trigger;
				}
				if ((bool)listFracturedChunk.GetComponent<Collider>())
				{
					listFracturedChunk.GetComponent<Collider>().material = fracturedComponent.ChunkPhysicMaterial;
				}
				listFracturedChunk.gameObject.AddComponent<Rigidbody>();
				listFracturedChunk.GetComponent<Rigidbody>().isKinematic = true;
				num++;
			}
			if (!IsFracturingCancelled())
			{
				fracturedComponent.ComputeChunksMass(fracturedComponent.TotalMass);
			}
			if (fracturedComponent.Verbose && fracturedComponent.ListFracturedChunks.Count > 0)
			{
				Debug.Log("Total collider triangles: " + num2 + ". Average = " + num2 / fracturedComponent.ListFracturedChunks.Count);
			}
		}

		public static void DeleteChunkColliders(FracturedObject fracturedComponent)
		{
			foreach (FracturedChunk listFracturedChunk in fracturedComponent.ListFracturedChunks)
			{
				while (listFracturedChunk.transform.childCount > 0)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.transform.GetChild(0).gameObject);
				}
				if (listFracturedChunk.GetComponent<Collider>() != null)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.GetComponent<Collider>());
				}
				if (listFracturedChunk.GetComponent<Rigidbody>() != null)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.GetComponent<Rigidbody>());
				}
			}
		}

		private static Matrix4x4 GetRandomPlaneSplitMatrix(MeshData meshDataIn, FracturedObject fracturedComponent, out int nSplitAxisPerformed)
		{
			Vector3 zero = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 vector = meshDataIn.v3Min - meshDataIn.v3Position;
			Vector3 vector2 = meshDataIn.v3Max - meshDataIn.v3Position;
			if (!fracturedComponent.SplitsWorldSpace)
			{
				vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
				Matrix4x4 inverse = Matrix4x4.TRS(meshDataIn.v3Position, meshDataIn.qRotation, meshDataIn.v3Scale).inverse;
				for (int i = 0; i < meshDataIn.aVertexData.Length; i++)
				{
					Vector3 vector3 = inverse.MultiplyPoint3x4(meshDataIn.aVertexData[i].v3Vertex);
					if (vector3.x < vector.x)
					{
						vector.x = vector3.x;
					}
					if (vector3.y < vector.y)
					{
						vector.y = vector3.y;
					}
					if (vector3.z < vector.z)
					{
						vector.z = vector3.z;
					}
					if (vector3.x > vector2.x)
					{
						vector2.x = vector3.x;
					}
					if (vector3.y > vector2.y)
					{
						vector2.y = vector3.y;
					}
					if (vector3.z > vector2.z)
					{
						vector2.z = vector3.z;
					}
				}
			}
			int num = -1;
			if (fracturedComponent.SplitRegularly)
			{
				float num2 = vector2.x - vector.x;
				float num3 = vector2.y - vector.y;
				float num4 = vector2.z - vector.z;
				num = ((!(num2 >= num3) || !(num2 >= num4)) ? ((num3 >= num2 && num3 >= num4) ? 1 : 2) : 0);
			}
			else
			{
				for (int j = 0; j < 3; j++)
				{
					float value = UnityEngine.Random.value;
					bool num5 = Mathf.Approximately(fracturedComponent.SplitXProbability, 0f);
					bool flag = Mathf.Approximately(fracturedComponent.SplitYProbability, 0f);
					bool flag2 = Mathf.Approximately(fracturedComponent.SplitZProbability, 0f);
					if (!num5 && (value <= fracturedComponent.SplitXProbability || (flag && flag2)))
					{
						num = 0;
					}
					if (!flag && num == -1)
					{
						float num6 = fracturedComponent.SplitXProbability + fracturedComponent.SplitYProbability;
						if (value <= num6 || flag2)
						{
							num = 1;
						}
					}
					if (num == -1)
					{
						num = 2;
					}
				}
			}
			nSplitAxisPerformed = num;
			float num7 = 0f;
			float num8 = 45f;
			switch (num)
			{
			case 0:
				num7 = (vector2.x - vector.x) * 0.5f;
				quaternion = Quaternion.LookRotation(-Vector3.up, Vector3.right) * Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitXVariation, UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitXVariation));
				break;
			case 1:
				num7 = (vector2.y - vector.y) * 0.5f;
				quaternion = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitYVariation, 0f, UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitYVariation));
				break;
			case 2:
				num7 = (vector2.z - vector.z) * 0.5f;
				quaternion = Quaternion.LookRotation(-Vector3.up, Vector3.forward) * Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitZVariation, UnityEngine.Random.Range(0f - num8, num8) * fracturedComponent.SplitZVariation, 0f));
				break;
			}
			num7 = num7 * fracturedComponent.SplitSizeVariation * 0.8f;
			zero = new Vector3(UnityEngine.Random.Range(-1f, 1f) * num7, UnityEngine.Random.Range(-1f, 1f) * num7, UnityEngine.Random.Range(-1f, 1f) * num7);
			if (!fracturedComponent.SplitsWorldSpace)
			{
				return Matrix4x4.TRS(zero + meshDataIn.v3Position, fracturedComponent.SourceObject.transform.rotation * quaternion, Vector3.one);
			}
			return Matrix4x4.TRS(zero + meshDataIn.v3Position, quaternion, Vector3.one);
		}

		private static GameObject CreateNewSplitGameObject(GameObject gameObjectIn, FracturedObject fracturedComponent, string strName, bool bTransformVerticesBackToLocal, MeshData meshData)
		{
			GameObject gameObject = new GameObject(strName);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			FracturedChunk fracturedChunk = gameObject.AddComponent<FracturedChunk>();
			fracturedChunk.transform.parent = fracturedComponent.transform;
			if ((bool)fracturedComponent.SourceObject)
			{
				gameObject.layer = fracturedComponent.SourceObject.layer;
			}
			else
			{
				gameObject.layer = fracturedComponent.gameObject.layer;
			}
			fracturedComponent.ListFracturedChunks.Add(fracturedChunk);
			meshData.FillMeshFilter(meshFilter, bTransformVerticesBackToLocal);
			fracturedChunk.SplitSubMeshIndex = meshData.nSplitCloseSubMesh;
			fracturedChunk.OnCreateFromFracturedObject(fracturedComponent, meshData.nSplitCloseSubMesh);
			gameObject.AddComponent<MeshRenderer>();
			gameObject.GetComponent<Renderer>().shadowCastingMode = gameObjectIn.GetComponent<Renderer>().shadowCastingMode;
			gameObject.GetComponent<Renderer>().receiveShadows = gameObjectIn.GetComponent<Renderer>().receiveShadows;
			gameObject.GetComponent<Renderer>().enabled = false;
			Material[] array = new Material[meshData.nSubMeshCount];
			meshData.aMaterials.CopyTo(array, 0);
			if (meshData.aMaterials.Length < meshData.nSubMeshCount)
			{
				array[meshData.nSubMeshCount - 1] = fracturedComponent.SplitMaterial;
			}
			gameObject.GetComponent<Renderer>().sharedMaterials = array;
			gameObject.GetComponent<Renderer>().lightmapIndex = gameObjectIn.GetComponent<Renderer>().lightmapIndex;
			gameObject.GetComponent<Renderer>().lightmapScaleOffset = gameObjectIn.GetComponent<Renderer>().lightmapScaleOffset;
			gameObject.GetComponent<Renderer>().lightProbeUsage = gameObjectIn.GetComponent<Renderer>().lightProbeUsage;
			return gameObject;
		}

		private static int CreateMeshConnectivityVoronoiHash(int nCell1, int nCell2)
		{
			int num = Mathf.Max(nCell1, nCell2) + 256;
			int num2 = Mathf.Min(nCell1, nCell2) + 256;
			return (num << 16) | num2;
		}

		private static void ComputeChunkConnections(FracturedObject fracturedObject, List<GameObject> listGameObjects, List<MeshData> listMeshDatas, ProgressDelegate progress = null)
		{
			for (int i = 0; i < listGameObjects.Count; i++)
			{
				if (progress != null)
				{
					progress("Fracturing", "Computing connections...", (float)i / (float)listGameObjects.Count);
				}
				if (IsFracturingCancelled())
				{
					break;
				}
				FracturedChunk component = listGameObjects[i].GetComponent<FracturedChunk>();
				List<FracturedChunk.AdjacencyInfo> list = new List<FracturedChunk.AdjacencyInfo>();
				for (int j = 0; j < listGameObjects.Count; j++)
				{
					if (i != j)
					{
						FracturedChunk component2 = listGameObjects[j].GetComponent<FracturedChunk>();
						float fSharedArea = 0f;
						bool sharedFacesArea = listMeshDatas[i].GetSharedFacesArea(fracturedObject, listMeshDatas[j], out fSharedArea);
						bool flag = fSharedArea >= fracturedObject.ChunkConnectionMinArea;
						if (Mathf.Approximately(fracturedObject.ChunkConnectionMinArea, 0f) && sharedFacesArea)
						{
							flag = true;
						}
						if (flag && sharedFacesArea)
						{
							list.Add(new FracturedChunk.AdjacencyInfo(component2, fSharedArea));
						}
					}
				}
				component.ListAdjacentChunks = list;
			}
		}

		private static void RemoveEmptySubmeshes(FracturedChunk fracturedChunk)
		{
			MeshFilter component = fracturedChunk.GetComponent<MeshFilter>();
			if (component == null)
			{
				return;
			}
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh.subMeshCount < 2)
			{
				return;
			}
			MeshRenderer component2 = fracturedChunk.GetComponent<MeshRenderer>();
			List<Material> list = new List<Material>();
			List<int[]> list2 = new List<int[]>();
			int num = 0;
			for (int i = 0; i < sharedMesh.subMeshCount; i++)
			{
				int[] indices = sharedMesh.GetIndices(i);
				if (indices != null && indices.Length != 0)
				{
					list.Add(component2.sharedMaterials[i]);
					list2.Add(indices);
				}
				else if (i < fracturedChunk.SplitSubMeshIndex)
				{
					num--;
				}
			}
			sharedMesh.subMeshCount = list2.Count;
			for (int j = 0; j < list2.Count; j++)
			{
				sharedMesh.SetTriangles(list2[j], j);
				component2.sharedMaterials = list.ToArray();
			}
			fracturedChunk.SplitSubMeshIndex -= num;
			component.sharedMesh = sharedMesh;
		}

		public static bool SplitMeshUsingPlane(GameObject gameObjectIn, FracturedObject fracturedComponent, SplitOptions splitOptions, Transform transformPlaneSplit, out List<GameObject> listGameObjectsPosOut, out List<GameObject> listGameObjectsNegOut, ProgressDelegate progress = null)
		{
			listGameObjectsPosOut = new List<GameObject>();
			listGameObjectsNegOut = new List<GameObject>();
			MeshFilter component = gameObjectIn.GetComponent<MeshFilter>();
			if (component == null)
			{
				return false;
			}
			foreach (FracturedChunk listFracturedChunk in fracturedComponent.ListFracturedChunks)
			{
				if (listFracturedChunk != null)
				{
					UnityEngine.Object.DestroyImmediate(listFracturedChunk.gameObject);
				}
			}
			fracturedComponent.ListFracturedChunks.Clear();
			fracturedComponent.DecomposeRadius = (component.sharedMesh.bounds.max - component.sharedMesh.bounds.min).magnitude;
			UnityEngine.Random.InitState(fracturedComponent.RandomSeed);
			FracturedChunk component2 = gameObjectIn.GetComponent<FracturedChunk>();
			int num = ((component2 != null) ? component2.SplitSubMeshIndex : (-1));
			if (num == -1 && (bool)gameObjectIn.GetComponent<Renderer>() && gameObjectIn.GetComponent<Renderer>().sharedMaterial == fracturedComponent.SplitMaterial)
			{
				num = 0;
			}
			Material[] aMaterials = (fracturedComponent.gameObject.GetComponent<Renderer>() ? fracturedComponent.gameObject.GetComponent<Renderer>().sharedMaterials : null);
			List<MeshData> listMeshDatasPosOut;
			List<MeshData> listMeshDatasNegOut;
			if (!SplitMeshUsingPlane(new MeshData(component.transform, component.sharedMesh, aMaterials, component.transform.localToWorldMatrix, !splitOptions.bVerticesAreLocal, num, true), fracturedComponent, splitOptions, transformPlaneSplit.up, transformPlaneSplit.right, transformPlaneSplit.position, out listMeshDatasPosOut, out listMeshDatasNegOut, progress))
			{
				return false;
			}
			if (listMeshDatasPosOut.Count > 0)
			{
				for (int i = 0; i < listMeshDatasPosOut.Count; i++)
				{
					GameObject item = CreateNewSplitGameObject(gameObjectIn, fracturedComponent, gameObjectIn.name + "0" + ((listMeshDatasPosOut.Count > 1) ? ("(" + i + ")") : ""), !splitOptions.bVerticesAreLocal, listMeshDatasPosOut[i]);
					listGameObjectsPosOut.Add(item);
				}
			}
			if (listMeshDatasNegOut.Count > 0)
			{
				for (int j = 0; j < listMeshDatasNegOut.Count; j++)
				{
					GameObject item2 = CreateNewSplitGameObject(gameObjectIn, fracturedComponent, gameObjectIn.name + "1" + ((listMeshDatasNegOut.Count > 1) ? ("(" + j + ")") : ""), !splitOptions.bVerticesAreLocal, listMeshDatasNegOut[j]);
					listGameObjectsNegOut.Add(item2);
				}
			}
			return true;
		}

		private static bool SplitMeshUsingPlane(MeshData meshDataIn, FracturedObject fracturedComponent, SplitOptions splitOptions, Vector3 v3PlaneNormal, Vector3 v3PlaneRight, Vector3 v3PlanePoint, out List<MeshData> listMeshDatasPosOut, out List<MeshData> listMeshDatasNegOut, ProgressDelegate progress = null)
		{
			Plane planeSplit = new Plane(v3PlaneNormal, v3PlanePoint);
			listMeshDatasPosOut = new List<MeshData>();
			listMeshDatasNegOut = new List<MeshData>();
			bool num = meshDataIn.nSplitCloseSubMesh == -1;
			int num2 = meshDataIn.nSplitCloseSubMesh;
			int nCurrentVertexHash = meshDataIn.nCurrentVertexHash;
			List<VertexData> list = new List<VertexData>();
			List<VertexData> list2 = new List<VertexData>();
			List<int>[] array = new List<int>[meshDataIn.nSubMeshCount + ((meshDataIn.nSplitCloseSubMesh == -1) ? 1 : 0)];
			List<int>[] array2 = new List<int>[meshDataIn.nSubMeshCount + ((meshDataIn.nSplitCloseSubMesh == -1) ? 1 : 0)];
			MeshFaceConnectivity meshFaceConnectivity = new MeshFaceConnectivity();
			MeshFaceConnectivity meshFaceConnectivity2 = new MeshFaceConnectivity();
			MeshDataConnectivity meshDataConnectivity = new MeshDataConnectivity();
			MeshDataConnectivity meshDataConnectivity2 = new MeshDataConnectivity();
			list.Capacity = meshDataIn.aVertexData.Length / 2;
			list2.Capacity = meshDataIn.aVertexData.Length / 2;
			if (num)
			{
				num2 = meshDataIn.nSubMeshCount;
				array[num2] = new List<int>();
				array2[num2] = new List<int>();
			}
			Dictionary<EdgeKeyByHash, int> dictionary = new Dictionary<EdgeKeyByHash, int>(new EdgeKeyByHash.EqualityComparer());
			Dictionary<EdgeKeyByHash, CapEdge> dictionary2 = new Dictionary<EdgeKeyByHash, CapEdge>(new EdgeKeyByHash.EqualityComparer());
			Dictionary<EdgeKeyByIndex, ClippedEdge> dictionary3 = new Dictionary<EdgeKeyByIndex, ClippedEdge>(new EdgeKeyByIndex.EqualityComparer());
			Dictionary<EdgeKeyByIndex, ClippedEdge> dictionary4 = new Dictionary<EdgeKeyByIndex, ClippedEdge>(new EdgeKeyByIndex.EqualityComparer());
			int num3 = 0;
			int num4 = 0;
			Dictionary<int, int> dictionary5 = new Dictionary<int, int>();
			Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
			for (int i = 0; i < meshDataIn.nSubMeshCount; i++)
			{
				array[i] = new List<int>();
				array2[i] = new List<int>();
				List<int> list3 = array[i];
				List<int> list4 = array2[i];
				array[i].Capacity = meshDataIn.aaIndices[i].Length / 2;
				array2[i].Capacity = meshDataIn.aaIndices[i].Length / 2;
				List<VertexData> list5 = list;
				List<int> list6 = list3;
				MeshFaceConnectivity meshFaceConnectivity3 = meshFaceConnectivity;
				MeshDataConnectivity meshDataConnectivity3 = meshDataConnectivity;
				Dictionary<EdgeKeyByIndex, ClippedEdge> dictionary7 = dictionary3;
				Dictionary<int, int> dictionary8 = dictionary5;
				for (int j = 0; j < meshDataIn.aaIndices[i].Length / 3; j++)
				{
					list5 = list;
					list6 = list3;
					meshFaceConnectivity3 = meshFaceConnectivity;
					meshDataConnectivity3 = meshDataConnectivity;
					dictionary7 = dictionary3;
					dictionary8 = dictionary5;
					int num5 = meshDataIn.aaIndices[i][j * 3];
					int num6 = meshDataIn.aaIndices[i][j * 3 + 1];
					int num7 = meshDataIn.aaIndices[i][j * 3 + 2];
					int nVertexHash = meshDataIn.aVertexData[num5].nVertexHash;
					int nVertexHash2 = meshDataIn.aVertexData[num6].nVertexHash;
					int nVertexHash3 = meshDataIn.aVertexData[num7].nVertexHash;
					Vector3 v3Vertex = meshDataIn.aVertexData[num5].v3Vertex;
					Vector3 v3Vertex2 = meshDataIn.aVertexData[num6].v3Vertex;
					Vector3 v3Vertex3 = meshDataIn.aVertexData[num7].v3Vertex;
					float num8 = v3Vertex.x * planeSplit.normal.x + v3Vertex.y * planeSplit.normal.y + v3Vertex.z * planeSplit.normal.z + planeSplit.distance;
					float num9 = v3Vertex2.x * planeSplit.normal.x + v3Vertex2.y * planeSplit.normal.y + v3Vertex2.z * planeSplit.normal.z + planeSplit.distance;
					float num10 = v3Vertex3.x * planeSplit.normal.x + v3Vertex3.y * planeSplit.normal.y + v3Vertex3.z * planeSplit.normal.z + planeSplit.distance;
					bool flag = false;
					int num11 = 0;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					float num12 = 0f;
					if (Mathf.Abs(num8) < Parameters.EPSILONDISTANCEPLANE)
					{
						flag2 = true;
						num11++;
					}
					if (Mathf.Abs(num9) < Parameters.EPSILONDISTANCEPLANE)
					{
						flag3 = true;
						num11++;
					}
					if (Mathf.Abs(num10) < Parameters.EPSILONDISTANCEPLANE)
					{
						flag4 = true;
						num11++;
					}
					if (Mathf.Abs(num8) > Mathf.Abs(num12))
					{
						num12 = num8;
					}
					if (Mathf.Abs(num9) > Mathf.Abs(num12))
					{
						num12 = num9;
					}
					if (Mathf.Abs(num10) > Mathf.Abs(num12))
					{
						num12 = num10;
					}
					if (num11 == 1)
					{
						if (flag2 && num9 * num10 > 0f)
						{
							flag = true;
						}
						if (flag3 && num8 * num10 > 0f)
						{
							flag = true;
						}
						if (flag4 && num8 * num9 > 0f)
						{
							flag = true;
						}
					}
					else if (num11 > 1)
					{
						flag = true;
						if (num11 == 3)
						{
							continue;
						}
					}
					if ((num8 * num9 > 0f && num9 * num10 > 0f) || flag)
					{
						if (num12 < 0f)
						{
							list5 = list2;
							list6 = list4;
							meshFaceConnectivity3 = meshFaceConnectivity2;
							meshDataConnectivity3 = meshDataConnectivity2;
							dictionary7 = dictionary4;
							dictionary8 = dictionary6;
						}
						int num13 = -1;
						int num14 = -1;
						int num15 = -1;
						if (dictionary8.ContainsKey(num5))
						{
							num13 = dictionary8[num5];
						}
						if (num13 == -1)
						{
							num13 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num5].Copy());
							dictionary8[num5] = num13;
						}
						if (dictionary8.ContainsKey(num6))
						{
							num14 = dictionary8[num6];
						}
						if (num14 == -1)
						{
							num14 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num6].Copy());
							dictionary8[num6] = num14;
						}
						if (dictionary8.ContainsKey(num7))
						{
							num15 = dictionary8[num7];
						}
						if (num15 == -1)
						{
							num15 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num7].Copy());
							dictionary8[num7] = num15;
						}
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num13);
						list6.Add(num14);
						list6.Add(num15);
						if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
						{
							meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex2, nVertexHash, nVertexHash2, num13, num14);
							meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex3, nVertexHash2, nVertexHash3, num14, num15);
							meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex, nVertexHash3, nVertexHash, num15, num13);
						}
						if (num11 != 2)
						{
							continue;
						}
						if (num12 > 0f)
						{
							if (flag2 && flag3 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash, nVertexHash2, v3Vertex, v3Vertex2);
							}
							if (flag3 && flag4 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash2, nVertexHash3, v3Vertex2, v3Vertex3);
							}
							if (flag4 && flag2 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash3, nVertexHash, v3Vertex3, v3Vertex);
							}
						}
						else
						{
							if (flag2 && flag3 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash2, nVertexHash, v3Vertex2, v3Vertex);
							}
							if (flag3 && flag4 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash3, nVertexHash2, v3Vertex3, v3Vertex2);
							}
							if (flag4 && flag2 && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash, nVertexHash3, v3Vertex, v3Vertex3);
							}
						}
					}
					else if (num11 == 1)
					{
						int num16 = -1;
						int num17 = -1;
						int num18 = -1;
						int num19 = -1;
						int num20 = -1;
						bool flag5 = false;
						if (flag2)
						{
							if (num9 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							EdgeKeyByIndex key = new EdgeKeyByIndex(num6, num7);
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num17 = dictionary7[key].GetFirstIndex(num6);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num6))
								{
									num17 = dictionary8[num6];
								}
							}
							EdgeKeyByHash key2 = new EdgeKeyByHash(nVertexHash2, nVertexHash3);
							if (dictionary.ContainsKey(key2))
							{
								num20 = dictionary[key2];
							}
							else
							{
								num20 = nCurrentVertexHash++;
								dictionary.Add(key2, num20);
							}
							VertexData clippedVertexDataOut = new VertexData(num20);
							if (!flag5 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num6, num7, v3Vertex2, v3Vertex3, planeSplit, ref clippedVertexDataOut))
							{
								return false;
							}
							if (num16 == -1 && dictionary8.ContainsKey(num5))
							{
								num16 = dictionary8[num5];
							}
							if (num16 == -1)
							{
								num16 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num5].Copy());
								dictionary8[num5] = num16;
							}
							if (num17 == -1)
							{
								num17 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num6].Copy());
								dictionary8[num6] = num17;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num16);
							list6.Add(num17);
							list6.Add(num19);
							Vector3 v3Vertex4 = list5[num19].v3Vertex;
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex2, nVertexHash, nVertexHash2, num16, num17);
								meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex4, nVertexHash2, num20, num17, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex4, v3Vertex, num20, nVertexHash, num19, num16);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, num20, nVertexHash, list5[num19].v3Vertex, list5[num16].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num6, num7, num17, num18, num19));
							}
							if (num10 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							else
							{
								list5 = list;
								list6 = list3;
								meshFaceConnectivity3 = meshFaceConnectivity;
								meshDataConnectivity3 = meshDataConnectivity;
								dictionary7 = dictionary3;
								dictionary8 = dictionary5;
							}
							num16 = -1;
							num17 = -1;
							num18 = -1;
							num19 = -1;
							flag5 = false;
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num18 = dictionary7[key].GetSecondIndex(num7);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num7))
								{
									num18 = dictionary8[num7];
								}
							}
							if (num16 == -1 && dictionary8.ContainsKey(num5))
							{
								num16 = dictionary8[num5];
							}
							if (num16 == -1)
							{
								num16 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num5].Copy());
								dictionary8[num5] = num16;
							}
							if (num18 == -1)
							{
								num18 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num7].Copy());
								dictionary8[num7] = num18;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num16);
							list6.Add(num19);
							list6.Add(num18);
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex4, nVertexHash, num20, num16, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex4, v3Vertex3, num20, nVertexHash3, num19, num18);
								meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex, nVertexHash3, nVertexHash, num18, num16);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash, num20, list5[num16].v3Vertex, list5[num19].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num6, num7, num17, num18, num19));
							}
						}
						else if (flag3)
						{
							if (num10 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							EdgeKeyByIndex key = new EdgeKeyByIndex(num7, num5);
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num18 = dictionary7[key].GetFirstIndex(num7);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num7))
								{
									num18 = dictionary8[num7];
								}
							}
							EdgeKeyByHash key3 = new EdgeKeyByHash(nVertexHash3, nVertexHash);
							if (dictionary.ContainsKey(key3))
							{
								num20 = dictionary[key3];
							}
							else
							{
								num20 = nCurrentVertexHash++;
								dictionary.Add(key3, num20);
							}
							VertexData clippedVertexDataOut2 = new VertexData(num20);
							if (!flag5 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num7, num5, v3Vertex3, v3Vertex, planeSplit, ref clippedVertexDataOut2))
							{
								return false;
							}
							if (num17 == -1 && dictionary8.ContainsKey(num6))
							{
								num17 = dictionary8[num6];
							}
							if (num17 == -1)
							{
								num17 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num6].Copy());
								dictionary8[num6] = num17;
							}
							if (num18 == -1)
							{
								num18 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num7].Copy());
								dictionary8[num7] = num18;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut2);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num17);
							list6.Add(num18);
							list6.Add(num19);
							Vector3 v3Vertex5 = list5[num19].v3Vertex;
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex3, nVertexHash2, nVertexHash3, num17, num18);
								meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex5, nVertexHash3, num20, num18, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex5, v3Vertex2, num20, nVertexHash2, num19, num17);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, num20, nVertexHash2, list5[num19].v3Vertex, list5[num17].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num7, num5, num18, num16, num19));
							}
							if (num8 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							else
							{
								list5 = list;
								list6 = list3;
								meshFaceConnectivity3 = meshFaceConnectivity;
								meshDataConnectivity3 = meshDataConnectivity;
								dictionary7 = dictionary3;
								dictionary8 = dictionary5;
							}
							num16 = -1;
							num17 = -1;
							num19 = -1;
							flag5 = false;
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num16 = dictionary7[key].GetSecondIndex(num5);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num5))
								{
									num16 = dictionary8[num5];
								}
							}
							if (num16 == -1)
							{
								num16 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num5].Copy());
								dictionary8[num5] = num16;
							}
							if (num17 == -1 && dictionary8.ContainsKey(num6))
							{
								num17 = dictionary8[num6];
							}
							if (num17 == -1)
							{
								num17 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num6].Copy());
								dictionary8[num6] = num17;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut2);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num17);
							list6.Add(num19);
							list6.Add(num16);
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex5, nVertexHash2, num20, num17, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex5, v3Vertex, num20, nVertexHash, num19, num16);
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex2, nVertexHash, nVertexHash2, num16, num17);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash2, num20, list5[num17].v3Vertex, list5[num19].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num7, num5, num18, num16, num19));
							}
						}
						else
						{
							if (!flag4)
							{
								continue;
							}
							if (num8 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							EdgeKeyByIndex key = new EdgeKeyByIndex(num5, num6);
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num16 = dictionary7[key].GetFirstIndex(num5);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num5))
								{
									num16 = dictionary8[num5];
								}
							}
							EdgeKeyByHash key4 = new EdgeKeyByHash(nVertexHash, nVertexHash2);
							if (dictionary.ContainsKey(key4))
							{
								num20 = dictionary[key4];
							}
							else
							{
								num20 = nCurrentVertexHash++;
								dictionary.Add(key4, num20);
							}
							VertexData clippedVertexDataOut3 = new VertexData(num20);
							if (!flag5 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num5, num6, v3Vertex, v3Vertex2, planeSplit, ref clippedVertexDataOut3))
							{
								return false;
							}
							if (num16 == -1)
							{
								num16 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num5].Copy());
								dictionary8[num5] = num16;
							}
							if (num18 == -1 && dictionary8.ContainsKey(num7))
							{
								num18 = dictionary8[num7];
							}
							if (num18 == -1)
							{
								num18 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num7].Copy());
								dictionary8[num7] = num18;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut3);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num16);
							list6.Add(num19);
							list6.Add(num18);
							Vector3 v3Vertex6 = list5[num19].v3Vertex;
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex6, nVertexHash, num20, num16, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex6, v3Vertex3, num20, nVertexHash3, num19, num18);
								meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex, nVertexHash3, nVertexHash, num18, num16);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, num20, nVertexHash3, list5[num19].v3Vertex, list5[num18].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num5, num6, num16, num17, num19));
							}
							if (num9 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							else
							{
								list5 = list;
								list6 = list3;
								meshFaceConnectivity3 = meshFaceConnectivity;
								meshDataConnectivity3 = meshDataConnectivity;
								dictionary7 = dictionary3;
								dictionary8 = dictionary5;
							}
							num17 = -1;
							num18 = -1;
							num19 = -1;
							flag5 = false;
							if (dictionary7.ContainsKey(key))
							{
								num3++;
								flag5 = true;
								num17 = dictionary7[key].GetSecondIndex(num6);
								num19 = dictionary7[key].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num6))
								{
									num17 = dictionary8[num6];
								}
							}
							if (num17 == -1)
							{
								num17 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num6].Copy());
								dictionary8[num6] = num17;
							}
							if (num18 == -1 && dictionary8.ContainsKey(num7))
							{
								num18 = dictionary8[num7];
							}
							if (num18 == -1)
							{
								num18 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num7].Copy());
								dictionary8[num7] = num18;
							}
							if (num19 == -1)
							{
								num19 = list5.Count;
								list5.Add(clippedVertexDataOut3);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num17);
							list6.Add(num18);
							list6.Add(num19);
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex3, nVertexHash2, nVertexHash3, num17, num18);
								meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex6, nVertexHash3, num20, num18, num19);
								meshFaceConnectivity3.AddEdge(i, v3Vertex6, v3Vertex2, num20, nVertexHash2, num19, num17);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, nVertexHash3, num20, list5[num18].v3Vertex, list5[num19].v3Vertex);
							}
							if (!flag5)
							{
								dictionary7.Add(key, new ClippedEdge(num5, num6, num16, num17, num19));
							}
						}
					}
					else if (num8 * num9 < 0f)
					{
						if (num9 * num10 < 0f)
						{
							if (num8 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							int num21 = -1;
							int num22 = -1;
							int num23 = -1;
							int num24 = -1;
							int num25 = -1;
							int num26 = -1;
							int num27 = -1;
							bool flag6 = false;
							bool flag7 = false;
							EdgeKeyByIndex key5 = new EdgeKeyByIndex(num5, num6);
							EdgeKeyByIndex key6 = new EdgeKeyByIndex(num6, num7);
							if (dictionary7.ContainsKey(key5))
							{
								num3++;
								flag6 = true;
								num21 = dictionary7[key5].GetFirstIndex(num5);
								num24 = dictionary7[key5].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num5))
								{
									num21 = dictionary8[num5];
								}
							}
							if (dictionary7.ContainsKey(key6))
							{
								num3++;
								flag7 = true;
								num23 = dictionary7[key6].GetSecondIndex(num7);
								num25 = dictionary7[key6].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num7))
								{
									num23 = dictionary8[num7];
								}
							}
							EdgeKeyByHash key7 = new EdgeKeyByHash(nVertexHash, nVertexHash2);
							if (dictionary.ContainsKey(key7))
							{
								num26 = dictionary[key7];
							}
							else
							{
								num26 = nCurrentVertexHash++;
								dictionary.Add(key7, num26);
							}
							key7 = new EdgeKeyByHash(nVertexHash2, nVertexHash3);
							if (dictionary.ContainsKey(key7))
							{
								num27 = dictionary[key7];
							}
							else
							{
								num27 = nCurrentVertexHash++;
								dictionary.Add(key7, num27);
							}
							VertexData clippedVertexDataOut4 = new VertexData(num26);
							VertexData clippedVertexDataOut5 = new VertexData(num27);
							if (!flag6 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num5, num6, v3Vertex, v3Vertex2, planeSplit, ref clippedVertexDataOut4))
							{
								return false;
							}
							if (!flag7 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num6, num7, v3Vertex2, v3Vertex3, planeSplit, ref clippedVertexDataOut5))
							{
								return false;
							}
							if (num21 == -1)
							{
								num21 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num5].Copy());
								dictionary8[num5] = num21;
							}
							if (num23 == -1)
							{
								num23 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num7].Copy());
								dictionary8[num7] = num23;
							}
							if (num24 == -1)
							{
								num24 = list5.Count;
								list5.Add(clippedVertexDataOut4);
							}
							if (num25 == -1)
							{
								num25 = list5.Count;
								list5.Add(clippedVertexDataOut5);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num21);
							list6.Add(num24);
							list6.Add(num25);
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num21);
							list6.Add(num25);
							list6.Add(num23);
							Vector3 v3Vertex7 = list5[num24].v3Vertex;
							Vector3 v3Vertex8 = list5[num25].v3Vertex;
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex7, nVertexHash, num26, num21, num24);
								meshFaceConnectivity3.AddEdge(i, v3Vertex7, v3Vertex8, num26, num27, num24, num25);
								meshFaceConnectivity3.AddEdge(i, v3Vertex8, v3Vertex, num27, nVertexHash, num25, num21);
								meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex8, nVertexHash, num27, num21, num25);
								meshFaceConnectivity3.AddEdge(i, v3Vertex8, v3Vertex3, num27, nVertexHash3, num25, num23);
								meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex, nVertexHash3, nVertexHash, num23, num21);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, num26, num27, list5[num24].v3Vertex, list5[num25].v3Vertex);
							}
							if (!dictionary7.ContainsKey(key5))
							{
								dictionary7.Add(key5, new ClippedEdge(num5, num6, num21, num22, num24));
							}
							if (!dictionary7.ContainsKey(key6))
							{
								dictionary7.Add(key6, new ClippedEdge(num6, num7, num22, num23, num25));
							}
							if (num9 < 0f)
							{
								list5 = list2;
								list6 = list4;
								meshFaceConnectivity3 = meshFaceConnectivity2;
								meshDataConnectivity3 = meshDataConnectivity2;
								dictionary7 = dictionary4;
								dictionary8 = dictionary6;
							}
							else
							{
								list5 = list;
								list6 = list3;
								meshFaceConnectivity3 = meshFaceConnectivity;
								meshDataConnectivity3 = meshDataConnectivity;
								dictionary7 = dictionary3;
								dictionary8 = dictionary5;
							}
							num21 = -1;
							num22 = -1;
							num23 = -1;
							num24 = -1;
							num25 = -1;
							flag6 = false;
							flag7 = false;
							if (dictionary7.ContainsKey(key5))
							{
								num3++;
								flag6 = true;
								num22 = dictionary7[key5].GetSecondIndex(num6);
								num24 = dictionary7[key5].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num6))
								{
									num22 = dictionary8[num6];
								}
							}
							if (dictionary7.ContainsKey(key6))
							{
								num3++;
								flag7 = true;
								num22 = dictionary7[key6].GetFirstIndex(num6);
								num25 = dictionary7[key6].nClippedIndex;
							}
							else
							{
								num4++;
								if (dictionary8.ContainsKey(num6))
								{
									num22 = dictionary8[num6];
								}
							}
							if (num22 == -1)
							{
								num22 = list5.Count;
								list5.Add(meshDataIn.aVertexData[num6].Copy());
								dictionary8[num6] = num22;
							}
							if (num24 == -1)
							{
								num24 = list5.Count;
								list5.Add(clippedVertexDataOut4);
							}
							if (num25 == -1)
							{
								num25 = list5.Count;
								list5.Add(clippedVertexDataOut5);
							}
							if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
							{
								meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
							}
							list6.Add(num24);
							list6.Add(num22);
							list6.Add(num25);
							if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
							{
								meshFaceConnectivity3.AddEdge(i, v3Vertex7, v3Vertex2, num26, nVertexHash2, num24, num22);
								meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex8, nVertexHash2, num27, num22, num25);
								meshFaceConnectivity3.AddEdge(i, v3Vertex8, v3Vertex7, num27, num26, num25, num24);
							}
							if (list5 == list && !splitOptions.bForceNoCap)
							{
								AddCapEdge(dictionary2, num27, num26, list5[num25].v3Vertex, list5[num24].v3Vertex);
							}
							if (!dictionary7.ContainsKey(key5))
							{
								dictionary7.Add(key5, new ClippedEdge(num5, num6, num21, num22, num24));
							}
							if (!dictionary7.ContainsKey(key6))
							{
								dictionary7.Add(key6, new ClippedEdge(num6, num7, num22, num23, num25));
							}
							continue;
						}
						if (num8 < 0f)
						{
							list5 = list2;
							list6 = list4;
							meshFaceConnectivity3 = meshFaceConnectivity2;
							meshDataConnectivity3 = meshDataConnectivity2;
							dictionary7 = dictionary4;
							dictionary8 = dictionary6;
						}
						int num28 = -1;
						int nNewIndexB = -1;
						int nNewIndexB2 = -1;
						int num29 = -1;
						int num30 = -1;
						int num31 = -1;
						int num32 = -1;
						bool flag8 = false;
						bool flag9 = false;
						EdgeKeyByIndex key8 = new EdgeKeyByIndex(num5, num6);
						EdgeKeyByIndex key9 = new EdgeKeyByIndex(num5, num7);
						if (dictionary7.ContainsKey(key8))
						{
							num3++;
							flag8 = true;
							num28 = dictionary7[key8].GetFirstIndex(num5);
							num29 = dictionary7[key8].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num5))
							{
								num28 = dictionary8[num5];
							}
						}
						if (dictionary7.ContainsKey(key9))
						{
							num3++;
							flag9 = true;
							num28 = dictionary7[key9].GetFirstIndex(num5);
							num30 = dictionary7[key9].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num5))
							{
								num28 = dictionary8[num5];
							}
						}
						EdgeKeyByHash key10 = new EdgeKeyByHash(nVertexHash, nVertexHash2);
						if (dictionary.ContainsKey(key10))
						{
							num31 = dictionary[key10];
						}
						else
						{
							num31 = nCurrentVertexHash++;
							dictionary.Add(key10, num31);
						}
						key10 = new EdgeKeyByHash(nVertexHash, nVertexHash3);
						if (dictionary.ContainsKey(key10))
						{
							num32 = dictionary[key10];
						}
						else
						{
							num32 = nCurrentVertexHash++;
							dictionary.Add(key10, num32);
						}
						VertexData clippedVertexDataOut6 = new VertexData(num31);
						VertexData clippedVertexDataOut7 = new VertexData(num32);
						if (!flag8 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num5, num6, v3Vertex, v3Vertex2, planeSplit, ref clippedVertexDataOut6))
						{
							return false;
						}
						if (!flag9 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num5, num7, v3Vertex, v3Vertex3, planeSplit, ref clippedVertexDataOut7))
						{
							return false;
						}
						if (num28 == -1)
						{
							num28 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num5].Copy());
							dictionary8[num5] = num28;
						}
						if (num29 == -1)
						{
							num29 = list5.Count;
							list5.Add(clippedVertexDataOut6);
						}
						if (num30 == -1)
						{
							num30 = list5.Count;
							list5.Add(clippedVertexDataOut7);
						}
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num28);
						list6.Add(num29);
						list6.Add(num30);
						Vector3 v3Vertex9 = list5[num29].v3Vertex;
						Vector3 v3Vertex10 = list5[num30].v3Vertex;
						if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
						{
							meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex9, nVertexHash, num31, num28, num29);
							meshFaceConnectivity3.AddEdge(i, v3Vertex9, v3Vertex10, num31, num32, num29, num30);
							meshFaceConnectivity3.AddEdge(i, v3Vertex10, v3Vertex, num32, nVertexHash, num30, num28);
						}
						if (list5 == list && !splitOptions.bForceNoCap)
						{
							AddCapEdge(dictionary2, num31, num32, list5[num29].v3Vertex, list5[num30].v3Vertex);
						}
						if (!dictionary7.ContainsKey(key8))
						{
							dictionary7.Add(key8, new ClippedEdge(num5, num6, num28, nNewIndexB, num29));
						}
						if (!dictionary7.ContainsKey(key9))
						{
							dictionary7.Add(key9, new ClippedEdge(num5, num7, num28, nNewIndexB2, num30));
						}
						if (num9 < 0f)
						{
							list5 = list2;
							list6 = list4;
							meshFaceConnectivity3 = meshFaceConnectivity2;
							meshDataConnectivity3 = meshDataConnectivity2;
							dictionary7 = dictionary4;
							dictionary8 = dictionary6;
						}
						else
						{
							list5 = list;
							list6 = list3;
							meshFaceConnectivity3 = meshFaceConnectivity;
							meshDataConnectivity3 = meshDataConnectivity;
							dictionary7 = dictionary3;
							dictionary8 = dictionary5;
						}
						num28 = -1;
						nNewIndexB = -1;
						nNewIndexB2 = -1;
						num29 = -1;
						num30 = -1;
						flag8 = false;
						flag9 = false;
						if (dictionary7.ContainsKey(key8))
						{
							num3++;
							flag8 = true;
							nNewIndexB = dictionary7[key8].GetSecondIndex(num6);
							num29 = dictionary7[key8].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num6))
							{
								nNewIndexB = dictionary8[num6];
							}
						}
						if (dictionary7.ContainsKey(key9))
						{
							num3++;
							flag9 = true;
							nNewIndexB2 = dictionary7[key9].GetSecondIndex(num7);
							num30 = dictionary7[key9].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num7))
							{
								nNewIndexB2 = dictionary8[num7];
							}
						}
						if (nNewIndexB == -1)
						{
							nNewIndexB = list5.Count;
							list5.Add(meshDataIn.aVertexData[num6].Copy());
							dictionary8[num6] = nNewIndexB;
						}
						if (nNewIndexB2 == -1)
						{
							nNewIndexB2 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num7].Copy());
							dictionary8[num7] = nNewIndexB2;
						}
						if (num29 == -1)
						{
							num29 = list5.Count;
							list5.Add(clippedVertexDataOut6);
						}
						if (num30 == -1)
						{
							num30 = list5.Count;
							list5.Add(clippedVertexDataOut7);
						}
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num29);
						list6.Add(nNewIndexB);
						list6.Add(nNewIndexB2);
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num29);
						list6.Add(nNewIndexB2);
						list6.Add(num30);
						if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
						{
							meshFaceConnectivity3.AddEdge(i, v3Vertex9, v3Vertex2, num31, nVertexHash2, num29, nNewIndexB);
							meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex3, nVertexHash2, nVertexHash3, nNewIndexB, nNewIndexB2);
							meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex9, nVertexHash3, num31, nNewIndexB2, num29);
							meshFaceConnectivity3.AddEdge(i, v3Vertex9, v3Vertex3, num31, nVertexHash3, num29, nNewIndexB2);
							meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex10, nVertexHash3, num32, nNewIndexB2, num30);
							meshFaceConnectivity3.AddEdge(i, v3Vertex10, v3Vertex9, num32, num31, num30, num29);
						}
						if (list5 == list && !splitOptions.bForceNoCap)
						{
							AddCapEdge(dictionary2, num32, num31, list5[num30].v3Vertex, list5[num29].v3Vertex);
						}
						if (!dictionary7.ContainsKey(key8))
						{
							dictionary7.Add(key8, new ClippedEdge(num5, num6, num28, nNewIndexB, num29));
						}
						if (!dictionary7.ContainsKey(key9))
						{
							dictionary7.Add(key9, new ClippedEdge(num5, num7, num28, nNewIndexB2, num30));
						}
					}
					else
					{
						if (!(num9 * num10 < 0f))
						{
							continue;
						}
						if (num8 < 0f)
						{
							list5 = list2;
							list6 = list4;
							meshFaceConnectivity3 = meshFaceConnectivity2;
							meshDataConnectivity3 = meshDataConnectivity2;
							dictionary7 = dictionary4;
							dictionary8 = dictionary6;
						}
						int num33 = -1;
						int num34 = -1;
						int nNewIndexB3 = -1;
						int num35 = -1;
						int num36 = -1;
						int num37 = -1;
						int num38 = -1;
						bool flag10 = false;
						bool flag11 = false;
						EdgeKeyByIndex key11 = new EdgeKeyByIndex(num6, num7);
						EdgeKeyByIndex key12 = new EdgeKeyByIndex(num5, num7);
						if (dictionary7.ContainsKey(key11))
						{
							num3++;
							flag10 = true;
							num34 = dictionary7[key11].GetFirstIndex(num6);
							num36 = dictionary7[key11].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num6))
							{
								num34 = dictionary8[num6];
							}
						}
						if (dictionary7.ContainsKey(key12))
						{
							num3++;
							flag11 = true;
							num33 = dictionary7[key12].GetFirstIndex(num5);
							num35 = dictionary7[key12].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num5))
							{
								num33 = dictionary8[num5];
							}
						}
						EdgeKeyByHash key13 = new EdgeKeyByHash(nVertexHash, nVertexHash3);
						if (dictionary.ContainsKey(key13))
						{
							num37 = dictionary[key13];
						}
						else
						{
							num37 = nCurrentVertexHash++;
							dictionary.Add(key13, num37);
						}
						key13 = new EdgeKeyByHash(nVertexHash2, nVertexHash3);
						if (dictionary.ContainsKey(key13))
						{
							num38 = dictionary[key13];
						}
						else
						{
							num38 = nCurrentVertexHash++;
							dictionary.Add(key13, num38);
						}
						VertexData clippedVertexDataOut8 = new VertexData(num37);
						VertexData clippedVertexDataOut9 = new VertexData(num38);
						if (!flag10 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num6, num7, v3Vertex2, v3Vertex3, planeSplit, ref clippedVertexDataOut9))
						{
							return false;
						}
						if (!flag11 && !VertexData.ClipAgainstPlane(meshDataIn.aVertexData, num5, num7, v3Vertex, v3Vertex3, planeSplit, ref clippedVertexDataOut8))
						{
							return false;
						}
						if (num33 == -1)
						{
							num33 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num5].Copy());
							dictionary8[num5] = num33;
						}
						if (num34 == -1)
						{
							num34 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num6].Copy());
							dictionary8[num6] = num34;
						}
						if (num35 == -1)
						{
							num35 = list5.Count;
							list5.Add(clippedVertexDataOut8);
						}
						if (num36 == -1)
						{
							num36 = list5.Count;
							list5.Add(clippedVertexDataOut9);
						}
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num34);
						list6.Add(num36);
						list6.Add(num35);
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num34);
						list6.Add(num35);
						list6.Add(num33);
						Vector3 v3Vertex11 = list5[num35].v3Vertex;
						Vector3 v3Vertex12 = list5[num36].v3Vertex;
						if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
						{
							meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex12, nVertexHash2, num38, num34, num36);
							meshFaceConnectivity3.AddEdge(i, v3Vertex12, v3Vertex11, num38, num37, num36, num35);
							meshFaceConnectivity3.AddEdge(i, v3Vertex11, v3Vertex2, num37, nVertexHash2, num35, num34);
							meshFaceConnectivity3.AddEdge(i, v3Vertex2, v3Vertex11, nVertexHash2, num37, num34, num35);
							meshFaceConnectivity3.AddEdge(i, v3Vertex11, v3Vertex, num37, nVertexHash, num35, num33);
							meshFaceConnectivity3.AddEdge(i, v3Vertex, v3Vertex2, nVertexHash, nVertexHash2, num33, num34);
						}
						if (list5 == list && !splitOptions.bForceNoCap)
						{
							AddCapEdge(dictionary2, num38, num37, list5[num36].v3Vertex, list5[num35].v3Vertex);
						}
						if (!dictionary7.ContainsKey(key11))
						{
							dictionary7.Add(key11, new ClippedEdge(num6, num7, num34, nNewIndexB3, num36));
						}
						if (!dictionary7.ContainsKey(key12))
						{
							dictionary7.Add(key12, new ClippedEdge(num5, num7, num33, nNewIndexB3, num35));
						}
						if (num10 < 0f)
						{
							list5 = list2;
							list6 = list4;
							meshFaceConnectivity3 = meshFaceConnectivity2;
							meshDataConnectivity3 = meshDataConnectivity2;
							dictionary7 = dictionary4;
							dictionary8 = dictionary6;
						}
						else
						{
							list5 = list;
							list6 = list3;
							meshFaceConnectivity3 = meshFaceConnectivity;
							meshDataConnectivity3 = meshDataConnectivity;
							dictionary7 = dictionary3;
							dictionary8 = dictionary5;
						}
						num33 = -1;
						num34 = -1;
						nNewIndexB3 = -1;
						num35 = -1;
						num36 = -1;
						flag10 = false;
						flag11 = false;
						if (dictionary7.ContainsKey(key11))
						{
							num3++;
							flag10 = true;
							nNewIndexB3 = dictionary7[key11].GetSecondIndex(num7);
							num36 = dictionary7[key11].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num7))
							{
								nNewIndexB3 = dictionary8[num7];
							}
						}
						if (dictionary7.ContainsKey(key12))
						{
							num3++;
							flag11 = true;
							nNewIndexB3 = dictionary7[key12].GetSecondIndex(num7);
							num35 = dictionary7[key12].nClippedIndex;
						}
						else
						{
							num4++;
							if (dictionary8.ContainsKey(num7))
							{
								nNewIndexB3 = dictionary8[num7];
							}
						}
						if (nNewIndexB3 == -1)
						{
							nNewIndexB3 = list5.Count;
							list5.Add(meshDataIn.aVertexData[num7].Copy());
							dictionary8[num7] = nNewIndexB3;
						}
						if (num35 == -1)
						{
							num35 = list5.Count;
							list5.Add(clippedVertexDataOut8);
						}
						if (num36 == -1)
						{
							num36 = list5.Count;
							list5.Add(clippedVertexDataOut9);
						}
						if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoChunkConnectionInfo)
						{
							meshDataConnectivity3.NotifyNewClippedFace(meshDataIn, i, j, i, list6.Count / 3);
						}
						list6.Add(num36);
						list6.Add(nNewIndexB3);
						list6.Add(num35);
						if (fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration)
						{
							meshFaceConnectivity3.AddEdge(i, v3Vertex12, v3Vertex3, num38, nVertexHash3, num36, nNewIndexB3);
							meshFaceConnectivity3.AddEdge(i, v3Vertex3, v3Vertex11, nVertexHash3, num37, nNewIndexB3, num35);
							meshFaceConnectivity3.AddEdge(i, v3Vertex11, v3Vertex12, num37, num38, num35, num36);
						}
						if (list5 == list && !splitOptions.bForceNoCap)
						{
							AddCapEdge(dictionary2, num37, num38, list5[num35].v3Vertex, list5[num36].v3Vertex);
						}
						if (!dictionary7.ContainsKey(key11))
						{
							dictionary7.Add(key11, new ClippedEdge(num6, num7, num34, nNewIndexB3, num36));
						}
						if (!dictionary7.ContainsKey(key12))
						{
							dictionary7.Add(key12, new ClippedEdge(num5, num7, num33, nNewIndexB3, num35));
						}
					}
				}
			}
			Vector3 vector = Vector3.zero;
			if (list.Count > 0)
			{
				Vector3 v3Min = Vector3.zero;
				Vector3 v3Max = Vector3.zero;
				MeshData.ComputeMinMax(list, ref v3Min, ref v3Max);
				vector = (v3Min + v3Max) * 0.5f;
			}
			Matrix4x4 inverse = Matrix4x4.TRS(vector, meshDataIn.qRotation, meshDataIn.v3Scale).inverse;
			if (splitOptions.bVerticesAreLocal)
			{
				inverse = Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one).inverse;
			}
			Vector3 vector2 = Vector3.zero;
			if (list2.Count > 0)
			{
				Vector3 v3Min2 = Vector3.zero;
				Vector3 v3Max2 = Vector3.zero;
				MeshData.ComputeMinMax(list2, ref v3Min2, ref v3Max2);
				vector2 = (v3Min2 + v3Max2) * 0.5f;
			}
			Matrix4x4 inverse2 = Matrix4x4.TRS(vector2, meshDataIn.qRotation, meshDataIn.v3Scale).inverse;
			if (splitOptions.bVerticesAreLocal)
			{
				inverse2 = Matrix4x4.TRS(vector2, Quaternion.identity, Vector3.one).inverse;
			}
			List<List<Vector3>> list7 = new List<List<Vector3>>();
			List<List<int>> list8 = new List<List<int>>();
			bool flag12 = false;
			Matrix4x4 mtxPlane = Matrix4x4.TRS(v3PlanePoint, Quaternion.LookRotation(Vector3.Cross(v3PlaneNormal, v3PlaneRight), v3PlaneNormal), Vector3.one);
			if (dictionary2.Count > 0 && !splitOptions.bForceNoCap)
			{
				if (ResolveCap(dictionary2, list7, list8, fracturedComponent))
				{
					if (list7.Count > 1)
					{
						flag12 = ((fracturedComponent.GenerateIslands && !splitOptions.bForceNoIslandGeneration) ? true : false);
					}
					TriangulateConstrainedDelaunay(list7, list8, splitOptions.bForceCapVertexSoup, fracturedComponent, flag12, meshFaceConnectivity, meshFaceConnectivity2, meshDataConnectivity, meshDataConnectivity2, splitOptions.nForceMeshConnectivityHash, num2, mtxPlane, inverse, inverse2, vector, vector2, array, list, array2, list2);
				}
				else if (fracturedComponent.Verbose)
				{
					Debug.LogWarning("Error resolving cap");
				}
			}
			if (flag12)
			{
				List<MeshData> collection = MeshData.PostProcessConnectivity(meshDataIn, meshFaceConnectivity, meshDataConnectivity, array, list, num2, nCurrentVertexHash, false);
				List<MeshData> collection2 = new List<MeshData>();
				if (!splitOptions.bIgnoreNegativeSide)
				{
					collection2 = MeshData.PostProcessConnectivity(meshDataIn, meshFaceConnectivity2, meshDataConnectivity2, array2, list2, num2, nCurrentVertexHash, false);
				}
				List<MeshData> list9 = new List<MeshData>();
				list9.AddRange(collection);
				list9.AddRange(collection2);
				if (fracturedComponent.GenerateChunkConnectionInfo && !splitOptions.bForceNoIslandConnectionInfo)
				{
					for (int k = 0; k < list9.Count; k++)
					{
						if (progress != null && list9.Count > 10 && !splitOptions.bForceNoProgressInfo)
						{
							progress("Fracturing", "Processing island connectivity...", (float)k / (float)list9.Count);
							if (IsFracturingCancelled())
							{
								return false;
							}
						}
						for (int l = 0; l < list9.Count; l++)
						{
							if (k != l)
							{
								ComputeIslandsMeshDataConnectivity(fracturedComponent, splitOptions.bVerticesAreLocal, list9[k], list9[l]);
							}
						}
					}
				}
				listMeshDatasPosOut.AddRange(collection);
				listMeshDatasNegOut.AddRange(collection2);
			}
			else
			{
				if (list.Count > 0 && array.Length != 0)
				{
					MeshData meshData = new MeshData(meshDataIn.aMaterials, array, list, num2, vector, meshDataIn.qRotation, meshDataIn.v3Scale, inverse, false, false);
					meshData.meshDataConnectivity = meshDataConnectivity;
					meshData.nCurrentVertexHash = nCurrentVertexHash;
					listMeshDatasPosOut.Add(meshData);
				}
				if (list2.Count > 0 && array2.Length != 0 && !splitOptions.bIgnoreNegativeSide)
				{
					MeshData meshData2 = new MeshData(meshDataIn.aMaterials, array2, list2, num2, vector2, meshDataIn.qRotation, meshDataIn.v3Scale, inverse2, false, false);
					meshData2.meshDataConnectivity = meshDataConnectivity2;
					meshData2.nCurrentVertexHash = nCurrentVertexHash;
					listMeshDatasNegOut.Add(meshData2);
				}
			}
			return true;
		}

		private static bool ComputeIslandsMeshDataConnectivity(FracturedObject fracturedComponent, bool bVerticesAreLocal, MeshData meshData1, MeshData meshData2)
		{
			float chunkIslandConnectionMaxDistance = fracturedComponent.ChunkIslandConnectionMaxDistance;
			Vector3 a = meshData1.v3Min;
			if (bVerticesAreLocal)
			{
				a = Vector3.Scale(a, meshData1.v3Scale);
			}
			Vector3 a2 = meshData1.v3Max;
			if (bVerticesAreLocal)
			{
				a2 = Vector3.Scale(a2, meshData1.v3Scale);
			}
			Vector3 a3 = meshData2.v3Min;
			if (bVerticesAreLocal)
			{
				a3 = Vector3.Scale(a3, meshData2.v3Scale);
			}
			Vector3 a4 = meshData2.v3Max;
			if (bVerticesAreLocal)
			{
				a4 = Vector3.Scale(a4, meshData2.v3Scale);
			}
			if (a.x > a4.x + chunkIslandConnectionMaxDistance || a.y > a4.y + chunkIslandConnectionMaxDistance || a.z > a4.z + chunkIslandConnectionMaxDistance)
			{
				return false;
			}
			if (a3.x > a2.x + chunkIslandConnectionMaxDistance || a3.y > a2.y + chunkIslandConnectionMaxDistance || a3.z > a2.z + chunkIslandConnectionMaxDistance)
			{
				return false;
			}
			bool result = false;
			float chunkIslandConnectionMaxDistance2 = fracturedComponent.ChunkIslandConnectionMaxDistance;
			for (int i = 0; i < meshData1.aaIndices.Length; i++)
			{
				for (int j = 0; j < meshData1.aaIndices[i].Length / 3; j++)
				{
					Vector3 vector = meshData1.aVertexData[meshData1.aaIndices[i][j * 3]].v3Vertex;
					Vector3 vector2 = meshData1.aVertexData[meshData1.aaIndices[i][j * 3 + 1]].v3Vertex;
					Vector3 vector3 = meshData1.aVertexData[meshData1.aaIndices[i][j * 3 + 2]].v3Vertex;
					if (bVerticesAreLocal)
					{
						vector = Vector3.Scale(vector, meshData1.v3Scale);
						vector2 = Vector3.Scale(vector2, meshData1.v3Scale);
						vector3 = Vector3.Scale(vector3, meshData1.v3Scale);
					}
					Vector3 vector4 = -Vector3.Cross(vector2 - vector, vector3 - vector);
					if (vector4.magnitude < Parameters.EPSILONCROSSPRODUCT)
					{
						continue;
					}
					Quaternion q = Quaternion.LookRotation(vector4.normalized, (vector2 - vector).normalized);
					Matrix4x4 inverse = Matrix4x4.TRS(vector, q, Vector3.one).inverse;
					Plane plane = new Plane(vector, vector2, vector3);
					for (int k = 0; k < meshData2.aaIndices.Length; k++)
					{
						for (int l = 0; l < meshData2.aaIndices[k].Length / 3; l++)
						{
							Vector3 vector5 = meshData2.aVertexData[meshData2.aaIndices[k][l * 3]].v3Vertex;
							Vector3 vector6 = meshData2.aVertexData[meshData2.aaIndices[k][l * 3 + 1]].v3Vertex;
							Vector3 vector7 = meshData2.aVertexData[meshData2.aaIndices[k][l * 3 + 2]].v3Vertex;
							if (bVerticesAreLocal)
							{
								vector5 = Vector3.Scale(vector5, meshData2.v3Scale);
								vector6 = Vector3.Scale(vector6, meshData2.v3Scale);
								vector7 = Vector3.Scale(vector7, meshData2.v3Scale);
							}
							if (Mathf.Abs(plane.GetDistanceToPoint(vector5)) > chunkIslandConnectionMaxDistance2 || Mathf.Abs(plane.GetDistanceToPoint(vector6)) > chunkIslandConnectionMaxDistance2 || Mathf.Abs(plane.GetDistanceToPoint(vector7)) > chunkIslandConnectionMaxDistance2)
							{
								continue;
							}
							Vector3 point = (vector5 + vector6 + vector7) / 3f;
							point = inverse.MultiplyPoint3x4(point);
							Vector3 vector8 = inverse.MultiplyPoint3x4(vector);
							Vector3 vector9 = inverse.MultiplyPoint3x4(vector2);
							Vector3 vector10 = inverse.MultiplyPoint3x4(vector3);
							Vector3 lhs = vector10 - vector9;
							Vector3 lhs2 = vector8 - vector10;
							bool flag = false;
							if (point.x >= 0f && Vector3.Cross(lhs, point - vector9).z <= 0f && Vector3.Cross(lhs2, point - vector10).z <= 0f)
							{
								flag = true;
							}
							if (!flag)
							{
								Vector3 vector11 = inverse.MultiplyPoint3x4(vector5);
								Vector3 vector12 = inverse.MultiplyPoint3x4(vector6);
								Vector3 vector13 = inverse.MultiplyPoint3x4(vector7);
								if (!flag && IntersectEdges2D(vector11.x, vector11.y, vector12.x, vector12.y, vector8.x, vector8.y, vector9.x, vector9.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector11.x, vector11.y, vector12.x, vector12.y, vector9.x, vector9.y, vector10.x, vector10.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector11.x, vector11.y, vector12.x, vector12.y, vector10.x, vector10.y, vector8.x, vector8.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector12.x, vector12.y, vector13.x, vector13.y, vector8.x, vector8.y, vector9.x, vector9.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector12.x, vector12.y, vector13.x, vector13.y, vector9.x, vector9.y, vector10.x, vector10.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector12.x, vector12.y, vector13.x, vector13.y, vector10.x, vector10.y, vector8.x, vector8.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector13.x, vector13.y, vector11.x, vector11.y, vector8.x, vector8.y, vector9.x, vector9.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector13.x, vector13.y, vector11.x, vector11.y, vector9.x, vector9.y, vector10.x, vector10.y))
								{
									flag = true;
								}
								if (!flag && IntersectEdges2D(vector13.x, vector13.y, vector11.x, vector11.y, vector10.x, vector10.y, vector8.x, vector8.y))
								{
									flag = true;
								}
							}
							if (flag)
							{
								int newHash = MeshDataConnectivity.GetNewHash();
								meshData1.meshDataConnectivity.NotifyNewCapFace(newHash, i, j);
								meshData2.meshDataConnectivity.NotifyNewCapFace(newHash, k, l);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		public static bool IntersectEdges2D(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			Vector2 vector = new Vector2(x1, y1);
			Vector2 vector2 = new Vector2(x3, y3);
			Vector2 vector3 = new Vector2(x2 - x1, y2 - y1);
			Vector2 b = new Vector2(x4 - x3, y4 - y3);
			float num = CrossProduct2D(vector3, b);
			if (num < Parameters.EPSILONCROSSPRODUCT)
			{
				return false;
			}
			float num2 = CrossProduct2D(vector2 - vector, b) / num;
			float num3 = CrossProduct2D(vector2 - vector, vector3) / num;
			float ePSILONINSIDETRIANGLE = Parameters.EPSILONINSIDETRIANGLE;
			if (num2 >= ePSILONINSIDETRIANGLE && num2 <= 1f - ePSILONINSIDETRIANGLE && num3 >= ePSILONINSIDETRIANGLE && num3 <= 1f - ePSILONINSIDETRIANGLE)
			{
				return true;
			}
			return false;
		}

		private static float CrossProduct2D(Vector2 a, Vector2 b)
		{
			return a.x * b.y - a.y * b.x;
		}

		private static void TriangulateConstrainedDelaunay(List<List<Vector3>> listlistPointsConstrainedDelaunay, List<List<int>> listlistHashValuesConstrainedDelaunay, bool bForceVertexSoup, FracturedObject fracturedComponent, bool bConnectivityPostprocess, MeshFaceConnectivity faceConnectivityPos, MeshFaceConnectivity faceConnectivityNeg, MeshDataConnectivity meshConnectivityPos, MeshDataConnectivity meshConnectivityNeg, int nForceMeshConnectivityHash, int nSplitCloseSubMesh, Matrix4x4 mtxPlane, Matrix4x4 mtxToLocalPos, Matrix4x4 mtxToLocalNeg, Vector3 v3CenterPos, Vector3 v3CenterNeg, List<int>[] aListIndicesPosInOut, List<VertexData> listVertexDataPosInOut, List<int>[] aListIndicesNegInOut, List<VertexData> listVertexDataNegInOut)
		{
			Matrix4x4 inverse = mtxPlane.inverse;
			List<List<Point2D>> list = new List<List<Point2D>>();
			List<List<PolygonPoint>> list2 = new List<List<PolygonPoint>>();
			List<Polygon> list3 = new List<Polygon>();
			for (int i = 0; i < listlistPointsConstrainedDelaunay.Count; i++)
			{
				List<Point2D> list4 = new List<Point2D>();
				List<PolygonPoint> list5 = new List<PolygonPoint>();
				foreach (Vector3 item in listlistPointsConstrainedDelaunay[i])
				{
					Vector3 vector = inverse.MultiplyPoint3x4(item);
					list4.Add(new Point2D(vector.x, vector.z));
					list5.Add(new PolygonPoint(vector.x, vector.z));
				}
				list.Add(list4);
				list2.Add(list5);
				list3.Add(new Polygon(list5));
			}
			float num = Mathf.Max(Parameters.EPSILONCAPPRECISIONMIN, fracturedComponent.CapPrecisionFix);
			if (num > 0f)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					double x = list2[j][list2[j].Count - 1].X;
					double y = list2[j][list2[j].Count - 1].Y;
					bool flag = false;
					for (int k = 0; k < list2[j].Count; k++)
					{
						double num2 = list2[j][k].X - x;
						double num3 = list2[j][k].Y - y;
						if (Math.Sqrt(num2 * num2 + num3 * num3) < (double)num)
						{
							list2[j].RemoveAt(k);
							k--;
							if (list2[j].Count < 3)
							{
								flag = true;
								break;
							}
						}
						else
						{
							x = list2[j][k].X;
							y = list2[j][k].Y;
						}
					}
					if (flag)
					{
						list2.RemoveAt(j);
						j--;
					}
				}
			}
			if (list2.Count == 0)
			{
				return;
			}
			int num4 = -1;
			Polygon polygon = null;
			if (!bForceVertexSoup)
			{
				for (int l = 0; l < list2.Count; l++)
				{
					for (int m = 0; m < list2.Count; m++)
					{
						if (l != m && list[l].Count >= 3 && list[m].Count >= 3)
						{
							if (PolygonUtil.PolygonContainsPolygon(list[l], list3[l].Bounds, list[m], list3[m].Bounds, true))
							{
								num4 = l;
								break;
							}
							if (PolygonUtil.PolygonContainsPolygon(list[m], list3[m].Bounds, list[l], list3[l].Bounds, true))
							{
								num4 = m;
								break;
							}
						}
					}
				}
				if (num4 != -1)
				{
					polygon = list3[num4];
					for (int n = 0; n < list2.Count; n++)
					{
						if (n != num4 && list3[n].Count >= 3)
						{
							polygon.AddHole(list3[n]);
						}
					}
				}
			}
			bool flag2 = false;
			if (polygon != null && !bForceVertexSoup)
			{
				try
				{
					P2T.Triangulate(polygon);
					if (polygon.Triangles != null)
					{
						List<Vector3> list6 = new List<Vector3>();
						List<int> list7 = new List<int>();
						CreateIndexedMesh(polygon.Triangles, list6, list7, mtxPlane, true);
						Triangulate(list6, list7, fracturedComponent, listlistPointsConstrainedDelaunay, listlistHashValuesConstrainedDelaunay, bConnectivityPostprocess, faceConnectivityPos, faceConnectivityNeg, meshConnectivityPos, meshConnectivityNeg, nForceMeshConnectivityHash, nSplitCloseSubMesh, mtxPlane, mtxToLocalPos, mtxToLocalNeg, v3CenterPos, v3CenterNeg, aListIndicesPosInOut, listVertexDataPosInOut, aListIndicesNegInOut, listVertexDataNegInOut);
					}
					flag2 = true;
				}
				catch (Exception ex)
				{
					if (fracturedComponent.Verbose)
					{
						Debug.LogWarning(string.Concat("Exception (", ex.GetType(), ") using hole triangulation (holes = ", list2.Count, "). Trying to use constrained delaunay."));
					}
					flag2 = false;
				}
			}
			if (flag2)
			{
				return;
			}
			if (bForceVertexSoup)
			{
				List<TriangulationPoint> list8 = new List<TriangulationPoint>();
				if (list2.Count <= 0)
				{
					return;
				}
				foreach (PolygonPoint item2 in list2[0])
				{
					list8.Add(item2);
				}
				try
				{
					if (list8.Count >= 3)
					{
						PointSet pointSet = new PointSet(list8);
						P2T.Triangulate(pointSet);
						if (pointSet.Triangles != null)
						{
							List<Vector3> list9 = new List<Vector3>();
							List<int> list10 = new List<int>();
							CreateIndexedMesh(pointSet.Triangles, list9, list10, mtxPlane, true);
							Triangulate(list9, list10, fracturedComponent, listlistPointsConstrainedDelaunay, listlistHashValuesConstrainedDelaunay, bConnectivityPostprocess, faceConnectivityPos, faceConnectivityNeg, meshConnectivityPos, meshConnectivityNeg, nForceMeshConnectivityHash, nSplitCloseSubMesh, mtxPlane, mtxToLocalPos, mtxToLocalNeg, v3CenterPos, v3CenterNeg, aListIndicesPosInOut, listVertexDataPosInOut, aListIndicesNegInOut, listVertexDataNegInOut);
						}
					}
					return;
				}
				catch (Exception ex2)
				{
					if (fracturedComponent.Verbose)
					{
						Debug.LogWarning(string.Concat("Exception (", ex2.GetType(), ") using vertex soup triangulation."));
					}
					return;
				}
			}
			int num5 = 0;
			foreach (List<PolygonPoint> item3 in list2)
			{
				IList<DelaunayTriangle> list11 = null;
				try
				{
					if (item3.Count >= 3)
					{
						Polygon polygon2 = new Polygon(item3);
						P2T.Triangulate(polygon2);
						list11 = polygon2.Triangles;
					}
				}
				catch (Exception ex3)
				{
					if (fracturedComponent.Verbose)
					{
						Debug.LogWarning(string.Concat("Exception (", ex3.GetType(), ") using polygon triangulation of cap polygon ", num5, ". Trying to use non constrained"));
					}
					list11 = null;
				}
				if (list11 == null)
				{
					List<TriangulationPoint> list12 = new List<TriangulationPoint>();
					foreach (PolygonPoint item4 in item3)
					{
						list12.Add(item4);
					}
					try
					{
						if (list12.Count >= 3)
						{
							PointSet pointSet2 = new PointSet(list12);
							P2T.Triangulate(pointSet2);
							list11 = pointSet2.Triangles;
						}
					}
					catch (Exception ex4)
					{
						if (fracturedComponent.Verbose)
						{
							Debug.LogWarning(string.Concat("Exception (", ex4.GetType(), ") using non constrained triangulation of cap polygon ", num5, ". Skipping"));
						}
					}
				}
				if (list11 != null)
				{
					List<Vector3> list13 = new List<Vector3>();
					List<int> list14 = new List<int>();
					CreateIndexedMesh(list11, list13, list14, mtxPlane, true);
					Triangulate(list13, list14, fracturedComponent, listlistPointsConstrainedDelaunay, listlistHashValuesConstrainedDelaunay, bConnectivityPostprocess, faceConnectivityPos, faceConnectivityNeg, meshConnectivityPos, meshConnectivityNeg, nForceMeshConnectivityHash, nSplitCloseSubMesh, mtxPlane, mtxToLocalPos, mtxToLocalNeg, v3CenterPos, v3CenterNeg, aListIndicesPosInOut, listVertexDataPosInOut, aListIndicesNegInOut, listVertexDataNegInOut);
				}
				num5++;
			}
		}

		private static void CreateIndexedMesh(IList<DelaunayTriangle> listTriangles, List<Vector3> listVerticesOut, List<int> listIndicesOut, Matrix4x4 mtxTransform, bool bTransform)
		{
			listVerticesOut.Clear();
			listIndicesOut.Clear();
			Vector3 zero = Vector3.zero;
			foreach (DelaunayTriangle listTriangle in listTriangles)
			{
				for (int i = 0; i < 3; i++)
				{
					bool flag = false;
					int num = 0;
					TriangulationPoint triangulationPoint = listTriangle.PointCWFrom(listTriangle.Points[i]);
					zero.x = triangulationPoint.Xf;
					zero.z = triangulationPoint.Yf;
					foreach (Vector3 item in listVerticesOut)
					{
						if ((zero - item).magnitude < Parameters.EPSILONDISTANCEVERTEX)
						{
							flag = true;
							break;
						}
						num++;
					}
					if (!flag)
					{
						listIndicesOut.Add(listVerticesOut.Count);
						listVerticesOut.Add(zero);
					}
					else
					{
						listIndicesOut.Add(num);
					}
				}
			}
			if (bTransform)
			{
				for (int j = 0; j < listVerticesOut.Count; j++)
				{
					listVerticesOut[j] = mtxTransform.MultiplyPoint3x4(listVerticesOut[j]);
				}
			}
		}

		private static void Triangulate(List<Vector3> listVertices, List<int> listIndices, FracturedObject fracturedComponent, List<List<Vector3>> listlistPointsConstrainedDelaunay, List<List<int>> listlistHashValuesConstrainedDelaunay, bool bConnectivityPostprocess, MeshFaceConnectivity faceConnectivityPos, MeshFaceConnectivity faceConnectivityNeg, MeshDataConnectivity meshConnectivityPos, MeshDataConnectivity meshConnectivityNeg, int nForceMeshConnectivityHash, int nSplitCloseSubMesh, Matrix4x4 mtxPlane, Matrix4x4 mtxToLocalPos, Matrix4x4 mtxToLocalNeg, Vector3 v3CenterPos, Vector3 v3CenterNeg, List<int>[] aListIndicesPosInOut, List<VertexData> listVertexDataPosInOut, List<int>[] aListIndicesNegInOut, List<VertexData> listVertexDataNegInOut)
		{
			int count = listVertexDataPosInOut.Count;
			int count2 = listVertexDataNegInOut.Count;
			if (listVertexDataPosInOut.Count < 1 || listVertexDataNegInOut.Count < 1)
			{
				return;
			}
			VertexData[] array = new VertexData[listVertices.Count];
			VertexData[] array2 = new VertexData[listVertices.Count];
			Vector3 vector = mtxPlane.MultiplyVector(Vector3.up);
			float num = (fracturedComponent.InvertCapNormals ? (-1f) : 1f);
			Vector3 v3Normal = mtxToLocalPos.MultiplyVector(-vector * num);
			Vector3 v3Normal2 = mtxToLocalNeg.MultiplyVector(vector * num);
			Vector3 right = Vector3.right;
			right = mtxPlane.MultiplyVector(right);
			right = mtxToLocalPos.MultiplyVector(right);
			Vector3 right2 = Vector3.right;
			right2 = mtxPlane.MultiplyVector(right2);
			right2 = mtxToLocalNeg.MultiplyVector(right2);
			Matrix4x4 inverse = mtxPlane.inverse;
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Vector3 vector2 = Vector2.zero;
			for (int i = 0; i < listVertices.Count; i++)
			{
				Vector3 vector3 = inverse.MultiplyPoint3x4(listVertices[i]);
				vector2.x = vector3.x * fracturedComponent.SplitMappingTileU;
				vector2.y = vector3.z * fracturedComponent.SplitMappingTileV;
				int nVertexHash = ComputeVertexHash(listVertices[i], listlistPointsConstrainedDelaunay, listlistHashValuesConstrainedDelaunay);
				array[i] = new VertexData(nVertexHash, listVertices[i], v3Normal, right, color, vector2, vector2, true, true, listVertexDataPosInOut[0].bHasColor32, listVertexDataPosInOut[0].bHasMapping1, listVertexDataPosInOut[0].bHasMapping2);
				array2[i] = new VertexData(nVertexHash, listVertices[i], v3Normal2, right2, color, vector2, vector2, true, true, listVertexDataNegInOut[0].bHasColor32, listVertexDataNegInOut[0].bHasMapping1, listVertexDataNegInOut[0].bHasMapping2);
			}
			listVertexDataPosInOut.AddRange(array);
			listVertexDataNegInOut.AddRange(array2);
			for (int j = 0; j < listIndices.Count / 3; j++)
			{
				int num2 = listIndices[j * 3];
				int num3 = listIndices[j * 3 + 1];
				int num4 = listIndices[j * 3 + 2];
				int nVertexHash2 = listVertexDataPosInOut[count + num2].nVertexHash;
				int nVertexHash3 = listVertexDataPosInOut[count + num3].nVertexHash;
				int nVertexHash4 = listVertexDataPosInOut[count + num4].nVertexHash;
				int nVertexHash5 = listVertexDataNegInOut[count2 + num2].nVertexHash;
				int nVertexHash6 = listVertexDataNegInOut[count2 + num3].nVertexHash;
				int nVertexHash7 = listVertexDataNegInOut[count2 + num4].nVertexHash;
				if (nVertexHash2 != -1 && nVertexHash3 != -1 && nVertexHash4 != -1 && nVertexHash5 != -1 && nVertexHash6 != -1 && nVertexHash7 != -1)
				{
					int nHash = ((nForceMeshConnectivityHash == -1) ? MeshDataConnectivity.GetNewHash() : nForceMeshConnectivityHash);
					if (fracturedComponent.GenerateChunkConnectionInfo)
					{
						meshConnectivityPos.NotifyNewCapFace(nHash, nSplitCloseSubMesh, aListIndicesPosInOut[nSplitCloseSubMesh].Count / 3);
					}
					aListIndicesPosInOut[nSplitCloseSubMesh].Add(count + num2);
					aListIndicesPosInOut[nSplitCloseSubMesh].Add(count + num3);
					aListIndicesPosInOut[nSplitCloseSubMesh].Add(count + num4);
					if (bConnectivityPostprocess)
					{
						faceConnectivityPos.AddEdge(nSplitCloseSubMesh, listVertices[num2], listVertices[num3], nVertexHash2, nVertexHash3, count + num2, count + num3);
						faceConnectivityPos.AddEdge(nSplitCloseSubMesh, listVertices[num3], listVertices[num4], nVertexHash3, nVertexHash4, count + num3, count + num4);
						faceConnectivityPos.AddEdge(nSplitCloseSubMesh, listVertices[num4], listVertices[num2], nVertexHash4, nVertexHash2, count + num4, count + num2);
					}
					if (fracturedComponent.GenerateChunkConnectionInfo)
					{
						meshConnectivityNeg.NotifyNewCapFace(nHash, nSplitCloseSubMesh, aListIndicesNegInOut[nSplitCloseSubMesh].Count / 3);
					}
					aListIndicesNegInOut[nSplitCloseSubMesh].Add(count2 + num2);
					aListIndicesNegInOut[nSplitCloseSubMesh].Add(count2 + num4);
					aListIndicesNegInOut[nSplitCloseSubMesh].Add(count2 + num3);
					if (bConnectivityPostprocess)
					{
						faceConnectivityNeg.AddEdge(nSplitCloseSubMesh, listVertices[num2], listVertices[num4], nVertexHash5, nVertexHash7, count2 + num2, count2 + num4);
						faceConnectivityNeg.AddEdge(nSplitCloseSubMesh, listVertices[num4], listVertices[num3], nVertexHash7, nVertexHash6, count2 + num4, count2 + num3);
						faceConnectivityNeg.AddEdge(nSplitCloseSubMesh, listVertices[num3], listVertices[num2], nVertexHash6, nVertexHash5, count2 + num3, count2 + num2);
					}
				}
			}
		}

		private static int ComputeVertexHash(Vector3 v3Vertex, List<List<Vector3>> listlistPointsConstrainedDelaunay, List<List<int>> listlistHashValuesConstrainedDelaunay)
		{
			float num = float.MaxValue;
			int result = -1;
			for (int i = 0; i < listlistPointsConstrainedDelaunay.Count; i++)
			{
				for (int j = 0; j < listlistPointsConstrainedDelaunay[i].Count; j++)
				{
					float num2 = Vector3.SqrMagnitude(listlistPointsConstrainedDelaunay[i][j] - v3Vertex);
					if (num2 < num)
					{
						result = listlistHashValuesConstrainedDelaunay[i][j];
						num = num2;
					}
				}
			}
			float num3 = Parameters.EPSILONCAPPRECISIONMIN * Parameters.EPSILONCAPPRECISIONMIN;
			if (num > num3)
			{
				return -1;
			}
			return result;
		}

		private static bool AddCapEdge(Dictionary<EdgeKeyByHash, CapEdge> dicCapEdges, int nVertexHash1, int nVertexHash2, Vector3 v3Vertex1, Vector3 v3Vertex2)
		{
			float fLength = Vector3.Distance(v3Vertex1, v3Vertex2);
			EdgeKeyByHash key = new EdgeKeyByHash(nVertexHash1, nVertexHash2);
			if (!dicCapEdges.ContainsKey(key))
			{
				dicCapEdges.Add(key, new CapEdge(nVertexHash1, nVertexHash2, v3Vertex1, v3Vertex2, fLength));
			}
			return true;
		}

		private static bool ResolveCap(Dictionary<EdgeKeyByHash, CapEdge> dicCapEdges, List<List<Vector3>> listlistResolvedCapVertices, List<List<int>> listlistResolvedCapHashValues, FracturedObject fracturedComponent)
		{
			if (dicCapEdges.Count < 3)
			{
				if (fracturedComponent.Verbose)
				{
					Debug.LogWarning("Cap has < 3 segments");
				}
				return false;
			}
			listlistResolvedCapVertices.Clear();
			listlistResolvedCapHashValues.Clear();
			List<CapEdge> list = new List<CapEdge>();
			List<CapEdge> list2 = new List<CapEdge>(dicCapEdges.Values);
			List<Vector3> list3 = new List<Vector3>();
			List<int> list4 = new List<int>();
			int count = list2.Count;
			list.Add(list2[0]);
			list3.Add(list2[0].v1);
			list3.Add(list2[0].v2);
			list4.Add(list2[0].nHash1);
			list4.Add(list2[0].nHash2);
			list2.RemoveAt(0);
			while (list2.Count > 0)
			{
				CapEdge capEdge = list[list.Count - 1];
				CapEdge capEdge2 = list[0];
				bool flag = false;
				for (int i = 0; i < list2.Count; i++)
				{
					CapEdge capEdge3 = list2[i];
					int num = capEdge.SharesVertex1Of(capEdge3);
					int num2 = capEdge2.SharesVertex2Of(capEdge3);
					if (num == 2)
					{
						list3.Add(capEdge3.v2);
						list4.Add(capEdge3.nHash2);
						list.Add(capEdge3);
						list2.RemoveAt(i);
						flag = true;
						break;
					}
					if (num2 == 1)
					{
						list3.Insert(0, capEdge3.v1);
						list4.Insert(0, capEdge3.nHash1);
						list.Insert(0, capEdge3);
						list2.RemoveAt(i);
						flag = true;
						break;
					}
				}
				bool flag2 = !flag;
				if (list.Count >= 3 && list[list.Count - 1].SharesVertex1Of(list[0]) == 2)
				{
					flag2 = true;
				}
				if (list2.Count == 0)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					continue;
				}
				int num3 = list3.Count;
				if (Vector3.Distance(list3[0], list3[list3.Count - 1]) < Parameters.EPSILONDISTANCEVERTEX)
				{
					num3 = list3.Count - 1;
				}
				if (num3 > 2)
				{
					List<Vector3> list5 = new List<Vector3>();
					List<int> list6 = new List<int>();
					for (int j = 0; j < num3; j++)
					{
						list5.Add(list3[j]);
						list6.Add(list4[j]);
					}
					listlistResolvedCapVertices.Add(list5);
					listlistResolvedCapHashValues.Add(list6);
				}
				else if (fracturedComponent.Verbose)
				{
					Debug.LogWarning("Cap group has less than 3 vertices (" + list3.Count + ")");
				}
				if (list2.Count > 0)
				{
					list.Clear();
					list.Add(list2[0]);
					list3.Clear();
					list4.Clear();
					list3.Add(list2[0].v1);
					list3.Add(list2[0].v2);
					list4.Add(list2[0].nHash1);
					list4.Add(list2[0].nHash2);
					list2.RemoveAt(0);
				}
			}
			if (list2.Count > 0 && fracturedComponent.Verbose)
			{
				Debug.LogWarning(string.Format("Cap has {0}/{1} unresolved segments left", list2.Count, count));
			}
			return true;
		}
	}
}
