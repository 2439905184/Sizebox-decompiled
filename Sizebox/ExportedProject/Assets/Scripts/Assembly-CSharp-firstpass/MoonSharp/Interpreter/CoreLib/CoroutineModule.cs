using System.Collections.Generic;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "coroutine")]
	public class CoroutineModule
	{
		[MoonSharpModuleMethod]
		public static DynValue create(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.Function && args[0].Type != DataType.ClrFunction)
			{
				args.AsType(0, "create", DataType.Function);
			}
			return executionContext.GetScript().CreateCoroutine(args[0]);
		}

		[MoonSharpModuleMethod]
		public static DynValue wrap(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.Function && args[0].Type != DataType.ClrFunction)
			{
				args.AsType(0, "wrap", DataType.Function);
			}
			DynValue additionalData = create(executionContext, args);
			DynValue dynValue = DynValue.NewCallback(__wrap_wrapper);
			dynValue.Callback.AdditionalData = additionalData;
			return dynValue;
		}

		public static DynValue __wrap_wrapper(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return ((DynValue)executionContext.AdditionalData).Coroutine.Resume(args.GetArray());
		}

		[MoonSharpModuleMethod]
		public static DynValue resume(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "resume", DataType.Thread);
			try
			{
				DynValue dynValue2 = dynValue.Coroutine.Resume(args.GetArray(1));
				List<DynValue> list = new List<DynValue>();
				list.Add(DynValue.True);
				if (dynValue2.Type == DataType.Tuple)
				{
					for (int i = 0; i < dynValue2.Tuple.Length; i++)
					{
						DynValue dynValue3 = dynValue2.Tuple[i];
						if (i == dynValue2.Tuple.Length - 1 && dynValue3.Type == DataType.Tuple)
						{
							list.AddRange(dynValue3.Tuple);
						}
						else
						{
							list.Add(dynValue3);
						}
					}
				}
				else
				{
					list.Add(dynValue2);
				}
				return DynValue.NewTuple(list.ToArray());
			}
			catch (ScriptRuntimeException ex)
			{
				return DynValue.NewTuple(DynValue.False, DynValue.NewString(ex.Message));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue yield(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewYieldReq(args.GetArray());
		}

		[MoonSharpModuleMethod]
		public static DynValue running(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Coroutine callingCoroutine = executionContext.GetCallingCoroutine();
			return DynValue.NewTuple(DynValue.NewCoroutine(callingCoroutine), DynValue.NewBoolean(callingCoroutine.State == CoroutineState.Main));
		}

		[MoonSharpModuleMethod]
		public static DynValue status(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "status", DataType.Thread);
			Coroutine callingCoroutine = executionContext.GetCallingCoroutine();
			CoroutineState state = dynValue.Coroutine.State;
			switch (state)
			{
			case CoroutineState.Main:
			case CoroutineState.Running:
				if (dynValue.Coroutine != callingCoroutine)
				{
					return DynValue.NewString("normal");
				}
				return DynValue.NewString("running");
			case CoroutineState.NotStarted:
			case CoroutineState.Suspended:
			case CoroutineState.ForceSuspended:
				return DynValue.NewString("suspended");
			case CoroutineState.Dead:
				return DynValue.NewString("dead");
			default:
				throw new InternalErrorException("Unexpected coroutine state {0}", state);
			}
		}
	}
}
