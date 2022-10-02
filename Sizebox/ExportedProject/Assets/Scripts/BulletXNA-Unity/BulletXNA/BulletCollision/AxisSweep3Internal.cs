using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class AxisSweep3Internal : IBroadphaseInterface
	{
		protected int m_bpHandleMask;

		protected ushort m_handleSentinel;

		protected IndexedVector3 m_worldAabbMin;

		protected IndexedVector3 m_worldAabbMax;

		protected IndexedVector3 m_quantize;

		protected ushort m_numHandles;

		protected ushort m_maxHandles;

		protected Handle[] m_pHandles;

		protected ushort m_firstFreeHandle;

		protected Edge[,] m_pEdges;

		protected IOverlappingPairCache m_pairCache;

		protected IOverlappingPairCallback m_userPairCallback;

		protected bool m_ownsPairCache;

		protected int m_invalidPair;

		protected DbvtBroadphase m_raycastAccelerator;

		protected IOverlappingPairCache m_nullPairCache;

		private static ushort[] min = new ushort[3];

		private static ushort[] max = new ushort[3];

		public DbvtBroadphase GetAccelerator()
		{
			return m_raycastAccelerator;
		}

		protected ushort AllocHandle()
		{
			ushort firstFreeHandle = m_firstFreeHandle;
			m_firstFreeHandle = GetHandle(firstFreeHandle).GetNextFree();
			m_numHandles++;
			return firstFreeHandle;
		}

		protected void FreeHandle(ushort handle)
		{
			GetHandle(handle).SetNextFree(m_firstFreeHandle);
			m_firstFreeHandle = handle;
			m_numHandles--;
		}

		protected bool TestOverlap2D(Handle pHandleA, Handle pHandleB, int axis0, int axis1)
		{
			if (pHandleA.m_maxEdges[axis0] < pHandleB.m_minEdges[axis0] || pHandleB.m_maxEdges[axis0] < pHandleA.m_minEdges[axis0] || pHandleA.m_maxEdges[axis1] < pHandleB.m_minEdges[axis1] || pHandleB.m_maxEdges[axis1] < pHandleA.m_minEdges[axis1])
			{
				return false;
			}
			return true;
		}

		protected void SortMinDown(int axis, ushort edge, IDispatcher dispatcher, bool updateOverlaps)
		{
			int num = edge;
			int num2 = num - 1;
			Edge edge2 = m_pEdges[axis, num];
			Edge edge3 = m_pEdges[axis, num2];
			Handle handle = GetHandle(edge2.m_handle);
			while (edge2.m_pos < edge3.m_pos)
			{
				Handle handle2 = GetHandle(edge3.m_handle);
				if (edge3.IsMax())
				{
					int num3 = (1 << axis) & 3;
					int axis2 = (1 << num3) & 3;
					if (updateOverlaps && TestOverlap2D(handle, handle2, num3, axis2))
					{
						m_pairCache.AddOverlappingPair(handle, handle2);
						if (m_userPairCallback != null)
						{
							m_userPairCallback.AddOverlappingPair(handle, handle2);
						}
					}
					handle2.m_maxEdges[axis]++;
				}
				else
				{
					handle2.m_minEdges[axis]++;
				}
				handle.m_minEdges[axis]--;
				SanityCheckHandle(handle, axis);
				Edge.Swap(edge2, edge3);
				num--;
				num2--;
				edge2 = m_pEdges[axis, num];
				edge3 = m_pEdges[axis, num2];
			}
		}

		protected void SortMinUp(int axis, ushort edge, IDispatcher dispatcher, bool updateOverlaps)
		{
			int num = edge;
			int num2 = num + 1;
			Edge edge2 = m_pEdges[axis, num];
			Edge edge3 = m_pEdges[axis, num2];
			Handle handle = GetHandle(edge2.m_handle);
			while (edge3.m_handle != 0 && edge2.m_pos >= edge3.m_pos)
			{
				Handle handle2 = GetHandle(edge3.m_handle);
				if (edge3.IsMax())
				{
					Handle handle3 = GetHandle(edge2.m_handle);
					Handle handle4 = GetHandle(edge3.m_handle);
					int num3 = (1 << axis) & 3;
					int axis2 = (1 << num3) & 3;
					if (updateOverlaps && TestOverlap2D(handle3, handle4, num3, axis2))
					{
						m_pairCache.RemoveOverlappingPair(handle3, handle4, dispatcher);
						if (m_userPairCallback != null)
						{
							m_userPairCallback.RemoveOverlappingPair(handle3, handle4, dispatcher);
						}
					}
					handle2.m_maxEdges[axis]--;
				}
				else
				{
					handle2.m_minEdges[axis]--;
				}
				handle.m_minEdges[axis]++;
				SanityCheckHandle(handle, axis);
				Edge.Swap(edge2, edge3);
				num++;
				num2++;
				edge2 = m_pEdges[axis, num];
				edge3 = m_pEdges[axis, num2];
			}
		}

		private void SanityCheckHandle(Handle handle, int axis)
		{
			ushort num = handle.m_minEdges[axis];
			int num2 = 65535;
		}

		protected void SortMaxDown(int axis, ushort edge, IDispatcher dispatcher, bool updateOverlaps)
		{
			int num = edge;
			int num2 = num - 1;
			Edge edge2 = m_pEdges[axis, num];
			Edge edge3 = m_pEdges[axis, num2];
			Handle handle = GetHandle(edge2.m_handle);
			while (edge2.m_pos < edge3.m_pos)
			{
				Handle handle2 = GetHandle(edge3.m_handle);
				if (!edge3.IsMax())
				{
					Handle handle3 = GetHandle(edge2.m_handle);
					Handle handle4 = GetHandle(edge3.m_handle);
					int num3 = (1 << axis) & 3;
					int axis2 = (1 << num3) & 3;
					if (updateOverlaps && TestOverlap2D(handle3, handle4, num3, axis2))
					{
						m_pairCache.RemoveOverlappingPair(handle3, handle4, dispatcher);
						if (m_userPairCallback != null)
						{
							m_userPairCallback.RemoveOverlappingPair(handle3, handle4, dispatcher);
						}
					}
					handle2.m_minEdges[axis]++;
				}
				else
				{
					handle2.m_maxEdges[axis]++;
				}
				handle.m_maxEdges[axis]--;
				SanityCheckHandle(handle, axis);
				Edge.Swap(edge2, edge3);
				num--;
				num2--;
				edge2 = m_pEdges[axis, num];
				edge3 = m_pEdges[axis, num2];
			}
		}

		protected void SortMaxUp(int axis, ushort edge, IDispatcher dispatcher, bool updateOverlaps)
		{
			int num = edge;
			int num2 = num + 1;
			Edge edge2 = m_pEdges[axis, num];
			Edge edge3 = m_pEdges[axis, num2];
			Handle handle = GetHandle(edge2.m_handle);
			while (edge3.m_handle != 0 && edge2.m_pos >= edge3.m_pos)
			{
				Handle handle2 = GetHandle(edge3.m_handle);
				int num3 = (1 << axis) & 3;
				int axis2 = (1 << num3) & 3;
				if (!edge3.IsMax())
				{
					if (updateOverlaps && TestOverlap2D(handle, handle2, num3, axis2))
					{
						Handle handle3 = GetHandle(edge2.m_handle);
						Handle handle4 = GetHandle(edge3.m_handle);
						m_pairCache.AddOverlappingPair(handle3, handle4);
						if (m_userPairCallback != null)
						{
							m_userPairCallback.AddOverlappingPair(handle3, handle4);
						}
					}
					handle2.m_minEdges[axis]--;
				}
				else
				{
					handle2.m_maxEdges[axis]--;
				}
				handle.m_maxEdges[axis]++;
				SanityCheckHandle(handle, axis);
				Edge.Swap(edge2, edge3);
				num++;
				num2++;
				edge2 = m_pEdges[axis, num];
				edge3 = m_pEdges[axis, num2];
			}
		}

		public AxisSweep3Internal(ref IndexedVector3 worldAabbMin, ref IndexedVector3 worldAabbMax, int handleMask, ushort handleSentinel, ushort userMaxHandles, IOverlappingPairCache pairCache, bool disableRaycastAccelerator)
		{
			m_bpHandleMask = handleMask;
			m_handleSentinel = handleSentinel;
			m_pairCache = pairCache;
			m_userPairCallback = null;
			m_ownsPairCache = false;
			m_invalidPair = 0;
			m_raycastAccelerator = null;
			ushort num = (ushort)(userMaxHandles + 1);
			if (m_pairCache == null)
			{
				m_pairCache = new HashedOverlappingPairCache();
				m_ownsPairCache = true;
			}
			if (!disableRaycastAccelerator)
			{
				m_nullPairCache = new NullPairCache();
				m_raycastAccelerator = new DbvtBroadphase(m_nullPairCache);
				m_raycastAccelerator.m_deferedcollide = true;
			}
			m_worldAabbMin = worldAabbMin;
			m_worldAabbMax = worldAabbMax;
			IndexedVector3 indexedVector = m_worldAabbMax - m_worldAabbMin;
			int handleSentinel2 = m_handleSentinel;
			m_quantize = new IndexedVector3(handleSentinel2) / indexedVector;
			m_pHandles = new Handle[num];
			for (int i = 0; i < m_pHandles.Length; i++)
			{
				m_pHandles[i] = new Handle();
			}
			m_maxHandles = num;
			m_numHandles = 0;
			m_firstFreeHandle = 1;
			for (ushort num2 = m_firstFreeHandle; num2 < num; num2 = (ushort)(num2 + 1))
			{
				ushort nextFree = (ushort)(num2 + 1);
				m_pHandles[num2].SetNextFree(nextFree);
			}
			m_pHandles[num - 1].SetNextFree(0);
			m_pEdges = new Edge[3, num * 2];
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < num * 2; k++)
				{
					m_pEdges[j, k] = new Edge();
				}
			}
			m_pHandles[0].SetClientObject(null);
			for (int l = 0; l < 3; l++)
			{
				m_pHandles[0].m_minEdges[l] = 0;
				m_pHandles[0].m_maxEdges[l] = 1;
				m_pEdges[l, 0].m_pos = 0;
				m_pEdges[l, 0].m_handle = 0;
				m_pEdges[l, 1].m_pos = m_handleSentinel;
				m_pEdges[l, 1].m_handle = 0;
			}
		}

		public virtual void Cleanup()
		{
			if (m_raycastAccelerator != null)
			{
				m_nullPairCache.Cleanup();
				m_nullPairCache = null;
				m_raycastAccelerator.Cleanup();
				m_raycastAccelerator = null;
			}
			for (int i = 0; i < m_pHandles.Length; i++)
			{
				m_pHandles[i].Cleanup();
			}
			m_pHandles = null;
			if (m_ownsPairCache)
			{
				m_pairCache = null;
			}
		}

		public ushort GetNumHandles()
		{
			return m_numHandles;
		}

		public virtual void CalculateOverlappingPairs(IDispatcher dispatcher)
		{
			if (!m_pairCache.HasDeferredRemoval())
			{
				return;
			}
			ObjectArray<BroadphasePair> overlappingPairArray = m_pairCache.GetOverlappingPairArray();
			overlappingPairArray.QuickSort(new BroadphasePairQuickSort());
			m_invalidPair = 0;
			BroadphasePair obj = new BroadphasePair();
			for (int i = 0; i < overlappingPairArray.Count; i++)
			{
				BroadphasePair broadphasePair = overlappingPairArray[i];
				bool flag = broadphasePair.Equals(obj);
				obj = broadphasePair;
				bool flag2 = false;
				if (flag || ((!TestAabbOverlap(broadphasePair.m_pProxy0, broadphasePair.m_pProxy1)) ? true : false))
				{
					m_pairCache.CleanOverlappingPair(broadphasePair, dispatcher);
					broadphasePair.m_pProxy0 = null;
					broadphasePair.m_pProxy1 = null;
					m_invalidPair++;
					OverlappingPairCacheGlobals.gOverlappingPairs--;
				}
			}
			overlappingPairArray.QuickSort(new BroadphasePairQuickSort());
			overlappingPairArray.Resize(overlappingPairArray.Count - m_invalidPair);
			m_invalidPair = 0;
		}

		public ushort AddHandle(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, object pOwner, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			ushort[] array = new ushort[3];
			ushort[] array2 = new ushort[3];
			Quantize(array, ref aabbMin, 0);
			Quantize(array2, ref aabbMax, 1);
			ushort num = AllocHandle();
			Handle handle = GetHandle(num);
			handle.m_uniqueId = num;
			handle.m_clientObject = pOwner;
			handle.m_collisionFilterGroup = collisionFilterGroup;
			handle.m_collisionFilterMask = collisionFilterMask;
			handle.m_multiSapParentProxy = multiSapProxy;
			ushort num2 = (ushort)(m_numHandles * 2);
			for (int i = 0; i < 3; i++)
			{
				m_pHandles[0].m_maxEdges[i] += 2;
				m_pEdges[i, num2 + 1].Copy(m_pEdges[i, num2 - 1]);
				m_pEdges[i, num2 - 1].m_pos = array[i];
				m_pEdges[i, num2 - 1].m_handle = num;
				m_pEdges[i, num2].m_pos = array2[i];
				m_pEdges[i, num2].m_handle = num;
				Edge edge = m_pEdges[i, num2];
				Edge edge2 = m_pEdges[i, num2 - 1];
				Edge edge3 = m_pEdges[i, num2 + 1];
				handle.m_minEdges[i] = (ushort)(num2 - 1);
				handle.m_maxEdges[i] = num2;
			}
			SortMinDown(0, handle.m_minEdges.X, dispatcher, false);
			SortMaxDown(0, handle.m_maxEdges.X, dispatcher, false);
			SortMinDown(1, handle.m_minEdges.Y, dispatcher, false);
			SortMaxDown(1, handle.m_maxEdges.Y, dispatcher, false);
			SortMinDown(2, handle.m_minEdges.Z, dispatcher, true);
			SortMaxDown(2, handle.m_maxEdges.Z, dispatcher, true);
			return num;
		}

		public void RemoveHandle(ushort handle, IDispatcher dispatcher)
		{
			Handle handle2 = GetHandle(handle);
			if (!m_pairCache.HasDeferredRemoval())
			{
				m_pairCache.RemoveOverlappingPairsContainingProxy(handle2, dispatcher);
			}
			int num = m_numHandles * 2;
			for (int i = 0; i < 3; i++)
			{
				m_pHandles[0].m_maxEdges[i] -= 2;
			}
			for (int j = 0; j < 3; j++)
			{
				ushort num2 = handle2.m_maxEdges[j];
				m_pEdges[j, num2].m_pos = m_handleSentinel;
				SortMaxUp(j, num2, dispatcher, false);
				ushort num3 = handle2.m_minEdges[j];
				m_pEdges[j, num3].m_pos = m_handleSentinel;
				SortMinUp(j, num3, dispatcher, false);
				m_pEdges[j, num - 1].m_handle = 0;
				m_pEdges[j, num - 1].m_pos = m_handleSentinel;
			}
			FreeHandle(handle);
		}

		public void UpdateHandle(ushort handle, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			Handle handle2 = GetHandle(handle);
			Quantize(min, ref aabbMin, 0);
			Quantize(max, ref aabbMax, 1);
			for (int i = 0; i < 3; i++)
			{
				ushort num = handle2.m_minEdges[i];
				ushort num2 = handle2.m_maxEdges[i];
				int num3 = min[i] - m_pEdges[i, num].m_pos;
				int num4 = max[i] - m_pEdges[i, num2].m_pos;
				m_pEdges[i, num].m_pos = min[i];
				m_pEdges[i, num2].m_pos = max[i];
				if (num3 < 0)
				{
					SortMinDown(i, num, dispatcher, true);
				}
				if (num4 > 0)
				{
					SortMaxUp(i, num2, dispatcher, true);
				}
				if (num3 > 0)
				{
					SortMinUp(i, num, dispatcher, true);
				}
				if (num4 < 0)
				{
					SortMaxDown(i, num2, dispatcher, true);
				}
			}
		}

		public Handle GetHandle(ushort index)
		{
			return m_pHandles[index];
		}

		public virtual void ResetPool(IDispatcher dispatcher)
		{
			if (m_numHandles == 0)
			{
				m_firstFreeHandle = 1;
				for (ushort num = m_firstFreeHandle; num < m_maxHandles; num = (ushort)(num + 1))
				{
					m_pHandles[num].SetNextFree((ushort)(num + 1));
				}
				m_pHandles[m_maxHandles - 1].SetNextFree(0);
			}
		}

		public void ProcessAllOverlappingPairs(IOverlapCallback callback)
		{
		}

		public virtual BroadphaseProxy CreateProxy(IndexedVector3 aabbMin, IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			return CreateProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, multiSapProxy);
		}

		public virtual BroadphaseProxy CreateProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy)
		{
			ushort index = AddHandle(ref aabbMin, ref aabbMax, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, multiSapProxy);
			Handle handle = GetHandle(index);
			if (m_raycastAccelerator != null)
			{
				BroadphaseProxy dbvtProxy = m_raycastAccelerator.CreateProxy(ref aabbMin, ref aabbMax, shapeType, userPtr, collisionFilterGroup, collisionFilterMask, dispatcher, 0);
				handle.m_dbvtProxy = dbvtProxy;
			}
			return handle;
		}

		public virtual void DestroyProxy(BroadphaseProxy proxy, IDispatcher dispatcher)
		{
			Handle handle = (Handle)proxy;
			if (m_raycastAccelerator != null)
			{
				m_raycastAccelerator.DestroyProxy(handle.m_dbvtProxy, dispatcher);
			}
			RemoveHandle((ushort)handle.GetUid(), dispatcher);
		}

		public virtual void SetAabb(BroadphaseProxy proxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher)
		{
			Handle handle = (Handle)proxy;
			handle.SetMinAABB(ref aabbMin);
			handle.SetMaxAABB(ref aabbMax);
			UpdateHandle((ushort)handle.GetUid(), ref aabbMin, ref aabbMax, dispatcher);
			if (m_raycastAccelerator != null)
			{
				m_raycastAccelerator.SetAabb(handle.m_dbvtProxy, ref aabbMin, ref aabbMax, dispatcher);
			}
		}

		public virtual void GetAabb(BroadphaseProxy proxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			Handle handle = (Handle)proxy;
			aabbMin = handle.GetMinAABB();
			aabbMax = handle.GetMaxAABB();
		}

		public virtual void AabbTest(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IBroadphaseAabbCallback callback)
		{
			if (m_raycastAccelerator != null)
			{
				m_raycastAccelerator.AabbTest(ref aabbMin, ref aabbMax, callback);
				return;
			}
			int num = 0;
			for (int i = 1; i < m_numHandles * 2 + 1; i++)
			{
				if (m_pEdges[num, i].IsMax())
				{
					Handle handle = GetHandle(m_pEdges[num, i].m_handle);
					if (AabbUtil2.TestAabbAgainstAabb2(ref aabbMin, ref aabbMax, ref handle.m_aabbMin, ref handle.m_aabbMax))
					{
						callback.Process(handle);
					}
				}
			}
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback)
		{
			IndexedVector3 aabbMin = MathUtil.MIN_VECTOR;
			IndexedVector3 aabbMax = MathUtil.MAX_VECTOR;
			RayTest(ref rayFrom, ref rayTo, rayCallback, ref aabbMin, ref aabbMax);
		}

		public virtual void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax)
		{
			if (m_raycastAccelerator != null)
			{
				m_raycastAccelerator.RayTest(ref rayFrom, ref rayTo, rayCallback, ref aabbMin, ref aabbMax);
				return;
			}
			ushort num = 0;
			for (int i = 1; i < m_numHandles * 2 + 1; i++)
			{
				if (m_pEdges[num, i].IsMax())
				{
					rayCallback.Process(GetHandle(m_pEdges[num, i].m_handle));
				}
			}
		}

		public void Quantize(ushort[] output, ref IndexedVector3 point, int isMax)
		{
			IndexedVector3 indexedVector = (point - m_worldAabbMin) * m_quantize;
			output[0] = ((indexedVector.X <= 0f) ? ((ushort)isMax) : ((indexedVector.X >= (float)(int)m_handleSentinel) ? ((ushort)((m_handleSentinel & m_bpHandleMask) | isMax)) : ((ushort)(((ushort)indexedVector.X & m_bpHandleMask) | isMax))));
			output[1] = ((indexedVector.Y <= 0f) ? ((ushort)isMax) : ((indexedVector.Y >= (float)(int)m_handleSentinel) ? ((ushort)((m_handleSentinel & m_bpHandleMask) | isMax)) : ((ushort)(((ushort)indexedVector.Y & m_bpHandleMask) | isMax))));
			output[2] = ((indexedVector.Z <= 0f) ? ((ushort)isMax) : ((indexedVector.Z >= (float)(int)m_handleSentinel) ? ((ushort)((m_handleSentinel & m_bpHandleMask) | isMax)) : ((ushort)(((ushort)indexedVector.Z & m_bpHandleMask) | isMax))));
		}

		public void UnQuantize(BroadphaseProxy proxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			Handle handle = (Handle)proxy;
			ushort[] array = new ushort[3];
			ushort[] array2 = new ushort[3];
			array[0] = m_pEdges[0, handle.m_minEdges[0]].m_pos;
			array2[0] = (ushort)(m_pEdges[0, handle.m_maxEdges[0]].m_pos + 1);
			array[1] = m_pEdges[1, handle.m_minEdges[1]].m_pos;
			array2[1] = (ushort)(m_pEdges[1, handle.m_maxEdges[1]].m_pos + 1);
			array[2] = m_pEdges[2, handle.m_minEdges[2]].m_pos;
			array2[2] = (ushort)(m_pEdges[2, handle.m_maxEdges[2]].m_pos + 1);
			aabbMin = new IndexedVector3((float)(int)array[0] / m_quantize.X, (float)(int)array[1] / m_quantize.Y, (float)(int)array[2] / m_quantize.Z);
			aabbMin += m_worldAabbMin;
			aabbMax = new IndexedVector3((float)(int)array2[0] / m_quantize.X, (float)(int)array2[1] / m_quantize.Y, (float)(int)array2[2] / m_quantize.Z);
			aabbMax += m_worldAabbMax;
		}

		public bool TestAabbOverlap(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			Handle handle = (Handle)proxy0;
			Handle handle2 = (Handle)proxy1;
			for (int i = 0; i < 3; i++)
			{
				if (handle.m_maxEdges[i] < handle2.m_minEdges[i] || handle2.m_maxEdges[i] < handle.m_minEdges[i])
				{
					return false;
				}
			}
			return true;
		}

		public IOverlappingPairCache GetOverlappingPairCache()
		{
			return m_pairCache;
		}

		public void SetOverlappingPairUserCallback(IOverlappingPairCallback pairCallback)
		{
			m_userPairCallback = pairCallback;
		}

		public IOverlappingPairCallback GetOverlappingPairUserCallback()
		{
			return m_userPairCallback;
		}

		public virtual void GetBroadphaseAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			aabbMin = m_worldAabbMin;
			aabbMax = m_worldAabbMax;
		}

		public virtual void PrintStats()
		{
		}
	}
}
