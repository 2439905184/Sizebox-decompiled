using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class LabelStatement : Statement
	{
		private List<GotoStatement> m_Gotos = new List<GotoStatement>();

		private RuntimeScopeBlock m_StackFrame;

		public string Label { get; private set; }

		public int Address { get; private set; }

		public SourceRef SourceRef { get; private set; }

		public Token NameToken { get; private set; }

		internal int DefinedVarsCount { get; private set; }

		internal string LastDefinedVarName { get; private set; }

		public LabelStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			NodeBase.CheckTokenType(lcontext, TokenType.DoubleColon);
			NameToken = NodeBase.CheckTokenType(lcontext, TokenType.Name);
			NodeBase.CheckTokenType(lcontext, TokenType.DoubleColon);
			SourceRef = NameToken.GetSourceRef();
			Label = NameToken.Text;
			lcontext.Scope.DefineLabel(this);
		}

		internal void SetDefinedVars(int definedVarsCount, string lastDefinedVarsName)
		{
			DefinedVarsCount = definedVarsCount;
			LastDefinedVarName = lastDefinedVarsName;
		}

		internal void RegisterGoto(GotoStatement gotostat)
		{
			m_Gotos.Add(gotostat);
		}

		public override void Compile(ByteCode bc)
		{
			bc.Emit_Clean(m_StackFrame);
			Address = bc.GetJumpPointForLastInstruction();
			foreach (GotoStatement @goto in m_Gotos)
			{
				@goto.SetAddress(Address);
			}
		}

		internal void SetScope(RuntimeScopeBlock runtimeScopeBlock)
		{
			m_StackFrame = runtimeScopeBlock;
		}
	}
}
