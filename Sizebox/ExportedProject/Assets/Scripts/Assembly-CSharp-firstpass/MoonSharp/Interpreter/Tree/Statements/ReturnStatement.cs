using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class ReturnStatement : Statement
	{
		private Expression m_Expression;

		private SourceRef m_Ref;

		public ReturnStatement(ScriptLoadingContext lcontext, Expression e, SourceRef sref)
			: base(lcontext)
		{
			m_Expression = e;
			m_Ref = sref;
			lcontext.Source.Refs.Add(sref);
		}

		public ReturnStatement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			Token current = lcontext.Lexer.Current;
			lcontext.Lexer.Next();
			Token current2 = lcontext.Lexer.Current;
			if (current2.IsEndOfBlock() || current2.Type == TokenType.SemiColon)
			{
				m_Expression = null;
				m_Ref = current.GetSourceRef();
			}
			else
			{
				m_Expression = new ExprListExpression(Expression.ExprList(lcontext), lcontext);
				m_Ref = current.GetSourceRefUpTo(lcontext.Lexer.Current);
			}
			lcontext.Source.Refs.Add(m_Ref);
		}

		public override void Compile(ByteCode bc)
		{
			using (bc.EnterSource(m_Ref))
			{
				if (m_Expression != null)
				{
					m_Expression.Compile(bc);
					bc.Emit_Ret(1);
				}
				else
				{
					bc.Emit_Ret(0);
				}
			}
		}
	}
}
