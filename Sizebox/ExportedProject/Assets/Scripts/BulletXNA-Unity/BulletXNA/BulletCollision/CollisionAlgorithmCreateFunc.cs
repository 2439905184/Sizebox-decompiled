namespace BulletXNA.BulletCollision
{
	public abstract class CollisionAlgorithmCreateFunc
	{
		public bool m_swapped;

		public virtual CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo caci, CollisionObject body0, CollisionObject body1)
		{
			return null;
		}
	}
}
