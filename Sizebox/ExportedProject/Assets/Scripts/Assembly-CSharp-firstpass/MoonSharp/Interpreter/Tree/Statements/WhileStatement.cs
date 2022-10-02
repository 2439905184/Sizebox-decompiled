using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class WhileStatement : Statement
	{
		private Expression m_Condition;

		private Statement m_Block;

		private RuntimeScopeBlock m_StackFrame;

		private SourceRef m_Start;

		private SourceRef m_End;

		public WhileStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			Token token = NodeBase.CheckTokenType(lcontext, TokenType.While);
			m_Condition = Expression.Expr(lcontext);
			m_Start = token.GetSourceRefUpTo(lcontext.Lexer.Current);
			lcontext.Scope.PushBlock();
			NodeBase.CheckTokenType(lcontext, TokenType.Do);
			m_Block = new CompositeStatement(lcontext);
			m_End = NodeBase.CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(m_Start);
			lcontext.Source.Refs.Add(m_End);
		}

		public override void Compile(ByteCode bc)
		{
			Loop loop = new Loop
			{
				Scope = m_StackFrame
			};
			bc.LoopTracker.Loops.Push(loop);
			bc.PushSourceRef(m_Start);
			int jumpPointForNextInstruction = bc.GetJumpPointForNextInstruction();
			m_Condition.Compile(bc);
			Instruction instruction = bc.Emit_Jump(OpCode.Jf, -1);
			bc.Emit_Enter(m_StackFrame);
			m_Block.Compile(bc);
			bc.PopSourceRef();
			bc.PushSourceRef(m_End);
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Jump(OpCode.Jump, jumpPointForNextInstruction);
			bc.LoopTracker.Loops.Pop();
			int jumpPointForNextInstruction2 = bc.GetJumpPointForNextInstruction();
			foreach (Instruction breakJump in loop.BreakJumps)
			{
				breakJump.NumVal = jumpPointForNextInstruction2;
			}
			instruction.NumVal = jumpPointForNextInstruction2;
			bc.PopSourceRef();
		}
	}
}
