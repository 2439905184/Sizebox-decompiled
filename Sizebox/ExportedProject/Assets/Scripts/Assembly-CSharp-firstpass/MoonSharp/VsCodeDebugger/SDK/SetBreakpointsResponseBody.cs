using System.Collections.Generic;
using System.Linq;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class SetBreakpointsResponseBody : ResponseBody
	{
		public Breakpoint[] breakpoints { get; private set; }

		public SetBreakpointsResponseBody(List<Breakpoint> bpts = null)
		{
			if (bpts == null)
			{
				breakpoints = new Breakpoint[0];
			}
			else
			{
				breakpoints = Enumerable.ToArray(bpts);
			}
		}
	}
}
