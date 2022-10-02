namespace MoonSharp.Interpreter
{
	public class TailCallData
	{
		public DynValue Function { get; set; }

		public DynValue[] Args { get; set; }

		public CallbackFunction Continuation { get; set; }

		public CallbackFunction ErrorHandler { get; set; }

		public DynValue ErrorHandlerBeforeUnwind { get; set; }
	}
}
