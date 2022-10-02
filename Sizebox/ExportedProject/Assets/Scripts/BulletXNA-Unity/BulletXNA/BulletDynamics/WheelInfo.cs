using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class WheelInfo
	{
		public WheelRaycastInfo m_raycastInfo;

		public IndexedMatrix m_worldTransform;

		public IndexedVector3 m_chassisConnectionPointCS;

		public IndexedVector3 m_wheelDirectionCS;

		public IndexedVector3 m_wheelAxleCS;

		public float m_suspensionRestLength1;

		public float m_maxSuspensionTravelCm;

		public float m_wheelsRadius;

		public float m_suspensionStiffness;

		public float m_wheelsDampingCompression;

		public float m_wheelsDampingRelaxation;

		public float m_frictionSlip;

		public float m_steering;

		public float m_rotation;

		public float m_deltaRotation;

		public float m_rollInfluence;

		public float m_maxSuspensionForce;

		public float m_engineForce;

		public float m_brake;

		public bool m_bIsFrontWheel;

		public object m_clientInfo;

		public float m_clippedInvContactDotSuspension;

		public float m_suspensionRelativeVelocity;

		public float m_wheelsSuspensionForce;

		public float m_skidInfo;

		public float GetSuspensionRestLength()
		{
			return m_suspensionRestLength1;
		}

		public WheelInfo(ref WheelInfoConstructionInfo ci)
		{
			m_suspensionRestLength1 = ci.m_suspensionRestLength;
			m_maxSuspensionTravelCm = ci.m_maxSuspensionTravelCm;
			m_wheelsRadius = ci.m_wheelRadius;
			m_suspensionStiffness = ci.m_suspensionStiffness;
			m_wheelsDampingCompression = ci.m_wheelsDampingCompression;
			m_wheelsDampingRelaxation = ci.m_wheelsDampingRelaxation;
			m_chassisConnectionPointCS = ci.m_chassisConnectionCS;
			m_wheelDirectionCS = ci.m_wheelDirectionCS;
			m_wheelAxleCS = ci.m_wheelAxleCS;
			m_frictionSlip = ci.m_frictionSlip;
			m_steering = 0f;
			m_engineForce = 0f;
			m_rotation = 0f;
			m_deltaRotation = 0f;
			m_brake = 0f;
			m_rollInfluence = 0.1f;
			m_bIsFrontWheel = ci.m_bIsFrontWheel;
			m_maxSuspensionForce = ci.m_maxSuspensionForce;
		}

		public void UpdateWheel(RigidBody chassis, ref WheelRaycastInfo raycastInfo)
		{
			if (m_raycastInfo.m_isInContact)
			{
				float num = IndexedVector3.Dot(m_raycastInfo.m_contactNormalWS, m_raycastInfo.m_wheelDirectionWS);
				IndexedVector3 rel_pos = m_raycastInfo.m_contactPointWS - chassis.GetCenterOfMassPosition();
				IndexedVector3 velocityInLocalPoint = chassis.GetVelocityInLocalPoint(ref rel_pos);
				float num2 = IndexedVector3.Dot(m_raycastInfo.m_contactNormalWS, velocityInLocalPoint);
				if (num >= -0.1f)
				{
					m_suspensionRelativeVelocity = 0f;
					m_clippedInvContactDotSuspension = 10f;
				}
				else
				{
					float num3 = -1f / num;
					m_suspensionRelativeVelocity = num2 * num3;
					m_clippedInvContactDotSuspension = num3;
				}
			}
			else
			{
				m_raycastInfo.m_suspensionLength = GetSuspensionRestLength();
				m_suspensionRelativeVelocity = 0f;
				m_raycastInfo.m_contactNormalWS = -m_raycastInfo.m_wheelDirectionWS;
				m_clippedInvContactDotSuspension = 1f;
			}
		}
	}
}
