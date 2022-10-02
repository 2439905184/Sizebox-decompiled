namespace BulletXNA.BulletCollision
{
	public class CompoundCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			CompoundCollisionAlgorithm compoundCollisionAlgorithm = BulletGlobals.CompoundCollisionAlgorithmPool.Get();
			compoundCollisionAlgorithm.Initialize(ci, body0, body1, false);
			return compoundCollisionAlgorithm;
		}
	}
}
