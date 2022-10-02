using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter
{
	public class Coroutine : RefIdObject, IScriptPrivateResource
	{
		public enum CoroutineType
		{
			Coroutine = 0,
			ClrCallback = 1,
			ClrCallbackDead = 2
		}

		private CallbackFunction m_ClrCallback;

		private Processor m_Processor;

		public CoroutineType Type { get; private set; }

		public CoroutineState State
		{
			get
			{
				if (Type == CoroutineType.ClrCallback)
				{
					return CoroutineState.NotStarted;
				}
				if (Type == CoroutineType.ClrCallbackDead)
				{
					return CoroutineState.Dead;
				}
				return m_Processor.State;
			}
		}

		public Script OwnerScript { get; private set; }

		public long AutoYieldCounter
		{
			get
			{
				return m_Processor.AutoYieldCounter;
			}
			set
			{
				m_Processor.AutoYieldCounter = value;
			}
		}

		internal Coroutine(CallbackFunction function)
		{
			Type = CoroutineType.ClrCallback;
			m_ClrCallback = function;
			OwnerScript = null;
		}

		internal Coroutine(Processor proc)
		{
			Type = CoroutineType.Coroutine;
			m_Processor = proc;
			m_Processor.AssociatedCoroutine = this;
			OwnerScript = proc.GetScript();
		}

		internal void MarkClrCallbackAsDead()
		{
			if (Type != CoroutineType.ClrCallback)
			{
				throw new InvalidOperationException("State must be CoroutineType.ClrCallback");
			}
			Type = CoroutineType.ClrCallbackDead;
		}

		public IEnumerable<DynValue> AsTypedEnumerable()
		{
			if (Type != 0)
			{
				throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");
			}
			while (State == CoroutineState.NotStarted || State == CoroutineState.Suspended || State == CoroutineState.ForceSuspended)
			{
				yield return Resume();
			}
		}

		public IEnumerable<object> AsEnumerable()
		{
			foreach (DynValue item in AsTypedEnumerable())
			{
				yield return item.ToScalar().ToObject();
			}
		}

		public IEnumerable<T> AsEnumerable<T>()
		{
			foreach (DynValue item in AsTypedEnumerable())
			{
				yield return item.ToScalar().ToObject<T>();
			}
		}

		public IEnumerator AsUnityCoroutine()
		{
			foreach (DynValue item in AsTypedEnumerable())
			{
				DynValue dynValue = item;
				yield return null;
			}
		}

		public DynValue Resume(params DynValue[] args)
		{
			this.CheckScriptOwnership(args);
			if (Type == CoroutineType.Coroutine)
			{
				return m_Processor.Coroutine_Resume(args);
			}
			throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");
		}

		public DynValue Resume(ScriptExecutionContext context, params DynValue[] args)
		{
			this.CheckScriptOwnership(context);
			this.CheckScriptOwnership(args);
			if (Type == CoroutineType.Coroutine)
			{
				return m_Processor.Coroutine_Resume(args);
			}
			if (Type == CoroutineType.ClrCallback)
			{
				DynValue result = m_ClrCallback.Invoke(context, args);
				MarkClrCallbackAsDead();
				return result;
			}
			throw ScriptRuntimeException.CannotResumeNotSuspended(CoroutineState.Dead);
		}

		public DynValue Resume()
		{
			return Resume(new DynValue[0]);
		}

		public DynValue Resume(ScriptExecutionContext context)
		{
			return Resume(context, new DynValue[0]);
		}

		public DynValue Resume(params object[] args)
		{
			if (Type != 0)
			{
				throw new InvalidOperationException("Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead");
			}
			DynValue[] array = new DynValue[args.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DynValue.FromObject(OwnerScript, args[i]);
			}
			return Resume(array);
		}

		public DynValue Resume(ScriptExecutionContext context, params object[] args)
		{
			DynValue[] array = new DynValue[args.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DynValue.FromObject(context.GetScript(), args[i]);
			}
			return Resume(context, array);
		}

		public WatchItem[] GetStackTrace(int skip, SourceRef entrySourceRef = null)
		{
			if (State != CoroutineState.Running)
			{
				entrySourceRef = m_Processor.GetCoroutineSuspendedLocation();
			}
			return m_Processor.Debugger_GetCallStack(entrySourceRef).Skip(skip).ToArray();
		}
	}
}
