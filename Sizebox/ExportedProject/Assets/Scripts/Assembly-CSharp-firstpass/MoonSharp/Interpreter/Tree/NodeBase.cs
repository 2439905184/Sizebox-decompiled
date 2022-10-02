using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree
{
	internal abstract class NodeBase
	{
		public Script Script { get; private set; }

		protected ScriptLoadingContext LoadingContext { get; private set; }

		public NodeBase(ScriptLoadingContext lcontext)
		{
			Script = lcontext.Script;
		}

		public abstract void Compile(ByteCode bc);

		protected static Token UnexpectedTokenType(Token t)
		{
			throw new SyntaxErrorException(t, "unexpected symbol near '{0}'", t.Text)
			{
				IsPrematureStreamTermination = (t.Type == TokenType.Eof)
			};
		}

		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType)
		{
			Token current = lcontext.Lexer.Current;
			if (current.Type != tokenType)
			{
				return UnexpectedTokenType(current);
			}
			lcontext.Lexer.Next();
			return current;
		}

		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType1, TokenType tokenType2)
		{
			Token current = lcontext.Lexer.Current;
			if (current.Type != tokenType1 && current.Type != tokenType2)
			{
				return UnexpectedTokenType(current);
			}
			lcontext.Lexer.Next();
			return current;
		}

		protected static Token CheckTokenType(ScriptLoadingContext lcontext, TokenType tokenType1, TokenType tokenType2, TokenType tokenType3)
		{
			Token current = lcontext.Lexer.Current;
			if (current.Type != tokenType1 && current.Type != tokenType2 && current.Type != tokenType3)
			{
				return UnexpectedTokenType(current);
			}
			lcontext.Lexer.Next();
			return current;
		}

		protected static void CheckTokenTypeNotNext(ScriptLoadingContext lcontext, TokenType tokenType)
		{
			Token current = lcontext.Lexer.Current;
			if (current.Type != tokenType)
			{
				UnexpectedTokenType(current);
			}
		}

		protected static Token CheckMatch(ScriptLoadingContext lcontext, Token originalToken, TokenType expectedTokenType, string expectedTokenText)
		{
			Token current = lcontext.Lexer.Current;
			if (current.Type != expectedTokenType)
			{
				throw new SyntaxErrorException(lcontext.Lexer.Current, "'{0}' expected (to close '{1}' at line {2}) near '{3}'", expectedTokenText, originalToken.Text, originalToken.FromLine, current.Text)
				{
					IsPrematureStreamTermination = (current.Type == TokenType.Eof)
				};
			}
			lcontext.Lexer.Next();
			return current;
		}
	}
}
