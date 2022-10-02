namespace BulletXNA.BulletCollision
{
	public class SwappedSphereBoxCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			SphereBoxCollisionAlgorithm sphereBoxCollisionAlgorithm = BulletGlobals.SphereBoxCollisionAlgorithmPool.Get();
			sphereBoxCollisionAlgorithm.Initialize(null, ci, body0, body1, true);
			return sphereBoxCollisionAlgorithm;
		}
	}
}
