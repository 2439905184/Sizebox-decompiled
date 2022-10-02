namespace BulletXNA.BulletCollision
{
	public class SphereSphereCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			SphereSphereCollisionAlgorithm sphereSphereCollisionAlgorithm = BulletGlobals.SphereSphereCollisionAlgorithmPool.Get();
			sphereSphereCollisionAlgorithm.Initialize(null, ci, body0, body1);
			return sphereSphereCollisionAlgorithm;
		}
	}
}
