using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class PHullResult
	{
		public int mVcount;

		public int mIndexCount;

		public int mFaceCount;

		public IList<IndexedVector3> mVertices = new List<IndexedVector3>();

		public IList<int> m_Indices = new List<int>();

		public PHullResult()
		{
			mVcount = 0;
			mIndexCount = 0;
			mFaceCount = 0;
		}
	}
}
