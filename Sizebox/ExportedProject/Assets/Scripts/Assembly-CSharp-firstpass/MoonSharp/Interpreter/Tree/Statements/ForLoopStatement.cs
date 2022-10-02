using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class ForLoopStatement : Statement
	{
		private RuntimeScopeBlock m_StackFrame;

		private Statement m_InnerBlock;

		private SymbolRef m_VarName;

		private Expression m_Start;

		private Expression m_End;

		private Expression m_Step;

		private SourceRef m_RefFor;

		private SourceRef m_RefEnd;

		public ForLoopStatement(ScriptLoadingContext lcontext, Token nameToken, Token forToken)
			: base(lcontext)
		{
			NodeBase.CheckTokenType(lcontext, TokenType.Op_Assignment);
			m_Start = Expression.Expr(lcontext);
			NodeBase.CheckTokenType(lcontext, TokenType.Comma);
			m_End = Expression.Expr(lcontext);
			if (lcontext.Lexer.Current.Type == TokenType.Comma)
			{
				lcontext.Lexer.Next();
				m_Step = Expression.Expr(lcontext);
			}
			else
			{
				m_Step = new LiteralExpression(lcontext, DynValue.NewNumber(1.0));
			}
			lcontext.Scope.PushBlock();
			m_VarName = lcontext.Scope.DefineLocal(nameToken.Text);
			m_RefFor = forToken.GetSourceRef(NodeBase.CheckTokenType(lcontext, TokenType.Do));
			m_InnerBlock = new CompositeStatement(lcontext);
			m_RefEnd = NodeBase.CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(m_RefFor);
			lcontext.Source.Refs.Add(m_RefEnd);
		}

		public override void Compile(ByteCode bc)
		{
			bc.PushSourceRef(m_RefFor);
			Loop loop = new Loop
			{
				Scope = m_StackFrame
			};
			bc.LoopTracker.Loops.Push(loop);
			m_End.Compile(bc);
			bc.Emit_ToNum(3);
			m_Step.Compile(bc);
			bc.Emit_ToNum(2);
			m_Start.Compile(bc);
			bc.Emit_ToNum(1);
			int jumpPointForNextInstruction = bc.GetJumpPointForNextInstruction();
			Instruction instruction = bc.Emit_Jump(OpCode.JFor, -1);
			bc.Emit_Enter(m_StackFrame);
			bc.Emit_Store(m_VarName, 0, 0);
			m_InnerBlock.Compile(bc);
			bc.PopSourceRef();
			bc.PushSourceRef(m_RefEnd);
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Incr(1);
			bc.Emit_Jump(OpCode.Jump, jumpPointForNextInstruction);
			bc.LoopTracker.Loops.Pop();
			int jumpPointForNextInstruction2 = bc.GetJumpPointForNextInstruction();
			foreach (Instruction breakJump in loop.BreakJumps)
			{
				breakJump.NumVal = jumpPointForNextInstruction2;
			}
			instruction.NumVal = jumpPointForNextInstruction2;
			bc.Emit_Pop(3);
			bc.PopSourceRef();
		}
	}
}
