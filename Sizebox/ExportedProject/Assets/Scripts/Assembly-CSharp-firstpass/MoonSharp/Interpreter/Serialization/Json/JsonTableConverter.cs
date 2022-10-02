using System.Text;
using MoonSharp.Interpreter.Tree;

namespace MoonSharp.Interpreter.Serialization.Json
{
	public static class JsonTableConverter
	{
		public static string TableToJson(this Table table)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TableToJson(stringBuilder, table);
			return stringBuilder.ToString();
		}

		private static void TableToJson(StringBuilder sb, Table table)
		{
			bool flag = true;
			if (table.Length == 0)
			{
				sb.Append("{");
				foreach (TablePair pair in table.Pairs)
				{
					if (pair.Key.Type == DataType.String && IsValueJsonCompatible(pair.Value))
					{
						if (!flag)
						{
							sb.Append(',');
						}
						ValueToJson(sb, pair.Key);
						sb.Append(':');
						ValueToJson(sb, pair.Value);
						flag = false;
					}
				}
				sb.Append("}");
				return;
			}
			sb.Append("[");
			for (int i = 1; i <= table.Length; i++)
			{
				DynValue value = table.Get(i);
				if (IsValueJsonCompatible(value))
				{
					if (!flag)
					{
						sb.Append(',');
					}
					ValueToJson(sb, value);
					flag = false;
				}
			}
			sb.Append("]");
		}

		public static string ObjectToJson(object obj)
		{
			return ObjectValueConverter.SerializeObjectToDynValue(null, obj, JsonNull.Create()).Table.TableToJson();
		}

		private static void ValueToJson(StringBuilder sb, DynValue value)
		{
			switch (value.Type)
			{
			case DataType.Boolean:
				sb.Append(value.Boolean ? "true" : "false");
				break;
			case DataType.Number:
				sb.Append(value.Number.ToString("r"));
				break;
			case DataType.String:
				sb.Append(EscapeString(value.String ?? ""));
				break;
			case DataType.Table:
				TableToJson(sb, value.Table);
				break;
			default:
				sb.Append("null");
				break;
			}
		}

		private static string EscapeString(string s)
		{
			s = s.Replace("\\", "\\\\");
			s = s.Replace("/", "\\/");
			s = s.Replace("\"", "\\\"");
			s = s.Replace("\f", "\\f");
			s = s.Replace("\b", "\\b");
			s = s.Replace("\n", "\\n");
			s = s.Replace("\r", "\\r");
			s = s.Replace("\t", "\\t");
			return "\"" + s + "\"";
		}

		private static bool IsValueJsonCompatible(DynValue value)
		{
			if (value.Type != DataType.Boolean && !value.IsNil() && value.Type != DataType.Number && value.Type != DataType.String && value.Type != DataType.Table)
			{
				return JsonNull.IsJsonNull(value);
			}
			return true;
		}

		public static Table JsonToTable(string json, Script script = null)
		{
			Lexer lexer = new Lexer(0, json, false);
			if (lexer.Current.Type == TokenType.Brk_Open_Curly)
			{
				return ParseJsonObject(lexer, script);
			}
			if (lexer.Current.Type == TokenType.Brk_Open_Square)
			{
				return ParseJsonArray(lexer, script);
			}
			throw new SyntaxErrorException(lexer.Current, "Unexpected token : '{0}'", lexer.Current.Text);
		}

		private static void AssertToken(Lexer L, TokenType type)
		{
			if (L.Current.Type != type)
			{
				throw new SyntaxErrorException(L.Current, "Unexpected token : '{0}'", L.Current.Text);
			}
		}

		private static Table ParseJsonArray(Lexer L, Script script)
		{
			Table table = new Table(script);
			L.Next();
			while (L.Current.Type != TokenType.Brk_Close_Square)
			{
				DynValue value = ParseJsonValue(L, script);
				table.Append(value);
				L.Next();
				if (L.Current.Type == TokenType.Comma)
				{
					L.Next();
				}
			}
			return table;
		}

		private static Table ParseJsonObject(Lexer L, Script script)
		{
			Table table = new Table(script);
			L.Next();
			while (L.Current.Type != TokenType.Brk_Close_Curly)
			{
				AssertToken(L, TokenType.String);
				string text = L.Current.Text;
				L.Next();
				AssertToken(L, TokenType.Colon);
				L.Next();
				DynValue value = ParseJsonValue(L, script);
				table.Set(text, value);
				L.Next();
				if (L.Current.Type == TokenType.Comma)
				{
					L.Next();
				}
			}
			return table;
		}

		private static DynValue ParseJsonValue(Lexer L, Script script)
		{
			if (L.Current.Type == TokenType.Brk_Open_Curly)
			{
				return DynValue.NewTable(ParseJsonObject(L, script));
			}
			if (L.Current.Type == TokenType.Brk_Open_Square)
			{
				return DynValue.NewTable(ParseJsonArray(L, script));
			}
			if (L.Current.Type == TokenType.String)
			{
				return DynValue.NewString(L.Current.Text);
			}
			if (L.Current.Type == TokenType.Number)
			{
				return DynValue.NewNumber(L.Current.GetNumberValue()).AsReadOnly();
			}
			if (L.Current.Type == TokenType.True)
			{
				return DynValue.True;
			}
			if (L.Current.Type == TokenType.False)
			{
				return DynValue.False;
			}
			if (L.Current.Type == TokenType.Name && L.Current.Text == "null")
			{
				return JsonNull.Create();
			}
			throw new SyntaxErrorException(L.Current, "Unexpected token : '{0}'", L.Current.Text);
		}
	}
}
