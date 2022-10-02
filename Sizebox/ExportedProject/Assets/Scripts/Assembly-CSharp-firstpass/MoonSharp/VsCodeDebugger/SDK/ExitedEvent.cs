namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ExitedEvent : Event
	{
		public ExitedEvent(int exCode)
			: base("exited", new _003C_003Ef__AnonymousType1<int>(exCode))
		{
		}
	}
}
