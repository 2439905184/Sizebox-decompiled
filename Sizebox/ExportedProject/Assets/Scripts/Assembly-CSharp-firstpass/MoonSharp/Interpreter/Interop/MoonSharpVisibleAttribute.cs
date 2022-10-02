using System;

namespace MoonSharp.Interpreter.Interop
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
	public sealed class MoonSharpVisibleAttribute : Attribute
	{
		public bool Visible { get; private set; }

		public MoonSharpVisibleAttribute(bool visible)
		{
			Visible = visible;
		}
	}
}
