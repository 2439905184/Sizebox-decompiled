using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class UniversalConstraint : Generic6DofConstraint
	{
		private const float UNIV_EPS = 0.01f;

		protected IndexedVector3 m_anchor;

		protected IndexedVector3 m_axis1;

		protected IndexedVector3 m_axis2;

		public UniversalConstraint(RigidBody rbA, RigidBody rbB, ref IndexedVector3 anchor, ref IndexedVector3 axis1, ref IndexedVector3 axis2)
			: base(rbA, rbB, ref BulletGlobals.IdentityMatrix, ref BulletGlobals.IdentityMatrix, true)
		{
			m_anchor = anchor;
			m_axis1 = axis1;
			m_axis2 = axis2;
			IndexedVector3 v = IndexedVector3.Normalize(m_axis1);
			IndexedVector3 v2 = IndexedVector3.Normalize(m_axis2);
			IndexedVector3 indexedVector = IndexedVector3.Cross(v2, v);
			IndexedMatrix identity = IndexedMatrix.Identity;
			identity._basis = new IndexedBasisMatrix(indexedVector.X, v2.X, v.X, indexedVector.Y, v2.Y, v.Y, indexedVector.Z, v2.Z, v.Z);
			identity._origin = anchor;
			m_frameInA = rbA.GetCenterOfMassTransform().Inverse() * identity;
			m_frameInB = rbB.GetCenterOfMassTransform().Inverse() * identity;
			SetLinearLowerLimit(IndexedVector3.Zero);
			SetLinearUpperLimit(IndexedVector3.Zero);
			SetAngularLowerLimit(new IndexedVector3(0f, -1.5607964f, -3.1315928f));
			SetAngularUpperLimit(new IndexedVector3(0f, 1.5607964f, 3.1315928f));
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
			return GetAngle(1);
		}

		public void SetUpperLimit(float ang1max, float ang2max)
		{
			SetAngularUpperLimit(new IndexedVector3(0f, ang1max, ang2max));
		}

		public void SetLowerLimit(float ang1min, float ang2min)
		{
			SetAngularLowerLimit(new IndexedVector3(0f, ang1min, ang2min));
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
