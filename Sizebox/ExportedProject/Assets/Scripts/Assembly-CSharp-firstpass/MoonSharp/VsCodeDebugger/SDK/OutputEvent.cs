namespace MoonSharp.VsCodeDebugger.SDK
{
	public class OutputEvent : Event
	{
		public OutputEvent(string cat, string outpt)
			: base("output", new _003C_003Ef__AnonymousType3<string, string>(cat, outpt))
		{
		}
	}
}
