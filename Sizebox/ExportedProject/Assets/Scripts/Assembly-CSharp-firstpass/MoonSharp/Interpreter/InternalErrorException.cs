using System;

namespace MoonSharp.Interpreter
{
	[Serializable]
	public class InternalErrorException : InterpreterException
	{
		internal InternalErrorException(string message)
			: base(message)
		{
		}

		internal InternalErrorException(string format, params object[] args)
			: base(format, args)
		{
		}
	}
}
