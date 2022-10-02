using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class MoonSharpModuleMethodAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
