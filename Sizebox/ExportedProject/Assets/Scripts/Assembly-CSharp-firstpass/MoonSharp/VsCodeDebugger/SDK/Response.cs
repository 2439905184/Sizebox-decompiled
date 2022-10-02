using MoonSharp.Interpreter;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Response : ProtocolMessage
	{
		public bool success { get; private set; }

		public string message { get; private set; }

		public int request_seq { get; private set; }

		public string command { get; private set; }

		public ResponseBody body { get; private set; }

		public Response(Table req)
			: base("response")
		{
			success = true;
			request_seq = req.Get("seq").ToObject<int>();
			command = req.Get("command").ToObject<string>();
		}

		public void SetBody(ResponseBody bdy)
		{
			success = true;
			body = bdy;
		}

		public void SetErrorBody(string msg, ResponseBody bdy = null)
		{
			success = false;
			message = msg;
			body = bdy;
		}
	}
}
