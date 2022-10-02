namespace BulletXNA.BulletCollision
{
	public class DefaultNearCallback : INearCallback
	{
		public void NearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo)
		{
			CollisionObject collisionObject = collisionPair.m_pProxy0.GetClientObject() as CollisionObject;
			CollisionObject collisionObject2 = collisionPair.m_pProxy1.GetClientObject() as CollisionObject;
			if (!dispatcher.NeedsCollision(collisionObject, collisionObject2))
			{
				return;
			}
			if (collisionPair.m_algorithm == null)
			{
				collisionPair.m_algorithm = dispatcher.FindAlgorithm(collisionObject, collisionObject2, null);
			}
			if (collisionPair.m_algorithm == null)
			{
				return;
			}
			ManifoldResult newManifoldResult = dispatcher.GetNewManifoldResult(collisionObject, collisionObject2);
			if (dispatchInfo.GetDispatchFunc() == DispatchFunc.DISPATCH_DISCRETE)
			{
				collisionPair.m_algorithm.ProcessCollision(collisionObject, collisionObject2, dispatchInfo, newManifoldResult);
			}
			else
			{
				float num = collisionPair.m_algorithm.CalculateTimeOfImpact(collisionObject, collisionObject2, dispatchInfo, newManifoldResult);
				if (dispatchInfo.GetTimeOfImpact() > num)
				{
					dispatchInfo.SetTimeOfImpact(num);
				}
			}
			dispatcher.FreeManifoldResult(newManifoldResult);
		}
	}
}
