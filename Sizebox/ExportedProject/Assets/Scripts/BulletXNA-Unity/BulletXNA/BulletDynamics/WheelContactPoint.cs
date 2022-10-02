using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class WheelContactPoint
	{
		public RigidBody m_body0;

		public RigidBody m_body1;

		public IndexedVector3 m_frictionPositionWorld;

		public IndexedVector3 m_frictionDirectionWorld;

		public float m_jacDiagABInv;

		public float m_maxImpulse;

		public WheelContactPoint(RigidBody body0, RigidBody body1, ref IndexedVector3 frictionPosWorld, ref IndexedVector3 frictionDirectionWorld, float maxImpulse)
		{
			m_body0 = body0;
			m_body1 = body1;
			m_frictionPositionWorld = frictionPosWorld;
			m_frictionDirectionWorld = frictionDirectionWorld;
			m_maxImpulse = maxImpulse;
			float num = body0.ComputeImpulseDenominator(ref frictionPosWorld, ref frictionDirectionWorld);
			float num2 = body1.ComputeImpulseDenominator(ref frictionPosWorld, ref frictionDirectionWorld);
			float num3 = 1f;
			m_jacDiagABInv = num3 / (num + num2);
		}
	}
}
