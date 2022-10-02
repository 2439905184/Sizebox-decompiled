namespace BulletXNA.BulletCollision
{
	public class Material
	{
		public float m_friction;

		public float m_restitution;

		public Material(float friction, float restitution)
		{
			m_friction = friction;
			m_restitution = restitution;
		}
	}
}
