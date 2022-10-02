using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class AabbCalculationCallback : IInternalTriangleIndexCallback
	{
		public IndexedVector3 m_aabbMin;

		public IndexedVector3 m_aabbMax;

		public AabbCalculationCallback()
		{
			m_aabbMin = MathUtil.MAX_VECTOR;
			m_aabbMax = MathUtil.MIN_VECTOR;
		}

		public virtual bool graphics()
		{
			return false;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			MathUtil.VectorMin(ref triangle[0], ref m_aabbMin);
			MathUtil.VectorMax(ref triangle[0], ref m_aabbMax);
			MathUtil.VectorMin(ref triangle[1], ref m_aabbMin);
			MathUtil.VectorMax(ref triangle[1], ref m_aabbMax);
			MathUtil.VectorMin(ref triangle[2], ref m_aabbMin);
			MathUtil.VectorMax(ref triangle[2], ref m_aabbMax);
		}

		public void Cleanup()
		{
		}
	}
}
