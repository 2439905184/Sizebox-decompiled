using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class BroadphaseProxy
	{
		public object m_clientObject;

		public CollisionFilterGroups m_collisionFilterGroup;

		public CollisionFilterGroups m_collisionFilterMask;

		public object m_multiSapParentProxy;

		public int m_uniqueId;

		public IndexedVector3 m_aabbMin;

		public IndexedVector3 m_aabbMax;

		public int GetUid()
		{
			return m_uniqueId;
		}

		public BroadphaseProxy()
		{
			m_clientObject = null;
			m_multiSapParentProxy = null;
		}

		public BroadphaseProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, object multiSapParentProxy)
		{
			m_clientObject = userPtr;
			m_collisionFilterGroup = collisionFilterGroup;
			m_collisionFilterMask = collisionFilterMask;
			m_aabbMin = aabbMin;
			m_aabbMax = aabbMax;
			m_multiSapParentProxy = multiSapParentProxy;
		}

		public static bool IsPolyhedral(BroadphaseNativeTypes proxyType)
		{
			return proxyType < BroadphaseNativeTypes.IMPLICIT_CONVEX_SHAPES_START_HERE;
		}

		public static bool IsConvex(BroadphaseNativeTypes proxyType)
		{
			return proxyType < BroadphaseNativeTypes.CONCAVE_SHAPES_START_HERE;
		}

		public static bool IsNonMoving(BroadphaseNativeTypes proxyType)
		{
			if (IsConcave(proxyType))
			{
				return proxyType != BroadphaseNativeTypes.GIMPACT_SHAPE_PROXYTYPE;
			}
			return false;
		}

		public static bool IsConcave(BroadphaseNativeTypes proxyType)
		{
			if (proxyType > BroadphaseNativeTypes.CONCAVE_SHAPES_START_HERE)
			{
				return proxyType < BroadphaseNativeTypes.CONCAVE_SHAPES_END_HERE;
			}
			return false;
		}

		public static bool IsCompound(BroadphaseNativeTypes proxyType)
		{
			return proxyType == BroadphaseNativeTypes.COMPOUND_SHAPE_PROXYTYPE;
		}

		public static bool IsSoftBody(BroadphaseNativeTypes proxyType)
		{
			return proxyType == BroadphaseNativeTypes.SOFTBODY_SHAPE_PROXYTYPE;
		}

		public static bool IsInfinite(BroadphaseNativeTypes proxyType)
		{
			return proxyType == BroadphaseNativeTypes.STATIC_PLANE_PROXYTYPE;
		}

		public static bool IsConvex2d(BroadphaseNativeTypes proxyType)
		{
			if (proxyType != BroadphaseNativeTypes.BOX_2D_SHAPE_PROXYTYPE)
			{
				return proxyType == BroadphaseNativeTypes.CONVEX_2D_SHAPE_PROXYTYPE;
			}
			return true;
		}

		public IndexedVector3 GetMinAABB()
		{
			return m_aabbMin;
		}

		public IndexedVector3 GetMaxAABB()
		{
			return m_aabbMax;
		}

		public void SetMinAABB(ref IndexedVector3 min)
		{
			m_aabbMin = min;
		}

		public void SetMaxAABB(ref IndexedVector3 max)
		{
			m_aabbMax = max;
		}

		public object GetClientObject()
		{
			return m_clientObject;
		}

		public void SetClientObject(object o)
		{
			m_clientObject = o;
		}

		public virtual void Cleanup()
		{
		}
	}
}
