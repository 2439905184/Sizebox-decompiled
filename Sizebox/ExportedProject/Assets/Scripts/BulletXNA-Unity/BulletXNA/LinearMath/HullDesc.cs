using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class HullDesc
	{
		public HullFlag mFlags;

		public int mVcount;

		public IList<IndexedVector3> mVertices = new List<IndexedVector3>();

		public int mVertexStride;

		public float mNormalEpsilon;

		public int mMaxVertices;

		public int mMaxFaces;

		public HullDesc()
		{
			mFlags = HullFlag.QF_TRIANGLES;
			mVcount = 0;
			mNormalEpsilon = 0.001f;
			mMaxVertices = 4096;
			mMaxFaces = 4096;
		}

		public HullDesc(HullFlag flag, int vcount, IList<IndexedVector3> vertices)
		{
			mFlags = flag;
			mVcount = vcount;
			mVertices = vertices;
			mNormalEpsilon = 0.001f;
			mMaxVertices = 4096;
		}

		public bool HasHullFlag(HullFlag flag)
		{
			return (mFlags & flag) != 0;
		}

		private void SetHullFlag(HullFlag flag)
		{
			mFlags |= flag;
		}

		private void ClearHullFlag(HullFlag flag)
		{
			mFlags &= ~flag;
		}
	}
}
