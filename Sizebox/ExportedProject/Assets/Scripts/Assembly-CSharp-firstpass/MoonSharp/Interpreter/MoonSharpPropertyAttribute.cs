using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpPropertyAttribute : Attribute
	{
		public string Name { get; private set; }

		public MoonSharpPropertyAttribute()
		{
		}

		public MoonSharpPropertyAttribute(string name)
		{
			Name = name;
		}
	}
}
