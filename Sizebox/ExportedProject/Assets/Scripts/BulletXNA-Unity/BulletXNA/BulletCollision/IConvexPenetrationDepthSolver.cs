using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface IConvexPenetrationDepthSolver
	{
		bool CalcPenDepth(ISimplexSolverInterface simplexSolver, ConvexShape convexA, ConvexShape convexB, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 v, ref IndexedVector3 pa, ref IndexedVector3 pb, IDebugDraw debugDraw);
	}
}
