using System;
using System.Text;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.REPL;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "debug")]
	public class DebugModule
	{
		[MoonSharpModuleMethod]
		public static DynValue debug(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Script script = executionContext.GetScript();
			if (script.Options.DebugInput == null)
			{
				throw new ScriptRuntimeException("debug.debug not supported on this platform/configuration");
			}
			ReplInterpreter replInterpreter = new ReplInterpreter(script)
			{
				HandleDynamicExprs = false,
				HandleClassicExprsSyntax = true
			};
			while (true)
			{
				string input = script.Options.DebugInput(replInterpreter.ClassicPrompt + " ");
				try
				{
					DynValue dynValue = replInterpreter.Evaluate(input);
					if (dynValue != null && dynValue.Type != DataType.Void)
					{
						script.Options.DebugPrint(string.Format("{0}", dynValue));
					}
				}
				catch (InterpreterException ex)
				{
					script.Options.DebugPrint(string.Format("{0}", ex.DecoratedMessage ?? ex.Message));
				}
				catch (Exception ex2)
				{
					script.Options.DebugPrint(string.Format("{0}", ex2.Message));
				}
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue getuservalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			if (dynValue.Type != DataType.UserData)
			{
				return DynValue.Nil;
			}
			return dynValue.UserData.UserValue ?? DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue setuservalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "setuservalue", DataType.UserData);
			DynValue dynValue2 = args.AsType(0, "setuservalue", DataType.Table, true);
			return dynValue.UserData.UserValue = dynValue2;
		}

		[MoonSharpModuleMethod]
		public static DynValue getregistry(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewTable(executionContext.GetScript().Registry);
		}

		[MoonSharpModuleMethod]
		public static DynValue getmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			Script script = executionContext.GetScript();
			if (dynValue.Type.CanHaveTypeMetatables())
			{
				return DynValue.NewTable(script.GetTypeMetatable(dynValue.Type));
			}
			if (dynValue.Type == DataType.Table)
			{
				return DynValue.NewTable(dynValue.Table.MetaTable);
			}
			return DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue setmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			DynValue dynValue2 = args.AsType(1, "setmetatable", DataType.Table, true);
			Table table = (dynValue2.IsNil() ? null : dynValue2.Table);
			Script script = executionContext.GetScript();
			if (dynValue.Type.CanHaveTypeMetatables())
			{
				script.SetTypeMetatable(dynValue.Type, table);
			}
			else
			{
				if (dynValue.Type != DataType.Table)
				{
					throw new ScriptRuntimeException("cannot debug.setmetatable on type {0}", dynValue.Type.ToErrorTypeString());
				}
				dynValue.Table.MetaTable = table;
			}
			return dynValue;
		}

		[MoonSharpModuleMethod]
		public static DynValue getupvalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			int num = (int)args.AsType(1, "getupvalue", DataType.Number).Number - 1;
			if (args[0].Type == DataType.ClrFunction)
			{
				return DynValue.Nil;
			}
			ClosureContext closureContext = args.AsType(0, "getupvalue", DataType.Function).Function.ClosureContext;
			if (num < 0 || num >= closureContext.Count)
			{
				return DynValue.Nil;
			}
			return DynValue.NewTuple(DynValue.NewString(closureContext.Symbols[num]), closureContext[num]);
		}

		[MoonSharpModuleMethod]
		public static DynValue upvalueid(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			int num = (int)args.AsType(1, "getupvalue", DataType.Number).Number - 1;
			if (args[0].Type == DataType.ClrFunction)
			{
				return DynValue.Nil;
			}
			ClosureContext closureContext = args.AsType(0, "getupvalue", DataType.Function).Function.ClosureContext;
			if (num < 0 || num >= closureContext.Count)
			{
				return DynValue.Nil;
			}
			return DynValue.NewNumber(closureContext[num].ReferenceID);
		}

		[MoonSharpModuleMethod]
		public static DynValue setupvalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			int num = (int)args.AsType(1, "setupvalue", DataType.Number).Number - 1;
			if (args[0].Type == DataType.ClrFunction)
			{
				return DynValue.Nil;
			}
			ClosureContext closureContext = args.AsType(0, "setupvalue", DataType.Function).Function.ClosureContext;
			if (num < 0 || num >= closureContext.Count)
			{
				return DynValue.Nil;
			}
			closureContext[num].Assign(args[2]);
			return DynValue.NewString(closureContext.Symbols[num]);
		}

		[MoonSharpModuleMethod]
		public static DynValue upvaluejoin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "upvaluejoin", DataType.Function);
			DynValue dynValue2 = args.AsType(2, "upvaluejoin", DataType.Function);
			int num = args.AsInt(1, "upvaluejoin") - 1;
			int num2 = args.AsInt(3, "upvaluejoin") - 1;
			Closure function = dynValue.Function;
			Closure function2 = dynValue2.Function;
			if (num < 0 || num >= function.ClosureContext.Count)
			{
				throw ScriptRuntimeException.BadArgument(1, "upvaluejoin", "invalid upvalue index");
			}
			if (num2 < 0 || num2 >= function2.ClosureContext.Count)
			{
				throw ScriptRuntimeException.BadArgument(3, "upvaluejoin", "invalid upvalue index");
			}
			function2.ClosureContext[num2] = function.ClosureContext[num];
			return DynValue.Void;
		}

		[MoonSharpModuleMethod]
		public static DynValue traceback(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DynValue dynValue = args[0];
			DynValue dynValue2 = args[1];
			double num = 1.0;
			Coroutine coroutine = executionContext.GetCallingCoroutine();
			if (dynValue.Type == DataType.Thread)
			{
				coroutine = dynValue.Coroutine;
				dynValue = args[1];
				dynValue2 = args[2];
				num = 0.0;
			}
			if (dynValue.IsNotNil() && dynValue.Type != DataType.String && dynValue.Type != DataType.Number)
			{
				return dynValue;
			}
			string text = dynValue.CastToString();
			int val = (int)(dynValue2.CastToNumber() ?? num);
			WatchItem[] stackTrace = coroutine.GetStackTrace(Math.Max(0, val));
			if (text != null)
			{
				stringBuilder.AppendLine(text);
			}
			stringBuilder.AppendLine("stack traceback:");
			WatchItem[] array = stackTrace;
			foreach (WatchItem watchItem in array)
			{
				string arg = ((watchItem.Name != null) ? ("function '" + watchItem.Name + "'") : ((watchItem.RetAddress >= 0) ? "?" : "main chunk"));
				string arg2 = ((watchItem.Location != null) ? watchItem.Location.FormatLocation(executionContext.GetScript()) : "[clr]");
				stringBuilder.AppendFormat("\t{0}: in {1}\n", arg2, arg);
			}
			return DynValue.NewString(stringBuilder);
		}
	}
}
