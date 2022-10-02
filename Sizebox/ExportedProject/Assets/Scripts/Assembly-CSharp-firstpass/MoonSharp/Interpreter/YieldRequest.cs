namespace MoonSharp.Interpreter
{
	public class YieldRequest
	{
		public DynValue[] ReturnValues;

		public bool Forced { get; internal set; }
	}
}
