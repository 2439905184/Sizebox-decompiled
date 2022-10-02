using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ClosestConvexResultCallback : ConvexResultCallback
	{
		public IndexedVector3 m_convexFromWorld;

		public IndexedVector3 m_convexToWorld;

		public IndexedVector3 m_hitNormalWorld;

		public IndexedVector3 m_hitPointWorld;

		public CollisionObject m_hitCollisionObject;

		public ClosestConvexResultCallback()
		{
		}

		public ClosestConvexResultCallback(IndexedVector3 convexFromWorld, IndexedVector3 convexToWorld)
			: this(ref convexFromWorld, ref convexToWorld)
		{
		}

		public ClosestConvexResultCallback(ref IndexedVector3 convexFromWorld, ref IndexedVector3 convexToWorld)
		{
			m_convexFromWorld = convexFromWorld;
			m_convexToWorld = convexToWorld;
			m_hitCollisionObject = null;
		}

		public virtual void Initialize(ref IndexedVector3 convexFromWorld, ref IndexedVector3 convexToWorld)
		{
			base.Initialize();
			m_convexFromWorld = convexFromWorld;
			m_convexToWorld = convexToWorld;
			m_hitCollisionObject = null;
		}

		public override float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace)
		{
			m_closestHitFraction = convexResult.m_hitFraction;
			m_hitCollisionObject = convexResult.m_hitCollisionObject;
			if (normalInWorldSpace)
			{
				m_hitNormalWorld = convexResult.m_hitNormalLocal;
			}
			else
			{
				m_hitNormalWorld = m_hitCollisionObject.GetWorldTransform()._basis * convexResult.m_hitNormalLocal;
			}
			m_hitPointWorld = convexResult.m_hitPointLocal;
			return convexResult.m_hitFraction;
		}
	}
}
