using System;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class SequentialImpulseConstraintSolver : IConstraintSolver
	{
		private static RigidBody s_fixed;

		protected ObjectArray<SolverConstraint> m_tmpSolverContactConstraintPool;

		protected ObjectArray<SolverConstraint> m_tmpSolverNonContactConstraintPool;

		protected ObjectArray<SolverConstraint> m_tmpSolverContactFrictionConstraintPool;

		protected ObjectArray<int> m_orderTmpConstraintPool;

		protected ObjectArray<int> m_orderNonContactConstraintPool;

		protected ObjectArray<int> m_orderFrictionConstraintPool;

		protected ObjectArray<ConstraintInfo1> m_tmpConstraintSizesPool;

		protected ObjectArray<ConstraintInfo2> m_tmpConstraintInfo2Pool;

		protected int m_maxOverrideNumSolverIterations;

		private static int gNumSplitImpulseRecoveries = 0;

		protected ulong m_btSeed2;

		protected int m_counter;

		protected int m_lowerLimitCount;

		protected int m_genericCount;

		protected int m_setupCount;

		protected int m_iterCount;

		protected int m_finishCount;

		public SequentialImpulseConstraintSolver()
		{
			m_tmpSolverContactConstraintPool = new ObjectArray<SolverConstraint>();
			m_tmpSolverNonContactConstraintPool = new ObjectArray<SolverConstraint>();
			m_tmpSolverContactFrictionConstraintPool = new ObjectArray<SolverConstraint>();
			m_tmpConstraintSizesPool = new ObjectArray<ConstraintInfo1>();
			m_orderTmpConstraintPool = new ObjectArray<int>();
			m_orderFrictionConstraintPool = new ObjectArray<int>();
			m_orderNonContactConstraintPool = new ObjectArray<int>();
			m_tmpConstraintInfo2Pool = new ObjectArray<ConstraintInfo2>();
		}

		public virtual void Cleanup()
		{
		}

		public void SetupFrictionConstraint(ref SolverConstraint solverConstraint, ref IndexedVector3 normalAxis, RigidBody solverBodyA, RigidBody solverBodyB, ManifoldPoint cp, ref IndexedVector3 rel_pos1, ref IndexedVector3 rel_pos2, CollisionObject colObj0, CollisionObject colObj1, float relaxation)
		{
			SetupFrictionConstraint(ref solverConstraint, ref normalAxis, solverBodyA, solverBodyB, cp, ref rel_pos1, ref rel_pos2, colObj0, colObj1, relaxation, 0f, 0f);
		}

		public void SetupFrictionConstraint(ref SolverConstraint solverConstraint, ref IndexedVector3 normalAxis, RigidBody solverBodyA, RigidBody solverBodyB, ManifoldPoint cp, ref IndexedVector3 rel_pos1, ref IndexedVector3 rel_pos2, CollisionObject colObj0, CollisionObject colObj1, float relaxation, float desiredVelocity, float cfmSlip)
		{
			RigidBody rigidBody = RigidBody.Upcast(colObj0);
			RigidBody rigidBody2 = RigidBody.Upcast(colObj1);
			solverConstraint.m_contactNormal = normalAxis;
			solverConstraint.m_solverBodyA = ((rigidBody != null) ? rigidBody : GetFixedBody());
			solverConstraint.m_solverBodyB = ((rigidBody2 != null) ? rigidBody2 : GetFixedBody());
			solverConstraint.m_friction = cp.GetCombinedFriction();
			solverConstraint.m_originalContactPoint = null;
			solverConstraint.m_appliedImpulse = 0f;
			solverConstraint.m_appliedPushImpulse = 0f;
			IndexedVector3 indexedVector = IndexedVector3.Cross(rel_pos1, solverConstraint.m_contactNormal);
			solverConstraint.m_relpos1CrossNormal = indexedVector;
			solverConstraint.m_angularComponentA = ((rigidBody != null) ? (rigidBody.GetInvInertiaTensorWorld() * indexedVector * rigidBody.GetAngularFactor()) : IndexedVector3.Zero);
			IndexedVector3 indexedVector2 = IndexedVector3.Cross(rel_pos2, -solverConstraint.m_contactNormal);
			solverConstraint.m_relpos2CrossNormal = indexedVector2;
			solverConstraint.m_angularComponentB = ((rigidBody2 != null) ? (rigidBody2.GetInvInertiaTensorWorld() * indexedVector2 * rigidBody2.GetAngularFactor()) : IndexedVector3.Zero);
			float num = 0f;
			float num2 = 0f;
			if (rigidBody != null)
			{
				IndexedVector3 b = IndexedVector3.Cross(solverConstraint.m_angularComponentA, rel_pos1);
				num = rigidBody.GetInvMass() + IndexedVector3.Dot(normalAxis, b);
			}
			if (rigidBody2 != null)
			{
				IndexedVector3 b = IndexedVector3.Cross(-solverConstraint.m_angularComponentB, rel_pos2);
				num2 = rigidBody2.GetInvMass() + IndexedVector3.Dot(normalAxis, b);
			}
			float jacDiagABInv = relaxation / (num + num2);
			solverConstraint.m_jacDiagABInv = jacDiagABInv;
			float num3 = IndexedVector3.Dot(solverConstraint.m_contactNormal, (rigidBody != null) ? rigidBody.GetLinearVelocity() : IndexedVector3.Zero) + IndexedVector3.Dot(solverConstraint.m_relpos1CrossNormal, (rigidBody != null) ? rigidBody.GetAngularVelocity() : IndexedVector3.Zero);
			float num4 = 0f - IndexedVector3.Dot(solverConstraint.m_contactNormal, (rigidBody2 != null) ? rigidBody2.GetLinearVelocity() : IndexedVector3.Zero) + IndexedVector3.Dot(solverConstraint.m_relpos2CrossNormal, (rigidBody2 != null) ? rigidBody2.GetAngularVelocity() : IndexedVector3.Zero);
			float num5 = num3 + num4;
			float num6 = desiredVelocity - num5;
			float num7 = 1f;
			float rhs = num6 * solverConstraint.m_jacDiagABInv * num7;
			solverConstraint.m_rhs = rhs;
			solverConstraint.m_cfm = cfmSlip;
			solverConstraint.m_lowerLimit = 0f;
			solverConstraint.m_upperLimit = 1E+10f;
		}

		public SolverConstraint AddFrictionConstraint(ref IndexedVector3 normalAxis, RigidBody solverBodyA, RigidBody solverBodyB, int frictionIndex, ManifoldPoint cp, ref IndexedVector3 rel_pos1, ref IndexedVector3 rel_pos2, CollisionObject colObj0, CollisionObject colObj1, float relaxation, float desiredVelocity, float cfmSlip)
		{
			SolverConstraint solverConstraint = m_tmpSolverContactFrictionConstraintPool[m_tmpSolverContactFrictionConstraintPool.Count];
			solverConstraint.Reset();
			solverConstraint.m_frictionIndex = frictionIndex;
			SetupFrictionConstraint(ref solverConstraint, ref normalAxis, solverBodyA, solverBodyB, cp, ref rel_pos1, ref rel_pos2, colObj0, colObj1, relaxation, desiredVelocity, cfmSlip);
			return solverConstraint;
		}

		protected void SetupContactConstraint(ref SolverConstraint solverConstraint, CollisionObject colObj0, CollisionObject colObj1, ManifoldPoint cp, ContactSolverInfo infoGlobal, ref IndexedVector3 vel, ref float rel_vel, ref float relaxation, out IndexedVector3 rel_pos1, out IndexedVector3 rel_pos2)
		{
			RigidBody rigidBody = colObj0 as RigidBody;
			RigidBody rigidBody2 = colObj1 as RigidBody;
			IndexedVector3 positionWorldOnA = cp.GetPositionWorldOnA();
			IndexedVector3 positionWorldOnB = cp.GetPositionWorldOnB();
			rel_pos1 = positionWorldOnA - colObj0.m_worldTransform._origin;
			rel_pos2 = positionWorldOnB - colObj1.m_worldTransform._origin;
			relaxation = 1f;
			IndexedVector3 indexedVector = new IndexedVector3(rel_pos1.Y * cp.m_normalWorldOnB.Z - rel_pos1.Z * cp.m_normalWorldOnB.Y, rel_pos1.Z * cp.m_normalWorldOnB.X - rel_pos1.X * cp.m_normalWorldOnB.Z, rel_pos1.X * cp.m_normalWorldOnB.Y - rel_pos1.Y * cp.m_normalWorldOnB.X);
			IndexedVector3 indexedVector2 = new IndexedVector3(rel_pos2.Y * cp.m_normalWorldOnB.Z - rel_pos2.Z * cp.m_normalWorldOnB.Y, rel_pos2.Z * cp.m_normalWorldOnB.X - rel_pos2.X * cp.m_normalWorldOnB.Z, rel_pos2.X * cp.m_normalWorldOnB.Y - rel_pos2.Y * cp.m_normalWorldOnB.X);
			solverConstraint.m_angularComponentA = ((rigidBody != null) ? (rigidBody.GetInvInertiaTensorWorld() * indexedVector * rigidBody.GetAngularFactor()) : IndexedVector3.Zero);
			solverConstraint.m_angularComponentB = ((rigidBody2 != null) ? (rigidBody2.GetInvInertiaTensorWorld() * -indexedVector2 * rigidBody2.GetAngularFactor()) : IndexedVector3.Zero);
			float num = 0f;
			float num2 = 0f;
			if (rigidBody != null)
			{
				IndexedVector3 b = IndexedVector3.Cross(ref solverConstraint.m_angularComponentA, ref rel_pos1);
				num = rigidBody.GetInvMass() + IndexedVector3.Dot(cp.m_normalWorldOnB, b);
			}
			if (rigidBody2 != null)
			{
				IndexedVector3 b = IndexedVector3.Cross(-solverConstraint.m_angularComponentB, rel_pos2);
				num2 = rigidBody2.GetInvMass() + IndexedVector3.Dot(cp.m_normalWorldOnB, b);
			}
			float jacDiagABInv = relaxation / (num + num2);
			solverConstraint.m_jacDiagABInv = jacDiagABInv;
			solverConstraint.m_contactNormal = cp.m_normalWorldOnB;
			solverConstraint.m_relpos1CrossNormal = IndexedVector3.Cross(rel_pos1, cp.m_normalWorldOnB);
			solverConstraint.m_relpos2CrossNormal = IndexedVector3.Cross(rel_pos2, -cp.m_normalWorldOnB);
			IndexedVector3 indexedVector3 = ((rigidBody != null) ? rigidBody.GetVelocityInLocalPoint(ref rel_pos1) : IndexedVector3.Zero);
			IndexedVector3 indexedVector4 = ((rigidBody2 != null) ? rigidBody2.GetVelocityInLocalPoint(ref rel_pos2) : IndexedVector3.Zero);
			vel = indexedVector3 - indexedVector4;
			rel_vel = IndexedVector3.Dot(cp.m_normalWorldOnB, vel);
			float num3 = cp.GetDistance() + infoGlobal.m_linearSlop;
			solverConstraint.m_friction = cp.GetCombinedFriction();
			float num4 = 0f;
			if (cp.GetLifeTime() > infoGlobal.m_restingContactRestitutionThreshold)
			{
				num4 = 0f;
			}
			else
			{
				num4 = RestitutionCurve(rel_vel, cp.GetCombinedResitution());
				if (num4 <= 0f)
				{
					num4 = 0f;
				}
			}
			if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_WARMSTARTING))
			{
				solverConstraint.m_appliedImpulse = cp.GetAppliedImpulse() * infoGlobal.m_warmstartingFactor;
				if (rigidBody != null)
				{
					IndexedVector3 contactNormal = solverConstraint.m_contactNormal;
					rigidBody.InternalApplyImpulse(solverConstraint.m_contactNormal * rigidBody.GetInvMass() * rigidBody.GetLinearFactor(), solverConstraint.m_angularComponentA, solverConstraint.m_appliedImpulse, "SetupContactConstraint-rb0");
				}
				if (rigidBody2 != null)
				{
					rigidBody2.InternalApplyImpulse(solverConstraint.m_contactNormal * rigidBody2.GetInvMass() * rigidBody2.GetLinearFactor(), -solverConstraint.m_angularComponentB, 0f - solverConstraint.m_appliedImpulse, "SetupContactConstraint-rb1");
				}
			}
			else
			{
				solverConstraint.m_appliedImpulse = 0f;
			}
			solverConstraint.m_appliedPushImpulse = 0f;
			float num5 = 0f;
			float num6 = IndexedVector3.Dot(solverConstraint.m_contactNormal, (rigidBody != null) ? rigidBody.GetLinearVelocity() : IndexedVector3.Zero) + IndexedVector3.Dot(solverConstraint.m_relpos1CrossNormal, (rigidBody != null) ? rigidBody.GetAngularVelocity() : IndexedVector3.Zero);
			float num7 = 0f - IndexedVector3.Dot(solverConstraint.m_contactNormal, (rigidBody2 != null) ? rigidBody2.GetLinearVelocity() : IndexedVector3.Zero) + IndexedVector3.Dot(solverConstraint.m_relpos2CrossNormal, (rigidBody2 != null) ? rigidBody2.GetAngularVelocity() : IndexedVector3.Zero);
			num5 = num6 + num7;
			float num8 = 0f;
			float num12 = 20f;
			float num9 = num4 - num5;
			if (num3 > 0f)
			{
				num8 = 0f;
				num9 -= num3 / infoGlobal.m_timeStep;
			}
			else
			{
				num8 = (0f - num3) * infoGlobal.m_erp / infoGlobal.m_timeStep;
			}
			float num10 = num8 * solverConstraint.m_jacDiagABInv;
			float num11 = num9 * solverConstraint.m_jacDiagABInv;
			if (!infoGlobal.m_splitImpulse || num3 > infoGlobal.m_splitImpulsePenetrationThreshold)
			{
				solverConstraint.m_rhs = num10 + num11;
				solverConstraint.m_rhsPenetration = 0f;
			}
			else
			{
				solverConstraint.m_rhs = num11;
				solverConstraint.m_rhsPenetration = num10;
			}
			solverConstraint.m_cfm = 0f;
			solverConstraint.m_lowerLimit = 0f;
			solverConstraint.m_upperLimit = 1E+10f;
		}

		protected void SetFrictionConstraintImpulse(ref SolverConstraint solverConstraint, RigidBody rb0, RigidBody rb1, ManifoldPoint cp, ContactSolverInfo infoGlobal)
		{
			if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_FRICTION_WARMSTARTING))
			{
				SolverConstraint solverConstraint2 = m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex];
				if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_WARMSTARTING))
				{
					solverConstraint2.m_appliedImpulse = cp.m_appliedImpulseLateral1 * infoGlobal.m_warmstartingFactor;
					if (rb0 != null)
					{
						rb0.InternalApplyImpulse(solverConstraint2.m_contactNormal * rb0.GetInvMass(), solverConstraint2.m_angularComponentA, solverConstraint2.m_appliedImpulse, "SetupFriction-rb0");
					}
					if (rb1 != null)
					{
						rb1.InternalApplyImpulse(solverConstraint2.m_contactNormal * rb1.GetInvMass(), -solverConstraint2.m_angularComponentB, 0f - solverConstraint2.m_appliedImpulse, "SetupFriction-rb1");
					}
				}
				else
				{
					solverConstraint2.m_appliedImpulse = 0f;
				}
				m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex] = solverConstraint2;
				if (!TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_2_FRICTION_DIRECTIONS))
				{
					return;
				}
				SolverConstraint solverConstraint3 = m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex + 1];
				if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_WARMSTARTING))
				{
					solverConstraint3.m_appliedImpulse = cp.m_appliedImpulseLateral2 * infoGlobal.m_warmstartingFactor;
					if (rb0 != null)
					{
						rb0.InternalApplyImpulse(solverConstraint3.m_contactNormal * rb0.GetInvMass(), solverConstraint3.m_angularComponentA, solverConstraint3.m_appliedImpulse, "SetFriction-rb0");
					}
					if (rb1 != null)
					{
						rb1.InternalApplyImpulse(solverConstraint3.m_contactNormal * rb1.GetInvMass(), -solverConstraint3.m_angularComponentB, 0f - solverConstraint3.m_appliedImpulse, "SetFriction-rb1");
					}
				}
				else
				{
					solverConstraint3.m_appliedImpulse = 0f;
				}
				m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex + 1] = solverConstraint3;
			}
			else
			{
				SolverConstraint solverConstraint4 = m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex];
				solverConstraint4.m_appliedImpulse = 0f;
				if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_2_FRICTION_DIRECTIONS))
				{
					SolverConstraint solverConstraint5 = m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex + 1];
					solverConstraint5.m_appliedImpulse = 0f;
					m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex + 1] = solverConstraint5;
				}
				m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex] = solverConstraint4;
			}
		}

		protected float RestitutionCurve(float rel_vel, float restitution)
		{
			return restitution * restitution * (0f - rel_vel);
		}

		protected void ConvertContact(PersistentManifold manifold, ContactSolverInfo infoGlobal)
		{
			CollisionObject collisionObject = null;
			CollisionObject collisionObject2 = null;
			collisionObject = manifold.GetBody0() as CollisionObject;
			collisionObject2 = manifold.GetBody1() as CollisionObject;
			RigidBody rigidBody = collisionObject as RigidBody;
			RigidBody rigidBody2 = collisionObject2 as RigidBody;
			if ((rigidBody == null || rigidBody.GetInvMass() == 0f) && (rigidBody2 == null || rigidBody2.GetInvMass() == 0f))
			{
				return;
			}
			for (int i = 0; i < manifold.GetNumContacts(); i++)
			{
				ManifoldPoint contactPoint = manifold.GetContactPoint(i);
				if (!(contactPoint.GetDistance() <= manifold.GetContactProcessingThreshold()))
				{
					continue;
				}
				float relaxation = 1f;
				float rel_vel = 0f;
				IndexedVector3 vel = IndexedVector3.Zero;
				int count = m_tmpSolverContactConstraintPool.Count;
				SolverConstraint solverConstraint = m_tmpSolverContactConstraintPool[m_tmpSolverContactConstraintPool.Count];
				solverConstraint.Reset();
				RigidBody rigidBody3 = rigidBody;
				RigidBody rigidBody4 = rigidBody2;
				solverConstraint.m_solverBodyA = ((rigidBody3 != null) ? rigidBody3 : GetFixedBody());
				solverConstraint.m_solverBodyB = ((rigidBody4 != null) ? rigidBody4 : GetFixedBody());
				solverConstraint.m_originalContactPoint = contactPoint;
				IndexedVector3 rel_pos;
				IndexedVector3 rel_pos2;
				SetupContactConstraint(ref solverConstraint, collisionObject, collisionObject2, contactPoint, infoGlobal, ref vel, ref rel_vel, ref relaxation, out rel_pos, out rel_pos2);
				solverConstraint.m_frictionIndex = m_tmpSolverContactFrictionConstraintPool.Count;
				if (!TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_ENABLE_FRICTION_DIRECTION_CACHING) || !contactPoint.GetLateralFrictionInitialized())
				{
					contactPoint.m_lateralFrictionDir1 = vel - contactPoint.m_normalWorldOnB * rel_vel;
					float num = contactPoint.m_lateralFrictionDir1.LengthSquared();
					if (!TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_DISABLE_VELOCITY_DEPENDENT_FRICTION_DIRECTION) && num > 1.1920929E-07f)
					{
						contactPoint.m_lateralFrictionDir1 /= (float)Math.Sqrt(num);
						if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_2_FRICTION_DIRECTIONS))
						{
							contactPoint.m_lateralFrictionDir2 = IndexedVector3.Cross(contactPoint.m_lateralFrictionDir1, contactPoint.m_normalWorldOnB);
							contactPoint.m_lateralFrictionDir2.Normalize();
							ApplyAnisotropicFriction(collisionObject, ref contactPoint.m_lateralFrictionDir2);
							ApplyAnisotropicFriction(collisionObject2, ref contactPoint.m_lateralFrictionDir2);
							AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir2, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, 0f, 0f);
						}
						ApplyAnisotropicFriction(collisionObject, ref contactPoint.m_lateralFrictionDir1);
						ApplyAnisotropicFriction(collisionObject2, ref contactPoint.m_lateralFrictionDir1);
						AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir1, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, 0f, 0f);
						contactPoint.m_lateralFrictionInitialized = true;
					}
					else
					{
						IndexedVector3 n = contactPoint.m_normalWorldOnB;
						TransformUtil.PlaneSpace1(ref n, out contactPoint.m_lateralFrictionDir1, out contactPoint.m_lateralFrictionDir2);
						if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_2_FRICTION_DIRECTIONS))
						{
							ApplyAnisotropicFriction(collisionObject, ref contactPoint.m_lateralFrictionDir2);
							ApplyAnisotropicFriction(collisionObject2, ref contactPoint.m_lateralFrictionDir2);
							AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir2, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, 0f, 0f);
						}
						ApplyAnisotropicFriction(collisionObject, ref contactPoint.m_lateralFrictionDir1);
						ApplyAnisotropicFriction(collisionObject2, ref contactPoint.m_lateralFrictionDir1);
						AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir1, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, 0f, 0f);
						contactPoint.m_lateralFrictionInitialized = true;
					}
				}
				else
				{
					AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir1, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, contactPoint.m_contactMotion1, contactPoint.m_contactCFM1);
					if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_USE_2_FRICTION_DIRECTIONS))
					{
						AddFrictionConstraint(ref contactPoint.m_lateralFrictionDir2, rigidBody, rigidBody2, count, contactPoint, ref rel_pos, ref rel_pos2, collisionObject, collisionObject2, relaxation, contactPoint.m_contactMotion2, contactPoint.m_contactCFM2);
					}
				}
				SetFrictionConstraintImpulse(ref solverConstraint, rigidBody3, rigidBody4, contactPoint, infoGlobal);
			}
		}

		protected int GetOrInitSolverBody(CollisionObject body)
		{
			return 0;
		}

		protected void ResolveSingleConstraintRowGeneric(RigidBody body1, RigidBody body2, ref SolverConstraint c)
		{
			m_genericCount++;
			float num = c.m_rhs - c.m_appliedImpulse * c.m_cfm;
			float num2 = c.m_contactNormal.X * body1.m_deltaLinearVelocity.X + c.m_contactNormal.Y * body1.m_deltaLinearVelocity.Y + c.m_contactNormal.Z * body1.m_deltaLinearVelocity.Z + c.m_relpos1CrossNormal.X * body1.m_deltaAngularVelocity.X + c.m_relpos1CrossNormal.Y * body1.m_deltaAngularVelocity.Y + c.m_relpos1CrossNormal.Z * body1.m_deltaAngularVelocity.Z;
			float num3 = 0f - (c.m_contactNormal.X * body2.m_deltaLinearVelocity.X + c.m_contactNormal.Y * body2.m_deltaLinearVelocity.Y + c.m_contactNormal.Z * body2.m_deltaLinearVelocity.Z) + c.m_relpos2CrossNormal.X * body2.m_deltaAngularVelocity.X + c.m_relpos2CrossNormal.Y * body2.m_deltaAngularVelocity.Y + c.m_relpos2CrossNormal.Z * body2.m_deltaAngularVelocity.Z;
			num -= num2 * c.m_jacDiagABInv;
			num -= num3 * c.m_jacDiagABInv;
			float num4 = c.m_appliedImpulse + num;
			if (num4 < c.m_lowerLimit)
			{
				num = c.m_lowerLimit - c.m_appliedImpulse;
				c.m_appliedImpulse = c.m_lowerLimit;
			}
			else if (num4 > c.m_upperLimit)
			{
				num = c.m_upperLimit - c.m_appliedImpulse;
				c.m_appliedImpulse = c.m_upperLimit;
			}
			else
			{
				c.m_appliedImpulse = num4;
			}
			IndexedVector3 linearComponent = new IndexedVector3(c.m_contactNormal.X * body1.m_invMass.X, c.m_contactNormal.Y * body1.m_invMass.Y, c.m_contactNormal.Z * body1.m_invMass.Z);
			body1.InternalApplyImpulse(ref linearComponent, ref c.m_angularComponentA, num, "ResolveSingleConstraintGeneric-body1");
			linearComponent = new IndexedVector3((0f - c.m_contactNormal.X) * body2.m_invMass.X, (0f - c.m_contactNormal.Y) * body2.m_invMass.Y, (0f - c.m_contactNormal.Z) * body2.m_invMass.Z);
			body2.InternalApplyImpulse(ref linearComponent, ref c.m_angularComponentB, num, "ResolveSingleConstraintGeneric-body2");
		}

		protected void ResolveSingleConstraintRowLowerLimit(RigidBody body1, RigidBody body2, ref SolverConstraint c)
		{
			m_lowerLimitCount++;
			float num = c.m_rhs - c.m_appliedImpulse * c.m_cfm;
			float num5 = 200f;
			float num2 = c.m_contactNormal.X * body1.m_deltaLinearVelocity.X + c.m_contactNormal.Y * body1.m_deltaLinearVelocity.Y + c.m_contactNormal.Z * body1.m_deltaLinearVelocity.Z + c.m_relpos1CrossNormal.X * body1.m_deltaAngularVelocity.X + c.m_relpos1CrossNormal.Y * body1.m_deltaAngularVelocity.Y + c.m_relpos1CrossNormal.Z * body1.m_deltaAngularVelocity.Z;
			float num3 = 0f - (c.m_contactNormal.X * body2.m_deltaLinearVelocity.X + c.m_contactNormal.Y * body2.m_deltaLinearVelocity.Y + c.m_contactNormal.Z * body2.m_deltaLinearVelocity.Z) + c.m_relpos2CrossNormal.X * body2.m_deltaAngularVelocity.X + c.m_relpos2CrossNormal.Y * body2.m_deltaAngularVelocity.Y + c.m_relpos2CrossNormal.Z * body2.m_deltaAngularVelocity.Z;
			num -= num2 * c.m_jacDiagABInv;
			num -= num3 * c.m_jacDiagABInv;
			float num4 = c.m_appliedImpulse + num;
			if (num4 < c.m_lowerLimit)
			{
				num = c.m_lowerLimit - c.m_appliedImpulse;
				c.m_appliedImpulse = c.m_lowerLimit;
			}
			else
			{
				c.m_appliedImpulse = num4;
			}
			IndexedVector3 linearComponent = new IndexedVector3(c.m_contactNormal.X * body1.m_invMass.X, c.m_contactNormal.Y * body1.m_invMass.Y, c.m_contactNormal.Z * body1.m_invMass.Z);
			body1.InternalApplyImpulse(ref linearComponent, ref c.m_angularComponentA, num, "ResolveSingleConstraintRowLowerLimit-body1");
			linearComponent = new IndexedVector3((0f - c.m_contactNormal.X) * body2.m_invMass.X, (0f - c.m_contactNormal.Y) * body2.m_invMass.Y, (0f - c.m_contactNormal.Z) * body2.m_invMass.Z);
			body2.InternalApplyImpulse(ref linearComponent, ref c.m_angularComponentB, num, "ResolveSingleConstraintRowLowerLimit-body2");
		}

		protected void ResolveSplitPenetrationImpulseCacheFriendly(RigidBody body1, RigidBody body2, ref SolverConstraint c)
		{
			if (c.m_rhsPenetration != 0f)
			{
				gNumSplitImpulseRecoveries++;
				float num = c.m_rhsPenetration - c.m_appliedPushImpulse * c.m_cfm;
				float num2 = IndexedVector3.Dot(c.m_contactNormal, body1.InternalGetPushVelocity()) + IndexedVector3.Dot(c.m_relpos1CrossNormal, body1.InternalGetTurnVelocity());
				float num3 = 0f - IndexedVector3.Dot(c.m_contactNormal, body2.InternalGetPushVelocity()) + IndexedVector3.Dot(c.m_relpos2CrossNormal, body2.InternalGetTurnVelocity());
				num -= num2 * c.m_jacDiagABInv;
				num -= num3 * c.m_jacDiagABInv;
				float num4 = c.m_appliedPushImpulse + num;
				if (num4 < c.m_lowerLimit)
				{
					num = c.m_lowerLimit - c.m_appliedPushImpulse;
					c.m_appliedPushImpulse = c.m_lowerLimit;
				}
				else
				{
					c.m_appliedPushImpulse = num4;
				}
				body1.InternalApplyPushImpulse(c.m_contactNormal * body1.InternalGetInvMass(), c.m_angularComponentA, num);
				body2.InternalApplyPushImpulse(-c.m_contactNormal * body2.InternalGetInvMass(), c.m_angularComponentB, num);
			}
		}

		public virtual float SolveGroup(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifoldPtr, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer, IDispatcher dispatcher)
		{
			BulletGlobals.StartProfile("solveGroup");
			SolveGroupCacheFriendlySetup(bodies, numBodies, manifoldPtr, startManifold, numManifolds, constraints, startConstraint, numConstraints, infoGlobal, debugDrawer, dispatcher);
			SolveGroupCacheFriendlyIterations(bodies, numBodies, manifoldPtr, startManifold, numManifolds, constraints, startConstraint, numConstraints, infoGlobal, debugDrawer, dispatcher);
			SolveGroupCacheFriendlyFinish(bodies, numBodies, manifoldPtr, startManifold, numManifolds, constraints, startConstraint, numConstraints, infoGlobal, debugDrawer);
			BulletGlobals.StopProfile();
			return 0f;
		}

		public virtual void PrepareSolve(int numBodies, int numManifolds)
		{
		}

		public virtual void AllSolved(ContactSolverInfo info, IDebugDraw debugDrawer)
		{
		}

		public static RigidBody GetFixedBody()
		{
			if (s_fixed == null)
			{
				s_fixed = new RigidBody(0f, null, null, IndexedVector3.Zero);
				s_fixed.SetUserPointer("SICS:Fixed");
			}
			s_fixed.SetMassProps(0f, IndexedVector3.Zero);
			return s_fixed;
		}

		protected virtual void SolveGroupCacheFriendlySplitImpulseIterations(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifold, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer)
		{
			if (!infoGlobal.m_splitImpulse)
			{
				return;
			}
			for (int i = 0; i < infoGlobal.m_numIterations; i++)
			{
				int count = m_tmpSolverContactConstraintPool.Count;
				for (int j = 0; j < count; j++)
				{
					SolverConstraint c = m_tmpSolverContactConstraintPool[m_orderTmpConstraintPool[j]];
					ResolveSplitPenetrationImpulseCacheFriendly(c.m_solverBodyA, c.m_solverBodyB, ref c);
					m_tmpSolverContactConstraintPool[m_orderTmpConstraintPool[j]] = c;
				}
			}
		}

		protected virtual float SolveGroupCacheFriendlyFinish(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifold, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer)
		{
			m_finishCount++;
			int count = m_tmpSolverContactConstraintPool.Count;
			for (int i = 0; i < count; i++)
			{
				SolverConstraint solverConstraint = m_tmpSolverContactConstraintPool[i];
				((ManifoldPoint)solverConstraint.m_originalContactPoint).SetAppliedImpulse(solverConstraint.m_appliedImpulse);
				if ((infoGlobal.m_solverMode & SolverMode.SOLVER_USE_FRICTION_WARMSTARTING) != 0)
				{
					((ManifoldPoint)solverConstraint.m_originalContactPoint).SetAppliedImpulseLateral1(m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex].m_appliedImpulse);
					((ManifoldPoint)solverConstraint.m_originalContactPoint).SetAppliedImpulseLateral2(m_tmpSolverContactFrictionConstraintPool[solverConstraint.m_frictionIndex + 1].m_appliedImpulse);
				}
			}
			count = m_tmpSolverNonContactConstraintPool.Count;
			for (int j = 0; j < count; j++)
			{
				SolverConstraint solverConstraint2 = m_tmpSolverNonContactConstraintPool[j];
				TypedConstraint typedConstraint = solverConstraint2.m_originalContactPoint as TypedConstraint;
				typedConstraint.InternalSetAppliedImpulse(solverConstraint2.m_appliedImpulse);
				if (Math.Abs(solverConstraint2.m_appliedImpulse) >= typedConstraint.GetBreakingImpulseThreshold())
				{
					typedConstraint.SetEnabled(false);
				}
				m_tmpSolverNonContactConstraintPool[j] = solverConstraint2;
			}
			if (infoGlobal.m_splitImpulse)
			{
				for (int k = 0; k < numBodies; k++)
				{
					RigidBody rigidBody = RigidBody.Upcast(bodies[k]);
					if (rigidBody != null)
					{
						rigidBody.InternalWritebackVelocity(infoGlobal.m_timeStep);
					}
				}
			}
			else
			{
				for (int l = 0; l < numBodies; l++)
				{
					RigidBody rigidBody2 = RigidBody.Upcast(bodies[l]);
					if (rigidBody2 != null)
					{
						rigidBody2.InternalWritebackVelocity();
					}
				}
			}
			m_tmpSolverContactConstraintPool.Resize(0);
			m_tmpSolverNonContactConstraintPool.Resize(0);
			m_tmpSolverContactFrictionConstraintPool.Resize(0);
			m_tmpConstraintInfo2Pool.Resize(0);
			return 0f;
		}

		protected virtual float SolveGroupCacheFriendlySetup(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifold, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer, IDispatcher dispatcher)
		{
			m_setupCount++;
			BulletGlobals.StartProfile("solveGroupCacheFriendlySetup");
			m_maxOverrideNumSolverIterations = 0;
			m_counter++;
			if (numConstraints + numManifolds == 0)
			{
				BulletGlobals.StopProfile();
				return 0f;
			}
			IndexedVector3 v = IndexedVector3.Zero;
			if (infoGlobal.m_splitImpulse)
			{
				for (int i = 0; i < numBodies; i++)
				{
					RigidBody rigidBody = bodies[i] as RigidBody;
					if (rigidBody != null)
					{
						rigidBody.InternalSetDeltaLinearVelocity(ref v);
						rigidBody.InternalSetDeltaAngularVelocity(ref v);
						rigidBody.InternalSetPushVelocity(ref v);
						rigidBody.InternalSetTurnVelocity(ref v);
					}
				}
			}
			else
			{
				for (int j = 0; j < numBodies; j++)
				{
					RigidBody rigidBody2 = bodies[j] as RigidBody;
					if (rigidBody2 != null)
					{
						rigidBody2.InternalSetDeltaLinearVelocity(ref v);
						rigidBody2.InternalSetDeltaAngularVelocity(ref v);
					}
				}
			}
			int num = startConstraint + numConstraints;
			for (int k = startConstraint; k < num; k++)
			{
				TypedConstraint typedConstraint = constraints[k];
				typedConstraint.InternalSetAppliedImpulse(0f);
			}
			int num2 = 0;
			m_tmpConstraintSizesPool.Resize(numConstraints);
			for (int l = 0; l < numConstraints; l++)
			{
				ConstraintInfo1 constraintInfo = m_tmpConstraintSizesPool[l];
				if (constraints[startConstraint + l].IsEnabled())
				{
					constraints[startConstraint + l].GetInfo1(constraintInfo);
				}
				else
				{
					constraintInfo.m_numConstraintRows = 0;
					constraintInfo.nub = 0;
				}
				num2 += constraintInfo.m_numConstraintRows;
			}
			m_tmpSolverNonContactConstraintPool.Resize(num2);
			int num3 = 0;
			for (int m = 0; m < numConstraints; m++)
			{
				ConstraintInfo1 constraintInfo2 = m_tmpConstraintSizesPool[m];
				if (constraintInfo2.m_numConstraintRows != 0)
				{
					TypedConstraint typedConstraint2 = constraints[startConstraint + m];
					RigidBody rigidBodyA = typedConstraint2.GetRigidBodyA();
					RigidBody rigidBodyB = typedConstraint2.GetRigidBodyB();
					int num4 = ((typedConstraint2.GetOverrideNumSolverIterations() > 0) ? typedConstraint2.GetOverrideNumSolverIterations() : infoGlobal.m_numIterations);
					if (num4 > m_maxOverrideNumSolverIterations)
					{
						m_maxOverrideNumSolverIterations = num4;
					}
					for (int n = 0; n < constraintInfo2.m_numConstraintRows; n++)
					{
						int index = num3 + n;
						SolverConstraint solverConstraint = m_tmpSolverNonContactConstraintPool[index];
						solverConstraint.Reset();
						solverConstraint.m_lowerLimit = float.MinValue;
						solverConstraint.m_upperLimit = float.MaxValue;
						solverConstraint.m_appliedImpulse = 0f;
						solverConstraint.m_appliedPushImpulse = 0f;
						solverConstraint.m_solverBodyA = rigidBodyA;
						solverConstraint.m_solverBodyB = rigidBodyB;
						solverConstraint.m_overrideNumSolverIterations = num4;
					}
					rigidBodyA.InternalSetDeltaLinearVelocity(ref v);
					rigidBodyA.InternalSetDeltaAngularVelocity(ref v);
					rigidBodyB.InternalSetDeltaLinearVelocity(ref v);
					rigidBodyB.InternalSetDeltaAngularVelocity(ref v);
					ConstraintInfo2 constraintInfo3 = m_tmpConstraintInfo2Pool[m_tmpConstraintInfo2Pool.Count];
					constraintInfo3.m_numRows = constraintInfo2.m_numConstraintRows;
					for (int num5 = 0; num5 < constraintInfo2.m_numConstraintRows; num5++)
					{
						constraintInfo3.m_solverConstraints[num5] = m_tmpSolverNonContactConstraintPool[num3 + num5];
					}
					constraintInfo3.fps = 1f / infoGlobal.m_timeStep;
					constraintInfo3.erp = infoGlobal.m_erp;
					constraintInfo3.m_numIterations = infoGlobal.m_numIterations;
					constraintInfo3.m_damping = infoGlobal.m_damping;
					typedConstraint2.GetInfo2(constraintInfo3);
					for (int num6 = 0; num6 < constraintInfo2.m_numConstraintRows; num6++)
					{
						SolverConstraint solverConstraint2 = m_tmpSolverNonContactConstraintPool[num3 + num6];
						if (solverConstraint2.m_upperLimit >= typedConstraint2.GetBreakingImpulseThreshold())
						{
							solverConstraint2.m_upperLimit = typedConstraint2.GetBreakingImpulseThreshold();
						}
						if (solverConstraint2.m_lowerLimit <= 0f - typedConstraint2.GetBreakingImpulseThreshold())
						{
							solverConstraint2.m_lowerLimit = 0f - typedConstraint2.GetBreakingImpulseThreshold();
						}
						solverConstraint2.m_originalContactPoint = typedConstraint2;
						IndexedVector3 relpos1CrossNormal = solverConstraint2.m_relpos1CrossNormal;
						solverConstraint2.m_angularComponentA = typedConstraint2.GetRigidBodyA().GetInvInertiaTensorWorld() * relpos1CrossNormal * typedConstraint2.GetRigidBodyA().GetAngularFactor();
						IndexedVector3 relpos2CrossNormal = solverConstraint2.m_relpos2CrossNormal;
						solverConstraint2.m_angularComponentB = typedConstraint2.GetRigidBodyB().GetInvInertiaTensorWorld() * relpos2CrossNormal * typedConstraint2.GetRigidBodyB().GetAngularFactor();
						IndexedVector3 a = solverConstraint2.m_contactNormal * rigidBodyA.GetInvMass();
						IndexedVector3 a2 = rigidBodyA.GetInvInertiaTensorWorld() * solverConstraint2.m_relpos1CrossNormal;
						IndexedVector3 a3 = solverConstraint2.m_contactNormal * rigidBodyB.GetInvMass();
						IndexedVector3 a4 = rigidBodyB.GetInvInertiaTensorWorld() * solverConstraint2.m_relpos2CrossNormal;
						float num7 = IndexedVector3.Dot(ref a, ref solverConstraint2.m_contactNormal);
						float num8 = IndexedVector3.Dot(ref a2, ref solverConstraint2.m_relpos1CrossNormal);
						float num9 = IndexedVector3.Dot(ref a3, ref solverConstraint2.m_contactNormal);
						float num10 = IndexedVector3.Dot(ref a4, ref solverConstraint2.m_relpos2CrossNormal);
						num7 += num8;
						num7 += num9;
						num7 += num10;
						solverConstraint2.m_jacDiagABInv = 1f / num7;
						float num11 = IndexedVector3.Dot(solverConstraint2.m_contactNormal, rigidBodyA.GetLinearVelocity()) + IndexedVector3.Dot(solverConstraint2.m_relpos1CrossNormal, rigidBodyA.GetAngularVelocity());
						float num12 = 0f - IndexedVector3.Dot(solverConstraint2.m_contactNormal, rigidBodyB.GetLinearVelocity()) + IndexedVector3.Dot(solverConstraint2.m_relpos2CrossNormal, rigidBodyB.GetAngularVelocity());
						float num13 = num11 + num12;
						float num14 = 0f;
						float rhs = solverConstraint2.m_rhs;
						float num15 = num14 - num13 * constraintInfo3.m_damping;
						float num16 = rhs * solverConstraint2.m_jacDiagABInv;
						float num17 = num15 * solverConstraint2.m_jacDiagABInv;
						solverConstraint2.m_rhs = num16 + num17;
						solverConstraint2.m_appliedImpulse = 0f;
					}
				}
				num3 += m_tmpConstraintSizesPool[m].m_numConstraintRows;
			}
			int num18 = startManifold + numManifolds;
			for (int num19 = startManifold; num19 < num18; num19++)
			{
				ConvertContact(manifold[num19], infoGlobal);
			}
			int count = m_tmpSolverNonContactConstraintPool.Count;
			int count2 = m_tmpSolverContactConstraintPool.Count;
			int count3 = m_tmpSolverContactFrictionConstraintPool.Count;
			m_orderNonContactConstraintPool.EnsureCapacity(count);
			m_orderTmpConstraintPool.EnsureCapacity(count2);
			m_orderFrictionConstraintPool.EnsureCapacity(count3);
			for (int num20 = 0; num20 < count; num20++)
			{
				m_orderNonContactConstraintPool[num20] = num20;
			}
			for (int num21 = 0; num21 < count2; num21++)
			{
				m_orderTmpConstraintPool[num21] = num21;
			}
			for (int num22 = 0; num22 < count3; num22++)
			{
				m_orderFrictionConstraintPool[num22] = num22;
			}
			BulletGlobals.StopProfile();
			return 0f;
		}

		protected float SolveGroupCacheFriendlyIterations(ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifoldPtr, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer, IDispatcher dispatcher)
		{
			m_iterCount++;
			BulletGlobals.StartProfile("solveGroupCacheFriendlyIterations");
			SolveGroupCacheFriendlySplitImpulseIterations(bodies, numBodies, manifoldPtr, startManifold, numManifolds, constraints, startConstraint, numConstraints, infoGlobal, debugDrawer);
			int num = ((m_maxOverrideNumSolverIterations > infoGlobal.m_numIterations) ? m_maxOverrideNumSolverIterations : infoGlobal.m_numIterations);
			for (int i = 0; i < num; i++)
			{
				SolveSingleIteration(i, bodies, numBodies, manifoldPtr, startManifold, numManifolds, constraints, startConstraint, numConstraints, infoGlobal, debugDrawer);
			}
			BulletGlobals.StopProfile();
			return 0f;
		}

		protected float SolveSingleIteration(int iteration, ObjectArray<CollisionObject> bodies, int numBodies, PersistentManifoldArray manifold, int startManifold, int numManifolds, ObjectArray<TypedConstraint> constraints, int startConstraint, int numConstraints, ContactSolverInfo infoGlobal, IDebugDraw debugDrawer)
		{
			int count = m_tmpSolverNonContactConstraintPool.Count;
			int count2 = m_tmpSolverContactConstraintPool.Count;
			int count3 = m_tmpSolverContactFrictionConstraintPool.Count;
			if (TestSolverMode(infoGlobal.m_solverMode, SolverMode.SOLVER_RANDMIZE_ORDER) && (iteration & 7) == 0)
			{
				for (int i = 0; i < count; i++)
				{
					int value = m_orderNonContactConstraintPool[i];
					int index = RandInt2(i + 1);
					m_orderNonContactConstraintPool[i] = m_orderNonContactConstraintPool[index];
					m_orderNonContactConstraintPool[index] = value;
				}
				if (iteration < infoGlobal.m_numIterations)
				{
					for (int j = 0; j < count2; j++)
					{
						int value2 = m_orderTmpConstraintPool[j];
						int index2 = RandInt2(j + 1);
						m_orderTmpConstraintPool[j] = m_orderTmpConstraintPool[index2];
						m_orderTmpConstraintPool[index2] = value2;
					}
					for (int k = 0; k < count3; k++)
					{
						int value3 = m_orderFrictionConstraintPool[k];
						int index3 = RandInt2(k + 1);
						m_orderFrictionConstraintPool[k] = m_orderFrictionConstraintPool[index3];
						m_orderFrictionConstraintPool[index3] = value3;
					}
				}
			}
			SolverConstraint[] rawArray = m_tmpSolverNonContactConstraintPool.GetRawArray();
			int[] rawArray2 = m_orderNonContactConstraintPool.GetRawArray();
			for (int l = 0; l < m_tmpSolverNonContactConstraintPool.Count; l++)
			{
				SolverConstraint c = rawArray[rawArray2[l]];
				if (iteration < c.m_overrideNumSolverIterations)
				{
					ResolveSingleConstraintRowGeneric(c.m_solverBodyA, c.m_solverBodyB, ref c);
				}
			}
			if (iteration < infoGlobal.m_numIterations)
			{
				SolverConstraint[] rawArray3 = m_tmpSolverContactConstraintPool.GetRawArray();
				int[] rawArray4 = m_orderTmpConstraintPool.GetRawArray();
				int count4 = m_tmpSolverContactConstraintPool.Count;
				for (int m = 0; m < count4; m++)
				{
					SolverConstraint c2 = rawArray3[rawArray4[m]];
					ResolveSingleConstraintRowLowerLimit(c2.m_solverBodyA, c2.m_solverBodyB, ref c2);
				}
				int count5 = m_tmpSolverContactFrictionConstraintPool.Count;
				SolverConstraint[] rawArray5 = m_tmpSolverContactFrictionConstraintPool.GetRawArray();
				int[] rawArray6 = m_orderFrictionConstraintPool.GetRawArray();
				for (int n = 0; n < count5; n++)
				{
					SolverConstraint c3 = rawArray5[rawArray6[n]];
					float appliedImpulse = m_tmpSolverContactConstraintPool[c3.m_frictionIndex].m_appliedImpulse;
					if (appliedImpulse > 0f)
					{
						c3.m_lowerLimit = 0f - c3.m_friction * appliedImpulse;
						c3.m_upperLimit = c3.m_friction * appliedImpulse;
						ResolveSingleConstraintRowGeneric(c3.m_solverBodyA, c3.m_solverBodyB, ref c3);
					}
				}
			}
			return 0f;
		}

		public virtual void Reset()
		{
			m_btSeed2 = 0uL;
		}

		public ulong Rand2()
		{
			m_btSeed2 = (1664525 * m_btSeed2 + 1013904223) & 0xFFFFFFFFu;
			return m_btSeed2;
		}

		public int RandInt2(int n)
		{
			ulong num = (ulong)n;
			ulong num2 = Rand2();
			if (num <= 65536)
			{
				num2 ^= num2 >> 16;
				if (num <= 256)
				{
					num2 ^= num2 >> 8;
					if (num <= 16)
					{
						num2 ^= num2 >> 4;
						if (num <= 4)
						{
							num2 ^= num2 >> 2;
							if (num <= 2)
							{
								num2 ^= num2 >> 1;
							}
						}
					}
				}
			}
			return (int)(num2 % num);
		}

		public void SetRandSeed(ulong seed)
		{
			m_btSeed2 = seed;
		}

		public ulong GetRandSeed()
		{
			return m_btSeed2;
		}

		private static void ApplyAnisotropicFriction(CollisionObject colObj, ref IndexedVector3 frictionDirection)
		{
			if (colObj != null && colObj.HasAnisotropicFriction())
			{
				IndexedVector3 indexedVector = frictionDirection * colObj.GetWorldTransform()._basis;
				IndexedVector3 anisotropicFriction = colObj.GetAnisotropicFriction();
				indexedVector *= anisotropicFriction;
				frictionDirection = colObj.GetWorldTransform()._basis * indexedVector;
			}
		}

		public static bool TestSolverMode(SolverMode val1, SolverMode val2)
		{
			return (val1 & val2) == val2;
		}
	}
}
