using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class AllMicros
	{
		public IDictionary<int, Entity> list
		{
			get
			{
				return MicroManager.Instance.GetLuaMicroList();
			}
		}
	}
}
