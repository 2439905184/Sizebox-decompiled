namespace MoonSharp.VsCodeDebugger.SDK
{
	public class ErrorResponseBody : ResponseBody
	{
		public Message error { get; private set; }

		public ErrorResponseBody(Message error)
		{
			this.error = error;
		}
	}
}
