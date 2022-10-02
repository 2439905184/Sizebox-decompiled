using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MultiSapBroadphase : IBroadphaseInterface
	{
		private class MyNodeOverlapCallback : INodeOverlapCallback
		{
			private MultiSapBroadphase m_multiSap;

			private MultiSapProxy m_multiProxy;

			private IDispatcher m_dispatcher;

			public MyNodeOverlapCallback(MultiSapBroadphase multiSap, MultiSapProxy multiProxy, IDispatcher dispatcher)
			{
				m_multiSap = multiSap;
				m_multiProxy = multiProxy;
				m_dispatcher = dispatcher;
			}

			public virtual void ProcessNode(int nodeSubPart, int broadphaseIndex)
			{
				IBroadphaseInterface broadphaseInterface = m_multiSap.GetBroadphaseArray()[broadphaseIndex];
				int num = -1;
				for (int i = 0; i < m_multiProxy.m_bridgeProxies.Count; i++)
				{
					if (m_multiProxy.m_bridgeProxies[i].m_childBroadphase == broadphaseInterface)
					{
						num = i;
						break;
					}
				}
				if (num < 0)
				{
					BroadphaseProxy childProxy = broadphaseInterface.CreateProxy(ref m_multiProxy.m_aabbMin, ref m_multiProxy.m_aabbMax, m_multiProxy.m_shapeType, m_multiProxy.m_clientObject, m_multiProxy.m_collisionFilterGroup, m_multiProxy.m_collisionFilterMask, m_dispatcher, m_multiProxy);
					m_multiSap.AddToChildBroadphase(m_multiProxy, childProxy, broadphaseInterface);
				}
			}

			public virtual void Cleanup()
			{
			}
		}

		private IList<IBroadphaseInterface> m_sapBroadphases = new List<IBroadphaseInterface>();

		private SimpleBroadphase m_simpleBroadphase;

		private IOverlappingPairCache m_overlappingPairs;

		private QuantizedBvh m_optimizedAabbTree;

		private bool m_ownsPairCache;

		private IOverlapFilterCallback m_filterCallback;

		private int m_invalidPair;

		protected IList<MultiSapProxy> m_multiSapProxies;

		protected bool m_stopUpdating;

		public MultiSapBroadphase()
			: this(16384, null)
		{
		}

		public MultiSapBroadphase(int maxProxies, IOverlappingPairCache pairCache)
		{
			m_overlappingPairs = pairCache;
			m_optimizedAabbTree = null;
			m_ownsPairCache = false;
			m_invalidPair = 0;
			if (m_overlappingPairs == null)
			{
				m_ownsPairCache = true;
				m_overlappingPairs = new SortedOverlappingPairCache();
			}
			m_filterCallback = new MultiSapOverlapFilterCallback();
			m_overlappingPairs.SetOverlapFilterCallback(m_filterCallback);
			m_simpleBroadphase = new SimpleBroadphase(maxProxies, m_overlappingPairs);
		}

		public IList<IBroadphaseInterface> GetBroadphaseArray()
		{
			return m_sapBroadphases;
		}

		public virtual void Cleanup()
		{
			if (m_ownsPairCache)
			{
				m_overlappingPairs.Cleanup();
				m_overlappingPairs = null;
				m_ownsPairCache = false;
			}
		}

		public virtual BroadphaseProxy CreateProxy(IndexedVector3 aabbMin, IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			return CreateProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, multiSapProxy);
		}

		public virtual BroadphaseProxy CreateProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			MultiSapProxy multiSapProxy2 = new MultiSapProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask);
			m_multiSapProxies.Add(multiSapProxy2);
			SetAabb(multiSapProxy2, ref aabbMin, ref aabbMax, dispatcher);
			return multiSapProxy2;
		}

		public virtual void DestroyProxy(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
		}

		public virtual void SetAabb(BroadphaseProxy proxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			MultiSapProxy multiSapProxy = (MultiSapProxy)proxy;
			multiSapProxy.m_aabbMin = aabbMin;
			multiSapProxy.m_aabbMax = aabbMax;
			MyNodeOverlapCallback nodeCallback = new MyNodeOverlapCallback(this, multiSapProxy, dispatcher);
			if (m_optimizedAabbTree != null)
			{
				m_optimizedAabbTree.ReportAabbOverlappingNodex(nodeCallback, ref aabbMin, ref aabbMax);
			}
			for (int i = 0; i < multiSapProxy.m_bridgeProxies.Count; i++)
			{
				IndexedVector3 aabbMin2;
				IndexedVector3 aabbMax2;
				multiSapProxy.m_bridgeProxies[i].m_childBroadphase.GetBroadphaseAabb(out aabbMin2, out aabbMax2);
				if (!AabbUtil2.TestAabbAgainstAabb2(ref aabbMin2, ref aabbMax2, ref multiSapProxy.m_aabbMin, ref multiSapProxy.m_aabbMax))
				{
					BridgeProxy bridgeProxy = multiSapProxy.m_bridgeProxies[i];
					BroadphaseProxy childProxy = bridgeProxy.m_childProxy;
					bridgeProxy.m_childBroadphase.DestroyProxy(childProxy, dispatcher);
					multiSapProxy.m_bridgeProxies.RemoveAtQuick(i);
				}
			}
			for (int j = 0; j < multiSapProxy.m_bridgeProxies.Count; j++)
			{
				BridgeProxy bridgeProxy2 = multiSapProxy.m_bridgeProxies[j];
				bridgeProxy2.m_childBroadphase.SetAabb(bridgeProxy2.m_childProxy, ref aabbMin, ref aabbMax, dispatcher);
			}
		}

		public virtual void GetAabb(BroadphaseProxy proxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			MultiSapProxy multiSapProxy = (MultiSapProxy)proxy;
			aabbMin = multiSapProxy.m_aabbMin;
			aabbMax = multiSapProxy.m_aabbMax;
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			RayTest(ref rayFrom, ref rayTo, rayCallback, ref aabbMin, ref aabbMax);
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			for (int i = 0; i < m_multiSapProxies.Count; i++)
			{
				rayCallback.Process(m_multiSapProxies[i]);
			}
		}

		public virtual void AabbTest(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IBroadphaseAabbCallback callback)
		{
		}

		public void AddToChildBroadphase(MultiSapProxy parentMultiSapProxy, BroadphaseProxy childProxy, IBroadphaseInterface childBroadphase)
		{
			BridgeProxy bridgeProxy = new BridgeProxy();
			bridgeProxy.m_childProxy = childProxy;
			bridgeProxy.m_childBroadphase = childBroadphase;
			parentMultiSapProxy.m_bridgeProxies.Add(bridgeProxy);
		}

		public void CalculateOverlappingPairs(IDispatcher dispatcher)
		{
			if (m_stopUpdating || !GetOverlappingPairCache().HasDeferredRemoval())
			{
				return;
			}
			IList<BroadphasePair> overlappingPairArray = GetOverlappingPairCache().GetOverlappingPairArray();
			((List<BroadphasePair>)overlappingPairArray).Sort();
			m_invalidPair = 0;
			BroadphasePair broadphasePair = new BroadphasePair();
			broadphasePair.m_pProxy0 = null;
			broadphasePair.m_pProxy1 = null;
			broadphasePair.m_algorithm = null;
			for (int i = 0; i < overlappingPairArray.Count; i++)
			{
				BroadphasePair broadphasePair2 = overlappingPairArray[i];
				MultiSapProxy multiSapProxy = ((broadphasePair2.m_pProxy0 != null) ? ((MultiSapProxy)broadphasePair2.m_pProxy0.m_multiSapParentProxy) : null);
				MultiSapProxy multiSapProxy2 = ((broadphasePair2.m_pProxy1 != null) ? ((MultiSapProxy)broadphasePair2.m_pProxy1.m_multiSapParentProxy) : null);
				MultiSapProxy multiSapProxy3 = ((broadphasePair.m_pProxy0 != null) ? ((MultiSapProxy)broadphasePair.m_pProxy0.m_multiSapParentProxy) : null);
				MultiSapProxy multiSapProxy4 = ((broadphasePair.m_pProxy1 != null) ? ((MultiSapProxy)broadphasePair.m_pProxy1.m_multiSapParentProxy) : null);
				bool flag = multiSapProxy == multiSapProxy3 && multiSapProxy2 == multiSapProxy4;
				broadphasePair = broadphasePair2;
				bool flag2 = false;
				if (flag || ((!TestAabbOverlap(broadphasePair2.m_pProxy0, broadphasePair2.m_pProxy1)) ? true : false))
				{
					GetOverlappingPairCache().CleanOverlappingPair(broadphasePair2, dispatcher);
					broadphasePair2.m_pProxy0 = null;
					broadphasePair2.m_pProxy1 = null;
					m_invalidPair++;
					BulletGlobals.gOverlappingPairs--;
				}
			}
		}

		public bool TestAabbOverlap(BroadphaseProxy childProxy0, BroadphaseProxy childProxy1)
		{
			MultiSapProxy multiSapProxy = (MultiSapProxy)childProxy0.m_multiSapParentProxy;
			MultiSapProxy multiSapProxy2 = (MultiSapProxy)childProxy1.m_multiSapParentProxy;
			return AabbUtil2.TestAabbAgainstAabb2(ref multiSapProxy.m_aabbMin, ref multiSapProxy.m_aabbMax, ref multiSapProxy2.m_aabbMin, ref multiSapProxy2.m_aabbMax);
		}

		public virtual IOverlappingPairCache GetOverlappingPairCache()
		{
			return m_overlappingPairs;
		}

		public virtual void GetBroadphaseAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = MathUtil.MIN_VECTOR;
			aabbMax = MathUtil.MAX_VECTOR;
		}

		public void BuildTree(ref IndexedVector3 bvhAabbMin, ref IndexedVector3 bvhAabbMax)
		{
			m_optimizedAabbTree = new QuantizedBvh();
			m_optimizedAabbTree.SetQuantizationValues(ref bvhAabbMin, ref bvhAabbMax);
			IList<QuantizedBvhNode> leafNodeArray = m_optimizedAabbTree.GetLeafNodeArray();
			for (int i = 0; i < m_sapBroadphases.Count; i++)
			{
				QuantizedBvhNode quantizedBvhNode = new QuantizedBvhNode();
				IndexedVector3 aabbMin;
				IndexedVector3 aabbMax;
				m_sapBroadphases[i].GetBroadphaseAabb(out aabbMin, out aabbMax);
				m_optimizedAabbTree.Quantize(out quantizedBvhNode.m_quantizedAabbMin, ref aabbMin, false);
				m_optimizedAabbTree.Quantize(out quantizedBvhNode.m_quantizedAabbMax, ref aabbMax, true);
				int num = 0;
				quantizedBvhNode.m_escapeIndexOrTriangleIndex = (num << 21) | i;
				leafNodeArray.Add(quantizedBvhNode);
			}
			m_optimizedAabbTree.BuildInternal();
		}

		public virtual void PrintStats()
		{
		}

		public virtual void ResetPool(IDispatcher dispatcher)
		{
		}
	}
}
