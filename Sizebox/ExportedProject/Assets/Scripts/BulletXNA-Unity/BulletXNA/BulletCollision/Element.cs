using System;

namespace BulletXNA.BulletCollision
{
	public struct Element : IComparable<Element>
	{
		public int m_id;

		public int m_sz;

		public int CompareTo(Element obj)
		{
			return m_id - obj.m_id;
		}

		public override string ToString()
		{
			return "id = " + m_id + " , sz = " + m_sz;
		}
	}
}
