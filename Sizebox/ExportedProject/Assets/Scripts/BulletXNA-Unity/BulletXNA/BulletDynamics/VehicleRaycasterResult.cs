using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class VehicleRaycasterResult
	{
		public IndexedVector3 m_hitPointInWorld;

		public IndexedVector3 m_hitNormalInWorld;

		public float m_distFraction;

		public VehicleRaycasterResult()
		{
			m_distFraction = -1f;
			m_hitNormalInWorld = IndexedVector3.Zero;
			m_hitPointInWorld = IndexedVector3.Zero;
		}
	}
}
