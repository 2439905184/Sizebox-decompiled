namespace BulletXNA.BulletCollision
{
	public struct GIM_PAIR
	{
		public int m_index1;

		public int m_index2;

		public GIM_PAIR(ref GIM_PAIR p)
		{
			m_index1 = p.m_index1;
			m_index2 = p.m_index2;
		}

		public GIM_PAIR(int index1, int index2)
		{
			m_index1 = index1;
			m_index2 = index2;
		}
	}
}
