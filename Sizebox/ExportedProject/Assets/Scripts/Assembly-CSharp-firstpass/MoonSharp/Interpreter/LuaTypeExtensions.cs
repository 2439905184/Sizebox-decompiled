namespace MoonSharp.Interpreter
{
	public static class LuaTypeExtensions
	{
		internal const DataType MaxMetaTypes = DataType.Table;

		internal const DataType MaxConvertibleTypes = DataType.ClrFunction;

		public static bool CanHaveTypeMetatables(this DataType type)
		{
			return type < DataType.Table;
		}

		public static string ToErrorTypeString(this DataType type)
		{
			switch (type)
			{
			case DataType.Void:
				return "no value";
			case DataType.Nil:
				return "nil";
			case DataType.Boolean:
				return "boolean";
			case DataType.Number:
				return "number";
			case DataType.String:
				return "string";
			case DataType.Function:
				return "function";
			case DataType.ClrFunction:
				return "function";
			case DataType.Table:
				return "table";
			case DataType.UserData:
				return "userdata";
			case DataType.Thread:
				return "coroutine";
			default:
				return string.Format("internal<{0}>", type.ToLuaDebuggerString());
			}
		}

		public static string ToLuaDebuggerString(this DataType type)
		{
			return type.ToString().ToLowerInvariant();
		}

		public static string ToLuaTypeString(this DataType type)
		{
			switch (type)
			{
			case DataType.Nil:
			case DataType.Void:
				return "nil";
			case DataType.Boolean:
				return "boolean";
			case DataType.Number:
				return "number";
			case DataType.String:
				return "string";
			case DataType.Function:
				return "function";
			case DataType.ClrFunction:
				return "function";
			case DataType.Table:
				return "table";
			case DataType.UserData:
				return "userdata";
			case DataType.Thread:
				return "thread";
			default:
				throw new ScriptRuntimeException("Unexpected LuaType {0}", type);
			}
		}
	}
}
