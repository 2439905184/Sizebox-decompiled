using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public struct ClosestPointInput
	{
		public IndexedMatrix m_transformA;

		public IndexedMatrix m_transformB;

		public float m_maximumDistanceSquared;

		private static ClosestPointInput _default = new ClosestPointInput(IndexedMatrix.Identity, IndexedMatrix.Identity, 1E+18f);

		public ClosestPointInput(IndexedMatrix ma, IndexedMatrix mb, float dist2)
		{
			m_transformA = ma;
			m_transformB = mb;
			m_maximumDistanceSquared = dist2;
		}

		public static ClosestPointInput Default()
		{
			return _default;
		}
	}
}
