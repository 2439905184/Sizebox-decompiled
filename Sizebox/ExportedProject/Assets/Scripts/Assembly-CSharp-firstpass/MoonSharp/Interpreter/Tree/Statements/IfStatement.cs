using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class IfStatement : Statement
	{
		private class IfBlock
		{
			public Expression Exp;

			public Statement Block;

			public RuntimeScopeBlock StackFrame;

			public SourceRef Source;
		}

		private List<IfBlock> m_Ifs = new List<IfBlock>();

		private IfBlock m_Else;

		private SourceRef m_End;

		public IfStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			while (lcontext.Lexer.Current.Type != TokenType.Else && lcontext.Lexer.Current.Type != TokenType.End)
			{
				m_Ifs.Add(CreateIfBlock(lcontext));
			}
			if (lcontext.Lexer.Current.Type == TokenType.Else)
			{
				m_Else = CreateElseBlock(lcontext);
			}
			m_End = NodeBase.CheckTokenType(lcontext, TokenType.End).GetSourceRef();
			lcontext.Source.Refs.Add(m_End);
		}

		private IfBlock CreateIfBlock(ScriptLoadingContext lcontext)
		{
			Token token = NodeBase.CheckTokenType(lcontext, TokenType.If, TokenType.ElseIf);
			lcontext.Scope.PushBlock();
			IfBlock ifBlock = new IfBlock();
			ifBlock.Exp = Expression.Expr(lcontext);
			ifBlock.Source = token.GetSourceRef(NodeBase.CheckTokenType(lcontext, TokenType.Then));
			ifBlock.Block = new CompositeStatement(lcontext);
			ifBlock.StackFrame = lcontext.Scope.PopBlock();
			lcontext.Source.Refs.Add(ifBlock.Source);
			return ifBlock;
		}

		private IfBlock CreateElseBlock(ScriptLoadingContext lcontext)
		{
			Token token = NodeBase.CheckTokenType(lcontext, TokenType.Else);
			lcontext.Scope.PushBlock();
			IfBlock ifBlock = new IfBlock();
			ifBlock.Block = new CompositeStatement(lcontext);
			ifBlock.StackFrame = lcontext.Scope.PopBlock();
			ifBlock.Source = token.GetSourceRef();
			lcontext.Source.Refs.Add(ifBlock.Source);
			return ifBlock;
		}

		public override void Compile(ByteCode bc)
		{
			List<Instruction> list = new List<Instruction>();
			Instruction instruction = null;
			foreach (IfBlock @if in m_Ifs)
			{
				using (bc.EnterSource(@if.Source))
				{
					if (instruction != null)
					{
						instruction.NumVal = bc.GetJumpPointForNextInstruction();
					}
					@if.Exp.Compile(bc);
					instruction = bc.Emit_Jump(OpCode.Jf, -1);
					bc.Emit_Enter(@if.StackFrame);
					@if.Block.Compile(bc);
				}
				using (bc.EnterSource(m_End))
				{
					bc.Emit_Leave(@if.StackFrame);
				}
				list.Add(bc.Emit_Jump(OpCode.Jump, -1));
			}
			instruction.NumVal = bc.GetJumpPointForNextInstruction();
			if (m_Else != null)
			{
				using (bc.EnterSource(m_Else.Source))
				{
					bc.Emit_Enter(m_Else.StackFrame);
					m_Else.Block.Compile(bc);
				}
				using (bc.EnterSource(m_End))
				{
					bc.Emit_Leave(m_Else.StackFrame);
				}
			}
			foreach (Instruction item in list)
			{
				item.NumVal = bc.GetJumpPointForNextInstruction();
			}
		}
	}
}
