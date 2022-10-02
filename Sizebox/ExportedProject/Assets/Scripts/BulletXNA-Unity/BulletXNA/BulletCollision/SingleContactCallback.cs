namespace BulletXNA.BulletCollision
{
	public class SingleContactCallback : IBroadphaseAabbCallback
	{
		private CollisionObject m_collisionObject;

		private CollisionWorld m_world;

		private ContactResultCallback m_resultCallback;

		public SingleContactCallback(CollisionObject collisionObject, CollisionWorld world, ContactResultCallback resultCallback)
		{
			m_collisionObject = collisionObject;
			m_world = world;
			m_resultCallback = resultCallback;
		}

		public virtual void Cleanup()
		{
		}

		public virtual bool Process(BroadphaseProxy proxy)
		{
			if (proxy.m_clientObject == m_collisionObject)
			{
				return true;
			}
			CollisionObject collisionObject = proxy.m_clientObject as CollisionObject;
			if (m_resultCallback.NeedsCollision(collisionObject.GetBroadphaseHandle()))
			{
				CollisionAlgorithm collisionAlgorithm = m_world.GetDispatcher().FindAlgorithm(m_collisionObject, collisionObject);
				if (collisionAlgorithm != null)
				{
					BridgedManifoldResult resultOut = new BridgedManifoldResult(m_collisionObject, collisionObject, m_resultCallback);
					collisionAlgorithm.ProcessCollision(m_collisionObject, collisionObject, m_world.GetDispatchInfo(), resultOut);
					m_world.GetDispatcher().FreeCollisionAlgorithm(collisionAlgorithm);
				}
			}
			return true;
		}
	}
}
