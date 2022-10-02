namespace BulletXNA.BulletCollision
{
	public abstract class ActivatingCollisionAlgorithm : CollisionAlgorithm
	{
		public ActivatingCollisionAlgorithm()
		{
		}

		public ActivatingCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
			: base(ci)
		{
		}

		public override void Initialize(CollisionAlgorithmConstructionInfo ci)
		{
			base.Initialize(ci);
		}

		public ActivatingCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject colObj0, CollisionObject colObj1)
			: base(ci)
		{
		}

		public virtual void Initialize(CollisionAlgorithmConstructionInfo ci, CollisionObject colObj0, CollisionObject colObj1)
		{
			base.Initialize(ci);
		}
	}
}
