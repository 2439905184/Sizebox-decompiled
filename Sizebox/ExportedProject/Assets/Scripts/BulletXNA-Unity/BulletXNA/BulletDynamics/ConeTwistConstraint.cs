using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class ConeTwistConstraint : TypedConstraint
	{
		public const float CONETWIST_DEF_FIX_THRESH = 0.05f;

		public static IndexedVector3 vTwist = new IndexedVector3(1f, 0f, 0f);

		public JacobianEntry[] m_jac = new JacobianEntry[3];

		public IndexedMatrix m_rbAFrame = IndexedMatrix.Identity;

		public IndexedMatrix m_rbBFrame = IndexedMatrix.Identity;

		public float m_limitSoftness;

		public float m_biasFactor;

		public float m_relaxationFactor;

		public float m_damping;

		public float m_swingSpan1;

		public float m_swingSpan2;

		public float m_twistSpan;

		public float m_fixThresh;

		public IndexedVector3 m_swingAxis;

		public IndexedVector3 m_twistAxis;

		public float m_kSwing;

		public float m_kTwist;

		public float m_twistLimitSign;

		public float m_swingCorrection;

		public float m_twistCorrection;

		public float m_twistAngle;

		public float m_accSwingLimitImpulse;

		public float m_accTwistLimitImpulse;

		public bool m_angularOnly;

		public bool m_solveTwistLimit;

		public bool m_solveSwingLimit;

		public float m_swingLimitRatio;

		public float m_twistLimitRatio;

		public IndexedVector3 m_twistAxisA;

		public bool m_bMotorEnabled;

		public bool m_bNormalizedMotorStrength;

		public IndexedQuaternion m_qTarget = IndexedQuaternion.Identity;

		public float m_maxMotorImpulse;

		public IndexedVector3 m_accMotorImpulse;

		public int m_flags;

		public float m_linCFM;

		public float m_linERP;

		public float m_angCFM;

		private bool m_useSolveConstraintObsolete;

		private static bool bDoTorque = true;

		public ConeTwistConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix rbAFrame, ref IndexedMatrix rbBFrame)
			: base(TypedConstraintType.CONETWIST_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_angularOnly = false;
			m_rbAFrame = rbAFrame;
			m_rbBFrame = rbBFrame;
			Init();
		}

		public ConeTwistConstraint(RigidBody rbA, ref IndexedMatrix rbAFrame)
			: base(TypedConstraintType.CONETWIST_CONSTRAINT_TYPE, rbA)
		{
			m_rbAFrame = rbAFrame;
			m_rbBFrame = rbAFrame;
			m_angularOnly = false;
			Init();
		}

		protected void Init()
		{
			m_angularOnly = false;
			m_solveTwistLimit = false;
			m_solveSwingLimit = false;
			m_bMotorEnabled = false;
			m_maxMotorImpulse = -1f;
			SetLimit(1E+18f, 1E+18f, 1E+18f);
			m_damping = 0.01f;
			m_fixThresh = 0.05f;
			m_flags = 0;
			m_linCFM = 0f;
			m_linERP = 0.7f;
			m_angCFM = 0f;
		}

		public static float ComputeAngularImpulseDenominator(ref IndexedVector3 axis, ref IndexedBasisMatrix invInertiaWorld)
		{
			IndexedVector3 v = axis * invInertiaWorld;
			return axis.Dot(ref v);
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 6;
			info.nub = 0;
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
			if (m_useSolveConstraintObsolete)
			{
				info.m_numConstraintRows = 0;
				info.nub = 0;
				return;
			}
			info.m_numConstraintRows = 3;
			info.nub = 3;
			CalcAngleInfo2(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetInvInertiaTensorWorld(), m_rbB.GetInvInertiaTensorWorld());
			if (m_solveSwingLimit)
			{
				info.m_numConstraintRows++;
				info.nub--;
				if (m_swingSpan1 < m_fixThresh && m_swingSpan2 < m_fixThresh)
				{
					info.m_numConstraintRows++;
					info.nub--;
				}
			}
			if (m_solveTwistLimit)
			{
				info.m_numConstraintRows++;
				info.nub--;
			}
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
			GetInfo2NonVirtual(info, m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetInvInertiaTensorWorld(), m_rbB.GetInvInertiaTensorWorld());
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedBasisMatrix invInertiaWorldA, IndexedBasisMatrix invInertiaWorldB)
		{
			CalcAngleInfo2(ref transA, ref transB, ref invInertiaWorldA, ref invInertiaWorldB);
			info.m_solverConstraints[0].m_contactNormal.X = 1f;
			info.m_solverConstraints[1].m_contactNormal.Y = 1f;
			info.m_solverConstraints[2].m_contactNormal.Z = 1f;
			IndexedVector3 indexedVector = transA._basis * m_rbAFrame._origin;
			IndexedVector3 vecin = -indexedVector;
			MathUtil.GetSkewSymmetricMatrix(ref vecin, out info.m_solverConstraints[0].m_relpos1CrossNormal, out info.m_solverConstraints[1].m_relpos1CrossNormal, out info.m_solverConstraints[2].m_relpos1CrossNormal);
			IndexedVector3 vecin2 = transB._basis * m_rbBFrame._origin;
			MathUtil.GetSkewSymmetricMatrix(ref vecin2, out info.m_solverConstraints[0].m_relpos2CrossNormal, out info.m_solverConstraints[1].m_relpos2CrossNormal, out info.m_solverConstraints[2].m_relpos2CrossNormal);
			float num = ((((uint)m_flags & 2u) != 0) ? m_linERP : info.erp);
			float num2 = info.fps * num;
			for (int i = 0; i < 3; i++)
			{
				info.m_solverConstraints[i].m_rhs = num2 * (vecin2[i] + transB._origin[i] - indexedVector[i] - transA._origin[i]);
				info.m_solverConstraints[i].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[i].m_upperLimit = float.MaxValue;
				if (((uint)m_flags & (true ? 1u : 0u)) != 0)
				{
					info.m_solverConstraints[i].m_cfm = m_linCFM;
				}
			}
			int num3 = 3;
			IndexedVector3 indexedVector2;
			if (m_solveSwingLimit)
			{
				if (m_swingSpan1 < m_fixThresh && m_swingSpan2 < m_fixThresh)
				{
					IndexedMatrix indexedMatrix = transA * m_rbAFrame;
					IndexedVector3 v = indexedMatrix._basis.GetColumn(1);
					IndexedVector3 v2 = indexedMatrix._basis.GetColumn(2);
					info.m_solverConstraints[num3].m_relpos1CrossNormal = v;
					info.m_solverConstraints[num3 + 1].m_relpos1CrossNormal = v2;
					info.m_solverConstraints[num3].m_relpos2CrossNormal = -v;
					info.m_solverConstraints[num3 + 1].m_relpos2CrossNormal = -v2;
					float num4 = info.fps * m_relaxationFactor;
					info.m_solverConstraints[num3].m_rhs = num4 * m_swingAxis.Dot(ref v);
					info.m_solverConstraints[num3 + 1].m_rhs = num4 * m_swingAxis.Dot(ref v2);
					info.m_solverConstraints[num3].m_lowerLimit = float.MinValue;
					info.m_solverConstraints[num3].m_upperLimit = float.MaxValue;
					info.m_solverConstraints[num3 + 1].m_lowerLimit = float.MinValue;
					info.m_solverConstraints[num3 + 1].m_upperLimit = float.MaxValue;
					num3 += 2;
				}
				else
				{
					indexedVector2 = m_swingAxis * m_relaxationFactor * m_relaxationFactor;
					info.m_solverConstraints[num3].m_relpos1CrossNormal = indexedVector2;
					info.m_solverConstraints[num3].m_relpos2CrossNormal = -indexedVector2;
					float num5 = info.fps * m_biasFactor;
					info.m_solverConstraints[num3].m_rhs = num5 * m_swingCorrection;
					if (((uint)m_flags & 4u) != 0)
					{
						info.m_solverConstraints[num3].m_cfm = m_angCFM;
					}
					info.m_solverConstraints[num3].m_lowerLimit = 0f;
					info.m_solverConstraints[num3].m_upperLimit = float.MaxValue;
					num3++;
				}
			}
			if (!m_solveTwistLimit)
			{
				return;
			}
			indexedVector2 = m_twistAxis * m_relaxationFactor * m_relaxationFactor;
			info.m_solverConstraints[num3].m_relpos1CrossNormal = indexedVector2;
			info.m_solverConstraints[num3].m_relpos2CrossNormal = -indexedVector2;
			float num6 = info.fps * m_biasFactor;
			info.m_solverConstraints[num3].m_rhs = num6 * m_twistCorrection;
			if (((uint)m_flags & 4u) != 0)
			{
				info.m_solverConstraints[num3].m_cfm = m_angCFM;
			}
			if (m_twistSpan > 0f)
			{
				if (m_twistCorrection > 0f)
				{
					info.m_solverConstraints[num3].m_lowerLimit = 0f;
					info.m_solverConstraints[num3].m_upperLimit = float.MaxValue;
				}
				else
				{
					info.m_solverConstraints[num3].m_lowerLimit = float.MinValue;
					info.m_solverConstraints[num3].m_upperLimit = 0f;
				}
			}
			else
			{
				info.m_solverConstraints[num3].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num3].m_upperLimit = float.MaxValue;
			}
			num3++;
		}

		public void UpdateRHS(float timeStep)
		{
		}

		private void SetAngularOnly(bool angularOnly)
		{
			m_angularOnly = angularOnly;
		}

		private void SetLimit(int limitIndex, float limitValue)
		{
			switch (limitIndex)
			{
			case 3:
				m_twistSpan = limitValue;
				break;
			case 4:
				m_swingSpan2 = limitValue;
				break;
			case 5:
				m_swingSpan1 = limitValue;
				break;
			}
		}

		public void SetLimit(float _swingSpan1, float _swingSpan2, float _twistSpan)
		{
			SetLimit(_swingSpan1, _swingSpan2, _twistSpan, 1f, 0.3f, 1f);
		}

		public void SetLimit(float _swingSpan1, float _swingSpan2, float _twistSpan, float _softness)
		{
			SetLimit(_swingSpan1, _swingSpan2, _twistSpan, _softness, 0.3f, 1f);
		}

		public void SetLimit(float _swingSpan1, float _swingSpan2, float _twistSpan, float _softness, float _biasFactor, float _relaxationFactor)
		{
			m_swingSpan1 = _swingSpan1;
			m_swingSpan2 = _swingSpan2;
			m_twistSpan = _twistSpan;
			m_limitSoftness = _softness;
			m_biasFactor = _biasFactor;
			m_relaxationFactor = _relaxationFactor;
		}

		public IndexedMatrix GetAFrame()
		{
			return m_rbAFrame;
		}

		public IndexedMatrix GetBFrame()
		{
			return m_rbBFrame;
		}

		public bool GetSolveTwistLimit()
		{
			return m_solveTwistLimit;
		}

		public bool GetSolveSwingLimit()
		{
			return m_solveTwistLimit;
		}

		public float GetTwistLimitSign()
		{
			return m_twistLimitSign;
		}

		public void CalcAngleInfo()
		{
			m_swingCorrection = 0f;
			m_twistLimitSign = 0f;
			m_solveTwistLimit = false;
			m_solveSwingLimit = false;
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 v = IndexedVector3.Zero;
			IndexedVector3 v2 = IndexedVector3.Zero;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			IndexedVector3 zero3 = IndexedVector3.Zero;
			GetRigidBodyA().GetCenterOfMassTransform();
			GetRigidBodyB().GetCenterOfMassTransform();
			zero = GetRigidBodyA().GetCenterOfMassTransform()._basis * m_rbAFrame._basis.GetColumn(0);
			zero2 = GetRigidBodyB().GetCenterOfMassTransform()._basis * m_rbBFrame._basis.GetColumn(0);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 10f;
			if (m_swingSpan1 >= 0.05f)
			{
				v = GetRigidBodyA().GetCenterOfMassTransform()._basis * m_rbAFrame._basis.GetColumn(1);
				num3 = zero2.Dot(ref zero);
				num4 = zero2.Dot(ref v);
				num = (float)Math.Atan2(num4, num3);
				float num6 = (num4 * num4 + num3 * num3) * num5 * num5;
				num6 /= num6 + 1f;
				num *= num6;
			}
			if (m_swingSpan2 >= 0.05f)
			{
				v2 = GetRigidBodyA().GetCenterOfMassTransform()._basis * m_rbAFrame._basis.GetColumn(2);
				num3 = zero2.Dot(ref zero);
				num4 = zero2.Dot(ref v2);
				num2 = (float)Math.Atan2(num4, num3);
				float num6 = (num4 * num4 + num3 * num3) * num5 * num5;
				num6 /= num6 + 1f;
				num2 *= num6;
			}
			float num7 = 1f / (m_swingSpan1 * m_swingSpan1);
			float num8 = 1f / (m_swingSpan2 * m_swingSpan2);
			float num9 = Math.Abs(num * num) * num7 + Math.Abs(num2 * num2) * num8;
			if (num9 > 1f)
			{
				m_swingCorrection = num9 - 1f;
				m_solveSwingLimit = true;
				m_swingAxis = zero2.Cross(v * zero2.Dot(ref v) + v2 * zero2.Dot(ref v2));
				m_swingAxis.Normalize();
				float num10 = ((zero2.Dot(ref zero) >= 0f) ? 1f : (-1f));
				m_swingAxis *= num10;
			}
			if (m_twistSpan >= 0f)
			{
				IndexedVector3 v3 = GetRigidBodyB().GetCenterOfMassTransform()._basis * m_rbBFrame._basis.GetColumn(1);
				IndexedQuaternion rotation = MathUtil.ShortestArcQuat(ref zero2, ref zero);
				IndexedVector3 indexedVector = MathUtil.QuatRotate(ref rotation, ref v3);
				float num11 = (m_twistAngle = (float)Math.Atan2(indexedVector.Dot(ref v2), indexedVector.Dot(ref v)));
				float num12 = ((m_twistSpan > 0.05f) ? 1f : 0f);
				if (num11 <= (0f - m_twistSpan) * num12)
				{
					m_twistCorrection = 0f - (num11 + m_twistSpan);
					m_solveTwistLimit = true;
					m_twistAxis = (zero2 + zero) * 0.5f;
					m_twistAxis.Normalize();
					m_twistAxis *= -1f;
				}
				else if (num11 > m_twistSpan * num12)
				{
					m_twistCorrection = num11 - m_twistSpan;
					m_solveTwistLimit = true;
					m_twistAxis = (zero2 + zero) * 0.5f;
					m_twistAxis.Normalize();
				}
			}
		}

		public void CalcAngleInfo2(IndexedMatrix transA, IndexedMatrix transB, IndexedBasisMatrix invInertiaWorldA, IndexedBasisMatrix invInertiaWorldB)
		{
			CalcAngleInfo2(ref transA, ref transB, ref invInertiaWorldA, ref invInertiaWorldB);
		}

		public void CalcAngleInfo2(ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedBasisMatrix invInertiaWorldA, ref IndexedBasisMatrix invInertiaWorldB)
		{
			m_swingCorrection = 0f;
			m_twistLimitSign = 0f;
			m_solveTwistLimit = false;
			m_solveSwingLimit = false;
			if (m_bMotorEnabled && !m_useSolveConstraintObsolete)
			{
				IndexedMatrix indexedMatrix = IndexedMatrix.CreateFromQuaternion(m_qTarget);
				IndexedMatrix indexedMatrix2 = transA * m_rbAFrame;
				IndexedMatrix indexedMatrix3 = transB * m_rbBFrame;
				IndexedQuaternion quat = (indexedMatrix3 * indexedMatrix * indexedMatrix2.Inverse()).GetRotation();
				IndexedVector3 swingAxis = new IndexedVector3(quat.X, quat.Y, quat.Z);
				float val = swingAxis.LengthSquared();
				if (!MathUtil.FuzzyZero(val))
				{
					m_swingAxis = swingAxis;
					m_swingAxis.Normalize();
					m_swingCorrection = MathUtil.QuatAngle(ref quat);
					if (!MathUtil.FuzzyZero(m_swingCorrection))
					{
						m_solveSwingLimit = true;
					}
				}
				return;
			}
			IndexedQuaternion indexedQuaternion = transA.GetRotation() * m_rbAFrame.GetRotation();
			IndexedQuaternion indexedQuaternion2 = transB.GetRotation() * m_rbBFrame.GetRotation();
			IndexedQuaternion rotation = MathUtil.QuaternionInverse(indexedQuaternion2) * indexedQuaternion;
			IndexedVector3 axisInB = MathUtil.QuatRotate(ref rotation, ref vTwist);
			axisInB.Normalize();
			IndexedQuaternion qCone = MathUtil.ShortestArcQuat(ref vTwist, ref axisInB);
			qCone.Normalize();
			IndexedQuaternion qTwist = MathUtil.QuaternionInverse(qCone) * rotation;
			qTwist.Normalize();
			if (m_swingSpan1 >= m_fixThresh && m_swingSpan2 >= m_fixThresh)
			{
				float swingAngle = 0f;
				float swingLimit = 0f;
				IndexedVector3 vSwingAxis = IndexedVector3.Zero;
				ComputeConeLimitInfo(ref qCone, ref swingAngle, ref vSwingAxis, ref swingLimit);
				if (swingAngle > swingLimit * m_limitSoftness)
				{
					m_solveSwingLimit = true;
					m_swingLimitRatio = 1f;
					if (swingAngle < swingLimit && m_limitSoftness < 0.9999999f)
					{
						m_swingLimitRatio = (swingAngle - swingLimit * m_limitSoftness) / (swingLimit - swingLimit * m_limitSoftness);
					}
					m_swingCorrection = swingAngle - swingLimit * m_limitSoftness;
					AdjustSwingAxisToUseEllipseNormal(ref vSwingAxis);
					m_swingAxis = MathUtil.QuatRotate(indexedQuaternion2, -vSwingAxis);
					m_twistAxisA = IndexedVector3.Zero;
					m_kSwing = 1f / (ComputeAngularImpulseDenominator(ref m_swingAxis, ref invInertiaWorldA) + ComputeAngularImpulseDenominator(ref m_swingAxis, ref invInertiaWorldB));
				}
			}
			else
			{
				IndexedVector3 v = transA._basis * m_rbAFrame._basis.GetColumn(0);
				IndexedVector3 v2 = transA._basis * m_rbAFrame._basis.GetColumn(1);
				IndexedVector3 v3 = transA._basis * m_rbAFrame._basis.GetColumn(2);
				IndexedVector3 indexedVector = transB._basis * m_rbBFrame._basis.GetColumn(0);
				IndexedVector3 v4 = IndexedVector3.Zero;
				float num = indexedVector.Dot(ref v);
				float num2 = indexedVector.Dot(ref v2);
				float num3 = indexedVector.Dot(ref v3);
				if (m_swingSpan1 < m_fixThresh && m_swingSpan2 < m_fixThresh)
				{
					if (!MathUtil.FuzzyZero(num2) || !MathUtil.FuzzyZero(num3))
					{
						m_solveSwingLimit = true;
						m_swingAxis = -indexedVector.Cross(ref v);
					}
				}
				else
				{
					if (m_swingSpan1 < m_fixThresh)
					{
						if (!MathUtil.FuzzyZero(num2))
						{
							m_solveSwingLimit = true;
							if (m_swingSpan2 >= m_fixThresh)
							{
								num2 = 0f;
								float num4 = (float)Math.Atan2(num3, num);
								if (num4 > m_swingSpan2)
								{
									num = (float)Math.Cos(m_swingSpan2);
									num3 = (float)Math.Sin(m_swingSpan2);
								}
								else if (num4 < 0f - m_swingSpan2)
								{
									num = (float)Math.Cos(m_swingSpan2);
									num3 = 0f - (float)Math.Sin(m_swingSpan2);
								}
							}
						}
					}
					else if (!MathUtil.FuzzyZero(num3))
					{
						m_solveSwingLimit = true;
						if (m_swingSpan1 >= m_fixThresh)
						{
							num3 = 0f;
							float num5 = (float)Math.Atan2(num2, num);
							if (num5 > m_swingSpan1)
							{
								num = (float)Math.Cos(m_swingSpan1);
								num2 = (float)Math.Sin(m_swingSpan1);
							}
							else if (num5 < 0f - m_swingSpan1)
							{
								num = (float)Math.Cos(m_swingSpan1);
								num2 = 0f - (float)Math.Sin(m_swingSpan1);
							}
						}
					}
					v4.X = num * v.X + num2 * v2.X + num3 * v3.X;
					v4.Y = num * v.Y + num2 * v2.Y + num3 * v3.Y;
					v4.Z = num * v.Z + num2 * v2.Z + num3 * v3.Z;
					v4.Normalize();
					m_swingAxis = -indexedVector.Cross(ref v4);
					m_swingCorrection = m_swingAxis.Length();
					m_swingAxis.Normalize();
				}
			}
			if (m_twistSpan >= 0f)
			{
				IndexedVector3 vTwistAxis;
				ComputeTwistLimitInfo(ref qTwist, out m_twistAngle, out vTwistAxis);
				if (m_twistAngle > m_twistSpan * m_limitSoftness)
				{
					m_solveTwistLimit = true;
					m_twistLimitRatio = 1f;
					if (m_twistAngle < m_twistSpan && m_limitSoftness < 0.9999999f)
					{
						m_twistLimitRatio = (m_twistAngle - m_twistSpan * m_limitSoftness) / (m_twistSpan - m_twistSpan * m_limitSoftness);
					}
					m_twistCorrection = m_twistAngle - m_twistSpan * m_limitSoftness;
					m_twistAxis = MathUtil.QuatRotate(indexedQuaternion2, -vTwistAxis);
					m_kTwist = 1f / (ComputeAngularImpulseDenominator(ref m_twistAxis, ref invInertiaWorldA) + ComputeAngularImpulseDenominator(ref m_twistAxis, ref invInertiaWorldB));
				}
				if (m_solveSwingLimit)
				{
					m_twistAxisA = MathUtil.QuatRotate(indexedQuaternion, -vTwistAxis);
				}
			}
			else
			{
				m_twistAngle = 0f;
			}
		}

		public float GetSwingSpan1()
		{
			return m_swingSpan1;
		}

		public float GetSwingSpan2()
		{
			return m_swingSpan2;
		}

		public float GetTwistSpan()
		{
			return m_twistSpan;
		}

		public float GetTwistAngle()
		{
			return m_twistAngle;
		}

		public bool IsPastSwingLimit()
		{
			return m_solveSwingLimit;
		}

		public void SetDamping(float damping)
		{
			m_damping = damping;
		}

		public void EnableMotor(bool b)
		{
			m_bMotorEnabled = b;
		}

		public void SetMaxMotorImpulse(float maxMotorImpulse)
		{
			m_maxMotorImpulse = maxMotorImpulse;
			m_bNormalizedMotorStrength = false;
		}

		public void SetMaxMotorImpulseNormalized(float maxMotorImpulse)
		{
			m_maxMotorImpulse = maxMotorImpulse;
			m_bNormalizedMotorStrength = true;
		}

		public float GetFixThresh()
		{
			return m_fixThresh;
		}

		public void SetFixThresh(float fixThresh)
		{
			m_fixThresh = fixThresh;
		}

		public void SetMotorTarget(ref IndexedQuaternion q)
		{
			IndexedMatrix centerOfMassTransform = m_rbA.GetCenterOfMassTransform();
			IndexedMatrix centerOfMassTransform2 = m_rbB.GetCenterOfMassTransform();
			(centerOfMassTransform2.Inverse() * centerOfMassTransform).GetRotation();
			((centerOfMassTransform2 * m_rbBFrame).Inverse() * (centerOfMassTransform * m_rbAFrame)).GetRotation();
			IndexedQuaternion q2 = MathUtil.QuaternionInverse(m_rbBFrame.GetRotation()) * q * m_rbAFrame.GetRotation();
			SetMotorTargetInConstraintSpace(ref q2);
		}

		public void SetMotorTargetInConstraintSpace(ref IndexedQuaternion q)
		{
			m_qTarget = q;
			float num = 1f;
			IndexedVector3 axisInB = MathUtil.QuatRotate(ref m_qTarget, ref vTwist);
			IndexedQuaternion qCone = MathUtil.ShortestArcQuat(ref vTwist, ref axisInB);
			qCone.Normalize();
			IndexedQuaternion qTwist = MathUtil.QuaternionMultiply(MathUtil.QuaternionInverse(qCone), m_qTarget);
			qTwist.Normalize();
			if (m_swingSpan1 >= 0.05f && m_swingSpan2 >= 0.05f)
			{
				float swingAngle = 0f;
				float swingLimit = 0f;
				IndexedVector3 vSwingAxis = IndexedVector3.Zero;
				ComputeConeLimitInfo(ref qCone, ref swingAngle, ref vSwingAxis, ref swingLimit);
				if (Math.Abs(swingAngle) > 1.1920929E-07f)
				{
					if (swingAngle > swingLimit * num)
					{
						swingAngle = swingLimit * num;
					}
					else if (swingAngle < (0f - swingLimit) * num)
					{
						swingAngle = (0f - swingLimit) * num;
					}
					qCone = new IndexedQuaternion(vSwingAxis, swingAngle);
				}
			}
			if (m_twistSpan >= 0.05f)
			{
				float twistAngle;
				IndexedVector3 vTwistAxis;
				ComputeTwistLimitInfo(ref qTwist, out twistAngle, out vTwistAxis);
				if (Math.Abs(twistAngle) > 1.1920929E-07f)
				{
					if (twistAngle > m_twistSpan * num)
					{
						twistAngle = m_twistSpan * num;
					}
					else if (twistAngle < (0f - m_twistSpan) * num)
					{
						twistAngle = (0f - m_twistSpan) * num;
					}
					qTwist = new IndexedQuaternion(vTwistAxis, twistAngle);
				}
			}
			m_qTarget = qCone * qTwist;
		}

		public IndexedVector3 GetPointForAngle(float fAngleInRadians, float fLength)
		{
			float num = (float)Math.Cos(fAngleInRadians);
			float num2 = (float)Math.Sin(fAngleInRadians);
			float angle = m_swingSpan1;
			if (Math.Abs(num) > 1.1920929E-07f)
			{
				float num3 = num2 * num2 / (num * num);
				float num4 = 1f / (m_swingSpan2 * m_swingSpan2);
				num4 += num3 / (m_swingSpan1 * m_swingSpan1);
				float num5 = (1f + num3) / num4;
				angle = (float)Math.Sqrt(num5);
			}
			IndexedVector3 axis = new IndexedVector3(0f, num, 0f - num2);
			IndexedQuaternion rotation = new IndexedQuaternion(axis, angle);
			IndexedVector3 v = new IndexedVector3(fLength, 0f, 0f);
			return MathUtil.QuatRotate(ref rotation, ref v);
		}

		protected void ComputeConeLimitInfo(ref IndexedQuaternion qCone, ref float swingAngle, ref IndexedVector3 vSwingAxis, ref float swingLimit)
		{
			swingAngle = MathUtil.QuatAngle(ref qCone);
			if (swingAngle > 1.1920929E-07f)
			{
				vSwingAxis = new IndexedVector3(qCone.X, qCone.Y, qCone.Z);
				vSwingAxis.Normalize();
				Math.Abs(vSwingAxis.X);
				float num5 = 1.1920929E-07f;
				float y = vSwingAxis.Y;
				float num = 0f - vSwingAxis.Z;
				swingLimit = m_swingSpan1;
				if (Math.Abs(y) > 1.1920929E-07f)
				{
					float num2 = num * num / (y * y);
					float num3 = 1f / (m_swingSpan2 * m_swingSpan2);
					num3 += num2 / (m_swingSpan1 * m_swingSpan1);
					float num4 = (1f + num2) / num3;
					swingLimit = (float)Math.Sqrt(num4);
				}
			}
			else
			{
				float num6 = swingAngle;
				float num7 = 0f;
			}
		}

		protected void ComputeTwistLimitInfo(ref IndexedQuaternion qTwist, out float twistAngle, out IndexedVector3 vTwistAxis)
		{
			IndexedQuaternion indexedQuaternion = qTwist;
			twistAngle = MathUtil.QuatAngle(ref qTwist);
			if (twistAngle > (float)Math.PI)
			{
				indexedQuaternion = -qTwist;
				twistAngle = MathUtil.QuatAngle(ref qTwist);
			}
			float num = twistAngle;
			float num2 = 0f;
			vTwistAxis = new IndexedVector3(indexedQuaternion.X, indexedQuaternion.Y, indexedQuaternion.Z);
			if (twistAngle > 1.1920929E-07f)
			{
				vTwistAxis.Normalize();
			}
		}

		protected void AdjustSwingAxisToUseEllipseNormal(ref IndexedVector3 vSwingAxis)
		{
			float num = 0f - vSwingAxis.Z;
			float y = vSwingAxis.Y;
			if (Math.Abs(y) > 1.1920929E-07f)
			{
				float num2 = num / y;
				num2 *= m_swingSpan2 / m_swingSpan1;
				num = ((!(num > 0f)) ? (0f - Math.Abs(num2 * y)) : Math.Abs(num2 * y));
				vSwingAxis.Z = 0f - num;
				vSwingAxis.Y = y;
				vSwingAxis.Normalize();
			}
		}

		public IndexedMatrix GetFrameOffsetA()
		{
			return m_rbAFrame;
		}

		public IndexedMatrix GetFrameOffsetB()
		{
			return m_rbBFrame;
		}

		public void SetFrames(ref IndexedMatrix frameA, ref IndexedMatrix frameB)
		{
			m_rbAFrame = frameA;
			m_rbBFrame = frameB;
			BuildJacobian();
		}

		public override void BuildJacobian()
		{
			if (!m_useSolveConstraintObsolete)
			{
				return;
			}
			m_appliedImpulse = 0f;
			m_accTwistLimitImpulse = 0f;
			m_accSwingLimitImpulse = 0f;
			m_accMotorImpulse = IndexedVector3.Zero;
			if (!m_angularOnly)
			{
				IndexedVector3 indexedVector = m_rbA.GetCenterOfMassTransform() * m_rbAFrame._origin;
				IndexedVector3 indexedVector2 = m_rbB.GetCenterOfMassTransform() * m_rbBFrame._origin;
				IndexedVector3 indexedVector3 = indexedVector2 - indexedVector;
				IndexedVector3[] array = new IndexedVector3[3];
				if (indexedVector3.LengthSquared() > 1.1920929E-07f)
				{
					array[0] = indexedVector3.Normalized();
				}
				else
				{
					array[0] = new IndexedVector3(1f, 0f, 0f);
				}
				TransformUtil.PlaneSpace1(ref array[0], out array[1], out array[2]);
				for (int i = 0; i < 3; i++)
				{
					m_jac[i] = new JacobianEntry(m_rbA.GetCenterOfMassTransform()._basis.Transpose(), m_rbB.GetCenterOfMassTransform()._basis.Transpose(), indexedVector - m_rbA.GetCenterOfMassPosition(), indexedVector2 - m_rbB.GetCenterOfMassPosition(), array[i], m_rbA.GetInvInertiaDiagLocal(), m_rbA.GetInvMass(), m_rbB.GetInvInertiaDiagLocal(), m_rbB.GetInvMass());
				}
			}
			CalcAngleInfo2(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetInvInertiaTensorWorld(), m_rbB.GetInvInertiaTensorWorld());
		}

		public void solveConstraintObsolete(RigidBody bodyA, RigidBody bodyB, float timeStep)
		{
			if (!m_useSolveConstraintObsolete)
			{
				return;
			}
			IndexedVector3 indexedVector = m_rbA.GetCenterOfMassTransform() * m_rbAFrame._origin;
			IndexedVector3 indexedVector2 = m_rbB.GetCenterOfMassTransform() * m_rbBFrame._origin;
			float num = 0.3f;
			if (!m_angularOnly)
			{
				IndexedVector3 rel_pos = indexedVector - m_rbA.GetCenterOfMassPosition();
				IndexedVector3 rel_pos2 = indexedVector2 - m_rbB.GetCenterOfMassPosition();
				IndexedVector3 velocity = IndexedVector3.Zero;
				bodyA.InternalGetVelocityInLocalPointObsolete(ref rel_pos, ref velocity);
				IndexedVector3 velocity2 = IndexedVector3.Zero;
				bodyB.InternalGetVelocityInLocalPointObsolete(ref rel_pos2, ref velocity2);
				IndexedVector3 v = velocity - velocity2;
				for (int i = 0; i < 3; i++)
				{
					IndexedVector3 v2 = m_jac[i].m_linearJointAxis;
					float num2 = 1f / m_jac[i].GetDiagonal();
					float num3 = v2.Dot(ref v);
					float num4 = 0f - (indexedVector - indexedVector2).Dot(ref v2);
					float num5 = num4 * num / timeStep * num2 - num3 * num2;
					m_appliedImpulse += num5;
					IndexedVector3 indexedVector3 = rel_pos.Cross(ref v2);
					IndexedVector3 indexedVector4 = rel_pos2.Cross(ref v2);
					bodyA.InternalApplyImpulse(v2 * m_rbA.GetInvMass(), m_rbA.GetInvInertiaTensorWorld() * indexedVector3, num5, null);
					bodyB.InternalApplyImpulse(v2 * m_rbB.GetInvMass(), m_rbB.GetInvInertiaTensorWorld() * indexedVector4, 0f - num5, null);
				}
			}
			if (m_bMotorEnabled)
			{
				IndexedMatrix curTrans = m_rbA.GetCenterOfMassTransform();
				IndexedMatrix curTrans2 = m_rbB.GetCenterOfMassTransform();
				IndexedVector3 angVel = IndexedVector3.Zero;
				bodyA.InternalGetAngularVelocity(ref angVel);
				IndexedVector3 angVel2 = IndexedVector3.Zero;
				bodyB.InternalGetAngularVelocity(ref angVel2);
				IndexedVector3 linvel = new IndexedVector3(0f, 0f, 0f);
				IndexedMatrix predictedTransform;
				TransformUtil.IntegrateTransform(ref curTrans, ref linvel, ref angVel, timeStep, out predictedTransform);
				IndexedMatrix predictedTransform2;
				TransformUtil.IntegrateTransform(ref curTrans2, ref linvel, ref angVel2, timeStep, out predictedTransform2);
				IndexedMatrix indexedMatrix = IndexedMatrix.CreateFromQuaternion(m_qTarget);
				IndexedMatrix indexedMatrix2 = m_rbBFrame * indexedMatrix * m_rbAFrame.Inverse();
				IndexedMatrix transform = predictedTransform2 * indexedMatrix2;
				IndexedMatrix transform2 = predictedTransform * indexedMatrix2.Inverse();
				IndexedVector3 angVel3;
				TransformUtil.CalculateVelocity(ref curTrans, ref transform, timeStep, out linvel, out angVel3);
				IndexedVector3 angVel4;
				TransformUtil.CalculateVelocity(ref curTrans2, ref transform2, timeStep, out linvel, out angVel4);
				IndexedVector3 indexedVector5 = angVel3 - angVel;
				IndexedVector3 indexedVector6 = angVel4 - angVel2;
				IndexedVector3 axis = IndexedVector3.Zero;
				IndexedVector3 axis2 = IndexedVector3.Zero;
				float num6 = 0f;
				float num7 = 0f;
				if (indexedVector5.LengthSquared() > 1.1920929E-07f)
				{
					axis = indexedVector5.Normalized();
					num6 = GetRigidBodyA().ComputeAngularImpulseDenominator(ref axis);
				}
				if (indexedVector6.LengthSquared() > 1.1920929E-07f)
				{
					axis2 = indexedVector6.Normalized();
					num7 = GetRigidBodyB().ComputeAngularImpulseDenominator(ref axis2);
				}
				IndexedVector3 axis3 = num6 * axis + num7 * axis2;
				if (bDoTorque && axis3.LengthSquared() > 1.1920929E-07f)
				{
					axis3.Normalize();
					num6 = GetRigidBodyA().ComputeAngularImpulseDenominator(ref axis3);
					num7 = GetRigidBodyB().ComputeAngularImpulseDenominator(ref axis3);
					float num8 = num6 + num7;
					IndexedVector3 indexedVector7 = (num6 * indexedVector5 - num7 * indexedVector6) / (num8 * num8);
					if (m_maxMotorImpulse >= 0f)
					{
						float num9 = m_maxMotorImpulse;
						if (m_bNormalizedMotorStrength)
						{
							num9 /= num6;
						}
						IndexedVector3 indexedVector8 = m_accMotorImpulse + indexedVector7;
						float num10 = indexedVector8.Length();
						if (num10 > num9)
						{
							indexedVector8.Normalize();
							indexedVector8 *= num9;
							indexedVector7 = indexedVector8 - m_accMotorImpulse;
						}
						m_accMotorImpulse += indexedVector7;
					}
					float num11 = indexedVector7.Length();
					IndexedVector3 indexedVector9 = indexedVector7 / num11;
					bodyA.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbA.GetInvInertiaTensorWorld() * indexedVector9, num11, null);
					bodyB.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbB.GetInvInertiaTensorWorld() * indexedVector9, 0f - num11, null);
				}
			}
			else if (m_damping > 1.1920929E-07f)
			{
				IndexedVector3 angVel5 = IndexedVector3.Zero;
				bodyA.InternalGetAngularVelocity(ref angVel5);
				IndexedVector3 angVel6 = IndexedVector3.Zero;
				bodyB.InternalGetAngularVelocity(ref angVel6);
				IndexedVector3 indexedVector10 = angVel6 - angVel5;
				if (indexedVector10.LengthSquared() > 1.1920929E-07f)
				{
					IndexedVector3 axis4 = indexedVector10.Normalized();
					float num12 = 1f / (GetRigidBodyA().ComputeAngularImpulseDenominator(ref axis4) + GetRigidBodyB().ComputeAngularImpulseDenominator(ref axis4));
					IndexedVector3 indexedVector11 = m_damping * num12 * indexedVector10;
					float num13 = indexedVector11.Length();
					IndexedVector3 indexedVector12 = indexedVector11 / num13;
					bodyA.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbA.GetInvInertiaTensorWorld() * indexedVector12, num13, null);
					bodyB.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbB.GetInvInertiaTensorWorld() * indexedVector12, 0f - num13, null);
				}
			}
			IndexedVector3 angVel7 = IndexedVector3.Zero;
			bodyA.InternalGetAngularVelocity(ref angVel7);
			IndexedVector3 angVel8 = IndexedVector3.Zero;
			bodyB.InternalGetAngularVelocity(ref angVel8);
			if (m_solveSwingLimit)
			{
				float num14 = m_swingLimitRatio * m_swingCorrection * m_biasFactor / timeStep;
				float num15 = (angVel8 - angVel7).Dot(ref m_swingAxis);
				if (num15 > 0f)
				{
					num14 += m_swingLimitRatio * num15 * m_relaxationFactor;
				}
				float num16 = num14 * m_kSwing;
				float accSwingLimitImpulse = m_accSwingLimitImpulse;
				m_accSwingLimitImpulse = Math.Max(m_accSwingLimitImpulse + num16, 0f);
				num16 = m_accSwingLimitImpulse - accSwingLimitImpulse;
				IndexedVector3 indexedVector13 = m_swingAxis * num16;
				IndexedVector3 indexedVector14 = indexedVector13.Dot(ref m_twistAxisA) * m_twistAxisA;
				IndexedVector3 indexedVector15 = indexedVector13 - indexedVector14;
				indexedVector13 = indexedVector15;
				num16 = indexedVector13.Length();
				IndexedVector3 indexedVector16 = indexedVector13 / num16;
				bodyA.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbA.GetInvInertiaTensorWorld() * indexedVector16, num16, null);
				bodyB.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbB.GetInvInertiaTensorWorld() * indexedVector16, 0f - num16, null);
			}
			if (m_solveTwistLimit)
			{
				float num17 = m_twistLimitRatio * m_twistCorrection * m_biasFactor / timeStep;
				float num18 = (angVel8 - angVel7).Dot(ref m_twistAxis);
				if (num18 > 0f)
				{
					num17 += m_twistLimitRatio * num18 * m_relaxationFactor;
				}
				float num19 = num17 * m_kTwist;
				float accTwistLimitImpulse = m_accTwistLimitImpulse;
				m_accTwistLimitImpulse = Math.Max(m_accTwistLimitImpulse + num19, 0f);
				num19 = m_accTwistLimitImpulse - accTwistLimitImpulse;
				IndexedVector3 indexedVector17 = m_twistAxis * num19;
				bodyA.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbA.GetInvInertiaTensorWorld() * m_twistAxis, num19, null);
				bodyB.InternalApplyImpulse(new IndexedVector3(0f, 0f, 0f), m_rbB.GetInvInertiaTensorWorld() * m_twistAxis, 0f - num19, null);
			}
		}
	}
}
