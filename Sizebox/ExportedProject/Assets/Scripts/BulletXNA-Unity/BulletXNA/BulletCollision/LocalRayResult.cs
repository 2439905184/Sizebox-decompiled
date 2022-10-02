using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct LocalRayResult
	{
		public CollisionObject m_collisionObject;

		public LocalShapeInfo m_localShapeInfo;

		public IndexedVector3 m_hitNormalLocal;

		public float m_hitFraction;

		public LocalRayResult(CollisionObject collisionObject, ref LocalShapeInfo localShapeInfo, ref IndexedVector3 hitNormalLocal, float hitFraction)
		{
			m_collisionObject = collisionObject;
			m_localShapeInfo = localShapeInfo;
			m_hitNormalLocal = hitNormalLocal;
			m_hitFraction = hitFraction;
		}

		public LocalRayResult(CollisionObject collisionObject, ref IndexedVector3 hitNormalLocal, float hitFraction)
		{
			m_collisionObject = collisionObject;
			m_hitNormalLocal = hitNormalLocal;
			m_hitFraction = hitFraction;
			m_localShapeInfo = default(LocalShapeInfo);
		}
	}
}
