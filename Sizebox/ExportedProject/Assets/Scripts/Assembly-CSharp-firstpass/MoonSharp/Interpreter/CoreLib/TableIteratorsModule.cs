namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class TableIteratorsModule
	{
		[MoonSharpModuleMethod]
		public static DynValue ipairs(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			return executionContext.GetMetamethodTailCall(dynValue, "__ipairs", args.GetArray()) ?? DynValue.NewTuple(DynValue.NewCallback(__next_i), dynValue, DynValue.NewNumber(0.0));
		}

		[MoonSharpModuleMethod]
		public static DynValue pairs(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			return executionContext.GetMetamethodTailCall(dynValue, "__pairs", args.GetArray()) ?? DynValue.NewTuple(DynValue.NewCallback(next), dynValue);
		}

		[MoonSharpModuleMethod]
		public static DynValue next(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "next", DataType.Table);
			DynValue v = args[1];
			TablePair? tablePair = dynValue.Table.NextKey(v);
			if (tablePair.HasValue)
			{
				return DynValue.NewTuple(tablePair.Value.Key, tablePair.Value.Value);
			}
			throw new ScriptRuntimeException("invalid key to 'next'");
		}

		public static DynValue __next_i(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "!!next_i!!", DataType.Table);
			int num = (int)args.AsType(1, "!!next_i!!", DataType.Number).Number + 1;
			DynValue dynValue2 = dynValue.Table.Get(num);
			if (dynValue2.Type != 0)
			{
				return DynValue.NewTuple(DynValue.NewNumber(num), dynValue2);
			}
			return DynValue.Nil;
		}
	}
}
