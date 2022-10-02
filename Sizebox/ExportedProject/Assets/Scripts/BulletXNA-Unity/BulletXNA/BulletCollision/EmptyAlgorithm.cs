using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class EmptyAlgorithm : CollisionAlgorithm
	{
		public EmptyAlgorithm(CollisionAlgorithmConstructionInfo ci)
		{
		}

		public override void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
		}

		public override float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		public override void GetAllContactManifolds(PersistentManifoldArray manifoldArray)
		{
		}

		public override void Cleanup()
		{
		}
	}
}
