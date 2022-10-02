using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class StorageResult : IDiscreteCollisionDetectorInterfaceResult
	{
		private IndexedVector3 m_normalOnSurfaceB;

		private IndexedVector3 m_closestPointInB;

		private float m_distance = 1E+18f;

		public StorageResult()
		{
			m_distance = float.MaxValue;
		}

		public virtual void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth)
		{
			AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
		}

		public virtual void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
			if (depth < m_distance)
			{
				m_normalOnSurfaceB = normalOnBInWorld;
				m_closestPointInB = pointInWorld;
				m_distance = depth;
			}
		}

		public virtual void SetShapeIdentifiersA(int partId0, int index0)
		{
		}

		public virtual void SetShapeIdentifiersB(int partId1, int index1)
		{
		}
	}
}
