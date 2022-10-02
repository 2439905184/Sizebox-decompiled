namespace BulletXNA.BulletCollision
{
	public interface IOverlappingPairCallback
	{
		BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1);

		object RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, IDispatcher dispatcher);

		void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, IDispatcher dispatcher);
	}
}
