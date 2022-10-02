using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class ScopeBlockStatement : Statement
	{
		private Statement m_Block;

		private RuntimeScopeBlock m_StackFrame;

		private SourceRef m_Do;

		private SourceRef m_End;

		public ScopeBlockStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			lcontext.Scope.PushBlock();
			m_Do = NodeBase.CheckTokenType(lcontext, TokenType.Do).GetSourceRef();
			m_Block = new CompositeStatement(lcontext);
			m_End = NodeBase.CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(m_Do);
			lcontext.Source.Refs.Add(m_End);
		}

		public override void Compile(ByteCode bc)
		{
			using (bc.EnterSource(m_Do))
			{
				bc.Emit_Enter(m_StackFrame);
			}
			m_Block.Compile(bc);
			using (bc.EnterSource(m_End))
			{
				bc.Emit_Leave(m_StackFrame);
			}
		}
	}
}
