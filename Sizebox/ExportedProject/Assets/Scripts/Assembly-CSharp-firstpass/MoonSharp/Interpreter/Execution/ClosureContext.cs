using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MoonSharp.Interpreter.Execution
{
	internal class ClosureContext : List<DynValue>
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<SymbolRef, string> _003C_003E9__4_0;

			internal string _003C_002Ector_003Eb__4_0(SymbolRef s)
			{
				return s.i_Name;
			}
		}

		public string[] Symbols { get; private set; }

		internal ClosureContext(SymbolRef[] symbols, IEnumerable<DynValue> values)
		{
			Symbols = symbols.Select(_003C_003Ec._003C_003E9__4_0 ?? (_003C_003Ec._003C_003E9__4_0 = _003C_003Ec._003C_003E9._003C_002Ector_003Eb__4_0)).ToArray();
			AddRange(values);
		}

		internal ClosureContext()
		{
			Symbols = new string[0];
		}
	}
}
