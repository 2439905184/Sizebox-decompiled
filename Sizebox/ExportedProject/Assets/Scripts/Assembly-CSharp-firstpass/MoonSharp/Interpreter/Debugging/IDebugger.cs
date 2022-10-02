using System.Collections.Generic;

namespace MoonSharp.Interpreter.Debugging
{
	public interface IDebugger
	{
		DebuggerCaps GetDebuggerCaps();

		void SetDebugService(DebugService debugService);

		void SetSourceCode(SourceCode sourceCode);

		void SetByteCode(string[] byteCode);

		bool IsPauseRequested();

		bool SignalRuntimeException(ScriptRuntimeException ex);

		DebuggerAction GetAction(int ip, SourceRef sourceref);

		void SignalExecutionEnded();

		void Update(WatchType watchType, IEnumerable<WatchItem> items);

		List<DynamicExpression> GetWatchItems();

		void RefreshBreakpoints(IEnumerable<SourceRef> refs);
	}
}
