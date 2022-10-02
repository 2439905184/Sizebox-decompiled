namespace BulletXNA.BulletCollision
{
	public class CollisionPairCallback : IOverlapCallback
	{
		private DispatcherInfo m_dispatchInfo;

		private CollisionDispatcher m_dispatcher;

		public CollisionPairCallback(DispatcherInfo dispatchInfo, CollisionDispatcher dispatcher)
		{
			m_dispatchInfo = dispatchInfo;
			m_dispatcher = dispatcher;
		}

		public void Initialize(DispatcherInfo dispatchInfo, CollisionDispatcher dispatcher)
		{
			m_dispatchInfo = dispatchInfo;
			m_dispatcher = dispatcher;
		}

		public virtual void cleanup()
		{
		}

		public virtual bool ProcessOverlap(BroadphasePair pair)
		{
			m_dispatcher.GetNearCallback().NearCallback(pair, m_dispatcher, m_dispatchInfo);
			return false;
		}
	}
}
