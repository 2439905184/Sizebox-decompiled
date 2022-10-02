namespace BulletXNA.BulletCollision
{
	public abstract class ConvexResultCallback
	{
		public float m_closestHitFraction;

		public CollisionFilterGroups m_collisionFilterGroup;

		public CollisionFilterGroups m_collisionFilterMask;

		public ConvexResultCallback()
		{
			m_closestHitFraction = 1f;
			m_collisionFilterGroup = CollisionFilterGroups.DefaultFilter;
			m_collisionFilterMask = CollisionFilterGroups.AllFilter;
		}

		public virtual void Initialize()
		{
			m_closestHitFraction = 1f;
			m_collisionFilterGroup = CollisionFilterGroups.DefaultFilter;
			m_collisionFilterMask = CollisionFilterGroups.AllFilter;
		}

		public bool HasHit()
		{
			return m_closestHitFraction < 1f;
		}

		public virtual bool NeedsCollision(BroadphaseProxy proxy0)
		{
			return (proxy0.m_collisionFilterGroup & m_collisionFilterMask) != 0 && (m_collisionFilterGroup & proxy0.m_collisionFilterMask) != 0;
		}

		public abstract float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace);
	}
}
