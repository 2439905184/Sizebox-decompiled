using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class InplaceSolverIslandCallback : IIslandCallback
	{
		public ContactSolverInfo m_solverInfo;

		public IConstraintSolver m_solver;

		public ObjectArray<TypedConstraint> m_sortedConstraints;

		public int m_numConstraints;

		public IDebugDraw m_debugDrawer;

		public IDispatcher m_dispatcher;

		public ObjectArray<CollisionObject> m_bodies;

		public PersistentManifoldArray m_manifolds;

		public ObjectArray<TypedConstraint> m_constraints;

		public InplaceSolverIslandCallback(ContactSolverInfo solverInfo, IConstraintSolver solver, ObjectArray<TypedConstraint> sortedConstraints, int numConstraints, IDebugDraw debugDrawer, IDispatcher dispatcher)
		{
			m_solverInfo = solverInfo;
			m_solver = solver;
			m_sortedConstraints = sortedConstraints;
			m_numConstraints = numConstraints;
			m_debugDrawer = debugDrawer;
			m_dispatcher = dispatcher;
			m_bodies = new ObjectArray<CollisionObject>();
			m_manifolds = new PersistentManifoldArray();
			m_constraints = new ObjectArray<TypedConstraint>();
		}

		public void Setup(ContactSolverInfo solverInfo, ObjectArray<TypedConstraint> sortedConstraints, int numConstraints, IDebugDraw debugDrawer)
		{
			m_solverInfo = solverInfo;
			m_sortedConstraints = sortedConstraints;
			m_numConstraints = numConstraints;
			m_debugDrawer = debugDrawer;
			m_bodies.Resize(0);
			m_manifolds.Resize(0);
			m_constraints.Resize(0);
		}

		public virtual void ProcessIsland(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifolds, int startManifold, int numManifolds, int islandId)
		{
			if (islandId < 0)
			{
				if (numManifolds + m_numConstraints > 0)
				{
					m_solver.SolveGroup(bodies, numBodies, manifolds, startManifold, numManifolds, m_sortedConstraints, 0, m_numConstraints, m_solverInfo, m_debugDrawer, m_dispatcher);
				}
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (num3 = 0; num3 < m_numConstraints; num3++)
			{
				if (DiscreteDynamicsWorld.GetConstraintIslandId(m_sortedConstraints[num3]) == islandId)
				{
					num = num3;
					break;
				}
			}
			for (; num3 < m_numConstraints; num3++)
			{
				if (DiscreteDynamicsWorld.GetConstraintIslandId(m_sortedConstraints[num3]) == islandId)
				{
					num2++;
				}
			}
			if (m_solverInfo.m_minimumSolverBatchSize <= 1)
			{
				if (numManifolds + num2 > 0)
				{
					m_solver.SolveGroup(bodies, numBodies, manifolds, startManifold, numManifolds, m_sortedConstraints, num, num2, m_solverInfo, m_debugDrawer, m_dispatcher);
				}
				return;
			}
			for (num3 = 0; num3 < numBodies; num3++)
			{
				m_bodies.Add(bodies[num3]);
			}
			int num4 = startManifold + numManifolds;
			for (num3 = startManifold; num3 < num4; num3++)
			{
				m_manifolds.Add(manifolds[num3]);
			}
			int num5 = num + num2;
			for (num3 = num; num3 < num5; num3++)
			{
				m_constraints.Add(m_sortedConstraints[num3]);
			}
			if (m_constraints.Count + m_manifolds.Count > m_solverInfo.m_minimumSolverBatchSize)
			{
				ProcessConstraints();
			}
		}

		public void ProcessConstraints()
		{
			if (m_manifolds.Count + m_constraints.Count > 0)
			{
				m_solver.SolveGroup(m_bodies, m_bodies.Count, m_manifolds, 0, m_manifolds.Count, m_constraints, 0, m_constraints.Count, m_solverInfo, m_debugDrawer, m_dispatcher);
			}
			m_bodies.Clear();
			m_manifolds.Clear();
			m_constraints.Clear();
		}
	}
}
