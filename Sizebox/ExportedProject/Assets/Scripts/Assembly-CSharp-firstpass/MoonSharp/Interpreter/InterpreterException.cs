using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;

namespace MoonSharp.Interpreter
{
	[Serializable]
	public class InterpreterException : Exception
	{
		public int InstructionPtr { get; internal set; }

		public IList<WatchItem> CallStack { get; internal set; }

		public string DecoratedMessage { get; internal set; }

		public bool DoNotDecorateMessage { get; set; }

		protected InterpreterException(Exception ex, string message)
			: base(message, ex)
		{
		}

		protected InterpreterException(Exception ex)
			: base(ex.Message, ex)
		{
		}

		protected InterpreterException(string message)
			: base(message)
		{
		}

		protected InterpreterException(string format, params object[] args)
			: base(string.Format(format, args))
		{
		}

		internal void DecorateMessage(Script script, SourceRef sref, int ip = -1)
		{
			if (string.IsNullOrEmpty(DecoratedMessage))
			{
				if (DoNotDecorateMessage)
				{
					DecoratedMessage = Message;
				}
				else if (sref != null)
				{
					DecoratedMessage = string.Format("{0}: {1}", sref.FormatLocation(script), Message);
				}
				else
				{
					DecoratedMessage = string.Format("bytecode:{0}: {1}", ip, Message);
				}
			}
		}

		public virtual void Rethrow()
		{
		}
	}
}
