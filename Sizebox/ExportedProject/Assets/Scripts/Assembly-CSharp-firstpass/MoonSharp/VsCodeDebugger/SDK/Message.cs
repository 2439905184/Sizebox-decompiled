namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Message
	{
		public int id { get; private set; }

		public string format { get; private set; }

		public object variables { get; private set; }

		public object showUser { get; private set; }

		public object sendTelemetry { get; private set; }

		public Message(int id, string format, object variables = null, bool user = true, bool telemetry = false)
		{
			this.id = id;
			this.format = format;
			this.variables = variables;
			showUser = user;
			sendTelemetry = telemetry;
		}
	}
}
