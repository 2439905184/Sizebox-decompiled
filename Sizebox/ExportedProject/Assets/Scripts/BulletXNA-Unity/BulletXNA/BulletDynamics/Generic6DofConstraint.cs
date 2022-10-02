using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class Generic6DofConstraint : TypedConstraint
	{
		public const int BT_6DOF_FLAGS_AXIS_SHIFT = 3;

		public const bool D6_USE_FRAME_OFFSET = true;

		protected IndexedMatrix m_frameInA = IndexedMatrix.Identity;

		protected IndexedMatrix m_frameInB = IndexedMatrix.Identity;

		protected JacobianEntry[] m_jacLinear = new JacobianEntry[3];

		protected JacobianEntry[] m_jacAng = new JacobianEntry[3];

		protected TranslationalLimitMotor m_linearLimits;

		protected RotationalLimitMotor[] m_angularLimits = new RotationalLimitMotor[3];

		protected float m_timeStep;

		protected IndexedMatrix m_calculatedTransformA = IndexedMatrix.Identity;

		protected IndexedMatrix m_calculatedTransformB = IndexedMatrix.Identity;

		protected IndexedVector3 m_calculatedAxisAngleDiff;

		protected IndexedVector3[] m_calculatedAxis = new IndexedVector3[3];

		protected IndexedVector3 m_calculatedLinearDiff;

		protected float m_factA;

		protected float m_factB;

		protected bool m_hasStaticBody;

		protected IndexedVector3 m_AnchorPos;

		protected bool m_useLinearReferenceFrameA;

		protected bool m_useOffsetForConstraintFrame;

		protected int m_flags;

		public Generic6DofConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix frameInA, ref IndexedMatrix frameInB, bool useLinearReferenceFrameA)
			: base(TypedConstraintType.D6_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_frameInA = frameInA;
			m_frameInB = frameInB;
			m_useLinearReferenceFrameA = useLinearReferenceFrameA;
			m_useOffsetForConstraintFrame = true;
			m_linearLimits = new TranslationalLimitMotor();
			m_angularLimits[0] = new RotationalLimitMotor();
			m_angularLimits[1] = new RotationalLimitMotor();
			m_angularLimits[2] = new RotationalLimitMotor();
			CalculateTransforms();
		}

		public Generic6DofConstraint(RigidBody rbB, ref IndexedMatrix frameInB, bool useLinearReferenceFrameB)
			: base(TypedConstraintType.D6_CONSTRAINT_TYPE, TypedConstraint.GetFixedBody(), rbB)
		{
			m_frameInB = frameInB;
			m_useLinearReferenceFrameA = useLinearReferenceFrameB;
			m_useOffsetForConstraintFrame = true;
			m_linearLimits = new TranslationalLimitMotor();
			m_angularLimits[0] = new RotationalLimitMotor();
			m_angularLimits[1] = new RotationalLimitMotor();
			m_angularLimits[2] = new RotationalLimitMotor();
			m_frameInA = rbB.GetCenterOfMassTransform() * m_frameInB;
			CalculateTransforms();
		}

		protected virtual int SetAngularLimits(ConstraintInfo2 info, int row_offset, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 linVelA, ref IndexedVector3 linVelB, ref IndexedVector3 angVelA, ref IndexedVector3 angVelB)
		{
			int num = row_offset;
			for (int i = 0; i < 3; i++)
			{
				if (GetRotationalLimitMotor(i).NeedApplyTorques())
				{
					IndexedVector3 ax = GetAxis(i);
					int num2 = m_flags >> (i + 3) * 3;
					SixDofFlags sixDofFlags = (SixDofFlags)num2;
					if ((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_CFM_NORM) == 0)
					{
						m_angularLimits[i].m_normalCFM = info.m_solverConstraints[0].m_cfm;
					}
					if ((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_CFM_STOP) == 0)
					{
						m_angularLimits[i].m_stopCFM = info.m_solverConstraints[0].m_cfm;
					}
					if ((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_ERP_STOP) == 0)
					{
						m_angularLimits[i].m_stopERP = info.erp;
					}
					num += GetLimitMotorInfo2(GetRotationalLimitMotor(i), ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB, info, num, ref ax, 1, false);
				}
			}
			return num;
		}

		protected virtual int SetLinearLimits(ConstraintInfo2 info, int row, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 linVelA, ref IndexedVector3 linVelB, ref IndexedVector3 angVelA, ref IndexedVector3 angVelB)
		{
			RotationalLimitMotor rotationalLimitMotor = new RotationalLimitMotor();
			for (int i = 0; i < 3; i++)
			{
				if (!m_linearLimits.NeedApplyForce(i))
				{
					continue;
				}
				rotationalLimitMotor.m_bounce = 0f;
				rotationalLimitMotor.m_currentLimit = m_linearLimits.m_currentLimit[i];
				rotationalLimitMotor.m_currentPosition = m_linearLimits.m_currentLinearDiff[i];
				rotationalLimitMotor.m_currentLimitError = m_linearLimits.m_currentLimitError[i];
				rotationalLimitMotor.m_damping = m_linearLimits.m_damping;
				rotationalLimitMotor.m_enableMotor = m_linearLimits.m_enableMotor[i];
				rotationalLimitMotor.m_hiLimit = m_linearLimits.m_upperLimit[i];
				rotationalLimitMotor.m_limitSoftness = m_linearLimits.m_limitSoftness;
				rotationalLimitMotor.m_loLimit = m_linearLimits.m_lowerLimit[i];
				rotationalLimitMotor.m_maxLimitForce = 0f;
				rotationalLimitMotor.m_maxMotorForce = m_linearLimits.m_maxMotorForce[i];
				rotationalLimitMotor.m_targetVelocity = m_linearLimits.m_targetVelocity[i];
				IndexedVector3 ax = m_calculatedTransformA._basis.GetColumn(i);
				int num = m_flags >> i * 3;
				SixDofFlags sixDofFlags = (SixDofFlags)num;
				rotationalLimitMotor.m_normalCFM = (((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_CFM_NORM) != 0) ? m_linearLimits.m_normalCFM[i] : info.m_solverConstraints[0].m_cfm);
				rotationalLimitMotor.m_stopCFM = (((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_CFM_STOP) != 0) ? m_linearLimits.m_stopCFM[i] : info.m_solverConstraints[0].m_cfm);
				rotationalLimitMotor.m_stopERP = (((sixDofFlags & SixDofFlags.BT_6DOF_FLAGS_ERP_STOP) != 0) ? m_linearLimits.m_stopERP[i] : info.erp);
				if (m_useOffsetForConstraintFrame)
				{
					int num2 = (i + 1) % 3;
					int num3 = (i + 2) % 3;
					bool rotAllowed = true;
					if (m_angularLimits[num2].m_currentLimit != 0 && m_angularLimits[num3].m_currentLimit != 0)
					{
						rotAllowed = false;
					}
					row += GetLimitMotorInfo2(rotationalLimitMotor, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB, info, row, ref ax, 0, rotAllowed);
				}
				else
				{
					row += GetLimitMotorInfo2(rotationalLimitMotor, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB, info, row, ref ax, 0, false);
				}
			}
			return row;
		}

		protected virtual void BuildLinearJacobian(out JacobianEntry jacLinear, ref IndexedVector3 normalWorld, ref IndexedVector3 pivotAInW, ref IndexedVector3 pivotBInW)
		{
			jacLinear = new JacobianEntry(m_rbA.GetCenterOfMassTransform()._basis.Transpose(), m_rbB.GetCenterOfMassTransform()._basis.Transpose(), pivotAInW - m_rbA.GetCenterOfMassPosition(), pivotBInW - m_rbB.GetCenterOfMassPosition(), normalWorld, m_rbA.GetInvInertiaDiagLocal(), m_rbA.GetInvMass(), m_rbB.GetInvInertiaDiagLocal(), m_rbB.GetInvMass());
		}

		protected virtual void BuildAngularJacobian(out JacobianEntry jacAngular, ref IndexedVector3 jointAxisW)
		{
			jacAngular = new JacobianEntry(jointAxisW, m_rbA.GetCenterOfMassTransform()._basis.Transpose(), m_rbB.GetCenterOfMassTransform()._basis.Transpose(), m_rbA.GetInvInertiaDiagLocal(), m_rbB.GetInvInertiaDiagLocal());
		}

		protected virtual void CalculateLinearInfo()
		{
			m_calculatedLinearDiff = m_calculatedTransformB._origin - m_calculatedTransformA._origin;
			m_calculatedLinearDiff = m_calculatedTransformA._basis.Inverse() * m_calculatedLinearDiff;
			m_linearLimits.m_currentLinearDiff = m_calculatedLinearDiff;
			m_linearLimits.TestLimitValue(0, m_calculatedLinearDiff.X);
			m_linearLimits.TestLimitValue(1, m_calculatedLinearDiff.Y);
			m_linearLimits.TestLimitValue(2, m_calculatedLinearDiff.Z);
		}

		protected virtual void CalculateAngleInfo()
		{
			IndexedBasisMatrix mat = m_calculatedTransformA._basis.Inverse() * m_calculatedTransformB._basis;
			MathUtil.MatrixToEulerXYZ(ref mat, out m_calculatedAxisAngleDiff);
			IndexedVector3 v = m_calculatedTransformB._basis.GetColumn(0);
			IndexedVector3 v2 = m_calculatedTransformA._basis.GetColumn(2);
			m_calculatedAxis[1] = v2.Cross(ref v);
			m_calculatedAxis[0] = m_calculatedAxis[1].Cross(ref v2);
			m_calculatedAxis[2] = v.Cross(ref m_calculatedAxis[1]);
			m_calculatedAxis[0].Normalize();
			m_calculatedAxis[1].Normalize();
			m_calculatedAxis[2].Normalize();
		}

		public virtual void CalculateTransforms()
		{
			CalculateTransforms(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
		}

		public virtual void CalculateTransforms(IndexedMatrix transA, IndexedMatrix transB)
		{
			CalculateTransforms(ref transA, ref transB);
		}

		public virtual void CalculateTransforms(ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			m_calculatedTransformA = transA * m_frameInA;
			m_calculatedTransformB = transB * m_frameInB;
			CalculateLinearInfo();
			CalculateAngleInfo();
			if (m_useOffsetForConstraintFrame)
			{
				float invMass = GetRigidBodyA().GetInvMass();
				float invMass2 = GetRigidBodyB().GetInvMass();
				m_hasStaticBody = invMass < 1.1920929E-07f || invMass2 < 1.1920929E-07f;
				float num = invMass + invMass2;
				if (num > 0f)
				{
					m_factA = invMass2 / num;
				}
				else
				{
					m_factA = 0.5f;
				}
				m_factB = 1f - m_factA;
			}
		}

		public IndexedMatrix GetCalculatedTransformA()
		{
			return m_calculatedTransformA;
		}

		public IndexedMatrix GetCalculatedTransformB()
		{
			return m_calculatedTransformB;
		}

		public IndexedMatrix GetFrameOffsetA()
		{
			return m_frameInA;
		}

		public IndexedMatrix GetFrameOffsetB()
		{
			return m_frameInB;
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
			CalculateTransforms(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
			info.m_numConstraintRows = 0;
			info.nub = 6;
			for (int i = 0; i < 3; i++)
			{
				if (m_linearLimits.NeedApplyForce(i))
				{
					info.m_numConstraintRows++;
					info.nub--;
				}
			}
			for (int i = 0; i < 3; i++)
			{
				if (TestAngularLimitMotor(i))
				{
					info.m_numConstraintRows++;
					info.nub--;
				}
			}
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 6;
			info.nub = 0;
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
			IndexedMatrix transA = m_rbA.GetCenterOfMassTransform();
			IndexedMatrix transB = m_rbB.GetCenterOfMassTransform();
			IndexedVector3 linVelA = m_rbA.GetLinearVelocity();
			IndexedVector3 linVelB = m_rbB.GetLinearVelocity();
			IndexedVector3 angVelA = m_rbA.GetAngularVelocity();
			IndexedVector3 angVelB = m_rbB.GetAngularVelocity();
			if (m_useOffsetForConstraintFrame)
			{
				int row = SetAngularLimits(info, 0, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
				SetLinearLimits(info, row, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
			}
			else
			{
				int row_offset = SetLinearLimits(info, 0, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
				SetAngularLimits(info, row_offset, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
			}
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedVector3 linVelA, IndexedVector3 linVelB, IndexedVector3 angVelA, IndexedVector3 angVelB)
		{
			CalculateTransforms(ref transA, ref transB);
			for (int i = 0; i < 3; i++)
			{
				TestAngularLimitMotor(i);
			}
			if (m_useOffsetForConstraintFrame)
			{
				int row = SetAngularLimits(info, 0, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
				SetLinearLimits(info, row, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
			}
			else
			{
				int row_offset = SetLinearLimits(info, 0, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
				SetAngularLimits(info, row_offset, ref transA, ref transB, ref linVelA, ref linVelB, ref angVelA, ref angVelB);
			}
		}

		public virtual void UpdateRHS(float timeStep)
		{
		}

		public virtual IndexedVector3 GetAxis(int axis_index)
		{
			return m_calculatedAxis[axis_index];
		}

		public virtual float GetAngle(int axis_index)
		{
			return m_calculatedAxisAngleDiff[axis_index];
		}

		public float GetRelativePivotPosition(int axisIndex)
		{
			return m_calculatedLinearDiff[axisIndex];
		}

		public virtual bool TestAngularLimitMotor(int axis_index)
		{
			float angleInRadians = m_calculatedAxisAngleDiff[axis_index];
			angleInRadians = AdjustAngleToLimits(angleInRadians, m_angularLimits[axis_index].m_loLimit, m_angularLimits[axis_index].m_hiLimit);
			m_angularLimits[axis_index].m_currentPosition = angleInRadians;
			m_angularLimits[axis_index].TestLimitValue(angleInRadians);
			return m_angularLimits[axis_index].NeedApplyTorques();
		}

		public void SetLinearLowerLimit(IndexedVector3 linearLower)
		{
			SetLinearLowerLimit(ref linearLower);
		}

		public void SetLinearLowerLimit(ref IndexedVector3 linearLower)
		{
			m_linearLimits.m_lowerLimit = linearLower;
		}

		public void SetLinearUpperLimit(IndexedVector3 linearUpper)
		{
			SetLinearUpperLimit(ref linearUpper);
		}

		public void SetLinearUpperLimit(ref IndexedVector3 linearUpper)
		{
			m_linearLimits.m_upperLimit = linearUpper;
		}

		public void SetAngularLowerLimit(IndexedVector3 angularLower)
		{
			SetAngularLowerLimit(ref angularLower);
		}

		public void SetAngularLowerLimit(ref IndexedVector3 angularLower)
		{
			m_angularLimits[0].m_loLimit = MathUtil.NormalizeAngle(angularLower.X);
			m_angularLimits[1].m_loLimit = MathUtil.NormalizeAngle(angularLower.Y);
			m_angularLimits[2].m_loLimit = MathUtil.NormalizeAngle(angularLower.Z);
		}

		public void SetAngularUpperLimit(IndexedVector3 angularUpper)
		{
			SetAngularUpperLimit(ref angularUpper);
		}

		public void SetAngularUpperLimit(ref IndexedVector3 angularUpper)
		{
			m_angularLimits[0].m_hiLimit = MathUtil.NormalizeAngle(angularUpper.X);
			m_angularLimits[1].m_hiLimit = MathUtil.NormalizeAngle(angularUpper.Y);
			m_angularLimits[2].m_hiLimit = MathUtil.NormalizeAngle(angularUpper.Z);
		}

		public RotationalLimitMotor GetRotationalLimitMotor(int index)
		{
			return m_angularLimits[index];
		}

		public TranslationalLimitMotor GetTranslationalLimitMotor()
		{
			return m_linearLimits;
		}

		public void SetLimit(int axis, float lo, float hi)
		{
			if (axis < 3)
			{
				m_linearLimits.m_lowerLimit[axis] = lo;
				m_linearLimits.m_upperLimit[axis] = hi;
				return;
			}
			lo = MathUtil.NormalizeAngle(lo);
			hi = MathUtil.NormalizeAngle(hi);
			m_linearLimits.m_lowerLimit[axis - 3] = lo;
			m_linearLimits.m_upperLimit[axis - 3] = hi;
		}

		public bool IsLimited(int limitIndex)
		{
			if (limitIndex < 3)
			{
				return m_linearLimits.IsLimited(limitIndex);
			}
			return m_angularLimits[limitIndex - 3].IsLimited();
		}

		public virtual void CalcAnchorPos()
		{
			float invMass = m_rbA.GetInvMass();
			float invMass2 = m_rbB.GetInvMass();
			float num = ((!MathUtil.FuzzyZero(invMass2)) ? (invMass / (invMass + invMass2)) : 1f);
			IndexedVector3 origin = m_calculatedTransformA._origin;
			IndexedVector3 origin2 = m_calculatedTransformB._origin;
			m_AnchorPos = origin * num + origin2 * (1f - num);
		}

		public virtual int GetLimitMotorInfo2(RotationalLimitMotor limot, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 linVelA, ref IndexedVector3 linVelB, ref IndexedVector3 angVelA, ref IndexedVector3 angVelB, ConstraintInfo2 info, int row, ref IndexedVector3 ax1, int rotational, bool rotAllowed)
		{
			bool flag = limot.m_enableMotor;
			int currentLimit = limot.m_currentLimit;
			if (flag || currentLimit != 0)
			{
				if (rotational != 0)
				{
					info.m_solverConstraints[row].m_relpos1CrossNormal = ax1;
				}
				else
				{
					info.m_solverConstraints[row].m_contactNormal = ax1;
				}
				if (rotational != 0)
				{
					info.m_solverConstraints[row].m_relpos2CrossNormal = -ax1;
				}
				if (rotational == 0)
				{
					if (m_useOffsetForConstraintFrame)
					{
						IndexedVector3 zero = IndexedVector3.Zero;
						IndexedVector3 zero2 = IndexedVector3.Zero;
						IndexedVector3 zero3 = IndexedVector3.Zero;
						IndexedVector3 zero4 = IndexedVector3.Zero;
						zero4 = m_calculatedTransformB._origin - transB._origin;
						IndexedVector3 indexedVector = ax1 * IndexedVector3.Dot(zero4, ax1);
						IndexedVector3 indexedVector2 = zero4 - indexedVector;
						zero3 = m_calculatedTransformA._origin - transA._origin;
						IndexedVector3 indexedVector3 = ax1 * IndexedVector3.Dot(zero3, ax1);
						IndexedVector3 indexedVector4 = zero3 - indexedVector3;
						float num = limot.m_currentPosition - limot.m_currentLimitError;
						IndexedVector3 indexedVector5 = indexedVector3 + ax1 * num - indexedVector;
						zero3 = indexedVector4 + indexedVector5 * m_factA;
						zero4 = indexedVector2 - indexedVector5 * m_factB;
						zero = IndexedVector3.Cross(zero3, ax1);
						zero2 = IndexedVector3.Cross(zero4, ax1);
						if (m_hasStaticBody && !rotAllowed)
						{
							zero *= m_factA;
							zero2 *= m_factB;
						}
						info.m_solverConstraints[row].m_relpos1CrossNormal = zero;
						info.m_solverConstraints[row].m_relpos2CrossNormal = -zero2;
					}
					else
					{
						IndexedVector3 v = m_calculatedTransformB._origin - transA._origin;
						IndexedVector3 relpos1CrossNormal = IndexedVector3.Cross(v, ax1);
						info.m_solverConstraints[row].m_relpos1CrossNormal = relpos1CrossNormal;
						v = m_calculatedTransformB._origin - transB._origin;
						relpos1CrossNormal = -IndexedVector3.Cross(v, ax1);
						info.m_solverConstraints[row].m_relpos2CrossNormal = relpos1CrossNormal;
					}
				}
				if (currentLimit != 0 && MathUtil.CompareFloat(limot.m_loLimit, limot.m_hiLimit))
				{
					flag = false;
				}
				info.m_solverConstraints[row].m_rhs = 0f;
				if (flag)
				{
					info.m_solverConstraints[row].m_cfm = limot.m_normalCFM;
					if (currentLimit == 0)
					{
						float vel = ((rotational != 0) ? limot.m_targetVelocity : (0f - limot.m_targetVelocity));
						float motorFactor = GetMotorFactor(limot.m_currentPosition, limot.m_loLimit, limot.m_hiLimit, vel, info.fps * limot.m_stopERP);
						info.m_solverConstraints[row].m_rhs += motorFactor * limot.m_targetVelocity;
						info.m_solverConstraints[row].m_lowerLimit = 0f - limot.m_maxMotorForce;
						info.m_solverConstraints[row].m_upperLimit = limot.m_maxMotorForce;
					}
				}
				if (currentLimit != 0)
				{
					float num2 = info.fps * limot.m_stopERP;
					if (rotational == 0)
					{
						info.m_solverConstraints[row].m_rhs += num2 * limot.m_currentLimitError;
					}
					else
					{
						info.m_solverConstraints[row].m_rhs += (0f - num2) * limot.m_currentLimitError;
					}
					info.m_solverConstraints[row].m_cfm = limot.m_stopCFM;
					if (MathUtil.CompareFloat(limot.m_loLimit, limot.m_hiLimit))
					{
						info.m_solverConstraints[row].m_lowerLimit = float.MinValue;
						info.m_solverConstraints[row].m_upperLimit = float.MaxValue;
					}
					else
					{
						if (currentLimit == 1)
						{
							info.m_solverConstraints[row].m_lowerLimit = 0f;
							info.m_solverConstraints[row].m_upperLimit = float.MaxValue;
						}
						else
						{
							info.m_solverConstraints[row].m_lowerLimit = float.MinValue;
							info.m_solverConstraints[row].m_upperLimit = 0f;
						}
						if (limot.m_bounce > 0f)
						{
							float num3;
							if (rotational != 0)
							{
								num3 = IndexedVector3.Dot(angVelA, ax1);
								num3 -= IndexedVector3.Dot(angVelB, ax1);
							}
							else
							{
								num3 = IndexedVector3.Dot(linVelA, ax1);
								num3 -= IndexedVector3.Dot(linVelB, ax1);
							}
							if (currentLimit == 1)
							{
								if (num3 < 0f)
								{
									float num4 = (0f - limot.m_bounce) * num3;
									if (num4 > info.m_solverConstraints[row].m_rhs)
									{
										info.m_solverConstraints[row].m_rhs = num4;
									}
								}
							}
							else if (num3 > 0f)
							{
								float num5 = (0f - limot.m_bounce) * num3;
								if (num5 < info.m_solverConstraints[row].m_rhs)
								{
									info.m_solverConstraints[row].m_rhs = num5;
								}
							}
						}
					}
				}
				return 1;
			}
			return 0;
		}

		public void GetLinearLowerLimit(out IndexedVector3 linearLower)
		{
			linearLower = m_linearLimits.m_lowerLimit;
		}

		public void GetLinearUpperLimit(out IndexedVector3 linearUpper)
		{
			linearUpper = m_linearLimits.m_upperLimit;
		}

		public void GetAngularLowerLimit(out IndexedVector3 angularLower)
		{
			angularLower = new IndexedVector3(m_angularLimits[0].m_loLimit, m_angularLimits[1].m_loLimit, m_angularLimits[2].m_loLimit);
		}

		public void GetAngularUpperLimit(out IndexedVector3 angularUpper)
		{
			angularUpper = new IndexedVector3(m_angularLimits[0].m_hiLimit, m_angularLimits[1].m_hiLimit, m_angularLimits[2].m_hiLimit);
		}

		public virtual void SetAxis(ref IndexedVector3 axis1, ref IndexedVector3 axis2)
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

		public bool GetUseFrameOffset()
		{
			return m_useOffsetForConstraintFrame;
		}

		public void SetUseFrameOffset(bool frameOffsetOnOff)
		{
			m_useOffsetForConstraintFrame = frameOffsetOnOff;
		}

		public void SetFrames(ref IndexedMatrix frameA, ref IndexedMatrix frameB)
		{
			m_frameInA = frameA;
			m_frameInB = frameB;
			CalculateTransforms();
		}

		public void SetParam(ConstraintParams num, float value, int axis)
		{
			if (axis >= 0 && axis < 3)
			{
				switch (num)
				{
				case ConstraintParams.BT_CONSTRAINT_STOP_ERP:
					m_linearLimits.m_stopERP[axis] = value;
					m_flags |= 4 << axis * 3;
					break;
				case ConstraintParams.BT_CONSTRAINT_STOP_CFM:
					m_linearLimits.m_stopCFM[axis] = value;
					m_flags |= 2 << axis * 3;
					break;
				case ConstraintParams.BT_CONSTRAINT_CFM:
					m_linearLimits.m_normalCFM[axis] = value;
					m_flags |= 1 << axis * 3;
					break;
				}
			}
			else if (axis >= 3 && axis < 6)
			{
				switch (num)
				{
				case ConstraintParams.BT_CONSTRAINT_STOP_ERP:
					m_angularLimits[axis - 3].m_stopERP = value;
					m_flags |= 4 << axis * 3;
					break;
				case ConstraintParams.BT_CONSTRAINT_STOP_CFM:
					m_angularLimits[axis - 3].m_stopCFM = value;
					m_flags |= 2 << axis * 3;
					break;
				case ConstraintParams.BT_CONSTRAINT_CFM:
					m_angularLimits[axis - 3].m_normalCFM = value;
					m_flags |= 1 << axis * 3;
					break;
				}
			}
		}
	}
}
