using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class LocalSupportVertexCallback : IInternalTriangleIndexCallback
	{
		private IndexedVector3 m_supportVertexLocal;

		public float m_maxDot;

		public IndexedVector3 m_supportVecLocal;

		public virtual bool graphics()
		{
			return false;
		}

		public LocalSupportVertexCallback(ref IndexedVector3 supportVecLocal)
		{
			m_supportVertexLocal = IndexedVector3.Zero;
			m_supportVecLocal = supportVecLocal;
			m_maxDot = float.MinValue;
		}

		public virtual void InternalProcessTriangleIndex(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			for (int i = 0; i < 3; i++)
			{
				float num = m_supportVecLocal.Dot(ref triangle[i]);
				if (num > m_maxDot)
				{
					m_maxDot = num;
					m_supportVertexLocal = triangle[i];
				}
			}
		}

		public IndexedVector3 GetSupportVertexLocal()
		{
			return m_supportVertexLocal;
		}

		public void Cleanup()
		{
		}
	}
}
