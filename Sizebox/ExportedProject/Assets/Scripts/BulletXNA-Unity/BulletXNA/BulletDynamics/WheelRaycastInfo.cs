using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public struct WheelRaycastInfo
	{
		public IndexedVector3 m_contactNormalWS;

		public IndexedVector3 m_contactPointWS;

		public float m_suspensionLength;

		public float m_suspensionLengthBak;

		public IndexedVector3 m_hardPointWS;

		public IndexedVector3 m_hardPointWSBak;

		public IndexedVector3 m_wheelDirectionWS;

		public IndexedVector3 m_wheelDirectionWSBak;

		public IndexedVector3 m_wheelAxleWS;

		public IndexedVector3 m_wheelAxleWSBak;

		public bool m_isInContact;

		public object m_groundObject;
	}
}
