namespace BulletXNA.BulletCollision
{
	public class PairCachingGhostObject : GhostObject
	{
		private HashedOverlappingPairCache m_hashPairCache;

		public PairCachingGhostObject()
		{
			m_hashPairCache = new HashedOverlappingPairCache();
		}

		public override void Cleanup()
		{
			m_hashPairCache.Cleanup();
			m_hashPairCache = null;
		}

		public override void AddOverlappingObjectInternal(BroadphaseProxy otherProxy, BroadphaseProxy thisProxy)
		{
			BroadphaseProxy proxy = ((thisProxy != null) ? thisProxy : GetBroadphaseHandle());
			CollisionObject item = otherProxy.m_clientObject as CollisionObject;
			if (!m_overlappingObjects.Contains(item))
			{
				m_overlappingObjects.Add(item);
				m_hashPairCache.AddOverlappingPair(proxy, otherProxy);
			}
		}

		public override void RemoveOverlappingObjectInternal(BroadphaseProxy otherProxy, IDispatcher dispatcher, BroadphaseProxy thisProxy)
		{
			CollisionObject item = otherProxy.m_clientObject as CollisionObject;
			BroadphaseProxy proxy = ((thisProxy != null) ? thisProxy : GetBroadphaseHandle());
			if (m_overlappingObjects.Remove(item))
			{
				m_hashPairCache.RemoveOverlappingPair(proxy, otherProxy, dispatcher);
			}
		}

		public HashedOverlappingPairCache GetOverlappingPairCache()
		{
			return m_hashPairCache;
		}
	}
}
