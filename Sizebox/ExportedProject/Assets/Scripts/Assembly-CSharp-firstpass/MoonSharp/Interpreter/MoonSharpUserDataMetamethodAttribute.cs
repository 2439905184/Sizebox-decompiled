using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpUserDataMetamethodAttribute : Attribute
	{
		public string Name { get; private set; }

		public MoonSharpUserDataMetamethodAttribute(string name)
		{
			Name = name;
		}
	}
}
