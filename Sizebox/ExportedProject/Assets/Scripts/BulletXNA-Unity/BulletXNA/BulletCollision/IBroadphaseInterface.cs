using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IBroadphaseInterface
	{
		BroadphaseProxy CreateProxy(IndexedVector3 aabbMin, IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy);

		BroadphaseProxy CreateProxy(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, BroadphaseNativeTypes shapeType, object userPtr, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask, IDispatcher dispatcher, object multiSapProxy);

		void DestroyProxy(BroadphaseProxy proxy, IDispatcher dispatcher);

		void SetAabb(BroadphaseProxy proxy, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IDispatcher dispatcher);

		void GetAabb(BroadphaseProxy proxy, out IndexedVector3 aabbMin, out IndexedVector3 aabbMax);

		void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback);

		void RayTest(ref IndexedVector3 rayFrom, ref IndexedVector3 rayTo, BroadphaseRayCallback rayCallback, ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax);

		void AabbTest(ref IndexedVector3 aabbMin, ref IndexedVector3 aabbMax, IBroadphaseAabbCallback callback);

		void CalculateOverlappingPairs(IDispatcher dispatcher);

		IOverlappingPairCache GetOverlappingPairCache();

		void GetBroadphaseAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax);

		void ResetPool(IDispatcher dispatcher);

		void PrintStats();

		void Cleanup();
	}
}
