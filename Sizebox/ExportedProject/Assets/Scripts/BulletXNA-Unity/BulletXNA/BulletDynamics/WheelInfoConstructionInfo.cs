using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public struct WheelInfoConstructionInfo
	{
		public IndexedVector3 m_chassisConnectionCS;

		public IndexedVector3 m_wheelDirectionCS;

		public IndexedVector3 m_wheelAxleCS;

		public float m_suspensionRestLength;

		public float m_maxSuspensionTravelCm;

		public float m_wheelRadius;

		public float m_suspensionStiffness;

		public float m_wheelsDampingCompression;

		public float m_wheelsDampingRelaxation;

		public float m_frictionSlip;

		public float m_maxSuspensionForce;

		public bool m_bIsFrontWheel;
	}
}
