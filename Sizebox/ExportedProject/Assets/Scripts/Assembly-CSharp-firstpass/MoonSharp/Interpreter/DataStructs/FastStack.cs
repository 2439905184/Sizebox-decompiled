using System;
using System.Collections;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.DataStructs
{
	internal class FastStack<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private T[] m_Storage;

		private int m_HeadIdx;

		public T this[int index]
		{
			get
			{
				return m_Storage[index];
			}
			set
			{
				m_Storage[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return m_HeadIdx;
			}
		}

		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = value;
			}
		}

		int ICollection<T>.Count
		{
			get
			{
				return Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public FastStack(int maxCapacity)
		{
			m_Storage = new T[maxCapacity];
		}

		public T Push(T item)
		{
			m_Storage[m_HeadIdx++] = item;
			return item;
		}

		public void Expand(int size)
		{
			m_HeadIdx += size;
		}

		private void Zero(int from, int to)
		{
			Array.Clear(m_Storage, from, to - from + 1);
		}

		private void Zero(int index)
		{
			m_Storage[index] = default(T);
		}

		public T Peek(int idxofs = 0)
		{
			return m_Storage[m_HeadIdx - 1 - idxofs];
		}

		public void Set(int idxofs, T item)
		{
			m_Storage[m_HeadIdx - 1 - idxofs] = item;
		}

		public void CropAtCount(int p)
		{
			RemoveLast(Count - p);
		}

		public void RemoveLast(int cnt = 1)
		{
			if (cnt == 1)
			{
				m_HeadIdx--;
				m_Storage[m_HeadIdx] = default(T);
			}
			else
			{
				int headIdx = m_HeadIdx;
				m_HeadIdx -= cnt;
				Zero(m_HeadIdx, headIdx);
			}
		}

		public T Pop()
		{
			m_HeadIdx--;
			T result = m_Storage[m_HeadIdx];
			m_Storage[m_HeadIdx] = default(T);
			return result;
		}

		public void Clear()
		{
			Array.Clear(m_Storage, 0, m_Storage.Length);
			m_HeadIdx = 0;
		}

		int IList<T>.IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		void ICollection<T>.Add(T item)
		{
			Push(item);
		}

		void ICollection<T>.Clear()
		{
			Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			throw new NotImplementedException();
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
