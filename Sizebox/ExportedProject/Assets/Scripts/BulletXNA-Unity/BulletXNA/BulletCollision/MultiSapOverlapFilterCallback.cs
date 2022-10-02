namespace BulletXNA.BulletCollision
{
	public class MultiSapOverlapFilterCallback : IOverlapFilterCallback
	{
		public virtual bool NeedBroadphaseCollision(BroadphaseProxy childProxy0, BroadphaseProxy childProxy1)
		{
			BroadphaseProxy broadphaseProxy = (BroadphaseProxy)childProxy0.m_multiSapParentProxy;
			BroadphaseProxy broadphaseProxy2 = (BroadphaseProxy)childProxy1.m_multiSapParentProxy;
			return (broadphaseProxy.m_collisionFilterGroup & broadphaseProxy2.m_collisionFilterMask) != 0 && (broadphaseProxy2.m_collisionFilterGroup & broadphaseProxy.m_collisionFilterMask) != 0;
		}
	}
}
