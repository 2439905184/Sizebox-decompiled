using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class JacobianEntry
	{
		public IndexedVector3 m_linearJointAxis;

		public IndexedVector3 m_aJ;

		public IndexedVector3 m_bJ;

		public IndexedVector3 m_0MinvJt;

		public IndexedVector3 m_1MinvJt;

		public float m_Adiag;

		public JacobianEntry()
		{
		}

		public JacobianEntry(IndexedBasisMatrix world2A, IndexedBasisMatrix world2B, IndexedVector3 rel_pos1, IndexedVector3 rel_pos2, IndexedVector3 jointAxis, IndexedVector3 inertiaInvA, float massInvA, IndexedVector3 inertiaInvB, float massInvB)
			: this(ref world2A, ref world2B, ref rel_pos1, ref rel_pos2, ref jointAxis, ref inertiaInvA, massInvA, ref inertiaInvB, massInvB)
		{
		}

		public JacobianEntry(ref IndexedBasisMatrix world2A, ref IndexedBasisMatrix world2B, ref IndexedVector3 rel_pos1, ref IndexedVector3 rel_pos2, ref IndexedVector3 jointAxis, ref IndexedVector3 inertiaInvA, float massInvA, ref IndexedVector3 inertiaInvB, float massInvB)
		{
			m_linearJointAxis = jointAxis;
			m_aJ = world2A * rel_pos1.Cross(ref m_linearJointAxis);
			m_bJ = world2B * rel_pos2.Cross(-m_linearJointAxis);
			m_0MinvJt = inertiaInvA * m_aJ;
			m_1MinvJt = inertiaInvB * m_bJ;
			m_Adiag = massInvA + IndexedVector3.Dot(m_0MinvJt, m_aJ) + massInvB + IndexedVector3.Dot(m_1MinvJt, m_bJ);
		}

		public JacobianEntry(IndexedVector3 jointAxis, IndexedBasisMatrix world2A, IndexedBasisMatrix world2B, IndexedVector3 inertiaInvA, IndexedVector3 inertiaInvB)
			: this(ref jointAxis, ref world2A, ref world2B, ref inertiaInvA, ref inertiaInvB)
		{
		}

		public JacobianEntry(ref IndexedVector3 jointAxis, ref IndexedBasisMatrix world2A, ref IndexedBasisMatrix world2B, ref IndexedVector3 inertiaInvA, ref IndexedVector3 inertiaInvB)
		{
			m_linearJointAxis = IndexedVector3.Zero;
			m_aJ = world2A * jointAxis;
			m_bJ = world2B * -jointAxis;
			m_0MinvJt = inertiaInvA * m_aJ;
			m_1MinvJt = inertiaInvB * m_bJ;
			m_Adiag = IndexedVector3.Dot(m_0MinvJt, m_aJ) + IndexedVector3.Dot(m_1MinvJt, m_bJ);
		}

		public JacobianEntry(IndexedVector3 axisInA, IndexedVector3 axisInB, IndexedVector3 inertiaInvA, IndexedVector3 inertiaInvB)
			: this(ref axisInA, ref axisInB, ref inertiaInvA, ref inertiaInvB)
		{
		}

		public JacobianEntry(ref IndexedVector3 axisInA, ref IndexedVector3 axisInB, ref IndexedVector3 inertiaInvA, ref IndexedVector3 inertiaInvB)
		{
			m_linearJointAxis = IndexedVector3.Zero;
			m_aJ = axisInA;
			m_bJ = -axisInB;
			m_0MinvJt = inertiaInvA * m_aJ;
			m_1MinvJt = inertiaInvB * m_bJ;
			m_Adiag = IndexedVector3.Dot(m_0MinvJt, m_aJ) + IndexedVector3.Dot(m_1MinvJt, m_bJ);
		}

		public JacobianEntry(IndexedBasisMatrix world2A, IndexedVector3 rel_pos1, IndexedVector3 rel_pos2, IndexedVector3 jointAxis, IndexedVector3 inertiaInvA, float massInvA)
			: this(ref world2A, ref rel_pos1, ref rel_pos2, ref jointAxis, ref inertiaInvA, massInvA)
		{
		}

		public JacobianEntry(ref IndexedBasisMatrix world2A, ref IndexedVector3 rel_pos1, ref IndexedVector3 rel_pos2, ref IndexedVector3 jointAxis, ref IndexedVector3 inertiaInvA, float massInvA)
		{
			m_linearJointAxis = jointAxis;
			m_aJ = world2A * rel_pos1.Cross(ref jointAxis);
			m_bJ = world2A * rel_pos2.Cross(-jointAxis);
			m_0MinvJt = inertiaInvA * m_aJ;
			m_1MinvJt = IndexedVector3.Zero;
			m_Adiag = massInvA + IndexedVector3.Dot(m_0MinvJt, m_aJ);
		}

		public float GetDiagonal()
		{
			return m_Adiag;
		}

		public float GetNonDiagonal(JacobianEntry jacB, float massInvA)
		{
			float num = massInvA * IndexedVector3.Dot(m_linearJointAxis, jacB.m_linearJointAxis);
			float num2 = IndexedVector3.Dot(m_0MinvJt, jacB.m_aJ);
			return num + num2;
		}

		public float GetNonDiagonal(JacobianEntry jacB, float massInvA, float massInvB)
		{
			IndexedVector3 indexedVector = m_linearJointAxis * jacB.m_linearJointAxis;
			IndexedVector3 indexedVector2 = m_0MinvJt * jacB.m_aJ;
			IndexedVector3 indexedVector3 = m_1MinvJt * jacB.m_bJ;
			IndexedVector3 indexedVector4 = massInvA * indexedVector;
			IndexedVector3 indexedVector5 = massInvB * indexedVector;
			IndexedVector3 indexedVector6 = indexedVector2 + indexedVector3 + indexedVector4 + indexedVector5;
			return indexedVector6.X + indexedVector6.Y + indexedVector6.Z;
		}

		public float GetRelativeVelocity(IndexedVector3 linvelA, IndexedVector3 angvelA, IndexedVector3 linvelB, IndexedVector3 angvelB)
		{
			return GetRelativeVelocity(ref linvelA, ref angvelA, ref linvelB, ref angvelB);
		}

		public float GetRelativeVelocity(ref IndexedVector3 linvelA, ref IndexedVector3 angvelA, ref IndexedVector3 linvelB, ref IndexedVector3 angvelB)
		{
			IndexedVector3 indexedVector = linvelA - linvelB;
			IndexedVector3 indexedVector2 = angvelA * m_aJ;
			IndexedVector3 indexedVector3 = angvelB * m_bJ;
			indexedVector *= m_linearJointAxis;
			indexedVector2 += indexedVector3;
			indexedVector2 += indexedVector;
			float num = indexedVector2.X + indexedVector2.Y + indexedVector2.Z;
			return num + 1.1920929E-07f;
		}
	}
}
