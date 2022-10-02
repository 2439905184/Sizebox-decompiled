using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree
{
	internal class Loop : ILoop
	{
		public RuntimeScopeBlock Scope;

		public List<Instruction> BreakJumps = new List<Instruction>();

		public void CompileBreak(ByteCode bc)
		{
			bc.Emit_Exit(Scope);
			BreakJumps.Add(bc.Emit_Jump(OpCode.Jump, -1));
		}

		public bool IsBoundary()
		{
			return false;
		}
	}
}
