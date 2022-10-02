namespace BulletXNA.BulletCollision
{
	public class GhostPairCallback : IOverlappingPairCallback
	{
		public virtual void Cleanup()
		{
		}

		public virtual BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
		{
			CollisionObject colObj = proxy0.m_clientObject as CollisionObject;
			CollisionObject colObj2 = proxy1.m_clientObject as CollisionObject;
			GhostObject ghostObject = GhostObject.Upcast(colObj);
			GhostObject ghostObject2 = GhostObject.Upcast(colObj2);
			if (ghostObject != null)
			{
				ghostObject.AddOverlappingObjectInternal(proxy1, proxy0);
			}
			if (ghostObject2 != null)
			{
				ghostObject2.AddOverlappingObjectInternal(proxy0, proxy1);
			}
			return null;
		}

		public virtual object RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, IDispatcher dispatcher)
		{
			CollisionObject colObj = proxy0.m_clientObject as CollisionObject;
			CollisionObject colObj2 = proxy1.m_clientObject as CollisionObject;
			GhostObject ghostObject = GhostObject.Upcast(colObj);
			GhostObject ghostObject2 = GhostObject.Upcast(colObj2);
			if (ghostObject != null)
			{
				ghostObject.RemoveOverlappingObjectInternal(proxy1, dispatcher, proxy0);
			}
			if (ghostObject2 != null)
			{
				ghostObject2.RemoveOverlappingObjectInternal(proxy0, dispatcher, proxy1);
			}
			return null;
		}

		public virtual void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, IDispatcher dispatcher)
		{
		}
	}
}
