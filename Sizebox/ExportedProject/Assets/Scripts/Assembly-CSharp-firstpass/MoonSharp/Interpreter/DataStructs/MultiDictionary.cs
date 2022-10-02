using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	internal class MultiDictionary<K, V>
	{
		private Dictionary<K, List<V>> m_Map;

		private V[] m_DefaultRet = new V[0];

		public IEnumerable<K> Keys
		{
			get
			{
				return m_Map.Keys;
			}
		}

		public MultiDictionary()
		{
			m_Map = new Dictionary<K, List<V>>();
		}

		public MultiDictionary(IEqualityComparer<K> eqComparer)
		{
			m_Map = new Dictionary<K, List<V>>(eqComparer);
		}

		public bool Add(K key, V value)
		{
			List<V> value2;
			if (m_Map.TryGetValue(key, out value2))
			{
				value2.Add(value);
				return false;
			}
			value2 = new List<V>();
			value2.Add(value);
			m_Map.Add(key, value2);
			return true;
		}

		public IEnumerable<V> Find(K key)
		{
			List<V> value;
			if (m_Map.TryGetValue(key, out value))
			{
				return value;
			}
			return m_DefaultRet;
		}

		public bool ContainsKey(K key)
		{
			return m_Map.ContainsKey(key);
		}

		public void Clear()
		{
			m_Map.Clear();
		}

		public void Remove(K key)
		{
			m_Map.Remove(key);
		}

		public bool RemoveValue(K key, V value)
		{
			List<V> value2;
			if (m_Map.TryGetValue(key, out value2))
			{
				value2.Remove(value);
				if (value2.Count == 0)
				{
					Remove(key);
					return true;
				}
			}
			return false;
		}
	}
}
