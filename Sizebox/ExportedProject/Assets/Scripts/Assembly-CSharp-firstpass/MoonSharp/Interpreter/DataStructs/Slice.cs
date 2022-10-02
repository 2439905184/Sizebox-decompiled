using System;
using System.Collections;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	internal class Slice<T> : IEnumerable<T>, IEnumerable, IList<T>, ICollection<T>
	{
		private IList<T> m_SourceList;

		private int m_From;

		private int m_Length;

		private bool m_Reversed;

		public T this[int index]
		{
			get
			{
				return m_SourceList[CalcRealIndex(index)];
			}
			set
			{
				m_SourceList[CalcRealIndex(index)] = value;
			}
		}

		public int From
		{
			get
			{
				return m_From;
			}
		}

		public int Count
		{
			get
			{
				return m_Length;
			}
		}

		public bool Reversed
		{
			get
			{
				return m_Reversed;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public Slice(IList<T> list, int from, int length, bool reversed)
		{
			m_SourceList = list;
			m_From = from;
			m_Length = length;
			m_Reversed = reversed;
		}

		private int CalcRealIndex(int index)
		{
			if (index < 0 || index >= m_Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (m_Reversed)
			{
				return m_From + m_Length - index - 1;
			}
			return m_From + index;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < m_Length; i++)
			{
				yield return m_SourceList[CalcRealIndex(i)];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < m_Length; i++)
			{
				yield return m_SourceList[CalcRealIndex(i)];
			}
		}

		public T[] ToArray()
		{
			T[] array = new T[m_Length];
			for (int i = 0; i < m_Length; i++)
			{
				array[i] = m_SourceList[CalcRealIndex(i)];
			}
			return array;
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>(m_Length);
			for (int i = 0; i < m_Length; i++)
			{
				list.Add(m_SourceList[CalcRealIndex(i)]);
			}
			return list;
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].Equals(item))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			throw new InvalidOperationException("Slices are readonly");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("Slices are readonly");
		}

		public void Add(T item)
		{
			throw new InvalidOperationException("Slices are readonly");
		}

		public void Clear()
		{
			throw new InvalidOperationException("Slices are readonly");
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++)
			{
				array[i + arrayIndex] = this[i];
			}
		}

		public bool Remove(T item)
		{
			throw new InvalidOperationException("Slices are readonly");
		}
	}
}
