namespace BulletXNA.BulletCollision
{
	public abstract class ContactResultCallback
	{
		public CollisionFilterGroups m_collisionFilterGroup;

		public CollisionFilterGroups m_collisionFilterMask;

		public ContactResultCallback()
		{
			m_collisionFilterGroup = CollisionFilterGroups.DefaultFilter;
			m_collisionFilterMask = CollisionFilterGroups.AllFilter;
		}

		public virtual bool NeedsCollision(BroadphaseProxy proxy0)
		{
			return (proxy0.m_collisionFilterGroup & m_collisionFilterMask) != 0 && (m_collisionFilterGroup & proxy0.m_collisionFilterMask) != 0;
		}

		public abstract float AddSingleResult(ref ManifoldPoint cp, CollisionObject colObj0, int partId0, int index0, CollisionObject colObj1, int partId1, int index1);
	}
}
