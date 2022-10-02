using System.Collections.Generic;

namespace BulletXNA.LinearMath
{
	public class PooledType<T> where T : new()
	{
		private Stack<T> m_pool = new Stack<T>();

		public T Get()
		{
			if (m_pool.Count == 0)
			{
				m_pool.Push(new T());
			}
			return m_pool.Pop();
		}

		public void Free(T obj)
		{
			m_pool.Push(obj);
		}
	}
}
