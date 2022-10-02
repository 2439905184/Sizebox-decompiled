using System.Collections.Generic;
using System.Linq;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ScopesResponseBody : ResponseBody
	{
		public Scope[] scopes { get; private set; }

		public ScopesResponseBody(List<Scope> scps = null)
		{
			if (scps == null)
			{
				scopes = new Scope[0];
			}
			else
			{
				scopes = Enumerable.ToArray(scps);
			}
		}
	}
}
