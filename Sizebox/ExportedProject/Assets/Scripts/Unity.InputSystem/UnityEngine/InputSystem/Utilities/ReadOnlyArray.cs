using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	public struct ReadOnlyArray<TValue> : IReadOnlyList<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyCollection<TValue>
	{
		internal class Enumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
		{
			private readonly T[] m_Array;

			private readonly int m_IndexStart;

			private readonly int m_IndexEnd;

			private int m_Index;

			public T Current
			{
				get
				{
					if (m_Index == m_IndexEnd)
					{
						throw new InvalidOperationException("Iterated beyond end");
					}
					return m_Array[m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public Enumerator(T[] array, int index, int length)
			{
				m_Array = array;
				m_IndexStart = index - 1;
				m_IndexEnd = index + length;
				m_Index = m_IndexStart;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (m_Index < m_IndexEnd)
				{
					m_Index++;
				}
				return m_Index != m_IndexEnd;
			}

			public void Reset()
			{
				m_Index = m_IndexStart;
			}
		}

		internal TValue[] m_Array;

		internal int m_StartIndex;

		internal int m_Length;

		public int Count
		{
			get
			{
				return m_Length;
			}
		}

		public TValue this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (m_Array == null)
				{
					throw new InvalidOperationException();
				}
				return m_Array[m_StartIndex + index];
			}
		}

		public ReadOnlyArray(TValue[] array)
		{
			m_Array = array;
			m_StartIndex = 0;
			m_Length = ((array != null) ? array.Length : 0);
		}

		public ReadOnlyArray(TValue[] array, int index, int length)
		{
			m_Array = array;
			m_StartIndex = index;
			m_Length = length;
		}

		public TValue[] ToArray()
		{
			TValue[] array = new TValue[m_Length];
			if (m_Length > 0)
			{
				Array.Copy(m_Array, m_StartIndex, array, 0, m_Length);
			}
			return array;
		}

		public int IndexOf(Predicate<TValue> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			for (int i = 0; i < m_Length; i++)
			{
				if (predicate(m_Array[m_StartIndex + i]))
				{
					return i;
				}
			}
			return -1;
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new Enumerator<TValue>(m_Array, m_StartIndex, m_Length);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static implicit operator ReadOnlyArray<TValue>(TValue[] array)
		{
			return new ReadOnlyArray<TValue>(array);
		}
	}
}
