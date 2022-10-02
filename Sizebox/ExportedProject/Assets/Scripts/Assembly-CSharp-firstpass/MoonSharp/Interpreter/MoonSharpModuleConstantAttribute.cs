using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class MoonSharpModuleConstantAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
