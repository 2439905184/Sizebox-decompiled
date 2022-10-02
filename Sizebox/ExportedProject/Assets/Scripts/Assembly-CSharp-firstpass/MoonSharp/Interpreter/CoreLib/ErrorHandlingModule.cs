using System.Collections.Generic;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class ErrorHandlingModule
	{
		[MoonSharpModuleMethod]
		public static DynValue pcall(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return SetErrorHandlerStrategy("pcall", executionContext, args, null);
		}

		private static DynValue SetErrorHandlerStrategy(string funcName, ScriptExecutionContext executionContext, CallbackArguments args, DynValue handlerBeforeUnwind)
		{
			DynValue function = args[0];
			DynValue[] array = new DynValue[args.Count - 1];
			for (int i = 1; i < args.Count; i++)
			{
				array[i - 1] = args[i];
			}
			if (args[0].Type == DataType.ClrFunction)
			{
				try
				{
					DynValue dynValue = args[0].Callback.Invoke(executionContext, array);
					if (dynValue.Type == DataType.TailCallRequest)
					{
						if (dynValue.TailCallData.Continuation != null || dynValue.TailCallData.ErrorHandler != null)
						{
							throw new ScriptRuntimeException("the function passed to {0} cannot be called directly by {0}. wrap in a script function instead.", funcName);
						}
						return DynValue.NewTailCallReq(new TailCallData
						{
							Args = dynValue.TailCallData.Args,
							Function = dynValue.TailCallData.Function,
							Continuation = new CallbackFunction(pcall_continuation, funcName),
							ErrorHandler = new CallbackFunction(pcall_onerror, funcName),
							ErrorHandlerBeforeUnwind = handlerBeforeUnwind
						});
					}
					if (dynValue.Type == DataType.YieldRequest)
					{
						throw new ScriptRuntimeException("the function passed to {0} cannot be called directly by {0}. wrap in a script function instead.", funcName);
					}
					return DynValue.NewTupleNested(DynValue.True, dynValue);
				}
				catch (ScriptRuntimeException ex)
				{
					executionContext.PerformMessageDecorationBeforeUnwind(handlerBeforeUnwind, ex);
					return DynValue.NewTupleNested(DynValue.False, DynValue.NewString(ex.DecoratedMessage));
				}
			}
			if (args[0].Type != DataType.Function)
			{
				return DynValue.NewTupleNested(DynValue.False, DynValue.NewString("attempt to " + funcName + " a non-function"));
			}
			return DynValue.NewTailCallReq(new TailCallData
			{
				Args = array,
				Function = function,
				Continuation = new CallbackFunction(pcall_continuation, funcName),
				ErrorHandler = new CallbackFunction(pcall_onerror, funcName),
				ErrorHandlerBeforeUnwind = handlerBeforeUnwind
			});
		}

		private static DynValue MakeReturnTuple(bool retstatus, CallbackArguments args)
		{
			DynValue[] array = new DynValue[args.Count + 1];
			for (int i = 0; i < args.Count; i++)
			{
				array[i + 1] = args[i];
			}
			array[0] = DynValue.NewBoolean(retstatus);
			return DynValue.NewTuple(array);
		}

		public static DynValue pcall_continuation(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return MakeReturnTuple(true, args);
		}

		public static DynValue pcall_onerror(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return MakeReturnTuple(false, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue xpcall(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			List<DynValue> list = new List<DynValue>();
			for (int i = 0; i < args.Count; i++)
			{
				if (i != 1)
				{
					list.Add(args[i]);
				}
			}
			DynValue handlerBeforeUnwind = null;
			if (args[1].Type == DataType.Function || args[1].Type == DataType.ClrFunction)
			{
				handlerBeforeUnwind = args[1];
			}
			else if (args[1].Type != 0)
			{
				args.AsType(1, "xpcall", DataType.Function);
			}
			return SetErrorHandlerStrategy("xpcall", executionContext, new CallbackArguments(list, false), handlerBeforeUnwind);
		}
	}
}
