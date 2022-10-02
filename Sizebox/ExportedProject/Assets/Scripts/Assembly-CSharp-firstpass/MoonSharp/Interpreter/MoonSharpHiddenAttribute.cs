using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
	public sealed class MoonSharpHiddenAttribute : Attribute
	{
	}
}
