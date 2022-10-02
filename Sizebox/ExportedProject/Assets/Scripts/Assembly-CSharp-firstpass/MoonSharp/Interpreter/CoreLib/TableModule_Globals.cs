namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class TableModule_Globals
	{
		[MoonSharpModuleMethod]
		public static DynValue unpack(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return TableModule.unpack(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue pack(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return TableModule.pack(executionContext, args);
		}
	}
}
