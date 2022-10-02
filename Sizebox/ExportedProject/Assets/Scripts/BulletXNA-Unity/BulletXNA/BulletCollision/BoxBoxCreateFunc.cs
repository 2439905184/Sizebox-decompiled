namespace BulletXNA.BulletCollision
{
	public class BoxBoxCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			BoxBoxCollisionAlgorithm boxBoxCollisionAlgorithm = BulletGlobals.BoxBoxCollisionAlgorithmPool.Get();
			boxBoxCollisionAlgorithm.Initialize(null, ci, body0, body1);
			return boxBoxCollisionAlgorithm;
		}
	}
}
