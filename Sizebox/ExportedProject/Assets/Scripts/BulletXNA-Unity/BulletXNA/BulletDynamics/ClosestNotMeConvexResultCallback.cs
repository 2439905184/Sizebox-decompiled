using System;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class ClosestNotMeConvexResultCallback : ClosestConvexResultCallback, IDisposable
	{
		public CollisionObject m_me;

		public float m_allowedPenetration;

		public IOverlappingPairCache m_pairCache;

		public IDispatcher m_dispatcher;

		public ClosestNotMeConvexResultCallback()
		{
		}

		public ClosestNotMeConvexResultCallback(CollisionObject me, IndexedVector3 fromA, IndexedVector3 toA, IOverlappingPairCache pairCache, IDispatcher dispatcher)
			: this(me, ref fromA, ref toA, pairCache, dispatcher)
		{
		}

		public virtual void Initialize(CollisionObject me, ref IndexedVector3 fromA, ref IndexedVector3 toA, IOverlappingPairCache pairCache, IDispatcher dispatcher)
		{
			base.Initialize(ref fromA, ref toA);
			m_allowedPenetration = 0f;
			m_me = me;
			m_pairCache = pairCache;
			m_dispatcher = dispatcher;
		}

		public virtual void Initialize(CollisionObject me, IndexedVector3 fromA, IndexedVector3 toA, IOverlappingPairCache pairCache, IDispatcher dispatcher)
		{
			base.Initialize(ref fromA, ref toA);
			m_allowedPenetration = 0f;
			m_me = me;
			m_pairCache = pairCache;
			m_dispatcher = dispatcher;
		}

		public ClosestNotMeConvexResultCallback(CollisionObject me, ref IndexedVector3 fromA, ref IndexedVector3 toA, IOverlappingPairCache pairCache, IDispatcher dispatcher)
			: base(ref fromA, ref toA)
		{
			m_allowedPenetration = 0f;
			m_me = me;
			m_pairCache = pairCache;
			m_dispatcher = dispatcher;
		}

		public override float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace)
		{
			if (convexResult.m_hitCollisionObject == m_me)
			{
				return 1f;
			}
			if (!convexResult.m_hitCollisionObject.HasContactResponse())
			{
				return 1f;
			}
			IndexedVector3 indexedVector = m_convexToWorld - m_convexFromWorld;
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 b = indexedVector - zero;
			if (IndexedVector3.Dot(convexResult.m_hitNormalLocal, b) >= 0f - m_allowedPenetration)
			{
				return 1f;
			}
			return base.AddSingleResult(ref convexResult, normalInWorldSpace);
		}

		public override bool NeedsCollision(BroadphaseProxy proxy0)
		{
			if (proxy0.m_clientObject == m_me)
			{
				return false;
			}
			if (!base.NeedsCollision(proxy0))
			{
				return false;
			}
			CollisionObject body = proxy0.m_clientObject as CollisionObject;
			if (m_dispatcher.NeedsResponse(m_me, body))
			{
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			BulletGlobals.ClosestNotMeConvexResultCallbackPool.Free(this);
		}
	}
}
