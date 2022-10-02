using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class NullPairCache : IOverlappingPairCache, IOverlappingPairCallback
	{
		private ObjectArray<BroadphasePair> m_overlappingPairArray = new ObjectArray<BroadphasePair>();

		public virtual void Cleanup()
		{
		}

		public ObjectArray<BroadphasePair> GetOverlappingPairArray()
		{
			return m_overlappingPairArray;
		}

		public virtual void CleanOverlappingPair(BroadphasePair pair, IDispatcher disaptcher)
		{
		}

		public virtual int GetNumOverlappingPairs()
		{
			return 0;
		}

		public virtual void CleanProxyFromPairs(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
		}

		public virtual void SetOverlapFilterCallback(IOverlapFilterCallback callback)
		{
		}

		public virtual void ProcessAllOverlappingPairs(IOverlapCallback callback, IDispatcher dispatcher)
		{
		}

		public virtual BroadphasePair FindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			return null;
		}

		public virtual bool HasDeferredRemoval()
		{
			return true;
		}

		public virtual void SetInternalGhostPairCallback(IOverlappingPairCallback ghostPairCallback)
		{
		}

		public virtual BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			return null;
		}

		public virtual object RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, IDispatcher dispatcher)
		{
			return null;
		}

		public virtual void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, IDispatcher dispatcher)
		{
		}

		public virtual void SortOverlappingPairs(IDispatcher dispatcher)
		{
		}
	}
}
