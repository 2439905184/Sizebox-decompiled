using System.Collections.Generic;
using System.Linq;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class VariablesResponseBody : ResponseBody
	{
		public Variable[] variables { get; private set; }

		public VariablesResponseBody(List<Variable> vars = null)
		{
			if (vars == null)
			{
				variables = new Variable[0];
			}
			else
			{
				variables = Enumerable.ToArray(vars);
			}
		}
	}
}
