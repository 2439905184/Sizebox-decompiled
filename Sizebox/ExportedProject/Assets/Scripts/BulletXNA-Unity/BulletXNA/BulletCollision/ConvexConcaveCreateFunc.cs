namespace BulletXNA.BulletCollision
{
	public class ConvexConcaveCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			ConvexConcaveCollisionAlgorithm convexConcaveCollisionAlgorithm = BulletGlobals.ConvexConcaveCollisionAlgorithmPool.Get();
			convexConcaveCollisionAlgorithm.Inititialize(ci, body0, body1, false);
			return convexConcaveCollisionAlgorithm;
		}
	}
}
