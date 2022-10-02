using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class HingeConstraint : TypedConstraint
	{
		private const bool HINGE_USE_FRAME_OFFSET = true;

		private static IndexedVector3 vHinge = new IndexedVector3(0f, 0f, 1f);

		private IndexedMatrix m_rbAFrame = IndexedMatrix.Identity;

		private IndexedMatrix m_rbBFrame = IndexedMatrix.Identity;

		private float m_motorTargetVelocity;

		private float m_maxMotorImpulse;

		private AngularLimit m_limit;

		private float m_kHinge;

		private float m_accLimitImpulse;

		private float m_hingeAngle;

		private float m_referenceSign;

		private bool m_angularOnly;

		private bool m_enableAngularMotor;

		private bool m_useOffsetForConstraintFrame;

		private bool m_useReferenceFrameA;

		private float m_accMotorImpulse;

		private int m_flags;

		private float m_normalCFM;

		private float m_stopCFM;

		private float m_stopERP;

		public HingeConstraint(RigidBody rbA, RigidBody rbB, ref IndexedVector3 pivotInA, ref IndexedVector3 pivotInB, ref IndexedVector3 axisInA, ref IndexedVector3 axisInB)
			: this(rbA, rbB, ref pivotInA, ref pivotInB, ref axisInA, ref axisInB, false)
		{
		}

		public HingeConstraint(RigidBody rbA, RigidBody rbB, ref IndexedVector3 pivotInA, ref IndexedVector3 pivotInB, ref IndexedVector3 axisInA, ref IndexedVector3 axisInB, bool useReferenceFrameA)
			: base(TypedConstraintType.HINGE_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_angularOnly = false;
			m_enableAngularMotor = false;
			m_useOffsetForConstraintFrame = true;
			m_useReferenceFrameA = useReferenceFrameA;
			m_rbAFrame._origin = pivotInA;
			m_limit = new AngularLimit();
			m_flags = 0;
			IndexedVector3 column = rbA.GetCenterOfMassTransform()._basis.GetColumn(0);
			IndexedVector3 zero = IndexedVector3.Zero;
			float num = IndexedVector3.Dot(axisInA, column);
			if (num >= 0.9999999f)
			{
				column = -rbA.GetCenterOfMassTransform()._basis.GetColumn(2);
				zero = rbA.GetCenterOfMassTransform()._basis.GetColumn(1);
			}
			else if (num <= -0.9999999f)
			{
				column = rbA.GetCenterOfMassTransform()._basis.GetColumn(2);
				zero = rbA.GetCenterOfMassTransform()._basis.GetColumn(1);
			}
			else
			{
				zero = IndexedVector3.Cross(axisInA, column);
				column = IndexedVector3.Cross(zero, axisInA);
			}
			m_rbAFrame._basis = new IndexedBasisMatrix(column.X, zero.X, axisInA.X, column.Y, zero.Y, axisInA.Y, column.Z, zero.Z, axisInA.Z);
			IndexedQuaternion rotation = MathUtil.ShortestArcQuat(ref axisInA, ref axisInB);
			IndexedVector3 v = MathUtil.QuatRotate(ref rotation, ref column);
			IndexedVector3 indexedVector = IndexedVector3.Cross(axisInB, v);
			m_rbBFrame._origin = pivotInB;
			m_rbBFrame._basis = new IndexedBasisMatrix(v.X, indexedVector.X, axisInB.X, v.Y, indexedVector.Y, axisInB.Y, v.Z, indexedVector.Z, axisInB.Z);
			m_referenceSign = (m_useReferenceFrameA ? (-1f) : 1f);
		}

		public HingeConstraint(RigidBody rbA, ref IndexedVector3 pivotInA, ref IndexedVector3 axisInA, bool useReferenceFrameA)
			: base(TypedConstraintType.HINGE_CONSTRAINT_TYPE, rbA)
		{
			m_angularOnly = false;
			m_enableAngularMotor = false;
			m_useReferenceFrameA = useReferenceFrameA;
			m_useOffsetForConstraintFrame = true;
			m_flags = 0;
			m_limit = new AngularLimit();
			IndexedVector3 p;
			IndexedVector3 q;
			TransformUtil.PlaneSpace1(ref axisInA, out p, out q);
			m_rbAFrame._origin = pivotInA;
			m_rbAFrame._basis = new IndexedBasisMatrix(p.X, q.X, axisInA.X, p.Y, q.Y, axisInA.Y, p.Z, q.Z, axisInA.Z);
			IndexedVector3 axisInB = rbA.GetCenterOfMassTransform()._basis * axisInA;
			IndexedQuaternion rotation = MathUtil.ShortestArcQuat(ref axisInA, ref axisInB);
			IndexedVector3 v = MathUtil.QuatRotate(ref rotation, ref p);
			IndexedVector3 indexedVector = IndexedVector3.Cross(axisInB, v);
			m_rbBFrame._origin = rbA.GetCenterOfMassTransform() * pivotInA;
			m_rbBFrame._basis = new IndexedBasisMatrix(v.X, indexedVector.X, axisInB.X, v.Y, indexedVector.Y, axisInB.Y, v.Z, indexedVector.Z, axisInB.Z);
			m_referenceSign = (m_useReferenceFrameA ? (-1f) : 1f);
		}

		public HingeConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix rbAFrame, ref IndexedMatrix rbBFrame)
			: this(rbA, rbB, ref rbAFrame, ref rbBFrame, false)
		{
		}

		public HingeConstraint(RigidBody rbA, RigidBody rbB, ref IndexedMatrix rbAFrame, ref IndexedMatrix rbBFrame, bool useReferenceFrameA)
			: base(TypedConstraintType.HINGE_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_rbAFrame = rbAFrame;
			m_rbBFrame = rbBFrame;
			m_angularOnly = false;
			m_enableAngularMotor = false;
			m_useOffsetForConstraintFrame = true;
			m_useReferenceFrameA = useReferenceFrameA;
			m_flags = 0;
			m_limit = new AngularLimit();
			m_referenceSign = (m_useReferenceFrameA ? (-1f) : 1f);
		}

		public HingeConstraint(RigidBody rbA, ref IndexedMatrix rbAFrame)
			: this(rbA, ref rbAFrame, false)
		{
		}

		public HingeConstraint(RigidBody rbA, ref IndexedMatrix rbAFrame, bool useReferenceFrameA)
			: base(TypedConstraintType.HINGE_CONSTRAINT_TYPE, rbA)
		{
			m_rbAFrame = rbAFrame;
			m_rbBFrame = rbAFrame;
			m_angularOnly = false;
			m_enableAngularMotor = false;
			m_useOffsetForConstraintFrame = true;
			m_useReferenceFrameA = useReferenceFrameA;
			m_flags = 0;
			m_rbBFrame._origin = m_rbA.GetCenterOfMassTransform() * m_rbAFrame._origin;
			m_limit = new AngularLimit();
			m_referenceSign = (m_useReferenceFrameA ? (-1f) : 1f);
		}

		public IndexedMatrix GetFrameOffsetA()
		{
			return m_rbAFrame;
		}

		public IndexedMatrix GetFrameOffsetB()
		{
			return m_rbBFrame;
		}

		public void setFrames(ref IndexedMatrix frameA, ref IndexedMatrix frameB)
		{
			m_rbAFrame = frameA;
			m_rbBFrame = frameB;
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 5;
			info.nub = 1;
			TestLimit(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
			if (GetSolveLimit() || GetEnableAngularMotor())
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
			if (m_useOffsetForConstraintFrame)
			{
				GetInfo2InternalUsingFrameOffset(info, m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetAngularVelocity(), m_rbB.GetAngularVelocity());
			}
			else
			{
				GetInfo2Internal(info, m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform(), m_rbA.GetAngularVelocity(), m_rbB.GetAngularVelocity());
			}
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedVector3 angVelA, IndexedVector3 angVelB)
		{
			TestLimit(ref transA, ref transB);
			GetInfo2Internal(info, transA, transB, angVelA, angVelB);
		}

		public void GetInfo2Internal(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedVector3 angVelA, IndexedVector3 angVelB)
		{
			IndexedMatrix indexedMatrix = transA * m_rbAFrame;
			IndexedMatrix indexedMatrix2 = transB * m_rbBFrame;
			IndexedVector3 origin = indexedMatrix._origin;
			IndexedVector3 origin2 = indexedMatrix2._origin;
			if (!m_angularOnly)
			{
				info.m_solverConstraints[0].m_contactNormal.X = 1f;
				info.m_solverConstraints[1].m_contactNormal.Y = 1f;
				info.m_solverConstraints[2].m_contactNormal.Z = 1f;
			}
			IndexedVector3 indexedVector = origin - transA._origin;
			IndexedVector3 vecin = -indexedVector;
			MathUtil.GetSkewSymmetricMatrix(ref vecin, out info.m_solverConstraints[0].m_relpos1CrossNormal, out info.m_solverConstraints[1].m_relpos1CrossNormal, out info.m_solverConstraints[2].m_relpos1CrossNormal);
			IndexedVector3 vecin2 = origin2 - transB._origin;
			MathUtil.GetSkewSymmetricMatrix(ref vecin2, out info.m_solverConstraints[0].m_relpos2CrossNormal, out info.m_solverConstraints[1].m_relpos2CrossNormal, out info.m_solverConstraints[2].m_relpos2CrossNormal);
			float num = info.fps * info.erp;
			if (!m_angularOnly)
			{
				for (int i = 0; i < 3; i++)
				{
					float rhs = num * (origin2[i] - origin[i]);
					info.m_solverConstraints[i].m_rhs = rhs;
				}
			}
			IndexedVector3 column = indexedMatrix._basis.GetColumn(2);
			IndexedVector3 column2 = indexedMatrix._basis.GetColumn(0);
			IndexedVector3 column3 = indexedMatrix._basis.GetColumn(1);
			int num2 = 3;
			int num3 = 4;
			info.m_solverConstraints[num2].m_relpos1CrossNormal = column2;
			info.m_solverConstraints[num3].m_relpos1CrossNormal = column3;
			info.m_solverConstraints[num2].m_relpos2CrossNormal = -column2;
			info.m_solverConstraints[num3].m_relpos2CrossNormal = -column3;
			IndexedVector3 column4 = indexedMatrix2._basis.GetColumn(2);
			IndexedVector3 a = IndexedVector3.Cross(column, column4);
			info.m_solverConstraints[num2].m_rhs = num * IndexedVector3.Dot(a, column2);
			info.m_solverConstraints[num3].m_rhs = num * IndexedVector3.Dot(a, column3);
			int num4 = 4;
			float num5 = 0f;
			int num6 = 0;
			if (GetSolveLimit())
			{
				num5 = m_limit.GetCorrection() * m_referenceSign;
			}
			bool flag = false;
			if (GetEnableAngularMotor())
			{
				flag = true;
			}
			if (num6 == 0 && !flag)
			{
				return;
			}
			num4++;
			info.m_solverConstraints[num4].m_relpos1CrossNormal = column;
			info.m_solverConstraints[num4].m_relpos2CrossNormal = -column;
			float lowerLimit = GetLowerLimit();
			float upperLimit = GetUpperLimit();
			if (num6 != 0 && MathUtil.CompareFloat(lowerLimit, upperLimit))
			{
				flag = false;
			}
			info.m_solverConstraints[num4].m_rhs = 0f;
			float num7 = ((((uint)m_flags & 2u) != 0) ? m_stopERP : info.erp);
			if (flag)
			{
				if (((uint)m_flags & 4u) != 0)
				{
					info.m_solverConstraints[num4].m_cfm = m_normalCFM;
				}
				float motorFactor = GetMotorFactor(m_hingeAngle, lowerLimit, upperLimit, m_motorTargetVelocity, info.fps * num7);
				info.m_solverConstraints[num4].m_rhs += motorFactor * m_motorTargetVelocity * m_referenceSign;
				info.m_solverConstraints[num4].m_lowerLimit = 0f - m_maxMotorImpulse;
				info.m_solverConstraints[num4].m_upperLimit = m_maxMotorImpulse;
			}
			if (num6 == 0)
			{
				return;
			}
			num = info.fps * num7;
			info.m_solverConstraints[num4].m_rhs += num * num5;
			if (((uint)m_flags & (true ? 1u : 0u)) != 0)
			{
				info.m_solverConstraints[num4].m_cfm = m_stopCFM;
			}
			if (MathUtil.CompareFloat(lowerLimit, upperLimit))
			{
				info.m_solverConstraints[num4].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num4].m_upperLimit = float.MaxValue;
			}
			else if (num6 == 1)
			{
				info.m_solverConstraints[num4].m_lowerLimit = 0f;
				info.m_solverConstraints[num4].m_upperLimit = float.MaxValue;
			}
			else
			{
				info.m_solverConstraints[num4].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num4].m_upperLimit = 0f;
			}
			float relaxationFactor = m_limit.GetRelaxationFactor();
			if (relaxationFactor > 0f)
			{
				float num8 = IndexedVector3.Dot(angVelA, column);
				num8 -= IndexedVector3.Dot(angVelB, column);
				if (num6 == 1)
				{
					if (num8 < 0f)
					{
						float num9 = (0f - relaxationFactor) * num8;
						if (num9 > info.m_solverConstraints[num4].m_rhs)
						{
							info.m_solverConstraints[num4].m_rhs = num9;
						}
					}
				}
				else if (num8 > 0f)
				{
					float num10 = (0f - relaxationFactor) * num8;
					if (num10 < info.m_solverConstraints[num4].m_rhs)
					{
						info.m_solverConstraints[num4].m_rhs = num10;
					}
				}
			}
			info.m_solverConstraints[num4].m_rhs *= m_limit.GetBiasFactor();
		}

		public void SetFrames(ref IndexedMatrix frameA, ref IndexedMatrix frameB)
		{
			m_rbAFrame = frameA;
			m_rbBFrame = frameB;
		}

		public void GetInfo2InternalUsingFrameOffset(ConstraintInfo2 info, IndexedMatrix transA, IndexedMatrix transB, IndexedVector3 angVelA, IndexedVector3 angVelB)
		{
			GetInfo2InternalUsingFrameOffset(info, ref transA, ref transB, ref angVelA, ref angVelB);
		}

		public void GetInfo2InternalUsingFrameOffset(ConstraintInfo2 info, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 angVelA, ref IndexedVector3 angVelB)
		{
			IndexedMatrix indexedMatrix = transA * m_rbAFrame;
			IndexedMatrix indexedMatrix2 = transB * m_rbBFrame;
			IndexedVector3 b = indexedMatrix2._origin - indexedMatrix._origin;
			float invMass = GetRigidBodyA().GetInvMass();
			float invMass2 = GetRigidBodyB().GetInvMass();
			bool flag = invMass < 1.1920929E-07f || invMass2 < 1.1920929E-07f;
			float num = invMass + invMass2;
			float num2 = ((!(num > 0f)) ? 0.5f : (invMass2 / num));
			float num3 = 1f - num2;
			IndexedVector3 v = indexedMatrix._basis.GetColumn(2);
			IndexedVector3 v2 = indexedMatrix2._basis.GetColumn(2);
			IndexedVector3 v3 = v * num2 + v2 * num3;
			v3.Normalize();
			IndexedMatrix indexedMatrix3 = transA;
			IndexedMatrix indexedMatrix4 = transB;
			int num4 = 0;
			int num5 = 1;
			int num6 = 2;
			int num7 = 2;
			IndexedVector3 indexedVector = indexedMatrix2._origin - indexedMatrix4._origin;
			IndexedVector3 indexedVector2 = v3 * IndexedVector3.Dot(indexedVector, v3);
			IndexedVector3 indexedVector3 = indexedVector - indexedVector2;
			IndexedVector3 indexedVector4 = indexedMatrix._origin - indexedMatrix3._origin;
			IndexedVector3 indexedVector5 = v3 * IndexedVector3.Dot(indexedVector4, v3);
			IndexedVector3 indexedVector6 = indexedVector4 - indexedVector5;
			IndexedVector3 indexedVector7 = indexedVector5 - indexedVector2;
			indexedVector4 = indexedVector6 + indexedVector7 * num2;
			indexedVector = indexedVector3 - indexedVector7 * num3;
			IndexedVector3 a = indexedVector3 * num2 + indexedVector6 * num3;
			float num8 = a.LengthSquared();
			if (num8 > 1.1920929E-07f)
			{
				a.Normalize();
			}
			else
			{
				a = indexedMatrix._basis.GetColumn(1);
			}
			IndexedVector3 v4 = IndexedVector3.Cross(v3, a);
			IndexedVector3 relpos1CrossNormal = IndexedVector3.Cross(indexedVector4, a);
			IndexedVector3 indexedVector8 = IndexedVector3.Cross(indexedVector, a);
			info.m_solverConstraints[num4].m_relpos1CrossNormal = relpos1CrossNormal;
			info.m_solverConstraints[num4].m_relpos2CrossNormal = -indexedVector8;
			relpos1CrossNormal = IndexedVector3.Cross(ref indexedVector4, ref v4);
			indexedVector8 = IndexedVector3.Cross(ref indexedVector, ref v4);
			if (flag && GetSolveLimit())
			{
				indexedVector8 *= num3;
				relpos1CrossNormal *= num2;
			}
			info.m_solverConstraints[num5].m_relpos1CrossNormal = relpos1CrossNormal;
			info.m_solverConstraints[num5].m_relpos2CrossNormal = -indexedVector8;
			relpos1CrossNormal = IndexedVector3.Cross(ref indexedVector4, ref v3);
			indexedVector8 = IndexedVector3.Cross(ref indexedVector, ref v3);
			if (flag)
			{
				indexedVector8 *= num3;
				relpos1CrossNormal *= num2;
			}
			info.m_solverConstraints[num6].m_relpos1CrossNormal = relpos1CrossNormal;
			info.m_solverConstraints[num6].m_relpos2CrossNormal = -indexedVector8;
			float num9 = info.fps * info.erp;
			if (!m_angularOnly)
			{
				info.m_solverConstraints[num4].m_contactNormal = a;
				info.m_solverConstraints[num5].m_contactNormal = v4;
				info.m_solverConstraints[num6].m_contactNormal = v3;
				float rhs = num9 * IndexedVector3.Dot(ref a, ref b);
				info.m_solverConstraints[num4].m_rhs = rhs;
				rhs = num9 * IndexedVector3.Dot(ref v4, ref b);
				info.m_solverConstraints[num5].m_rhs = rhs;
				rhs = num9 * IndexedVector3.Dot(ref v3, ref b);
				info.m_solverConstraints[num6].m_rhs = rhs;
			}
			int num10 = 3;
			int num11 = 4;
			info.m_solverConstraints[num10].m_relpos1CrossNormal = a;
			info.m_solverConstraints[num11].m_relpos1CrossNormal = v4;
			info.m_solverConstraints[num10].m_relpos2CrossNormal = -a;
			info.m_solverConstraints[num11].m_relpos2CrossNormal = -v4;
			num9 = info.fps * info.erp;
			IndexedVector3 a2 = IndexedVector3.Cross(ref v, ref v2);
			info.m_solverConstraints[num10].m_rhs = num9 * IndexedVector3.Dot(a2, a);
			info.m_solverConstraints[num11].m_rhs = num9 * IndexedVector3.Dot(a2, v4);
			num7 = 4;
			float num12 = 0f;
			int num13 = 0;
			if (GetSolveLimit())
			{
				num12 = m_limit.GetCorrection() * m_referenceSign;
				num13 = ((num12 > 0f) ? 1 : 2);
			}
			bool flag2 = false;
			if (GetEnableAngularMotor())
			{
				flag2 = true;
			}
			if (num13 == 0 && !flag2)
			{
				return;
			}
			num7++;
			int num14 = num7;
			info.m_solverConstraints[num14].m_relpos1CrossNormal = v3;
			info.m_solverConstraints[num14].m_relpos2CrossNormal = -v3;
			float lowerLimit = GetLowerLimit();
			float upperLimit = GetUpperLimit();
			if (num13 != 0 && MathUtil.CompareFloat(lowerLimit, upperLimit))
			{
				flag2 = false;
			}
			info.m_solverConstraints[num14].m_rhs = 0f;
			float num15 = ((((uint)m_flags & 2u) != 0) ? m_stopERP : info.erp);
			if (flag2)
			{
				if (((uint)m_flags & 4u) != 0)
				{
					info.m_solverConstraints[num14].m_cfm = m_normalCFM;
				}
				float motorFactor = GetMotorFactor(m_hingeAngle, lowerLimit, upperLimit, m_motorTargetVelocity, info.fps * num15);
				info.m_solverConstraints[num14].m_rhs += motorFactor * m_motorTargetVelocity * m_referenceSign;
				info.m_solverConstraints[num14].m_lowerLimit = 0f - m_maxMotorImpulse;
				info.m_solverConstraints[num14].m_upperLimit = m_maxMotorImpulse;
			}
			if (num13 == 0)
			{
				return;
			}
			num9 = info.fps * num15;
			info.m_solverConstraints[num14].m_rhs += num9 * num12;
			if (((uint)m_flags & (true ? 1u : 0u)) != 0)
			{
				info.m_solverConstraints[num14].m_cfm = m_stopCFM;
			}
			if (MathUtil.CompareFloat(lowerLimit, upperLimit))
			{
				info.m_solverConstraints[num14].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num14].m_upperLimit = float.MaxValue;
			}
			else if (num13 == 1)
			{
				info.m_solverConstraints[num14].m_lowerLimit = 0f;
				info.m_solverConstraints[num14].m_upperLimit = float.MaxValue;
			}
			else
			{
				info.m_solverConstraints[num14].m_lowerLimit = float.MinValue;
				info.m_solverConstraints[num14].m_upperLimit = 0f;
			}
			float relaxationFactor = m_limit.GetRelaxationFactor();
			if (relaxationFactor > 0f)
			{
				float num16 = IndexedVector3.Dot(ref angVelA, ref v3);
				num16 -= IndexedVector3.Dot(ref angVelB, ref v3);
				if (num13 == 1)
				{
					if (num16 < 0f)
					{
						float num17 = (0f - relaxationFactor) * num16;
						if (num17 > info.m_solverConstraints[num14].m_rhs)
						{
							info.m_solverConstraints[num14].m_rhs = num17;
						}
					}
				}
				else if (num16 > 0f)
				{
					float num18 = (0f - relaxationFactor) * num16;
					if (num18 < info.m_solverConstraints[num14].m_rhs)
					{
						info.m_solverConstraints[num14].m_rhs = num18;
					}
				}
			}
			info.m_solverConstraints[num14].m_rhs *= m_limit.GetBiasFactor();
		}

		public void UpdateRHS(float timeStep)
		{
		}

		public void SetAngularOnly(bool angularOnly)
		{
			m_angularOnly = angularOnly;
		}

		public void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse)
		{
			m_enableAngularMotor = enableMotor;
			m_motorTargetVelocity = targetVelocity;
			m_maxMotorImpulse = maxMotorImpulse;
		}

		public void EnableMotor(bool enableMotor)
		{
			m_enableAngularMotor = enableMotor;
		}

		public void SetMaxMotorImpulse(float maxMotorImpulse)
		{
			m_maxMotorImpulse = maxMotorImpulse;
		}

		public void SetMotorTarget(ref IndexedQuaternion qAinB, float dt)
		{
			IndexedQuaternion rotation = MathUtil.QuaternionInverse(m_rbBFrame.GetRotation()) * qAinB * m_rbAFrame.GetRotation();
			rotation.Normalize();
			IndexedVector3 axisInB = MathUtil.QuatRotate(ref rotation, ref vHinge);
			axisInB.Normalize();
			IndexedQuaternion q = MathUtil.ShortestArcQuat(ref vHinge, ref axisInB);
			IndexedQuaternion quat = MathUtil.QuaternionInverse(ref q) * rotation;
			quat.Normalize();
			float num = MathUtil.QuatAngle(ref quat);
			if (num > (float)Math.PI)
			{
				quat = -quat;
				num = MathUtil.QuatAngle(ref quat);
			}
			if (quat.Z < 0f)
			{
				num = 0f - num;
			}
			SetMotorTarget(num, dt);
		}

		public void SetMotorTarget(float targetAngle, float dt)
		{
			m_limit.Fit(ref targetAngle);
			float hingeAngle = GetHingeAngle(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
			float num = targetAngle - hingeAngle;
			m_motorTargetVelocity = num / dt;
		}

		public void SetLimit(float low, float high)
		{
			SetLimit(low, high, 0.9f, 0.3f, 1f);
		}

		public void SetLimit(float low, float high, float _softness, float _biasFactor, float _relaxationFactor)
		{
			m_limit.Set(low, high, _softness, _biasFactor, _relaxationFactor);
		}

		public void SetAxis(ref IndexedVector3 axisInA)
		{
			IndexedVector3 p;
			IndexedVector3 q;
			TransformUtil.PlaneSpace1(ref axisInA, out p, out q);
			IndexedVector3 origin = m_rbAFrame._origin;
			m_rbAFrame._basis = new IndexedBasisMatrix(p.X, q.X, axisInA.X, p.Y, q.Y, axisInA.Y, p.Z, q.Z, axisInA.Z);
			IndexedVector3 axisInB = m_rbA.GetCenterOfMassTransform()._basis * axisInA;
			IndexedQuaternion rotation = MathUtil.ShortestArcQuat(ref axisInA, ref axisInB);
			IndexedVector3 v = MathUtil.QuatRotate(ref rotation, ref p);
			IndexedVector3 indexedVector = IndexedVector3.Cross(ref axisInB, ref v);
			m_rbBFrame._origin = m_rbB.GetCenterOfMassTransform().Inverse() * (m_rbA.GetCenterOfMassTransform() * origin);
			m_rbBFrame._basis = new IndexedBasisMatrix(v.X, indexedVector.X, axisInB.X, v.Y, indexedVector.Y, axisInB.Y, v.Z, indexedVector.Z, axisInB.Z);
		}

		public float GetLowerLimit()
		{
			return m_limit.GetLow();
		}

		public float GetUpperLimit()
		{
			return m_limit.GetHigh();
		}

		public float GetHingeAngle()
		{
			return GetHingeAngle(m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
		}

		public float GetHingeAngle(IndexedMatrix transA, IndexedMatrix transB)
		{
			return GetHingeAngle(ref transA, ref transB);
		}

		public float GetHingeAngle(ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			IndexedVector3 b = transA._basis * m_rbAFrame._basis.GetColumn(0);
			IndexedVector3 b2 = transA._basis * m_rbAFrame._basis.GetColumn(1);
			IndexedVector3 a = transB._basis * m_rbBFrame._basis.GetColumn(1);
			b.Normalize();
			b2.Normalize();
			a.Normalize();
			float num = IndexedVector3.Dot(ref a, ref b);
			float num2 = IndexedVector3.Dot(ref a, ref b2);
			float num3 = (float)Math.Atan2(num, num2);
			return m_referenceSign * num3;
		}

		public void TestLimit(IndexedMatrix transA, IndexedMatrix transB)
		{
			TestLimit(ref transA, ref transB);
		}

		public void TestLimit(ref IndexedMatrix transA, ref IndexedMatrix transB)
		{
			m_hingeAngle = GetHingeAngle(ref transA, ref transB);
			m_limit.Test(m_hingeAngle);
		}

		public IndexedMatrix GetAFrame()
		{
			return m_rbAFrame;
		}

		public IndexedMatrix GetBFrame()
		{
			return m_rbBFrame;
		}

		public bool GetSolveLimit()
		{
			return m_limit.IsLimit();
		}

		public float GetLimitSign()
		{
			return m_limit.GetSign();
		}

		public bool GetAngularOnly()
		{
			return m_angularOnly;
		}

		public bool GetEnableAngularMotor()
		{
			return m_enableAngularMotor;
		}

		public float GetMotorTargetVelocity()
		{
			return m_motorTargetVelocity;
		}

		public float GetMaxMotorImpulse()
		{
			return m_maxMotorImpulse;
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
