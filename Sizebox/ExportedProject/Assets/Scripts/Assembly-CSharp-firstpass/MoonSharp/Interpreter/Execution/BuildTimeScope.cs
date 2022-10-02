using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter.Execution.Scopes;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Execution
{
	internal class BuildTimeScope
	{
		private List<BuildTimeScopeFrame> m_Frames = new List<BuildTimeScopeFrame>();

		private List<IClosureBuilder> m_ClosureBuilders = new List<IClosureBuilder>();

		public void PushFunction(IClosureBuilder closureBuilder, bool hasVarArgs)
		{
			m_ClosureBuilders.Add(closureBuilder);
			m_Frames.Add(new BuildTimeScopeFrame(hasVarArgs));
		}

		public void PushBlock()
		{
			m_Frames.Last().PushBlock();
		}

		public RuntimeScopeBlock PopBlock()
		{
			return m_Frames.Last().PopBlock();
		}

		public RuntimeScopeFrame PopFunction()
		{
			BuildTimeScopeFrame buildTimeScopeFrame = m_Frames.Last();
			buildTimeScopeFrame.ResolveLRefs();
			m_Frames.RemoveAt(m_Frames.Count - 1);
			m_ClosureBuilders.RemoveAt(m_ClosureBuilders.Count - 1);
			return buildTimeScopeFrame.GetRuntimeFrameData();
		}

		public SymbolRef Find(string name)
		{
			SymbolRef symbolRef = m_Frames.Last().Find(name);
			if (symbolRef != null)
			{
				return symbolRef;
			}
			for (int num = m_Frames.Count - 2; num >= 0; num--)
			{
				SymbolRef symbolRef2 = m_Frames[num].Find(name);
				if (symbolRef2 != null)
				{
					symbolRef2 = CreateUpValue(this, symbolRef2, num, m_Frames.Count - 2);
					if (symbolRef2 != null)
					{
						return symbolRef2;
					}
				}
			}
			return CreateGlobalReference(name);
		}

		public SymbolRef CreateGlobalReference(string name)
		{
			if (name == "_ENV")
			{
				throw new InternalErrorException("_ENV passed in CreateGlobalReference");
			}
			SymbolRef envSymbol = Find("_ENV");
			return SymbolRef.Global(name, envSymbol);
		}

		public void ForceEnvUpValue()
		{
			Find("_ENV");
		}

		private SymbolRef CreateUpValue(BuildTimeScope buildTimeScope, SymbolRef symb, int closuredFrame, int currentFrame)
		{
			if (closuredFrame == currentFrame)
			{
				return m_ClosureBuilders[currentFrame + 1].CreateUpvalue(this, symb);
			}
			SymbolRef symbol = CreateUpValue(buildTimeScope, symb, closuredFrame, currentFrame - 1);
			return m_ClosureBuilders[currentFrame + 1].CreateUpvalue(this, symbol);
		}

		public SymbolRef DefineLocal(string name)
		{
			return m_Frames.Last().DefineLocal(name);
		}

		public SymbolRef TryDefineLocal(string name)
		{
			return m_Frames.Last().TryDefineLocal(name);
		}

		public bool CurrentFunctionHasVarArgs()
		{
			return m_Frames.Last().HasVarArgs;
		}

		internal void DefineLabel(LabelStatement label)
		{
			m_Frames.Last().DefineLabel(label);
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			m_Frames.Last().RegisterGoto(gotostat);
		}
	}
}
