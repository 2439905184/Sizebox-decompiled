using System;

namespace MoonSharp.Interpreter
{
	[Flags]
	public enum TypeValidationFlags
	{
		None = 0,
		AllowNil = 1,
		AutoConvert = 2,
		Default = 2
	}
}
