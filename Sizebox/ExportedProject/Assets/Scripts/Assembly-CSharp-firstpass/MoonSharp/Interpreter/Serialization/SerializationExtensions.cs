using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Serialization
{
	public static class SerializationExtensions
	{
		private static HashSet<string> LUAKEYWORDS = new HashSet<string>
		{
			"and", "break", "do", "else", "elseif", "end", "false", "for", "function", "goto",
			"if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true",
			"until", "while"
		};

		public static string Serialize(this Table table, bool prefixReturn = false, int tabs = 0)
		{
			if (table.OwnerScript != null)
			{
				throw new ScriptRuntimeException("Table is not a prime table.");
			}
			string value = new string('\t', tabs);
			StringBuilder stringBuilder = new StringBuilder();
			if (prefixReturn)
			{
				stringBuilder.Append("return ");
			}
			if (!table.Values.Any())
			{
				stringBuilder.Append("${ }");
				return stringBuilder.ToString();
			}
			stringBuilder.AppendLine("${");
			foreach (TablePair pair in table.Pairs)
			{
				stringBuilder.Append(value);
				string arg = (IsStringIdentifierValid(pair.Key) ? pair.Key.String : ("[" + pair.Key.SerializeValue(tabs + 1) + "]"));
				stringBuilder.AppendFormat("\t{0} = {1},\n", arg, pair.Value.SerializeValue(tabs + 1));
			}
			stringBuilder.Append(value);
			stringBuilder.Append("}");
			if (tabs == 0)
			{
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		private static bool IsStringIdentifierValid(DynValue dynValue)
		{
			if (dynValue.Type != DataType.String)
			{
				return false;
			}
			if (dynValue.String.Length == 0)
			{
				return false;
			}
			if (LUAKEYWORDS.Contains(dynValue.String))
			{
				return false;
			}
			if (!char.IsLetter(dynValue.String[0]) && dynValue.String[0] != '_')
			{
				return false;
			}
			string @string = dynValue.String;
			foreach (char c in @string)
			{
				if (!char.IsLetterOrDigit(c) && c != '_')
				{
					return false;
				}
			}
			return true;
		}

		public static string SerializeValue(this DynValue dynValue, int tabs = 0)
		{
			if (dynValue.Type == DataType.Nil || dynValue.Type == DataType.Void)
			{
				return "nil";
			}
			if (dynValue.Type == DataType.Tuple)
			{
				if (!dynValue.Tuple.Any())
				{
					return "nil";
				}
				return dynValue.Tuple[0].SerializeValue(tabs);
			}
			if (dynValue.Type == DataType.Number)
			{
				return dynValue.Number.ToString("r");
			}
			if (dynValue.Type == DataType.Boolean)
			{
				if (!dynValue.Boolean)
				{
					return "false";
				}
				return "true";
			}
			if (dynValue.Type == DataType.String)
			{
				return EscapeString(dynValue.String ?? "");
			}
			if (dynValue.Type == DataType.Table && dynValue.Table.OwnerScript == null)
			{
				return dynValue.Table.Serialize(false, tabs);
			}
			throw new ScriptRuntimeException("Value is not a primitive value or a prime table.");
		}

		private static string EscapeString(string s)
		{
			s = s.Replace("\\", "\\\\");
			s = s.Replace("\n", "\\n");
			s = s.Replace("\r", "\\r");
			s = s.Replace("\t", "\\t");
			s = s.Replace("\a", "\\a");
			s = s.Replace("\f", "\\f");
			s = s.Replace("\b", "\\b");
			s = s.Replace("\v", "\\v");
			s = s.Replace("\"", "\\\"");
			s = s.Replace("'", "\\'");
			return "\"" + s + "\"";
		}
	}
}
