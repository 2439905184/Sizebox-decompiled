namespace MoonSharp.VsCodeDebugger.SDK
{
	public class StoppedEvent : Event
	{
		public StoppedEvent(int tid, string reasn, string txt = null)
			: base("stopped", new _003C_003Ef__AnonymousType0<int, string, string>(tid, reasn, txt))
		{
		}
	}
}
