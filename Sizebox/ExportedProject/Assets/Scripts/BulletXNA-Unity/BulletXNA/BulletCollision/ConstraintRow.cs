using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct ConstraintRow
	{
		public IndexedVector3 m_normal;

		public float m_rhs;

		public float m_jacDiagInv;

		public float m_lowerLimit;

		public float m_upperLimit;

		public float m_accumImpulse;

		public void Reset()
		{
			m_normal = default(IndexedVector3);
			m_rhs = 0f;
			m_jacDiagInv = 0f;
			m_lowerLimit = 0f;
			m_upperLimit = 0f;
			m_accumImpulse = 0f;
		}
	}
}
