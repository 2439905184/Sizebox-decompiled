using System;

namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	public interface IOverloadableMemberDescriptor : IMemberDescriptor
	{
		Type ExtensionMethodType { get; }

		ParameterDescriptor[] Parameters { get; }

		Type VarArgsArrayType { get; }

		Type VarArgsElementType { get; }

		string SortDiscriminant { get; }

		DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args);
	}
}
