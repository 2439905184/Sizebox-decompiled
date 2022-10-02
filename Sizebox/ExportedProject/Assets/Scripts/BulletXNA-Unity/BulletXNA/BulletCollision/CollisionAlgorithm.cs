using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public abstract class CollisionAlgorithm
	{
		protected IDispatcher m_dispatcher;

		public int colAgorithmId;

		public CollisionAlgorithm()
		{
			BulletGlobals.s_collisionAlgorithmInstanceCount++;
			colAgorithmId = BulletGlobals.s_collisionAlgorithmInstanceCount;
		}

		public CollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
		{
			m_dispatcher = ci.GetDispatcher();
			BulletGlobals.s_collisionAlgorithmInstanceCount++;
			colAgorithmId = BulletGlobals.s_collisionAlgorithmInstanceCount;
		}

		public virtual void Initialize(CollisionAlgorithmConstructionInfo ci)
		{
			m_dispatcher = ci.GetDispatcher();
			BulletGlobals.s_collisionAlgorithmInstanceCount++;
			colAgorithmId = BulletGlobals.s_collisionAlgorithmInstanceCount;
		}

		public abstract void ProcessCollision(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut);

		public abstract float CalculateTimeOfImpact(CollisionObject body0, CollisionObject body1, DispatcherInfo dispatchInfo, ManifoldResult resultOut);

		public abstract void GetAllContactManifolds(PersistentManifoldArray manifoldArray);

		public virtual void Cleanup()
		{
		}

		protected int GetDispatcherId()
		{
			return 1;
		}
	}
}
