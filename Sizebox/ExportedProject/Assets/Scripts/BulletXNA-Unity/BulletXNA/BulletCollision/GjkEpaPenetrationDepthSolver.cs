using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class GjkEpaPenetrationDepthSolver : IConvexPenetrationDepthSolver
	{
		public virtual bool CalcPenDepth(ISimplexSolverInterface simplexSolver, ConvexShape convexA, ConvexShape convexB, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 v, ref IndexedVector3 wWitnessOnA, ref IndexedVector3 wWitnessOnB, IDebugDraw debugDraw)
		{
			IndexedVector3 guess = transA._origin - transB._origin;
			GjkEpaSolver2Results results = default(GjkEpaSolver2Results);
			if (GjkEpaSolver2.Penetration(convexA, ref transA, convexB, ref transB, ref guess, ref results))
			{
				wWitnessOnA = results.witnesses0;
				wWitnessOnB = results.witnesses1;
				v = results.normal;
				return true;
			}
			if (GjkEpaSolver2.Distance(convexA, ref transA, convexB, ref transB, ref guess, ref results))
			{
				wWitnessOnA = results.witnesses0;
				wWitnessOnB = results.witnesses1;
				v = results.normal;
				return false;
			}
			return false;
		}
	}
}
