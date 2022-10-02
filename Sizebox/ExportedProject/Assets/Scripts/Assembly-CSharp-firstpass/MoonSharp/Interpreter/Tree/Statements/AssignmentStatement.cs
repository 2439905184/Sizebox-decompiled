using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree.Statements
{
	internal class AssignmentStatement : Statement
	{
		private List<IVariable> m_LValues = new List<IVariable>();

		private List<Expression> m_RValues;

		private SourceRef m_Ref;

		public AssignmentStatement(ScriptLoadingContext lcontext, Token startToken)
			: base(lcontext)
		{
			List<string> list = new List<string>();
			while (true)
			{
				Token token = NodeBase.CheckTokenType(lcontext, TokenType.Name);
				list.Add(token.Text);
				if (lcontext.Lexer.Current.Type != TokenType.Comma)
				{
					break;
				}
				lcontext.Lexer.Next();
			}
			if (lcontext.Lexer.Current.Type == TokenType.Op_Assignment)
			{
				NodeBase.CheckTokenType(lcontext, TokenType.Op_Assignment);
				m_RValues = Expression.ExprList(lcontext);
			}
			else
			{
				m_RValues = new List<Expression>();
			}
			foreach (string item2 in list)
			{
				SymbolRef refr = lcontext.Scope.TryDefineLocal(item2);
				SymbolRefExpression item = new SymbolRefExpression(lcontext, refr);
				m_LValues.Add(item);
			}
			Token current2 = lcontext.Lexer.Current;
			m_Ref = startToken.GetSourceRefUpTo(current2);
			lcontext.Source.Refs.Add(m_Ref);
		}

		public AssignmentStatement(ScriptLoadingContext lcontext, Expression firstExpression, Token first)
			: base(lcontext)
		{
			m_LValues.Add(CheckVar(lcontext, firstExpression));
			while (lcontext.Lexer.Current.Type == TokenType.Comma)
			{
				lcontext.Lexer.Next();
				Expression firstExpression2 = Expression.PrimaryExp(lcontext);
				m_LValues.Add(CheckVar(lcontext, firstExpression2));
			}
			NodeBase.CheckTokenType(lcontext, TokenType.Op_Assignment);
			m_RValues = Expression.ExprList(lcontext);
			Token current = lcontext.Lexer.Current;
			m_Ref = first.GetSourceRefUpTo(current);
			lcontext.Source.Refs.Add(m_Ref);
		}

		private IVariable CheckVar(ScriptLoadingContext lcontext, Expression firstExpression)
		{
			IVariable variable = firstExpression as IVariable;
			if (variable == null)
			{
				throw new SyntaxErrorException(lcontext.Lexer.Current, "unexpected symbol near '{0}' - not a l-value", lcontext.Lexer.Current);
			}
			return variable;
		}

		public override void Compile(ByteCode bc)
		{
			using (bc.EnterSource(m_Ref))
			{
				foreach (Expression rValue in m_RValues)
				{
					rValue.Compile(bc);
				}
				for (int i = 0; i < m_LValues.Count; i++)
				{
					m_LValues[i].CompileAssignment(bc, Math.Max(m_RValues.Count - 1 - i, 0), i - Math.Min(i, m_RValues.Count - 1));
				}
				bc.Emit_Pop(m_RValues.Count);
			}
		}
	}
}
