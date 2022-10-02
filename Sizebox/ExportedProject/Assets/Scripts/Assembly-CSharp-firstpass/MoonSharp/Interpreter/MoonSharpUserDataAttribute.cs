using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class MoonSharpUserDataAttribute : Attribute
	{
		public InteropAccessMode AccessMode { get; set; }

		public MoonSharpUserDataAttribute()
		{
			AccessMode = InteropAccessMode.Default;
		}
	}
}
