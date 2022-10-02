namespace BulletXNA.BulletCollision
{
	public class Convex2dConvex2dCreateFunc : CollisionAlgorithmCreateFunc
	{
		private IConvexPenetrationDepthSolver m_pdSolver;

		private ISimplexSolverInterface m_simplexSolver;

		private int m_numPerturbationIterations;

		private int m_minimumPointsPerturbationThreshold;

		public Convex2dConvex2dCreateFunc(ISimplexSolverInterface simplexSolver, IConvexPenetrationDepthSolver pdSolver)
		{
			m_numPerturbationIterations = 0;
			m_minimumPointsPerturbationThreshold = 3;
			m_simplexSolver = simplexSolver;
			m_pdSolver = pdSolver;
		}

		public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObject body0, CollisionObject body1)
		{
			return new Convex2dConvex2dAlgorithm(ci.GetManifold(), ci, body0, body1, m_simplexSolver, m_pdSolver, m_numPerturbationIterations, m_minimumPointsPerturbationThreshold);
		}
	}
}
