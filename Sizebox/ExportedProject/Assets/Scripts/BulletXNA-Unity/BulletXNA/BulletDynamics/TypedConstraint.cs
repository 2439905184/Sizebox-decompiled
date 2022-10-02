using System;
using System.Collections.Generic;
using System.IO;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class TypedConstraint : TypedObject
	{
		public const float DEFAULT_DEBUGDRAW_SIZE = 0.3f;

		private static RigidBody s_fixed;

		public string m_debugName;

		private int m_userConstraintType;

		private int m_userConstraintId;

		private object m_userConstraintPtr;

		private float m_breakingImpulseThreshold;

		private bool m_isEnabled;

		private bool m_needsFeedback;

		private int m_overrideNumSolverIterations;

		protected TypedConstraintType m_constraintType;

		protected RigidBody m_rbA;

		protected RigidBody m_rbB;

		protected float m_appliedImpulse;

		protected float m_dbgDrawSize;

		public TypedConstraint()
			: base(-1)
		{
		}

		public TypedConstraint(TypedConstraintType type, RigidBody rbA)
			: base((int)type)
		{
			m_userConstraintType = -1;
			m_userConstraintId = -1;
			m_constraintType = type;
			m_rbA = rbA;
			m_rbB = GetFixedBody();
			m_appliedImpulse = 0f;
			m_breakingImpulseThreshold = float.MaxValue;
			m_isEnabled = true;
			m_dbgDrawSize = 0.3f;
			s_fixed.SetMassProps(0f, IndexedVector3.Zero);
		}

		public TypedConstraint(TypedConstraintType type, RigidBody rbA, RigidBody rbB)
			: base((int)type)
		{
			m_userConstraintType = -1;
			m_userConstraintId = -1;
			m_constraintType = type;
			m_rbA = rbA;
			m_rbB = rbB;
			m_appliedImpulse = 0f;
			m_breakingImpulseThreshold = float.MaxValue;
			m_isEnabled = true;
			m_dbgDrawSize = 0.3f;
			GetFixedBody().SetMassProps(0f, IndexedVector3.Zero);
		}

		public virtual void SetupSolverConstraint(IList<SolverConstraint> ca, int solverBodyA, int solverBodyB, float timeStep)
		{
		}

		public virtual void GetInfo1(ConstraintInfo1 info)
		{
		}

		public virtual void GetInfo2(ConstraintInfo2 info)
		{
		}

		public void InternalSetAppliedImpulse(float appliedImpulse)
		{
			m_appliedImpulse = appliedImpulse;
		}

		public float InternalGetAppliedImpulse()
		{
			return m_appliedImpulse;
		}

		public int GetOverrideNumSolverIterations()
		{
			return m_overrideNumSolverIterations;
		}

		public void SetOverrideNumSolverIterations(int overideNumIterations)
		{
			m_overrideNumSolverIterations = overideNumIterations;
		}

		protected float GetMotorFactor(float pos, float lowLim, float uppLim, float vel, float timeFact)
		{
			if (lowLim > uppLim)
			{
				return 1f;
			}
			if (MathUtil.CompareFloat(lowLim, uppLim))
			{
				return 0f;
			}
			float num = 1f;
			float num2 = vel / timeFact;
			if (num2 < 0f)
			{
				if (pos >= lowLim && pos < lowLim - num2)
				{
					return (lowLim - pos) / num2;
				}
				if (pos < lowLim)
				{
					return 0f;
				}
				return 1f;
			}
			if (num2 > 0f)
			{
				if (pos <= uppLim && pos > uppLim - num2)
				{
					return (uppLim - pos) / num2;
				}
				if (pos > uppLim)
				{
					return 0f;
				}
				return 1f;
			}
			return 0f;
		}

		public static RigidBody GetFixedBody()
		{
			if (s_fixed == null)
			{
				s_fixed = new RigidBody(0f, null, null, IndexedVector3.Zero);
			}
			IndexedVector3 inertia = IndexedVector3.Zero;
			s_fixed.SetMassProps(0f, ref inertia);
			return s_fixed;
		}

		public RigidBody GetRigidBodyA()
		{
			return m_rbA;
		}

		public RigidBody GetRigidBodyB()
		{
			return m_rbB;
		}

		public int GetUserConstraintType()
		{
			return m_userConstraintType;
		}

		public void SetUserConstraintType(int userConstraintType)
		{
			m_userConstraintType = userConstraintType;
		}

		public void SetUserConstraintId(int uid)
		{
			m_userConstraintId = uid;
		}

		public int GetUserConstraintId()
		{
			return m_userConstraintId;
		}

		public int GetUid()
		{
			return m_userConstraintId;
		}

		public bool NeedsFeedback()
		{
			return m_needsFeedback;
		}

		public void EnableFeedback(bool needsFeedback)
		{
			m_needsFeedback = needsFeedback;
		}

		public float GetAppliedImpulse()
		{
			return m_appliedImpulse;
		}

		public TypedConstraintType GetConstraintType()
		{
			return m_constraintType;
		}

		public void SetDbgDrawSize(float dbgDrawSize)
		{
			m_dbgDrawSize = dbgDrawSize;
		}

		public float GetDbgDrawSize()
		{
			return m_dbgDrawSize;
		}

		public float AdjustAngleToLimits(float angleInRadians, float angleLowerLimitInRadians, float angleUpperLimitInRadians)
		{
			if (angleLowerLimitInRadians >= angleUpperLimitInRadians)
			{
				return angleInRadians;
			}
			if (angleInRadians < angleLowerLimitInRadians)
			{
				float num = Math.Abs(MathUtil.NormalizeAngle(angleLowerLimitInRadians - angleInRadians));
				float num2 = Math.Abs(MathUtil.NormalizeAngle(angleUpperLimitInRadians - angleInRadians));
				if (!(num < num2))
				{
					return angleInRadians + (float)Math.PI * 2f;
				}
				return angleInRadians;
			}
			if (angleInRadians > angleUpperLimitInRadians)
			{
				float num3 = Math.Abs(MathUtil.NormalizeAngle(angleInRadians - angleUpperLimitInRadians));
				float num4 = Math.Abs(MathUtil.NormalizeAngle(angleInRadians - angleLowerLimitInRadians));
				if (!(num4 < num3))
				{
					return angleInRadians;
				}
				return angleInRadians - (float)Math.PI * 2f;
			}
			return angleInRadians;
		}

		public virtual void Cleanup()
		{
		}

		public float GetBreakingImpulseThreshold()
		{
			return m_breakingImpulseThreshold;
		}

		public void SetBreakingImpulseThreshold(float threshold)
		{
			m_breakingImpulseThreshold = threshold;
		}

		public bool IsEnabled()
		{
			return m_isEnabled;
		}

		public void SetEnabled(bool enabled)
		{
			m_isEnabled = enabled;
		}

		public virtual void SolveConstraintObsolete(RigidBody bodyA, RigidBody bodyB, float timeStep)
		{
		}

		public virtual void BuildJacobian()
		{
		}

		public static void PrintInfo1(StreamWriter writer, TypedConstraint constraint, ConstraintInfo1 info)
		{
			if (writer != null)
			{
				writer.WriteLine("getInfo1 [{0}] [{1}] [{2}] [{3}]", constraint.m_userConstraintId, constraint.GetObjectType(), (string)constraint.GetRigidBodyA().GetUserPointer(), (string)constraint.GetRigidBodyB().GetUserPointer());
				MathUtil.PrintMatrix(writer, "rBA cmot", constraint.GetRigidBodyA().GetCenterOfMassTransform());
				MathUtil.PrintMatrix(writer, "rBB cmot", constraint.GetRigidBodyB().GetCenterOfMassTransform());
				MathUtil.PrintMatrix(writer, "rBA inv tensor", constraint.GetRigidBodyA().GetInvInertiaTensorWorld());
				MathUtil.PrintMatrix(writer, "rBB inv tensor", constraint.GetRigidBodyB().GetInvInertiaTensorWorld());
				writer.WriteLine(string.Format("NumRows [{0}] Nub[{1}]", info.m_numConstraintRows, info.nub));
			}
		}

		public static void PrintInfo2(StreamWriter writer, TypedConstraint constraint, ConstraintInfo2 info2)
		{
			if (writer != null)
			{
				writer.WriteLine(string.Format("getInfo2 [{0}] [{1}] [{2}] [{3}]", constraint.m_userConstraintId, constraint.GetObjectType(), (string)constraint.GetRigidBodyA().GetUserPointer(), (string)constraint.GetRigidBodyB().GetUserPointer()));
				writer.WriteLine(string.Format("numRows [{0}] fps[{1:0.00000000}] erp[{2:0.00000000}] findex[{3}] numIter[{4}]", info2.m_numRows, info2.fps, info2.erp, info2.findex, info2.m_numIterations));
				for (int i = 0; i < info2.m_numRows; i++)
				{
					writer.WriteLine(string.Format("TypedConstraint[{0}]", i));
					writer.WriteLine("ContactNormal");
					MathUtil.PrintVector3(writer, info2.m_solverConstraints[i].m_contactNormal);
					writer.WriteLine("rel1pos1CrossNormal");
					MathUtil.PrintVector3(writer, info2.m_solverConstraints[i].m_relpos1CrossNormal);
					writer.WriteLine("rel1pos2CrossNormal");
					MathUtil.PrintVector3(writer, info2.m_solverConstraints[i].m_relpos2CrossNormal);
				}
			}
		}

		public static void PrintSolverConstraint(StreamWriter writer, SolverConstraint constraint, int index)
		{
			if (writer != null)
			{
				writer.WriteLine("SolverConstraint[{0}][{1}][{2}]", index, (string)constraint.m_solverBodyA.GetUserPointer(), (string)constraint.m_solverBodyB.GetUserPointer());
				MathUtil.PrintVector3(writer, "relPos1CrossNormal", constraint.m_relpos1CrossNormal);
				MathUtil.PrintVector3(writer, "contactNormal", constraint.m_contactNormal);
				MathUtil.PrintVector3(writer, "m_angularComponentA", constraint.m_angularComponentA);
				MathUtil.PrintVector3(writer, "m_angularComponentB", constraint.m_angularComponentB);
				writer.WriteLine("Friction [{0:0.00000000}] jagDiag[{1:0.00000000}] rhs[{2:0.00000000}] cfm[{3:0.00000000}] lower[{4:0.00000000}] upper[{5:0.00000000}] rhsPen[{6:0.00000000}]", constraint.m_friction, constraint.m_jacDiagABInv, constraint.m_rhs, constraint.m_cfm, constraint.m_lowerLimit, constraint.m_lowerLimit, constraint.m_rhsPenetration);
			}
		}
	}
}
