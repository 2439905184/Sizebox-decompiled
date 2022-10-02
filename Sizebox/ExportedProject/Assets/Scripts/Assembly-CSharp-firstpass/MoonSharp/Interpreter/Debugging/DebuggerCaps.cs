using System;

namespace MoonSharp.Interpreter.Debugging
{
	[Flags]
	public enum DebuggerCaps
	{
		CanDebugSourceCode = 1,
		CanDebugByteCode = 2,
		HasLineBasedBreakpoints = 4
	}
}
