using System.Collections.Generic;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class DiscreteDynamicsWorld : DynamicsWorld
	{
		protected const float s_fixedTimeStep = 1f / 60f;

		protected IConstraintSolver m_constraintSolver;

		protected InplaceSolverIslandCallback m_solverIslandCallback;

		protected SimulationIslandManager m_islandManager;

		protected ObjectArray<TypedConstraint> m_constraints;

		protected ObjectArray<TypedConstraint> m_sortedConstraints;

		protected SortConstraintOnIslandPredicate m_islandSortPredicate;

		protected QuickSortConstraintOnIslandPredicate m_islandQuickSortPredicate;

		protected ObjectArray<RigidBody> m_nonStaticRigidBodies;

		protected IndexedVector3 m_gravity;

		protected float m_localTime;

		protected bool m_ownsIslandManager;

		protected bool m_ownsConstraintSolver;

		protected bool m_synchronizeAllMotionStates;

		protected IList<IActionInterface> m_actions;

		protected int m_profileTimings;

		protected int gNumClampedCcdMotions;

		public DiscreteDynamicsWorld(IDispatcher dispatcher, IBroadphaseInterface pairCache, IConstraintSolver constraintSolver, ICollisionConfiguration collisionConfiguration)
			: base(dispatcher, pairCache, collisionConfiguration)
		{
			m_ownsIslandManager = true;
			m_constraints = new ObjectArray<TypedConstraint>();
			m_sortedConstraints = new ObjectArray<TypedConstraint>();
			m_islandSortPredicate = new SortConstraintOnIslandPredicate();
			m_islandQuickSortPredicate = new QuickSortConstraintOnIslandPredicate();
			m_actions = new List<IActionInterface>();
			m_nonStaticRigidBodies = new ObjectArray<RigidBody>();
			m_islandManager = new SimulationIslandManager();
			m_constraintSolver = constraintSolver;
			IndexedVector3 gravity = new IndexedVector3(0f, -10f, 0f);
			SetGravity(ref gravity);
			m_localTime = 0f;
			m_profileTimings = 0;
			m_synchronizeAllMotionStates = false;
			if (m_constraintSolver == null)
			{
				m_constraintSolver = new SequentialImpulseConstraintSolver();
				m_ownsConstraintSolver = true;
			}
			else
			{
				m_ownsConstraintSolver = false;
			}
		}

		public override void Cleanup()
		{
			base.Cleanup();
			if (m_ownsIslandManager)
			{
				m_islandManager.Cleanup();
				m_islandManager = null;
				m_ownsIslandManager = false;
			}
			if (m_ownsConstraintSolver)
			{
				m_constraintSolver.Cleanup();
				m_constraintSolver = null;
				m_ownsConstraintSolver = false;
			}
		}

		public override int StepSimulation(float timeStep, int maxSubSteps)
		{
			return StepSimulation(timeStep, maxSubSteps, 1f / 60f);
		}

		public override int StepSimulation(float timeStep, int maxSubSteps, float fixedTimeStep)
		{
			StartProfiling(timeStep);
			BulletGlobals.StartProfile("stepSimulation");
			int num = 0;
			if (maxSubSteps != 0)
			{
				m_localTime += timeStep;
				if (m_localTime >= fixedTimeStep)
				{
					num = (int)(m_localTime / fixedTimeStep);
					m_localTime -= (float)num * fixedTimeStep;
				}
			}
			else
			{
				fixedTimeStep = timeStep;
				m_localTime = timeStep;
				if (MathUtil.FuzzyZero(timeStep))
				{
					num = 0;
					maxSubSteps = 0;
				}
				else
				{
					num = 1;
					maxSubSteps = 1;
				}
			}
			if (GetDebugDrawer() != null)
			{
				IDebugDraw debugDrawer = GetDebugDrawer();
				BulletGlobals.gDisableDeactivation = (debugDrawer.GetDebugMode() & DebugDrawModes.DBG_NoDeactivation) != 0;
			}
			if (num != 0)
			{
				int num2 = ((num > maxSubSteps) ? maxSubSteps : num);
				SaveKinematicState(fixedTimeStep * (float)num2);
				ApplyGravity();
				for (int i = 0; i < num2; i++)
				{
					InternalSingleStepSimulation(fixedTimeStep);
					SynchronizeMotionStates();
				}
			}
			else
			{
				SynchronizeMotionStates();
			}
			ClearForces();
			if (m_profileManager != null)
			{
				m_profileManager.Increment_Frame_Counter();
			}
			return num;
		}

		public override void SynchronizeMotionStates()
		{
			if (m_synchronizeAllMotionStates)
			{
				int count = m_collisionObjects.Count;
				for (int i = 0; i < count; i++)
				{
					RigidBody rigidBody = RigidBody.Upcast(m_collisionObjects[i]);
					if (rigidBody != null)
					{
						SynchronizeSingleMotionState(rigidBody);
					}
				}
				return;
			}
			int count2 = m_nonStaticRigidBodies.Count;
			for (int j = 0; j < count2; j++)
			{
				RigidBody rigidBody2 = m_nonStaticRigidBodies[j];
				if (rigidBody2.IsActive())
				{
					SynchronizeSingleMotionState(rigidBody2);
				}
			}
		}

		public void SynchronizeSingleMotionState(RigidBody body)
		{
			if (body.GetMotionState() != null && !body.IsStaticOrKinematicObject())
			{
				IndexedMatrix predictedTransform;
				TransformUtil.IntegrateTransform(body.GetInterpolationWorldTransform(), body.SetInterpolationLinearVelocity(), body.GetInterpolationAngularVelocity(), m_localTime * body.GetHitFraction(), out predictedTransform);
				body.GetMotionState().SetWorldTransform(ref predictedTransform);
			}
		}

		public override void AddConstraint(TypedConstraint constraint, bool disableCollisionsBetweenLinkedBodies)
		{
			m_constraints.Add(constraint);
			if (disableCollisionsBetweenLinkedBodies)
			{
				constraint.GetRigidBodyA().AddConstraintRef(constraint);
				constraint.GetRigidBodyB().AddConstraintRef(constraint);
			}
		}

		public override void RemoveConstraint(TypedConstraint constraint)
		{
			m_constraints.Remove(constraint);
			constraint.GetRigidBodyA().RemoveConstraintRef(constraint);
			constraint.GetRigidBodyB().RemoveConstraintRef(constraint);
		}

		public override void AddAction(IActionInterface action)
		{
			m_actions.Add(action);
		}

		public override void RemoveAction(IActionInterface action)
		{
			m_actions.Remove(action);
		}

		public SimulationIslandManager GetSimulationIslandManager()
		{
			return m_islandManager;
		}

		public CollisionWorld GetCollisionWorld()
		{
			return this;
		}

		public override void SetGravity(IndexedVector3 gravity)
		{
			SetGravity(ref gravity);
		}

		public override void SetGravity(ref IndexedVector3 gravity)
		{
			m_gravity = gravity;
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < count; i++)
			{
				RigidBody rigidBody = m_nonStaticRigidBodies[i];
				if (rigidBody.IsActive() && (rigidBody.GetFlags() & RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY) == 0)
				{
					rigidBody.SetGravity(ref gravity);
				}
			}
		}

		public override IndexedVector3 GetGravity()
		{
			return m_gravity;
		}

		public override void AddCollisionObject(CollisionObject collisionObject)
		{
			AddCollisionObject(collisionObject, CollisionFilterGroups.StaticFilter, ~CollisionFilterGroups.StaticFilter);
		}

		public override void AddCollisionObject(CollisionObject collisionObject, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask)
		{
			base.AddCollisionObject(collisionObject, collisionFilterGroup, collisionFilterMask);
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

		public void SetSynchronizeAllMotionStates(bool synchronizeAll)
		{
			m_synchronizeAllMotionStates = synchronizeAll;
		}

		public bool GetSynchronizeAllMotionStates()
		{
			return m_synchronizeAllMotionStates;
		}

		public override void AddRigidBody(RigidBody body)
		{
			if (!body.IsStaticOrKinematicObject() && (body.GetFlags() & RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY) == 0)
			{
				body.SetGravity(ref m_gravity);
			}
			if (body.GetCollisionShape() != null)
			{
				if (!body.IsStaticObject())
				{
					m_nonStaticRigidBodies.Add(body);
				}
				else
				{
					body.SetActivationState(ActivationState.ISLAND_SLEEPING);
				}
				bool flag = !body.IsStaticObject() && !body.IsKinematicObject();
				CollisionFilterGroups collisionFilterGroup = (flag ? CollisionFilterGroups.DefaultFilter : CollisionFilterGroups.StaticFilter);
				CollisionFilterGroups collisionFilterMask = (flag ? CollisionFilterGroups.AllFilter : (~CollisionFilterGroups.StaticFilter));
				AddCollisionObject(body, collisionFilterGroup, collisionFilterMask);
			}
		}

		public override void AddRigidBody(RigidBody body, CollisionFilterGroups group, CollisionFilterGroups mask)
		{
			if (!body.IsStaticOrKinematicObject() && (body.GetFlags() & RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY) == 0)
			{
				body.SetGravity(ref m_gravity);
			}
			if (body.GetCollisionShape() == null)
			{
				return;
			}
			if (!body.IsStaticObject())
			{
				if (!m_nonStaticRigidBodies.Contains(body))
				{
					m_nonStaticRigidBodies.Add(body);
				}
			}
			else
			{
				body.SetActivationState(ActivationState.ISLAND_SLEEPING);
			}
			AddCollisionObject(body, group, mask);
		}

		public override void RemoveRigidBody(RigidBody body)
		{
			m_nonStaticRigidBodies.Remove(body);
			base.RemoveCollisionObject(body);
		}

		public override void DebugDrawWorld()
		{
			BulletGlobals.StartProfile("debugDrawWorld");
			base.DebugDrawWorld();
			if (GetDebugDrawer() != null)
			{
				DebugDrawModes debugMode = GetDebugDrawer().GetDebugMode();
				if ((debugMode & (DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits)) != 0)
				{
					for (int num = GetNumConstraints() - 1; num >= 0; num--)
					{
						TypedConstraint constraint = GetConstraint(num);
						DrawHelper.DebugDrawConstraint(constraint, GetDebugDrawer());
					}
				}
				if (debugMode != 0)
				{
					int count = m_actions.Count;
					for (int i = 0; i < count; i++)
					{
						m_actions[i].DebugDraw(m_debugDrawer);
					}
				}
			}
			BulletGlobals.StopProfile();
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

		public override int GetNumConstraints()
		{
			return m_constraints.Count;
		}

		public override TypedConstraint GetConstraint(int index)
		{
			return m_constraints[index];
		}

		public override DynamicsWorldType GetWorldType()
		{
			return DynamicsWorldType.BT_DISCRETE_DYNAMICS_WORLD;
		}

		public override void ClearForces()
		{
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < count; i++)
			{
				m_nonStaticRigidBodies[i].ClearForces();
			}
		}

		public virtual void ApplyGravity()
		{
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < count; i++)
			{
				RigidBody rigidBody = m_nonStaticRigidBodies[i];
				if (rigidBody != null && rigidBody.IsActive())
				{
					rigidBody.ApplyGravity();
				}
			}
		}

		public virtual void SetNumTasks(int numTasks)
		{
		}

		public virtual void UpdateVehicles(float timeStep)
		{
			UpdateActions(timeStep);
		}

		public override void AddVehicle(IActionInterface vehicle)
		{
			AddAction(vehicle);
		}

		public override void RemoveVehicle(IActionInterface vehicle)
		{
			RemoveAction(vehicle);
		}

		public override void AddCharacter(IActionInterface character)
		{
			AddAction(character);
		}

		public override void RemoveCharacter(IActionInterface character)
		{
			RemoveAction(character);
		}

		protected virtual void PredictUnconstraintMotion(float timeStep)
		{
			BulletGlobals.StartProfile("predictUnconstraintMotion");
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < m_nonStaticRigidBodies.Count; i++)
			{
				RigidBody rigidBody = m_nonStaticRigidBodies[i];
				if (!rigidBody.IsStaticOrKinematicObject())
				{
					rigidBody.IntegrateVelocities(timeStep);
					rigidBody.ApplyDamping(timeStep);
					IndexedMatrix predictedTransform;
					rigidBody.PredictIntegratedTransform(timeStep, out predictedTransform);
					rigidBody.SetInterpolationWorldTransform(ref predictedTransform);
				}
			}
			BulletGlobals.StopProfile();
		}

		protected virtual void IntegrateTransforms(float timeStep)
		{
			BulletGlobals.StartProfile("integrateTransforms");
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < count; i++)
			{
				RigidBody rigidBody = m_nonStaticRigidBodies[i];
				if (rigidBody == null)
				{
					continue;
				}
				rigidBody.SetHitFraction(1f);
				if (!rigidBody.IsActive() || rigidBody.IsStaticOrKinematicObject())
				{
					continue;
				}
				IndexedMatrix newTrans;
				rigidBody.PredictIntegratedTransform(timeStep, out newTrans);
				float num = (newTrans._origin - rigidBody.GetWorldTransform()._origin).LengthSquared();
				if (GetDispatchInfo().m_useContinuous && rigidBody.GetCcdSquareMotionThreshold() != 0f && rigidBody.GetCcdSquareMotionThreshold() < num)
				{
					BulletGlobals.StartProfile("CCD motion clamping");
					if (rigidBody.GetCollisionShape().IsConvex())
					{
						gNumClampedCcdMotions++;
						using (ClosestNotMeConvexResultCallback closestNotMeConvexResultCallback = BulletGlobals.ClosestNotMeConvexResultCallbackPool.Get())
						{
							closestNotMeConvexResultCallback.Initialize(rigidBody, rigidBody.GetWorldTransform()._origin, newTrans._origin, GetBroadphase().GetOverlappingPairCache(), GetDispatcher());
							SphereShape sphereShape = BulletGlobals.SphereShapePool.Get();
							sphereShape.Initialize(rigidBody.GetCcdSweptSphereRadius());
							closestNotMeConvexResultCallback.m_allowedPenetration = GetDispatchInfo().GetAllowedCcdPenetration();
							closestNotMeConvexResultCallback.m_collisionFilterGroup = rigidBody.GetBroadphaseProxy().m_collisionFilterGroup;
							closestNotMeConvexResultCallback.m_collisionFilterMask = rigidBody.GetBroadphaseProxy().m_collisionFilterMask;
							IndexedMatrix convexToWorld = newTrans;
							convexToWorld._basis = rigidBody.GetWorldTransform()._basis;
							convexToWorld._origin = newTrans._origin;
							ConvexSweepTest(sphereShape, rigidBody.GetWorldTransform(), convexToWorld, closestNotMeConvexResultCallback, 0f);
							if (closestNotMeConvexResultCallback.HasHit() && closestNotMeConvexResultCallback.m_closestHitFraction < 1f)
							{
								rigidBody.SetHitFraction(closestNotMeConvexResultCallback.m_closestHitFraction);
								rigidBody.PredictIntegratedTransform(timeStep * rigidBody.GetHitFraction(), out newTrans);
								rigidBody.SetHitFraction(0f);
								rigidBody.ProceedToTransform(ref newTrans);
								float distance = 0f;
								ContactConstraint.ResolveSingleCollision(rigidBody, closestNotMeConvexResultCallback.m_hitCollisionObject, ref closestNotMeConvexResultCallback.m_hitPointWorld, ref closestNotMeConvexResultCallback.m_hitNormalWorld, GetSolverInfo(), distance);
								continue;
							}
							BulletGlobals.SphereShapePool.Free(sphereShape);
							goto IL_021f;
						}
					}
					goto IL_021f;
				}
				goto IL_0224;
				IL_0224:
				rigidBody.ProceedToTransform(ref newTrans);
				continue;
				IL_021f:
				BulletGlobals.StopProfile();
				goto IL_0224;
			}
		}

		protected virtual void CalculateSimulationIslands()
		{
			BulletGlobals.StartProfile("calculateSimulationIslands");
			GetSimulationIslandManager().UpdateActivationState(GetCollisionWorld(), GetCollisionWorld().GetDispatcher());
			int count = m_constraints.Count;
			for (int i = 0; i < count; i++)
			{
				TypedConstraint typedConstraint = m_constraints[i];
				RigidBody rigidBodyA = typedConstraint.GetRigidBodyA();
				RigidBody rigidBodyB = typedConstraint.GetRigidBodyB();
				if (rigidBodyA != null && !rigidBodyA.IsStaticOrKinematicObject() && rigidBodyB != null && !rigidBodyB.IsStaticOrKinematicObject() && (rigidBodyA.IsActive() || rigidBodyB.IsActive()))
				{
					GetSimulationIslandManager().GetUnionFind().Unite(rigidBodyA.GetIslandTag(), rigidBodyB.GetIslandTag());
				}
			}
			GetSimulationIslandManager().StoreIslandActivationState(GetCollisionWorld());
			BulletGlobals.StopProfile();
		}

		protected virtual void SolveConstraints(ContactSolverInfo solverInfo)
		{
			m_sortedConstraints.Resize(m_constraints.Count);
			int numConstraints = GetNumConstraints();
			for (int i = 0; i < numConstraints; i++)
			{
				m_sortedConstraints[i] = m_constraints[i];
			}
			if (numConstraints > 1)
			{
				m_sortedConstraints.QuickSort(m_islandQuickSortPredicate);
			}
			if (m_solverIslandCallback == null)
			{
				m_solverIslandCallback = new InplaceSolverIslandCallback(solverInfo, m_constraintSolver, m_sortedConstraints, GetNumConstraints(), m_debugDrawer, m_dispatcher1);
			}
			else
			{
				m_solverIslandCallback.Setup(solverInfo, m_sortedConstraints, numConstraints, m_debugDrawer);
			}
			m_constraintSolver.PrepareSolve(GetCollisionWorld().GetNumCollisionObjects(), GetCollisionWorld().GetDispatcher().GetNumManifolds());
			m_islandManager.BuildAndProcessIslands(GetCollisionWorld().GetDispatcher(), GetCollisionWorld(), m_solverIslandCallback);
			m_solverIslandCallback.ProcessConstraints();
			m_constraintSolver.AllSolved(solverInfo, m_debugDrawer);
		}

		protected void UpdateActivationState(float timeStep)
		{
			BulletGlobals.StartProfile("updateActivationState");
			int count = m_nonStaticRigidBodies.Count;
			for (int i = 0; i < count; i++)
			{
				RigidBody rigidBody = m_nonStaticRigidBodies[i];
				if (rigidBody == null)
				{
					continue;
				}
				rigidBody.UpdateDeactivation(timeStep);
				if (rigidBody.WantsSleeping())
				{
					if (rigidBody.IsStaticOrKinematicObject())
					{
						rigidBody.SetActivationState(ActivationState.ISLAND_SLEEPING);
						continue;
					}
					if (rigidBody.GetActivationState() == ActivationState.ACTIVE_TAG)
					{
						rigidBody.SetActivationState(ActivationState.WANTS_DEACTIVATION);
					}
					if (rigidBody.GetActivationState() == ActivationState.ISLAND_SLEEPING)
					{
						IndexedVector3 ang_vel = IndexedVector3.Zero;
						rigidBody.SetAngularVelocity(ref ang_vel);
						rigidBody.SetLinearVelocity(ref ang_vel);
					}
				}
				else if (rigidBody.GetActivationState() != ActivationState.DISABLE_DEACTIVATION)
				{
					rigidBody.SetActivationState(ActivationState.ACTIVE_TAG);
				}
			}
			BulletGlobals.StopProfile();
		}

		protected void UpdateActions(float timeStep)
		{
			BulletGlobals.StartProfile("updateActions");
			int count = m_actions.Count;
			for (int i = 0; i < count; i++)
			{
				m_actions[i].UpdateAction(this, timeStep);
			}
			BulletGlobals.StopProfile();
		}

		protected void StartProfiling(float timeStep)
		{
			BulletGlobals.ResetProfile();
		}

		protected virtual void InternalSingleStepSimulation(float timeStep)
		{
			BulletGlobals.StartProfile("internalSingleStepSimulation");
			if (m_internalPreTickCallback != null)
			{
				m_internalPreTickCallback.InternalTickCallback(this, timeStep);
			}
			PredictUnconstraintMotion(timeStep);
			DispatcherInfo dispatchInfo = GetDispatchInfo();
			dispatchInfo.SetTimeStep(timeStep);
			dispatchInfo.SetStepCount(0);
			dispatchInfo.SetDebugDraw(GetDebugDrawer());
			PerformDiscreteCollisionDetection();
			CalculateSimulationIslands();
			GetSolverInfo().m_timeStep = timeStep;
			SolveConstraints(GetSolverInfo());
			IntegrateTransforms(timeStep);
			UpdateActions(timeStep);
			UpdateActivationState(timeStep);
			if (m_internalTickCallback != null)
			{
				m_internalTickCallback.InternalTickCallback(this, timeStep);
			}
			BulletGlobals.StopProfile();
		}

		protected virtual void SaveKinematicState(float timeStep)
		{
			int count = m_collisionObjects.Count;
			for (int i = 0; i < count; i++)
			{
				RigidBody rigidBody = RigidBody.Upcast(m_collisionObjects[i]);
				if (rigidBody != null && rigidBody.GetActivationState() != ActivationState.ISLAND_SLEEPING && rigidBody.IsKinematicObject())
				{
					rigidBody.SaveKinematicState(timeStep);
				}
			}
		}

		public static int GetConstraintIslandId(TypedConstraint lhs)
		{
			CollisionObject rigidBodyA = lhs.GetRigidBodyA();
			CollisionObject rigidBodyB = lhs.GetRigidBodyB();
			return (rigidBodyA.GetIslandTag() >= 0) ? rigidBodyA.GetIslandTag() : rigidBodyB.GetIslandTag();
		}
	}
}
