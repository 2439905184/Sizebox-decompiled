using System;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class UnixEncoding : Encoding
	{
		[Serializable]
		private class UnixDecoder : Decoder
		{
			private uint leftOverBits;

			private uint leftOverCount;

			public UnixDecoder()
			{
				leftOverBits = 0u;
				leftOverCount = 0u;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return InternalGetCharCount(bytes, index, count, leftOverBits, leftOverCount, true, false);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOverBits, ref leftOverCount, true, false);
			}
		}

		[Serializable]
		private class UnixEncoder : Encoder
		{
			private uint leftOver;

			public UnixEncoder()
			{
				leftOver = 0u;
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return InternalGetByteCount(chars, index, count, leftOver, flush);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteCount, bool flush)
			{
				return InternalGetBytes(chars, charIndex, charCount, bytes, byteCount, ref leftOver, flush);
			}
		}

		public static readonly Encoding Instance = new UnixEncoding();

		public static readonly char EscapeByte = '\0';

		private static int InternalGetByteCount(char[] chars, int index, int count, uint leftOver, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", _("ArgRange_Array"));
			}
			if (count < 0 || count > chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
			}
			int num = 0;
			uint num2 = leftOver;
			while (count > 0)
			{
				char c = chars[index];
				if (num2 == 0)
				{
					if (c == EscapeByte && count > 1)
					{
						num++;
						index++;
						count--;
					}
					else if (c < '\u0080')
					{
						num++;
					}
					else if (c < 'ࠀ')
					{
						num += 2;
					}
					else if (c >= '\ud800' && c <= '\udbff')
					{
						num2 = c;
					}
					else
					{
						num += 3;
					}
				}
				else
				{
					if (c < '\udc00' || c > '\udfff')
					{
						num += 3;
						num2 = 0u;
						continue;
					}
					num += 4;
					num2 = 0u;
				}
				index++;
				count--;
			}
			if (flush && num2 != 0)
			{
				num += 3;
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return InternalGetByteCount(chars, index, count, 0u, true);
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int num = 0;
			int num2 = s.Length;
			int num3 = 0;
			while (num2 > 0)
			{
				char c = s[num++];
				if (c == EscapeByte && num2 > 1)
				{
					num3++;
					num++;
					num2--;
				}
				else if (c < '\u0080')
				{
					num3++;
				}
				else if (c < 'ࠀ')
				{
					num3 += 2;
				}
				else if (c >= '\ud800' && c <= '\udbff' && num2 > 1)
				{
					uint num4 = s[num];
					if (num4 >= 56320 && num4 <= 57343)
					{
						num3 += 4;
						num++;
						num2--;
					}
					else
					{
						num3 += 3;
					}
				}
				else
				{
					num3 += 3;
				}
				num2--;
			}
			return num3;
		}

		private static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref uint leftOver, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", _("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
			}
			int num = bytes.Length;
			uint num2 = leftOver;
			int num3 = byteIndex;
			while (charCount > 0)
			{
				char c = chars[charIndex++];
				charCount--;
				uint num4;
				if (num2 == 0)
				{
					if (c >= '\ud800' && c <= '\udbff')
					{
						num2 = c;
						continue;
					}
					if (c == EscapeByte)
					{
						if (num3 >= num)
						{
							throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
						}
						if (--charCount >= 0)
						{
							bytes[num3++] = (byte)chars[charIndex++];
						}
						continue;
					}
					num4 = c;
				}
				else if (c >= '\udc00' && c <= '\udfff')
				{
					num4 = (uint)((int)(num2 - 55296 << 10) + (c - 56320) + 65536);
					num2 = 0u;
				}
				else
				{
					num4 = num2;
					num2 = 0u;
					charIndex--;
					charCount++;
				}
				if (num4 < 128)
				{
					if (num3 >= num)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)num4;
					continue;
				}
				if (num4 < 2048)
				{
					if (num3 + 2 > num)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)(0xC0u | (num4 >> 6));
					bytes[num3++] = (byte)(0x80u | (num4 & 0x3Fu));
					continue;
				}
				if (num4 < 65536)
				{
					if (num3 + 3 > num)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num3++] = (byte)(0xE0u | (num4 >> 12));
					bytes[num3++] = (byte)(0x80u | ((num4 >> 6) & 0x3Fu));
					bytes[num3++] = (byte)(0x80u | (num4 & 0x3Fu));
					continue;
				}
				if (num3 + 4 > num)
				{
					throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
				}
				bytes[num3++] = (byte)(0xF0u | (num4 >> 18));
				bytes[num3++] = (byte)(0x80u | ((num4 >> 12) & 0x3Fu));
				bytes[num3++] = (byte)(0x80u | ((num4 >> 6) & 0x3Fu));
				bytes[num3++] = (byte)(0x80u | (num4 & 0x3Fu));
			}
			if (flush && num2 != 0)
			{
				if (num3 + 3 > num)
				{
					throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
				}
				bytes[num3++] = (byte)(0xE0u | (num2 >> 12));
				bytes[num3++] = (byte)(0x80u | ((num2 >> 6) & 0x3Fu));
				bytes[num3++] = (byte)(0x80u | (num2 & 0x3Fu));
				num2 = 0u;
			}
			leftOver = num2;
			return num3 - byteIndex;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			uint leftOver = 0u;
			return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref leftOver, true);
		}

		public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", _("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
			}
			fixed (char* ptr = s)
			{
				fixed (byte* ptr2 = bytes)
				{
					return GetBytes(ptr + charIndex, charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (bytes == null || chars == null)
			{
				throw new ArgumentNullException((bytes == null) ? "bytes" : "chars");
			}
			if (charCount < 0 || byteCount < 0)
			{
				throw new ArgumentOutOfRangeException((charCount < 0) ? "charCount" : "byteCount");
			}
			int num = 0;
			int num2 = 0;
			while (charCount > 0)
			{
				char c = chars[num2++];
				uint num3;
				if (c >= '\ud800' && c <= '\udbff' && charCount > 1)
				{
					num3 = chars[num2];
					if (num3 >= 56320 && num3 <= 57343)
					{
						num3 = (uint)((int)(num3 - 56320) + (c - 55296 << 10) + 65536);
						num2++;
						charCount--;
					}
					else
					{
						num3 = c;
					}
				}
				else
				{
					if (c == EscapeByte && charCount > 1)
					{
						if (num >= byteCount)
						{
							throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
						}
						charCount -= 2;
						if (charCount >= 0)
						{
							bytes[num++] = (byte)chars[num2++];
						}
						continue;
					}
					num3 = c;
				}
				charCount--;
				if (num3 < 128)
				{
					if (num >= byteCount)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)num3;
					continue;
				}
				if (num3 < 2048)
				{
					if (num + 2 > byteCount)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)(0xC0u | (num3 >> 6));
					bytes[num++] = (byte)(0x80u | (num3 & 0x3Fu));
					continue;
				}
				if (num3 < 65536)
				{
					if (num + 3 > byteCount)
					{
						throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)(0xE0u | (num3 >> 12));
					bytes[num++] = (byte)(0x80u | ((num3 >> 6) & 0x3Fu));
					bytes[num++] = (byte)(0x80u | (num3 & 0x3Fu));
					continue;
				}
				if (num + 4 > byteCount)
				{
					throw new ArgumentException(_("Arg_InsufficientSpace"), "bytes");
				}
				bytes[num++] = (byte)(0xF0u | (num3 >> 18));
				bytes[num++] = (byte)(0x80u | ((num3 >> 12) & 0x3Fu));
				bytes[num++] = (byte)(0x80u | ((num3 >> 6) & 0x3Fu));
				bytes[num++] = (byte)(0x80u | (num3 & 0x3Fu));
			}
			return num;
		}

		private static int InternalGetCharCount(byte[] bytes, int index, int count, uint leftOverBits, uint leftOverCount, bool throwOnInvalid, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", _("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
			}
			int num = 0;
			int num2 = 0;
			uint num3 = leftOverBits;
			uint num4 = leftOverCount & 0xFu;
			uint num5 = (leftOverCount >> 4) & 0xFu;
			while (count > 0)
			{
				uint num6 = bytes[index++];
				num++;
				count--;
				if (num5 == 0)
				{
					if (num6 < 128)
					{
						num2++;
						num = 0;
					}
					else if ((num6 & 0xE0) == 192)
					{
						num3 = num6 & 0x1Fu;
						num4 = 1u;
						num5 = 2u;
					}
					else if ((num6 & 0xF0) == 224)
					{
						num3 = num6 & 0xFu;
						num4 = 1u;
						num5 = 3u;
					}
					else if ((num6 & 0xF8) == 240)
					{
						num3 = num6 & 7u;
						num4 = 1u;
						num5 = 4u;
					}
					else if ((num6 & 0xFC) == 248)
					{
						num3 = num6 & 3u;
						num4 = 1u;
						num5 = 5u;
					}
					else if ((num6 & 0xFE) == 252)
					{
						num3 = num6 & 3u;
						num4 = 1u;
						num5 = 6u;
					}
					else
					{
						num2 += num * 2;
						num = 0;
					}
				}
				else if ((num6 & 0xC0) == 128)
				{
					num3 = (num3 << 6) | (num6 & 0x3Fu);
					if (++num4 < num5)
					{
						continue;
					}
					if (num3 < 65536)
					{
						bool flag = false;
						switch (num5)
						{
						case 2u:
							flag = num3 <= 127;
							break;
						case 3u:
							flag = num3 <= 2047;
							break;
						case 4u:
							flag = num3 <= 65535;
							break;
						case 5u:
							flag = num3 <= 2097151;
							break;
						case 6u:
							flag = num3 <= 67108863;
							break;
						}
						num2 = ((!flag) ? (num2 + 1) : (num2 + num * 2));
					}
					else if (num3 < 1114112)
					{
						num2 += 2;
					}
					else if (throwOnInvalid)
					{
						num2 += num * 2;
					}
					num5 = 0u;
					num = 0;
				}
				else
				{
					if (num6 < 128)
					{
						index--;
						count++;
						num--;
					}
					num2 += num * 2;
					num5 = 0u;
					num = 0;
				}
			}
			if (flush && num5 != 0 && throwOnInvalid)
			{
				num2 += num * 2;
			}
			return num2;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return InternalGetCharCount(bytes, index, count, 0u, 0u, true, true);
		}

		private static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref uint leftOverBits, ref uint leftOverCount, bool throwOnInvalid, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", _("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
			}
			if (charIndex == chars.Length)
			{
				return 0;
			}
			byte[] array = new byte[6];
			int next_raw = 0;
			int num = chars.Length;
			int posn = charIndex;
			uint num2 = leftOverBits;
			uint num3 = leftOverCount & 0xFu;
			uint num4 = (leftOverCount >> 4) & 0xFu;
			while (byteCount > 0)
			{
				uint num5 = bytes[byteIndex++];
				array[next_raw++] = (byte)num5;
				byteCount--;
				if (num4 == 0)
				{
					if (num5 < 128)
					{
						if (posn >= num)
						{
							throw new ArgumentException(_("Arg_InsufficientSpace"), "chars");
						}
						next_raw = 0;
						chars[posn++] = (char)num5;
					}
					else if ((num5 & 0xE0) == 192)
					{
						num2 = num5 & 0x1Fu;
						num3 = 1u;
						num4 = 2u;
					}
					else if ((num5 & 0xF0) == 224)
					{
						num2 = num5 & 0xFu;
						num3 = 1u;
						num4 = 3u;
					}
					else if ((num5 & 0xF8) == 240)
					{
						num2 = num5 & 7u;
						num3 = 1u;
						num4 = 4u;
					}
					else if ((num5 & 0xFC) == 248)
					{
						num2 = num5 & 3u;
						num3 = 1u;
						num4 = 5u;
					}
					else if ((num5 & 0xFE) == 252)
					{
						num2 = num5 & 3u;
						num3 = 1u;
						num4 = 6u;
					}
					else
					{
						next_raw = 0;
						chars[posn++] = EscapeByte;
						chars[posn++] = (char)num5;
					}
				}
				else if ((num5 & 0xC0) == 128)
				{
					num2 = (num2 << 6) | (num5 & 0x3Fu);
					if (++num3 < num4)
					{
						continue;
					}
					if (num2 < 65536)
					{
						bool flag = false;
						switch (num4)
						{
						case 2u:
							flag = num2 <= 127;
							break;
						case 3u:
							flag = num2 <= 2047;
							break;
						case 4u:
							flag = num2 <= 65535;
							break;
						case 5u:
							flag = num2 <= 2097151;
							break;
						case 6u:
							flag = num2 <= 67108863;
							break;
						}
						if (flag)
						{
							CopyRaw(array, ref next_raw, chars, ref posn, num);
						}
						else
						{
							if (posn >= num)
							{
								throw new ArgumentException(_("Arg_InsufficientSpace"), "chars");
							}
							chars[posn++] = (char)num2;
						}
					}
					else if (num2 < 1114112)
					{
						if (posn + 2 > num)
						{
							throw new ArgumentException(_("Arg_InsufficientSpace"), "chars");
						}
						num2 -= 65536;
						chars[posn++] = (char)((num2 >> 10) + 55296);
						chars[posn++] = (char)((num2 & 0x3FF) + 56320);
					}
					else if (throwOnInvalid)
					{
						CopyRaw(array, ref next_raw, chars, ref posn, num);
					}
					num4 = 0u;
					next_raw = 0;
				}
				else
				{
					if (num5 < 128)
					{
						byteIndex--;
						byteCount++;
						next_raw--;
					}
					CopyRaw(array, ref next_raw, chars, ref posn, num);
					num4 = 0u;
					next_raw = 0;
				}
			}
			if (flush && num4 != 0 && throwOnInvalid)
			{
				CopyRaw(array, ref next_raw, chars, ref posn, num);
			}
			leftOverBits = num2;
			leftOverCount = num3 | (num4 << 4);
			return posn - charIndex;
		}

		private static void CopyRaw(byte[] raw, ref int next_raw, char[] chars, ref int posn, int length)
		{
			if (posn + next_raw * 2 > length)
			{
				throw new ArgumentException(_("Arg_InsufficientSpace"), "chars");
			}
			for (int i = 0; i < next_raw; i++)
			{
				chars[posn++] = EscapeByte;
				chars[posn++] = (char)raw[i];
			}
			next_raw = 0;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			uint leftOverBits = 0u;
			uint leftOverCount = 0u;
			return InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOverBits, ref leftOverCount, true, true);
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", _("ArgRange_NonNegative"));
			}
			return charCount * 4;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", _("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public override Decoder GetDecoder()
		{
			return new UnixDecoder();
		}

		public override Encoder GetEncoder()
		{
			return new UnixEncoder();
		}

		public override byte[] GetPreamble()
		{
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			if (value is UnixEncoding)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override byte[] GetBytes(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			byte[] array = new byte[GetByteCount(s)];
			GetBytes(s, 0, s.Length, array, 0);
			return array;
		}

		private static string _(string arg)
		{
			return arg;
		}
	}
}
