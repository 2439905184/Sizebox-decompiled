using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.VsCodeDebugger.DebuggerLogic;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger
{
	public class MoonSharpVsCodeDebugServer : IDisposable
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<SourceCode, string> _003C_003E9__7_0;

			public static Func<SourceCode, string> _003C_003E9__8_1;

			public static Func<AsyncDebugger, int> _003C_003E9__9_0;

			public static Func<AsyncDebugger, KeyValuePair<int, string>> _003C_003E9__9_1;

			internal string _003C_002Ector_003Eb__7_0(SourceCode s)
			{
				return s.Name;
			}

			internal string _003CAttachToScript_003Eb__8_1(SourceCode s)
			{
				return s.Name;
			}

			internal int _003CGetAttachedDebuggersByIdAndName_003Eb__9_0(AsyncDebugger d)
			{
				return d.Id;
			}

			internal KeyValuePair<int, string> _003CGetAttachedDebuggersByIdAndName_003Eb__9_1(AsyncDebugger d)
			{
				return new KeyValuePair<int, string>(d.Id, d.Name);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass8_0
		{
			public Script script;

			internal bool _003CAttachToScript_003Eb__0(AsyncDebugger d)
			{
				return d.Script == script;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass12_0
		{
			public int? value;

			internal bool _003Cset_CurrentId_003Eb__0(AsyncDebugger d)
			{
				return d.Id == value;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public Script value;

			internal bool _003Cset_Current_003Eb__0(AsyncDebugger d)
			{
				return d.Script == value;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass16_0
		{
			public Script script;

			internal bool _003CDetach_003Eb__0(AsyncDebugger d)
			{
				return d.Script == script;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass23_0
		{
			public TcpListener serverSocket;

			public MoonSharpVsCodeDebugServer _003C_003E4__this;

			internal void _003CStart_003Eb__0()
			{
				_003C_003E4__this.ListenThread(serverSocket);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass24_0
		{
			public Socket clientSocket;

			public MoonSharpVsCodeDebugServer _003C_003E4__this;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass24_1
		{
			public string sessionId;

			public _003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals1;

			internal void _003CListenThread_003Eb__0()
			{
				using (NetworkStream stream = new NetworkStream(CS_0024_003C_003E8__locals1.clientSocket))
				{
					try
					{
						CS_0024_003C_003E8__locals1._003C_003E4__this.RunSession(sessionId, stream);
					}
					catch (Exception ex)
					{
						CS_0024_003C_003E8__locals1._003C_003E4__this.Log("[{0}] : Error : {1}", ex.Message);
					}
				}
				CS_0024_003C_003E8__locals1.clientSocket.Close();
				CS_0024_003C_003E8__locals1._003C_003E4__this.Log("[{0}] : Client connection closed", sessionId);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public Action threadProc;

			internal void _003CSpawnThread_003Eb__0()
			{
				threadProc();
			}
		}

		private object m_Lock = new object();

		private List<AsyncDebugger> m_DebuggerList = new List<AsyncDebugger>();

		private AsyncDebugger m_Current;

		private ManualResetEvent m_StopEvent = new ManualResetEvent(false);

		private bool m_Started;

		private int m_Port;

		public int? CurrentId
		{
			get
			{
				lock (m_Lock)
				{
					return (m_Current != null) ? new int?(m_Current.Id) : null;
				}
			}
			set
			{
				_003C_003Ec__DisplayClass12_0 _003C_003Ec__DisplayClass12_ = new _003C_003Ec__DisplayClass12_0();
				_003C_003Ec__DisplayClass12_.value = value;
				lock (m_Lock)
				{
					if (!_003C_003Ec__DisplayClass12_.value.HasValue)
					{
						m_Current = null;
						return;
					}
					AsyncDebugger asyncDebugger = m_DebuggerList.FirstOrDefault(_003C_003Ec__DisplayClass12_._003Cset_CurrentId_003Eb__0);
					if (asyncDebugger == null)
					{
						throw new ArgumentException("Cannot find debugger with given Id.");
					}
					m_Current = asyncDebugger;
				}
			}
		}

		public Script Current
		{
			get
			{
				lock (m_Lock)
				{
					return (m_Current != null) ? m_Current.Script : null;
				}
			}
			set
			{
				_003C_003Ec__DisplayClass15_0 _003C_003Ec__DisplayClass15_ = new _003C_003Ec__DisplayClass15_0();
				_003C_003Ec__DisplayClass15_.value = value;
				lock (m_Lock)
				{
					if (_003C_003Ec__DisplayClass15_.value == null)
					{
						m_Current = null;
						return;
					}
					AsyncDebugger asyncDebugger = m_DebuggerList.FirstOrDefault(_003C_003Ec__DisplayClass15_._003Cset_Current_003Eb__0);
					if (asyncDebugger == null)
					{
						throw new ArgumentException("Cannot find debugger with given script associated.");
					}
					m_Current = asyncDebugger;
				}
			}
		}

		public Action<string> Logger { get; set; }

		public MoonSharpVsCodeDebugServer(int port = 41912)
		{
			m_Port = port;
		}

		[Obsolete("Use the constructor taking only a port, and the 'Attach' method instead.")]
		public MoonSharpVsCodeDebugServer(Script script, int port, Func<SourceCode, string> sourceFinder = null)
		{
			m_Port = port;
			m_Current = new AsyncDebugger(script, sourceFinder ?? _003C_003Ec._003C_003E9__7_0 ?? (_003C_003Ec._003C_003E9__7_0 = _003C_003Ec._003C_003E9._003C_002Ector_003Eb__7_0), "Default script");
			m_DebuggerList.Add(m_Current);
		}

		public void AttachToScript(Script script, string name, Func<SourceCode, string> sourceFinder = null)
		{
			_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = new _003C_003Ec__DisplayClass8_0();
			_003C_003Ec__DisplayClass8_.script = script;
			lock (m_Lock)
			{
				if (m_DebuggerList.Any(_003C_003Ec__DisplayClass8_._003CAttachToScript_003Eb__0))
				{
					throw new ArgumentException("Script already attached to this debugger.");
				}
				AsyncDebugger asyncDebugger = new AsyncDebugger(_003C_003Ec__DisplayClass8_.script, sourceFinder ?? _003C_003Ec._003C_003E9__8_1 ?? (_003C_003Ec._003C_003E9__8_1 = _003C_003Ec._003C_003E9._003CAttachToScript_003Eb__8_1), name);
				_003C_003Ec__DisplayClass8_.script.AttachDebugger(asyncDebugger);
				m_DebuggerList.Add(asyncDebugger);
				if (m_Current == null)
				{
					m_Current = asyncDebugger;
				}
			}
		}

		public IEnumerable<KeyValuePair<int, string>> GetAttachedDebuggersByIdAndName()
		{
			lock (m_Lock)
			{
				return m_DebuggerList.OrderBy(_003C_003Ec._003C_003E9__9_0 ?? (_003C_003Ec._003C_003E9__9_0 = _003C_003Ec._003C_003E9._003CGetAttachedDebuggersByIdAndName_003Eb__9_0)).Select(_003C_003Ec._003C_003E9__9_1 ?? (_003C_003Ec._003C_003E9__9_1 = _003C_003Ec._003C_003E9._003CGetAttachedDebuggersByIdAndName_003Eb__9_1)).ToArray();
			}
		}

		public void Detach(Script script)
		{
			_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_ = new _003C_003Ec__DisplayClass16_0();
			_003C_003Ec__DisplayClass16_.script = script;
			lock (m_Lock)
			{
				AsyncDebugger asyncDebugger = m_DebuggerList.FirstOrDefault(_003C_003Ec__DisplayClass16_._003CDetach_003Eb__0);
				if (asyncDebugger == null)
				{
					throw new ArgumentException("Cannot detach script - not found.");
				}
				asyncDebugger.Client = null;
				m_DebuggerList.Remove(asyncDebugger);
				if (m_Current == asyncDebugger)
				{
					if (m_DebuggerList.Count > 0)
					{
						m_Current = m_DebuggerList[m_DebuggerList.Count - 1];
					}
					else
					{
						m_Current = null;
					}
				}
			}
		}

		[Obsolete("Use the Attach method instead.")]
		public IDebugger GetDebugger()
		{
			lock (m_Lock)
			{
				return m_Current;
			}
		}

		public void Dispose()
		{
			m_StopEvent.Set();
		}

		public MoonSharpVsCodeDebugServer Start()
		{
			lock (m_Lock)
			{
				_003C_003Ec__DisplayClass23_0 _003C_003Ec__DisplayClass23_ = new _003C_003Ec__DisplayClass23_0();
				_003C_003Ec__DisplayClass23_._003C_003E4__this = this;
				if (m_Started)
				{
					throw new InvalidOperationException("Cannot start; server has already been started.");
				}
				m_StopEvent.Reset();
				_003C_003Ec__DisplayClass23_.serverSocket = null;
				_003C_003Ec__DisplayClass23_.serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), m_Port);
				_003C_003Ec__DisplayClass23_.serverSocket.Start();
				SpawnThread("VsCodeDebugServer_" + m_Port, _003C_003Ec__DisplayClass23_._003CStart_003Eb__0);
				m_Started = true;
				return this;
			}
		}

		private void ListenThread(TcpListener serverSocket)
		{
			try
			{
				while (!m_StopEvent.WaitOne(0))
				{
					_003C_003Ec__DisplayClass24_0 _003C_003Ec__DisplayClass24_ = new _003C_003Ec__DisplayClass24_0();
					_003C_003Ec__DisplayClass24_._003C_003E4__this = this;
					_003C_003Ec__DisplayClass24_.clientSocket = serverSocket.AcceptSocket();
					if (_003C_003Ec__DisplayClass24_.clientSocket != null)
					{
						_003C_003Ec__DisplayClass24_1 _003C_003Ec__DisplayClass24_2 = new _003C_003Ec__DisplayClass24_1();
						_003C_003Ec__DisplayClass24_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass24_;
						_003C_003Ec__DisplayClass24_2.sessionId = Guid.NewGuid().ToString("N");
						Log("[{0}] : Accepted connection from client {1}", _003C_003Ec__DisplayClass24_2.sessionId, _003C_003Ec__DisplayClass24_2.CS_0024_003C_003E8__locals1.clientSocket.RemoteEndPoint);
						SpawnThread("VsCodeDebugSession_" + _003C_003Ec__DisplayClass24_2.sessionId, _003C_003Ec__DisplayClass24_2._003CListenThread_003Eb__0);
					}
				}
			}
			catch (Exception ex)
			{
				Log("Fatal error in listening thread : {0}", ex.Message);
			}
			finally
			{
				if (serverSocket != null)
				{
					serverSocket.Stop();
				}
			}
		}

		private void RunSession(string sessionId, NetworkStream stream)
		{
			DebugSession debugSession = null;
			lock (m_Lock)
			{
				debugSession = ((m_Current == null) ? ((DebugSession)new EmptyDebugSession(this)) : ((DebugSession)new MoonSharpDebugSession(this, m_Current)));
			}
			debugSession.ProcessLoop(stream, stream);
		}

		private void Log(string format, params object[] args)
		{
			Action<string> logger = Logger;
			if (logger != null)
			{
				string obj = string.Format(format, args);
				logger(obj);
			}
		}

		private static void SpawnThread(string name, Action threadProc)
		{
			_003C_003Ec__DisplayClass27_0 _003C_003Ec__DisplayClass27_ = new _003C_003Ec__DisplayClass27_0();
			_003C_003Ec__DisplayClass27_.threadProc = threadProc;
			System.Threading.Thread thread = new System.Threading.Thread(_003C_003Ec__DisplayClass27_._003CSpawnThread_003Eb__0);
			thread.IsBackground = true;
			thread.Name = name;
			thread.Start();
		}
	}
}
