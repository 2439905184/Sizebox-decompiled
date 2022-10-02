using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class SliderConstraint : TypedConstraint
	{
		public const float SLIDER_CONSTRAINT_DEF_SOFTNESS = 1f;

		public const float SLIDER_CONSTRAINT_DEF_DAMPING = 1f;

		public const float SLIDER_CONSTRAINT_DEF_RESTITUTION = 0.7f;

		public const float SLIDER_CONSTRAINT_DEF_CFM = 0f;

		private const bool m_useSolveConstraintObsolete = false;

		private static bool USE_OFFSET_FOR_CONSTANT_FRAME = true;

		protected bool m_useOffsetForConstraintFrame;

		protected IndexedMatrix m_frameInA = IndexedMatrix.Identity;

		protected IndexedMatrix m_frameInB = IndexedMatrix.Identity;

		protected bool m_useLinearReferenceFrameA;

		protected float m_lowerLinLimit;

		protected float m_upperLinLimit;

		protected float m_lowerAngLimit;

		protected float m_upperAngLimit;

		protected float m_softnessDirLin;

		protected float m_restitutionDirLin;

		protected float m_dampingDirLin;

		protected float m_cfmDirLin;

		protected float m_softnessDirAng;

		protected float m_restitutionDirAng;

		protected float m_dampingDirAng;

		protected float m_cfmDirAng;

		protected float m_softnessLimLin;

		protected float m_restitutionLimLin;

		protected float m_dampingLimLin;

		protected float m_cfmLimLin;

		protected float m_softnessLimAng;

		protected float m_restitutionLimAng;

		protected float m_dampingLimAng;

		protected float m_cfmLimAng;

		protected float m_softnessOrthoLin;

		protected float m_restitutionOrthoLin;

		protected float m_dampingOrthoLin;

		protected float m_cfmOrthoLin;

		protected float m_softnessOrthoAng;

		protected float m_restitutionOrthoAng;

		protected float m_dampingOrthoAng;

		protected float m_cfmOrthoAng;

		protected bool m_solveLinLim;

		protected bool m_solveAngLim;

		protected int m_flags;

		protected JacobianEntry[] m_jacLin = new JacobianEntry[3];

		protected float[] m_jacLinDiagABInv = new float[3];

		protected JacobianEntry[] m_jacAng = new JacobianEntry[3];

		protected float m_timeStep;

		protected IndexedMatrix m_calculatedTransformA = IndexedMatrix.Identity;

		protected IndexedMatrix m_calculatedTransformB = IndexedMatrix.Identity;

		protected IndexedVector3 m_sliderAxis;

		protected IndexedVector3 m_realPivotAInW;

		protected IndexedVector3 m_realPivotBInW;

		protected IndexedVector3 m_projPivotInW;

		protected IndexedVector3 m_delta;

		protected IndexedVector3 m_depth;

		protected IndexedVector3 m_relPosA;

		protected IndexedVector3 m_relPosB;

		protected float m_linPos;

		protected float m_angPos;

		protected float m_angDepth;

		protected float m_kAngle;

		protected bool m_poweredLinMotor;

		protected float m_targetLinMotorVelocity;

		protected float m_maxLinMotorForce;

		protected float m_accumulatedLinMotorImpulse;

		protected bool m_poweredAngMotor;

		protected float m_targetAngMotorVelocity;

		protected float m_maxAngMotorForce;

		protected float m_accumulatedAngMotorImpulse;

		public SliderConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix frameInA, ref IndexedMatrix frameInB, bool useLinearReferenceFrameA)
			: base(TypedConstraintType.SLIDER_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_frameInA = frameInA;
			m_frameInB = frameInB;
			m_useLinearReferenceFrameA = useLinearReferenceFrameA;
			InitParams();
		}

		public SliderConstraint(RigidBody rbB, ref IndexedMatrix frameInB, bool useLinearReferenceFrameA)
			: base(TypedConstraintType.SLIDER_CONSTRAINT_TYPE, TypedConstraint.GetFixedBody(), rbB)
		{
			m_frameInB = frameInB;
			m_frameInA = rbB.GetCenterOfMassTransform() * m_frameInB;
			InitParams();
		}

		protected void InitParams()
		{
			m_lowerLinLimit = 1f;
			m_upperLinLimit = -1f;
			m_lowerAngLimit = 0f;
			m_upperAngLimit = 0f;
			m_softnessDirLin = 1f;
			m_restitutionDirLin = 0.7f;
			m_dampingDirLin = 0f;
			m_cfmDirLin = 0f;
			m_softnessDirAng = 1f;
			m_restitutionDirAng = 0.7f;
			m_dampingDirAng = 0f;
			m_cfmDirAng = 0f;
			m_softnessOrthoLin = 1f;
			m_restitutionOrthoLin = 0.7f;
			m_dampingOrthoLin = 1f;
			m_cfmOrthoLin = 0f;
			m_softnessOrthoAng = 1f;
			m_restitutionOrthoAng = 0.7f;
			m_dampingOrthoAng = 1f;
			m_cfmOrthoAng = 0f;
			m_softnessLimLin = 1f;
			m_restitutionLimLin = 0.7f;
			m_dampingLimLin = 1f;
			m_cfmLimLin = 0f;
			m_softnessLimAng = 1f;
			m_restitutionLimAng = 0.7f;
			m_dampingLimAng = 1f;
			m_cfmLimAng = 0f;
			m_poweredLinMotor = false;
			m_targetLinMotorVelocity = 0f;
			m_maxLinMotorForce = 0f;
			m_accumulatedLinMotorImpulse = 0f;
			m_poweredAngMotor = false;
			m_targetAngMotorVelocity = 0f;
			m_maxAngMotorForce = 0f;
			m_accumulatedAngMotorImpulse = 0f;
			m_useOffsetForConstraintFrame = USE_OFFSET_FOR_CONSTANT_FRAME;
			m_flags = 0;
			CalculateTransforms(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 4;
			info.nub = 2;
			CalculateTransforms(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
			TestAngLimits();
			TestLinLimits();
			if (GetSolveLinLimit() || GetPoweredLinMotor())
			{
				info.m_numConstraintRows++;
				info.nub--;
			}
			if (GetSolveAngLimit() || GetPoweredAngMotor())
			{
				info.m_numConstraintRows++;
				info.nub--;
			}
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 6;
			info.nub = 0;
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
			GetInfo2NonVirtual(info, m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetLinearVelocity(), m_rbB.GetLinearVelocity(), m_rbA.GetInvMass(), m_rbB.GetInvMass());
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedVector3 linVelA, IndexedVector3 linVelB, float rbAinvMass, float rbBinvMass)
		{
			IndexedMatrix calculatedTransformA = GetCalculatedTransformA();
			IndexedMatrix calculatedTransformB = GetCalculatedTransformB();
			int num = 1;
			float num2 = (m_useLinearReferenceFrameA ? 1f : (-1f));
			IndexedVector3 b = calculatedTransformB._origin - calculatedTransformA._origin;
			bool flag = rbAinvMass < 1.1920929E-07f || rbBinvMass < 1.1920929E-07f;
			float num3 = rbAinvMass + rbBinvMass;
			float num4 = ((!(num3 > 0f)) ? 0.5f : (rbBinvMass / num3));
			float num5 = 1f - num4;
			IndexedVector3 zero = IndexedVector3.Zero;
			IndexedVector3 column = calculatedTransformA._basis.GetColumn(0);
			IndexedVector3 column2 = calculatedTransformB._basis.GetColumn(0);
			IndexedVector3 p;
			IndexedVector3 q;
			if (m_useOffsetForConstraintFrame)
			{
				zero = column * num4 + column2 * num5;
				zero.Normalize();
				TransformUtil.PlaneSpace1(ref zero, out p, out q);
			}
			else
			{
				zero = calculatedTransformA._basis.GetColumn(0);
				p = calculatedTransformA._basis.GetColumn(1);
				q = calculatedTransformA._basis.GetColumn(2);
			}
			info.m_solverConstraints[0].m_relpos1CrossNormal = p;
			info.m_solverConstraints[num].m_relpos1CrossNormal = q;
			info.m_solverConstraints[0].m_relpos2CrossNormal = -p;
			info.m_solverConstraints[num].m_relpos2CrossNormal = -q;
			float num6 = ((((uint)m_flags & 0x80u) != 0) ? m_softnessOrthoAng : (m_softnessOrthoAng * info.erp));
			float num7 = info.fps * num6;
			IndexedVector3 a = IndexedVector3.Cross(column, column2);
			info.m_solverConstraints[0].m_rhs = num7 * IndexedVector3.Dot(a, p);
			info.m_solverConstraints[num].m_rhs = num7 * IndexedVector3.Dot(a, q);
			if (((uint)m_flags & 0x40u) != 0)
			{
				info.m_solverConstraints[0].m_cfm = m_cfmOrthoAng;
				info.m_solverConstraints[num].m_cfm = m_cfmOrthoAng;
			}
			int num8 = 1;
			int num9 = num8;
			IndexedMatrix indexedMatrix = transA;
			IndexedMatrix indexedMatrix2 = transB;
			num8++;
			int num10 = num8 * num;
			num8++;
			int num11 = num8 * num;
			IndexedVector3 zero2 = IndexedVector3.Zero;
			IndexedVector3 zero3 = IndexedVector3.Zero;
			IndexedVector3 v = IndexedVector3.Zero;
			IndexedVector3 v2 = IndexedVector3.Zero;
			IndexedVector3 zero4 = IndexedVector3.Zero;
			if (m_useOffsetForConstraintFrame)
			{
				v2 = calculatedTransformB._origin - indexedMatrix2._origin;
				IndexedVector3 indexedVector = zero * IndexedVector3.Dot(v2, zero);
				IndexedVector3 indexedVector2 = v2 - indexedVector;
				v = calculatedTransformA._origin - indexedMatrix._origin;
				IndexedVector3 indexedVector3 = zero * IndexedVector3.Dot(v, zero);
				IndexedVector3 indexedVector4 = v - indexedVector3;
				float num12 = m_linPos - m_depth.X;
				IndexedVector3 indexedVector5 = indexedVector3 + zero * num12 - indexedVector;
				v = indexedVector4 + indexedVector5 * num4;
				v2 = indexedVector2 - indexedVector5 * num5;
				p = indexedVector2 * num4 + indexedVector4 * num5;
				float num13 = p.LengthSquared();
				if (num13 > 1.1920929E-07f)
				{
					p.Normalize();
				}
				else
				{
					p = calculatedTransformA._basis.GetColumn(1);
				}
				q = IndexedVector3.Cross(zero, p);
				zero2 = IndexedVector3.Cross(v, p);
				zero3 = IndexedVector3.Cross(v2, p);
				info.m_solverConstraints[num10].m_relpos1CrossNormal = zero2;
				info.m_solverConstraints[num10].m_relpos2CrossNormal = -zero3;
				zero2 = IndexedVector3.Cross(v, q);
				zero3 = IndexedVector3.Cross(v2, q);
				if (flag && GetSolveAngLimit())
				{
					zero3 *= num5;
					zero2 *= num4;
				}
				info.m_solverConstraints[num11].m_relpos1CrossNormal = zero2;
				info.m_solverConstraints[num11].m_relpos2CrossNormal = -zero3;
				info.m_solverConstraints[num10].m_contactNormal = p;
				info.m_solverConstraints[num11].m_contactNormal = q;
			}
			else
			{
				IndexedVector3 indexedVector6 = IndexedVector3.Cross(zero4, p);
				info.m_solverConstraints[num10].m_relpos1CrossNormal = num4 * indexedVector6;
				info.m_solverConstraints[num10].m_relpos2CrossNormal = num5 * indexedVector6;
				indexedVector6 = IndexedVector3.Cross(zero4, q);
				info.m_solverConstraints[num11].m_relpos1CrossNormal = num4 * indexedVector6;
				info.m_solverConstraints[num11].m_relpos2CrossNormal = num5 * indexedVector6;
				info.m_solverConstraints[num10].m_contactNormal = p;
				info.m_solverConstraints[num11].m_contactNormal = q;
			}
			num6 = ((((uint)m_flags & 0x20u) != 0) ? m_softnessOrthoLin : (m_softnessOrthoLin * info.erp));
			num7 = info.fps * num6;
			float rhs = num7 * IndexedVector3.Dot(p, b);
			info.m_solverConstraints[num10].m_rhs = rhs;
			rhs = num7 * IndexedVector3.Dot(q, b);
			info.m_solverConstraints[num11].m_rhs = rhs;
			if (((uint)m_flags & 0x10u) != 0)
			{
				info.m_solverConstraints[num10].m_cfm = m_cfmOrthoLin;
				info.m_solverConstraints[num11].m_cfm = m_cfmOrthoLin;
			}
			float num14 = 0f;
			int num15 = 0;
			if (GetSolveLinLimit())
			{
				num14 = GetLinDepth() * num2;
				num15 = ((!(num14 > 0f)) ? 1 : 2);
			}
			bool flag2 = false;
			if (GetPoweredLinMotor())
			{
				flag2 = true;
			}
			if (num15 != 0 || flag2)
			{
				num8++;
				num9 = num8;
				info.m_solverConstraints[num9].m_contactNormal = zero;
				if (m_useOffsetForConstraintFrame)
				{
					if (!flag)
					{
						zero2 = IndexedVector3.Cross(v, zero);
						zero3 = IndexedVector3.Cross(v2, zero);
						info.m_solverConstraints[num9].m_relpos1CrossNormal = zero2;
						info.m_solverConstraints[num9].m_relpos2CrossNormal = -zero3;
					}
				}
				else
				{
					IndexedVector3 indexedVector7 = IndexedVector3.Cross(zero4, zero);
					info.m_solverConstraints[num8].m_relpos1CrossNormal = num4 * indexedVector7;
					info.m_solverConstraints[num8].m_relpos2CrossNormal = num5 * indexedVector7;
				}
				float lowerLinLimit = GetLowerLinLimit();
				float upperLinLimit = GetUpperLinLimit();
				if (num15 != 0 && MathUtil.CompareFloat(lowerLinLimit, upperLinLimit))
				{
					flag2 = false;
				}
				info.m_solverConstraints[num8].m_rhs = 0f;
				info.m_solverConstraints[num8].m_lowerLimit = 0f;
				info.m_solverConstraints[num8].m_upperLimit = 0f;
				num6 = ((((uint)m_flags & 0x200u) != 0) ? m_softnessLimLin : info.erp);
				if (flag2)
				{
					if (((uint)m_flags & (true ? 1u : 0u)) != 0)
					{
						info.m_solverConstraints[num8].m_cfm = m_cfmDirLin;
					}
					float targetLinMotorVelocity = GetTargetLinMotorVelocity();
					float motorFactor = GetMotorFactor(m_linPos, m_lowerLinLimit, m_upperLinLimit, targetLinMotorVelocity, info.fps * num6);
					info.m_solverConstraints[num8].m_rhs -= num2 * motorFactor * GetTargetLinMotorVelocity();
					info.m_solverConstraints[num8].m_lowerLimit += (0f - GetMaxLinMotorForce()) * info.fps;
					info.m_solverConstraints[num8].m_upperLimit += GetMaxLinMotorForce() * info.fps;
				}
				if (num15 != 0)
				{
					num7 = info.fps * num6;
					info.m_solverConstraints[num8].m_rhs += num7 * num14;
					if (((uint)m_flags & 0x100u) != 0)
					{
						info.m_solverConstraints[num8].m_cfm = m_cfmLimLin;
					}
					if (MathUtil.CompareFloat(lowerLinLimit, upperLinLimit))
					{
						info.m_solverConstraints[num8].m_lowerLimit = float.MinValue;
						info.m_solverConstraints[num8].m_upperLimit = float.MaxValue;
					}
					else if (num15 == 1)
					{
						info.m_solverConstraints[num8].m_lowerLimit = float.MinValue;
						info.m_solverConstraints[num8].m_upperLimit = 0f;
					}
					else
					{
						info.m_solverConstraints[num8].m_lowerLimit = 0f;
						info.m_solverConstraints[num8].m_upperLimit = float.MaxValue;
					}
					float num16 = Math.Abs(1f - GetDampingLimLin());
					if (num16 > 0f)
					{
						float num17 = IndexedVector3.Dot(linVelA, zero);
						num17 -= IndexedVector3.Dot(linVelB, zero);
						num17 *= num2;
						if (num15 == 1)
						{
							if (num17 < 0f)
							{
								float num18 = (0f - num16) * num17;
								if (num18 > info.m_solverConstraints[num8].m_rhs)
								{
									info.m_solverConstraints[num8].m_rhs = num18;
								}
							}
						}
						else if (num17 > 0f)
						{
							float num19 = (0f - num16) * num17;
							if (num19 < info.m_solverConstraints[num8].m_rhs)
							{
								info.m_solverConstraints[num8].m_rhs = num19;
							}
						}
					}
					info.m_solverConstraints[num8].m_rhs *= GetSoftnessLimLin();
				}
			}
			num14 = 0f;
			num15 = 0;
			if (GetSolveAngLimit())
			{
				num14 = GetAngDepth();
				num15 = ((num14 > 0f) ? 1 : 2);
			}
			flag2 = false;
			if (GetPoweredAngMotor())
			{
				flag2 = true;
			}
			if (num15 == 0 && !flag2)
			{
				return;
			}
			num8++;
			num9 = num8;
			info.m_solverConstraints[num9].m_relpos1CrossNormal = zero;
			info.m_solverConstraints[num9].m_relpos2CrossNormal = -zero;
			float lowerAngLimit = GetLowerAngLimit();
			float upperAngLimit = GetUpperAngLimit();
			if (num15 != 0 && MathUtil.CompareFloat(lowerAngLimit, upperAngLimit))
			{
				flag2 = false;
			}
			num6 = ((((uint)m_flags & 0x800u) != 0) ? m_softnessLimAng : info.erp);
			if (flag2)
			{
				if (((uint)m_flags & 4u) != 0)
				{
					info.m_solverConstraints[num8].m_cfm = m_cfmDirAng;
				}
				float motorFactor2 = GetMotorFactor(m_angPos, m_lowerAngLimit, m_upperAngLimit, GetTargetAngMotorVelocity(), info.fps * num6);
				info.m_solverConstraints[num8].m_rhs = motorFactor2 * GetTargetAngMotorVelocity();
				info.m_solverConstraints[num8].m_lowerLimit = (0f - GetMaxAngMotorForce()) * info.fps;
				info.m_solverConstraints[num8].m_upperLimit = GetMaxAngMotorForce() * info.fps;
			}
			if (num15 == 0)
			{
				return;
			}
			num7 = info.fps * num6;
			info.m_solverConstraints[num8].m_rhs += num7 * num14;
			if (((uint)m_flags & 0x400u) != 0)
			{
				info.m_solverConstraints[num8].m_cfm = m_cfmLimAng;
			}
			if (MathUtil.CompareFloat(lowerAngLimit, upperAngLimit))
			{
				info.m_solverConstraints[num8].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num8].m_upperLimit = float.MaxValue;
			}
			else if (num15 == 1)
			{
				info.m_solverConstraints[num8].m_lowerLimit = 0f;
				info.m_solverConstraints[num8].m_upperLimit = float.MaxValue;
			}
			else
			{
				info.m_solverConstraints[num8].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num8].m_upperLimit = 0f;
			}
			float num20 = Math.Abs(1f - GetDampingLimAng());
			if (num20 > 0f)
			{
				float num21 = IndexedVector3.Dot(m_rbA.GetAngularVelocity(), zero);
				num21 -= IndexedVector3.Dot(m_rbB.GetAngularVelocity(), zero);
				if (num15 == 1)
				{
					if (num21 < 0f)
					{
						float num22 = (0f - num20) * num21;
						if (num22 > info.m_solverConstraints[num8].m_rhs)
						{
							info.m_solverConstraints[num8].m_rhs = num22;
						}
					}
				}
				else if (num21 > 0f)
				{
					float num23 = (0f - num20) * num21;
					if (num23 < info.m_solverConstraints[num8].m_rhs)
					{
						info.m_solverConstraints[num8].m_rhs = num23;
					}
				}
			}
			info.m_solverConstraints[num8].m_rhs *= GetSoftnessLimAng();
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

		public float GetLowerLinLimit()
		{
			return m_lowerLinLimit;
		}

		public void SetLowerLinLimit(float lowerLimit)
		{
			m_lowerLinLimit = lowerLimit;
		}

		public float GetUpperLinLimit()
		{
			return m_upperLinLimit;
		}

		public void SetUpperLinLimit(float upperLimit)
		{
			m_upperLinLimit = upperLimit;
		}

		public float GetLowerAngLimit()
		{
			return m_lowerAngLimit;
		}

		public void SetLowerAngLimit(float lowerLimit)
		{
			m_lowerAngLimit = MathUtil.NormalizeAngle(lowerLimit);
		}

		public float GetUpperAngLimit()
		{
			return m_upperAngLimit;
		}

		public void SetUpperAngLimit(float upperLimit)
		{
			m_upperAngLimit = MathUtil.NormalizeAngle(upperLimit);
		}

		public bool GetUseLinearReferenceFrameA()
		{
			return m_useLinearReferenceFrameA;
		}

		public float GetSoftnessDirLin()
		{
			return m_softnessDirLin;
		}

		public float GetRestitutionDirLin()
		{
			return m_restitutionDirLin;
		}

		public float GetDampingDirLin()
		{
			return m_dampingDirLin;
		}

		public float GetSoftnessDirAng()
		{
			return m_softnessDirAng;
		}

		public float GetRestitutionDirAng()
		{
			return m_restitutionDirAng;
		}

		public float GetDampingDirAng()
		{
			return m_dampingDirAng;
		}

		public float GetSoftnessLimLin()
		{
			return m_softnessLimLin;
		}

		public float GetRestitutionLimLin()
		{
			return m_restitutionLimLin;
		}

		public float GetDampingLimLin()
		{
			return m_dampingLimLin;
		}

		public float GetSoftnessLimAng()
		{
			return m_softnessLimAng;
		}

		public float GetRestitutionLimAng()
		{
			return m_restitutionLimAng;
		}

		public float GetDampingLimAng()
		{
			return m_dampingLimAng;
		}

		public float GetSoftnessOrthoLin()
		{
			return m_softnessOrthoLin;
		}

		public float GetRestitutionOrthoLin()
		{
			return m_restitutionOrthoLin;
		}

		public float GetDampingOrthoLin()
		{
			return m_dampingOrthoLin;
		}

		public float GetSoftnessOrthoAng()
		{
			return m_softnessOrthoAng;
		}

		public float GetRestitutionOrthoAng()
		{
			return m_restitutionOrthoAng;
		}

		public float GetDampingOrthoAng()
		{
			return m_dampingOrthoAng;
		}

		public void SetSoftnessDirLin(float softnessDirLin)
		{
			m_softnessDirLin = softnessDirLin;
		}

		public void SetRestitutionDirLin(float restitutionDirLin)
		{
			m_restitutionDirLin = restitutionDirLin;
		}

		public void SetDampingDirLin(float dampingDirLin)
		{
			m_dampingDirLin = dampingDirLin;
		}

		public void SetSoftnessDirAng(float softnessDirAng)
		{
			m_softnessDirAng = softnessDirAng;
		}

		public void SetRestitutionDirAng(float restitutionDirAng)
		{
			m_restitutionDirAng = restitutionDirAng;
		}

		public void SetDampingDirAng(float dampingDirAng)
		{
			m_dampingDirAng = dampingDirAng;
		}

		public void SetSoftnessLimLin(float softnessLimLin)
		{
			m_softnessLimLin = softnessLimLin;
		}

		public void SetRestitutionLimLin(float restitutionLimLin)
		{
			m_restitutionLimLin = restitutionLimLin;
		}

		public void SetDampingLimLin(float dampingLimLin)
		{
			m_dampingLimLin = dampingLimLin;
		}

		public void SetSoftnessLimAng(float softnessLimAng)
		{
			m_softnessLimAng = softnessLimAng;
		}

		public void SetRestitutionLimAng(float restitutionLimAng)
		{
			m_restitutionLimAng = restitutionLimAng;
		}

		public void SetDampingLimAng(float dampingLimAng)
		{
			m_dampingLimAng = dampingLimAng;
		}

		public void SetSoftnessOrthoLin(float softnessOrthoLin)
		{
			m_softnessOrthoLin = softnessOrthoLin;
		}

		public void SetRestitutionOrthoLin(float restitutionOrthoLin)
		{
			m_restitutionOrthoLin = restitutionOrthoLin;
		}

		public void SetDampingOrthoLin(float dampingOrthoLin)
		{
			m_dampingOrthoLin = dampingOrthoLin;
		}

		public void SetSoftnessOrthoAng(float softnessOrthoAng)
		{
			m_softnessOrthoAng = softnessOrthoAng;
		}

		public void SetRestitutionOrthoAng(float restitutionOrthoAng)
		{
			m_restitutionOrthoAng = restitutionOrthoAng;
		}

		public void SetDampingOrthoAng(float dampingOrthoAng)
		{
			m_dampingOrthoAng = dampingOrthoAng;
		}

		public void SetPoweredLinMotor(bool onOff)
		{
			m_poweredLinMotor = onOff;
		}

		public bool GetPoweredLinMotor()
		{
			return m_poweredLinMotor;
		}

		public void SetTargetLinMotorVelocity(float targetLinMotorVelocity)
		{
			m_targetLinMotorVelocity = targetLinMotorVelocity;
		}

		public float GetTargetLinMotorVelocity()
		{
			return m_targetLinMotorVelocity;
		}

		public void SetMaxLinMotorForce(float maxLinMotorForce)
		{
			m_maxLinMotorForce = maxLinMotorForce;
		}

		public float GetMaxLinMotorForce()
		{
			return m_maxLinMotorForce;
		}

		public void SetPoweredAngMotor(bool onOff)
		{
			m_poweredAngMotor = onOff;
		}

		public bool GetPoweredAngMotor()
		{
			return m_poweredAngMotor;
		}

		public void SetTargetAngMotorVelocity(float targetAngMotorVelocity)
		{
			m_targetAngMotorVelocity = targetAngMotorVelocity;
		}

		public float GetTargetAngMotorVelocity()
		{
			return m_targetAngMotorVelocity;
		}

		public void SetMaxAngMotorForce(float maxAngMotorForce)
		{
			m_maxAngMotorForce = maxAngMotorForce;
		}

		public float GetMaxAngMotorForce()
		{
			return m_maxAngMotorForce;
		}

		public float GetLinearPos()
		{
			return m_linPos;
		}

		public float GetAngularPos()
		{
			return m_angPos;
		}

		public bool GetSolveLinLimit()
		{
			return m_solveLinLim;
		}

		public float GetLinDepth()
		{
			return m_depth.X;
		}

		public bool GetSolveAngLimit()
		{
			return m_solveAngLim;
		}

		public float GetAngDepth()
		{
			return m_angDepth;
		}

		public void SetFrames(ref IndexedMatrix frameA, ref IndexedMatrix frameB)
		{
			m_frameInA = frameA;
			m_frameInB = frameB;
			CalculateTransforms(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
		}

		public void CalculateTransforms(IndexedMatrix transA, IndexedMatrix transB)
		{
			CalculateTransforms(ref transA, ref transB);
		}

		public void CalculateTransforms(ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			m_calculatedTransformA = transA * m_frameInA;
			m_calculatedTransformB = transB * m_frameInB;
			m_realPivotAInW = m_calculatedTransformA._origin;
			m_realPivotBInW = m_calculatedTransformB._origin;
			m_sliderAxis = m_calculatedTransformA._basis.GetColumn(0);
			if (m_useLinearReferenceFrameA)
			{
				m_delta = m_realPivotBInW - m_realPivotAInW;
			}
			else
			{
				m_delta = m_realPivotAInW - m_realPivotBInW;
			}
			m_projPivotInW = m_realPivotAInW + IndexedVector3.Dot(m_sliderAxis, m_delta) * m_sliderAxis;
			for (int i = 0; i < 3; i++)
			{
				IndexedVector3 v = m_calculatedTransformA._basis.GetColumn(i);
				m_depth[i] = m_delta.Dot(ref v);
			}
		}

		public void TestLinLimits()
		{
			m_solveLinLim = false;
			m_linPos = m_depth.X;
			if (m_lowerLinLimit <= m_upperLinLimit)
			{
				if (m_depth.X > m_upperLinLimit)
				{
					m_depth.X -= m_upperLinLimit;
					m_solveLinLim = true;
				}
				else if (m_depth.X < m_lowerLinLimit)
				{
					m_depth.X -= m_lowerLinLimit;
					m_solveLinLim = true;
				}
				else
				{
					m_depth.X = 0f;
				}
			}
			else
			{
				m_depth.X = 0f;
			}
		}

		public void TestLinLimits2(ConstraintInfo2 info)
		{
		}

		public void TestAngLimits()
		{
			m_angDepth = 0f;
			m_solveAngLim = false;
			if (m_lowerAngLimit <= m_upperAngLimit)
			{
				IndexedVector3 column = m_calculatedTransformA._basis.GetColumn(1);
				IndexedVector3 column2 = m_calculatedTransformA._basis.GetColumn(2);
				IndexedVector3 column3 = m_calculatedTransformB._basis.GetColumn(1);
				float angleInRadians = (float)Math.Atan2(IndexedVector3.Dot(column3, column2), IndexedVector3.Dot(column3, column));
				angleInRadians = (m_angPos = AdjustAngleToLimits(angleInRadians, m_lowerAngLimit, m_upperAngLimit));
				if (angleInRadians < m_lowerAngLimit)
				{
					m_angDepth = angleInRadians - m_lowerAngLimit;
					m_solveAngLim = true;
				}
				else if (angleInRadians > m_upperAngLimit)
				{
					m_angDepth = angleInRadians - m_upperAngLimit;
					m_solveAngLim = true;
				}
			}
		}

		public IndexedVector3 GetAncorInA()
		{
			IndexedVector3 indexedVector = m_realPivotAInW + (m_lowerLinLimit + m_upperLinLimit) * 0.5f * m_sliderAxis;
			return m_rbA.GetCenterOfMassTransform().Inverse() * indexedVector;
		}

		public IndexedVector3 GetAncorInB()
		{
			return m_frameInB._origin;
		}

		public bool GetUseFrameOffset()
		{
			return m_useOffsetForConstraintFrame;
		}

		public void SetUseFrameOffset(bool frameOffsetOnOff)
		{
			m_useOffsetForConstraintFrame = frameOffsetOnOff;
		}
	}
}
