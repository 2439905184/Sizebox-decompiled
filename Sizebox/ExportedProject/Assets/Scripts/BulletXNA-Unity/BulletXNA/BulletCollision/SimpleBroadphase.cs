using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class SimpleBroadphase : IBroadphaseInterface
	{
		private int m_proxyCounter = 2;

		private IOverlappingPairCache m_pairCache;

		private bool m_ownsPairCache;

		private int m_invalidPair;

		protected int m_numHandles;

		protected int m_maxHandles;

		protected int m_LastHandleIndex;

		protected SimpleBroadphaseProxy[] m_pHandles;

		protected object m_pHandlesRawPtr;

		protected int m_firstFreeHandle;

		public SimpleBroadphase(int maxProxies, IOverlappingPairCache overlappingPairCache)
		{
			if (overlappingPairCache == null)
			{
				overlappingPairCache = new HashedOverlappingPairCache();
				m_ownsPairCache = true;
			}
			m_pHandles = new SimpleBroadphaseProxy[maxProxies];
			m_pairCache = overlappingPairCache;
			m_maxHandles = maxProxies;
			m_numHandles = 0;
			m_firstFreeHandle = 0;
			m_LastHandleIndex = -1;
			for (int i = m_firstFreeHandle; i < maxProxies; i++)
			{
				m_pHandles[i] = new SimpleBroadphaseProxy(i);
				m_pHandles[i].SetNextFree(i + 1);
				m_pHandles[i].m_uniqueId = ++m_proxyCounter;
			}
			m_pHandles[maxProxies - 1].SetNextFree(0);
		}

		public virtual void Cleanup()
		{
			for (int i = 0; i < m_pHandles.Length; i++)
			{
				if (m_pHandles[i] != null)
				{
					m_pHandles[i].Cleanup();
				}
				m_pHandles[i] = null;
			}
			if (m_ownsPairCache)
			{
				m_pairCache.Cleanup();
				m_pairCache = null;
				m_ownsPairCache = false;
			}
		}

		public static bool AabbOverlap(SimpleBroadphaseProxy proxy0, SimpleBroadphaseProxy proxy1)
		{
			IndexedVector3 minAABB = proxy0.GetMinAABB();
			IndexedVector3 maxAABB = proxy0.GetMaxAABB();
			IndexedVector3 minAABB2 = proxy1.GetMinAABB();
			IndexedVector3 maxAABB2 = proxy1.GetMaxAABB();
			if (minAABB.X <= maxAABB2.X && minAABB2.X <= maxAABB.X && minAABB.Y <= maxAABB2.Y && minAABB2.Y <= maxAABB.Y && minAABB.Z <= maxAABB2.Z)
			{
				return minAABB2.Z <= maxAABB.Z;
			}
			return false;
		}

		private int AllocHandle()
		{
			int firstFreeHandle = m_firstFreeHandle;
			m_firstFreeHandle = m_pHandles[firstFreeHandle].GetNextFree();
			m_numHandles++;
			if (firstFreeHandle > m_LastHandleIndex)
			{
				m_LastHandleIndex = firstFreeHandle;
			}
			return firstFreeHandle;
		}

		private void FreeHandle(SimpleBroadphaseProxy proxy)
		{
			int position = proxy.GetPosition();
			if (position == m_LastHandleIndex)
			{
				m_LastHandleIndex--;
			}
			proxy.SetNextFree(m_firstFreeHandle);
			m_firstFreeHandle = position;
			proxy.m_clientObject = null;
			m_numHandles--;
		}

		public virtual BroadphaseProxy CreateProxy(IndexedVector3 aabbMin, IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			return CreateProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, multiSapProxy);
		}

		public virtual BroadphaseProxy CreateProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			if (m_numHandles >= m_maxHandles)
			{
				return null;
			}
			int num = AllocHandle();
			m_pHandles[num] = new SimpleBroadphaseProxy(num, ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, multiSapProxy);
			m_pHandles[num].m_uniqueId = ++m_proxyCounter;
			return m_pHandles[num];
		}

		public virtual void CalculateOverlappingPairs(IDispatcher dispatcher)
		{
			if (m_numHandles <= 0)
			{
				return;
			}
			int lastHandleIndex = -1;
			for (int i = 0; i <= m_LastHandleIndex; i++)
			{
				SimpleBroadphaseProxy simpleBroadphaseProxy = m_pHandles[i];
				if (simpleBroadphaseProxy.GetClientObject() == null)
				{
					continue;
				}
				lastHandleIndex = i;
				for (int j = i + 1; j <= m_LastHandleIndex; j++)
				{
					SimpleBroadphaseProxy simpleBroadphaseProxy2 = m_pHandles[j];
					if (simpleBroadphaseProxy2.GetClientObject() == null)
					{
						continue;
					}
					if (AabbOverlap(simpleBroadphaseProxy, simpleBroadphaseProxy2))
					{
						if (m_pairCache.FindPair(simpleBroadphaseProxy, simpleBroadphaseProxy2) == null)
						{
							m_pairCache.AddOverlappingPair(simpleBroadphaseProxy, simpleBroadphaseProxy2);
						}
					}
					else if (!m_pairCache.HasDeferredRemoval() && m_pairCache.FindPair(simpleBroadphaseProxy, simpleBroadphaseProxy2) != null)
					{
						m_pairCache.RemoveOverlappingPair(simpleBroadphaseProxy, simpleBroadphaseProxy2, dispatcher);
					}
				}
			}
			m_LastHandleIndex = lastHandleIndex;
			if (!m_ownsPairCache || !m_pairCache.HasDeferredRemoval())
			{
				return;
			}
			IList<BroadphasePair> overlappingPairArray = m_pairCache.GetOverlappingPairArray();
			((List<BroadphasePair>)overlappingPairArray).Sort();
			m_invalidPair = 0;
			BroadphasePair broadphasePair = new BroadphasePair();
			for (int k = 0; k < overlappingPairArray.Count; k++)
			{
				BroadphasePair broadphasePair2 = overlappingPairArray[k];
				bool flag = broadphasePair2 == broadphasePair;
				broadphasePair = broadphasePair2;
				bool flag2 = false;
				if (flag || ((!TestAabbOverlap(broadphasePair2.m_pProxy0, broadphasePair2.m_pProxy1)) ? true : false))
				{
					m_pairCache.CleanOverlappingPair(broadphasePair2, dispatcher);
					broadphasePair2.m_pProxy0 = null;
					broadphasePair2.m_pProxy1 = null;
					m_invalidPair++;
					BulletGlobals.gOverlappingPairs--;
				}
			}
			((List<BroadphasePair>)overlappingPairArray).Sort();
			m_invalidPair = 0;
		}

		public virtual void DestroyProxy(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			SimpleBroadphaseProxy proxy2 = (SimpleBroadphaseProxy)proxy;
			FreeHandle(proxy2);
			m_pairCache.RemoveOverlappingPairsContainingProxy(proxy, dispatcher);
		}

		public virtual void SetAabb(BroadphaseProxy proxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			SimpleBroadphaseProxy simpleProxyFromProxy = GetSimpleProxyFromProxy(proxy);
			simpleProxyFromProxy.SetMinAABB(ref aabbMin);
			simpleProxyFromProxy.SetMaxAABB(ref aabbMax);
		}

		public virtual void GetAabb(BroadphaseProxy proxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			SimpleBroadphaseProxy simpleProxyFromProxy = GetSimpleProxyFromProxy(proxy);
			aabbMin = simpleProxyFromProxy.GetMinAABB();
			aabbMax = simpleProxyFromProxy.GetMaxAABB();
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			RayTest(ref rayFrom, ref rayTo, rayCallback, ref aabbMin, ref aabbMax);
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			for (int i = 0; i <= m_LastHandleIndex; i++)
			{
				SimpleBroadphaseProxy simpleBroadphaseProxy = m_pHandles[i];
				if (simpleBroadphaseProxy.m_clientObject != null)
				{
					rayCallback.Process(simpleBroadphaseProxy);
				}
			}
		}

		public virtual void AabbTest(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IBroadphaseAabbCallback callback)
		{
			for (int i = 0; i <= m_LastHandleIndex; i++)
			{
				SimpleBroadphaseProxy simpleBroadphaseProxy = m_pHandles[i];
				if (simpleBroadphaseProxy.m_clientObject != null && AabbUtil2.TestAabbAgainstAabb2(ref aabbMin, ref aabbMax, ref simpleBroadphaseProxy.m_aabbMin, ref simpleBroadphaseProxy.m_aabbMax))
				{
					callback.Process(simpleBroadphaseProxy);
				}
			}
		}

		public IOverlappingPairCache GetOverlappingPairCache()
		{
			return m_pairCache;
		}

		public bool TestAabbOverlap(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			return AabbOverlap((SimpleBroadphaseProxy)proxy0, (SimpleBroadphaseProxy)proxy1);
		}

		public virtual void GetBroadphaseAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = MathUtil.MIN_VECTOR;
			aabbMax = MathUtil.MAX_VECTOR;
		}

		public virtual void PrintStats()
		{
		}

		protected SimpleBroadphaseProxy GetSimpleProxyFromProxy(BroadphaseProxy proxy)
		{
			return proxy as SimpleBroadphaseProxy;
		}

		public virtual void ResetPool(IDispatcher dispatcher)
		{
		}

		protected void Validate()
		{
			for (int i = 0; i < m_numHandles; i++)
			{
				for (int j = i + 1; j < m_numHandles; j++)
				{
				}
			}
		}
	}
}
