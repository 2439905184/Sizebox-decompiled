namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ThreadEvent : Event
	{
		public ThreadEvent(string reasn, int tid)
			: base("thread", new _003C_003Ef__AnonymousType2<string, int>(reasn, tid))
		{
		}
	}
}
