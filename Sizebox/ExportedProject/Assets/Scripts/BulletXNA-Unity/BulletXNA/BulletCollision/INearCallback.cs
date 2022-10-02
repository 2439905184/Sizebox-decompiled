namespace BulletXNA.BulletCollision
{
	public interface INearCallback
	{
		void NearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo);
	}
}
