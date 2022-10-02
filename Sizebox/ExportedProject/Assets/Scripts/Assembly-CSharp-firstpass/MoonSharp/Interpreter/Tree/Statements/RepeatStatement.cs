using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class RepeatStatement : Statement
	{
		private Expression m_Condition;

		private Statement m_Block;

		private RuntimeScopeBlock m_StackFrame;

		private SourceRef m_Repeat;

		private SourceRef m_Until;

		public RepeatStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			m_Repeat = NodeBase.CheckTokenType(lcontext, TokenType.Repeat).GetSourceRef();
			lcontext.Scope.PushBlock();
			m_Block = new CompositeStatement(lcontext);
			Token token = NodeBase.CheckTokenType(lcontext, TokenType.Until);
			m_Condition = Expression.Expr(lcontext);
			m_Until = token.GetSourceRefUpTo(lcontext.Lexer.Current);
			m_StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(m_Repeat);
			lcontext.Source.Refs.Add(m_Until);
		}

		public override void Compile(ByteCode bc)
		{
			Loop loop = new Loop
			{
				Scope = m_StackFrame
			};
			bc.PushSourceRef(m_Repeat);
			bc.LoopTracker.Loops.Push(loop);
			int jumpPointForNextInstruction = bc.GetJumpPointForNextInstruction();
			bc.Emit_Enter(m_StackFrame);
			m_Block.Compile(bc);
			bc.PopSourceRef();
			bc.PushSourceRef(m_Until);
			m_Condition.Compile(bc);
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Jump(OpCode.Jf, jumpPointForNextInstruction);
			bc.LoopTracker.Loops.Pop();
			int jumpPointForNextInstruction2 = bc.GetJumpPointForNextInstruction();
			foreach (Instruction breakJump in loop.BreakJumps)
			{
				breakJump.NumVal = jumpPointForNextInstruction2;
			}
			bc.PopSourceRef();
		}
	}
}
