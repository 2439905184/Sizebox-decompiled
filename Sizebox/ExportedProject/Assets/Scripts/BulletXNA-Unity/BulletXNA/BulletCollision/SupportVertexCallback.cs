using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SupportVertexCallback : ITriangleCallback
	{
		private IndexedVector3 m_supportVertexLocal;

		public IndexedMatrix m_worldTrans;

		public float m_maxDot;

		public IndexedVector3 m_supportVecLocal;

		public virtual bool graphics()
		{
			return false;
		}

		public SupportVertexCallback(ref IndexedVector3 supportVecWorld, ref IndexedMatrix trans)
		{
			m_supportVertexLocal = IndexedVector3.Zero;
			m_worldTrans = trans;
			m_maxDot = -1E+18f;
			m_supportVecLocal = supportVecWorld * m_worldTrans._basis;
		}

		public virtual void ProcessTriangle(IndexedVector3[] triangle, int partId, int triangleIndex)
		{
			for (int i = 0; i < 3; i++)
			{
				float num = IndexedVector3.Dot(ref m_supportVecLocal, ref triangle[i]);
				if (num > m_maxDot)
				{
					m_maxDot = num;
					m_supportVertexLocal = triangle[i];
				}
			}
		}

		public IndexedVector3 GetSupportVertexWorldSpace()
		{
			return m_worldTrans * m_supportVertexLocal;
		}

		public IndexedVector3 GetSupportVertexLocal()
		{
			return m_supportVertexLocal;
		}

		public virtual void Cleanup()
		{
		}
	}
}
