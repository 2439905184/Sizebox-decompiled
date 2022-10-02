using System;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "os")]
	public class OsSystemModule
	{
		[MoonSharpModuleMethod]
		public static DynValue execute(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "execute", DataType.String, true);
			if (dynValue.IsNil())
			{
				return DynValue.NewBoolean(true);
			}
			try
			{
				int num = Script.GlobalOptions.Platform.OS_Execute(dynValue.String);
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString("exit"), DynValue.NewNumber(num));
			}
			catch (Exception)
			{
				return DynValue.Nil;
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue exit(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "exit", DataType.Number, true);
			int exitCode = 0;
			if (dynValue.IsNotNil())
			{
				exitCode = (int)dynValue.Number;
			}
			Script.GlobalOptions.Platform.OS_ExitFast(exitCode);
			throw new InvalidOperationException("Unreachable code.. reached.");
		}

		[MoonSharpModuleMethod]
		public static DynValue getenv(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "getenv", DataType.String);
			string environmentVariable = Script.GlobalOptions.Platform.GetEnvironmentVariable(dynValue.String);
			if (environmentVariable == null)
			{
				return DynValue.Nil;
			}
			return DynValue.NewString(environmentVariable);
		}

		[MoonSharpModuleMethod]
		public static DynValue remove(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string @string = args.AsType(0, "remove", DataType.String).String;
			try
			{
				if (Script.GlobalOptions.Platform.OS_FileExists(@string))
				{
					Script.GlobalOptions.Platform.OS_FileDelete(@string);
					return DynValue.True;
				}
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString("{0}: No such file or directory.", @string), DynValue.NewNumber(-1.0));
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message), DynValue.NewNumber(-1.0));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue rename(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string @string = args.AsType(0, "rename", DataType.String).String;
			string string2 = args.AsType(1, "rename", DataType.String).String;
			try
			{
				if (!Script.GlobalOptions.Platform.OS_FileExists(@string))
				{
					return DynValue.NewTuple(DynValue.Nil, DynValue.NewString("{0}: No such file or directory.", @string), DynValue.NewNumber(-1.0));
				}
				Script.GlobalOptions.Platform.OS_FileMove(@string, string2);
				return DynValue.True;
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.Message), DynValue.NewNumber(-1.0));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue setlocale(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewString("n/a");
		}

		[MoonSharpModuleMethod]
		public static DynValue tmpname(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewString(Script.GlobalOptions.Platform.IO_OS_GetTempFilename());
		}
	}
}
