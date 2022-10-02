using System.Collections.Generic;
using UnityEngine;

namespace UltimateFracturing
{
	public class MeshFaceConnectivity
	{
		public struct EdgeData
		{
			public struct SideData
			{
				public int nFace;

				public int nSubMesh;

				public int nSubMeshFace;

				public int nEdgePos;

				public int nVertexDataV1;

				public int nVertexDataV2;

				public SideData(int nFace, int nSubMesh, int nSubMeshFace, int nEdgePos, int nVertexDataV1, int nVertexDataV2)
				{
					this.nFace = nFace;
					this.nSubMesh = nSubMesh;
					this.nSubMeshFace = nSubMeshFace;
					this.nEdgePos = nEdgePos;
					this.nVertexDataV1 = nVertexDataV1;
					this.nVertexDataV2 = nVertexDataV2;
				}
			}

			public int nEdgeIndex;

			private int nVertex1Hash;

			private int nVertex2Hash;

			public Vector3 v1;

			public Vector3 v2;

			public List<SideData> listSides;

			public EdgeData(int nEdgeIndex, int nFace, int nSubMesh, int nSubMeshFace, int nEdgePos, Vector3 v1, Vector3 v2, int nVertex1Hash, int nVertex2Hash, int nVertexDataV1, int nVertexDataV2)
			{
				this.nEdgeIndex = nEdgeIndex;
				this.v1 = v1;
				this.v2 = v2;
				this.nVertex1Hash = nVertex1Hash;
				this.nVertex2Hash = nVertex2Hash;
				listSides = new List<SideData>();
				listSides.Add(new SideData(nFace, nSubMesh, nSubMeshFace, nEdgePos, nVertexDataV1, nVertexDataV2));
			}

			public bool Compare(int nVertex1Hash, int nVertex2Hash)
			{
				if (this.nVertex1Hash == nVertex1Hash && this.nVertex2Hash == nVertex2Hash)
				{
					return true;
				}
				if (this.nVertex1Hash == nVertex2Hash && this.nVertex2Hash == nVertex1Hash)
				{
					return true;
				}
				return false;
			}

			public void AddSideData(int nFace, int nSubMesh, int nSubMeshFace, int nEdgePos, int nVertexDataV1, int nVertexDataV2)
			{
				if (listSides == null)
				{
					listSides = new List<SideData>();
				}
				listSides.Add(new SideData(nFace, nSubMesh, nSubMeshFace, nEdgePos, nVertexDataV1, nVertexDataV2));
			}

			public bool HasMoreThanOneSide()
			{
				if (listSides == null)
				{
					return false;
				}
				return listSides.Count > 1;
			}
		}

		public class TriangleData
		{
			public int nSubMesh;

			public int nTriangle;

			public int[] anEdges;

			public List<int>[] alistNeighborSubMeshes;

			public List<int>[] alistNeighborTriangles;

			public bool bVisited;

			public TriangleData(int nSubMesh, int nTriangle)
			{
				this.nSubMesh = nSubMesh;
				this.nTriangle = nTriangle;
				anEdges = new int[3];
				alistNeighborSubMeshes = new List<int>[3];
				alistNeighborTriangles = new List<int>[3];
				for (int i = 0; i < 3; i++)
				{
					anEdges[i] = -1;
					alistNeighborSubMeshes[i] = new List<int>();
					alistNeighborTriangles[i] = new List<int>();
				}
				bVisited = false;
			}
		}

		public List<TriangleData> listTriangles;

		private List<EdgeData> listEdges;

		private List<int> listEdgeIndices;

		private Dictionary<EdgeKeyByHash, EdgeData> dicEdges;

		private int nEdgeCount;

		private Dictionary<int, int> dicSubMeshTriangleCount;

		public MeshFaceConnectivity()
		{
			listTriangles = new List<TriangleData>();
			listEdges = new List<EdgeData>();
			listEdgeIndices = new List<int>();
			dicEdges = new Dictionary<EdgeKeyByHash, EdgeData>(new EdgeKeyByHash.EqualityComparer());
			dicSubMeshTriangleCount = new Dictionary<int, int>();
			nEdgeCount = 0;
		}

		public void Clear()
		{
			listTriangles.Clear();
			listEdges.Clear();
			listEdgeIndices.Clear();
			dicEdges.Clear();
			dicSubMeshTriangleCount.Clear();
			nEdgeCount = 0;
		}

		public void ResetVisited()
		{
			for (int i = 0; i < listTriangles.Count; i++)
			{
				listTriangles[i].bVisited = false;
			}
		}

		public void AddEdge(int nSubMesh, Vector3 v1, Vector3 v2, int nVertex1Hash, int nVertex2Hash, int nVertexDataIndex1, int nVertexDataIndex2)
		{
			int nEdgePos = nEdgeCount % 3;
			int num = nEdgeCount / 3;
			if (!dicSubMeshTriangleCount.ContainsKey(nSubMesh))
			{
				dicSubMeshTriangleCount.Add(nSubMesh, 0);
			}
			int num2 = dicSubMeshTriangleCount[nSubMesh];
			int num3 = -1;
			EdgeKeyByHash key = new EdgeKeyByHash(nVertex1Hash, nVertex2Hash);
			if (dicEdges.ContainsKey(key))
			{
				EdgeData edgeData = dicEdges[key];
				edgeData.AddSideData(num, nSubMesh, num2, nEdgePos, nVertexDataIndex1, nVertexDataIndex2);
				num3 = edgeData.nEdgeIndex;
			}
			else
			{
				num3 = listEdges.Count;
				EdgeData edgeData2 = new EdgeData(num3, num, nSubMesh, num2, nEdgePos, v1, v2, nVertex1Hash, nVertex2Hash, nVertexDataIndex1, nVertexDataIndex2);
				listEdges.Add(edgeData2);
				dicEdges.Add(key, edgeData2);
			}
			listEdgeIndices.Add(num3);
			nEdgeCount++;
			if (nEdgeCount % 3 != 0)
			{
				return;
			}
			TriangleData triangleData = new TriangleData(nSubMesh, num2);
			EdgeData edgeData3 = listEdges[listEdgeIndices[listEdgeIndices.Count - 3]];
			EdgeData edgeData4 = listEdges[listEdgeIndices[listEdgeIndices.Count - 2]];
			EdgeData edgeData5 = listEdges[listEdgeIndices[listEdgeIndices.Count - 1]];
			triangleData.anEdges[0] = edgeData3.nEdgeIndex;
			triangleData.anEdges[1] = edgeData4.nEdgeIndex;
			triangleData.anEdges[2] = edgeData5.nEdgeIndex;
			foreach (EdgeData.SideData listSide in edgeData3.listSides)
			{
				if (listSide.nFace != num)
				{
					listTriangles[listSide.nFace].alistNeighborSubMeshes[listSide.nEdgePos].Add(nSubMesh);
					listTriangles[listSide.nFace].alistNeighborTriangles[listSide.nEdgePos].Add(num2);
					triangleData.alistNeighborSubMeshes[0].Add(listSide.nSubMesh);
					triangleData.alistNeighborTriangles[0].Add(listSide.nSubMeshFace);
				}
			}
			foreach (EdgeData.SideData listSide2 in edgeData4.listSides)
			{
				if (listSide2.nFace != num)
				{
					listTriangles[listSide2.nFace].alistNeighborSubMeshes[listSide2.nEdgePos].Add(nSubMesh);
					listTriangles[listSide2.nFace].alistNeighborTriangles[listSide2.nEdgePos].Add(num2);
					triangleData.alistNeighborSubMeshes[1].Add(listSide2.nSubMesh);
					triangleData.alistNeighborTriangles[1].Add(listSide2.nSubMeshFace);
				}
			}
			foreach (EdgeData.SideData listSide3 in edgeData5.listSides)
			{
				if (listSide3.nFace != num)
				{
					listTriangles[listSide3.nFace].alistNeighborSubMeshes[listSide3.nEdgePos].Add(nSubMesh);
					listTriangles[listSide3.nFace].alistNeighborTriangles[listSide3.nEdgePos].Add(num2);
					triangleData.alistNeighborSubMeshes[2].Add(listSide3.nSubMesh);
					triangleData.alistNeighborTriangles[2].Add(listSide3.nSubMeshFace);
				}
			}
			listTriangles.Add(triangleData);
			dicSubMeshTriangleCount[nSubMesh]++;
		}
	}
}
