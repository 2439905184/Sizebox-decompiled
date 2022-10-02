using System;
using MoonSharp.Interpreter;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public abstract class DebugSession : ProtocolServer
	{
		private bool _debuggerLinesStartAt1;

		private bool _debuggerPathsAreURI;

		private bool _clientLinesStartAt1 = true;

		private bool _clientPathsAreURI = true;

		public DebugSession(bool debuggerLinesStartAt1, bool debuggerPathsAreURI = false)
		{
			_debuggerLinesStartAt1 = debuggerLinesStartAt1;
			_debuggerPathsAreURI = debuggerPathsAreURI;
		}

		public void SendResponse(Response response, ResponseBody body = null)
		{
			if (body != null)
			{
				response.SetBody(body);
			}
			SendMessage(response);
		}

		public void SendErrorResponse(Response response, int id, string format, object arguments = null, bool user = true, bool telemetry = false)
		{
			Message message = new Message(id, format, arguments, user, telemetry);
			string msg = Utilities.ExpandVariables(message.format, message.variables);
			response.SetErrorBody(msg, new ErrorResponseBody(message));
			SendMessage(response);
		}

		protected override void DispatchRequest(string command, Table args, Response response)
		{
			if (args == null)
			{
				args = new Table(null);
			}
			try
			{
				switch (command)
				{
				case "initialize":
				{
					if (args["linesStartAt1"] != null)
					{
						_clientLinesStartAt1 = args.Get("linesStartAt1").ToObject<bool>();
					}
					string text = args.Get("pathFormat").ToObject<string>();
					switch (text)
					{
					case "uri":
						_clientPathsAreURI = true;
						break;
					case "path":
						_clientPathsAreURI = false;
						break;
					default:
						SendErrorResponse(response, 1015, "initialize: bad value '{_format}' for pathFormat", new _003C_003Ef__AnonymousType4<string>(text));
						return;
					case null:
						break;
					}
					Initialize(response, args);
					break;
				}
				case "launch":
					Launch(response, args);
					break;
				case "attach":
					Attach(response, args);
					break;
				case "disconnect":
					Disconnect(response, args);
					break;
				case "next":
					Next(response, args);
					break;
				case "continue":
					Continue(response, args);
					break;
				case "stepIn":
					StepIn(response, args);
					break;
				case "stepOut":
					StepOut(response, args);
					break;
				case "pause":
					Pause(response, args);
					break;
				case "stackTrace":
					StackTrace(response, args);
					break;
				case "scopes":
					Scopes(response, args);
					break;
				case "variables":
					Variables(response, args);
					break;
				case "source":
					Source(response, args);
					break;
				case "threads":
					Threads(response, args);
					break;
				case "setBreakpoints":
					SetBreakpoints(response, args);
					break;
				case "setFunctionBreakpoints":
					SetFunctionBreakpoints(response, args);
					break;
				case "setExceptionBreakpoints":
					SetExceptionBreakpoints(response, args);
					break;
				case "evaluate":
					Evaluate(response, args);
					break;
				default:
					SendErrorResponse(response, 1014, "unrecognized request: {_request}", new _003C_003Ef__AnonymousType5<string>(command));
					break;
				}
			}
			catch (Exception ex)
			{
				SendErrorResponse(response, 1104, "error while processing request '{_request}' (exception: {_exception})", new _003C_003Ef__AnonymousType6<string, string>(command, ex.Message));
			}
			if (command == "disconnect")
			{
				Stop();
			}
		}

		public abstract void Initialize(Response response, Table args);

		public abstract void Launch(Response response, Table arguments);

		public abstract void Attach(Response response, Table arguments);

		public abstract void Disconnect(Response response, Table arguments);

		public virtual void SetFunctionBreakpoints(Response response, Table arguments)
		{
		}

		public virtual void SetExceptionBreakpoints(Response response, Table arguments)
		{
		}

		public abstract void SetBreakpoints(Response response, Table arguments);

		public abstract void Continue(Response response, Table arguments);

		public abstract void Next(Response response, Table arguments);

		public abstract void StepIn(Response response, Table arguments);

		public abstract void StepOut(Response response, Table arguments);

		public abstract void Pause(Response response, Table arguments);

		public abstract void StackTrace(Response response, Table arguments);

		public abstract void Scopes(Response response, Table arguments);

		public abstract void Variables(Response response, Table arguments);

		public virtual void Source(Response response, Table arguments)
		{
			SendErrorResponse(response, 1020, "Source not supported");
		}

		public abstract void Threads(Response response, Table arguments);

		public abstract void Evaluate(Response response, Table arguments);

		protected int ConvertDebuggerLineToClient(int line)
		{
			if (_debuggerLinesStartAt1)
			{
				if (!_clientLinesStartAt1)
				{
					return line - 1;
				}
				return line;
			}
			if (!_clientLinesStartAt1)
			{
				return line;
			}
			return line + 1;
		}

		protected int ConvertClientLineToDebugger(int line)
		{
			if (_debuggerLinesStartAt1)
			{
				if (!_clientLinesStartAt1)
				{
					return line + 1;
				}
				return line;
			}
			if (!_clientLinesStartAt1)
			{
				return line;
			}
			return line - 1;
		}

		protected string ConvertDebuggerPathToClient(string path)
		{
			if (_debuggerPathsAreURI)
			{
				if (_clientPathsAreURI)
				{
					return path;
				}
				return new Uri(path).LocalPath;
			}
			if (_clientPathsAreURI)
			{
				try
				{
					return new Uri(path).AbsoluteUri;
				}
				catch
				{
					return null;
				}
			}
			return path;
		}

		protected string ConvertClientPathToDebugger(string clientPath)
		{
			if (clientPath == null)
			{
				return null;
			}
			if (_debuggerPathsAreURI)
			{
				if (_clientPathsAreURI)
				{
					return clientPath;
				}
				return new Uri(clientPath).AbsoluteUri;
			}
			if (_clientPathsAreURI)
			{
				if (Uri.IsWellFormedUriString(clientPath, UriKind.Absolute))
				{
					return new Uri(clientPath).LocalPath;
				}
				Console.Error.WriteLine("path not well formed: '{0}'", clientPath);
				return null;
			}
			return clientPath;
		}
	}
}
