using System.Text;

namespace MoonSharp.Interpreter.Tree
{
	internal class Lexer
	{
		private Token m_Current;

		private string m_Code;

		private int m_PrevLineTo;

		private int m_PrevColTo = 1;

		private int m_Cursor;

		private int m_Line = 1;

		private int m_Col;

		private int m_SourceId;

		private bool m_AutoSkipComments;

		public Token Current
		{
			get
			{
				if (m_Current == null)
				{
					Next();
				}
				return m_Current;
			}
		}

		public Lexer(int sourceID, string scriptContent, bool autoSkipComments)
		{
			m_Code = scriptContent;
			m_SourceId = sourceID;
			if (m_Code.Length > 0 && m_Code[0] == '\ufeff')
			{
				m_Code = m_Code.Substring(1);
			}
			m_AutoSkipComments = autoSkipComments;
		}

		private Token FetchNewToken()
		{
			Token token;
			do
			{
				token = ReadToken();
			}
			while ((token.Type == TokenType.Comment || token.Type == TokenType.HashBang) && m_AutoSkipComments);
			return token;
		}

		public void Next()
		{
			m_Current = FetchNewToken();
		}

		public Token PeekNext()
		{
			int cursor = m_Cursor;
			Token current = m_Current;
			int line = m_Line;
			int col = m_Col;
			Next();
			Token current2 = Current;
			m_Cursor = cursor;
			m_Current = current;
			m_Line = line;
			m_Col = col;
			return current2;
		}

		private void CursorNext()
		{
			if (CursorNotEof())
			{
				if (CursorChar() == '\n')
				{
					m_Col = 0;
					m_Line++;
				}
				else
				{
					m_Col++;
				}
				m_Cursor++;
			}
		}

		private char CursorChar()
		{
			if (m_Cursor < m_Code.Length)
			{
				return m_Code[m_Cursor];
			}
			return '\0';
		}

		private char CursorCharNext()
		{
			CursorNext();
			return CursorChar();
		}

		private bool CursorMatches(string pattern)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				int num = m_Cursor + i;
				if (num >= m_Code.Length)
				{
					return false;
				}
				if (m_Code[num] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		private bool CursorNotEof()
		{
			return m_Cursor < m_Code.Length;
		}

		private bool IsWhiteSpace(char c)
		{
			return char.IsWhiteSpace(c);
		}

		private void SkipWhiteSpace()
		{
			while (CursorNotEof() && IsWhiteSpace(CursorChar()))
			{
				CursorNext();
			}
		}

		private Token ReadToken()
		{
			SkipWhiteSpace();
			int line = m_Line;
			int col = m_Col;
			if (!CursorNotEof())
			{
				return CreateToken(TokenType.Eof, line, col, "<eof>");
			}
			char c = CursorChar();
			switch (c)
			{
			case '|':
				CursorCharNext();
				return CreateToken(TokenType.Lambda, line, col, "|");
			case ';':
				CursorCharNext();
				return CreateToken(TokenType.SemiColon, line, col, ";");
			case '=':
				return PotentiallyDoubleCharOperator('=', TokenType.Op_Assignment, TokenType.Op_Equal, line, col);
			case '<':
				return PotentiallyDoubleCharOperator('=', TokenType.Op_LessThan, TokenType.Op_LessThanEqual, line, col);
			case '>':
				return PotentiallyDoubleCharOperator('=', TokenType.Op_GreaterThan, TokenType.Op_GreaterThanEqual, line, col);
			case '!':
			case '~':
				if (CursorCharNext() != '=')
				{
					throw new SyntaxErrorException(CreateToken(TokenType.Invalid, line, col), "unexpected symbol near '{0}'", c);
				}
				CursorCharNext();
				return CreateToken(TokenType.Op_NotEqual, line, col, "~=");
			case '.':
			{
				char c3 = CursorCharNext();
				if (c3 == '.')
				{
					return PotentiallyDoubleCharOperator('.', TokenType.Op_Concat, TokenType.VarArgs, line, col);
				}
				if (LexerUtils.CharIsDigit(c3))
				{
					return ReadNumberToken(line, col, true);
				}
				return CreateToken(TokenType.Dot, line, col, ".");
			}
			case '+':
				return CreateSingleCharToken(TokenType.Op_Add, line, col);
			case '-':
				if (CursorCharNext() == '-')
				{
					return ReadComment(line, col);
				}
				return CreateToken(TokenType.Op_MinusOrSub, line, col, "-");
			case '*':
				return CreateSingleCharToken(TokenType.Op_Mul, line, col);
			case '/':
				return CreateSingleCharToken(TokenType.Op_Div, line, col);
			case '%':
				return CreateSingleCharToken(TokenType.Op_Mod, line, col);
			case '^':
				return CreateSingleCharToken(TokenType.Op_Pwr, line, col);
			case '$':
				return PotentiallyDoubleCharOperator('{', TokenType.Op_Dollar, TokenType.Brk_Open_Curly_Shared, line, col);
			case '#':
				if (m_Cursor == 0 && m_Code.Length > 1 && m_Code[1] == '!')
				{
					return ReadHashBang(line, col);
				}
				return CreateSingleCharToken(TokenType.Op_Len, line, col);
			case '[':
			{
				char c2 = CursorCharNext();
				if (c2 == '=' || c2 == '[')
				{
					string text = ReadLongString(line, col, null, "string");
					return CreateToken(TokenType.String_Long, line, col, text);
				}
				return CreateToken(TokenType.Brk_Open_Square, line, col, "[");
			}
			case ']':
				return CreateSingleCharToken(TokenType.Brk_Close_Square, line, col);
			case '(':
				return CreateSingleCharToken(TokenType.Brk_Open_Round, line, col);
			case ')':
				return CreateSingleCharToken(TokenType.Brk_Close_Round, line, col);
			case '{':
				return CreateSingleCharToken(TokenType.Brk_Open_Curly, line, col);
			case '}':
				return CreateSingleCharToken(TokenType.Brk_Close_Curly, line, col);
			case ',':
				return CreateSingleCharToken(TokenType.Comma, line, col);
			case ':':
				return PotentiallyDoubleCharOperator(':', TokenType.Colon, TokenType.DoubleColon, line, col);
			case '"':
			case '\'':
				return ReadSimpleStringToken(line, col);
			case '\0':
				throw new SyntaxErrorException(CreateToken(TokenType.Invalid, line, col), "unexpected symbol near '{0}'", CursorChar())
				{
					IsPrematureStreamTermination = true
				};
			default:
				if (char.IsLetter(c) || c == '_')
				{
					string name = ReadNameToken();
					return CreateNameToken(name, line, col);
				}
				if (LexerUtils.CharIsDigit(c))
				{
					return ReadNumberToken(line, col, false);
				}
				throw new SyntaxErrorException(CreateToken(TokenType.Invalid, line, col), "unexpected symbol near '{0}'", CursorChar());
			}
		}

		private string ReadLongString(int fromLine, int fromCol, string startpattern, string subtypeforerrors)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			string text = "]";
			if (startpattern == null)
			{
				char c = CursorChar();
				while (true)
				{
					if (c == '\0' || !CursorNotEof())
					{
						throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unfinished long {0} near '<eof>'", subtypeforerrors)
						{
							IsPrematureStreamTermination = true
						};
					}
					switch (c)
					{
					case '=':
						goto IL_0056;
					case '[':
						break;
					default:
						throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "invalid long {0} delimiter near '{1}'", subtypeforerrors, c)
						{
							IsPrematureStreamTermination = true
						};
					}
					break;
					IL_0056:
					text += "=";
					c = CursorCharNext();
				}
				text += "]";
			}
			else
			{
				text = startpattern.Replace('[', ']');
			}
			char c2 = CursorCharNext();
			while (true)
			{
				if (c2 != '\r')
				{
					if (c2 == '\0' || !CursorNotEof())
					{
						throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unfinished long {0} near '{1}'", subtypeforerrors, stringBuilder.ToString())
						{
							IsPrematureStreamTermination = true
						};
					}
					if (c2 == ']' && CursorMatches(text))
					{
						break;
					}
					stringBuilder.Append(c2);
				}
				c2 = CursorCharNext();
			}
			for (int i = 0; i < text.Length; i++)
			{
				CursorCharNext();
			}
			return LexerUtils.AdjustLuaLongString(stringBuilder.ToString());
		}

		private Token ReadNumberToken(int fromLine, int fromCol, bool leadingDot)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (leadingDot)
			{
				stringBuilder.Append("0.");
			}
			else if (CursorChar() == '0')
			{
				stringBuilder.Append(CursorChar());
				char c = CursorCharNext();
				if (c == 'x' || c == 'X')
				{
					flag = true;
					stringBuilder.Append(CursorChar());
					CursorCharNext();
				}
			}
			char c2 = CursorChar();
			while (CursorNotEof())
			{
				if (flag4 && (c2 == '+' || c2 == '-'))
				{
					flag4 = false;
					stringBuilder.Append(c2);
				}
				else if (LexerUtils.CharIsDigit(c2))
				{
					stringBuilder.Append(c2);
				}
				else if (c2 == '.' && !flag2)
				{
					flag2 = true;
					stringBuilder.Append(c2);
				}
				else if (LexerUtils.CharIsHexDigit(c2) && flag && !flag3)
				{
					stringBuilder.Append(c2);
				}
				else
				{
					if (c2 != 'e' && c2 != 'E' && (!flag || (c2 != 'p' && c2 != 'P')))
					{
						break;
					}
					stringBuilder.Append(c2);
					flag3 = true;
					flag4 = true;
					flag2 = true;
				}
				c2 = CursorCharNext();
			}
			TokenType tokenType = TokenType.Number;
			if (flag && (flag2 || flag3))
			{
				tokenType = TokenType.Number_HexFloat;
			}
			else if (flag)
			{
				tokenType = TokenType.Number_Hex;
			}
			string text = stringBuilder.ToString();
			return CreateToken(tokenType, fromLine, fromCol, text);
		}

		private Token CreateSingleCharToken(TokenType tokenType, int fromLine, int fromCol)
		{
			char c = CursorChar();
			CursorCharNext();
			return CreateToken(tokenType, fromLine, fromCol, c.ToString());
		}

		private Token ReadHashBang(int fromLine, int fromCol)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			char c = CursorChar();
			while (CursorNotEof())
			{
				switch (c)
				{
				case '\n':
					CursorCharNext();
					return CreateToken(TokenType.HashBang, fromLine, fromCol, stringBuilder.ToString());
				default:
					stringBuilder.Append(c);
					break;
				case '\r':
					break;
				}
				c = CursorCharNext();
			}
			return CreateToken(TokenType.HashBang, fromLine, fromCol, stringBuilder.ToString());
		}

		private Token ReadComment(int fromLine, int fromCol)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			bool flag = false;
			char c = CursorCharNext();
			while (CursorNotEof())
			{
				if (c == '[' && !flag && stringBuilder.Length > 0)
				{
					stringBuilder.Append('[');
					string text = ReadLongString(fromLine, fromCol, stringBuilder.ToString(), "comment");
					return CreateToken(TokenType.Comment, fromLine, fromCol, text);
				}
				if (c == '\n')
				{
					flag = true;
					CursorCharNext();
					return CreateToken(TokenType.Comment, fromLine, fromCol, stringBuilder.ToString());
				}
				if (c != '\r')
				{
					if (c != '[' && c != '=')
					{
						flag = true;
					}
					stringBuilder.Append(c);
				}
				c = CursorCharNext();
			}
			return CreateToken(TokenType.Comment, fromLine, fromCol, stringBuilder.ToString());
		}

		private Token ReadSimpleStringToken(int fromLine, int fromCol)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			char c = CursorChar();
			char c2 = CursorCharNext();
			while (CursorNotEof())
			{
				while (true)
				{
					switch (c2)
					{
					case '\\':
						stringBuilder.Append(c2);
						c2 = CursorCharNext();
						stringBuilder.Append(c2);
						switch (c2)
						{
						case '\r':
							c2 = CursorCharNext();
							if (c2 != '\n')
							{
								continue;
							}
							stringBuilder.Append(c2);
							break;
						case 'z':
							c2 = CursorCharNext();
							if (char.IsWhiteSpace(c2))
							{
								SkipWhiteSpace();
							}
							c2 = CursorChar();
							continue;
						}
						break;
					case '\n':
					case '\r':
						throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unfinished string near '{0}'", stringBuilder.ToString());
					default:
						if (c2 == c)
						{
							CursorCharNext();
							Token token = CreateToken(TokenType.String, fromLine, fromCol);
							token.Text = LexerUtils.UnescapeLuaString(token, stringBuilder.ToString());
							return token;
						}
						stringBuilder.Append(c2);
						break;
					}
					break;
				}
				c2 = CursorCharNext();
			}
			throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unfinished string near '{0}'", stringBuilder.ToString())
			{
				IsPrematureStreamTermination = true
			};
		}

		private Token PotentiallyDoubleCharOperator(char expectedSecondChar, TokenType singleCharToken, TokenType doubleCharToken, int fromLine, int fromCol)
		{
			string text = CursorChar().ToString();
			CursorCharNext();
			if (CursorChar() == expectedSecondChar)
			{
				CursorCharNext();
				return CreateToken(doubleCharToken, fromLine, fromCol, text + expectedSecondChar);
			}
			return CreateToken(singleCharToken, fromLine, fromCol, text);
		}

		private Token CreateNameToken(string name, int fromLine, int fromCol)
		{
			TokenType? reservedTokenType = Token.GetReservedTokenType(name);
			if (reservedTokenType.HasValue)
			{
				return CreateToken(reservedTokenType.Value, fromLine, fromCol, name);
			}
			return CreateToken(TokenType.Name, fromLine, fromCol, name);
		}

		private Token CreateToken(TokenType tokenType, int fromLine, int fromCol, string text = null)
		{
			Token result = new Token(tokenType, m_SourceId, fromLine, fromCol, m_Line, m_Col, m_PrevLineTo, m_PrevColTo)
			{
				Text = text
			};
			m_PrevLineTo = m_Line;
			m_PrevColTo = m_Col;
			return result;
		}

		private string ReadNameToken()
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			char c = CursorChar();
			while (CursorNotEof() && (char.IsLetterOrDigit(c) || c == '_'))
			{
				stringBuilder.Append(c);
				c = CursorCharNext();
			}
			return stringBuilder.ToString();
		}
	}
}
