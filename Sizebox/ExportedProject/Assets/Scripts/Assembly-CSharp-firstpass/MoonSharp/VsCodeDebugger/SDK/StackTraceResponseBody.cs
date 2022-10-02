using System.Collections.Generic;
using System.Linq;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class StackTraceResponseBody : ResponseBody
	{
		public StackFrame[] stackFrames { get; private set; }

		public StackTraceResponseBody(List<StackFrame> frames = null)
		{
			if (frames == null)
			{
				stackFrames = new StackFrame[0];
			}
			else
			{
				stackFrames = Enumerable.ToArray(frames);
			}
		}
	}
}
