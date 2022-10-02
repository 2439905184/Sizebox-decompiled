using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MinkowskiIntermediateResult : IDiscreteCollisionDetectorInterfaceResult
	{
		public IndexedVector3 m_normalOnBInWorld;

		public IndexedVector3 m_pointInWorld;

		public float m_depth;

		public bool m_hasResult;

		public MinkowskiIntermediateResult()
		{
			m_hasResult = false;
		}

		public virtual void SetShapeIdentifiersA(int partId0, int index0)
		{
		}

		public virtual void SetShapeIdentifiersB(int partId1, int index1)
		{
		}

		public void AddContactPoint(IndexedVector3 normalOnBInWorld, IndexedVector3 pointInWorld, float depth)
		{
			AddContactPoint(ref normalOnBInWorld, ref pointInWorld, depth);
		}

		public void AddContactPoint(ref IndexedVector3 normalOnBInWorld, ref IndexedVector3 pointInWorld, float depth)
		{
			m_normalOnBInWorld = normalOnBInWorld;
			m_pointInWorld = pointInWorld;
			m_depth = depth;
			m_hasResult = true;
		}
	}
}
