using System;
using System.Globalization;
using System.Text;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Tree
{
	internal static class LexerUtils
	{
		public static double ParseNumber(Token T)
		{
			string text = T.Text;
			double result;
			if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				throw new SyntaxErrorException(T, "malformed number near '{0}'", text);
			}
			return result;
		}

		public static double ParseHexInteger(Token T)
		{
			string text = T.Text;
			if (text.Length < 2 || (text[0] != '0' && char.ToUpper(text[1]) != 'X'))
			{
				throw new InternalErrorException("hex numbers must start with '0x' near '{0}'.", text);
			}
			ulong result;
			if (!ulong.TryParse(text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
			{
				throw new SyntaxErrorException(T, "malformed number near '{0}'", text);
			}
			return result;
		}

		public static string ReadHexProgressive(string s, ref double d, out int digits)
		{
			digits = 0;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (CharIsHexDigit(c))
				{
					int num = HexDigit2Value(c);
					d *= 16.0;
					d += num;
					digits++;
					continue;
				}
				return s.Substring(i);
			}
			return string.Empty;
		}

		public static double ParseHexFloat(Token T)
		{
			string text = T.Text;
			try
			{
				if (text.Length < 2 || (text[0] != '0' && char.ToUpper(text[1]) != 'X'))
				{
					throw new InternalErrorException("hex float must start with '0x' near '{0}'", text);
				}
				text = text.Substring(2);
				double d = 0.0;
				int digits = 0;
				int digits2;
				text = ReadHexProgressive(text, ref d, out digits2);
				if (text.Length > 0 && text[0] == '.')
				{
					text = text.Substring(1);
					text = ReadHexProgressive(text, ref d, out digits);
				}
				digits *= -4;
				if (text.Length > 0 && char.ToUpper(text[0]) == 'P')
				{
					if (text.Length == 1)
					{
						throw new SyntaxErrorException(T, "invalid hex float format near '{0}'", text);
					}
					text = text.Substring((text[1] != '+') ? 1 : 2);
					int num = int.Parse(text, CultureInfo.InvariantCulture);
					digits += num;
				}
				return d * Math.Pow(2.0, digits);
			}
			catch (FormatException)
			{
				throw new SyntaxErrorException(T, "malformed number near '{0}'", text);
			}
		}

		public static int HexDigit2Value(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - 48;
			}
			if (c >= 'A' && c <= 'F')
			{
				return 10 + (c - 65);
			}
			if (c >= 'a' && c <= 'f')
			{
				return 10 + (c - 97);
			}
			throw new InternalErrorException("invalid hex digit near '{0}'", c);
		}

		public static bool CharIsDigit(char c)
		{
			if (c >= '0')
			{
				return c <= '9';
			}
			return false;
		}

		public static bool CharIsHexDigit(char c)
		{
			if (!CharIsDigit(c) && c != 'a' && c != 'b' && c != 'c' && c != 'd' && c != 'e' && c != 'f' && c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E')
			{
				return c == 'F';
			}
			return true;
		}

		public static string AdjustLuaLongString(string str)
		{
			if (str.StartsWith("\r\n"))
			{
				str = str.Substring(2);
			}
			else if (str.StartsWith("\n"))
			{
				str = str.Substring(1);
			}
			return str;
		}

		public static string UnescapeLuaString(Token token, string str)
		{
			if (!Framework.Do.StringContainsChar(str, '\\'))
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			string text = "";
			string text2 = "";
			bool flag3 = false;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				do
				{
					int num2;
					if (flag)
					{
						if (text2.Length == 0 && !flag2 && num == 0)
						{
							switch (c)
							{
							case 'a':
								stringBuilder.Append('\a');
								flag = false;
								flag3 = false;
								break;
							case '\n':
								stringBuilder.Append('\n');
								flag = false;
								break;
							case 'b':
								stringBuilder.Append('\b');
								flag = false;
								break;
							case 'f':
								stringBuilder.Append('\f');
								flag = false;
								break;
							case 'n':
								stringBuilder.Append('\n');
								flag = false;
								break;
							case 'r':
								stringBuilder.Append('\r');
								flag = false;
								break;
							case 't':
								stringBuilder.Append('\t');
								flag = false;
								break;
							case 'v':
								stringBuilder.Append('\v');
								flag = false;
								break;
							case '\\':
								stringBuilder.Append('\\');
								flag = false;
								flag3 = false;
								break;
							case '"':
								stringBuilder.Append('"');
								flag = false;
								flag3 = false;
								break;
							case '\'':
								stringBuilder.Append('\'');
								flag = false;
								flag3 = false;
								break;
							case '[':
								stringBuilder.Append('[');
								flag = false;
								flag3 = false;
								break;
							case ']':
								stringBuilder.Append(']');
								flag = false;
								flag3 = false;
								break;
							case 'x':
								flag2 = true;
								break;
							case 'u':
								num = 1;
								break;
							case 'z':
								flag3 = true;
								flag = false;
								break;
							default:
								if (CharIsDigit(c))
								{
									text2 += c;
									break;
								}
								throw new SyntaxErrorException(token, "invalid escape sequence near '\\{0}'", c);
							case '\r':
								break;
							}
							break;
						}
						switch (num)
						{
						case 1:
							if (c != '{')
							{
								throw new SyntaxErrorException(token, "'{' expected near '\\u'");
							}
							num = 2;
							break;
						case 2:
							if (c == '}')
							{
								int i3 = int.Parse(text2, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
								stringBuilder.Append(ConvertUtf32ToChar(i3));
								num = 0;
								text2 = string.Empty;
								flag = false;
							}
							else
							{
								if (text2.Length >= 8)
								{
									throw new SyntaxErrorException(token, "'}' missing, or unicode code point too large after '\\u'");
								}
								text2 += c;
							}
							break;
						default:
							if (flag2)
							{
								if (CharIsHexDigit(c))
								{
									text2 += c;
									if (text2.Length == 2)
									{
										int i2 = int.Parse(text2, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
										stringBuilder.Append(ConvertUtf32ToChar(i2));
										flag3 = false;
										flag = false;
									}
									break;
								}
								throw new SyntaxErrorException(token, "hexadecimal digit expected near '\\{0}{1}{2}'", text, text2, c);
							}
							if (text2.Length <= 0)
							{
								break;
							}
							if (CharIsDigit(c))
							{
								text2 += c;
							}
							if (text2.Length != 3 && CharIsDigit(c))
							{
								break;
							}
							num2 = int.Parse(text2, CultureInfo.InvariantCulture);
							if (num2 > 255)
							{
								throw new SyntaxErrorException(token, "decimal escape too large near '\\{0}'", text2);
							}
							goto IL_0361;
						}
					}
					else if (c == '\\')
					{
						flag = true;
						flag2 = false;
						text2 = "";
					}
					else if (!flag3 || !char.IsWhiteSpace(c))
					{
						stringBuilder.Append(c);
						flag3 = false;
					}
					break;
					IL_0361:
					stringBuilder.Append(ConvertUtf32ToChar(num2));
					flag3 = false;
					flag = false;
				}
				while (!CharIsDigit(c));
			}
			if (flag && !flag2 && text2.Length > 0)
			{
				int i4 = int.Parse(text2, CultureInfo.InvariantCulture);
				stringBuilder.Append(ConvertUtf32ToChar(i4));
				flag = false;
			}
			if (flag)
			{
				throw new SyntaxErrorException(token, "unfinished string near '\"{0}\"'", stringBuilder.ToString());
			}
			return stringBuilder.ToString();
		}

		private static string ConvertUtf32ToChar(int i)
		{
			return char.ConvertFromUtf32(i);
		}
	}
}
