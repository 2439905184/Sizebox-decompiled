namespace BulletXNA.BulletCollision
{
	public struct CollisionAlgorithmConstructionInfo
	{
		private IDispatcher m_dispatcher1;

		private PersistentManifold m_manifold;

		public static CollisionAlgorithmConstructionInfo Default()
		{
			return new CollisionAlgorithmConstructionInfo(null, 0);
		}

		public CollisionAlgorithmConstructionInfo(IDispatcher dispatcher, int temp)
		{
			m_dispatcher1 = dispatcher;
			m_manifold = null;
		}

		public void SetManifold(PersistentManifold manifold)
		{
			m_manifold = manifold;
		}

		public PersistentManifold GetManifold()
		{
			return m_manifold;
		}

		public IDispatcher GetDispatcher()
		{
			return m_dispatcher1;
		}

		public void SetDispatcher(IDispatcher dispatcher)
		{
			m_dispatcher1 = dispatcher;
		}
	}
}
