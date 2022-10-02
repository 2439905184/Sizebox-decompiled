namespace BulletXNA.BulletCollision
{
	public class InternalVertexPair
	{
		public int m_v0;

		public int m_v1;

		public InternalVertexPair(int v0, int v1)
		{
			m_v0 = ((v1 > v0) ? v1 : v0);
			m_v1 = ((v1 > v0) ? v0 : v1);
		}

		public int GetHash()
		{
			return m_v0 + (m_v1 << 16);
		}

		public bool Equals(ref InternalVertexPair other)
		{
			if (m_v0 == other.m_v0)
			{
				return m_v1 == other.m_v1;
			}
			return false;
		}
	}
}
