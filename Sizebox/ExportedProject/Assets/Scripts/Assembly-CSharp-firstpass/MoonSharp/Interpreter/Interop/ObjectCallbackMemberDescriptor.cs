using System;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class ObjectCallbackMemberDescriptor : FunctionMemberDescriptorBase
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<object, ScriptExecutionContext, CallbackArguments, object> _003C_003E9__1_0;

			internal object _003C_002Ector_003Eb__1_0(object o, ScriptExecutionContext c, CallbackArguments a)
			{
				return DynValue.Void;
			}
		}

		private Func<object, ScriptExecutionContext, CallbackArguments, object> m_CallbackFunc;

		public ObjectCallbackMemberDescriptor(string funcName)
			: this(funcName, _003C_003Ec._003C_003E9__1_0 ?? (_003C_003Ec._003C_003E9__1_0 = _003C_003Ec._003C_003E9._003C_002Ector_003Eb__1_0), new ParameterDescriptor[0])
		{
		}

		public ObjectCallbackMemberDescriptor(string funcName, Func<object, ScriptExecutionContext, CallbackArguments, object> callBack)
			: this(funcName, callBack, new ParameterDescriptor[0])
		{
		}

		public ObjectCallbackMemberDescriptor(string funcName, Func<object, ScriptExecutionContext, CallbackArguments, object> callBack, ParameterDescriptor[] parameters)
		{
			m_CallbackFunc = callBack;
			Initialize(funcName, false, parameters, false);
		}

		public override DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			if (m_CallbackFunc != null)
			{
				object obj2 = m_CallbackFunc(obj, context, args);
				return ClrToScriptConversions.ObjectToDynValue(script, obj2);
			}
			return DynValue.Void;
		}
	}
}
