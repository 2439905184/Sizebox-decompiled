namespace BulletXNA.BulletCollision
{
	public class ConvexConvexCreateFunc : CollisionAlgorithmCreateFunc
	{
		public IConvexPenetrationDepthSolver m_pdSolver;

		public ISimplexSolverInterface m_simplexSolver;

		public int m_numPerturbationIterations;

		public int m_minimumPointsPerturbationThreshold;

		public ConvexConvexCreateFunc(ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver depthSolver)
		{
			m_numPerturbationIterations = 0;
			m_minimumPointsPerturbationThreshold = 3;
			m_simplexSolver = simplexSolver;
			m_pdSolver = depthSolver;
		}

		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			ConvexConvexAlgorithm convexConvexAlgorithm = BulletGlobals.ConvexConvexAlgorithmPool.Get();
			convexConvexAlgorithm.Initialize(ci.GetManifold(), ci, body0, body1, m_simplexSolver, m_pdSolver, m_numPerturbationIterations, m_minimumPointsPerturbationThreshold);
			return convexConvexAlgorithm;
		}
	}
}
