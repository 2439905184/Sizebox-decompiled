using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class PointCollector : IDiscreteCollisionDetectorInterfaceResult
	{
		public IndexedVector3 m_normalOnBInWorld;

		public IndexedVector3 m_pointInWorld;

		public float m_distance;

		public bool m_hasResult;

		public PointCollector()
		{
			m_distance = float.MaxValue;
			m_hasResult = false;
		}

		public virtual void SetShapeIdentifiersA(int partId0, int index0)
		{
		}

		public virtual void SetShapeIdentifiersB(int partId1, int index1)
		{
		}

		public virtual void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth)
		{
			AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
		}

		public virtual void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
			if (depth < m_distance)
			{
				m_hasResult = true;
				m_normalOnBInWorld = normalOnBInWorld;
				m_pointInWorld = pointInWorld;
				m_distance = depth;
			}
		}
	}
}
