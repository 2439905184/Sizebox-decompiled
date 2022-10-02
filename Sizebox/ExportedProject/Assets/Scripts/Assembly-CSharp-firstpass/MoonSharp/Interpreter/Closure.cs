using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Execution;

namespace MoonSharp.Interpreter
{
	public class Closure : RefIdObject, IScriptPrivateResource
	{
		public enum UpvaluesType
		{
			None = 0,
			Environment = 1,
			Closure = 2
		}

		private static ClosureContext emptyClosure = new ClosureContext();

		public int EntryPointByteCodeLocation { get; private set; }

		public Script OwnerScript { get; private set; }

		internal ClosureContext ClosureContext { get; private set; }

		internal Closure(Script script, int idx, SymbolRef[] symbols, IEnumerable<DynValue> resolvedLocals)
		{
			OwnerScript = script;
			EntryPointByteCodeLocation = idx;
			if (symbols.Length != 0)
			{
				ClosureContext = new ClosureContext(symbols, resolvedLocals);
			}
			else
			{
				ClosureContext = emptyClosure;
			}
		}

		public DynValue Call()
		{
			return OwnerScript.Call(this);
		}

		public DynValue Call(params object[] args)
		{
			return OwnerScript.Call(this, args);
		}

		public DynValue Call(params DynValue[] args)
		{
			return OwnerScript.Call(this, args);
		}

		public ScriptFunctionDelegate GetDelegate()
		{
			return _003CGetDelegate_003Eb__18_0;
		}

		public ScriptFunctionDelegate<T> GetDelegate<T>()
		{
			return _003CGetDelegate_003Eb__19_0<T>;
		}

		public int GetUpvaluesCount()
		{
			return ClosureContext.Count;
		}

		public string GetUpvalueName(int idx)
		{
			return ClosureContext.Symbols[idx];
		}

		public DynValue GetUpvalue(int idx)
		{
			return ClosureContext[idx];
		}

		public UpvaluesType GetUpvaluesType()
		{
			switch (GetUpvaluesCount())
			{
			case 0:
				return UpvaluesType.None;
			case 1:
				if (GetUpvalueName(0) == "_ENV")
				{
					return UpvaluesType.Environment;
				}
				break;
			}
			return UpvaluesType.Closure;
		}

		[CompilerGenerated]
		private object _003CGetDelegate_003Eb__18_0(object[] args)
		{
			return Call(args).ToObject();
		}

		[CompilerGenerated]
		private T _003CGetDelegate_003Eb__19_0<T>(object[] args)
		{
			return Call(args).ToObject<T>();
		}
	}
}
