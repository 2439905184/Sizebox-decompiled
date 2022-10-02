using System;
using System.Collections;
using System.Collections.Generic;

namespace Poly2Tri
{
	public struct FixedArray3<T> : IEnumerable<T>, IEnumerable where T : class
	{
		public T _0;

		public T _1;

		public T _2;

		public T this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return _0;
				case 1:
					return _1;
				case 2:
					return _2;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					_0 = value;
					break;
				case 1:
					_1 = value;
					break;
				case 2:
					_2 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool Contains(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		public void Clear()
		{
			_0 = (_1 = (_2 = null));
		}

		public void Clear(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] != null && this[i].Equals(value))
				{
					this[i] = null;
				}
			}
		}

		private IEnumerable<T> Enumerate()
		{
			int i = 0;
			while (i < 3)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
