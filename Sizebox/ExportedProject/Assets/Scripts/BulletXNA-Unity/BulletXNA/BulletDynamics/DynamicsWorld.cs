using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public abstract class DynamicsWorld : CollisionWorld
	{
		protected IInternalTickCallback m_internalTickCallback;

		protected IInternalTickCallback m_internalPreTickCallback;

		protected object m_worldUserInfo;

		protected ContactSolverInfo m_solverInfo;

		public DynamicsWorld(IDispatcher dispatcher, IBroadphaseInterface broadphase, ICollisionConfiguration collisionConfiguration)
			: base(dispatcher, broadphase, collisionConfiguration)
		{
			m_internalTickCallback = null;
			m_worldUserInfo = null;
			m_solverInfo = new ContactSolverInfo();
		}

		public abstract int StepSimulation(float timeStep, int maxSubSteps);

		public abstract int StepSimulation(float timeStep, int maxSubSteps, float fixedTimeStep);

		public override void DebugDrawWorld()
		{
			base.DebugDrawWorld();
		}

		public virtual void AddConstraint(TypedConstraint constraint)
		{
			AddConstraint(constraint, false);
		}

		public virtual void AddConstraint(TypedConstraint constraint, bool disableCollisionsBetweenLinkedBodies)
		{
		}

		public virtual void RemoveConstraint(TypedConstraint constraint)
		{
		}

		public abstract void AddAction(IActionInterface action);

		public abstract void RemoveAction(IActionInterface action);

		public abstract void SetGravity(ref IndexedVector3 gravity);

		public abstract void SetGravity(IndexedVector3 gravity);

		public abstract IndexedVector3 GetGravity();

		public abstract void SynchronizeMotionStates();

		public abstract void AddRigidBody(RigidBody body);

		public abstract void AddRigidBody(RigidBody body, CollisionFilterGroups group, CollisionFilterGroups mask);

		public abstract void RemoveRigidBody(RigidBody body);

		public abstract void SetConstraintSolver(IConstraintSolver solver);

		public abstract IConstraintSolver GetConstraintSolver();

		public virtual int GetNumConstraints()
		{
			return 0;
		}

		public virtual TypedConstraint GetConstraint(int index)
		{
			return null;
		}

		public abstract DynamicsWorldType GetWorldType();

		public abstract void ClearForces();

		public void SetInternalTickCallback(IInternalTickCallback cb, object worldUserInfo, bool isPreTick)
		{
			if (isPreTick)
			{
				m_internalPreTickCallback = cb;
			}
			else
			{
				m_internalTickCallback = cb;
			}
			m_worldUserInfo = worldUserInfo;
		}

		public void SetWorldUserInfo(object worldUserInfo)
		{
			m_worldUserInfo = worldUserInfo;
		}

		public object GetWorldUserInfo()
		{
			return m_worldUserInfo;
		}

		public ContactSolverInfo GetSolverInfo()
		{
			return m_solverInfo;
		}

		public virtual void AddVehicle(IActionInterface vehicle)
		{
		}

		public virtual void RemoveVehicle(IActionInterface vehicle)
		{
		}

		public virtual void AddCharacter(IActionInterface character)
		{
		}

		public virtual void RemoveCharacter(IActionInterface character)
		{
		}
	}
}
