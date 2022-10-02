using MoonSharp.Interpreter.DataStructs;

namespace MoonSharp.Interpreter.Execution
{
	internal class LoopTracker
	{
		public FastStack<ILoop> Loops = new FastStack<ILoop>(16384);
	}
}
