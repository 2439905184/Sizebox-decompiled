using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MultiSapProxy : BroadphaseProxy
	{
		public ObjectArray<BridgeProxy> m_bridgeProxies;

		public BroadphaseNativeTypes m_shapeType;

		public MultiSapProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask)
			: base(ref aabbMin, ref aabbMax, userPtr, collisionFilterGroup, collisionFilterMask, null)
		{
			m_aabbMin = aabbMin;
			m_aabbMax = aabbMax;
			m_shapeType = shapeType;
			m_multiSapParentProxy = this;
		}
	}
}
