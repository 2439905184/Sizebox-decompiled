using System.Collections.Generic;
using System.Diagnostics;
using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
	internal class EmptyDebugSession : DebugSession
	{
		private MoonSharpVsCodeDebugServer m_Server;

		internal EmptyDebugSession(MoonSharpVsCodeDebugServer server)
			: base(true)
		{
			m_Server = server;
		}

		public override void Initialize(Response response, Table args)
		{
			SendText("Connected to MoonSharp {0} [{1}] on process {2} (PID {3})", "2.0.0.0", Script.GlobalOptions.Platform.GetPlatformName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
			SendText("No script is set as default for debugging; use the debug console to select the script to debug.\n");
			SendList();
			SendResponse(response, new Capabilities
			{
				supportsConfigurationDoneRequest = false,
				supportsFunctionBreakpoints = false,
				supportsConditionalBreakpoints = false,
				supportsEvaluateForHovers = false,
				exceptionBreakpointFilters = new object[0]
			});
			SendEvent(new InitializedEvent());
		}

		private void SendList()
		{
			int num = m_Server.CurrentId ?? (-1000);
			SendText("==========================================================");
			foreach (KeyValuePair<int, string> item in m_Server.GetAttachedDebuggersByIdAndName())
			{
				string text = ((item.Key == num) ? " (default)" : "");
				SendText("{0} : {1}{2}", item.Key.ToString().PadLeft(9), item.Value, text);
			}
			SendText("");
			SendText("Type the number of the script to debug, or '!' to refresh");
		}

		public override void Attach(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Continue(Response response, Table arguments)
		{
			SendList();
			SendResponse(response);
		}

		public override void Disconnect(Response response, Table arguments)
		{
			SendResponse(response);
		}

		private static string getString(Table args, string property, string dflt = null)
		{
			string text = (string)args[property];
			if (text == null)
			{
				return dflt;
			}
			text = text.Trim();
			if (text.Length == 0)
			{
				return dflt;
			}
			return text;
		}

		public override void Evaluate(Response response, Table args)
		{
			string @string = getString(args, "expression");
			if ((getString(args, "context") ?? "hover") == "repl")
			{
				ExecuteRepl(@string);
			}
			SendResponse(response);
		}

		private void ExecuteRepl(string cmd)
		{
			int result = 0;
			if (int.TryParse(cmd, out result))
			{
				m_Server.CurrentId = result;
				SendText("Re-attach the debugger to debug the selected script.");
				Unbind();
			}
			else
			{
				SendList();
			}
		}

		public override void Launch(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Next(Response response, Table arguments)
		{
			SendList();
			SendResponse(response);
		}

		public override void Pause(Response response, Table arguments)
		{
			SendList();
			SendResponse(response);
		}

		public override void Scopes(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void SetBreakpoints(Response response, Table args)
		{
			SendResponse(response);
		}

		public override void StackTrace(Response response, Table args)
		{
			SendResponse(response);
		}

		public override void StepIn(Response response, Table arguments)
		{
			SendList();
			SendResponse(response);
		}

		public override void StepOut(Response response, Table arguments)
		{
			SendList();
			SendResponse(response);
		}

		public override void Threads(Response response, Table arguments)
		{
			List<Thread> vars = new List<Thread>
			{
				new Thread(0, "Main Thread")
			};
			SendResponse(response, new ThreadsResponseBody(vars));
		}

		public override void Variables(Response response, Table arguments)
		{
			SendResponse(response);
		}

		private void SendText(string msg, params object[] args)
		{
			msg = string.Format(msg, args);
			SendEvent(new OutputEvent("console", msg + "\n"));
		}

		public void Unbind()
		{
			SendText("Bye.");
			SendEvent(new TerminatedEvent());
		}
	}
}
