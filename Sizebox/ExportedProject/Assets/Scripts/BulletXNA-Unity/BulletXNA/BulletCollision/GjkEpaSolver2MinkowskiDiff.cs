using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GjkEpaSolver2MinkowskiDiff : IDisposable
	{
		public bool m_enableMargin;

		public ConvexShape[] m_shapes = new ConvexShape[2];

		public IndexedBasisMatrix m_toshape1 = IndexedBasisMatrix.Identity;

		public IndexedMatrix m_toshape0 = IndexedMatrix.Identity;

		public void EnableMargin(bool enable)
		{
			m_enableMargin = enable;
		}

		public IndexedVector3 Support0(ref IndexedVector3 d)
		{
			if (m_enableMargin)
			{
				return m_shapes[0].LocalGetSupportVertexNonVirtual(ref d);
			}
			return m_shapes[0].LocalGetSupportVertexWithoutMarginNonVirtual(ref d);
		}

		public IndexedVector3 Support1(ref IndexedVector3 d)
		{
			IndexedVector3 localDir = m_toshape1 * d;
			IndexedVector3 indexedVector = (m_enableMargin ? m_shapes[1].LocalGetSupportVertexNonVirtual(ref localDir) : m_shapes[1].LocalGetSupportVertexWithoutMarginNonVirtual(ref localDir));
			return m_toshape0 * indexedVector;
		}

		public IndexedVector3 Support(ref IndexedVector3 d)
		{
			IndexedVector3 d2 = -d;
			IndexedVector3 indexedVector = Support1(ref d2);
			return Support0(ref d) - indexedVector;
		}

		public IndexedVector3 Support(ref IndexedVector3 d, uint index)
		{
			if (index != 0)
			{
				return Support1(ref d);
			}
			return Support0(ref d);
		}

		public virtual void Dispose()
		{
			BulletGlobals.GjkEpaSolver2MinkowskiDiffPool.Free(this);
		}
	}
}
