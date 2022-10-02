namespace BulletXNA.BulletCollision
{
	public class ProfileBlock
	{
		public ulong m_total;

		public ulong m_ddcollide;

		public ulong m_fdcollide;

		public ulong m_cleanup;

		public ulong m_jobcount;

		public void clear()
		{
			m_total = 0uL;
			m_ddcollide = 0uL;
			m_fdcollide = 0uL;
			m_cleanup = 0uL;
			m_jobcount = 0uL;
		}
	}
}
