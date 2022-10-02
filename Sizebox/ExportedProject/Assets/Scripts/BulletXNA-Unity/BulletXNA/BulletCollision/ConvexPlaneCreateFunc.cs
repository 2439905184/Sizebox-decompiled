namespace BulletXNA.BulletCollision
{
	public class ConvexPlaneCreateFunc : CollisionAlgorithmCreateFunc
	{
		public int m_numPerturbationIterations;

		public int m_minimumPointsPerturbationThreshold;

		public ConvexPlaneCreateFunc()
		{
			m_numPerturbationIterations = 1;
			m_minimumPointsPerturbationThreshold = 0;
		}

		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			ConvexPlaneCollisionAlgorithm convexPlaneCollisionAlgorithm = BulletGlobals.ConvexPlaneAlgorithmPool.Get();
			if (!m_swapped)
			{
				convexPlaneCollisionAlgorithm.Initialize(null, ci, body0, body1, false, m_numPerturbationIterations, m_minimumPointsPerturbationThreshold);
			}
			else
			{
				convexPlaneCollisionAlgorithm.Initialize(null, ci, body0, body1, true, m_numPerturbationIterations, m_minimumPointsPerturbationThreshold);
			}
			return convexPlaneCollisionAlgorithm;
		}
	}
}
