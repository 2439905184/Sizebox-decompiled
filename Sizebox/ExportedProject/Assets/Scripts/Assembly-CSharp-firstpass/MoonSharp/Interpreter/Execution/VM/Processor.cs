using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MoonSharp.Interpreter.DataStructs;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Diagnostics;
using MoonSharp.Interpreter.IO;
using MoonSharp.Interpreter.Interop;

namespace MoonSharp.Interpreter.Execution.VM
{
	internal sealed class Processor
	{
		private class DebugContext
		{
			public bool DebuggerEnabled = true;

			public IDebugger DebuggerAttached;

			public DebuggerAction.ActionType DebuggerCurrentAction = DebuggerAction.ActionType.None;

			public int DebuggerCurrentActionTarget = -1;

			public SourceRef LastHlRef;

			public int ExStackDepthAtStep = -1;

			public List<SourceRef> BreakPoints = new List<SourceRef>();

			public bool LineBasedBreakPoints;
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Processor, WatchItem> _003C_003E9__46_0;

			internal WatchItem _003CDebugger_RefreshThreads_003Eb__46_0(Processor c)
			{
				return new WatchItem
				{
					Address = c.AssociatedCoroutine.ReferenceID,
					Name = "coroutine #" + c.AssociatedCoroutine.ReferenceID
				};
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass48_0
		{
			public Processor _003C_003E4__this;

			public ScriptExecutionContext context;

			internal WatchItem _003CDebugger_RefreshWatches_003Eb__0(DynamicExpression w)
			{
				return _003C_003E4__this.Debugger_RefreshWatch(context, w);
			}
		}

		private ByteCode m_RootChunk;

		private FastStack<DynValue> m_ValueStack = new FastStack<DynValue>(131072);

		private FastStack<CallStackItem> m_ExecutionStack = new FastStack<CallStackItem>(131072);

		private List<Processor> m_CoroutinesStack;

		private Table m_GlobalTable;

		private Script m_Script;

		private Processor m_Parent;

		private CoroutineState m_State;

		private bool m_CanYield = true;

		private int m_SavedInstructionPtr = -1;

		private DebugContext m_Debug;

		private int m_OwningThreadID = -1;

		private int m_ExecutionNesting;

		private const ulong DUMP_CHUNK_MAGIC = 1877195438928383261uL;

		private const int DUMP_CHUNK_VERSION = 336;

		private const int YIELD_SPECIAL_TRAP = -99;

		internal long AutoYieldCounter;

		public CoroutineState State
		{
			get
			{
				return m_State;
			}
		}

		public Coroutine AssociatedCoroutine { get; set; }

		internal bool DebuggerEnabled
		{
			get
			{
				return m_Debug.DebuggerEnabled;
			}
			set
			{
				m_Debug.DebuggerEnabled = value;
			}
		}

		public Processor(Script script, Table globalContext, ByteCode byteCode)
		{
			m_CoroutinesStack = new List<Processor>();
			m_Debug = new DebugContext();
			m_RootChunk = byteCode;
			m_GlobalTable = globalContext;
			m_Script = script;
			m_State = CoroutineState.Main;
			DynValue.NewCoroutine(new Coroutine(this));
		}

		private Processor(Processor parentProcessor)
		{
			m_Debug = parentProcessor.m_Debug;
			m_RootChunk = parentProcessor.m_RootChunk;
			m_GlobalTable = parentProcessor.m_GlobalTable;
			m_Script = parentProcessor.m_Script;
			m_Parent = parentProcessor;
			m_State = CoroutineState.NotStarted;
		}

		public DynValue Call(DynValue function, DynValue[] args)
		{
			List<Processor> list = ((m_Parent != null) ? m_Parent.m_CoroutinesStack : m_CoroutinesStack);
			if (list.Count > 0 && list[list.Count - 1] != this)
			{
				return list[list.Count - 1].Call(function, args);
			}
			EnterProcessor();
			try
			{
				IDisposable disposable = m_Script.PerformanceStats.StartStopwatch(PerformanceCounter.Execution);
				m_CanYield = false;
				try
				{
					int instructionPtr = PushClrToScriptStackFrame(CallStackItemFlags.CallEntryPoint, function, args);
					return Processing_Loop(instructionPtr);
				}
				finally
				{
					m_CanYield = true;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			finally
			{
				LeaveProcessor();
			}
		}

		private int PushClrToScriptStackFrame(CallStackItemFlags flags, DynValue function, DynValue[] args)
		{
			if (function == null)
			{
				function = m_ValueStack.Peek();
			}
			else
			{
				m_ValueStack.Push(function);
			}
			args = Internal_AdjustTuple(args);
			for (int i = 0; i < args.Length; i++)
			{
				m_ValueStack.Push(args[i]);
			}
			m_ValueStack.Push(DynValue.NewNumber(args.Length));
			m_ExecutionStack.Push(new CallStackItem
			{
				BasePointer = m_ValueStack.Count,
				Debug_EntryPoint = function.Function.EntryPointByteCodeLocation,
				ReturnAddress = -1,
				ClosureScope = function.Function.ClosureContext,
				CallingSourceRef = SourceRef.GetClrLocation(),
				Flags = flags
			});
			return function.Function.EntryPointByteCodeLocation;
		}

		private void LeaveProcessor()
		{
			m_ExecutionNesting--;
			m_OwningThreadID = -1;
			if (m_Parent != null)
			{
				m_Parent.m_CoroutinesStack.RemoveAt(m_Parent.m_CoroutinesStack.Count - 1);
			}
			if (m_ExecutionNesting == 0 && m_Debug != null && m_Debug.DebuggerEnabled && m_Debug.DebuggerAttached != null)
			{
				m_Debug.DebuggerAttached.SignalExecutionEnded();
			}
		}

		private int GetThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		private void EnterProcessor()
		{
			int threadId = GetThreadId();
			if (m_OwningThreadID >= 0 && m_OwningThreadID != threadId && m_Script.Options.CheckThreadAccess)
			{
				throw new InvalidOperationException(string.Format("Cannot enter the same MoonSharp processor from two different threads : {0} and {1}", m_OwningThreadID, threadId));
			}
			m_OwningThreadID = threadId;
			m_ExecutionNesting++;
			if (m_Parent != null)
			{
				m_Parent.m_CoroutinesStack.Add(this);
			}
		}

		internal SourceRef GetCoroutineSuspendedLocation()
		{
			return GetCurrentSourceRef(m_SavedInstructionPtr);
		}

		internal static bool IsDumpStream(Stream stream)
		{
			if (stream.Length >= 8)
			{
				using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
				{
					ulong num = binaryReader.ReadUInt64();
					stream.Seek(-8L, SeekOrigin.Current);
					return num == 1877195438928383261L;
				}
			}
			return false;
		}

		internal int Dump(Stream stream, int baseAddress, bool hasUpvalues)
		{
			using (BinaryWriter binaryWriter = new BinDumpBinaryWriter(stream, Encoding.UTF8))
			{
				Dictionary<SymbolRef, int> dictionary = new Dictionary<SymbolRef, int>();
				Instruction instruction = FindMeta(ref baseAddress);
				if (instruction == null)
				{
					throw new ArgumentException("baseAddress");
				}
				binaryWriter.Write(1877195438928383261uL);
				binaryWriter.Write(336);
				binaryWriter.Write(hasUpvalues);
				binaryWriter.Write(instruction.NumVal);
				SymbolRef[] array;
				for (int i = 0; i <= instruction.NumVal; i++)
				{
					SymbolRef[] symbolList;
					SymbolRef symbol;
					m_RootChunk.Code[baseAddress + i].GetSymbolReferences(out symbolList, out symbol);
					if (symbol != null)
					{
						AddSymbolToMap(dictionary, symbol);
					}
					if (symbolList != null)
					{
						array = symbolList;
						foreach (SymbolRef s in array)
						{
							AddSymbolToMap(dictionary, s);
						}
					}
				}
				array = dictionary.Keys.ToArray();
				foreach (SymbolRef symbolRef in array)
				{
					if (symbolRef.i_Env != null)
					{
						AddSymbolToMap(dictionary, symbolRef.i_Env);
					}
				}
				SymbolRef[] array2 = new SymbolRef[dictionary.Count];
				foreach (KeyValuePair<SymbolRef, int> item in dictionary)
				{
					array2[item.Value] = item.Key;
				}
				binaryWriter.Write(dictionary.Count);
				array = array2;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].WriteBinary(binaryWriter);
				}
				array = array2;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].WriteBinaryEnv(binaryWriter, dictionary);
				}
				for (int k = 0; k <= instruction.NumVal; k++)
				{
					m_RootChunk.Code[baseAddress + k].WriteBinary(binaryWriter, baseAddress, dictionary);
				}
				return instruction.NumVal + baseAddress + 1;
			}
		}

		private void AddSymbolToMap(Dictionary<SymbolRef, int> symbolMap, SymbolRef s)
		{
			if (!symbolMap.ContainsKey(s))
			{
				symbolMap.Add(s, symbolMap.Count);
			}
		}

		internal int Undump(Stream stream, int sourceID, Table envTable, out bool hasUpvalues)
		{
			int count = m_RootChunk.Code.Count;
			SourceRef chunkRef = new SourceRef(sourceID, 0, 0, 0, 0, false);
			using (BinaryReader binaryReader = new BinDumpBinaryReader(stream, Encoding.UTF8))
			{
				if (binaryReader.ReadUInt64() != 1877195438928383261L)
				{
					throw new ArgumentException("Not a MoonSharp chunk");
				}
				if (binaryReader.ReadInt32() != 336)
				{
					throw new ArgumentException("Invalid version");
				}
				hasUpvalues = binaryReader.ReadBoolean();
				int num = binaryReader.ReadInt32();
				int num2 = binaryReader.ReadInt32();
				SymbolRef[] array = new SymbolRef[num2];
				for (int i = 0; i < num2; i++)
				{
					array[i] = SymbolRef.ReadBinary(binaryReader);
				}
				for (int j = 0; j < num2; j++)
				{
					array[j].ReadBinaryEnv(binaryReader, array);
				}
				for (int k = 0; k <= num; k++)
				{
					Instruction item = Instruction.ReadBinary(chunkRef, binaryReader, count, envTable, array);
					m_RootChunk.Code.Add(item);
				}
				return count;
			}
		}

		public DynValue Coroutine_Create(Closure closure)
		{
			Processor processor = new Processor(this);
			processor.m_ValueStack.Push(DynValue.NewClosure(closure));
			return DynValue.NewCoroutine(new Coroutine(processor));
		}

		public DynValue Coroutine_Resume(DynValue[] args)
		{
			EnterProcessor();
			try
			{
				int instructionPtr = 0;
				if (m_State != CoroutineState.NotStarted && m_State != CoroutineState.Suspended && m_State != CoroutineState.ForceSuspended)
				{
					throw ScriptRuntimeException.CannotResumeNotSuspended(m_State);
				}
				if (m_State == CoroutineState.NotStarted)
				{
					instructionPtr = PushClrToScriptStackFrame(CallStackItemFlags.ResumeEntryPoint, null, args);
				}
				else if (m_State == CoroutineState.Suspended)
				{
					m_ValueStack.Push(DynValue.NewTuple(args));
					instructionPtr = m_SavedInstructionPtr;
				}
				else if (m_State == CoroutineState.ForceSuspended)
				{
					if (args != null && args.Length != 0)
					{
						throw new ArgumentException("When resuming a force-suspended coroutine, args must be empty.");
					}
					instructionPtr = m_SavedInstructionPtr;
				}
				m_State = CoroutineState.Running;
				DynValue dynValue = Processing_Loop(instructionPtr);
				if (dynValue.Type == DataType.YieldRequest)
				{
					if (dynValue.YieldRequest.Forced)
					{
						m_State = CoroutineState.ForceSuspended;
						return dynValue;
					}
					m_State = CoroutineState.Suspended;
					return DynValue.NewTuple(dynValue.YieldRequest.ReturnValues);
				}
				m_State = CoroutineState.Dead;
				return dynValue;
			}
			catch (Exception)
			{
				m_State = CoroutineState.Dead;
				throw;
			}
			finally
			{
				LeaveProcessor();
			}
		}

		internal Instruction FindMeta(ref int baseAddress)
		{
			Instruction instruction = m_RootChunk.Code[baseAddress];
			while (instruction.OpCode == OpCode.Nop)
			{
				baseAddress++;
				instruction = m_RootChunk.Code[baseAddress];
			}
			if (instruction.OpCode != OpCode.Meta)
			{
				return null;
			}
			return instruction;
		}

		internal void AttachDebugger(IDebugger debugger)
		{
			m_Debug.DebuggerAttached = debugger;
			m_Debug.LineBasedBreakPoints = (debugger.GetDebuggerCaps() & DebuggerCaps.HasLineBasedBreakpoints) != 0;
			debugger.SetDebugService(new DebugService(m_Script, this));
		}

		private void ListenDebugger(Instruction instr, int instructionPtr)
		{
			bool flag = false;
			if (instr.SourceCodeRef != null && m_Debug.LastHlRef != null)
			{
				flag = ((!m_Debug.LineBasedBreakPoints) ? (instr.SourceCodeRef != m_Debug.LastHlRef) : (instr.SourceCodeRef.SourceIdx != m_Debug.LastHlRef.SourceIdx || instr.SourceCodeRef.FromLine != m_Debug.LastHlRef.FromLine));
			}
			else if (m_Debug.LastHlRef == null)
			{
				flag = instr.SourceCodeRef != null;
			}
			if (m_Debug.DebuggerAttached.IsPauseRequested() || (instr.SourceCodeRef != null && instr.SourceCodeRef.Breakpoint && flag))
			{
				m_Debug.DebuggerCurrentAction = DebuggerAction.ActionType.None;
				m_Debug.DebuggerCurrentActionTarget = -1;
			}
			switch (m_Debug.DebuggerCurrentAction)
			{
			case DebuggerAction.ActionType.Run:
				if (m_Debug.LineBasedBreakPoints)
				{
					m_Debug.LastHlRef = instr.SourceCodeRef;
				}
				return;
			case DebuggerAction.ActionType.ByteCodeStepOver:
				if (m_Debug.DebuggerCurrentActionTarget != instructionPtr)
				{
					return;
				}
				break;
			case DebuggerAction.ActionType.ByteCodeStepOut:
			case DebuggerAction.ActionType.StepOut:
				if (m_ExecutionStack.Count >= m_Debug.ExStackDepthAtStep)
				{
					return;
				}
				break;
			case DebuggerAction.ActionType.StepIn:
				if (m_ExecutionStack.Count >= m_Debug.ExStackDepthAtStep && (instr.SourceCodeRef == null || instr.SourceCodeRef == m_Debug.LastHlRef))
				{
					return;
				}
				break;
			case DebuggerAction.ActionType.StepOver:
				if (instr.SourceCodeRef == null || instr.SourceCodeRef == m_Debug.LastHlRef || m_ExecutionStack.Count > m_Debug.ExStackDepthAtStep)
				{
					return;
				}
				break;
			}
			RefreshDebugger(false, instructionPtr);
			while (true)
			{
				DebuggerAction action = m_Debug.DebuggerAttached.GetAction(instructionPtr, instr.SourceCodeRef);
				switch (action.Action)
				{
				case DebuggerAction.ActionType.ByteCodeStepOut:
				case DebuggerAction.ActionType.StepIn:
				case DebuggerAction.ActionType.StepOver:
				case DebuggerAction.ActionType.StepOut:
					m_Debug.DebuggerCurrentAction = action.Action;
					m_Debug.LastHlRef = instr.SourceCodeRef;
					m_Debug.ExStackDepthAtStep = m_ExecutionStack.Count;
					return;
				case DebuggerAction.ActionType.ByteCodeStepIn:
					m_Debug.DebuggerCurrentAction = DebuggerAction.ActionType.ByteCodeStepIn;
					m_Debug.DebuggerCurrentActionTarget = -1;
					return;
				case DebuggerAction.ActionType.ByteCodeStepOver:
					m_Debug.DebuggerCurrentAction = DebuggerAction.ActionType.ByteCodeStepOver;
					m_Debug.DebuggerCurrentActionTarget = instructionPtr + 1;
					return;
				case DebuggerAction.ActionType.Run:
					m_Debug.DebuggerCurrentAction = DebuggerAction.ActionType.Run;
					m_Debug.LastHlRef = instr.SourceCodeRef;
					m_Debug.DebuggerCurrentActionTarget = -1;
					return;
				case DebuggerAction.ActionType.ToggleBreakpoint:
					ToggleBreakPoint(action, null);
					RefreshDebugger(true, instructionPtr);
					break;
				case DebuggerAction.ActionType.ResetBreakpoints:
					ResetBreakPoints(action);
					RefreshDebugger(true, instructionPtr);
					break;
				case DebuggerAction.ActionType.SetBreakpoint:
					ToggleBreakPoint(action, true);
					RefreshDebugger(true, instructionPtr);
					break;
				case DebuggerAction.ActionType.ClearBreakpoint:
					ToggleBreakPoint(action, false);
					RefreshDebugger(true, instructionPtr);
					break;
				case DebuggerAction.ActionType.Refresh:
					RefreshDebugger(false, instructionPtr);
					break;
				case DebuggerAction.ActionType.HardRefresh:
					RefreshDebugger(true, instructionPtr);
					break;
				}
			}
		}

		private void ResetBreakPoints(DebuggerAction action)
		{
			SourceCode sourceCode = m_Script.GetSourceCode(action.SourceID);
			ResetBreakPoints(sourceCode, new HashSet<int>(action.Lines));
		}

		internal HashSet<int> ResetBreakPoints(SourceCode src, HashSet<int> lines)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (SourceRef @ref in src.Refs)
			{
				if (!@ref.CannotBreakpoint)
				{
					@ref.Breakpoint = lines.Contains(@ref.FromLine);
					if (@ref.Breakpoint)
					{
						hashSet.Add(@ref.FromLine);
					}
				}
			}
			return hashSet;
		}

		private bool ToggleBreakPoint(DebuggerAction action, bool? state)
		{
			SourceCode sourceCode = m_Script.GetSourceCode(action.SourceID);
			bool flag = false;
			foreach (SourceRef @ref in sourceCode.Refs)
			{
				if (!@ref.CannotBreakpoint && @ref.IncludesLocation(action.SourceID, action.SourceLine, action.SourceCol))
				{
					flag = true;
					if (!state.HasValue)
					{
						@ref.Breakpoint = !@ref.Breakpoint;
					}
					else
					{
						@ref.Breakpoint = state.Value;
					}
					if (@ref.Breakpoint)
					{
						m_Debug.BreakPoints.Add(@ref);
					}
					else
					{
						m_Debug.BreakPoints.Remove(@ref);
					}
				}
			}
			if (!flag)
			{
				int num = int.MaxValue;
				SourceRef sourceRef = null;
				foreach (SourceRef ref2 in sourceCode.Refs)
				{
					if (!ref2.CannotBreakpoint)
					{
						int locationDistance = ref2.GetLocationDistance(action.SourceID, action.SourceLine, action.SourceCol);
						if (locationDistance < num)
						{
							num = locationDistance;
							sourceRef = ref2;
						}
					}
				}
				if (sourceRef != null)
				{
					if (!state.HasValue)
					{
						sourceRef.Breakpoint = !sourceRef.Breakpoint;
					}
					else
					{
						sourceRef.Breakpoint = state.Value;
					}
					if (sourceRef.Breakpoint)
					{
						m_Debug.BreakPoints.Add(sourceRef);
					}
					else
					{
						m_Debug.BreakPoints.Remove(sourceRef);
					}
					return true;
				}
				return false;
			}
			return true;
		}

		private void RefreshDebugger(bool hard, int instructionPtr)
		{
			SourceRef currentSourceRef = GetCurrentSourceRef(instructionPtr);
			ScriptExecutionContext context = new ScriptExecutionContext(this, null, currentSourceRef);
			List<DynamicExpression> watchItems = m_Debug.DebuggerAttached.GetWatchItems();
			List<WatchItem> items = Debugger_GetCallStack(currentSourceRef);
			List<WatchItem> items2 = Debugger_RefreshWatches(context, watchItems);
			List<WatchItem> items3 = Debugger_RefreshVStack();
			List<WatchItem> items4 = Debugger_RefreshLocals(context);
			List<WatchItem> items5 = Debugger_RefreshThreads(context);
			m_Debug.DebuggerAttached.Update(WatchType.CallStack, items);
			m_Debug.DebuggerAttached.Update(WatchType.Watches, items2);
			m_Debug.DebuggerAttached.Update(WatchType.VStack, items3);
			m_Debug.DebuggerAttached.Update(WatchType.Locals, items4);
			m_Debug.DebuggerAttached.Update(WatchType.Threads, items5);
			if (hard)
			{
				m_Debug.DebuggerAttached.RefreshBreakpoints(m_Debug.BreakPoints);
			}
		}

		private List<WatchItem> Debugger_RefreshThreads(ScriptExecutionContext context)
		{
			return ((m_Parent != null) ? m_Parent.m_CoroutinesStack : m_CoroutinesStack).Select(_003C_003Ec._003C_003E9__46_0 ?? (_003C_003Ec._003C_003E9__46_0 = _003C_003Ec._003C_003E9._003CDebugger_RefreshThreads_003Eb__46_0)).ToList();
		}

		private List<WatchItem> Debugger_RefreshVStack()
		{
			List<WatchItem> list = new List<WatchItem>();
			for (int i = 0; i < Math.Min(32, m_ValueStack.Count); i++)
			{
				list.Add(new WatchItem
				{
					Address = i,
					Value = m_ValueStack.Peek(i)
				});
			}
			return list;
		}

		private List<WatchItem> Debugger_RefreshWatches(ScriptExecutionContext context, List<DynamicExpression> watchList)
		{
			_003C_003Ec__DisplayClass48_0 _003C_003Ec__DisplayClass48_ = new _003C_003Ec__DisplayClass48_0();
			_003C_003Ec__DisplayClass48_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass48_.context = context;
			return watchList.Select(_003C_003Ec__DisplayClass48_._003CDebugger_RefreshWatches_003Eb__0).ToList();
		}

		private List<WatchItem> Debugger_RefreshLocals(ScriptExecutionContext context)
		{
			List<WatchItem> list = new List<WatchItem>();
			CallStackItem callStackItem = m_ExecutionStack.Peek();
			if (callStackItem != null && callStackItem.Debug_Symbols != null && callStackItem.LocalScope != null)
			{
				int num = Math.Min(callStackItem.Debug_Symbols.Length, callStackItem.LocalScope.Length);
				for (int i = 0; i < num; i++)
				{
					list.Add(new WatchItem
					{
						IsError = false,
						LValue = callStackItem.Debug_Symbols[i],
						Value = callStackItem.LocalScope[i],
						Name = callStackItem.Debug_Symbols[i].i_Name
					});
				}
			}
			return list;
		}

		private WatchItem Debugger_RefreshWatch(ScriptExecutionContext context, DynamicExpression dynExpr)
		{
			try
			{
				SymbolRef lValue = dynExpr.FindSymbol(context);
				DynValue value = dynExpr.Evaluate(context);
				return new WatchItem
				{
					IsError = dynExpr.IsConstant(),
					LValue = lValue,
					Value = value,
					Name = dynExpr.ExpressionCode
				};
			}
			catch (Exception ex)
			{
				return new WatchItem
				{
					IsError = true,
					Value = DynValue.NewString(ex.Message),
					Name = dynExpr.ExpressionCode
				};
			}
		}

		internal List<WatchItem> Debugger_GetCallStack(SourceRef startingRef)
		{
			List<WatchItem> list = new List<WatchItem>();
			for (int i = 0; i < m_ExecutionStack.Count; i++)
			{
				CallStackItem callStackItem = m_ExecutionStack.Peek(i);
				Instruction instruction = m_RootChunk.Code[callStackItem.Debug_EntryPoint];
				string name = ((instruction.OpCode == OpCode.Meta) ? instruction.Name : null);
				if (callStackItem.ClrFunction != null)
				{
					list.Add(new WatchItem
					{
						Address = -1,
						BasePtr = -1,
						RetAddress = callStackItem.ReturnAddress,
						Location = startingRef,
						Name = callStackItem.ClrFunction.Name
					});
				}
				else
				{
					list.Add(new WatchItem
					{
						Address = callStackItem.Debug_EntryPoint,
						BasePtr = callStackItem.BasePointer,
						RetAddress = callStackItem.ReturnAddress,
						Name = name,
						Location = startingRef
					});
				}
				startingRef = callStackItem.CallingSourceRef;
				if (callStackItem.Continuation != null)
				{
					list.Add(new WatchItem
					{
						Name = callStackItem.Continuation.Name,
						Location = SourceRef.GetClrLocation()
					});
				}
			}
			return list;
		}

		private SourceRef GetCurrentSourceRef(int instructionPtr)
		{
			if (instructionPtr >= 0 && instructionPtr < m_RootChunk.Code.Count)
			{
				return m_RootChunk.Code[instructionPtr].SourceCodeRef;
			}
			return null;
		}

		private void FillDebugData(InterpreterException ex, int ip)
		{
			ip = ((ip != -99) ? (ip - 1) : m_SavedInstructionPtr);
			ex.InstructionPtr = ip;
			SourceRef currentSourceRef = GetCurrentSourceRef(ip);
			ex.DecorateMessage(m_Script, currentSourceRef, ip);
			ex.CallStack = Debugger_GetCallStack(currentSourceRef);
		}

		internal Table GetMetatable(DynValue value)
		{
			if (value.Type == DataType.Table)
			{
				return value.Table.MetaTable;
			}
			if (value.Type.CanHaveTypeMetatables())
			{
				return m_Script.GetTypeMetatable(value.Type);
			}
			return null;
		}

		internal DynValue GetBinaryMetamethod(DynValue op1, DynValue op2, string eventName)
		{
			Table metatable = GetMetatable(op1);
			if (metatable != null)
			{
				DynValue dynValue = metatable.RawGet(eventName);
				if (dynValue != null && dynValue.IsNotNil())
				{
					return dynValue;
				}
			}
			Table metatable2 = GetMetatable(op2);
			if (metatable2 != null)
			{
				DynValue dynValue2 = metatable2.RawGet(eventName);
				if (dynValue2 != null && dynValue2.IsNotNil())
				{
					return dynValue2;
				}
			}
			if (op1.Type == DataType.UserData)
			{
				DynValue dynValue3 = op1.UserData.Descriptor.MetaIndex(m_Script, op1.UserData.Object, eventName);
				if (dynValue3 != null)
				{
					return dynValue3;
				}
			}
			if (op2.Type == DataType.UserData)
			{
				DynValue dynValue4 = op2.UserData.Descriptor.MetaIndex(m_Script, op2.UserData.Object, eventName);
				if (dynValue4 != null)
				{
					return dynValue4;
				}
			}
			return null;
		}

		internal DynValue GetMetamethod(DynValue value, string metamethod)
		{
			if (value.Type == DataType.UserData)
			{
				DynValue dynValue = value.UserData.Descriptor.MetaIndex(m_Script, value.UserData.Object, metamethod);
				if (dynValue != null)
				{
					return dynValue;
				}
			}
			return GetMetamethodRaw(value, metamethod);
		}

		internal DynValue GetMetamethodRaw(DynValue value, string metamethod)
		{
			Table metatable = GetMetatable(value);
			if (metatable == null)
			{
				return null;
			}
			DynValue dynValue = metatable.RawGet(metamethod);
			if (dynValue == null || dynValue.IsNil())
			{
				return null;
			}
			return dynValue;
		}

		internal Script GetScript()
		{
			return m_Script;
		}

		private DynValue Processing_Loop(int instructionPtr)
		{
			long num = 0L;
			bool flag = AutoYieldCounter > 0 && m_CanYield && State != CoroutineState.Main;
			while (true)
			{
				try
				{
					while (true)
					{
						Instruction instruction = m_RootChunk.Code[instructionPtr];
						if (m_Debug.DebuggerAttached != null)
						{
							ListenDebugger(instruction, instructionPtr);
						}
						num++;
						if (flag && num > AutoYieldCounter)
						{
							m_SavedInstructionPtr = instructionPtr;
							return DynValue.NewForcedYieldReq();
						}
						instructionPtr++;
						switch (instruction.OpCode)
						{
						case OpCode.Nop:
						case OpCode.Debug:
						case OpCode.Meta:
							continue;
						case OpCode.Pop:
							m_ValueStack.RemoveLast(instruction.NumVal);
							continue;
						case OpCode.Copy:
							m_ValueStack.Push(m_ValueStack.Peek(instruction.NumVal));
							continue;
						case OpCode.Swap:
							ExecSwap(instruction);
							continue;
						case OpCode.Literal:
							m_ValueStack.Push(instruction.Value);
							continue;
						case OpCode.Add:
							instructionPtr = ExecAdd(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Concat:
							instructionPtr = ExecConcat(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Neg:
							instructionPtr = ExecNeg(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Sub:
							instructionPtr = ExecSub(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Mul:
							instructionPtr = ExecMul(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Div:
							instructionPtr = ExecDiv(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Mod:
							instructionPtr = ExecMod(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Power:
							instructionPtr = ExecPower(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Eq:
							instructionPtr = ExecEq(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.LessEq:
							instructionPtr = ExecLessEq(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Less:
							instructionPtr = ExecLess(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Len:
							instructionPtr = ExecLen(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Call:
						case OpCode.ThisCall:
							instructionPtr = Internal_ExecCall(instruction.NumVal, instructionPtr, null, null, instruction.OpCode == OpCode.ThisCall, instruction.Name);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Scalar:
							m_ValueStack.Push(m_ValueStack.Pop().ToScalar());
							continue;
						case OpCode.Not:
							ExecNot(instruction);
							continue;
						case OpCode.CNot:
							ExecCNot(instruction);
							continue;
						case OpCode.JtOrPop:
						case OpCode.JfOrPop:
							instructionPtr = ExecShortCircuitingOperator(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.JNil:
						{
							DynValue dynValue = m_ValueStack.Pop().ToScalar();
							if (dynValue.Type == DataType.Nil || dynValue.Type == DataType.Void)
							{
								instructionPtr = instruction.NumVal;
							}
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						}
						case OpCode.Jf:
							instructionPtr = JumpBool(instruction, false, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Jump:
							instructionPtr = instruction.NumVal;
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.MkTuple:
							ExecMkTuple(instruction);
							continue;
						case OpCode.Clean:
							ClearBlockData(instruction);
							continue;
						case OpCode.Closure:
							ExecClosure(instruction);
							continue;
						case OpCode.BeginFn:
							ExecBeginFn(instruction);
							continue;
						case OpCode.ToBool:
							m_ValueStack.Push(DynValue.NewBoolean(m_ValueStack.Pop().ToScalar().CastToBool()));
							continue;
						case OpCode.Args:
							ExecArgs(instruction);
							continue;
						case OpCode.Ret:
							instructionPtr = ExecRet(instruction);
							if (instructionPtr == -99)
							{
								break;
							}
							if (instructionPtr >= 0)
							{
								continue;
							}
							goto end_IL_0023;
						case OpCode.Incr:
							ExecIncr(instruction);
							continue;
						case OpCode.ToNum:
							ExecToNum(instruction);
							continue;
						case OpCode.JFor:
							instructionPtr = ExecJFor(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.NewTable:
							if (instruction.NumVal == 0)
							{
								m_ValueStack.Push(DynValue.NewTable(m_Script));
							}
							else
							{
								m_ValueStack.Push(DynValue.NewPrimeTable());
							}
							continue;
						case OpCode.IterPrep:
							ExecIterPrep(instruction);
							continue;
						case OpCode.IterUpd:
							ExecIterUpd(instruction);
							continue;
						case OpCode.ExpTuple:
							ExecExpTuple(instruction);
							continue;
						case OpCode.Local:
						{
							DynValue[] localScope = m_ExecutionStack.Peek().LocalScope;
							int i_Index = instruction.Symbol.i_Index;
							m_ValueStack.Push(localScope[i_Index].AsReadOnly());
							continue;
						}
						case OpCode.Upvalue:
							m_ValueStack.Push(m_ExecutionStack.Peek().ClosureScope[instruction.Symbol.i_Index].AsReadOnly());
							continue;
						case OpCode.StoreUpv:
							ExecStoreUpv(instruction);
							continue;
						case OpCode.StoreLcl:
							ExecStoreLcl(instruction);
							continue;
						case OpCode.TblInitN:
							ExecTblInitN(instruction);
							continue;
						case OpCode.TblInitI:
							ExecTblInitI(instruction);
							continue;
						case OpCode.Index:
						case OpCode.IndexN:
						case OpCode.IndexL:
							instructionPtr = ExecIndex(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.IndexSet:
						case OpCode.IndexSetN:
						case OpCode.IndexSetL:
							instructionPtr = ExecIndexSet(instruction, instructionPtr);
							if (instructionPtr != -99)
							{
								continue;
							}
							break;
						case OpCode.Invalid:
							throw new NotImplementedException(string.Format("Invalid opcode : {0}", instruction.Name));
						default:
							throw new NotImplementedException(string.Format("Execution for {0} not implented yet!", instruction.OpCode));
						}
						DynValue result = m_ValueStack.Pop().ToScalar();
						if (m_CanYield)
						{
							return result;
						}
						if (State == CoroutineState.Main)
						{
							throw ScriptRuntimeException.CannotYieldMain();
						}
						throw ScriptRuntimeException.CannotYield();
						continue;
						end_IL_0023:
						break;
					}
				}
				catch (InterpreterException ex)
				{
					FillDebugData(ex, instructionPtr);
					if (!(ex is ScriptRuntimeException))
					{
						ex.Rethrow();
						throw;
					}
					if (m_Debug.DebuggerAttached != null && m_Debug.DebuggerAttached.SignalRuntimeException((ScriptRuntimeException)ex) && instructionPtr >= 0 && instructionPtr < m_RootChunk.Code.Count)
					{
						ListenDebugger(m_RootChunk.Code[instructionPtr], instructionPtr);
					}
					for (int i = 0; i < m_ExecutionStack.Count; i++)
					{
						CallStackItem callStackItem = m_ExecutionStack.Peek(i);
						if (callStackItem.ErrorHandlerBeforeUnwind != null)
						{
							ex.DecoratedMessage = PerformMessageDecorationBeforeUnwind(callStackItem.ErrorHandlerBeforeUnwind, ex.DecoratedMessage, GetCurrentSourceRef(instructionPtr));
						}
					}
					CallStackItem callStackItem2;
					while (true)
					{
						if (m_ExecutionStack.Count > 0)
						{
							callStackItem2 = PopToBasePointer();
							if (callStackItem2.ErrorHandler != null)
							{
								break;
							}
							if ((callStackItem2.Flags & CallStackItemFlags.EntryPoint) != 0)
							{
								ex.Rethrow();
								throw;
							}
							continue;
						}
						ex.Rethrow();
						throw;
					}
					instructionPtr = callStackItem2.ReturnAddress;
					if (callStackItem2.ClrFunction == null)
					{
						int num2 = (int)m_ValueStack.Pop().Number;
						m_ValueStack.RemoveLast(num2 + 1);
					}
					DynValue[] args = new DynValue[1] { DynValue.NewString(ex.DecoratedMessage) };
					DynValue item = callStackItem2.ErrorHandler.Invoke(new ScriptExecutionContext(this, callStackItem2.ErrorHandler, GetCurrentSourceRef(instructionPtr)), args);
					m_ValueStack.Push(item);
					continue;
				}
				break;
			}
			return m_ValueStack.Pop();
		}

		internal string PerformMessageDecorationBeforeUnwind(DynValue messageHandler, string decoratedMessage, SourceRef sourceRef)
		{
			try
			{
				DynValue[] args = new DynValue[1] { DynValue.NewString(decoratedMessage) };
				DynValue nil = DynValue.Nil;
				if (messageHandler.Type == DataType.Function)
				{
					nil = Call(messageHandler, args);
				}
				else
				{
					if (messageHandler.Type != DataType.ClrFunction)
					{
						throw new ScriptRuntimeException("error handler not set to a function");
					}
					ScriptExecutionContext executionContext = new ScriptExecutionContext(this, messageHandler.Callback, sourceRef);
					nil = messageHandler.Callback.Invoke(executionContext, args);
				}
				string text = nil.ToPrintString();
				if (text != null)
				{
					return text;
				}
				return decoratedMessage;
			}
			catch (ScriptRuntimeException ex)
			{
				return ex.Message + "\n" + decoratedMessage;
			}
		}

		private void AssignLocal(SymbolRef symref, DynValue value)
		{
			CallStackItem callStackItem = m_ExecutionStack.Peek();
			DynValue dynValue = callStackItem.LocalScope[symref.i_Index];
			if (dynValue == null)
			{
				dynValue = (callStackItem.LocalScope[symref.i_Index] = DynValue.NewNil());
			}
			dynValue.Assign(value);
		}

		private void ExecStoreLcl(Instruction i)
		{
			DynValue storeValue = GetStoreValue(i);
			SymbolRef symbol = i.Symbol;
			AssignLocal(symbol, storeValue);
		}

		private void ExecStoreUpv(Instruction i)
		{
			DynValue storeValue = GetStoreValue(i);
			SymbolRef symbol = i.Symbol;
			CallStackItem callStackItem = m_ExecutionStack.Peek();
			DynValue dynValue = callStackItem.ClosureScope[symbol.i_Index];
			if (dynValue == null)
			{
				dynValue = (callStackItem.ClosureScope[symbol.i_Index] = DynValue.NewNil());
			}
			dynValue.Assign(storeValue);
		}

		private void ExecSwap(Instruction i)
		{
			DynValue item = m_ValueStack.Peek(i.NumVal);
			DynValue item2 = m_ValueStack.Peek(i.NumVal2);
			m_ValueStack.Set(i.NumVal, item2);
			m_ValueStack.Set(i.NumVal2, item);
		}

		private DynValue GetStoreValue(Instruction i)
		{
			int numVal = i.NumVal;
			int numVal2 = i.NumVal2;
			DynValue dynValue = m_ValueStack.Peek(numVal);
			if (dynValue.Type == DataType.Tuple)
			{
				if (numVal2 >= dynValue.Tuple.Length)
				{
					return DynValue.NewNil();
				}
				return dynValue.Tuple[numVal2];
			}
			if (numVal2 != 0)
			{
				return DynValue.NewNil();
			}
			return dynValue;
		}

		private void ExecClosure(Instruction i)
		{
			Closure function = new Closure(m_Script, i.NumVal, i.SymbolList, i.SymbolList.Select(_003CExecClosure_003Eb__68_0).ToList());
			m_ValueStack.Push(DynValue.NewClosure(function));
		}

		private DynValue GetUpvalueSymbol(SymbolRef s)
		{
			if (s.Type == SymbolRefType.Local)
			{
				return m_ExecutionStack.Peek().LocalScope[s.i_Index];
			}
			if (s.Type == SymbolRefType.Upvalue)
			{
				return m_ExecutionStack.Peek().ClosureScope[s.i_Index];
			}
			throw new Exception("unsupported symbol type");
		}

		private void ExecMkTuple(Instruction i)
		{
			Slice<DynValue> values = new Slice<DynValue>(m_ValueStack, m_ValueStack.Count - i.NumVal, i.NumVal, false);
			DynValue[] values2 = Internal_AdjustTuple(values);
			m_ValueStack.RemoveLast(i.NumVal);
			m_ValueStack.Push(DynValue.NewTuple(values2));
		}

		private void ExecToNum(Instruction i)
		{
			double? num = m_ValueStack.Pop().ToScalar().CastToNumber();
			if (num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(num.Value));
				return;
			}
			throw ScriptRuntimeException.ConvertToNumberFailed(i.NumVal);
		}

		private void ExecIterUpd(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Peek();
			m_ValueStack.Peek(1).Tuple[2] = dynValue;
		}

		private void ExecExpTuple(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Peek(i.NumVal);
			if (dynValue.Type == DataType.Tuple)
			{
				for (int j = 0; j < dynValue.Tuple.Length; j++)
				{
					m_ValueStack.Push(dynValue.Tuple[j]);
				}
			}
			else
			{
				m_ValueStack.Push(dynValue);
			}
		}

		private void ExecIterPrep(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Pop();
			if (dynValue.Type != DataType.Tuple)
			{
				dynValue = DynValue.NewTuple(dynValue, DynValue.Nil, DynValue.Nil);
			}
			DynValue dynValue2 = ((dynValue.Tuple.Length >= 1) ? dynValue.Tuple[0] : DynValue.Nil);
			DynValue dynValue3 = ((dynValue.Tuple.Length >= 2) ? dynValue.Tuple[1] : DynValue.Nil);
			DynValue dynValue4 = ((dynValue.Tuple.Length >= 3) ? dynValue.Tuple[2] : DynValue.Nil);
			if (dynValue2.Type != DataType.Function && dynValue2.Type != DataType.ClrFunction)
			{
				DynValue metamethod = GetMetamethod(dynValue2, "__iterator");
				if (metamethod != null && !metamethod.IsNil())
				{
					dynValue = ((metamethod.Type == DataType.Tuple) ? metamethod : GetScript().Call(metamethod, dynValue2, dynValue3, dynValue4));
					dynValue2 = ((dynValue.Tuple.Length >= 1) ? dynValue.Tuple[0] : DynValue.Nil);
					dynValue3 = ((dynValue.Tuple.Length >= 2) ? dynValue.Tuple[1] : DynValue.Nil);
					dynValue4 = ((dynValue.Tuple.Length >= 3) ? dynValue.Tuple[2] : DynValue.Nil);
					m_ValueStack.Push(DynValue.NewTuple(dynValue2, dynValue3, dynValue4));
				}
				else if (dynValue2.Type == DataType.Table)
				{
					DynValue metamethod2 = GetMetamethod(dynValue2, "__call");
					if (metamethod2 == null || metamethod2.IsNil())
					{
						m_ValueStack.Push(EnumerableWrapper.ConvertTable(dynValue2.Table));
					}
				}
			}
			m_ValueStack.Push(DynValue.NewTuple(dynValue2, dynValue3, dynValue4));
		}

		private int ExecJFor(Instruction i, int instructionPtr)
		{
			double number = m_ValueStack.Peek().Number;
			double number2 = m_ValueStack.Peek(1).Number;
			double number3 = m_ValueStack.Peek(2).Number;
			if (!((number2 > 0.0) ? (number <= number3) : (number >= number3)))
			{
				return i.NumVal;
			}
			return instructionPtr;
		}

		private void ExecIncr(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Peek();
			DynValue dynValue2 = m_ValueStack.Peek(i.NumVal);
			if (dynValue.ReadOnly)
			{
				m_ValueStack.Pop();
				if (dynValue.ReadOnly)
				{
					dynValue = dynValue.CloneAsWritable();
				}
				m_ValueStack.Push(dynValue);
			}
			dynValue.AssignNumber(dynValue.Number + dynValue2.Number);
		}

		private void ExecCNot(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			if (dynValue2.Type != DataType.Boolean)
			{
				throw new InternalErrorException("CNOT had non-bool arg");
			}
			if (dynValue2.CastToBool())
			{
				m_ValueStack.Push(DynValue.NewBoolean(!dynValue.CastToBool()));
			}
			else
			{
				m_ValueStack.Push(DynValue.NewBoolean(dynValue.CastToBool()));
			}
		}

		private void ExecNot(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			m_ValueStack.Push(DynValue.NewBoolean(!dynValue.CastToBool()));
		}

		private void ExecBeginFn(Instruction i)
		{
			CallStackItem callStackItem = m_ExecutionStack.Peek();
			callStackItem.Debug_Symbols = i.SymbolList;
			callStackItem.LocalScope = new DynValue[i.NumVal];
			ClearBlockData(i);
		}

		private CallStackItem PopToBasePointer()
		{
			CallStackItem callStackItem = m_ExecutionStack.Pop();
			if (callStackItem.BasePointer >= 0)
			{
				m_ValueStack.CropAtCount(callStackItem.BasePointer);
			}
			return callStackItem;
		}

		private int PopExecStackAndCheckVStack(int vstackguard)
		{
			CallStackItem callStackItem = m_ExecutionStack.Pop();
			if (vstackguard != callStackItem.BasePointer)
			{
				throw new InternalErrorException("StackGuard violation");
			}
			return callStackItem.ReturnAddress;
		}

		private IList<DynValue> CreateArgsListForFunctionCall(int numargs, int offsFromTop)
		{
			if (numargs == 0)
			{
				return new DynValue[0];
			}
			DynValue dynValue = m_ValueStack.Peek(offsFromTop);
			if (dynValue.Type == DataType.Tuple && dynValue.Tuple.Length > 1)
			{
				List<DynValue> list = new List<DynValue>();
				for (int i = 0; i < numargs - 1; i++)
				{
					list.Add(m_ValueStack.Peek(numargs - i - 1 + offsFromTop));
				}
				for (int j = 0; j < dynValue.Tuple.Length; j++)
				{
					list.Add(dynValue.Tuple[j]);
				}
				return list;
			}
			return new Slice<DynValue>(m_ValueStack, m_ValueStack.Count - numargs - offsFromTop, numargs, false);
		}

		private void ExecArgs(Instruction I)
		{
			int numargs = (int)m_ValueStack.Peek().Number;
			IList<DynValue> list = CreateArgsListForFunctionCall(numargs, 1);
			for (int i = 0; i < I.SymbolList.Length; i++)
			{
				if (i >= list.Count)
				{
					AssignLocal(I.SymbolList[i], DynValue.NewNil());
				}
				else if (i == I.SymbolList.Length - 1 && I.SymbolList[i].i_Name == "...")
				{
					int num = list.Count - i;
					DynValue[] array = new DynValue[num];
					int num2 = 0;
					while (num2 < num)
					{
						array[num2] = list[i].ToScalar().CloneAsWritable();
						num2++;
						i++;
					}
					AssignLocal(I.SymbolList[I.SymbolList.Length - 1], DynValue.NewTuple(Internal_AdjustTuple(array)));
				}
				else
				{
					AssignLocal(I.SymbolList[i], list[i].ToScalar().CloneAsWritable());
				}
			}
		}

		private int Internal_ExecCall(int argsCount, int instructionPtr, CallbackFunction handler = null, CallbackFunction continuation = null, bool thisCall = false, string debugText = null, DynValue unwindHandler = null)
		{
			DynValue dynValue = m_ValueStack.Peek(argsCount);
			CallStackItemFlags callStackItemFlags = (thisCall ? CallStackItemFlags.MethodCall : CallStackItemFlags.None);
			if (((m_ExecutionStack.Count > m_Script.Options.TailCallOptimizationThreshold && m_ExecutionStack.Count > 1) || (m_ValueStack.Count > m_Script.Options.TailCallOptimizationThreshold && m_ValueStack.Count > 1)) && instructionPtr >= 0 && instructionPtr < m_RootChunk.Code.Count)
			{
				Instruction instruction = m_RootChunk.Code[instructionPtr];
				if (instruction.OpCode == OpCode.Ret && instruction.NumVal == 1)
				{
					CallStackItem callStackItem = m_ExecutionStack.Peek();
					if (callStackItem.ClrFunction == null && callStackItem.Continuation == null && callStackItem.ErrorHandler == null && callStackItem.ErrorHandlerBeforeUnwind == null && continuation == null && unwindHandler == null && handler == null)
					{
						instructionPtr = PerformTCO(instructionPtr, argsCount);
						callStackItemFlags |= CallStackItemFlags.TailCall;
					}
				}
			}
			if (dynValue.Type == DataType.ClrFunction)
			{
				IList<DynValue> args = CreateArgsListForFunctionCall(argsCount, 0);
				SourceRef currentSourceRef = GetCurrentSourceRef(instructionPtr);
				m_ExecutionStack.Push(new CallStackItem
				{
					ClrFunction = dynValue.Callback,
					ReturnAddress = instructionPtr,
					CallingSourceRef = currentSourceRef,
					BasePointer = -1,
					ErrorHandler = handler,
					Continuation = continuation,
					ErrorHandlerBeforeUnwind = unwindHandler,
					Flags = callStackItemFlags
				});
				DynValue item = dynValue.Callback.Invoke(new ScriptExecutionContext(this, dynValue.Callback, currentSourceRef), args, thisCall);
				m_ValueStack.RemoveLast(argsCount + 1);
				m_ValueStack.Push(item);
				m_ExecutionStack.Pop();
				return Internal_CheckForTailRequests(null, instructionPtr);
			}
			if (dynValue.Type == DataType.Function)
			{
				m_ValueStack.Push(DynValue.NewNumber(argsCount));
				m_ExecutionStack.Push(new CallStackItem
				{
					BasePointer = m_ValueStack.Count,
					ReturnAddress = instructionPtr,
					Debug_EntryPoint = dynValue.Function.EntryPointByteCodeLocation,
					CallingSourceRef = GetCurrentSourceRef(instructionPtr),
					ClosureScope = dynValue.Function.ClosureContext,
					ErrorHandler = handler,
					Continuation = continuation,
					ErrorHandlerBeforeUnwind = unwindHandler,
					Flags = callStackItemFlags
				});
				return dynValue.Function.EntryPointByteCodeLocation;
			}
			DynValue metamethod = GetMetamethod(dynValue, "__call");
			if (metamethod != null && metamethod.IsNotNil())
			{
				DynValue[] array = new DynValue[argsCount + 1];
				for (int i = 0; i < argsCount + 1; i++)
				{
					array[i] = m_ValueStack.Pop();
				}
				m_ValueStack.Push(metamethod);
				for (int num = argsCount; num >= 0; num--)
				{
					m_ValueStack.Push(array[num]);
				}
				return Internal_ExecCall(argsCount + 1, instructionPtr, handler, continuation);
			}
			throw ScriptRuntimeException.AttemptToCallNonFunc(dynValue.Type, debugText);
		}

		private int PerformTCO(int instructionPtr, int argsCount)
		{
			DynValue[] array = new DynValue[argsCount + 1];
			for (int i = 0; i <= argsCount; i++)
			{
				array[i] = m_ValueStack.Pop();
			}
			int returnAddress = PopToBasePointer().ReturnAddress;
			int num = (int)m_ValueStack.Pop().Number;
			m_ValueStack.RemoveLast(num + 1);
			for (int num2 = argsCount; num2 >= 0; num2--)
			{
				m_ValueStack.Push(array[num2]);
			}
			return returnAddress;
		}

		private int ExecRet(Instruction i)
		{
			int num = 0;
			CallStackItem callStackItem;
			if (i.NumVal == 0)
			{
				callStackItem = PopToBasePointer();
				num = callStackItem.ReturnAddress;
				int num2 = (int)m_ValueStack.Pop().Number;
				m_ValueStack.RemoveLast(num2 + 1);
				m_ValueStack.Push(DynValue.Void);
			}
			else
			{
				if (i.NumVal != 1)
				{
					throw new InternalErrorException("RET supports only 0 and 1 ret val scenarios");
				}
				DynValue item = m_ValueStack.Pop();
				callStackItem = PopToBasePointer();
				num = callStackItem.ReturnAddress;
				int num3 = (int)m_ValueStack.Pop().Number;
				m_ValueStack.RemoveLast(num3 + 1);
				m_ValueStack.Push(item);
				num = Internal_CheckForTailRequests(i, num);
			}
			if (callStackItem.Continuation != null)
			{
				m_ValueStack.Push(callStackItem.Continuation.Invoke(new ScriptExecutionContext(this, callStackItem.Continuation, i.SourceCodeRef), new DynValue[1] { m_ValueStack.Pop() }));
			}
			return num;
		}

		private int Internal_CheckForTailRequests(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Peek();
			if (dynValue.Type == DataType.TailCallRequest)
			{
				m_ValueStack.Pop();
				TailCallData tailCallData = dynValue.TailCallData;
				m_ValueStack.Push(tailCallData.Function);
				for (int j = 0; j < tailCallData.Args.Length; j++)
				{
					m_ValueStack.Push(tailCallData.Args[j]);
				}
				return Internal_ExecCall(tailCallData.Args.Length, instructionPtr, tailCallData.ErrorHandler, tailCallData.Continuation, false, null, tailCallData.ErrorHandlerBeforeUnwind);
			}
			if (dynValue.Type == DataType.YieldRequest)
			{
				m_SavedInstructionPtr = instructionPtr;
				return -99;
			}
			return instructionPtr;
		}

		private int JumpBool(Instruction i, bool expectedValueForJump, int instructionPtr)
		{
			if (m_ValueStack.Pop().ToScalar().CastToBool() == expectedValueForJump)
			{
				return i.NumVal;
			}
			return instructionPtr;
		}

		private int ExecShortCircuitingOperator(Instruction i, int instructionPtr)
		{
			bool flag = i.OpCode == OpCode.JtOrPop;
			if (m_ValueStack.Peek().ToScalar().CastToBool() == flag)
			{
				return i.NumVal;
			}
			m_ValueStack.Pop();
			return instructionPtr;
		}

		private int ExecAdd(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(num2.Value + num.Value));
				return instructionPtr;
			}
			int num3 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__add", instructionPtr);
			if (num3 >= 0)
			{
				return num3;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecSub(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(num2.Value - num.Value));
				return instructionPtr;
			}
			int num3 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__sub", instructionPtr);
			if (num3 >= 0)
			{
				return num3;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecMul(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(num2.Value * num.Value));
				return instructionPtr;
			}
			int num3 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__mul", instructionPtr);
			if (num3 >= 0)
			{
				return num3;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecMod(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				double num3 = Math.IEEERemainder(num2.Value, num.Value);
				if (num3 < 0.0)
				{
					num3 += num.Value;
				}
				m_ValueStack.Push(DynValue.NewNumber(num3));
				return instructionPtr;
			}
			int num4 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__mod", instructionPtr);
			if (num4 >= 0)
			{
				return num4;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecDiv(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(num2.Value / num.Value));
				return instructionPtr;
			}
			int num3 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__div", instructionPtr);
			if (num3 >= 0)
			{
				return num3;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecPower(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			double? num2 = dynValue2.CastToNumber();
			if (num2.HasValue && num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(Math.Pow(num2.Value, num.Value)));
				return instructionPtr;
			}
			int num3 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__pow", instructionPtr);
			if (num3 >= 0)
			{
				return num3;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue2, dynValue);
		}

		private int ExecNeg(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			double? num = dynValue.CastToNumber();
			if (num.HasValue)
			{
				m_ValueStack.Push(DynValue.NewNumber(0.0 - num.Value));
				return instructionPtr;
			}
			int num2 = Internal_InvokeUnaryMetaMethod(dynValue, "__unm", instructionPtr);
			if (num2 >= 0)
			{
				return num2;
			}
			throw ScriptRuntimeException.ArithmeticOnNonNumber(dynValue);
		}

		private int ExecEq(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			if (dynValue == dynValue2)
			{
				m_ValueStack.Push(DynValue.True);
				return instructionPtr;
			}
			if (dynValue2.Type == DataType.UserData || dynValue.Type == DataType.UserData)
			{
				int num = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__eq", instructionPtr);
				if (num >= 0)
				{
					return num;
				}
			}
			if (dynValue.Type != dynValue2.Type)
			{
				if ((dynValue2.Type == DataType.Nil && dynValue.Type == DataType.Void) || (dynValue2.Type == DataType.Void && dynValue.Type == DataType.Nil))
				{
					m_ValueStack.Push(DynValue.True);
				}
				else
				{
					m_ValueStack.Push(DynValue.False);
				}
				return instructionPtr;
			}
			if (dynValue2.Type == DataType.Table && GetMetatable(dynValue2) != null && GetMetatable(dynValue2) == GetMetatable(dynValue))
			{
				int num2 = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__eq", instructionPtr);
				if (num2 >= 0)
				{
					return num2;
				}
			}
			m_ValueStack.Push(DynValue.NewBoolean(dynValue.Equals(dynValue2)));
			return instructionPtr;
		}

		private int ExecLess(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			if (dynValue2.Type == DataType.Number && dynValue.Type == DataType.Number)
			{
				m_ValueStack.Push(DynValue.NewBoolean(dynValue2.Number < dynValue.Number));
			}
			else
			{
				if (dynValue2.Type != DataType.String || dynValue.Type != DataType.String)
				{
					int num = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__lt", instructionPtr);
					if (num < 0)
					{
						throw ScriptRuntimeException.CompareInvalidType(dynValue2, dynValue);
					}
					return num;
				}
				m_ValueStack.Push(DynValue.NewBoolean(dynValue2.String.CompareTo(dynValue.String) < 0));
			}
			return instructionPtr;
		}

		private int ExecLessEq(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			if (dynValue2.Type == DataType.Number && dynValue.Type == DataType.Number)
			{
				m_ValueStack.Push(DynValue.False);
				m_ValueStack.Push(DynValue.NewBoolean(dynValue2.Number <= dynValue.Number));
			}
			else
			{
				if (dynValue2.Type != DataType.String || dynValue.Type != DataType.String)
				{
					int num = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__le", instructionPtr, DynValue.False);
					if (num < 0)
					{
						num = Internal_InvokeBinaryMetaMethod(dynValue, dynValue2, "__lt", instructionPtr, DynValue.True);
						if (num < 0)
						{
							throw ScriptRuntimeException.CompareInvalidType(dynValue2, dynValue);
						}
						return num;
					}
					return num;
				}
				m_ValueStack.Push(DynValue.False);
				m_ValueStack.Push(DynValue.NewBoolean(dynValue2.String.CompareTo(dynValue.String) <= 0));
			}
			return instructionPtr;
		}

		private int ExecLen(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			if (dynValue.Type == DataType.String)
			{
				m_ValueStack.Push(DynValue.NewNumber(dynValue.String.Length));
			}
			else
			{
				int num = Internal_InvokeUnaryMetaMethod(dynValue, "__len", instructionPtr);
				if (num >= 0)
				{
					return num;
				}
				if (dynValue.Type != DataType.Table)
				{
					throw ScriptRuntimeException.LenOnInvalidType(dynValue);
				}
				m_ValueStack.Push(DynValue.NewNumber(dynValue.Table.Length));
			}
			return instructionPtr;
		}

		private int ExecConcat(Instruction i, int instructionPtr)
		{
			DynValue dynValue = m_ValueStack.Pop().ToScalar();
			DynValue dynValue2 = m_ValueStack.Pop().ToScalar();
			string text = dynValue.CastToString();
			string text2 = dynValue2.CastToString();
			if (text != null && text2 != null)
			{
				m_ValueStack.Push(DynValue.NewString(text2 + text));
				return instructionPtr;
			}
			int num = Internal_InvokeBinaryMetaMethod(dynValue2, dynValue, "__concat", instructionPtr);
			if (num >= 0)
			{
				return num;
			}
			throw ScriptRuntimeException.ConcatOnNonString(dynValue2, dynValue);
		}

		private void ExecTblInitI(Instruction i)
		{
			DynValue val = m_ValueStack.Pop();
			DynValue dynValue = m_ValueStack.Peek();
			if (dynValue.Type != DataType.Table)
			{
				throw new InternalErrorException("Unexpected type in table ctor : {0}", dynValue);
			}
			dynValue.Table.InitNextArrayKeys(val, i.NumVal != 0);
		}

		private void ExecTblInitN(Instruction i)
		{
			DynValue dynValue = m_ValueStack.Pop();
			DynValue key = m_ValueStack.Pop();
			DynValue dynValue2 = m_ValueStack.Peek();
			if (dynValue2.Type != DataType.Table)
			{
				throw new InternalErrorException("Unexpected type in table ctor : {0}", dynValue2);
			}
			dynValue2.Table.Set(key, dynValue.ToScalar());
		}

		private int ExecIndexSet(Instruction i, int instructionPtr)
		{
			int num = 100;
			bool isDirectIndexing = i.OpCode == OpCode.IndexSetN;
			bool flag = i.OpCode == OpCode.IndexSetL;
			DynValue dynValue = i.Value ?? m_ValueStack.Pop();
			DynValue dynValue2 = dynValue.ToScalar();
			DynValue dynValue3 = m_ValueStack.Pop().ToScalar();
			DynValue storeValue = GetStoreValue(i);
			DynValue dynValue4 = null;
			while (num > 0)
			{
				num--;
				if (dynValue3.Type == DataType.Table)
				{
					if (!flag && !dynValue3.Table.Get(dynValue2).IsNil())
					{
						dynValue3.Table.Set(dynValue2, storeValue);
						return instructionPtr;
					}
					dynValue4 = GetMetamethodRaw(dynValue3, "__newindex");
					if (dynValue4 == null || dynValue4.IsNil())
					{
						if (flag)
						{
							throw new ScriptRuntimeException("cannot multi-index a table. userdata expected");
						}
						dynValue3.Table.Set(dynValue2, storeValue);
						return instructionPtr;
					}
				}
				else
				{
					if (dynValue3.Type == DataType.UserData)
					{
						UserData userData = dynValue3.UserData;
						if (!userData.Descriptor.SetIndex(GetScript(), userData.Object, dynValue, storeValue, isDirectIndexing))
						{
							throw ScriptRuntimeException.UserDataMissingField(userData.Descriptor.Name, dynValue2.String);
						}
						return instructionPtr;
					}
					dynValue4 = GetMetamethodRaw(dynValue3, "__newindex");
					if (dynValue4 == null || dynValue4.IsNil())
					{
						throw ScriptRuntimeException.IndexType(dynValue3);
					}
				}
				if (dynValue4.Type == DataType.Function || dynValue4.Type == DataType.ClrFunction)
				{
					if (flag)
					{
						throw new ScriptRuntimeException("cannot multi-index through metamethods. userdata expected");
					}
					m_ValueStack.Pop();
					m_ValueStack.Push(dynValue4);
					m_ValueStack.Push(dynValue3);
					m_ValueStack.Push(dynValue2);
					m_ValueStack.Push(storeValue);
					return Internal_ExecCall(3, instructionPtr);
				}
				dynValue3 = dynValue4;
				dynValue4 = null;
			}
			throw ScriptRuntimeException.LoopInNewIndex();
		}

		private int ExecIndex(Instruction i, int instructionPtr)
		{
			int num = 100;
			bool isDirectIndexing = i.OpCode == OpCode.IndexN;
			bool flag = i.OpCode == OpCode.IndexL;
			DynValue dynValue = i.Value ?? m_ValueStack.Pop();
			DynValue dynValue2 = dynValue.ToScalar();
			DynValue dynValue3 = m_ValueStack.Pop().ToScalar();
			DynValue dynValue4 = null;
			while (num > 0)
			{
				num--;
				if (dynValue3.Type == DataType.Table)
				{
					if (!flag)
					{
						DynValue dynValue5 = dynValue3.Table.Get(dynValue2);
						if (!dynValue5.IsNil())
						{
							m_ValueStack.Push(dynValue5.AsReadOnly());
							return instructionPtr;
						}
					}
					dynValue4 = GetMetamethodRaw(dynValue3, "__index");
					if (dynValue4 == null || dynValue4.IsNil())
					{
						if (flag)
						{
							throw new ScriptRuntimeException("cannot multi-index a table. userdata expected");
						}
						m_ValueStack.Push(DynValue.Nil);
						return instructionPtr;
					}
				}
				else
				{
					if (dynValue3.Type == DataType.UserData)
					{
						UserData userData = dynValue3.UserData;
						DynValue dynValue6 = userData.Descriptor.Index(GetScript(), userData.Object, dynValue, isDirectIndexing);
						if (dynValue6 == null)
						{
							throw ScriptRuntimeException.UserDataMissingField(userData.Descriptor.Name, dynValue2.String);
						}
						m_ValueStack.Push(dynValue6.AsReadOnly());
						return instructionPtr;
					}
					dynValue4 = GetMetamethodRaw(dynValue3, "__index");
					if (dynValue4 == null || dynValue4.IsNil())
					{
						throw ScriptRuntimeException.IndexType(dynValue3);
					}
				}
				if (dynValue4.Type == DataType.Function || dynValue4.Type == DataType.ClrFunction)
				{
					if (flag)
					{
						throw new ScriptRuntimeException("cannot multi-index through metamethods. userdata expected");
					}
					m_ValueStack.Push(dynValue4);
					m_ValueStack.Push(dynValue3);
					m_ValueStack.Push(dynValue2);
					return Internal_ExecCall(2, instructionPtr);
				}
				dynValue3 = dynValue4;
				dynValue4 = null;
			}
			throw ScriptRuntimeException.LoopInIndex();
		}

		private void ClearBlockData(Instruction I)
		{
			int numVal = I.NumVal;
			int numVal2 = I.NumVal2;
			DynValue[] localScope = m_ExecutionStack.Peek().LocalScope;
			if (numVal2 >= 0 && numVal >= 0 && numVal2 >= numVal)
			{
				Array.Clear(localScope, numVal, numVal2 - numVal + 1);
			}
		}

		public DynValue GetGenericSymbol(SymbolRef symref)
		{
			switch (symref.i_Type)
			{
			case SymbolRefType.DefaultEnv:
				return DynValue.NewTable(GetScript().Globals);
			case SymbolRefType.Global:
				return GetGlobalSymbol(GetGenericSymbol(symref.i_Env), symref.i_Name);
			case SymbolRefType.Local:
				return GetTopNonClrFunction().LocalScope[symref.i_Index];
			case SymbolRefType.Upvalue:
				return GetTopNonClrFunction().ClosureScope[symref.i_Index];
			default:
				throw new InternalErrorException("Unexpected {0} LRef at resolution: {1}", symref.i_Type, symref.i_Name);
			}
		}

		private DynValue GetGlobalSymbol(DynValue dynValue, string name)
		{
			if (dynValue.Type != DataType.Table)
			{
				throw new InvalidOperationException(string.Format("_ENV is not a table but a {0}", dynValue.Type));
			}
			return dynValue.Table.Get(name);
		}

		private void SetGlobalSymbol(DynValue dynValue, string name, DynValue value)
		{
			if (dynValue.Type != DataType.Table)
			{
				throw new InvalidOperationException(string.Format("_ENV is not a table but a {0}", dynValue.Type));
			}
			dynValue.Table.Set(name, value ?? DynValue.Nil);
		}

		public void AssignGenericSymbol(SymbolRef symref, DynValue value)
		{
			switch (symref.i_Type)
			{
			case SymbolRefType.Global:
				SetGlobalSymbol(GetGenericSymbol(symref.i_Env), symref.i_Name, value);
				break;
			case SymbolRefType.Local:
			{
				CallStackItem topNonClrFunction2 = GetTopNonClrFunction();
				DynValue dynValue3 = topNonClrFunction2.LocalScope[symref.i_Index];
				if (dynValue3 == null)
				{
					dynValue3 = (topNonClrFunction2.LocalScope[symref.i_Index] = DynValue.NewNil());
				}
				dynValue3.Assign(value);
				break;
			}
			case SymbolRefType.Upvalue:
			{
				CallStackItem topNonClrFunction = GetTopNonClrFunction();
				DynValue dynValue = topNonClrFunction.ClosureScope[symref.i_Index];
				if (dynValue == null)
				{
					dynValue = (topNonClrFunction.ClosureScope[symref.i_Index] = DynValue.NewNil());
				}
				dynValue.Assign(value);
				break;
			}
			case SymbolRefType.DefaultEnv:
				throw new ArgumentException("Can't AssignGenericSymbol on a DefaultEnv symbol");
			default:
				throw new InternalErrorException("Unexpected {0} LRef at resolution: {1}", symref.i_Type, symref.i_Name);
			}
		}

		private CallStackItem GetTopNonClrFunction()
		{
			CallStackItem callStackItem = null;
			for (int i = 0; i < m_ExecutionStack.Count; i++)
			{
				callStackItem = m_ExecutionStack.Peek(i);
				if (callStackItem.ClrFunction == null)
				{
					break;
				}
			}
			return callStackItem;
		}

		public SymbolRef FindSymbolByName(string name)
		{
			if (m_ExecutionStack.Count > 0)
			{
				CallStackItem topNonClrFunction = GetTopNonClrFunction();
				if (topNonClrFunction != null)
				{
					if (topNonClrFunction.Debug_Symbols != null)
					{
						for (int num = topNonClrFunction.Debug_Symbols.Length - 1; num >= 0; num--)
						{
							SymbolRef symbolRef = topNonClrFunction.Debug_Symbols[num];
							if (symbolRef.i_Name == name && topNonClrFunction.LocalScope[num] != null)
							{
								return symbolRef;
							}
						}
					}
					ClosureContext closureScope = topNonClrFunction.ClosureScope;
					if (closureScope != null)
					{
						for (int i = 0; i < closureScope.Symbols.Length; i++)
						{
							if (closureScope.Symbols[i] == name)
							{
								return SymbolRef.Upvalue(name, i);
							}
						}
					}
				}
			}
			if (name != "_ENV")
			{
				SymbolRef envSymbol = FindSymbolByName("_ENV");
				return SymbolRef.Global(name, envSymbol);
			}
			return SymbolRef.DefaultEnv;
		}

		private DynValue[] Internal_AdjustTuple(IList<DynValue> values)
		{
			if (values == null || values.Count == 0)
			{
				return new DynValue[0];
			}
			if (values[values.Count - 1].Type == DataType.Tuple)
			{
				DynValue[] array = new DynValue[values.Count - 1 + values[values.Count - 1].Tuple.Length];
				for (int i = 0; i < values.Count - 1; i++)
				{
					array[i] = values[i].ToScalar();
				}
				for (int j = 0; j < values[values.Count - 1].Tuple.Length; j++)
				{
					array[values.Count + j - 1] = values[values.Count - 1].Tuple[j];
				}
				if (array[array.Length - 1].Type == DataType.Tuple)
				{
					return Internal_AdjustTuple(array);
				}
				return array;
			}
			DynValue[] array2 = new DynValue[values.Count];
			for (int k = 0; k < values.Count; k++)
			{
				array2[k] = values[k].ToScalar();
			}
			return array2;
		}

		private int Internal_InvokeUnaryMetaMethod(DynValue op1, string eventName, int instructionPtr)
		{
			DynValue dynValue = null;
			if (op1.Type == DataType.UserData)
			{
				dynValue = op1.UserData.Descriptor.MetaIndex(m_Script, op1.UserData.Object, eventName);
			}
			if (dynValue == null)
			{
				Table metatable = GetMetatable(op1);
				if (metatable != null)
				{
					DynValue dynValue2 = metatable.RawGet(eventName);
					if (dynValue2 != null && dynValue2.IsNotNil())
					{
						dynValue = dynValue2;
					}
				}
			}
			if (dynValue != null)
			{
				m_ValueStack.Push(dynValue);
				m_ValueStack.Push(op1);
				return Internal_ExecCall(1, instructionPtr);
			}
			return -1;
		}

		private int Internal_InvokeBinaryMetaMethod(DynValue l, DynValue r, string eventName, int instructionPtr, DynValue extraPush = null)
		{
			DynValue binaryMetamethod = GetBinaryMetamethod(l, r, eventName);
			if (binaryMetamethod != null)
			{
				if (extraPush != null)
				{
					m_ValueStack.Push(extraPush);
				}
				m_ValueStack.Push(binaryMetamethod);
				m_ValueStack.Push(l);
				m_ValueStack.Push(r);
				return Internal_ExecCall(2, instructionPtr);
			}
			return -1;
		}

		private DynValue[] StackTopToArray(int items, bool pop)
		{
			DynValue[] array = new DynValue[items];
			if (pop)
			{
				for (int i = 0; i < items; i++)
				{
					array[i] = m_ValueStack.Pop();
				}
			}
			else
			{
				for (int j = 0; j < items; j++)
				{
					array[j] = m_ValueStack[m_ValueStack.Count - 1 - j];
				}
			}
			return array;
		}

		private DynValue[] StackTopToArrayReverse(int items, bool pop)
		{
			DynValue[] array = new DynValue[items];
			if (pop)
			{
				for (int i = 0; i < items; i++)
				{
					array[items - 1 - i] = m_ValueStack.Pop();
				}
			}
			else
			{
				for (int j = 0; j < items; j++)
				{
					array[items - 1 - j] = m_ValueStack[m_ValueStack.Count - 1 - j];
				}
			}
			return array;
		}

		[CompilerGenerated]
		private DynValue _003CExecClosure_003Eb__68_0(SymbolRef s)
		{
			return GetUpvalueSymbol(s);
		}
	}
}
