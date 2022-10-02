using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct LocalConvexResult
	{
		public CollisionObject m_hitCollisionObject;

		public LocalShapeInfo m_localShapeInfo;

		public IndexedVector3 m_hitNormalLocal;

		public IndexedVector3 m_hitPointLocal;

		public float m_hitFraction;

		public LocalConvexResult(CollisionObject hitCollisionObject, ref LocalShapeInfo localShapeInfo, ref IndexedVector3 hitNormalLocal, ref IndexedVector3 hitPointLocal, float hitFraction)
		{
			m_hitCollisionObject = hitCollisionObject;
			m_localShapeInfo = localShapeInfo;
			m_hitNormalLocal = hitNormalLocal;
			m_hitPointLocal = hitPointLocal;
			m_hitFraction = hitFraction;
		}

		public LocalConvexResult(CollisionObject hitCollisionObject, ref IndexedVector3 hitNormalLocal, ref IndexedVector3 hitPointLocal, float hitFraction)
		{
			m_hitCollisionObject = hitCollisionObject;
			m_hitNormalLocal = hitNormalLocal;
			m_hitPointLocal = hitPointLocal;
			m_hitFraction = hitFraction;
			m_localShapeInfo = default(LocalShapeInfo);
		}
	}
}
