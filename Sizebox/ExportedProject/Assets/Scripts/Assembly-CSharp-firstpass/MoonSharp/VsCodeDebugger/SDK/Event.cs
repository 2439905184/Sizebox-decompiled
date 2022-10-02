namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Event : ProtocolMessage
	{
		public string @event { get; private set; }

		public object body { get; private set; }

		public Event(string type, object bdy = null)
			: base("event")
		{
			@event = type;
			body = bdy;
		}
	}
}
