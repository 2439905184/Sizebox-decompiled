namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class MetaTableModule
	{
		[MoonSharpModuleMethod]
		public static DynValue setmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "setmetatable", DataType.Table);
			DynValue dynValue2 = args.AsType(1, "setmetatable", DataType.Table, true);
			if (executionContext.GetMetamethod(dynValue, "__metatable") != null)
			{
				throw new ScriptRuntimeException("cannot change a protected metatable");
			}
			dynValue.Table.MetaTable = dynValue2.Table;
			return dynValue;
		}

		[MoonSharpModuleMethod]
		public static DynValue getmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			Table table = null;
			if (dynValue.Type.CanHaveTypeMetatables())
			{
				table = executionContext.GetScript().GetTypeMetatable(dynValue.Type);
			}
			if (dynValue.Type == DataType.Table)
			{
				table = dynValue.Table.MetaTable;
			}
			if (table == null)
			{
				return DynValue.Nil;
			}
			if (table.RawGet("__metatable") != null)
			{
				return table.Get("__metatable");
			}
			return DynValue.NewTable(table);
		}

		[MoonSharpModuleMethod]
		public static DynValue rawget(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "rawget", DataType.Table);
			DynValue key = args[1];
			return dynValue.Table.Get(key);
		}

		[MoonSharpModuleMethod]
		public static DynValue rawset(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "rawset", DataType.Table);
			DynValue key = args[1];
			dynValue.Table.Set(key, args[2]);
			return dynValue;
		}

		[MoonSharpModuleMethod]
		public static DynValue rawequal(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			DynValue obj = args[1];
			return DynValue.NewBoolean(dynValue.Equals(obj));
		}

		[MoonSharpModuleMethod]
		public static DynValue rawlen(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.String && args[0].Type != DataType.Table)
			{
				throw ScriptRuntimeException.BadArgument(0, "rawlen", "table or string", args[0].Type.ToErrorTypeString(), false);
			}
			return args[0].GetLength();
		}
	}
}
