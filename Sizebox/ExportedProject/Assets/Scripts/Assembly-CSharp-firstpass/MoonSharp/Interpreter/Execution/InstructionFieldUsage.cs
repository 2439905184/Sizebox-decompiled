using System;

namespace MoonSharp.Interpreter.Execution
{
	[Flags]
	internal enum InstructionFieldUsage
	{
		None = 0,
		Symbol = 1,
		SymbolList = 2,
		Name = 4,
		Value = 8,
		NumVal = 0x10,
		NumVal2 = 0x20,
		NumValAsCodeAddress = 0x8010
	}
}
