using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Execution.Scopes
{
	internal class BuildTimeScopeFrame
	{
		private BuildTimeScopeBlock m_ScopeTreeRoot;

		private BuildTimeScopeBlock m_ScopeTreeHead;

		private RuntimeScopeFrame m_ScopeFrame = new RuntimeScopeFrame();

		public bool HasVarArgs { get; private set; }

		internal BuildTimeScopeFrame(bool hasVarArgs)
		{
			HasVarArgs = hasVarArgs;
			m_ScopeTreeHead = (m_ScopeTreeRoot = new BuildTimeScopeBlock(null));
		}

		internal void PushBlock()
		{
			m_ScopeTreeHead = m_ScopeTreeHead.AddChild();
		}

		internal RuntimeScopeBlock PopBlock()
		{
			BuildTimeScopeBlock scopeTreeHead = m_ScopeTreeHead;
			m_ScopeTreeHead.ResolveGotos();
			m_ScopeTreeHead = m_ScopeTreeHead.Parent;
			if (m_ScopeTreeHead == null)
			{
				throw new InternalErrorException("Can't pop block - stack underflow");
			}
			return scopeTreeHead.ScopeBlock;
		}

		internal RuntimeScopeFrame GetRuntimeFrameData()
		{
			if (m_ScopeTreeHead != m_ScopeTreeRoot)
			{
				throw new InternalErrorException("Misaligned scope frames/blocks!");
			}
			m_ScopeFrame.ToFirstBlock = m_ScopeTreeRoot.ScopeBlock.To;
			return m_ScopeFrame;
		}

		internal SymbolRef Find(string name)
		{
			for (BuildTimeScopeBlock buildTimeScopeBlock = m_ScopeTreeHead; buildTimeScopeBlock != null; buildTimeScopeBlock = buildTimeScopeBlock.Parent)
			{
				SymbolRef symbolRef = buildTimeScopeBlock.Find(name);
				if (symbolRef != null)
				{
					return symbolRef;
				}
			}
			return null;
		}

		internal SymbolRef DefineLocal(string name)
		{
			return m_ScopeTreeHead.Define(name);
		}

		internal SymbolRef TryDefineLocal(string name)
		{
			if (m_ScopeTreeHead.Find(name) != null)
			{
				m_ScopeTreeHead.Rename(name);
			}
			return m_ScopeTreeHead.Define(name);
		}

		internal void ResolveLRefs()
		{
			m_ScopeTreeRoot.ResolveGotos();
			m_ScopeTreeRoot.ResolveLRefs(this);
		}

		internal int AllocVar(SymbolRef var)
		{
			var.i_Index = m_ScopeFrame.DebugSymbols.Count;
			m_ScopeFrame.DebugSymbols.Add(var);
			return var.i_Index;
		}

		internal int GetPosForNextVar()
		{
			return m_ScopeFrame.DebugSymbols.Count;
		}

		internal void DefineLabel(LabelStatement label)
		{
			m_ScopeTreeHead.DefineLabel(label);
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			m_ScopeTreeHead.RegisterGoto(gotostat);
		}
	}
}
