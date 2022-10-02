using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class RotationalLimitMotor
	{
		public float m_loLimit;

		public float m_hiLimit;

		public float m_targetVelocity;

		public float m_maxMotorForce;

		public float m_maxLimitForce;

		public float m_damping;

		public float m_limitSoftness;

		public float m_normalCFM;

		public float m_stopERP;

		public float m_stopCFM;

		public float m_bounce;

		public bool m_enableMotor;

		public float m_currentLimitError;

		public float m_currentPosition;

		public int m_currentLimit;

		public float m_accumulatedImpulse;

		public RotationalLimitMotor()
		{
			m_accumulatedImpulse = 0f;
			m_targetVelocity = 0f;
			m_maxMotorForce = 0.1f;
			m_maxLimitForce = 300f;
			m_loLimit = 1f;
			m_hiLimit = -1f;
			m_normalCFM = 0f;
			m_stopERP = 0.2f;
			m_stopCFM = 0f;
			m_bounce = 0f;
			m_damping = 1f;
			m_limitSoftness = 0.5f;
			m_currentLimit = 0;
			m_currentLimitError = 0f;
			m_enableMotor = false;
		}

		public RotationalLimitMotor(RotationalLimitMotor limot)
		{
			m_targetVelocity = limot.m_targetVelocity;
			m_maxMotorForce = limot.m_maxMotorForce;
			m_limitSoftness = limot.m_limitSoftness;
			m_loLimit = limot.m_loLimit;
			m_hiLimit = limot.m_hiLimit;
			m_normalCFM = limot.m_normalCFM;
			m_stopERP = limot.m_stopERP;
			m_stopCFM = limot.m_stopCFM;
			m_bounce = limot.m_bounce;
			m_currentLimit = limot.m_currentLimit;
			m_currentLimitError = limot.m_currentLimitError;
			m_enableMotor = limot.m_enableMotor;
		}

		public bool IsLimited()
		{
			if (m_loLimit > m_hiLimit)
			{
				return false;
			}
			return true;
		}

		public bool NeedApplyTorques()
		{
			if (m_currentLimit == 0 && !m_enableMotor)
			{
				return false;
			}
			return true;
		}

		public int TestLimitValue(float test_value)
		{
			if (m_loLimit > m_hiLimit)
			{
				m_currentLimit = 0;
				return 0;
			}
			if (test_value < m_loLimit)
			{
				m_currentLimit = 1;
				m_currentLimitError = test_value - m_loLimit;
				if (m_currentLimitError > (float)Math.PI)
				{
					m_currentLimitError -= (float)Math.PI * 2f;
				}
				else if (m_currentLimitError < -(float)Math.PI)
				{
					m_currentLimitError += (float)Math.PI * 2f;
				}
				return 1;
			}
			if (test_value > m_hiLimit)
			{
				m_currentLimit = 2;
				m_currentLimitError = test_value - m_hiLimit;
				if (m_currentLimitError > (float)Math.PI)
				{
					m_currentLimitError -= (float)Math.PI * 2f;
				}
				else if (m_currentLimitError < -(float)Math.PI)
				{
					m_currentLimitError += (float)Math.PI * 2f;
				}
				return 2;
			}
			m_currentLimit = 0;
			return 0;
		}

		public float SolveAngularLimits(float timeStep, ref IndexedVector3 axis, float jacDiagABInv, RigidBody body0, RigidBody body1)
		{
			if (!NeedApplyTorques())
			{
				return 0f;
			}
			float num = m_targetVelocity;
			float num2 = m_maxMotorForce;
			if (m_currentLimit != 0)
			{
				num = (0f - m_stopERP) * m_currentLimitError / timeStep;
				num2 = m_maxLimitForce;
			}
			num2 *= timeStep;
			IndexedVector3 angVel = IndexedVector3.Zero;
			body0.InternalGetAngularVelocity(ref angVel);
			IndexedVector3 angVel2 = IndexedVector3.Zero;
			body1.InternalGetAngularVelocity(ref angVel2);
			IndexedVector3 b = angVel - angVel2;
			float num3 = IndexedVector3.Dot(axis, b);
			float num4 = m_limitSoftness * (num - m_damping * num3);
			if (num4 < 1.1920929E-07f && num4 > -1.1920929E-07f)
			{
				return 0f;
			}
			float num5 = (1f + m_bounce) * num4 * jacDiagABInv;
			float num6 = ((!(num5 > 0f)) ? ((num5 < 0f - num2) ? (0f - num2) : num5) : ((num5 > num2) ? num2 : num5));
			float num7 = float.MinValue;
			float num8 = float.MaxValue;
			float accumulatedImpulse = m_accumulatedImpulse;
			float num9 = accumulatedImpulse + num6;
			m_accumulatedImpulse = ((num9 > num8) ? 0f : ((num9 < num7) ? 0f : num9));
			num6 = m_accumulatedImpulse - accumulatedImpulse;
			IndexedVector3 indexedVector = num6 * axis;
			body0.InternalApplyImpulse(IndexedVector3.Zero, body0.GetInvInertiaTensorWorld() * axis, num6, "Generic6DoF body0");
			body1.InternalApplyImpulse(IndexedVector3.Zero, body1.GetInvInertiaTensorWorld() * axis, 0f - num6, "Generic6DoF body1");
			return num6;
		}
	}
}
