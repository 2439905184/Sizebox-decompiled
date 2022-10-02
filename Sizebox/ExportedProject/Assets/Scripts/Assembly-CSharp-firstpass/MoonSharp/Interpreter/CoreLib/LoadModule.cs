using System.IO;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class LoadModule
	{
		[MoonSharpModuleMethod]
		public const string require = "\nfunction(modulename)\n\tif (package == nil) then package = { }; end\n\tif (package.loaded == nil) then package.loaded = { }; end\n\n\tlocal m = package.loaded[modulename];\n\n\tif (m ~= nil) then\n\t\treturn m;\n\tend\n\n\tlocal func = __require_clr_impl(modulename);\n\n\tlocal res = func(modulename);\n\n\tif (res == nil) then\n\t\tres = true;\n\tend\n\n\tpackage.loaded[modulename] = res;\n\n\treturn res;\nend";

		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			DynValue dynValue = globalTable.Get("package");
			if (dynValue.IsNil())
			{
				dynValue = (DynValue)(globalTable["package"] = DynValue.NewTable(globalTable.OwnerScript));
			}
			else if (dynValue.Type != DataType.Table)
			{
				throw new InternalErrorException("'package' global variable was found and it is not a table");
			}
			string str = Path.DirectorySeparatorChar + "\n;\n?\n!\n-\n";
			dynValue.Table.Set("config", DynValue.NewString(str));
		}

		[MoonSharpModuleMethod]
		public static DynValue load(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return load_impl(executionContext, args, null);
		}

		[MoonSharpModuleMethod]
		public static DynValue loadsafe(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return load_impl(executionContext, args, GetSafeDefaultEnv(executionContext));
		}

		public static DynValue load_impl(ScriptExecutionContext executionContext, CallbackArguments args, Table defaultEnv)
		{
			try
			{
				Script script = executionContext.GetScript();
				DynValue dynValue = args[0];
				string text = "";
				if (dynValue.Type == DataType.Function)
				{
					DynValue dynValue2;
					while (true)
					{
						dynValue2 = executionContext.GetScript().Call(dynValue);
						if (dynValue2.Type != DataType.String || dynValue2.String.Length <= 0)
						{
							break;
						}
						text += dynValue2.String;
					}
					if (!dynValue2.IsNil())
					{
						return DynValue.NewTuple(DynValue.Nil, DynValue.NewString("reader function must return a string"));
					}
				}
				else if (dynValue.Type == DataType.String)
				{
					text = dynValue.String;
				}
				else
				{
					args.AsType(0, "load", DataType.Function);
				}
				DynValue dynValue3 = args.AsType(1, "load", DataType.String, true);
				DynValue dynValue4 = args.AsType(3, "load", DataType.Table, true);
				return script.LoadString(text, (!dynValue4.IsNil()) ? dynValue4.Table : defaultEnv, (!dynValue3.IsNil()) ? dynValue3.String : "=(load)");
			}
			catch (SyntaxErrorException ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.DecoratedMessage ?? ex.Message));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue loadfile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return loadfile_impl(executionContext, args, null);
		}

		[MoonSharpModuleMethod]
		public static DynValue loadfilesafe(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return loadfile_impl(executionContext, args, GetSafeDefaultEnv(executionContext));
		}

		private static DynValue loadfile_impl(ScriptExecutionContext executionContext, CallbackArguments args, Table defaultEnv)
		{
			try
			{
				Script script = executionContext.GetScript();
				DynValue dynValue = args.AsType(0, "loadfile", DataType.String);
				DynValue dynValue2 = args.AsType(2, "loadfile", DataType.Table, true);
				return script.LoadFile(dynValue.String, dynValue2.IsNil() ? defaultEnv : dynValue2.Table);
			}
			catch (SyntaxErrorException ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.DecoratedMessage ?? ex.Message));
			}
		}

		private static Table GetSafeDefaultEnv(ScriptExecutionContext executionContext)
		{
			Table currentGlobalEnv = executionContext.CurrentGlobalEnv;
			if (currentGlobalEnv == null)
			{
				throw new ScriptRuntimeException("current environment cannot be backtracked.");
			}
			return currentGlobalEnv;
		}

		[MoonSharpModuleMethod]
		public static DynValue dofile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				Script script = executionContext.GetScript();
				DynValue dynValue = args.AsType(0, "dofile", DataType.String);
				return DynValue.NewTailCallReq(script.LoadFile(dynValue.String));
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue __require_clr_impl(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Script script = executionContext.GetScript();
			DynValue dynValue = args.AsType(0, "__require_clr_impl", DataType.String);
			return script.RequireModule(dynValue.String);
		}
	}
}
