using System;
using System.Collections.Generic;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class RigidBody : CollisionObject
	{
		private const float MAX_ANGVEL = (float)Math.PI / 2f;

		public static int uniqueId;

		private IndexedBasisMatrix m_invInertiaTensorWorld = IndexedBasisMatrix.Identity;

		private IndexedVector3 m_linearVelocity;

		private IndexedVector3 m_angularVelocity;

		private float m_inverseMass;

		private IndexedVector3 m_linearFactor;

		private IndexedVector3 m_gravity;

		private IndexedVector3 m_gravity_acceleration;

		private IndexedVector3 m_invInertiaLocal;

		private IndexedVector3 m_totalForce;

		private IndexedVector3 m_totalTorque;

		private float m_linearDamping;

		private float m_angularDamping;

		private bool m_additionalDamping;

		private float m_additionalDampingFactor;

		private float m_additionalLinearDampingThresholdSqr;

		private float m_additionalAngularDampingThresholdSqr;

		private float m_additionalAngularDampingFactor;

		private float m_linearSleepingThreshold;

		private float m_angularSleepingThreshold;

		private IMotionState m_optionalMotionState;

		private IList<TypedConstraint> m_constraintRefs;

		private RigidBodyFlags m_rigidbodyFlags;

		public int m_debugBodyId;

		public IndexedVector3 m_deltaLinearVelocity;

		public IndexedVector3 m_deltaAngularVelocity;

		protected IndexedVector3 m_angularFactor;

		public IndexedVector3 m_invMass;

		protected IndexedVector3 m_pushVelocity;

		protected IndexedVector3 m_turnVelocity;

		private int m_contactSolverType;

		private int m_frictionSolverType;

		public RigidBody()
		{
		}

		public RigidBody(RigidBodyConstructionInfo constructionInfo)
		{
			SetupRigidBody(constructionInfo);
		}

		public RigidBody(float mass, IMotionState motionState, CollisionShape collisionShape, IndexedVector3 localInertia)
		{
			RigidBodyConstructionInfo constructionInfo = new RigidBodyConstructionInfo(mass, motionState, collisionShape, localInertia);
			SetupRigidBody(constructionInfo);
		}

		public override void Cleanup()
		{
			base.Cleanup();
		}

		protected void SetupRigidBody(RigidBodyConstructionInfo constructionInfo)
		{
			m_internalType = CollisionObjectTypes.CO_RIGID_BODY;
			m_linearVelocity = IndexedVector3.Zero;
			m_angularVelocity = IndexedVector3.Zero;
			m_angularFactor = IndexedVector3.One;
			m_linearFactor = IndexedVector3.One;
			m_gravity = IndexedVector3.Zero;
			m_gravity_acceleration = IndexedVector3.Zero;
			m_totalForce = IndexedVector3.Zero;
			m_totalTorque = IndexedVector3.Zero;
			SetDamping(constructionInfo.m_linearDamping, constructionInfo.m_angularDamping);
			m_linearSleepingThreshold = constructionInfo.m_linearSleepingThreshold;
			m_angularSleepingThreshold = constructionInfo.m_angularSleepingThreshold;
			m_optionalMotionState = constructionInfo.m_motionState;
			m_contactSolverType = 0;
			m_frictionSolverType = 0;
			m_additionalDamping = constructionInfo.m_additionalDamping;
			m_additionalDampingFactor = constructionInfo.m_additionalDampingFactor;
			m_additionalLinearDampingThresholdSqr = constructionInfo.m_additionalLinearDampingThresholdSqr;
			m_additionalAngularDampingThresholdSqr = constructionInfo.m_additionalAngularDampingThresholdSqr;
			m_additionalAngularDampingFactor = constructionInfo.m_additionalAngularDampingFactor;
			if (m_optionalMotionState != null)
			{
				m_optionalMotionState.GetWorldTransform(out m_worldTransform);
			}
			else
			{
				SetWorldTransform(ref constructionInfo.m_startWorldTransform);
			}
			m_interpolationWorldTransform = m_worldTransform;
			m_interpolationLinearVelocity = IndexedVector3.Zero;
			m_interpolationAngularVelocity = IndexedVector3.Zero;
			m_friction = constructionInfo.m_friction;
			m_restitution = constructionInfo.m_restitution;
			SetCollisionShape(constructionInfo.m_collisionShape);
			m_debugBodyId = uniqueId++;
			SetMassProps(constructionInfo.m_mass, constructionInfo.m_localInertia);
			UpdateInertiaTensor();
			m_rigidbodyFlags = RigidBodyFlags.BT_NONE;
			m_constraintRefs = new List<TypedConstraint>();
			m_deltaLinearVelocity = IndexedVector3.Zero;
			m_deltaAngularVelocity = IndexedVector3.Zero;
			m_invMass = m_inverseMass * m_linearFactor;
			m_pushVelocity = IndexedVector3.Zero;
			m_turnVelocity = IndexedVector3.Zero;
		}

		public void ProceedToTransform(ref IndexedMatrix newTrans)
		{
			SetCenterOfMassTransform(ref newTrans);
		}

		public static RigidBody Upcast(CollisionObject colObj)
		{
			if ((colObj.GetInternalType() & CollisionObjectTypes.CO_RIGID_BODY) != 0)
			{
				return colObj as RigidBody;
			}
			return null;
		}

		public void PredictIntegratedTransform(float timeStep, out IndexedMatrix predictedTransform)
		{
			TransformUtil.IntegrateTransform(ref m_worldTransform, ref m_linearVelocity, ref m_angularVelocity, timeStep, out predictedTransform);
		}

		public void SaveKinematicState(float timeStep)
		{
			if (timeStep != 0f)
			{
				if (GetMotionState() != null)
				{
					GetMotionState().GetWorldTransform(out m_worldTransform);
				}
				IndexedVector3 zero = IndexedVector3.Zero;
				IndexedVector3 zero2 = IndexedVector3.Zero;
				IndexedMatrix transform = m_worldTransform;
				TransformUtil.CalculateVelocity(ref m_interpolationWorldTransform, ref transform, timeStep, out m_linearVelocity, out m_angularVelocity);
				SetWorldTransform(ref transform);
				m_interpolationLinearVelocity = m_linearVelocity;
				m_interpolationAngularVelocity = m_angularVelocity;
				SetInterpolationWorldTransform(ref m_worldTransform);
			}
		}

		public void ApplyGravity()
		{
			if (!IsStaticOrKinematicObject())
			{
				ApplyCentralForce(ref m_gravity);
			}
		}

		public void SetGravity(IndexedVector3 acceleration)
		{
			SetGravity(ref acceleration);
		}

		public void SetGravity(ref IndexedVector3 acceleration)
		{
			if (m_inverseMass != 0f)
			{
				m_gravity = acceleration * (1f / m_inverseMass);
			}
			m_gravity_acceleration = acceleration;
		}

		public IndexedVector3 GetGravity()
		{
			return m_gravity_acceleration;
		}

		public void SetDamping(float lin_damping, float ang_damping)
		{
			m_linearDamping = MathUtil.Clamp(lin_damping, 0f, 1f);
			m_angularDamping = MathUtil.Clamp(ang_damping, 0f, 1f);
		}

		public float GetLinearDamping()
		{
			return m_linearDamping;
		}

		public float GetAngularDamping()
		{
			return m_angularDamping;
		}

		public float GetLinearSleepingThreshold()
		{
			return m_linearSleepingThreshold;
		}

		public float GetAngularSleepingThreshold()
		{
			return m_angularSleepingThreshold;
		}

		public void ApplyDamping(float timeStep)
		{
			m_linearVelocity *= (float)Math.Pow(1f - m_linearDamping, timeStep);
			m_angularVelocity *= (float)Math.Pow(1f - m_angularDamping, timeStep);
			if (!m_additionalDamping)
			{
				return;
			}
			if (m_angularVelocity.LengthSquared() < m_additionalAngularDampingThresholdSqr && m_linearVelocity.LengthSquared() < m_additionalLinearDampingThresholdSqr)
			{
				m_angularVelocity *= m_additionalDampingFactor;
				m_linearVelocity *= m_additionalDampingFactor;
			}
			float num = m_linearVelocity.Length();
			if (num < m_linearDamping)
			{
				float num2 = 0.005f;
				if (num > num2)
				{
					IndexedVector3 linearVelocity = m_linearVelocity;
					linearVelocity.Normalize();
					m_linearVelocity -= linearVelocity * num2;
				}
				else
				{
					m_linearVelocity = IndexedVector3.Zero;
				}
			}
			float num3 = m_angularVelocity.Length();
			if (num3 < m_angularDamping)
			{
				float num4 = 0.005f;
				if (num3 > num4)
				{
					IndexedVector3 angularVelocity = m_angularVelocity;
					angularVelocity.Normalize();
					m_angularVelocity -= angularVelocity * num4;
				}
				else
				{
					m_angularVelocity = IndexedVector3.Zero;
				}
			}
		}

		public void SetMassProps(float mass, IndexedVector3 inertia)
		{
			SetMassProps(mass, ref inertia);
		}

		public void SetMassProps(float mass, ref IndexedVector3 inertia)
		{
			if (MathUtil.FuzzyZero(mass))
			{
				m_collisionFlags |= CollisionFlags.CF_STATIC_OBJECT;
				m_inverseMass = 0f;
			}
			else
			{
				m_collisionFlags &= ~CollisionFlags.CF_STATIC_OBJECT;
				m_inverseMass = 1f / mass;
			}
			m_gravity = mass * m_gravity_acceleration;
			m_invInertiaLocal = new IndexedVector3((inertia.X != 0f) ? (1f / inertia.X) : 0f, (inertia.Y != 0f) ? (1f / inertia.Y) : 0f, (inertia.Z != 0f) ? (1f / inertia.Z) : 0f);
			m_invMass = m_linearFactor * m_inverseMass;
		}

		public IndexedVector3 GetLinearFactor()
		{
			return m_linearFactor;
		}

		public void SetLinearFactor(IndexedVector3 linearFactor)
		{
			m_linearFactor = linearFactor;
			m_invMass = m_linearFactor * m_inverseMass;
		}

		public float GetInvMass()
		{
			return m_inverseMass;
		}

		public IndexedBasisMatrix GetInvInertiaTensorWorld()
		{
			return m_invInertiaTensorWorld;
		}

		public void IntegrateVelocities(float step)
		{
			if (!IsStaticOrKinematicObject())
			{
				m_linearVelocity += m_totalForce * (m_inverseMass * step);
				m_angularVelocity += m_invInertiaTensorWorld * m_totalTorque * step;
				float num = m_angularVelocity.Length();
				if (num * step > (float)Math.PI / 2f)
				{
					m_angularVelocity *= (float)Math.PI / 2f / step / num;
				}
			}
		}

		public void SetCenterOfMassTransform(ref IndexedMatrix xform)
		{
			if (IsKinematicObject())
			{
				SetInterpolationWorldTransform(ref m_worldTransform);
			}
			else
			{
				SetInterpolationWorldTransform(ref xform);
			}
			m_interpolationLinearVelocity = GetLinearVelocity();
			m_interpolationAngularVelocity = GetAngularVelocity();
			SetWorldTransform(ref xform);
			UpdateInertiaTensor();
		}

		public void ApplyCentralForce(ref IndexedVector3 force)
		{
			m_totalForce += force * m_linearFactor;
		}

		public IndexedVector3 GetTotalForce()
		{
			return m_totalForce;
		}

		public IndexedVector3 GetTotalTorque()
		{
			return m_totalTorque;
		}

		public IndexedVector3 GetInvInertiaDiagLocal()
		{
			return m_invInertiaLocal;
		}

		public void SetInvInertiaDiagLocal(ref IndexedVector3 diagInvInertia)
		{
			m_invInertiaLocal = diagInvInertia;
		}

		public void SetSleepingThresholds(float linear, float angular)
		{
			m_linearSleepingThreshold = linear;
			m_angularSleepingThreshold = angular;
		}

		public void ApplyTorque(IndexedVector3 torque)
		{
			ApplyTorque(ref torque);
		}

		public void ApplyTorque(ref IndexedVector3 torque)
		{
			m_totalTorque += torque * m_angularFactor;
		}

		public void ApplyForce(ref IndexedVector3 force, ref IndexedVector3 rel_pos)
		{
			ApplyCentralForce(ref force);
			IndexedVector3 indexedVector = IndexedVector3.Cross(rel_pos, force);
			indexedVector *= m_angularFactor;
			ApplyTorque(IndexedVector3.Cross(rel_pos, force * m_linearFactor));
		}

		public void ApplyCentralImpulse(ref IndexedVector3 impulse)
		{
			m_linearVelocity += impulse * m_linearFactor * m_inverseMass;
		}

		public void ApplyTorqueImpulse(IndexedVector3 torque)
		{
			ApplyTorqueImpulse(ref torque);
		}

		public void ApplyTorqueImpulse(ref IndexedVector3 torque)
		{
			m_angularVelocity += m_invInertiaTensorWorld * torque * m_angularFactor;
		}

		public void ApplyImpulse(IndexedVector3 impulse, IndexedVector3 rel_pos)
		{
			ApplyImpulse(ref impulse, ref rel_pos);
		}

		public void ApplyImpulse(ref IndexedVector3 impulse, ref IndexedVector3 rel_pos)
		{
			if (m_inverseMass != 0f)
			{
				ApplyCentralImpulse(ref impulse);
				if (m_angularFactor.LengthSquared() > 0f)
				{
					ApplyTorqueImpulse(IndexedVector3.Cross(rel_pos, impulse * m_linearFactor));
				}
			}
		}

		public void InternalApplyImpulse(IndexedVector3 linearComponent, IndexedVector3 angularComponent, float impulseMagnitude, string caller)
		{
			float num = 20f;
			InternalApplyImpulse(ref linearComponent, ref angularComponent, impulseMagnitude, caller);
		}

		public void ClearForces()
		{
			m_totalForce = IndexedVector3.Zero;
			m_totalTorque = IndexedVector3.Zero;
		}

		public void UpdateInertiaTensor()
		{
			m_invInertiaTensorWorld = m_worldTransform._basis.Scaled(ref m_invInertiaLocal) * m_worldTransform._basis.Transpose();
		}

		public IndexedVector3 GetCenterOfMassPosition()
		{
			return m_worldTransform._origin;
		}

		public IndexedQuaternion GetOrientation()
		{
			return m_worldTransform._basis.GetRotation();
		}

		public IndexedMatrix GetCenterOfMassTransform()
		{
			return m_worldTransform;
		}

		public IndexedVector3 GetLinearVelocity()
		{
			return m_linearVelocity;
		}

		public IndexedVector3 GetAngularVelocity()
		{
			return m_angularVelocity;
		}

		public void SetLinearVelocity(IndexedVector3 lin_vel)
		{
			SetLinearVelocity(ref lin_vel);
		}

		public void SetLinearVelocity(ref IndexedVector3 lin_vel)
		{
			m_linearVelocity = lin_vel;
		}

		public void SetAngularVelocity(IndexedVector3 ang_vel)
		{
			SetAngularVelocity(ref ang_vel);
		}

		public void SetAngularVelocity(ref IndexedVector3 ang_vel)
		{
			m_angularVelocity = ang_vel;
		}

		public IndexedVector3 GetVelocityInLocalPoint(ref IndexedVector3 rel_pos)
		{
			IndexedVector3 indexedVector = new IndexedVector3(m_angularVelocity.Y * rel_pos.Z - m_angularVelocity.Z * rel_pos.Y, m_angularVelocity.Z * rel_pos.X - m_angularVelocity.X * rel_pos.Z, m_angularVelocity.X * rel_pos.Y - m_angularVelocity.Y * rel_pos.X);
			return new IndexedVector3(m_linearVelocity.X + indexedVector.X, m_linearVelocity.Y + indexedVector.Y, m_linearVelocity.Z + indexedVector.Z);
		}

		public new void Translate(ref IndexedVector3 v)
		{
			m_worldTransform._origin += v;
		}

		public void GetAabb(out IndexedVector3 aabbMin, out IndexedVector3 aabbMax)
		{
			GetCollisionShape().GetAabb(m_worldTransform, out aabbMin, out aabbMax);
		}

		public float ComputeImpulseDenominator(ref IndexedVector3 pos, ref IndexedVector3 normal)
		{
			IndexedVector3 v = pos - GetCenterOfMassPosition();
			IndexedVector3 indexedVector = v.Cross(ref normal);
			IndexedVector3 b = (indexedVector * GetInvInertiaTensorWorld()).Cross(ref v);
			return m_inverseMass + IndexedVector3.Dot(normal, b);
		}

		public float ComputeAngularImpulseDenominator(ref IndexedVector3 axis)
		{
			IndexedVector3 v = axis * GetInvInertiaTensorWorld();
			return axis.Dot(ref v);
		}

		public void UpdateDeactivation(float timeStep)
		{
			if (GetActivationState() != ActivationState.ISLAND_SLEEPING && GetActivationState() != ActivationState.DISABLE_DEACTIVATION)
			{
				if (GetLinearVelocity().LengthSquared() < m_linearSleepingThreshold * m_linearSleepingThreshold && GetAngularVelocity().LengthSquared() < m_angularSleepingThreshold * m_angularSleepingThreshold)
				{
					m_deactivationTime += timeStep;
					return;
				}
				m_deactivationTime = 0f;
				SetActivationState(ActivationState.UNDEFINED);
			}
		}

		public bool WantsSleeping()
		{
			if (GetActivationState() == ActivationState.DISABLE_DEACTIVATION)
			{
				return false;
			}
			if (BulletGlobals.gDisableDeactivation || BulletGlobals.gDeactivationTime == 0f)
			{
				return false;
			}
			if (GetActivationState() == ActivationState.ISLAND_SLEEPING || GetActivationState() == ActivationState.WANTS_DEACTIVATION)
			{
				return true;
			}
			if (m_deactivationTime > BulletGlobals.gDeactivationTime)
			{
				return true;
			}
			return false;
		}

		public BroadphaseProxy GetBroadphaseProxy()
		{
			return m_broadphaseHandle;
		}

		public void SetNewBroadphaseProxy(BroadphaseProxy broadphaseProxy)
		{
			m_broadphaseHandle = broadphaseProxy;
		}

		public IMotionState GetMotionState()
		{
			return m_optionalMotionState;
		}

		public void SetMotionState(IMotionState motionState)
		{
			m_optionalMotionState = motionState;
			if (m_optionalMotionState != null)
			{
				motionState.GetWorldTransform(out m_worldTransform);
			}
		}

		public void SetAngularFactor(float angFac)
		{
			m_angularFactor = new IndexedVector3(angFac);
		}

		public void SetAngularFactor(IndexedVector3 angFac)
		{
			SetAngularFactor(ref angFac);
		}

		public void SetAngularFactor(ref IndexedVector3 angFac)
		{
			m_angularFactor = angFac;
		}

		public IndexedVector3 GetAngularFactor()
		{
			return m_angularFactor;
		}

		public void SetFlags(RigidBodyFlags flags)
		{
			m_rigidbodyFlags = flags;
		}

		public RigidBodyFlags GetFlags()
		{
			return m_rigidbodyFlags;
		}

		public IndexedVector3 GetDeltaLinearVelocity()
		{
			return m_deltaLinearVelocity;
		}

		public IndexedVector3 GetDeltaAngularVelocity()
		{
			return m_deltaAngularVelocity;
		}

		public IndexedVector3 GetPushVelocity()
		{
			return m_pushVelocity;
		}

		public IndexedVector3 GetTurnVelocity()
		{
			return m_turnVelocity;
		}

		public bool IsInWorld()
		{
			return GetBroadphaseProxy() != null;
		}

		public override bool CheckCollideWithOverride(CollisionObject co)
		{
			RigidBody rigidBody = Upcast(co);
			if (rigidBody == null)
			{
				return true;
			}
			for (int i = 0; i < m_constraintRefs.Count; i++)
			{
				TypedConstraint typedConstraint = m_constraintRefs[i];
				if (typedConstraint.IsEnabled() && (typedConstraint.GetRigidBodyA() == rigidBody || typedConstraint.GetRigidBodyB() == rigidBody))
				{
					return false;
				}
			}
			return true;
		}

		public void AddConstraintRef(TypedConstraint c)
		{
			if (!m_constraintRefs.Contains(c))
			{
				m_constraintRefs.Add(c);
			}
			m_checkCollideWith = true;
		}

		public void RemoveConstraintRef(TypedConstraint c)
		{
			m_constraintRefs.Remove(c);
			m_checkCollideWith = m_constraintRefs.Count > 0;
		}

		public TypedConstraint GetConstraintRef(int index)
		{
			return m_constraintRefs[index];
		}

		public int GetNumConstraintRefs()
		{
			return m_constraintRefs.Count;
		}

		public IndexedVector3 InternalGetDeltaLinearVelocity()
		{
			return m_deltaLinearVelocity;
		}

		public void InternalSetDeltaLinearVelocity(ref IndexedVector3 v)
		{
			m_deltaLinearVelocity = v;
		}

		public IndexedVector3 InternalGetDeltaAngularVelocity()
		{
			return m_deltaAngularVelocity;
		}

		public void InternalSetDeltaAngularVelocity(ref IndexedVector3 v)
		{
			m_deltaAngularVelocity = v;
		}

		public IndexedVector3 InternalGetAngularFactor()
		{
			return m_angularFactor;
		}

		public IndexedVector3 InternalGetInvMass()
		{
			return m_invMass;
		}

		public IndexedVector3 InternalGetPushVelocity()
		{
			return m_pushVelocity;
		}

		public IndexedVector3 InternalGetTurnVelocity()
		{
			return m_turnVelocity;
		}

		public void InternalSetTurnVelocity(ref IndexedVector3 velocity)
		{
			m_turnVelocity = velocity;
		}

		public void InternalSetPushVelocity(ref IndexedVector3 velocity)
		{
			m_pushVelocity = velocity;
		}

		public void InternalGetVelocityInLocalPointObsolete(ref IndexedVector3 rel_pos, ref IndexedVector3 velocity)
		{
			velocity = GetLinearVelocity() + m_deltaLinearVelocity + IndexedVector3.Cross(GetAngularVelocity() + m_deltaAngularVelocity, rel_pos);
		}

		public void InternalGetAngularVelocity(ref IndexedVector3 angVel)
		{
			angVel = GetAngularVelocity() + m_deltaAngularVelocity;
		}

		public void InternalApplyImpulse(ref IndexedVector3 linearComponent, ref IndexedVector3 angularComponent, float impulseMagnitude, string caller)
		{
			if (m_inverseMass != 0f)
			{
				m_deltaLinearVelocity.X += impulseMagnitude * linearComponent.X;
				m_deltaLinearVelocity.Y += impulseMagnitude * linearComponent.Y;
				m_deltaLinearVelocity.Z += impulseMagnitude * linearComponent.Z;
				m_deltaAngularVelocity.X += angularComponent.X * (impulseMagnitude * m_angularFactor.X);
				m_deltaAngularVelocity.Y += angularComponent.Y * (impulseMagnitude * m_angularFactor.Y);
				m_deltaAngularVelocity.Z += angularComponent.Z * (impulseMagnitude * m_angularFactor.Z);
			}
		}

		public void InternalApplyPushImpulse(IndexedVector3 linearComponent, IndexedVector3 angularComponent, float impulseMagnitude)
		{
			InternalApplyPushImpulse(ref linearComponent, ref angularComponent, impulseMagnitude);
		}

		public void InternalApplyPushImpulse(ref IndexedVector3 linearComponent, ref IndexedVector3 angularComponent, float impulseMagnitude)
		{
			if (m_inverseMass != 0f)
			{
				m_pushVelocity += linearComponent * impulseMagnitude;
				m_turnVelocity += angularComponent * (impulseMagnitude * m_angularFactor);
			}
		}

		public void InternalWritebackVelocity()
		{
			if (m_inverseMass != 0f)
			{
				SetLinearVelocity(GetLinearVelocity() + m_deltaLinearVelocity);
				SetAngularVelocity(GetAngularVelocity() + m_deltaAngularVelocity);
			}
		}

		public void InternalWritebackVelocity(float timeStep)
		{
			if (m_inverseMass != 0f)
			{
				SetLinearVelocity(GetLinearVelocity() + m_deltaLinearVelocity);
				SetAngularVelocity(GetAngularVelocity() + m_deltaAngularVelocity);
				IndexedMatrix predictedTransform;
				TransformUtil.IntegrateTransform(GetWorldTransform(), m_pushVelocity, m_turnVelocity, timeStep, out predictedTransform);
				SetWorldTransform(ref predictedTransform);
			}
		}
	}
}
