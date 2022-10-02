using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class TranslationalLimitMotor
	{
		public IndexedVector3 m_lowerLimit;

		public IndexedVector3 m_upperLimit;

		public IndexedVector3 m_accumulatedImpulse;

		public float m_limitSoftness;

		public float m_damping;

		public float m_restitution;

		public IndexedVector3 m_normalCFM;

		public IndexedVector3 m_stopERP;

		public IndexedVector3 m_stopCFM;

		public bool[] m_enableMotor = new bool[3];

		public IndexedVector3 m_targetVelocity;

		public IndexedVector3 m_maxMotorForce;

		public IndexedVector3 m_currentLimitError;

		public IndexedVector3 m_currentLinearDiff;

		public int[] m_currentLimit = new int[3];

		public TranslationalLimitMotor()
		{
			m_lowerLimit = IndexedVector3.Zero;
			m_upperLimit = IndexedVector3.Zero;
			m_accumulatedImpulse = IndexedVector3.Zero;
			m_normalCFM = IndexedVector3.Zero;
			m_stopERP = new IndexedVector3(0.2f, 0.2f, 0.2f);
			m_stopCFM = IndexedVector3.Zero;
			m_limitSoftness = 0.7f;
			m_damping = 1f;
			m_restitution = 0.5f;
			for (int i = 0; i < 3; i++)
			{
				m_enableMotor[i] = false;
			}
			m_targetVelocity = IndexedVector3.Zero;
			m_maxMotorForce = IndexedVector3.Zero;
		}

		public TranslationalLimitMotor(TranslationalLimitMotor other)
		{
			m_lowerLimit = other.m_lowerLimit;
			m_upperLimit = other.m_upperLimit;
			m_accumulatedImpulse = other.m_accumulatedImpulse;
			m_limitSoftness = other.m_limitSoftness;
			m_damping = other.m_damping;
			m_restitution = other.m_restitution;
			m_normalCFM = other.m_normalCFM;
			m_stopERP = other.m_stopERP;
			m_stopCFM = other.m_stopCFM;
			for (int i = 0; i < 3; i++)
			{
				m_enableMotor[i] = other.m_enableMotor[i];
			}
			m_targetVelocity = other.m_targetVelocity;
			m_maxMotorForce = other.m_maxMotorForce;
		}

		public bool IsLimited(int limitIndex)
		{
			return m_upperLimit[limitIndex] >= m_lowerLimit[limitIndex];
		}

		public bool NeedApplyForce(int limitIndex)
		{
			if (m_currentLimit[limitIndex] == 0 && !m_enableMotor[limitIndex])
			{
				return false;
			}
			return true;
		}

		public int TestLimitValue(int limitIndex, float test_value)
		{
			float num = m_lowerLimit[limitIndex];
			float num2 = m_upperLimit[limitIndex];
			if (num > num2)
			{
				m_currentLimit[limitIndex] = 0;
				m_currentLimitError[limitIndex] = 0f;
				return 0;
			}
			if (test_value < num)
			{
				m_currentLimit[limitIndex] = 2;
				m_currentLimitError[limitIndex] = test_value - num;
				return 2;
			}
			if (test_value > num2)
			{
				m_currentLimit[limitIndex] = 1;
				m_currentLimitError[limitIndex] = test_value - num2;
				return 1;
			}
			m_currentLimit[limitIndex] = 0;
			m_currentLimitError[limitIndex] = 0f;
			return 0;
		}

		public float SolveLinearAxis(float timeStep, float jacDiagABInv, RigidBody body1, ref IndexedVector3 pointInA, RigidBody body2, ref IndexedVector3 pointInB, int limit_index, ref IndexedVector3 axis_normal_on_a, ref IndexedVector3 anchorPos)
		{
			IndexedVector3 rel_pos = anchorPos - body1.GetCenterOfMassPosition();
			IndexedVector3 rel_pos2 = anchorPos - body2.GetCenterOfMassPosition();
			IndexedVector3 velocity = IndexedVector3.Zero;
			body1.InternalGetVelocityInLocalPointObsolete(ref rel_pos, ref velocity);
			IndexedVector3 velocity2 = IndexedVector3.Zero;
			body2.InternalGetVelocityInLocalPointObsolete(ref rel_pos2, ref velocity2);
			IndexedVector3 b = velocity - velocity2;
			float num = IndexedVector3.Dot(axis_normal_on_a, b);
			float num2 = 0f - IndexedVector3.Dot(pointInA - pointInB, axis_normal_on_a);
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			float num5 = m_lowerLimit[limit_index];
			float num6 = m_upperLimit[limit_index];
			if (num5 < num6)
			{
				if (num2 > num6)
				{
					num2 -= num6;
					num3 = 0f;
				}
				else
				{
					if (!(num2 < num5))
					{
						return 0f;
					}
					num2 -= num5;
					num4 = 0f;
				}
			}
			float num7 = m_limitSoftness * (m_restitution * num2 / timeStep - m_damping * num) * jacDiagABInv;
			float num8 = m_accumulatedImpulse[limit_index];
			float num9 = num8 + num7;
			m_accumulatedImpulse[limit_index] = ((num9 > num4) ? 0f : ((num9 < num3) ? 0f : num9));
			num7 = m_accumulatedImpulse[limit_index] - num8;
			IndexedVector3 indexedVector3 = axis_normal_on_a * num7;
			IndexedVector3 indexedVector = IndexedVector3.Cross(rel_pos, axis_normal_on_a);
			IndexedVector3 indexedVector2 = IndexedVector3.Cross(rel_pos2, axis_normal_on_a);
			body1.InternalApplyImpulse(axis_normal_on_a * body1.GetInvMass(), body1.GetInvInertiaTensorWorld() * indexedVector, num7, "Generic6DoF body1");
			body2.InternalApplyImpulse(axis_normal_on_a * body2.GetInvMass(), body2.GetInvInertiaTensorWorld() * indexedVector2, 0f - num7, "Generic6DoF body2");
			return num7;
		}
	}
}
