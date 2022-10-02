using System.Collections.Generic;

namespace UltimateFracturing
{
	public class MeshDataConnectivity
	{
		public struct Face
		{
			public class EqualityComparer : IEqualityComparer<Face>
			{
				public bool Equals(Face x, Face y)
				{
					if (x.nSubMesh == y.nSubMesh)
					{
						return x.nFaceIndex == y.nFaceIndex;
					}
					return false;
				}

				public int GetHashCode(Face x)
				{
					return x.nSubMesh.GetHashCode() + x.nFaceIndex.GetHashCode();
				}
			}

			public int nSubMesh;

			public int nFaceIndex;

			public Face(int nSubMesh, int nFaceIndex)
			{
				this.nSubMesh = nSubMesh;
				this.nFaceIndex = nFaceIndex;
			}
		}

		public static int s_CurrentSharedFaceHash;

		public Dictionary<int, List<Face>> dicHash2FaceList;

		public Dictionary<Face, List<int>> dicFace2HashList;

		public Dictionary<Face, bool> dicFace2IsClipped;

		public MeshDataConnectivity()
		{
			dicHash2FaceList = new Dictionary<int, List<Face>>();
			dicFace2HashList = new Dictionary<Face, List<int>>(new Face.EqualityComparer());
			dicFace2IsClipped = new Dictionary<Face, bool>(new Face.EqualityComparer());
		}

		public MeshDataConnectivity GetDeepCopy()
		{
			MeshDataConnectivity meshDataConnectivity = new MeshDataConnectivity();
			foreach (KeyValuePair<int, List<Face>> dicHash2Face in dicHash2FaceList)
			{
				meshDataConnectivity.dicHash2FaceList.Add(dicHash2Face.Key, new List<Face>());
				foreach (Face item in dicHash2Face.Value)
				{
					meshDataConnectivity.dicHash2FaceList[dicHash2Face.Key].Add(item);
				}
			}
			foreach (KeyValuePair<Face, List<int>> dicFace2Hash in dicFace2HashList)
			{
				meshDataConnectivity.dicFace2HashList.Add(dicFace2Hash.Key, new List<int>());
				foreach (int item2 in dicFace2Hash.Value)
				{
					meshDataConnectivity.dicFace2HashList[dicFace2Hash.Key].Add(item2);
				}
			}
			foreach (KeyValuePair<Face, bool> item3 in dicFace2IsClipped)
			{
				meshDataConnectivity.dicFace2IsClipped.Add(item3.Key, item3.Value);
			}
			return meshDataConnectivity;
		}

		public void NotifyNewClippedFace(MeshData meshDataSource, int nSourceSubMesh, int nSourceFaceIndex, int nDestSubMesh, int nDestFaceIndex)
		{
			Face key = new Face(nSourceSubMesh, nSourceFaceIndex);
			Face face = new Face(nDestSubMesh, nDestFaceIndex);
			if (!meshDataSource.meshDataConnectivity.dicFace2HashList.ContainsKey(key))
			{
				return;
			}
			foreach (int item in meshDataSource.meshDataConnectivity.dicFace2HashList[key])
			{
				if (!dicHash2FaceList.ContainsKey(item))
				{
					dicHash2FaceList.Add(item, new List<Face>());
				}
				if (!dicFace2HashList.ContainsKey(face))
				{
					dicFace2HashList.Add(face, new List<int>());
				}
				dicHash2FaceList[item].Add(face);
				dicFace2HashList[face].Add(item);
				if (!dicFace2IsClipped.ContainsKey(face))
				{
					dicFace2IsClipped.Add(face, true);
				}
				else
				{
					dicFace2IsClipped[face] = true;
				}
			}
		}

		public static int GetNewHash()
		{
			int num = 0;
			lock (typeof(MeshDataConnectivity))
			{
				num = s_CurrentSharedFaceHash;
				s_CurrentSharedFaceHash++;
				return num;
			}
		}

		public void NotifyNewCapFace(int nHash, int nSubMesh, int nFaceIndex)
		{
			Face face = new Face(nSubMesh, nFaceIndex);
			if (!dicHash2FaceList.ContainsKey(nHash))
			{
				dicHash2FaceList.Add(nHash, new List<Face>());
			}
			dicHash2FaceList[nHash].Add(face);
			if (!dicFace2HashList.ContainsKey(face))
			{
				dicFace2HashList.Add(face, new List<int>());
			}
			dicFace2HashList[face].Add(nHash);
		}

		public void NotifyRemappedFace(MeshDataConnectivity source, int nSourceSubMesh, int nSourceFaceIndex, int nDestSubMesh, int nDestFaceIndex)
		{
			Face key = new Face(nSourceSubMesh, nSourceFaceIndex);
			Face face = new Face(nDestSubMesh, nDestFaceIndex);
			if (!source.dicFace2HashList.ContainsKey(key))
			{
				return;
			}
			foreach (int item in source.dicFace2HashList[key])
			{
				if (!dicHash2FaceList.ContainsKey(item))
				{
					dicHash2FaceList.Add(item, new List<Face>());
				}
				if (!dicFace2HashList.ContainsKey(face))
				{
					dicFace2HashList.Add(face, new List<int>());
				}
				dicHash2FaceList[item].Add(face);
				dicFace2HashList[face].Add(item);
				if (source.dicFace2IsClipped.ContainsKey(key))
				{
					if (!dicFace2IsClipped.ContainsKey(face))
					{
						dicFace2IsClipped.Add(face, true);
					}
					else
					{
						dicFace2IsClipped[face] = true;
					}
				}
			}
		}
	}
}
