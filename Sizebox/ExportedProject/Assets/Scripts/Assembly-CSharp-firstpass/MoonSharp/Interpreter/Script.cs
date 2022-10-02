using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Diagnostics;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.IO;
using MoonSharp.Interpreter.Platforms;
using MoonSharp.Interpreter.Tree.Expressions;
using MoonSharp.Interpreter.Tree.Fast_Interface;

namespace MoonSharp.Interpreter
{
	public class Script : IScriptPrivateResource
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Instruction, string> _003C_003E9__30_0;

			internal void _003C_002Ecctor_003Eb__8_0(string s)
			{
				GlobalOptions.Platform.DefaultPrint(s);
			}

			internal string _003C_002Ecctor_003Eb__8_1(string s)
			{
				return GlobalOptions.Platform.DefaultInput(s);
			}

			internal string _003CSignalByteCodeChange_003Eb__30_0(Instruction s)
			{
				return s.ToString();
			}
		}

		public const string VERSION = "2.0.0.0";

		public const string LUA_VERSION = "5.2";

		private Processor m_MainProcessor;

		private ByteCode m_ByteCode;

		private List<SourceCode> m_Sources = new List<SourceCode>();

		private Table m_GlobalTable;

		private IDebugger m_Debugger;

		private Table[] m_TypeMetatables = new Table[6];

		public static ScriptOptions DefaultOptions { get; private set; }

		public ScriptOptions Options { get; private set; }

		public static ScriptGlobalOptions GlobalOptions { get; private set; }

		public PerformanceStatistics PerformanceStats { get; private set; }

		public Table Globals
		{
			get
			{
				return m_GlobalTable;
			}
		}

		public bool DebuggerEnabled
		{
			get
			{
				return m_MainProcessor.DebuggerEnabled;
			}
			set
			{
				m_MainProcessor.DebuggerEnabled = value;
			}
		}

		public int SourceCodeCount
		{
			get
			{
				return m_Sources.Count;
			}
		}

		public Table Registry { get; private set; }

		Script IScriptPrivateResource.OwnerScript
		{
			get
			{
				return this;
			}
		}

		static Script()
		{
			GlobalOptions = new ScriptGlobalOptions();
			DefaultOptions = new ScriptOptions
			{
				DebugPrint = _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__8_0,
				DebugInput = _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__8_1,
				CheckThreadAccess = true,
				ScriptLoader = PlatformAutoDetector.GetDefaultScriptLoader(),
				TailCallOptimizationThreshold = 65536
			};
		}

		public Script()
			: this(CoreModules.Preset_Default)
		{
		}

		public Script(CoreModules coreModules)
		{
			Options = new ScriptOptions(DefaultOptions);
			PerformanceStats = new PerformanceStatistics();
			Registry = new Table(this);
			m_ByteCode = new ByteCode(this);
			m_MainProcessor = new Processor(this, m_GlobalTable, m_ByteCode);
			m_GlobalTable = new Table(this).RegisterCoreModules(coreModules);
		}

		public DynValue LoadFunction(string code, Table globalTable = null, string funcFriendlyName = null)
		{
			this.CheckScriptOwnership(globalTable);
			SourceCode sourceCode = new SourceCode(string.Format("libfunc_{0}", funcFriendlyName ?? m_Sources.Count.ToString()), code, m_Sources.Count, this);
			m_Sources.Add(sourceCode);
			int address = Loader_Fast.LoadFunction(this, sourceCode, m_ByteCode, globalTable != null || m_GlobalTable != null);
			SignalSourceCodeChange(sourceCode);
			SignalByteCodeChange();
			return MakeClosure(address, globalTable ?? m_GlobalTable);
		}

		private void SignalByteCodeChange()
		{
			if (m_Debugger != null)
			{
				m_Debugger.SetByteCode(m_ByteCode.Code.Select(_003C_003Ec._003C_003E9__30_0 ?? (_003C_003Ec._003C_003E9__30_0 = _003C_003Ec._003C_003E9._003CSignalByteCodeChange_003Eb__30_0)).ToArray());
			}
		}

		private void SignalSourceCodeChange(SourceCode source)
		{
			if (m_Debugger != null)
			{
				m_Debugger.SetSourceCode(source);
			}
		}

		public DynValue LoadString(string code, Table globalTable = null, string codeFriendlyName = null)
		{
			this.CheckScriptOwnership(globalTable);
			if (code.StartsWith("MoonSharp_dump_b64::"))
			{
				code = code.Substring("MoonSharp_dump_b64::".Length);
				using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(code)))
				{
					return LoadStream(stream, globalTable, codeFriendlyName);
				}
			}
			string text = string.Format("{0}", codeFriendlyName ?? ("chunk_" + m_Sources.Count));
			SourceCode sourceCode = new SourceCode(codeFriendlyName ?? text, code, m_Sources.Count, this);
			m_Sources.Add(sourceCode);
			int address = Loader_Fast.LoadChunk(this, sourceCode, m_ByteCode);
			SignalSourceCodeChange(sourceCode);
			SignalByteCodeChange();
			return MakeClosure(address, globalTable ?? m_GlobalTable);
		}

		public DynValue LoadStream(Stream stream, Table globalTable = null, string codeFriendlyName = null)
		{
			this.CheckScriptOwnership(globalTable);
			Stream stream2 = new UndisposableStream(stream);
			if (!Processor.IsDumpStream(stream2))
			{
				using (StreamReader streamReader = new StreamReader(stream2))
				{
					string code = streamReader.ReadToEnd();
					return LoadString(code, globalTable, codeFriendlyName);
				}
			}
			string text = string.Format("{0}", codeFriendlyName ?? ("dump_" + m_Sources.Count));
			SourceCode sourceCode = new SourceCode(codeFriendlyName ?? text, string.Format("-- This script was decoded from a binary dump - dump_{0}", m_Sources.Count), m_Sources.Count, this);
			m_Sources.Add(sourceCode);
			bool hasUpvalues;
			int address = m_MainProcessor.Undump(stream2, m_Sources.Count - 1, globalTable ?? m_GlobalTable, out hasUpvalues);
			SignalSourceCodeChange(sourceCode);
			SignalByteCodeChange();
			if (hasUpvalues)
			{
				return MakeClosure(address, globalTable ?? m_GlobalTable);
			}
			return MakeClosure(address);
		}

		public void Dump(DynValue function, Stream stream)
		{
			this.CheckScriptOwnership(function);
			if (function.Type != DataType.Function)
			{
				throw new ArgumentException("function arg is not a function!");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("stream is readonly!");
			}
			Closure.UpvaluesType upvaluesType = function.Function.GetUpvaluesType();
			if (upvaluesType == Closure.UpvaluesType.Closure)
			{
				throw new ArgumentException("function arg has upvalues other than _ENV");
			}
			UndisposableStream stream2 = new UndisposableStream(stream);
			m_MainProcessor.Dump(stream2, function.Function.EntryPointByteCodeLocation, upvaluesType == Closure.UpvaluesType.Environment);
		}

		public DynValue LoadFile(string filename, Table globalContext = null, string friendlyFilename = null)
		{
			this.CheckScriptOwnership(globalContext);
			filename = Options.ScriptLoader.ResolveFileName(filename, globalContext ?? m_GlobalTable);
			object obj = Options.ScriptLoader.LoadFile(filename, globalContext ?? m_GlobalTable);
			if (obj is string)
			{
				return LoadString((string)obj, globalContext, friendlyFilename ?? filename);
			}
			if (obj is byte[])
			{
				using (MemoryStream stream = new MemoryStream((byte[])obj))
				{
					return LoadStream(stream, globalContext, friendlyFilename ?? filename);
				}
			}
			if (obj is Stream)
			{
				try
				{
					return LoadStream((Stream)obj, globalContext, friendlyFilename ?? filename);
				}
				finally
				{
					((Stream)obj).Dispose();
				}
			}
			if (obj == null)
			{
				throw new InvalidCastException("Unexpected null from IScriptLoader.LoadFile");
			}
			throw new InvalidCastException(string.Format("Unsupported return type from IScriptLoader.LoadFile : {0}", obj.GetType()));
		}

		public DynValue DoString(string code, Table globalContext = null, string codeFriendlyName = null)
		{
			DynValue function = LoadString(code, globalContext, codeFriendlyName);
			return Call(function);
		}

		public DynValue DoStream(Stream stream, Table globalContext = null, string codeFriendlyName = null)
		{
			DynValue function = LoadStream(stream, globalContext, codeFriendlyName);
			return Call(function);
		}

		public DynValue DoFile(string filename, Table globalContext = null, string codeFriendlyName = null)
		{
			DynValue function = LoadFile(filename, globalContext, codeFriendlyName);
			return Call(function);
		}

		public static DynValue RunFile(string filename)
		{
			return new Script().DoFile(filename);
		}

		public static DynValue RunString(string code)
		{
			return new Script().DoString(code);
		}

		private DynValue MakeClosure(int address, Table envTable = null)
		{
			this.CheckScriptOwnership(envTable);
			Closure function;
			if (envTable == null)
			{
				Instruction instruction = m_MainProcessor.FindMeta(ref address);
				function = ((instruction == null || instruction.NumVal2 != 0) ? new Closure(this, address, new SymbolRef[0], new DynValue[0]) : new Closure(this, address, new SymbolRef[1] { SymbolRef.Upvalue("_ENV", 0) }, new DynValue[1] { instruction.Value }));
			}
			else
			{
				SymbolRef[] symbols = new SymbolRef[1]
				{
					new SymbolRef
					{
						i_Env = null,
						i_Index = 0,
						i_Name = "_ENV",
						i_Type = SymbolRefType.DefaultEnv
					}
				};
				DynValue[] resolvedLocals = new DynValue[1] { DynValue.NewTable(envTable) };
				function = new Closure(this, address, symbols, resolvedLocals);
			}
			return DynValue.NewClosure(function);
		}

		public DynValue Call(DynValue function)
		{
			return Call(function, new DynValue[0]);
		}

		public DynValue Call(DynValue function, params DynValue[] args)
		{
			this.CheckScriptOwnership(function);
			this.CheckScriptOwnership(args);
			if (function.Type != DataType.Function && function.Type != DataType.ClrFunction)
			{
				DynValue metamethod = m_MainProcessor.GetMetamethod(function, "__call");
				if (metamethod == null)
				{
					throw new ArgumentException("function is not a function and has no __call metamethod.");
				}
				DynValue[] array = new DynValue[args.Length + 1];
				array[0] = function;
				for (int i = 0; i < args.Length; i++)
				{
					array[i + 1] = args[i];
				}
				function = metamethod;
				args = array;
			}
			else if (function.Type == DataType.ClrFunction)
			{
				return function.Callback.ClrCallback(CreateDynamicExecutionContext(function.Callback), new CallbackArguments(args, false));
			}
			return m_MainProcessor.Call(function, args);
		}

		public DynValue Call(DynValue function, params object[] args)
		{
			DynValue[] array = new DynValue[args.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DynValue.FromObject(this, args[i]);
			}
			return Call(function, array);
		}

		public DynValue Call(object function)
		{
			return Call(DynValue.FromObject(this, function));
		}

		public DynValue Call(object function, params object[] args)
		{
			return Call(DynValue.FromObject(this, function), args);
		}

		public DynValue CreateCoroutine(DynValue function)
		{
			this.CheckScriptOwnership(function);
			if (function.Type == DataType.Function)
			{
				return m_MainProcessor.Coroutine_Create(function.Function);
			}
			if (function.Type == DataType.ClrFunction)
			{
				return DynValue.NewCoroutine(new Coroutine(function.Callback));
			}
			throw new ArgumentException("function is not of DataType.Function or DataType.ClrFunction");
		}

		public DynValue CreateCoroutine(object function)
		{
			return CreateCoroutine(DynValue.FromObject(this, function));
		}

		public void AttachDebugger(IDebugger debugger)
		{
			DebuggerEnabled = true;
			m_Debugger = debugger;
			m_MainProcessor.AttachDebugger(debugger);
			foreach (SourceCode source in m_Sources)
			{
				SignalSourceCodeChange(source);
			}
			SignalByteCodeChange();
		}

		public SourceCode GetSourceCode(int sourceCodeID)
		{
			return m_Sources[sourceCodeID];
		}

		public DynValue RequireModule(string modname, Table globalContext = null)
		{
			this.CheckScriptOwnership(globalContext);
			Table globalContext2 = globalContext ?? m_GlobalTable;
			string text = Options.ScriptLoader.ResolveModuleName(modname, globalContext2);
			if (text == null)
			{
				throw new ScriptRuntimeException("module '{0}' not found", modname);
			}
			return LoadFile(text, globalContext, text);
		}

		public Table GetTypeMetatable(DataType type)
		{
			if (type >= DataType.Nil && (int)type < m_TypeMetatables.Length)
			{
				return m_TypeMetatables[(int)type];
			}
			return null;
		}

		public void SetTypeMetatable(DataType type, Table metatable)
		{
			this.CheckScriptOwnership(metatable);
			int num = (int)type;
			if (num >= 0 && num < m_TypeMetatables.Length)
			{
				m_TypeMetatables[num] = metatable;
				return;
			}
			throw new ArgumentException("Specified type not supported : " + type);
		}

		public static void WarmUp()
		{
			new Script(CoreModules.Basic).LoadString("return 1;");
		}

		public DynamicExpression CreateDynamicExpression(string code)
		{
			DynamicExprExpression expr = Loader_Fast.LoadDynamicExpr(this, new SourceCode("__dynamic", code, -1, this));
			return new DynamicExpression(this, code, expr);
		}

		public DynamicExpression CreateConstantDynamicExpression(string code, DynValue constant)
		{
			this.CheckScriptOwnership(constant);
			return new DynamicExpression(this, code, constant);
		}

		internal ScriptExecutionContext CreateDynamicExecutionContext(CallbackFunction func = null)
		{
			return new ScriptExecutionContext(m_MainProcessor, func, null, true);
		}

		public static string GetBanner(string subproduct = null)
		{
			subproduct = ((subproduct != null) ? (subproduct + " ") : "");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("MoonSharp {0}{1} [{2}]", subproduct, "2.0.0.0", GlobalOptions.Platform.GetPlatformName()));
			stringBuilder.AppendLine("Copyright (C) 2014-2016 Marco Mastropaolo");
			stringBuilder.AppendLine("http://www.moonsharp.org");
			return stringBuilder.ToString();
		}
	}
}
