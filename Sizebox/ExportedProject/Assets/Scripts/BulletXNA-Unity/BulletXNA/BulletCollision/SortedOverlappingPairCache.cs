using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SortedOverlappingPairCache : IOverlappingPairCache, IOverlappingPairCallback
	{
		protected ObjectArray<BroadphasePair> m_overlappingPairArray;

		protected bool m_blockedForChanges;

		protected bool m_hasDeferredRemoval;

		protected IOverlapFilterCallback m_overlapFilterCallback;

		protected IOverlappingPairCallback m_ghostPairCallback;

		public SortedOverlappingPairCache()
		{
			m_blockedForChanges = false;
			m_hasDeferredRemoval = true;
			m_overlapFilterCallback = null;
			m_ghostPairCallback = null;
			m_overlappingPairArray = new ObjectArray<BroadphasePair>(2);
		}

		public virtual void Cleanup()
		{
		}

		public virtual void ProcessAllOverlappingPairs(IOverlapCallback callback, IDispatcher dispatcher)
		{
			int num = 0;
			while (num < m_overlappingPairArray.Count)
			{
				BroadphasePair broadphasePair = m_overlappingPairArray[num];
				if (callback.ProcessOverlap(broadphasePair))
				{
					CleanOverlappingPair(broadphasePair, dispatcher);
					broadphasePair.m_pProxy0 = null;
					broadphasePair.m_pProxy1 = null;
					m_overlappingPairArray.RemoveAtQuick(num);
					OverlappingPairCacheGlobals.gOverlappingPairs--;
				}
				else
				{
					num++;
				}
			}
		}

		public object RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, IDispatcher dispatcher)
		{
			if (!HasDeferredRemoval())
			{
				BroadphasePair item = new BroadphasePair(proxy0, proxy1);
				int num = m_overlappingPairArray.IndexOf(item);
				if (num >= 0 && num < m_overlappingPairArray.Count)
				{
					OverlappingPairCacheGlobals.gOverlappingPairs--;
					BroadphasePair broadphasePair = m_overlappingPairArray[num];
					object internalInfo = broadphasePair.m_internalInfo1;
					CleanOverlappingPair(broadphasePair, dispatcher);
					if (m_ghostPairCallback != null)
					{
						m_ghostPairCallback.RemoveOverlappingPair(proxy0, proxy1, dispatcher);
					}
					m_overlappingPairArray.RemoveAtQuick(num);
					return internalInfo;
				}
			}
			return null;
		}

		public void CleanOverlappingPair(BroadphasePair pair, IDispatcher dispatcher)
		{
			if (pair.m_algorithm != null)
			{
				dispatcher.FreeCollisionAlgorithm(pair.m_algorithm);
				pair.m_algorithm = null;
				OverlappingPairCacheGlobals.gRemovePairs--;
			}
		}

		public BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (!NeedsBroadphaseCollision(proxy0, proxy1))
			{
				return null;
			}
			BroadphasePair broadphasePair = new BroadphasePair(proxy0, proxy1);
			m_overlappingPairArray.Add(broadphasePair);
			OverlappingPairCacheGlobals.gOverlappingPairs++;
			OverlappingPairCacheGlobals.gAddedPairs++;
			if (m_ghostPairCallback != null)
			{
				m_ghostPairCallback.AddOverlappingPair(proxy0, proxy1);
			}
			return broadphasePair;
		}

		public BroadphasePair FindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (!NeedsBroadphaseCollision(proxy0, proxy1))
			{
				return null;
			}
			BroadphasePair item = new BroadphasePair(proxy0, proxy1);
			int num = m_overlappingPairArray.IndexOf(item);
			if (num != -1)
			{
				return m_overlappingPairArray[num];
			}
			return null;
		}

		public void CleanProxyFromPairs(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			CleanPairCallback callback = new CleanPairCallback(proxy, this, dispatcher);
			ProcessAllOverlappingPairs(callback, dispatcher);
		}

		public void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			RemovePairCallback callback = new RemovePairCallback(proxy);
			ProcessAllOverlappingPairs(callback, dispatcher);
		}

		public bool NeedsBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (m_overlapFilterCallback != null)
			{
				return m_overlapFilterCallback.NeedBroadphaseCollision(proxy0, proxy1);
			}
			return (proxy0.m_collisionFilterGroup & proxy1.m_collisionFilterMask) != 0 && (proxy1.m_collisionFilterGroup & proxy0.m_collisionFilterMask) != 0;
		}

		public ObjectArray<BroadphasePair> GetOverlappingPairArray()
		{
			return m_overlappingPairArray;
		}

		public int GetNumOverlappingPairs()
		{
			return m_overlappingPairArray.Count;
		}

		public IOverlapFilterCallback GetOverlapFilterCallback()
		{
			return m_overlapFilterCallback;
		}

		public void SetOverlapFilterCallback(IOverlapFilterCallback callback)
		{
			m_overlapFilterCallback = callback;
		}

		public virtual bool HasDeferredRemoval()
		{
			return m_hasDeferredRemoval;
		}

		public virtual void SetInternalGhostPairCallback(IOverlappingPairCallback ghostPairCallback)
		{
			m_ghostPairCallback = ghostPairCallback;
		}

		public virtual void SortOverlappingPairs(IDispatcher dispatcher)
		{
		}
	}
}
