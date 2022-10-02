using MoonSharp.Interpreter.DataStructs;

namespace MoonSharp.Interpreter.Execution.VM
{
	internal sealed class ExecutionState
	{
		public FastStack<DynValue> ValueStack = new FastStack<DynValue>(131072);

		public FastStack<CallStackItem> ExecutionStack = new FastStack<CallStackItem>(131072);

		public int InstructionPtr;

		public CoroutineState State = CoroutineState.NotStarted;
	}
}
