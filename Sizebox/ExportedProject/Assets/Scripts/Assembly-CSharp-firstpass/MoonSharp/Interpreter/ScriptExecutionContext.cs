using System;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Interop.LuaStateInterop;

namespace MoonSharp.Interpreter
{
	public class ScriptExecutionContext : IScriptPrivateResource
	{
		private Processor m_Processor;

		private CallbackFunction m_Callback;

		public bool IsDynamicExecution { get; private set; }

		public SourceRef CallingLocation { get; private set; }

		public object AdditionalData
		{
			get
			{
				if (m_Callback == null)
				{
					return null;
				}
				return m_Callback.AdditionalData;
			}
			set
			{
				if (m_Callback == null)
				{
					throw new InvalidOperationException("Cannot set additional data on a context which has no callback");
				}
				m_Callback.AdditionalData = value;
			}
		}

		public Table CurrentGlobalEnv
		{
			get
			{
				DynValue dynValue = EvaluateSymbolByName("_ENV");
				if (dynValue == null || dynValue.Type != DataType.Table)
				{
					return null;
				}
				return dynValue.Table;
			}
		}

		public Script OwnerScript
		{
			get
			{
				return GetScript();
			}
		}

		internal ScriptExecutionContext(Processor p, CallbackFunction callBackFunction, SourceRef sourceRef, bool isDynamic = false)
		{
			IsDynamicExecution = isDynamic;
			m_Processor = p;
			m_Callback = callBackFunction;
			CallingLocation = sourceRef;
		}

		public Table GetMetatable(DynValue value)
		{
			return m_Processor.GetMetatable(value);
		}

		public DynValue GetMetamethod(DynValue value, string metamethod)
		{
			return m_Processor.GetMetamethod(value, metamethod);
		}

		public DynValue GetMetamethodTailCall(DynValue value, string metamethod, params DynValue[] args)
		{
			DynValue metamethod2 = GetMetamethod(value, metamethod);
			if (metamethod2 == null)
			{
				return null;
			}
			return DynValue.NewTailCallReq(metamethod2, args);
		}

		public DynValue GetBinaryMetamethod(DynValue op1, DynValue op2, string eventName)
		{
			return m_Processor.GetBinaryMetamethod(op1, op2, eventName);
		}

		public Script GetScript()
		{
			return m_Processor.GetScript();
		}

		public Coroutine GetCallingCoroutine()
		{
			return m_Processor.AssociatedCoroutine;
		}

		public DynValue EmulateClassicCall(CallbackArguments args, string functionName, Func<LuaState, int> callback)
		{
			LuaState luaState = new LuaState(this, args, functionName);
			int retvals = callback(luaState);
			return luaState.GetReturnValue(retvals);
		}

		public DynValue Call(DynValue func, params DynValue[] args)
		{
			if (func.Type == DataType.Function)
			{
				return GetScript().Call(func, args);
			}
			if (func.Type == DataType.ClrFunction)
			{
				DynValue dynValue;
				while (true)
				{
					dynValue = func.Callback.Invoke(this, args);
					if (dynValue.Type == DataType.YieldRequest)
					{
						throw ScriptRuntimeException.CannotYield();
					}
					if (dynValue.Type != DataType.TailCallRequest)
					{
						break;
					}
					TailCallData tailCallData = dynValue.TailCallData;
					if (tailCallData.Continuation != null || tailCallData.ErrorHandler != null)
					{
						throw new ScriptRuntimeException("the function passed cannot be called directly. wrap in a script function instead.");
					}
					args = tailCallData.Args;
					func = tailCallData.Function;
				}
				return dynValue;
			}
			int num = 10;
			while (num > 0)
			{
				DynValue metamethod = GetMetamethod(func, "__call");
				if (metamethod == null && metamethod.IsNil())
				{
					throw ScriptRuntimeException.AttemptToCallNonFunc(func.Type);
				}
				func = metamethod;
				if (func.Type == DataType.Function || func.Type == DataType.ClrFunction)
				{
					return Call(func, args);
				}
			}
			throw ScriptRuntimeException.LoopInCall();
		}

		public DynValue EvaluateSymbol(SymbolRef symref)
		{
			if (symref == null)
			{
				return DynValue.Nil;
			}
			return m_Processor.GetGenericSymbol(symref);
		}

		public DynValue EvaluateSymbolByName(string symbol)
		{
			return EvaluateSymbol(FindSymbolByName(symbol));
		}

		public SymbolRef FindSymbolByName(string symbol)
		{
			return m_Processor.FindSymbolByName(symbol);
		}

		public void PerformMessageDecorationBeforeUnwind(DynValue messageHandler, ScriptRuntimeException exception)
		{
			if (messageHandler != null)
			{
				exception.DecoratedMessage = m_Processor.PerformMessageDecorationBeforeUnwind(messageHandler, exception.Message, CallingLocation);
			}
			else
			{
				exception.DecoratedMessage = exception.Message;
			}
		}
	}
}
