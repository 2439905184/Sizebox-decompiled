namespace BulletXNA
{
	public class TypedObject
	{
		private int m_objectType;

		public TypedObject(int objectType)
		{
			m_objectType = objectType;
		}

		public int GetObjectType()
		{
			return m_objectType;
		}
	}
}
