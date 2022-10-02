using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct JsonParser
	{
		public enum JsonValueType
		{
			None = 0,
			Bool = 1,
			Real = 2,
			Integer = 3,
			String = 4,
			Array = 5,
			Object = 6,
			Any = 7
		}

		public struct JsonString : IEquatable<JsonString>
		{
			public Substring text;

			public bool hasEscapes;

			public override string ToString()
			{
				if (!hasEscapes)
				{
					return text.ToString();
				}
				StringBuilder stringBuilder = new StringBuilder();
				int length = text.length;
				for (int i = 0; i < length; i++)
				{
					char c = text[i];
					if (c == '\\')
					{
						i++;
						if (i == length)
						{
							break;
						}
						c = text[i];
					}
					stringBuilder.Append(c);
				}
				return stringBuilder.ToString();
			}

			public bool Equals(JsonString other)
			{
				if (hasEscapes == other.hasEscapes)
				{
					return Substring.Compare(text, other.text, StringComparison.InvariantCultureIgnoreCase) == 0;
				}
				int length = text.length;
				int length2 = other.text.length;
				int num = 0;
				int num2 = 0;
				while (num < length && num2 < length2)
				{
					char c = text[num];
					char c2 = other.text[num2];
					if (c == '\\')
					{
						num++;
						if (num == length)
						{
							return false;
						}
						c = text[num];
					}
					if (c2 == '\\')
					{
						num2++;
						if (num2 == length2)
						{
							return false;
						}
						c2 = other.text[num2];
					}
					if (char.ToUpperInvariant(c) != char.ToUpperInvariant(c2))
					{
						return false;
					}
					num++;
					num2++;
				}
				if (num == length)
				{
					return num2 == length2;
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				object obj2;
				if ((obj2 = obj) is JsonString)
				{
					JsonString other = (JsonString)obj2;
					return Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (text.GetHashCode() * 397) ^ hasEscapes.GetHashCode();
			}

			public static bool operator ==(JsonString left, JsonString right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(JsonString left, JsonString right)
			{
				return !left.Equals(right);
			}

			public static implicit operator JsonString(string str)
			{
				JsonString result = default(JsonString);
				result.text = str;
				return result;
			}
		}

		public struct JsonValue : IEquatable<JsonValue>
		{
			[Serializable]
			[CompilerGenerated]
			private sealed class _003C_003Ec
			{
				public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

				public static Func<JsonValue, string> _003C_003E9__11_0;

				public static Func<KeyValuePair<string, JsonValue>, string> _003C_003E9__11_1;

				internal string _003CToString_003Eb__11_0(JsonValue x)
				{
					return x.ToString();
				}

				internal string _003CToString_003Eb__11_1(KeyValuePair<string, JsonValue> pair)
				{
					return string.Format("\"{0}\" : \"{1}\"", pair.Key, pair.Value);
				}
			}

			public JsonValueType type;

			public bool boolValue;

			public double realValue;

			public long integerValue;

			public JsonString stringValue;

			public List<JsonValue> arrayValue;

			public Dictionary<string, JsonValue> objectValue;

			public object anyValue;

			public bool ToBoolean()
			{
				switch (type)
				{
				case JsonValueType.Bool:
					return boolValue;
				case JsonValueType.Integer:
					return integerValue != 0;
				case JsonValueType.Real:
					return NumberHelpers.Approximately(0.0, realValue);
				case JsonValueType.String:
					return Convert.ToBoolean(ToString());
				default:
					return false;
				}
			}

			public long ToInteger()
			{
				switch (type)
				{
				case JsonValueType.Bool:
					return boolValue ? 1 : 0;
				case JsonValueType.Integer:
					return integerValue;
				case JsonValueType.Real:
					return (long)realValue;
				case JsonValueType.String:
					return Convert.ToInt64(ToString());
				default:
					return 0L;
				}
			}

			public double ToDouble()
			{
				switch (type)
				{
				case JsonValueType.Bool:
					return boolValue ? 1 : 0;
				case JsonValueType.Integer:
					return integerValue;
				case JsonValueType.Real:
					return realValue;
				case JsonValueType.String:
					return Convert.ToSingle(ToString());
				default:
					return 0.0;
				}
			}

			public override string ToString()
			{
				switch (type)
				{
				case JsonValueType.None:
					return "null";
				case JsonValueType.Bool:
					return boolValue.ToString();
				case JsonValueType.Integer:
					return integerValue.ToString(CultureInfo.InvariantCulture);
				case JsonValueType.Real:
					return realValue.ToString(CultureInfo.InvariantCulture);
				case JsonValueType.String:
					return stringValue.ToString();
				case JsonValueType.Array:
					if (arrayValue == null)
					{
						return "[]";
					}
					return "[" + string.Join(",", arrayValue.Select(_003C_003Ec._003C_003E9__11_0 ?? (_003C_003Ec._003C_003E9__11_0 = _003C_003Ec._003C_003E9._003CToString_003Eb__11_0))) + "]";
				case JsonValueType.Object:
				{
					if (objectValue == null)
					{
						return "{}";
					}
					IEnumerable<string> values = objectValue.Select(_003C_003Ec._003C_003E9__11_1 ?? (_003C_003Ec._003C_003E9__11_1 = _003C_003Ec._003C_003E9._003CToString_003Eb__11_1));
					return "{" + string.Join(",", values) + "}";
				}
				case JsonValueType.Any:
					return anyValue.ToString();
				default:
					return base.ToString();
				}
			}

			public static implicit operator JsonValue(bool val)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Bool;
				result.boolValue = val;
				return result;
			}

			public static implicit operator JsonValue(long val)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Integer;
				result.integerValue = val;
				return result;
			}

			public static implicit operator JsonValue(double val)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Real;
				result.realValue = val;
				return result;
			}

			public static implicit operator JsonValue(string str)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.String;
				result.stringValue = new JsonString
				{
					text = str
				};
				return result;
			}

			public static implicit operator JsonValue(JsonString str)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.String;
				result.stringValue = str;
				return result;
			}

			public static implicit operator JsonValue(List<JsonValue> array)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Array;
				result.arrayValue = array;
				return result;
			}

			public static implicit operator JsonValue(Dictionary<string, JsonValue> obj)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Object;
				result.objectValue = obj;
				return result;
			}

			public static implicit operator JsonValue(Enum val)
			{
				JsonValue result = default(JsonValue);
				result.type = JsonValueType.Any;
				result.anyValue = val;
				return result;
			}

			public bool Equals(JsonValue other)
			{
				if (type == other.type)
				{
					switch (type)
					{
					case JsonValueType.None:
						return true;
					case JsonValueType.Bool:
						return boolValue == other.boolValue;
					case JsonValueType.Integer:
						return integerValue == other.integerValue;
					case JsonValueType.Real:
						return NumberHelpers.Approximately(realValue, other.realValue);
					case JsonValueType.String:
						return stringValue == other.stringValue;
					case JsonValueType.Object:
						throw new NotImplementedException();
					case JsonValueType.Array:
						throw new NotImplementedException();
					case JsonValueType.Any:
						return anyValue.Equals(other.anyValue);
					default:
						return false;
					}
				}
				if (anyValue != null)
				{
					return Equals(anyValue, other);
				}
				if (other.anyValue != null)
				{
					return Equals(other.anyValue, this);
				}
				return false;
			}

			private static bool Equals(object obj, JsonValue value)
			{
				if (obj == null)
				{
					return false;
				}
				Regex regex;
				if ((regex = obj as Regex) != null)
				{
					return regex.IsMatch(value.ToString());
				}
				string text;
				if ((text = obj as string) != null)
				{
					switch (value.type)
					{
					case JsonValueType.String:
						return value.stringValue == text;
					case JsonValueType.Integer:
					{
						long result;
						if (long.TryParse(text, out result))
						{
							return result == value.integerValue;
						}
						return false;
					}
					case JsonValueType.Real:
					{
						double result2;
						if (double.TryParse(text, out result2))
						{
							return NumberHelpers.Approximately(result2, value.realValue);
						}
						return false;
					}
					case JsonValueType.Bool:
						if (value.boolValue)
						{
							if (!(text == "True") && !(text == "true"))
							{
								return text == "1";
							}
							return true;
						}
						if (!(text == "False") && !(text == "false"))
						{
							return text == "0";
						}
						return true;
					}
				}
				object obj2;
				if ((obj2 = obj) is float)
				{
					float num = (float)obj2;
					if (value.type == JsonValueType.Real)
					{
						return NumberHelpers.Approximately(num, value.realValue);
					}
					if (value.type == JsonValueType.String)
					{
						float result3;
						if (float.TryParse(value.ToString(), out result3))
						{
							return Mathf.Approximately(num, result3);
						}
						return false;
					}
				}
				if ((obj2 = obj) is double)
				{
					double a = (double)obj2;
					if (value.type == JsonValueType.Real)
					{
						return NumberHelpers.Approximately(a, value.realValue);
					}
					if (value.type == JsonValueType.String)
					{
						double result4;
						if (double.TryParse(value.ToString(), out result4))
						{
							return NumberHelpers.Approximately(a, result4);
						}
						return false;
					}
				}
				if ((obj2 = obj) is int)
				{
					int num2 = (int)obj2;
					if (value.type == JsonValueType.Integer)
					{
						return num2 == value.integerValue;
					}
					if (value.type == JsonValueType.String)
					{
						int result5;
						if (int.TryParse(value.ToString(), out result5))
						{
							return num2 == result5;
						}
						return false;
					}
				}
				if ((obj2 = obj) is long)
				{
					long num3 = (long)obj2;
					if (value.type == JsonValueType.Integer)
					{
						return num3 == value.integerValue;
					}
					if (value.type == JsonValueType.String)
					{
						long result6;
						if (long.TryParse(value.ToString(), out result6))
						{
							return num3 == result6;
						}
						return false;
					}
				}
				if ((obj2 = obj) is bool)
				{
					bool flag = (bool)obj2;
					if (value.type == JsonValueType.Bool)
					{
						return flag == value.boolValue;
					}
					if (value.type == JsonValueType.String)
					{
						if (flag)
						{
							if (!(value.stringValue == "true") && !(value.stringValue == "True"))
							{
								return value.stringValue == "1";
							}
							return true;
						}
						if (!(value.stringValue == "false") && !(value.stringValue == "False"))
						{
							return value.stringValue == "0";
						}
						return true;
					}
				}
				if (obj is Enum)
				{
					if (value.type == JsonValueType.Integer)
					{
						return Convert.ToInt64(obj) == value.integerValue;
					}
					if (value.type == JsonValueType.String)
					{
						return value.stringValue == Enum.GetName(obj.GetType(), obj);
					}
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				object obj2;
				if ((obj2 = obj) is JsonValue)
				{
					JsonValue other = (JsonValue)obj2;
					return Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return ((((((((((((((int)type * 397) ^ boolValue.GetHashCode()) * 397) ^ realValue.GetHashCode()) * 397) ^ integerValue.GetHashCode()) * 397) ^ stringValue.GetHashCode()) * 397) ^ ((arrayValue != null) ? arrayValue.GetHashCode() : 0)) * 397) ^ ((objectValue != null) ? objectValue.GetHashCode() : 0)) * 397) ^ ((anyValue != null) ? anyValue.GetHashCode() : 0);
			}

			public static bool operator ==(JsonValue left, JsonValue right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(JsonValue left, JsonValue right)
			{
				return !left.Equals(right);
			}
		}

		private readonly string m_Text;

		private readonly int m_Length;

		private int m_Position;

		private bool m_MatchAnyElementInArray;

		private bool m_DryRun;

		public bool isAtEnd
		{
			get
			{
				return m_Position >= m_Length;
			}
		}

		public JsonParser(string json)
		{
			this = default(JsonParser);
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			m_Text = json;
			m_Length = json.Length;
		}

		public void Reset()
		{
			m_Position = 0;
			m_MatchAnyElementInArray = false;
			m_DryRun = false;
		}

		public override string ToString()
		{
			if (m_Text != null)
			{
				return string.Format("{0}: {1}", m_Position, m_Text.Substring(m_Position));
			}
			return base.ToString();
		}

		public bool NavigateToProperty(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			int length = path.Length;
			int i = 0;
			m_DryRun = true;
			if (!ParseToken('{'))
			{
				return false;
			}
			while (m_Position < m_Length && i < length)
			{
				SkipWhitespace();
				if (m_Position == m_Length)
				{
					return false;
				}
				if (m_Text[m_Position] != '"')
				{
					return false;
				}
				m_Position++;
				int num = i;
				for (; i < length; i++)
				{
					char c = path[i];
					if (c == '/' || c == '[' || m_Text[m_Position] != c)
					{
						break;
					}
					m_Position++;
				}
				if (m_Position < m_Length && m_Text[m_Position] == '"' && (i >= length || path[i] == '/' || path[i] == '['))
				{
					m_Position++;
					if (!SkipToValue())
					{
						return false;
					}
					if (i >= length)
					{
						return true;
					}
					if (path[i] == '/')
					{
						i++;
						if (!ParseToken('{'))
						{
							return false;
						}
					}
					else if (path[i] == '[')
					{
						i++;
						if (i == length)
						{
							throw new ArgumentException("Malformed JSON property path: " + path, "path");
						}
						if (path[i] != ']')
						{
							throw new NotImplementedException("Navigating to specific array element");
						}
						m_MatchAnyElementInArray = true;
						i++;
						if (i == length)
						{
							return true;
						}
					}
				}
				else
				{
					i = num;
					while (m_Position < m_Length && m_Text[m_Position] != '"')
					{
						m_Position++;
					}
					if (m_Position == m_Length || m_Text[m_Position] != '"')
					{
						return false;
					}
					m_Position++;
					if (!SkipToValue() || !ParseValue())
					{
						return false;
					}
					SkipWhitespace();
					if (m_Position == m_Length || m_Text[m_Position] == '}' || m_Text[m_Position] != ',')
					{
						return false;
					}
					m_Position++;
				}
			}
			return false;
		}

		public bool CurrentPropertyHasValueEqualTo(JsonValue expectedValue)
		{
			int position = m_Position;
			m_DryRun = false;
			JsonValue result;
			if (!ParseValue(out result))
			{
				m_Position = position;
				return false;
			}
			m_Position = position;
			bool flag = false;
			if (result.type == JsonValueType.Array && m_MatchAnyElementInArray)
			{
				List<JsonValue> arrayValue = result.arrayValue;
				int num = 0;
				while (!flag && num < arrayValue.Count)
				{
					flag = arrayValue[num] == expectedValue;
					num++;
				}
			}
			else
			{
				flag = result == expectedValue;
			}
			return flag;
		}

		public bool ParseToken(char token)
		{
			SkipWhitespace();
			if (m_Position == m_Length)
			{
				return false;
			}
			if (m_Text[m_Position] != token)
			{
				return false;
			}
			m_Position++;
			SkipWhitespace();
			return m_Position < m_Length;
		}

		public bool ParseValue()
		{
			JsonValue result;
			return ParseValue(out result);
		}

		public bool ParseValue(out JsonValue result)
		{
			result = default(JsonValue);
			SkipWhitespace();
			if (m_Position == m_Length)
			{
				return false;
			}
			switch (m_Text[m_Position])
			{
			case '"':
				if (ParseStringValue(out result))
				{
					return true;
				}
				break;
			case '[':
				if (ParseArrayValue(out result))
				{
					return true;
				}
				break;
			case '{':
				if (ParseObjectValue(out result))
				{
					return true;
				}
				break;
			case 'f':
			case 't':
				if (ParseBooleanValue(out result))
				{
					return true;
				}
				break;
			case 'n':
				if (ParseNullValue(out result))
				{
					return true;
				}
				break;
			default:
				if (ParseNumber(out result))
				{
					return true;
				}
				break;
			}
			return false;
		}

		public bool ParseStringValue(out JsonValue result)
		{
			result = default(JsonValue);
			SkipWhitespace();
			if (m_Position == m_Length || m_Text[m_Position] != '"')
			{
				return false;
			}
			m_Position++;
			int position = m_Position;
			bool hasEscapes = false;
			for (; m_Position < m_Length; m_Position++)
			{
				switch (m_Text[m_Position])
				{
				case '\\':
					m_Position++;
					if (m_Position != m_Length)
					{
						hasEscapes = true;
						continue;
					}
					break;
				case '"':
					m_Position++;
					result = new JsonString
					{
						text = new Substring(m_Text, position, m_Position - position - 1),
						hasEscapes = hasEscapes
					};
					return true;
				default:
					continue;
				}
				break;
			}
			return false;
		}

		public bool ParseArrayValue(out JsonValue result)
		{
			result = default(JsonValue);
			SkipWhitespace();
			if (m_Position == m_Length || m_Text[m_Position] != '[')
			{
				return false;
			}
			m_Position++;
			if (m_Position == m_Length)
			{
				return false;
			}
			if (m_Text[m_Position] == ']')
			{
				result = new JsonValue
				{
					type = JsonValueType.Array
				};
				m_Position++;
				return true;
			}
			List<JsonValue> list = null;
			if (!m_DryRun)
			{
				list = new List<JsonValue>();
			}
			while (m_Position < m_Length)
			{
				JsonValue result2;
				if (!ParseValue(out result2))
				{
					return false;
				}
				if (!m_DryRun)
				{
					list.Add(result2);
				}
				SkipWhitespace();
				if (m_Position == m_Length)
				{
					return false;
				}
				switch (m_Text[m_Position])
				{
				case ']':
					m_Position++;
					if (!m_DryRun)
					{
						result = list;
					}
					return true;
				case ',':
					m_Position++;
					break;
				}
			}
			return false;
		}

		public bool ParseObjectValue(out JsonValue result)
		{
			result = default(JsonValue);
			if (!ParseToken('{'))
			{
				return false;
			}
			if (m_Position < m_Length && m_Text[m_Position] == '}')
			{
				result = new JsonValue
				{
					type = JsonValueType.Object
				};
				m_Position++;
				return true;
			}
			while (m_Position < m_Length)
			{
				JsonValue result2;
				if (!ParseStringValue(out result2))
				{
					return false;
				}
				if (!SkipToValue())
				{
					return false;
				}
				JsonValue result3;
				if (!ParseValue(out result3))
				{
					return false;
				}
				if (!m_DryRun)
				{
					throw new NotImplementedException();
				}
				SkipWhitespace();
				if (m_Position < m_Length && m_Text[m_Position] == '}')
				{
					if (!m_DryRun)
					{
						throw new NotImplementedException();
					}
					m_Position++;
					return true;
				}
			}
			return false;
		}

		public bool ParseNumber(out JsonValue result)
		{
			result = default(JsonValue);
			SkipWhitespace();
			if (m_Position == m_Length)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			long num = 0L;
			double num2 = 0.0;
			double num3 = 10.0;
			int num4 = 0;
			if (m_Text[m_Position] == '-')
			{
				flag = true;
				m_Position++;
			}
			if (m_Position == m_Length || !char.IsDigit(m_Text[m_Position]))
			{
				return false;
			}
			while (m_Position < m_Length)
			{
				char c = m_Text[m_Position];
				if (c == '.' || c < '0' || c > '9')
				{
					break;
				}
				num = num * 10 + c - 48;
				m_Position++;
			}
			if (m_Position < m_Length && m_Text[m_Position] == '.')
			{
				flag2 = true;
				m_Position++;
				if (m_Position == m_Length || !char.IsDigit(m_Text[m_Position]))
				{
					return false;
				}
				while (m_Position < m_Length)
				{
					char c2 = m_Text[m_Position];
					if (c2 < '0' || c2 > '9')
					{
						break;
					}
					num2 = (double)(c2 - 48) / num3 + num2;
					num3 *= 10.0;
					m_Position++;
				}
			}
			if (m_Position < m_Length && (m_Text[m_Position] == 'e' || m_Text[m_Position] == 'E'))
			{
				m_Position++;
				bool flag3 = false;
				if (m_Position < m_Length && m_Text[m_Position] == '-')
				{
					flag3 = true;
					m_Position++;
				}
				else if (m_Position < m_Length && m_Text[m_Position] == '+')
				{
					m_Position++;
				}
				int num5 = 1;
				while (m_Position < m_Length && char.IsDigit(m_Text[m_Position]))
				{
					int num6 = m_Text[m_Position] - 48;
					num4 *= num5;
					num4 += num6;
					num5 *= 10;
					m_Position++;
				}
				if (flag3)
				{
					num4 *= -1;
				}
			}
			if (!m_DryRun)
			{
				if (!flag2 && num4 == 0)
				{
					if (flag)
					{
						result = -num;
					}
					else
					{
						result = num;
					}
				}
				else
				{
					float num7 = ((!flag) ? ((float)((double)num + num2)) : ((float)(0.0 - ((double)num + num2))));
					if (num4 != 0)
					{
						num7 *= Mathf.Pow(10f, num4);
					}
					result = num7;
				}
			}
			return true;
		}

		public bool ParseBooleanValue(out JsonValue result)
		{
			SkipWhitespace();
			if (SkipString("true"))
			{
				result = true;
				return true;
			}
			if (SkipString("false"))
			{
				result = false;
				return true;
			}
			result = default(JsonValue);
			return false;
		}

		public bool ParseNullValue(out JsonValue result)
		{
			result = default(JsonValue);
			return SkipString("null");
		}

		public bool SkipToValue()
		{
			SkipWhitespace();
			if (m_Position == m_Length || m_Text[m_Position] != ':')
			{
				return false;
			}
			m_Position++;
			SkipWhitespace();
			return true;
		}

		private bool SkipString(string text)
		{
			SkipWhitespace();
			int length = text.Length;
			if (m_Position + length >= m_Length)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (m_Text[m_Position + i] != text[i])
				{
					return false;
				}
			}
			m_Position += length;
			return true;
		}

		private void SkipWhitespace()
		{
			while (m_Position < m_Length && char.IsWhiteSpace(m_Text[m_Position]))
			{
				m_Position++;
			}
		}
	}
}
