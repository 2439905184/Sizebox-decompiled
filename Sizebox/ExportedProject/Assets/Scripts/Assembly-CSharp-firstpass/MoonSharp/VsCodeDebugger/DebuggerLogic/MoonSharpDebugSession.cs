using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
	internal class MoonSharpDebugSession : DebugSession, IAsyncDebuggerClient
	{
		private AsyncDebugger m_Debug;

		private MoonSharpVsCodeDebugServer m_Server;

		private List<DynValue> m_Variables = new List<DynValue>();

		private bool m_NotifyExecutionEnd;

		private const int SCOPE_LOCALS = 65536;

		private const int SCOPE_SELF = 65537;

		private readonly SourceRef DefaultSourceRef = new SourceRef(-1, 0, 0, 0, 0, false);

		internal MoonSharpDebugSession(MoonSharpVsCodeDebugServer server, AsyncDebugger debugger)
			: base(true)
		{
			m_Server = server;
			m_Debug = debugger;
		}

		public override void Initialize(Response response, Table args)
		{
			SendText("Connected to MoonSharp {0} [{1}] on process {2} (PID {3})", "2.0.0.0", Script.GlobalOptions.Platform.GetPlatformName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
			SendText("Debugging script '{0}'; use the debug console to debug another script.", m_Debug.Name);
			SendText("Type '!help' in the Debug Console for available commands.");
			SendResponse(response, new Capabilities
			{
				supportsConfigurationDoneRequest = false,
				supportsFunctionBreakpoints = false,
				supportsConditionalBreakpoints = false,
				supportsEvaluateForHovers = false,
				exceptionBreakpointFilters = new object[0]
			});
			SendEvent(new InitializedEvent());
			m_Debug.Client = this;
		}

		public override void Attach(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Continue(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction
			{
				Action = DebuggerAction.ActionType.Run
			});
			SendResponse(response);
		}

		public override void Disconnect(Response response, Table arguments)
		{
			m_Debug.Client = null;
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
			int @int = getInt(args, "frameId", 0);
			string text = getString(args, "context") ?? "hover";
			if (@int != 0 && text != "repl")
			{
				SendText("Warning : Evaluation of variables/watches is always done with the top-level scope.");
			}
			if (text == "repl" && @string.StartsWith("!"))
			{
				ExecuteRepl(@string.Substring(1));
				SendResponse(response);
				return;
			}
			DynValue dynValue = m_Debug.Evaluate(@string) ?? DynValue.Nil;
			m_Variables.Add(dynValue);
			SendResponse(response, new EvaluateResponseBody(dynValue.ToDebugPrintString(), m_Variables.Count - 1)
			{
				type = dynValue.Type.ToLuaDebuggerString()
			});
		}

		private void ExecuteRepl(string cmd)
		{
			bool flag = false;
			cmd = cmd.Trim();
			if (cmd == "help")
			{
				flag = true;
			}
			else if (cmd.StartsWith("geterror"))
			{
				SendText("Current error regex : {0}", m_Debug.ErrorRegex.ToString());
			}
			else if (cmd.StartsWith("seterror"))
			{
				string pattern = cmd.Substring("seterror".Length).Trim();
				try
				{
					Regex errorRegex = new Regex(pattern);
					m_Debug.ErrorRegex = errorRegex;
					SendText("Current error regex : {0}", m_Debug.ErrorRegex.ToString());
				}
				catch (Exception ex)
				{
					SendText("Error setting regex: {0}", ex.Message);
				}
			}
			else if (cmd.StartsWith("execendnotify"))
			{
				string text = cmd.Substring("execendnotify".Length).Trim();
				if (text == "off")
				{
					m_NotifyExecutionEnd = false;
				}
				else if (text == "on")
				{
					m_NotifyExecutionEnd = true;
				}
				else if (text.Length > 0)
				{
					SendText("Error : expected 'on' or 'off'");
				}
				SendText("Notifications of execution end are : {0}", m_NotifyExecutionEnd ? "enabled" : "disabled");
			}
			else if (cmd == "list")
			{
				int num = m_Server.CurrentId ?? (-1000);
				foreach (KeyValuePair<int, string> item in m_Server.GetAttachedDebuggersByIdAndName())
				{
					string text2 = ((item.Key == m_Debug.Id) ? " (this)" : "");
					string text3 = ((item.Key == num) ? " (default)" : "");
					SendText("{0} : {1}{2}{3}", item.Key.ToString().PadLeft(9), item.Value, text3, text2);
				}
			}
			else if (cmd.StartsWith("select") || cmd.StartsWith("switch"))
			{
				string s = cmd.Substring("switch".Length).Trim();
				try
				{
					int num2 = int.Parse(s);
					m_Server.CurrentId = num2;
					if (cmd.StartsWith("switch"))
					{
						Unbind();
					}
					else
					{
						SendText("Next time you'll attach the debugger, it will be atteched to script #{0}", num2);
					}
				}
				catch (Exception ex2)
				{
					SendText("Error setting regex: {0}", ex2.Message);
				}
			}
			else
			{
				SendText("Syntax error : {0}\n", cmd);
				flag = true;
			}
			if (flag)
			{
				SendText("Available commands : ");
				SendText("    !help - gets this help");
				SendText("    !list - lists the other scripts which can be debugged");
				SendText("    !select <id> - select another script for future sessions");
				SendText("    !switch <id> - switch to another script (same as select + disconnect)");
				SendText("    !seterror <regex> - sets the regex which tells which errors to trap");
				SendText("    !geterror - gets the current value of the regex which tells which errors to trap");
				SendText("    !execendnotify [on|off] - sets the notification of end of execution on or off (default = off)");
				SendText("    ... or type an expression to evaluate it on the fly.");
			}
		}

		public override void Launch(Response response, Table arguments)
		{
			SendResponse(response);
		}

		public override void Next(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction
			{
				Action = DebuggerAction.ActionType.StepOver
			});
			SendResponse(response);
		}

		private StoppedEvent CreateStoppedEvent(string reason, string text = null)
		{
			return new StoppedEvent(0, reason, text);
		}

		public override void Pause(Response response, Table arguments)
		{
			m_Debug.PauseRequested = true;
			SendResponse(response);
			SendText("Pause pending -- will pause at first script statement.");
		}

		public override void Scopes(Response response, Table arguments)
		{
			List<Scope> list = new List<Scope>();
			list.Add(new Scope("Locals", 65536));
			list.Add(new Scope("Self", 65537));
			SendResponse(response, new ScopesResponseBody(list));
		}

		public override void SetBreakpoints(Response response, Table args)
		{
			string text = null;
			Table table = args["source"] as Table;
			if (table != null)
			{
				string text2 = table["path"].ToString();
				if (text2 != null && text2.Trim().Length > 0)
				{
					text = text2;
				}
			}
			if (text == null)
			{
				SendErrorResponse(response, 3010, "setBreakpoints: property 'source' is empty or misformed", null, false, true);
				return;
			}
			text = ConvertClientPathToDebugger(text);
			SourceCode sourceCode = m_Debug.FindSourceByName(text);
			if (sourceCode == null)
			{
				SendResponse(response, new SetBreakpointsResponseBody());
				return;
			}
			HashSet<int> hashSet = new HashSet<int>(args.Get("lines").Table.Values.Select(_003CSetBreakpoints_003Eb__19_0).ToArray());
			HashSet<int> hashSet2 = m_Debug.DebugService.ResetBreakPoints(sourceCode, hashSet);
			List<Breakpoint> list = new List<Breakpoint>();
			foreach (int item in hashSet)
			{
				list.Add(new Breakpoint(hashSet2.Contains(item), item));
			}
			response.SetBody(new SetBreakpointsResponseBody(list));
			SendResponse(response);
		}

		public override void StackTrace(Response response, Table args)
		{
			int @int = getInt(args, "levels", 10);
			List<MoonSharp.VsCodeDebugger.SDK.StackFrame> list = new List<MoonSharp.VsCodeDebugger.SDK.StackFrame>();
			List<WatchItem> watches = m_Debug.GetWatches(WatchType.CallStack);
			WatchItem watchItem = m_Debug.GetWatches(WatchType.Threads).LastOrDefault();
			int i = 0;
			for (int num = Math.Min(@int - 3, watches.Count); i < num; i++)
			{
				WatchItem watchItem2 = watches[i];
				string name = watchItem2.Name;
				SourceRef sourceRef = watchItem2.Location ?? DefaultSourceRef;
				int sourceIdx = sourceRef.SourceIdx;
				string path = (sourceRef.IsClrLocation ? "(native)" : (m_Debug.GetSourceFile(sourceIdx) ?? "???"));
				Source source = new Source(Path.GetFileName(path), path);
				list.Add(new MoonSharp.VsCodeDebugger.SDK.StackFrame(i, name, source, ConvertDebuggerLineToClient(sourceRef.FromLine), sourceRef.FromChar, ConvertDebuggerLineToClient(sourceRef.ToLine), sourceRef.ToChar));
			}
			if (watches.Count > @int - 3)
			{
				list.Add(new MoonSharp.VsCodeDebugger.SDK.StackFrame(i++, "(...)", null, 0));
			}
			if (watchItem != null)
			{
				list.Add(new MoonSharp.VsCodeDebugger.SDK.StackFrame(i++, "(" + watchItem.Name + ")", null, 0));
			}
			else
			{
				list.Add(new MoonSharp.VsCodeDebugger.SDK.StackFrame(i++, "(main coroutine)", null, 0));
			}
			list.Add(new MoonSharp.VsCodeDebugger.SDK.StackFrame(i++, "(native)", null, 0));
			SendResponse(response, new StackTraceResponseBody(list));
		}

		private int getInt(Table args, string propName, int defaultValue)
		{
			DynValue dynValue = args.Get(propName);
			if (dynValue.Type != DataType.Number)
			{
				return defaultValue;
			}
			return dynValue.ToObject<int>();
		}

		public override void StepIn(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction
			{
				Action = DebuggerAction.ActionType.StepIn
			});
			SendResponse(response);
		}

		public override void StepOut(Response response, Table arguments)
		{
			m_Debug.QueueAction(new DebuggerAction
			{
				Action = DebuggerAction.ActionType.StepOut
			});
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
			int @int = getInt(arguments, "variablesReference", -1);
			List<Variable> list = new List<Variable>();
			if (@int == 65537)
			{
				VariableInspector.InspectVariable(m_Debug.Evaluate("self"), list);
			}
			else if (@int == 65536)
			{
				foreach (WatchItem watch in m_Debug.GetWatches(WatchType.Locals))
				{
					list.Add(new Variable(watch.Name, (watch.Value ?? DynValue.Void).ToDebugPrintString()));
				}
			}
			else if (@int < 0 || @int >= m_Variables.Count)
			{
				list.Add(new Variable("<error>", null));
			}
			else
			{
				VariableInspector.InspectVariable(m_Variables[@int], list);
			}
			SendResponse(response, new VariablesResponseBody(list));
		}

		void IAsyncDebuggerClient.SendStopEvent()
		{
			SendEvent(CreateStoppedEvent("step"));
		}

		void IAsyncDebuggerClient.OnWatchesUpdated(WatchType watchType)
		{
			if (watchType == WatchType.CallStack)
			{
				m_Variables.Clear();
			}
		}

		void IAsyncDebuggerClient.OnSourceCodeChanged(int sourceID)
		{
			if (m_Debug.IsSourceOverride(sourceID))
			{
				SendText("Loaded source '{0}' -> '{1}'", m_Debug.GetSource(sourceID).Name, m_Debug.GetSourceFile(sourceID));
			}
			else
			{
				SendText("Loaded source '{0}'", m_Debug.GetSource(sourceID).Name);
			}
		}

		public void OnExecutionEnded()
		{
			if (m_NotifyExecutionEnd)
			{
				SendText("Execution ended.");
			}
		}

		private void SendText(string msg, params object[] args)
		{
			msg = string.Format(msg, args);
			SendEvent(new OutputEvent("console", msg + "\n"));
		}

		public void OnException(ScriptRuntimeException ex)
		{
			SendText("runtime error : {0}", ex.DecoratedMessage);
		}

		public void Unbind()
		{
			SendText("Debug session has been closed by the hosting process.");
			SendText("Bye.");
			SendEvent(new TerminatedEvent());
		}

		[CompilerGenerated]
		private int _003CSetBreakpoints_003Eb__19_0(DynValue jt)
		{
			return ConvertClientLineToDebugger(jt.ToObject<int>());
		}
	}
}
