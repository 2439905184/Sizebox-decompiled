using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree
{
	internal class LoopBoundary : ILoop
	{
		public void CompileBreak(ByteCode bc)
		{
			throw new InternalErrorException("CompileBreak called on LoopBoundary");
		}

		public bool IsBoundary()
		{
			return true;
		}
	}
}
