using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct Substring : IComparable<Substring>, IEquatable<Substring>
	{
		private readonly string m_String;

		private readonly int m_Index;

		private readonly int m_Length;

		public bool isEmpty
		{
			get
			{
				return m_Length == 0;
			}
		}

		public int length
		{
			get
			{
				return m_Length;
			}
		}

		public int index
		{
			get
			{
				return m_Index;
			}
		}

		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return m_String[m_Index + index];
			}
		}

		public Substring(string str)
		{
			m_String = str;
			m_Index = 0;
			if (str != null)
			{
				m_Length = str.Length;
			}
			else
			{
				m_Length = 0;
			}
		}

		public Substring(string str, int index, int length)
		{
			m_String = str;
			m_Index = index;
			m_Length = length;
		}

		public Substring(string str, int index)
		{
			m_String = str;
			m_Index = index;
			m_Length = str.Length - index;
		}

		public override bool Equals(object obj)
		{
			object obj2;
			if ((obj2 = obj) is Substring)
			{
				Substring other = (Substring)obj2;
				return Equals(other);
			}
			string other2;
			if ((other2 = obj as string) != null)
			{
				return Equals(other2);
			}
			return false;
		}

		public bool Equals(string other)
		{
			if (string.IsNullOrEmpty(other))
			{
				return m_Length == 0;
			}
			if (other.Length != m_Length)
			{
				return false;
			}
			for (int i = 0; i < m_Length; i++)
			{
				if (other[i] != m_String[m_Index + i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(Substring other)
		{
			return CompareTo(other) == 0;
		}

		public bool Equals(InternedString other)
		{
			if (length != other.length)
			{
				return false;
			}
			return string.Compare(m_String, m_Index, other.ToString(), 0, length, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public int CompareTo(Substring other)
		{
			return Compare(this, other, StringComparison.CurrentCulture);
		}

		public static int Compare(Substring left, Substring right, StringComparison comparison)
		{
			if (left.m_Length != right.m_Length)
			{
				if (left.m_Length < right.m_Length)
				{
					return -1;
				}
				return 1;
			}
			return string.Compare(left.m_String, left.m_Index, right.m_String, right.m_Index, left.m_Length, comparison);
		}

		public bool StartsWith(string str)
		{
			if (str.Length > length)
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (m_String[m_Index + i] != str[i])
				{
					return false;
				}
			}
			return true;
		}

		public string Substr(int index = 0, int length = -1)
		{
			if (length < 0)
			{
				length = this.length - index;
			}
			return m_String.Substring(m_Index + index, length);
		}

		public override string ToString()
		{
			if (m_String == null)
			{
				return string.Empty;
			}
			return m_String.Substring(m_Index, m_Length);
		}

		public override int GetHashCode()
		{
			if (m_String == null)
			{
				return 0;
			}
			if (m_Index == 0 && m_Length == m_String.Length)
			{
				return m_String.GetHashCode();
			}
			return ToString().GetHashCode();
		}

		public static bool operator ==(Substring a, Substring b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Substring a, Substring b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Substring a, InternedString b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Substring a, InternedString b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(InternedString a, Substring b)
		{
			return b.Equals(a);
		}

		public static bool operator !=(InternedString a, Substring b)
		{
			return !b.Equals(a);
		}

		public static implicit operator Substring(string s)
		{
			return new Substring(s);
		}
	}
}
