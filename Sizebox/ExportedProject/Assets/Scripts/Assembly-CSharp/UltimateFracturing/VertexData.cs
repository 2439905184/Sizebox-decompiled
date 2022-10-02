using UnityEngine;

namespace UltimateFracturing
{
	public class VertexData
	{
		public int nVertexHash;

		public Vector3 v3Vertex;

		public Vector3 v3Normal;

		public Vector4 v4Tangent;

		public Color32 color32;

		public Vector2 v2Mapping1;

		public Vector2 v2Mapping2;

		public bool bHasNormal;

		public bool bHasTangent;

		public bool bHasColor32;

		public bool bHasMapping1;

		public bool bHasMapping2;

		public VertexData(int nVertexHash)
		{
			this.nVertexHash = nVertexHash;
			v3Vertex = Vector3.zero;
			v3Normal = Vector3.zero;
			v4Tangent = Vector4.zero;
			color32.r = (color32.g = (color32.b = (color32.a = byte.MaxValue)));
			v2Mapping1 = Vector2.zero;
			v2Mapping2 = Vector2.zero;
			bHasNormal = false;
			bHasTangent = false;
			bHasColor32 = false;
			bHasMapping1 = false;
			bHasMapping2 = false;
		}

		public VertexData(int nVertexHash, Vector3 v3Vertex, Vector3 v3Normal, Vector3 v4Tangent, Color32 color32, Vector2 v2Mapping1, Vector2 v2Mapping2, bool bHasNormal, bool bHasTangent, bool bHasColor32, bool bHasMapping1, bool bHasMapping2)
		{
			this.nVertexHash = nVertexHash;
			this.v3Vertex = v3Vertex;
			this.v3Normal = v3Normal;
			this.v4Tangent = v4Tangent;
			this.color32 = color32;
			this.v2Mapping1 = v2Mapping1;
			this.v2Mapping2 = v2Mapping2;
			this.bHasNormal = bHasNormal;
			this.bHasTangent = bHasTangent;
			this.bHasColor32 = bHasColor32;
			this.bHasMapping1 = bHasMapping1;
			this.bHasMapping2 = bHasMapping2;
		}

		public VertexData Copy()
		{
			return new VertexData(nVertexHash)
			{
				v3Vertex = v3Vertex,
				v3Normal = v3Normal,
				v4Tangent = v4Tangent,
				color32 = color32,
				v2Mapping1 = v2Mapping1,
				v2Mapping2 = v2Mapping2,
				bHasNormal = bHasNormal,
				bHasTangent = bHasTangent,
				bHasColor32 = bHasColor32,
				bHasMapping1 = bHasMapping1,
				bHasMapping2 = bHasMapping2
			};
		}

		public static VertexData Lerp(int nVertexHash, VertexData vd1, VertexData vd2, float fT)
		{
			VertexData vertexData = new VertexData(nVertexHash);
			vertexData.bHasNormal = vd1.bHasNormal;
			vertexData.bHasTangent = vd1.bHasTangent;
			vertexData.bHasColor32 = vd1.bHasColor32;
			vertexData.bHasMapping1 = vd1.bHasMapping1;
			vertexData.bHasMapping2 = vd1.bHasMapping2;
			Vector3 vector = Vector3.Lerp(vd1.v4Tangent, vd2.v4Tangent, fT);
			vertexData.v3Vertex = Vector3.Lerp(vd1.v3Vertex, vd2.v3Vertex, fT);
			if (vd1.bHasNormal)
			{
				vertexData.v3Normal = Vector3.Lerp(vd1.v3Normal, vd2.v3Normal, fT);
			}
			if (vd1.bHasColor32)
			{
				vertexData.color32 = Color32.Lerp(vd1.color32, vd2.color32, fT);
			}
			if (vd1.bHasMapping1)
			{
				vertexData.v2Mapping1 = Vector2.Lerp(vd1.v2Mapping1, vd2.v2Mapping1, fT);
			}
			if (vd1.bHasMapping2)
			{
				vertexData.v2Mapping2 = Vector2.Lerp(vd1.v2Mapping2, vd2.v2Mapping2, fT);
			}
			if (vd1.bHasTangent)
			{
				vertexData.v4Tangent = new Vector4(vector.x, vector.y, vector.z, vd1.v4Tangent.w);
			}
			return vertexData;
		}

		public static bool ClipAgainstPlane(VertexData[] aVertexDataInput, int nIndexA, int nIndexB, Vector3 v3A, Vector3 v3B, Plane planeSplit, ref VertexData clippedVertexDataOut)
		{
			Vector3 normalized = (v3B - v3A).normalized;
			Ray ray = new Ray(v3A, normalized);
			float enter;
			if (!planeSplit.Raycast(ray, out enter))
			{
				Debug.LogWarning("Raycast returned false");
				clippedVertexDataOut = new VertexData(clippedVertexDataOut.nVertexHash);
				return false;
			}
			float fT = enter / (v3B - v3A).magnitude;
			clippedVertexDataOut = Lerp(clippedVertexDataOut.nVertexHash, aVertexDataInput[nIndexA], aVertexDataInput[nIndexB], fT);
			return true;
		}

		public static VertexData[] BuildVertexDataArray(Mesh mesh, Matrix4x4 mtxLocalToWorld, bool bTransformVerticesToWorld)
		{
			VertexData[] array = new VertexData[mesh.vertexCount];
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			Vector4[] tangents = mesh.tangents;
			Vector2[] uv = mesh.uv;
			Vector2[] uv2 = mesh.uv2;
			Color[] colors = mesh.colors;
			Color32[] colors2 = mesh.colors32;
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				array[i] = new VertexData(-1);
				if (bTransformVerticesToWorld)
				{
					array[i].v3Vertex = mtxLocalToWorld.MultiplyPoint3x4(vertices[i]);
					if (normals != null)
					{
						if (normals.Length > i)
						{
							array[i].v3Normal = mtxLocalToWorld.MultiplyVector(normals[i]);
							array[i].bHasNormal = true;
						}
					}
					else
					{
						array[i].bHasNormal = false;
					}
					if (tangents != null)
					{
						if (tangents.Length > i)
						{
							float w = tangents[i].w;
							array[i].v4Tangent = mtxLocalToWorld.MultiplyVector(tangents[i]);
							array[i].v4Tangent.w = w;
							array[i].bHasTangent = true;
						}
					}
					else
					{
						array[i].bHasTangent = false;
					}
				}
				else
				{
					array[i].v3Vertex = vertices[i];
					if (normals != null)
					{
						if (normals.Length > i)
						{
							array[i].v3Normal = normals[i];
							array[i].bHasNormal = true;
						}
					}
					else
					{
						array[i].bHasNormal = false;
					}
					if (tangents != null)
					{
						if (tangents.Length > i)
						{
							array[i].v4Tangent = tangents[i];
							array[i].bHasTangent = true;
						}
					}
					else
					{
						array[i].bHasTangent = false;
					}
				}
				if (colors2 != null)
				{
					if (colors2.Length > i)
					{
						array[i].color32 = colors2[i];
						array[i].bHasColor32 = true;
					}
				}
				else if (colors != null)
				{
					if (colors.Length > i)
					{
						array[i].color32 = colors[i];
						array[i].bHasColor32 = true;
					}
				}
				else
				{
					array[i].bHasColor32 = false;
				}
				if (uv != null)
				{
					if (uv.Length > i)
					{
						array[i].v2Mapping1 = uv[i];
						array[i].bHasMapping1 = true;
					}
				}
				else
				{
					array[i].bHasMapping1 = false;
				}
				if (uv2 != null)
				{
					if (uv2.Length > i)
					{
						array[i].v2Mapping2 = uv2[i];
						array[i].bHasMapping2 = true;
					}
				}
				else
				{
					array[i].bHasMapping2 = false;
				}
			}
			return array;
		}

		public static void SetMeshDataFromVertexDataArray(MeshFilter meshFilter, MeshData meshData, bool bTransformVertexToLocal)
		{
			VertexData[] aVertexData = meshData.aVertexData;
			Vector3[] array = new Vector3[aVertexData.Length];
			Vector3[] array2 = (aVertexData[0].bHasNormal ? new Vector3[aVertexData.Length] : null);
			Vector4[] array3 = (aVertexData[0].bHasTangent ? new Vector4[aVertexData.Length] : null);
			Color32[] array4 = (aVertexData[0].bHasColor32 ? new Color32[aVertexData.Length] : null);
			Vector2[] array5 = (aVertexData[0].bHasMapping1 ? new Vector2[aVertexData.Length] : null);
			Vector2[] array6 = (aVertexData[0].bHasMapping2 ? new Vector2[aVertexData.Length] : null);
			Matrix4x4 worldToLocalMatrix = meshFilter.transform.worldToLocalMatrix;
			for (int i = 0; i < aVertexData.Length; i++)
			{
				if (bTransformVertexToLocal)
				{
					array[i] = worldToLocalMatrix.MultiplyPoint(aVertexData[i].v3Vertex);
					if (array2 != null)
					{
						array2[i] = worldToLocalMatrix.MultiplyVector(aVertexData[i].v3Normal);
					}
					if (array3 != null)
					{
						float w = aVertexData[i].v4Tangent.w;
						array3[i] = worldToLocalMatrix.MultiplyVector(aVertexData[i].v4Tangent);
						array3[i].w = w;
					}
				}
				else
				{
					array[i] = aVertexData[i].v3Vertex;
					if (array2 != null)
					{
						array2[i] = aVertexData[i].v3Normal;
					}
					if (array3 != null)
					{
						array3[i] = aVertexData[i].v4Tangent;
					}
				}
				if (array4 != null)
				{
					array4[i] = aVertexData[i].color32;
				}
				if (array5 != null)
				{
					array5[i] = aVertexData[i].v2Mapping1;
				}
				if (array6 != null)
				{
					array6[i] = aVertexData[i].v2Mapping2;
				}
			}
			meshFilter.sharedMesh.vertices = array;
			meshFilter.sharedMesh.normals = array2;
			meshFilter.sharedMesh.tangents = array3;
			meshFilter.sharedMesh.colors32 = array4;
			meshFilter.sharedMesh.uv = array5;
			meshFilter.sharedMesh.uv2 = array6;
		}
	}
}
