using System;
using System.Text;

namespace MoonSharp.Interpreter.Interop.LuaStateInterop
{
	public class CharPtr
	{
		public char[] chars;

		public int index;

		public char this[int offset]
		{
			get
			{
				return chars[index + offset];
			}
			set
			{
				chars[index + offset] = value;
			}
		}

		public char this[uint offset]
		{
			get
			{
				return chars[index + offset];
			}
			set
			{
				chars[index + offset] = value;
			}
		}

		public char this[long offset]
		{
			get
			{
				return chars[index + (int)offset];
			}
			set
			{
				chars[index + (int)offset] = value;
			}
		}

		public static implicit operator CharPtr(string str)
		{
			return new CharPtr(str);
		}

		public static implicit operator CharPtr(char[] chars)
		{
			return new CharPtr(chars);
		}

		public static implicit operator CharPtr(byte[] bytes)
		{
			return new CharPtr(bytes);
		}

		public CharPtr()
		{
			chars = null;
			index = 0;
		}

		public CharPtr(string str)
		{
			chars = (str + "\0").ToCharArray();
			index = 0;
		}

		public CharPtr(CharPtr ptr)
		{
			chars = ptr.chars;
			index = ptr.index;
		}

		public CharPtr(CharPtr ptr, int index)
		{
			chars = ptr.chars;
			this.index = index;
		}

		public CharPtr(char[] chars)
		{
			this.chars = chars;
			index = 0;
		}

		public CharPtr(char[] chars, int index)
		{
			this.chars = chars;
			this.index = index;
		}

		public CharPtr(byte[] bytes)
		{
			chars = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				chars[i] = (char)bytes[i];
			}
			index = 0;
		}

		public CharPtr(IntPtr ptr)
		{
			chars = new char[0];
			index = 0;
		}

		public static CharPtr operator +(CharPtr ptr, int offset)
		{
			return new CharPtr(ptr.chars, ptr.index + offset);
		}

		public static CharPtr operator -(CharPtr ptr, int offset)
		{
			return new CharPtr(ptr.chars, ptr.index - offset);
		}

		public static CharPtr operator +(CharPtr ptr, uint offset)
		{
			return new CharPtr(ptr.chars, ptr.index + (int)offset);
		}

		public static CharPtr operator -(CharPtr ptr, uint offset)
		{
			return new CharPtr(ptr.chars, ptr.index - (int)offset);
		}

		public void inc()
		{
			index++;
		}

		public void dec()
		{
			index--;
		}

		public CharPtr next()
		{
			return new CharPtr(chars, index + 1);
		}

		public CharPtr prev()
		{
			return new CharPtr(chars, index - 1);
		}

		public CharPtr add(int ofs)
		{
			return new CharPtr(chars, index + ofs);
		}

		public CharPtr sub(int ofs)
		{
			return new CharPtr(chars, index - ofs);
		}

		public static bool operator ==(CharPtr ptr, char ch)
		{
			return ptr[0] == ch;
		}

		public static bool operator ==(char ch, CharPtr ptr)
		{
			return ptr[0] == ch;
		}

		public static bool operator !=(CharPtr ptr, char ch)
		{
			return ptr[0] != ch;
		}

		public static bool operator !=(char ch, CharPtr ptr)
		{
			return ptr[0] != ch;
		}

		public static CharPtr operator +(CharPtr ptr1, CharPtr ptr2)
		{
			string text = "";
			for (int i = 0; ptr1[i] != 0; i++)
			{
				text += ptr1[i];
			}
			for (int j = 0; ptr2[j] != 0; j++)
			{
				text += ptr2[j];
			}
			return new CharPtr(text);
		}

		public static int operator -(CharPtr ptr1, CharPtr ptr2)
		{
			return ptr1.index - ptr2.index;
		}

		public static bool operator <(CharPtr ptr1, CharPtr ptr2)
		{
			return ptr1.index < ptr2.index;
		}

		public static bool operator <=(CharPtr ptr1, CharPtr ptr2)
		{
			return ptr1.index <= ptr2.index;
		}

		public static bool operator >(CharPtr ptr1, CharPtr ptr2)
		{
			return ptr1.index > ptr2.index;
		}

		public static bool operator >=(CharPtr ptr1, CharPtr ptr2)
		{
			return ptr1.index >= ptr2.index;
		}

		public static bool operator ==(CharPtr ptr1, CharPtr ptr2)
		{
			if ((object)ptr1 == null && (object)ptr2 == null)
			{
				return true;
			}
			if ((object)ptr1 == null)
			{
				return false;
			}
			if ((object)ptr2 == null)
			{
				return false;
			}
			if (ptr1.chars == ptr2.chars)
			{
				return ptr1.index == ptr2.index;
			}
			return false;
		}

		public static bool operator !=(CharPtr ptr1, CharPtr ptr2)
		{
			return !(ptr1 == ptr2);
		}

		public override bool Equals(object o)
		{
			return this == o as CharPtr;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = index; i < chars.Length && chars[i] != 0; i++)
			{
				stringBuilder.Append(chars[i]);
			}
			return stringBuilder.ToString();
		}

		public string ToString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = index; i < chars.Length && i < length + index; i++)
			{
				stringBuilder.Append(chars[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
