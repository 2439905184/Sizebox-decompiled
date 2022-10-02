using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	internal class LinkedListIndex<TKey, TValue>
	{
		private LinkedList<TValue> m_LinkedList;

		private Dictionary<TKey, LinkedListNode<TValue>> m_Map;

		public LinkedListIndex(LinkedList<TValue> linkedList)
		{
			m_LinkedList = linkedList;
		}

		public LinkedListNode<TValue> Find(TKey key)
		{
			if (m_Map == null)
			{
				return null;
			}
			LinkedListNode<TValue> value;
			if (m_Map.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		public TValue Set(TKey key, TValue value)
		{
			LinkedListNode<TValue> linkedListNode = Find(key);
			if (linkedListNode == null)
			{
				Add(key, value);
				return default(TValue);
			}
			TValue value2 = linkedListNode.Value;
			linkedListNode.Value = value;
			return value2;
		}

		public void Add(TKey key, TValue value)
		{
			LinkedListNode<TValue> value2 = m_LinkedList.AddLast(value);
			if (m_Map == null)
			{
				m_Map = new Dictionary<TKey, LinkedListNode<TValue>>();
			}
			m_Map.Add(key, value2);
		}

		public bool Remove(TKey key)
		{
			LinkedListNode<TValue> linkedListNode = Find(key);
			if (linkedListNode != null)
			{
				m_LinkedList.Remove(linkedListNode);
				return m_Map.Remove(key);
			}
			return false;
		}

		public bool ContainsKey(TKey key)
		{
			if (m_Map == null)
			{
				return false;
			}
			return m_Map.ContainsKey(key);
		}

		public void Clear()
		{
			if (m_Map != null)
			{
				m_Map.Clear();
			}
		}
	}
}
