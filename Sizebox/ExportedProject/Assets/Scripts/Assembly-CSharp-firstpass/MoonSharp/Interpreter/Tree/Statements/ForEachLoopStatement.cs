using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class ForEachLoopStatement : Statement
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{
			public ScriptLoadingContext lcontext;

			internal SymbolRef _003C_002Ector_003Eb__0(string n)
			{
				return lcontext.Scope.TryDefineLocal(n);
			}

			internal SymbolRefExpression _003C_002Ector_003Eb__1(SymbolRef s)
			{
				return new SymbolRefExpression(lcontext, s);
			}
		}

		private RuntimeScopeBlock m_StackFrame;

		private SymbolRef[] m_Names;

		private IVariable[] m_NameExps;

		private Expression m_RValues;

		private Statement m_Block;

		private SourceRef m_RefFor;

		private SourceRef m_RefEnd;

		public ForEachLoopStatement(ScriptLoadingContext lcontext, Token firstNameToken, Token forToken)
		{
			_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
			_003C_003Ec__DisplayClass7_.lcontext = lcontext;
			base._002Ector(_003C_003Ec__DisplayClass7_.lcontext);
			List<string> list = new List<string> { firstNameToken.Text };
			while (_003C_003Ec__DisplayClass7_.lcontext.Lexer.Current.Type == TokenType.Comma)
			{
				_003C_003Ec__DisplayClass7_.lcontext.Lexer.Next();
				Token token = NodeBase.CheckTokenType(_003C_003Ec__DisplayClass7_.lcontext, TokenType.Name);
				list.Add(token.Text);
			}
			NodeBase.CheckTokenType(_003C_003Ec__DisplayClass7_.lcontext, TokenType.In);
			m_RValues = new ExprListExpression(Expression.ExprList(_003C_003Ec__DisplayClass7_.lcontext), _003C_003Ec__DisplayClass7_.lcontext);
			_003C_003Ec__DisplayClass7_.lcontext.Scope.PushBlock();
			m_Names = list.Select(_003C_003Ec__DisplayClass7_._003C_002Ector_003Eb__0).ToArray();
			m_NameExps = m_Names.Select(_003C_003Ec__DisplayClass7_._003C_002Ector_003Eb__1).Cast<IVariable>().ToArray();
			m_RefFor = forToken.GetSourceRef(NodeBase.CheckTokenType(_003C_003Ec__DisplayClass7_.lcontext, TokenType.Do));
			m_Block = new CompositeStatement(_003C_003Ec__DisplayClass7_.lcontext);
			m_RefEnd = NodeBase.CheckTokenType(_003C_003Ec__DisplayClass7_.lcontext, TokenType.End).GetSourceRef();
			m_StackFrame = _003C_003Ec__DisplayClass7_.lcontext.Scope.PopBlock();
			_003C_003Ec__DisplayClass7_.lcontext.Source.Refs.Add(m_RefFor);
			_003C_003Ec__DisplayClass7_.lcontext.Source.Refs.Add(m_RefEnd);
		}

		public override void Compile(ByteCode bc)
		{
			bc.PushSourceRef(m_RefFor);
			Loop loop = new Loop
			{
				Scope = m_StackFrame
			};
			bc.LoopTracker.Loops.Push(loop);
			m_RValues.Compile(bc);
			bc.Emit_IterPrep();
			int jumpPointForNextInstruction = bc.GetJumpPointForNextInstruction();
			bc.Emit_Enter(m_StackFrame);
			bc.Emit_ExpTuple(0);
			bc.Emit_Call(2, "for..in");
			for (int i = 0; i < m_NameExps.Length; i++)
			{
				m_NameExps[i].CompileAssignment(bc, 0, i);
			}
			bc.Emit_Pop();
			bc.Emit_Load(m_Names[0]);
			bc.Emit_IterUpd();
			Instruction instruction = bc.Emit_Jump(OpCode.JNil, -1);
			m_Block.Compile(bc);
			bc.PopSourceRef();
			bc.PushSourceRef(m_RefEnd);
			bc.Emit_Leave(m_StackFrame);
			bc.Emit_Jump(OpCode.Jump, jumpPointForNextInstruction);
			bc.LoopTracker.Loops.Pop();
			int jumpPointForNextInstruction2 = bc.GetJumpPointForNextInstruction();
			bc.Emit_Leave(m_StackFrame);
			int jumpPointForNextInstruction3 = bc.GetJumpPointForNextInstruction();
			bc.Emit_Pop();
			foreach (Instruction breakJump in loop.BreakJumps)
			{
				breakJump.NumVal = jumpPointForNextInstruction3;
			}
			instruction.NumVal = jumpPointForNextInstruction2;
			bc.PopSourceRef();
		}
	}
}
