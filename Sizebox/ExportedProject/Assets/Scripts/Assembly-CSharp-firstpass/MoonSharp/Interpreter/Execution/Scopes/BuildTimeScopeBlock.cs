using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Execution.Scopes
{
	internal class BuildTimeScopeBlock
	{
		private Dictionary<string, SymbolRef> m_DefinedNames = new Dictionary<string, SymbolRef>();

		private List<GotoStatement> m_PendingGotos;

		private Dictionary<string, LabelStatement> m_LocalLabels;

		private string m_LastDefinedName;

		internal BuildTimeScopeBlock Parent { get; private set; }

		internal List<BuildTimeScopeBlock> ChildNodes { get; private set; }

		internal RuntimeScopeBlock ScopeBlock { get; private set; }

		internal void Rename(string name)
		{
			SymbolRef value = m_DefinedNames[name];
			m_DefinedNames.Remove(name);
			m_DefinedNames.Add(string.Format("@{0}_{1}", name, Guid.NewGuid().ToString("N")), value);
		}

		internal BuildTimeScopeBlock(BuildTimeScopeBlock parent)
		{
			Parent = parent;
			ChildNodes = new List<BuildTimeScopeBlock>();
			ScopeBlock = new RuntimeScopeBlock();
		}

		internal BuildTimeScopeBlock AddChild()
		{
			BuildTimeScopeBlock buildTimeScopeBlock = new BuildTimeScopeBlock(this);
			ChildNodes.Add(buildTimeScopeBlock);
			return buildTimeScopeBlock;
		}

		internal SymbolRef Find(string name)
		{
			return m_DefinedNames.GetOrDefault(name);
		}

		internal SymbolRef Define(string name)
		{
			SymbolRef symbolRef = SymbolRef.Local(name, -1);
			m_DefinedNames.Add(name, symbolRef);
			m_LastDefinedName = name;
			return symbolRef;
		}

		internal int ResolveLRefs(BuildTimeScopeFrame buildTimeScopeFrame)
		{
			int num = -1;
			int num2 = -1;
			foreach (SymbolRef value in m_DefinedNames.Values)
			{
				int num3 = buildTimeScopeFrame.AllocVar(value);
				if (num < 0)
				{
					num = num3;
				}
				num2 = num3;
			}
			ScopeBlock.From = num;
			RuntimeScopeBlock scopeBlock = ScopeBlock;
			int toInclusive = (ScopeBlock.To = num2);
			scopeBlock.ToInclusive = toInclusive;
			if (num < 0)
			{
				ScopeBlock.From = buildTimeScopeFrame.GetPosForNextVar();
			}
			foreach (BuildTimeScopeBlock childNode in ChildNodes)
			{
				ScopeBlock.ToInclusive = Math.Max(ScopeBlock.ToInclusive, childNode.ResolveLRefs(buildTimeScopeFrame));
			}
			if (m_LocalLabels != null)
			{
				foreach (LabelStatement value2 in m_LocalLabels.Values)
				{
					value2.SetScope(ScopeBlock);
				}
			}
			return ScopeBlock.ToInclusive;
		}

		internal void DefineLabel(LabelStatement label)
		{
			if (m_LocalLabels == null)
			{
				m_LocalLabels = new Dictionary<string, LabelStatement>();
			}
			if (m_LocalLabels.ContainsKey(label.Label))
			{
				throw new SyntaxErrorException(label.NameToken, "label '{0}' already defined on line {1}", label.Label, m_LocalLabels[label.Label].SourceRef.FromLine);
			}
			m_LocalLabels.Add(label.Label, label);
			label.SetDefinedVars(m_DefinedNames.Count, m_LastDefinedName);
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			if (m_PendingGotos == null)
			{
				m_PendingGotos = new List<GotoStatement>();
			}
			m_PendingGotos.Add(gotostat);
			gotostat.SetDefinedVars(m_DefinedNames.Count, m_LastDefinedName);
		}

		internal void ResolveGotos()
		{
			if (m_PendingGotos == null)
			{
				return;
			}
			foreach (GotoStatement pendingGoto in m_PendingGotos)
			{
				LabelStatement value;
				if (m_LocalLabels != null && m_LocalLabels.TryGetValue(pendingGoto.Label, out value))
				{
					if (value.DefinedVarsCount > pendingGoto.DefinedVarsCount)
					{
						throw new SyntaxErrorException(pendingGoto.GotoToken, "<goto {0}> at line {1} jumps into the scope of local '{2}'", pendingGoto.Label, pendingGoto.GotoToken.FromLine, value.LastDefinedVarName);
					}
					value.RegisterGoto(pendingGoto);
				}
				else
				{
					if (Parent == null)
					{
						throw new SyntaxErrorException(pendingGoto.GotoToken, "no visible label '{0}' for <goto> at line {1}", pendingGoto.Label, pendingGoto.GotoToken.FromLine);
					}
					Parent.RegisterGoto(pendingGoto);
				}
			}
			m_PendingGotos.Clear();
		}
	}
}
