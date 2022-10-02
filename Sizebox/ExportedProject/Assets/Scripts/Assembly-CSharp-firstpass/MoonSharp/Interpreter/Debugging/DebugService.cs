using System.Collections.Generic;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Debugging
{
	public sealed class DebugService : IScriptPrivateResource
	{
		private Processor m_Processor;

		public Script OwnerScript { get; private set; }

		internal DebugService(Script script, Processor processor)
		{
			OwnerScript = script;
			m_Processor = processor;
		}

		public HashSet<int> ResetBreakPoints(SourceCode src, HashSet<int> lines)
		{
			return m_Processor.ResetBreakPoints(src, lines);
		}
	}
}
