using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class ContactConstraint : TypedConstraint
	{
		public const bool ONLY_USE_LINEAR_MASS = false;

		public const bool USE_INTERNAL_APPLY_IMPULSE = true;

		protected PersistentManifold m_contactManifold;

		public ContactConstraint(PersistentManifold contactManifold, RigidBody rbA, RigidBody rbB)
			: base(TypedConstraintType.CONTACT_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_contactManifold = contactManifold;
		}

		public PersistentManifold GetContactManifold()
		{
			return m_contactManifold;
		}

		public void SetContactManifold(PersistentManifold contactManifold)
		{
			m_contactManifold = contactManifold;
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
		}

		public static float ResolveSingleCollision(RigidBody body1, CollisionObject colObj2, ref IndexedVector3 contactPositionWorld, ref IndexedVector3 contactNormalOnB, ContactSolverInfo solverInfo, float distance)
		{
			RigidBody rigidBody = RigidBody.Upcast(colObj2);
			IndexedVector3 normal = contactNormalOnB;
			IndexedVector3 rel_pos = contactPositionWorld - body1.GetWorldTransform()._origin;
			IndexedVector3 rel_pos2 = contactPositionWorld - colObj2.GetWorldTransform()._origin;
			IndexedVector3 velocityInLocalPoint = body1.GetVelocityInLocalPoint(ref rel_pos);
			IndexedVector3 indexedVector = ((rigidBody != null) ? rigidBody.GetVelocityInLocalPoint(ref rel_pos2) : IndexedVector3.Zero);
			IndexedVector3 v = velocityInLocalPoint - indexedVector;
			float num = normal.Dot(ref v);
			float num2 = body1.GetRestitution() * colObj2.GetRestitution();
			float num3 = num2 * (0f - num);
			float num4 = solverInfo.m_erp * (0f - distance) / solverInfo.m_timeStep;
			float num5 = (0f - (1f + num3)) * num;
			float num6 = body1.ComputeImpulseDenominator(ref contactPositionWorld, ref normal);
			float num7 = ((rigidBody != null) ? rigidBody.ComputeImpulseDenominator(ref contactPositionWorld, ref normal) : 0f);
			float num8 = 1f;
			float num9 = num8 / (num6 + num7);
			float num10 = num4 * num9;
			float num11 = num5 * num9;
			float num12 = num10 + num11;
			num12 = ((0f > num12) ? 0f : num12);
			body1.ApplyImpulse(normal * num12, rel_pos);
			if (rigidBody != null)
			{
				rigidBody.ApplyImpulse(-normal * num12, rel_pos2);
			}
			return num12;
		}

		public static void ResolveSingleBilateral(RigidBody body1, ref IndexedVector3 pos1, RigidBody body2, ref IndexedVector3 pos2, float distance, ref IndexedVector3 normal, ref float impulse, float timeStep)
		{
			float num = normal.LengthSquared();
			if (num > 1.1f)
			{
				impulse = 0f;
				return;
			}
			IndexedVector3 rel_pos = pos1 - body1.GetCenterOfMassPosition();
			IndexedVector3 rel_pos2 = pos2 - body2.GetCenterOfMassPosition();
			IndexedVector3 velocityInLocalPoint = body1.GetVelocityInLocalPoint(ref rel_pos);
			IndexedVector3 velocityInLocalPoint2 = body2.GetVelocityInLocalPoint(ref rel_pos2);
			IndexedVector3 v = velocityInLocalPoint - velocityInLocalPoint2;
			IndexedBasisMatrix world2A = body1.GetCenterOfMassTransform()._basis.Transpose();
			IndexedBasisMatrix world2B = body2.GetCenterOfMassTransform()._basis.Transpose();
			JacobianEntry jacobianEntry = new JacobianEntry(world2A, world2B, rel_pos, rel_pos2, normal, body1.GetInvInertiaDiagLocal(), body1.GetInvMass(), body2.GetInvInertiaDiagLocal(), body2.GetInvMass());
			float diagonal = jacobianEntry.GetDiagonal();
			float num2 = 1f / diagonal;
			float relativeVelocity = jacobianEntry.GetRelativeVelocity(body1.GetLinearVelocity(), body1.GetCenterOfMassTransform()._basis.Transpose() * body1.GetAngularVelocity(), body2.GetLinearVelocity(), body2.GetCenterOfMassTransform()._basis.Transpose() * body2.GetAngularVelocity());
			relativeVelocity = normal.Dot(ref v);
			float num3 = 0.2f;
			float num4 = (0f - num3) * relativeVelocity * num2;
			impulse = num4;
		}
	}
}
