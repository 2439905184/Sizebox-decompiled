namespace BulletXNA.BulletCollision
{
	public class GImpactCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			GImpactCollisionAlgorithm gImpactCollisionAlgorithm = BulletGlobals.GImpactCollisionAlgorithmPool.Get();
			gImpactCollisionAlgorithm.Initialize(ci, body0, body1);
			return gImpactCollisionAlgorithm;
		}
	}
}
