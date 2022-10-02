using System.Collections.Generic;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public abstract class SimpleDynamicsWorld : DynamicsWorld
	{
		protected IConstraintSolver m_constraintSolver;

		protected bool m_ownsConstraintSolver;

		protected IndexedVector3 m_gravity;

		public SimpleDynamicsWorld(IDispatcher dispatcher, IBroadphaseInterface pairCache, IConstraintSolver constraintSolver, ICollisionConfiguration collisionConfiguration)
			: base(dispatcher, pairCache, collisionConfiguration)
		{
			m_constraintSolver = constraintSolver;
			m_ownsConstraintSolver = false;
			IndexedVector3 gravity = new IndexedVector3(0f, 0f, -10f);
			SetGravity(ref gravity);
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (m_ownsConstraintSolver)
			{
				m_constraintSolver.Cleanup();
				m_constraintSolver = null;
			}
		}

		public override int StepSimulation(float timeStep, int maxSubSteps, float fixedTimeStep)
		{
			PredictUnconstraintMotion(timeStep);
			DispatcherInfo dispatchInfo = GetDispatchInfo();
			dispatchInfo.SetTimeStep(timeStep);
			dispatchInfo.SetStepCount(0);
			dispatchInfo.SetDebugDraw(GetDebugDrawer());
			PerformDiscreteCollisionDetection();
			int numManifolds = m_dispatcher1.GetNumManifolds();
			if (numManifolds != 0)
			{
				PersistentManifoldArray internalManifoldPointer = (m_dispatcher1 as CollisionDispatcher).GetInternalManifoldPointer();
				ContactSolverInfo contactSolverInfo = new ContactSolverInfo();
				contactSolverInfo.m_timeStep = timeStep;
				m_constraintSolver.PrepareSolve(0, numManifolds);
				m_constraintSolver.SolveGroup(null, 0, internalManifoldPointer, 0, numManifolds, null, 0, 0, contactSolverInfo, m_debugDrawer, m_dispatcher1);
				m_constraintSolver.AllSolved(contactSolverInfo, m_debugDrawer);
			}
			IntegrateTransforms(timeStep);
			UpdateAabbs();
			SynchronizeMotionStates();
			ClearForces();
			return 1;
		}

		public override void SetGravity(ref IndexedVector3 gravity)
		{
			m_gravity = gravity;
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				RigidBody rigidBody = RigidBody.Upcast(item);
				if (rigidBody != null)
				{
					rigidBody.SetGravity(ref gravity);
				}
			}
		}

		public override IndexedVector3 GetGravity()
		{
			return m_gravity;
		}

		public override void AddRigidBody(RigidBody body)
		{
			body.SetGravity(ref m_gravity);
			if (body.GetCollisionShape() != null)
			{
				AddCollisionObject(body);
			}
		}

		public override void AddRigidBody(RigidBody body, CollisionFilterGroups group, CollisionFilterGroups mask)
		{
			body.SetGravity(ref m_gravity);
			if (body.GetCollisionShape() != null)
			{
				AddCollisionObject(body, group, mask);
			}
		}

		public override void DebugDrawWorld()
		{
		}

		public override void AddAction(IActionInterface action)
		{
		}

		public override void RemoveAction(IActionInterface action)
		{
		}

		public override void RemoveRigidBody(RigidBody body)
		{
			base.RemoveCollisionObject(body);
		}

		public override void RemoveCollisionObject(CollisionObject collisionObject)
		{
			RigidBody rigidBody = RigidBody.Upcast(collisionObject);
			if (rigidBody != null)
			{
				RemoveRigidBody(rigidBody);
			}
			else
			{
				base.RemoveCollisionObject(collisionObject);
			}
		}

		public override void UpdateAabbs()
		{
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				RigidBody rigidBody = RigidBody.Upcast(item);
				if (rigidBody != null && rigidBody.IsActive() && !rigidBody.IsStaticObject())
				{
					IndexedVector3 aabbMin;
					IndexedVector3 aabbMax;
					item.GetCollisionShape().GetAabb(item.GetWorldTransform(), out aabbMin, out aabbMax);
					IBroadphaseInterface broadphase = GetBroadphase();
					broadphase.SetAabb(rigidBody.GetBroadphaseHandle(), ref aabbMin, ref aabbMax, m_dispatcher1);
				}
			}
		}

		public override void SynchronizeMotionStates()
		{
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				RigidBody rigidBody = RigidBody.Upcast(item);
				if (rigidBody != null && rigidBody.GetMotionState() != null && rigidBody.GetActivationState() != ActivationState.ISLAND_SLEEPING)
				{
					rigidBody.GetMotionState().SetWorldTransform(rigidBody.GetWorldTransform());
				}
			}
		}

		public override void SetConstraintSolver(IConstraintSolver solver)
		{
			m_ownsConstraintSolver = false;
			m_constraintSolver = solver;
		}

		public override IConstraintSolver GetConstraintSolver()
		{
			return m_constraintSolver;
		}

		public override DynamicsWorldType GetWorldType()
		{
			return DynamicsWorldType.BT_SIMPLE_DYNAMICS_WORLD;
		}

		public override void ClearForces()
		{
			for (int i = 0; i < m_collisionObjects.Count; i++)
			{
				CollisionObject colObj = m_collisionObjects[i];
				RigidBody rigidBody = RigidBody.Upcast(colObj);
				if (rigidBody != null)
				{
					rigidBody.ClearForces();
				}
			}
		}

		protected void PredictUnconstraintMotion(float timeStep)
		{
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				RigidBody rigidBody = RigidBody.Upcast(item);
				if (rigidBody != null && !rigidBody.IsStaticObject() && rigidBody.IsActive())
				{
					rigidBody.ApplyGravity();
					rigidBody.IntegrateVelocities(timeStep);
					rigidBody.ApplyDamping(timeStep);
					IndexedMatrix predictedTransform = rigidBody.GetInterpolationWorldTransform();
					rigidBody.PredictIntegratedTransform(timeStep, out predictedTransform);
					rigidBody.SetInterpolationWorldTransform(ref predictedTransform);
				}
			}
		}

		protected void IntegrateTransforms(float timeStep)
		{
			foreach (CollisionObject item in (IEnumerable<CollisionObject>)m_collisionObjects)
			{
				RigidBody rigidBody = RigidBody.Upcast(item);
				if (rigidBody != null && rigidBody.IsActive() && !rigidBody.IsStaticObject())
				{
					IndexedMatrix predictedTransform;
					rigidBody.PredictIntegratedTransform(timeStep, out predictedTransform);
					rigidBody.ProceedToTransform(ref predictedTransform);
				}
			}
		}
	}
}
