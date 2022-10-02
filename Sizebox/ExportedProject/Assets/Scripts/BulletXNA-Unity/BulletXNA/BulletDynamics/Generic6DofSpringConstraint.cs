using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class Generic6DofSpringConstraint : Generic6DofConstraint
	{
		private const int s_degreesOfFreedom = 6;

		protected bool[] m_springEnabled = new bool[6];

		protected float[] m_equilibriumPoint = new float[6];

		protected float[] m_springStiffness = new float[6];

		protected float[] m_springDamping = new float[6];

		public Generic6DofSpringConstraint(RigidBody rbA, RigidBody rbB, IndexedMatrix frameInA, IndexedMatrix frameInB, bool useLinearReferenceFrameA)
			: this(rbA, rbB, ref frameInA, ref frameInB, useLinearReferenceFrameA)
		{
			Init();
		}

		public Generic6DofSpringConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix frameInA, ref IndexedMatrix frameInB, bool useLinearReferenceFrameA)
			: base(rbA, rbB, ref frameInA, ref frameInB, useLinearReferenceFrameA)
		{
			Init();
		}

		protected void Init()
		{
			m_constraintType = TypedConstraintType.D6_SPRING_CONSTRAINT_TYPE;
			for (int i = 0; i < 6; i++)
			{
				m_springEnabled[i] = false;
				m_equilibriumPoint[i] = 0f;
				m_springStiffness[i] = 0f;
				m_springDamping[i] = 1f;
			}
		}

		protected void InternalUpdateSprings(ConstraintInfo2 info)
		{
			IndexedVector3 indexedVector = m_rbB.GetLinearVelocity() - m_rbA.GetLinearVelocity();
			for (int i = 0; i < 3; i++)
			{
				if (m_springEnabled[i])
				{
					float num = m_calculatedLinearDiff[i];
					float num2 = num - m_equilibriumPoint[i];
					float num3 = num2 * m_springStiffness[i];
					float num4 = info.fps * m_springDamping[i] / (float)info.m_numIterations;
					m_linearLimits.m_targetVelocity[i] = num4 * num3;
					m_linearLimits.m_maxMotorForce[i] = Math.Abs(num3) / info.fps;
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (m_springEnabled[j + 3])
				{
					float num5 = m_calculatedAxisAngleDiff[j];
					float num6 = num5 - m_equilibriumPoint[j + 3];
					float num7 = (0f - num6) * m_springStiffness[j + 3];
					float num8 = info.fps * m_springDamping[j + 3] / (float)info.m_numIterations;
					m_angularLimits[j].m_targetVelocity = num8 * num7;
					m_angularLimits[j].m_maxMotorForce = Math.Abs(num7) / info.fps;
				}
			}
		}

		public void EnableSpring(int index, bool onOff)
		{
			m_springEnabled[index] = onOff;
			if (index < 3)
			{
				m_linearLimits.m_enableMotor[index] = onOff;
			}
			else
			{
				m_angularLimits[index - 3].m_enableMotor = onOff;
			}
		}

		public void SetStiffness(int index, float stiffness)
		{
			m_springStiffness[index] = stiffness;
		}

		public void SetDamping(int index, float damping)
		{
			m_springDamping[index] = damping;
		}

		public void SetEquilibriumPoint()
		{
			CalculateTransforms();
			for (int i = 0; i < 3; i++)
			{
				m_equilibriumPoint[i] = m_calculatedLinearDiff[i];
			}
			for (int j = 0; j < 3; j++)
			{
				m_equilibriumPoint[j + 3] = m_calculatedAxisAngleDiff[j];
			}
		}

		public void SetEquilibriumPoint(int index)
		{
			CalculateTransforms();
			if (index < 3)
			{
				m_equilibriumPoint[index] = m_calculatedLinearDiff[index];
			}
			else
			{
				m_equilibriumPoint[index] = m_calculatedAxisAngleDiff[index - 3];
			}
		}

		public void SetEquilibriumPoint(int index, float val)
		{
			m_equilibriumPoint[index] = val;
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
			InternalUpdateSprings(info);
			base.GetInfo2(info);
		}

		public override void SetAxis(ref IndexedVector3 axis1, ref IndexedVector3 axis2)
		{
			IndexedVector3 v = IndexedVector3.Normalize(axis1);
			IndexedVector3 v2 = IndexedVector3.Normalize(axis2);
			IndexedVector3 indexedVector = IndexedVector3.Cross(v2, v);
			IndexedMatrix identity = IndexedMatrix.Identity;
			identity._basis = new IndexedBasisMatrix(indexedVector.X, v2.X, v.X, indexedVector.Y, v2.Y, v.Y, indexedVector.Z, v2.Z, v.Z);
			m_frameInA = m_rbA.GetCenterOfMassTransform().Inverse() * identity;
			m_frameInB = m_rbB.GetCenterOfMassTransform().Inverse() * identity;
			CalculateTransforms();
		}
	}
}
