using System.Collections.Generic;
using System.Linq;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ThreadsResponseBody : ResponseBody
	{
		public Thread[] threads { get; private set; }

		public ThreadsResponseBody(List<Thread> vars = null)
		{
			if (vars == null)
			{
				threads = new Thread[0];
			}
			else
			{
				threads = Enumerable.ToArray(vars);
			}
		}
	}
}
