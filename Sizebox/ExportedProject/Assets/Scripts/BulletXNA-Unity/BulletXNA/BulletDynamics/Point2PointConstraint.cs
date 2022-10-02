using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class Point2PointConstraint : TypedConstraint
	{
		public JacobianEntry[] m_jac = new JacobianEntry[3];

		public IndexedVector3 m_pivotInA;

		public IndexedVector3 m_pivotInB;

		public Point2PointFlags m_flags;

		public float m_erp;

		public float m_cfm;

		public ConstraintSetting m_setting = new ConstraintSetting();

		public Point2PointConstraint(RigidBody rbA, RigidBody rbB, ref IndexedVector3 pivotInA, ref IndexedVector3 pivotInB)
			: base(TypedConstraintType.POINT2POINT_CONSTRAINT_TYPE, rbA, rbB)
		{
			m_pivotInA = pivotInA;
			m_pivotInB = pivotInB;
		}

		public Point2PointConstraint(RigidBody rbA, ref IndexedVector3 pivotInA)
			: base(TypedConstraintType.POINT2POINT_CONSTRAINT_TYPE, rbA)
		{
			m_pivotInA = pivotInA;
			m_pivotInB = rbA.GetCenterOfMassTransform() * pivotInA;
		}

		public override void GetInfo1(ConstraintInfo1 info)
		{
			GetInfo1NonVirtual(info);
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			info.m_numConstraintRows = 3;
			info.nub = 3;
		}

		public override void GetInfo2(ConstraintInfo2 info)
		{
			GetInfo2NonVirtual(info, m_rbA.GetCenterOfMassTransform(), m_rbB.GetCenterOfMassTransform());
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, IndexedMatrix body0_trans, IndexedMatrix body1_trans)
		{
			info.m_solverConstraints[0].m_contactNormal.X = 1f;
			info.m_solverConstraints[1].m_contactNormal.Y = 1f;
			info.m_solverConstraints[2].m_contactNormal.Z = 1f;
			IndexedVector3 indexedVector = body0_trans._basis * GetPivotInA();
			IndexedVector3 vecin = -indexedVector;
			MathUtil.GetSkewSymmetricMatrix(ref vecin, out info.m_solverConstraints[0].m_relpos1CrossNormal, out info.m_solverConstraints[1].m_relpos1CrossNormal, out info.m_solverConstraints[2].m_relpos1CrossNormal);
			IndexedVector3 vecin2 = body1_trans._basis * GetPivotInB();
			IndexedVector3 indexedVector2 = -vecin2;
			MathUtil.GetSkewSymmetricMatrix(ref vecin2, out info.m_solverConstraints[0].m_relpos2CrossNormal, out info.m_solverConstraints[1].m_relpos2CrossNormal, out info.m_solverConstraints[2].m_relpos2CrossNormal);
			float num = (((m_flags & Point2PointFlags.BT_P2P_FLAGS_ERP) != 0) ? m_erp : info.erp);
			float num2 = info.fps * num;
			IndexedVector3 origin = body0_trans._origin;
			IndexedVector3 origin2 = body1_trans._origin;
			for (int i = 0; i < 3; i++)
			{
				info.m_solverConstraints[i].m_rhs = num2 * (vecin2[i] + origin2[i] - indexedVector[i] - origin[i]);
			}
			if ((m_flags & Point2PointFlags.BT_P2P_FLAGS_CFM) != 0)
			{
				for (int i = 0; i < 3; i++)
				{
					info.m_solverConstraints[i].m_cfm = m_cfm;
				}
			}
			float impulseClamp = m_setting.m_impulseClamp;
			for (int i = 0; i < 3; i++)
			{
				if (m_setting.m_impulseClamp > 0f)
				{
					info.m_solverConstraints[i].m_lowerLimit = 0f - impulseClamp;
					info.m_solverConstraints[i].m_upperLimit = impulseClamp;
				}
			}
			info.m_damping = m_setting.m_damping;
		}

		public void UpdateRHS(float timeStep)
		{
		}

		public void SetPivotA(ref IndexedVector3 pivotA)
		{
			m_pivotInA = pivotA;
		}

		public void SetPivotB(ref IndexedVector3 pivotB)
		{
			m_pivotInB = pivotB;
		}

		public IndexedVector3 GetPivotInA()
		{
			return m_pivotInA;
		}

		public IndexedVector3 GetPivotInB()
		{
			return m_pivotInB;
		}
	}
}
