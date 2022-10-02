using System;

namespace MoonSharp.Interpreter
{
	[Flags]
	public enum CoreModules
	{
		None = 0,
		Basic = 0x40,
		GlobalConsts = 1,
		TableIterators = 2,
		Metatables = 4,
		String = 8,
		LoadMethods = 0x10,
		Table = 0x20,
		ErrorHandling = 0x80,
		Math = 0x100,
		Coroutine = 0x200,
		Bit32 = 0x400,
		OS_Time = 0x800,
		OS_System = 0x1000,
		IO = 0x2000,
		Debug = 0x4000,
		Dynamic = 0x8000,
		Json = 0x10000,
		Preset_HardSandbox = 0x56B,
		Preset_SoftSandbox = 0x18FEF,
		Preset_Default = 0x1BFFF,
		Preset_Complete = 0x1FFFF
	}
}
