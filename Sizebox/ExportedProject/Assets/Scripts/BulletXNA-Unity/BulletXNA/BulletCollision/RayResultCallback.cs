namespace BulletXNA.BulletCollision
{
	public abstract class RayResultCallback
	{
		public float m_closestHitFraction;

		public CollisionObject m_collisionObject;

		public CollisionFilterGroups m_collisionFilterGroup;

		public CollisionFilterGroups m_collisionFilterMask;

		public EFlags m_flags;

		public bool HasHit()
		{
			return m_collisionObject != null;
		}

		public RayResultCallback()
		{
			m_closestHitFraction = 1f;
			m_collisionObject = null;
			m_collisionFilterGroup = CollisionFilterGroups.DefaultFilter;
			m_collisionFilterMask = CollisionFilterGroups.AllFilter;
			m_flags = EFlags.kF_None;
		}

		public virtual bool NeedsCollision(BroadphaseProxy proxy0)
		{
			return (proxy0.m_collisionFilterGroup & m_collisionFilterMask) != 0 && (m_collisionFilterGroup & proxy0.m_collisionFilterMask) != 0;
		}

		public abstract float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace);

		public virtual void Cleanup()
		{
		}
	}
}
