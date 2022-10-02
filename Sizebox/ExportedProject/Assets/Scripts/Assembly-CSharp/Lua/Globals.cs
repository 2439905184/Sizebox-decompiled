using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class Globals
	{
		private Dictionary<DynValue, DynValue> globalTable;

		public DynValue this[DynValue idx]
		{
			get
			{
				if (globalTable.ContainsKey(idx))
				{
					return globalTable[idx];
				}
				return null;
			}
			set
			{
				globalTable[idx] = value;
			}
		}

		[MoonSharpHidden]
		public Globals()
		{
			globalTable = new Dictionary<DynValue, DynValue>();
		}
	}
}
