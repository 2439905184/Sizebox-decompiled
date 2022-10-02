using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Execution
{
	internal interface ILoop
	{
		void CompileBreak(ByteCode bc);

		bool IsBoundary();
	}
}
