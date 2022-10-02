using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IOverlappingPairCache : IOverlappingPairCallback
	{
		ObjectArray<BroadphasePair> GetOverlappingPairArray();

		void CleanOverlappingPair(BroadphasePair pair, IDispatcher dispatcher);

		int GetNumOverlappingPairs();

		void CleanProxyFromPairs(BroadphaseProxy proxy, IDispatcher dispatcher);

		void SetOverlapFilterCallback(IOverlapFilterCallback callback);

		void ProcessAllOverlappingPairs(IOverlapCallback callback, IDispatcher dispatcher);

		BroadphasePair FindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1);

		bool HasDeferredRemoval();

		void SetInternalGhostPairCallback(IOverlappingPairCallback ghostPairCallback);

		void SortOverlappingPairs(IDispatcher dispatcher);

		void Cleanup();
	}
}
