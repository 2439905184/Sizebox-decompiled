using System;
using System.Collections.Generic;
using System.Reflection;
using MoonSharp.Interpreter.Interop;

namespace MoonSharp.Interpreter
{
	public sealed class CallbackFunction : RefIdObject
	{
		private static InteropAccessMode m_DefaultAccessMode = InteropAccessMode.LazyOptimized;

		public string Name { get; private set; }

		public Func<ScriptExecutionContext, CallbackArguments, DynValue> ClrCallback { get; private set; }

		public static InteropAccessMode DefaultAccessMode
		{
			get
			{
				return m_DefaultAccessMode;
			}
			set
			{
				if (value == InteropAccessMode.Default || value == InteropAccessMode.HideMembers || value == InteropAccessMode.BackgroundOptimized)
				{
					throw new ArgumentException("DefaultAccessMode");
				}
				m_DefaultAccessMode = value;
			}
		}

		public object AdditionalData { get; set; }

		public CallbackFunction(Func<ScriptExecutionContext, CallbackArguments, DynValue> callBack, string name = null)
		{
			ClrCallback = callBack;
			Name = name;
		}

		public DynValue Invoke(ScriptExecutionContext executionContext, IList<DynValue> args, bool isMethodCall = false)
		{
			if (isMethodCall)
			{
				switch (executionContext.GetScript().Options.ColonOperatorClrCallbackBehaviour)
				{
				case ColonOperatorBehaviour.TreatAsColon:
					isMethodCall = false;
					break;
				case ColonOperatorBehaviour.TreatAsDotOnUserData:
					isMethodCall = args.Count > 0 && args[0].Type == DataType.UserData;
					break;
				}
			}
			return ClrCallback(executionContext, new CallbackArguments(args, isMethodCall));
		}

		public static CallbackFunction FromDelegate(Script script, Delegate del, InteropAccessMode accessMode = InteropAccessMode.Default)
		{
			if (accessMode == InteropAccessMode.Default)
			{
				accessMode = m_DefaultAccessMode;
			}
			return new MethodMemberDescriptor(del.Method, accessMode).GetCallbackFunction(script, del.Target);
		}

		public static CallbackFunction FromMethodInfo(Script script, MethodInfo mi, object obj = null, InteropAccessMode accessMode = InteropAccessMode.Default)
		{
			if (accessMode == InteropAccessMode.Default)
			{
				accessMode = m_DefaultAccessMode;
			}
			return new MethodMemberDescriptor(mi, accessMode).GetCallbackFunction(script, obj);
		}

		public static bool CheckCallbackSignature(MethodInfo mi, bool requirePublicVisibility)
		{
			ParameterInfo[] parameters = mi.GetParameters();
			if (parameters.Length == 2 && parameters[0].ParameterType == typeof(ScriptExecutionContext) && parameters[1].ParameterType == typeof(CallbackArguments) && mi.ReturnType == typeof(DynValue))
			{
				if (!requirePublicVisibility)
				{
					return mi.IsPublic;
				}
				return true;
			}
			return false;
		}
	}
}
