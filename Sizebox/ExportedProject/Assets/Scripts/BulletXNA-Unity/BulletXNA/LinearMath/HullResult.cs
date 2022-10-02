using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class HullResult
	{
		public bool mPolygons;

		public int mNumOutputVertices;

		public IList<IndexedVector3> m_OutputVertices = new List<IndexedVector3>();

		public int mNumFaces;

		public int mNumIndices;

		public IList<int> m_Indices = new List<int>();

		public HullResult()
		{
			mPolygons = true;
			mNumOutputVertices = 0;
			mNumFaces = 0;
			mNumIndices = 0;
		}
	}
}
