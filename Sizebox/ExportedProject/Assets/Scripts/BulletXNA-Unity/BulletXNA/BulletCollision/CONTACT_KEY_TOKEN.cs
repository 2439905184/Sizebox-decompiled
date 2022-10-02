namespace BulletXNA.BulletCollision
{
	public class CONTACT_KEY_TOKEN
	{
		public uint m_key;

		public int m_value;

		public CONTACT_KEY_TOKEN()
		{
		}

		public CONTACT_KEY_TOKEN(uint key, int token)
		{
			m_key = key;
			m_value = token;
		}

		public CONTACT_KEY_TOKEN(CONTACT_KEY_TOKEN rtoken)
		{
			m_key = rtoken.m_key;
			m_value = rtoken.m_value;
		}

		public static int SortPredicate(CONTACT_KEY_TOKEN lhs, CONTACT_KEY_TOKEN rhs)
		{
			if (lhs.m_key >= rhs.m_key)
			{
				return -1;
			}
			return 1;
		}
	}
}
