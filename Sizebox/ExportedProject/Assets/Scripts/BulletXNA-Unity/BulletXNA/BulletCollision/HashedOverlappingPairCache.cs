using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class HashedOverlappingPairCache : IOverlappingPairCache, IOverlappingPairCallback
	{
		private const int BT_NULL_PAIR = -1;

		protected ObjectArray<int> m_hashTable = new ObjectArray<int>();

		protected ObjectArray<int> m_next = new ObjectArray<int>();

		protected IOverlappingPairCallback m_ghostPairCallback;

		private ObjectArray<BroadphasePair> m_overlappingPairArray;

		private IOverlapFilterCallback m_overlapFilterCallback;

		private bool m_blockedForChanges;

		private static object NULL_PAIR = new object();

		public HashedOverlappingPairCache()
		{
			m_overlapFilterCallback = null;
			m_blockedForChanges = false;
			m_ghostPairCallback = null;
			int capacity = 2;
			m_overlappingPairArray = new ObjectArray<BroadphasePair>(capacity);
			GrowTables();
		}

		public virtual void Cleanup()
		{
		}

		public virtual void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			RemovePairCallback callback = new RemovePairCallback(proxy);
			ProcessAllOverlappingPairs(callback, dispatcher);
		}

		public virtual object RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, IDispatcher dispatcher)
		{
			OverlappingPairCacheGlobals.gRemovePairs++;
			if (proxy0.m_uniqueId > proxy1.m_uniqueId)
			{
				BroadphaseProxy broadphaseProxy = proxy0;
				proxy0 = proxy1;
				proxy1 = broadphaseProxy;
			}
			int uid = proxy0.GetUid();
			int uid2 = proxy1.GetUid();
			int num = (int)(GetHash((uint)uid, (uint)uid2) & (m_overlappingPairArray.Capacity - 1));
			BroadphasePair broadphasePair = InternalFindPair(proxy0, proxy1, num);
			if (broadphasePair == null)
			{
				return null;
			}
			CleanOverlappingPair(broadphasePair, dispatcher);
			object internalInfo = broadphasePair.m_internalInfo1;
			int index = broadphasePair.m_index;
			int num2 = m_hashTable[num];
			int num3 = -1;
			while (num2 != index)
			{
				num3 = num2;
				num2 = m_next[num2];
			}
			if (num3 != -1)
			{
				m_next[num3] = m_next[index];
			}
			else
			{
				m_hashTable[num] = m_next[index];
			}
			int num4 = m_overlappingPairArray.Count - 1;
			if (m_ghostPairCallback != null)
			{
				m_ghostPairCallback.RemoveOverlappingPair(proxy0, proxy1, dispatcher);
			}
			if (num4 == index)
			{
				m_overlappingPairArray.RemoveAt(num4);
				return internalInfo;
			}
			BroadphasePair broadphasePair2 = m_overlappingPairArray[num4];
			int index2 = (int)(GetHash((uint)broadphasePair2.m_pProxy0.GetUid(), (uint)broadphasePair2.m_pProxy1.GetUid()) & (m_overlappingPairArray.Capacity - 1));
			num2 = m_hashTable[index2];
			num3 = -1;
			while (num2 != num4)
			{
				num3 = num2;
				num2 = m_next[num2];
			}
			if (num3 != -1)
			{
				m_next[num3] = m_next[num4];
			}
			else
			{
				m_hashTable[index2] = m_next[num4];
			}
			m_overlappingPairArray[index] = m_overlappingPairArray[num4];
			m_next[index] = m_hashTable[index2];
			m_hashTable[index2] = index;
			m_overlappingPairArray.RemoveAt(num4);
			return internalInfo;
		}

		public bool NeedsBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (m_overlapFilterCallback != null)
			{
				return m_overlapFilterCallback.NeedBroadphaseCollision(proxy0, proxy1);
			}
			return (proxy0.m_collisionFilterGroup & proxy1.m_collisionFilterMask) != 0 && (proxy1.m_collisionFilterGroup & proxy0.m_collisionFilterMask) != 0;
		}

		public virtual BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			OverlappingPairCacheGlobals.gAddedPairs++;
			if (!NeedsBroadphaseCollision(proxy0, proxy1))
			{
				return null;
			}
			return InternalAddPair(proxy0, proxy1);
		}

		public void CleanProxyFromPairs(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			CleanPairCallback callback = new CleanPairCallback(proxy, this, dispatcher);
			ProcessAllOverlappingPairs(callback, dispatcher);
		}

		public virtual void ProcessAllOverlappingPairs(IOverlapCallback callback, IDispatcher dispatcher)
		{
			int num = 0;
			while (num < m_overlappingPairArray.Count)
			{
				BroadphasePair broadphasePair = m_overlappingPairArray[num];
				if (callback.ProcessOverlap(broadphasePair))
				{
					RemoveOverlappingPair(broadphasePair.m_pProxy0, broadphasePair.m_pProxy1, dispatcher);
					OverlappingPairCacheGlobals.gOverlappingPairs--;
				}
				else
				{
					num++;
				}
			}
		}

		public ObjectArray<BroadphasePair> GetOverlappingPairArray()
		{
			return m_overlappingPairArray;
		}

		public void CleanOverlappingPair(BroadphasePair pair, IDispatcher dispatcher)
		{
			if (pair.m_algorithm != null)
			{
				dispatcher.FreeCollisionAlgorithm(pair.m_algorithm);
				pair.m_algorithm = null;
			}
		}

		public BroadphasePair FindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			OverlappingPairCacheGlobals.gFindPairs++;
			if (proxy0.m_uniqueId > proxy1.m_uniqueId)
			{
				BroadphaseProxy broadphaseProxy = proxy0;
				proxy0 = proxy1;
				proxy1 = broadphaseProxy;
			}
			int uid = proxy0.GetUid();
			int uid2 = proxy1.GetUid();
			int num = (int)(GetHash((uint)uid, (uint)uid2) & (m_overlappingPairArray.Capacity - 1));
			if (num >= m_hashTable.Count)
			{
				return null;
			}
			int num2 = m_hashTable[num];
			while (num2 != -1 && !EqualsPair(m_overlappingPairArray[num2], uid, uid2))
			{
				num2 = m_next[num2];
			}
			if (num2 == -1)
			{
				return null;
			}
			return m_overlappingPairArray[num2];
		}

		public int GetCount()
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

		public int GetNumOverlappingPairs()
		{
			return m_overlappingPairArray.Count;
		}

		private BroadphasePair InternalAddPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			if (proxy0.m_uniqueId > proxy1.m_uniqueId)
			{
				BroadphaseProxy broadphaseProxy = proxy0;
				proxy0 = proxy1;
				proxy1 = broadphaseProxy;
			}
			int uid = proxy0.GetUid();
			int uid2 = proxy1.GetUid();
			int num = (int)(GetHash((uint)uid, (uint)uid2) & (m_overlappingPairArray.Capacity - 1));
			BroadphasePair broadphasePair = InternalFindPair(proxy0, proxy1, num);
			if (broadphasePair != null)
			{
				return broadphasePair;
			}
			int count = m_overlappingPairArray.Count;
			int capacity = m_overlappingPairArray.Capacity;
			if (m_ghostPairCallback != null)
			{
				m_ghostPairCallback.AddOverlappingPair(proxy0, proxy1);
			}
			broadphasePair = new BroadphasePair(proxy0, proxy1);
			m_overlappingPairArray.Add(broadphasePair);
			int capacity2 = m_overlappingPairArray.Capacity;
			if (capacity < capacity2)
			{
				GrowTables();
				num = (int)(GetHash((uint)uid, (uint)uid2) & (m_overlappingPairArray.Capacity - 1));
			}
			m_next[count] = m_hashTable[num];
			m_hashTable[num] = count;
			return broadphasePair;
		}

		private void GrowTables()
		{
			int capacity = m_overlappingPairArray.Capacity;
			if (m_hashTable.Capacity < capacity)
			{
				int count = m_hashTable.Count;
				m_hashTable.Capacity = capacity;
				m_next.Capacity = capacity;
				for (int i = 0; i < capacity; i++)
				{
					m_hashTable[i] = -1;
				}
				for (int j = 0; j < capacity; j++)
				{
					m_next[j] = -1;
				}
				for (int k = 0; k < count; k++)
				{
					BroadphasePair broadphasePair = m_overlappingPairArray[k];
					int uid = broadphasePair.m_pProxy0.GetUid();
					int uid2 = broadphasePair.m_pProxy1.GetUid();
					int index = (int)(GetHash((uint)uid, (uint)uid2) & (m_overlappingPairArray.Capacity - 1));
					m_next[k] = m_hashTable[index];
					m_hashTable[index] = k;
				}
			}
		}

		private bool EqualsPair(BroadphasePair pair, int proxyId1, int proxyId2)
		{
			if (pair.m_pProxy0.m_uniqueId == proxyId1)
			{
				return pair.m_pProxy1.m_uniqueId == proxyId2;
			}
			return false;
		}

		private uint GetHash(uint proxyId1, uint proxyId2)
		{
			int num = (int)(proxyId1 | (proxyId2 << 16));
			num += ~(num << 15);
			num ^= num >> 10;
			num += num << 3;
			num ^= num >> 6;
			num += ~(num << 11);
			return (uint)(num ^ (num >> 16));
		}

		public BroadphasePair InternalFindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, int hash)
		{
			BroadphasePair[] rawArray = m_overlappingPairArray.GetRawArray();
			int uid = proxy0.GetUid();
			int uid2 = proxy1.GetUid();
			int num = m_hashTable[hash];
			while (num != -1 && !EqualsPair(rawArray[num], uid, uid2))
			{
				num = m_next[num];
			}
			if (num == -1)
			{
				return null;
			}
			rawArray[num].m_index = num;
			return rawArray[num];
		}

		public virtual bool HasDeferredRemoval()
		{
			return false;
		}

		public virtual void SetInternalGhostPairCallback(IOverlappingPairCallback ghostPairCallback)
		{
			m_ghostPairCallback = ghostPairCallback;
		}

		public virtual void SortOverlappingPairs(IDispatcher dispatcher)
		{
			ObjectArray<BroadphasePair> objectArray = new ObjectArray<BroadphasePair>();
			objectArray.AddRange(m_overlappingPairArray);
			for (int i = 0; i < objectArray.Count; i++)
			{
				RemoveOverlappingPair(objectArray[i].m_pProxy0, objectArray[i].m_pProxy1, dispatcher);
			}
			for (int j = 0; j < m_next.Count; j++)
			{
				m_next[j] = -1;
			}
			objectArray.Sort();
			for (int k = 0; k < objectArray.Count; k++)
			{
				AddOverlappingPair(objectArray[k].m_pProxy0, objectArray[k].m_pProxy1);
			}
		}
	}
}
