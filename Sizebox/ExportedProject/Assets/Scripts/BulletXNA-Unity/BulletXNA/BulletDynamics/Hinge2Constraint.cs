using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class Hinge2Constraint : Generic6DofSpringConstraint
	{
		protected IndexedVector3 m_anchor;

		protected IndexedVector3 m_axis1;

		protected IndexedVector3 m_axis2;

		public Hinge2Constraint(RigidBody rbA, RigidBody rbB, ref IndexedVector3 anchor, ref IndexedVector3 axis1, ref IndexedVector3 axis2)
			: base(rbA, rbB, IndexedMatrix.Identity, IndexedMatrix.Identity, true)
		{
			m_anchor = anchor;
			m_axis1 = axis1;
			m_axis2 = axis2;
			IndexedVector3 v = IndexedVector3.Normalize(axis1);
			IndexedVector3 v2 = IndexedVector3.Normalize(axis2);
			IndexedVector3 indexedVector = IndexedVector3.Cross(v, v2);
			IndexedMatrix identity = IndexedMatrix.Identity;
			identity._basis = new IndexedBasisMatrix(v2.X, indexedVector.X, v.X, v2.Y, indexedVector.Y, v.Y, v2.Z, indexedVector.Z, v.Z);
			identity._origin = anchor;
			m_frameInA = rbA.GetCenterOfMassTransform().Inverse() * identity;
			m_frameInB = rbB.GetCenterOfMassTransform().Inverse() * identity;
			SetLinearLowerLimit(new IndexedVector3(0f, 0f, -1f));
			SetLinearUpperLimit(new IndexedVector3(0f, 0f, 1f));
			SetAngularLowerLimit(new IndexedVector3(1f, 0f, -(float)Math.PI / 4f));
			SetAngularUpperLimit(new IndexedVector3(-1f, 0f, (float)Math.PI / 4f));
			EnableSpring(2, true);
			SetStiffness(2, 39.47842f);
			SetDamping(2, 0.01f);
			SetEquilibriumPoint();
		}

		public IndexedVector3 GetAnchor()
		{
			return m_calculatedTransformA._origin;
		}

		public IndexedVector3 GetAnchor2()
		{
			return m_calculatedTransformB._origin;
		}

		public IndexedVector3 GetAxis1()
		{
			return m_axis1;
		}

		public IndexedVector3 GetAxis2()
		{
			return m_axis2;
		}

		public float GetAngle1()
		{
			return GetAngle(2);
		}

		public float GetAngle2()
		{
			return GetAngle(0);
		}

		public void SetUpperLimit(float ang1max)
		{
			SetAngularUpperLimit(new IndexedVector3(-1f, 0f, ang1max));
		}

		public void SetLowerLimit(float ang1min)
		{
			SetAngularLowerLimit(new IndexedVector3(1f, 0f, ang1min));
		}
	}
}
