using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine.InputSystem.Utilities
{
	public struct NameAndParameters
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<NamedValue, string> _003C_003E9__8_0;

			internal string _003CToString_003Eb__8_0(NamedValue x)
			{
				return x.ToString();
			}
		}

		public string name { get; set; }

		public ReadOnlyArray<NamedValue> parameters { get; set; }

		public override string ToString()
		{
			if (parameters.Count == 0)
			{
				return name;
			}
			string text = string.Join(",", parameters.Select(_003C_003Ec._003C_003E9__8_0 ?? (_003C_003Ec._003C_003E9__8_0 = _003C_003Ec._003C_003E9._003CToString_003Eb__8_0)).ToArray());
			return name + "(" + text + ")";
		}

		public static IEnumerable<NameAndParameters> ParseMultiple(string text)
		{
			List<NameAndParameters> list = null;
			if (!ParseMultiple(text, ref list))
			{
				return Enumerable.Empty<NameAndParameters>();
			}
			return list;
		}

		internal static bool ParseMultiple(string text, ref List<NameAndParameters> list)
		{
			text = text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (list == null)
			{
				list = new List<NameAndParameters>();
			}
			else
			{
				list.Clear();
			}
			int index = 0;
			int length = text.Length;
			while (index < length)
			{
				list.Add(ParseNameAndParameters(text, ref index));
			}
			return true;
		}

		public static NameAndParameters Parse(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			int index = 0;
			return ParseNameAndParameters(text, ref index);
		}

		private static NameAndParameters ParseNameAndParameters(string text, ref int index)
		{
			int length = text.Length;
			while (index < length && char.IsWhiteSpace(text[index]))
			{
				index++;
			}
			int num = index;
			while (index < length)
			{
				char c = text[index];
				if (c == '(' || c == ","[0] || char.IsWhiteSpace(c))
				{
					break;
				}
				index++;
			}
			if (index - num == 0)
			{
				throw new ArgumentException(string.Format("Expecting name at position {0} in '{1}'", num, text), "text");
			}
			string text2 = text.Substring(num, index - num);
			while (index < length && char.IsWhiteSpace(text[index]))
			{
				index++;
			}
			NamedValue[] array = null;
			if (index < length && text[index] == '(')
			{
				index++;
				int num2 = text.IndexOf(')', index);
				if (num2 == -1)
				{
					throw new ArgumentException(string.Format("Expecting ')' after '(' at position {0} in '{1}'", index, text), "text");
				}
				array = NamedValue.ParseMultiple(text.Substring(index, num2 - index));
				index = num2 + 1;
			}
			if (index < length && (text[index] == ',' || text[index] == ';'))
			{
				index++;
			}
			NameAndParameters result = default(NameAndParameters);
			result.name = text2;
			result.parameters = new ReadOnlyArray<NamedValue>(array);
			return result;
		}
	}
}
