namespace BulletXNA.BulletCollision
{
	public class EmptyCreateFunc : CollisionAlgorithmCreateFunc
	{
		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			return new EmptyAlgorithm(ci);
		}
	}
}
