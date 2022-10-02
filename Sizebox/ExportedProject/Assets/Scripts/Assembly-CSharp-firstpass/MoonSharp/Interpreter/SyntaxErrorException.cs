using System;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Tree;

namespace MoonSharp.Interpreter
{
	[Serializable]
	public class SyntaxErrorException : InterpreterException
	{
		internal Token Token { get; private set; }

		public bool IsPrematureStreamTermination { get; set; }

		internal SyntaxErrorException(Token t, string format, params object[] args)
			: base(format, args)
		{
			Token = t;
		}

		internal SyntaxErrorException(Token t, string message)
			: base(message)
		{
			Token = t;
		}

		internal SyntaxErrorException(Script script, SourceRef sref, string format, params object[] args)
			: base(format, args)
		{
			DecorateMessage(script, sref);
		}

		internal SyntaxErrorException(Script script, SourceRef sref, string message)
			: base(message)
		{
			DecorateMessage(script, sref);
		}

		private SyntaxErrorException(SyntaxErrorException syntaxErrorException)
			: base(syntaxErrorException, syntaxErrorException.DecoratedMessage)
		{
			Token = syntaxErrorException.Token;
			base.DecoratedMessage = Message;
		}

		internal void DecorateMessage(Script script)
		{
			if (Token != null)
			{
				DecorateMessage(script, Token.GetSourceRef(false));
			}
		}

		public override void Rethrow()
		{
			if (Script.GlobalOptions.RethrowExceptionNested)
			{
				throw new SyntaxErrorException(this);
			}
		}
	}
}
