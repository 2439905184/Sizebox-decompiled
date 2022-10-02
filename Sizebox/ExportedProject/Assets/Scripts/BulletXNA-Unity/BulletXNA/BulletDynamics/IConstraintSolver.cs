using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public interface IConstraintSolver
	{
		void PrepareSolve(int numBodies, int numManifolds);

		float SolveGroup(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifold, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo info, IDebugDraw debugDrawer, IDispatcher dispatcher);

		void AllSolved(ContactSolverInfo info, IDebugDraw debugDrawer);

		void Reset();

		void Cleanup();
	}
}
