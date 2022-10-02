using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class FilteredCallback : IInternalTriangleIndexCallback
	{
		public ITriangleCallback m_callback;

		public IndexedVector3 m_aabbMin;

		public IndexedVector3 m_aabbMax;

		public FilteredCallback(ITriangleCallback callback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			m_callback = callback;
			m_aabbMin = aabbMin;
			m_aabbMax = aabbMax;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			if (AabbUtil2.TestTriangleAgainstAabb2(triangle, ref m_aabbMin, ref m_aabbMax))
			{
				m_callback.ProcessTriangle(triangle, partId, triangleIndex);
			}
		}

		public virtual void Cleanup()
		{
		}
	}
}
