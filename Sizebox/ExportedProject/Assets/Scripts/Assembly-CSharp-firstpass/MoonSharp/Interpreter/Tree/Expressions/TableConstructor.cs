using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class TableConstructor : Expression
	{
		private bool m_Shared;

		private List<Expression> m_PositionalValues = new List<Expression>();

		private List<KeyValuePair<Expression, Expression>> m_CtorArgs = new List<KeyValuePair<Expression, Expression>>();

		public TableConstructor(ScriptLoadingContext lcontext, bool shared)
			: base(lcontext)
		{
			m_Shared = shared;
			NodeBase.CheckTokenType(lcontext, TokenType.Brk_Open_Curly, TokenType.Brk_Open_Curly_Shared);
			while (lcontext.Lexer.Current.Type != TokenType.Brk_Close_Curly)
			{
				switch (lcontext.Lexer.Current.Type)
				{
				case TokenType.Name:
					if (lcontext.Lexer.PeekNext().Type == TokenType.Op_Assignment)
					{
						StructField(lcontext);
					}
					else
					{
						ArrayField(lcontext);
					}
					break;
				case TokenType.Brk_Open_Square:
					MapField(lcontext);
					break;
				default:
					ArrayField(lcontext);
					break;
				}
				Token current = lcontext.Lexer.Current;
				if (current.Type != TokenType.Comma && current.Type != TokenType.SemiColon)
				{
					break;
				}
				lcontext.Lexer.Next();
			}
			NodeBase.CheckTokenType(lcontext, TokenType.Brk_Close_Curly);
		}

		private void MapField(ScriptLoadingContext lcontext)
		{
			lcontext.Lexer.Next();
			Expression key = Expression.Expr(lcontext);
			NodeBase.CheckTokenType(lcontext, TokenType.Brk_Close_Square);
			NodeBase.CheckTokenType(lcontext, TokenType.Op_Assignment);
			Expression value = Expression.Expr(lcontext);
			m_CtorArgs.Add(new KeyValuePair<Expression, Expression>(key, value));
		}

		private void StructField(ScriptLoadingContext lcontext)
		{
			Expression key = new LiteralExpression(lcontext, DynValue.NewString(lcontext.Lexer.Current.Text));
			lcontext.Lexer.Next();
			NodeBase.CheckTokenType(lcontext, TokenType.Op_Assignment);
			Expression value = Expression.Expr(lcontext);
			m_CtorArgs.Add(new KeyValuePair<Expression, Expression>(key, value));
		}

		private void ArrayField(ScriptLoadingContext lcontext)
		{
			Expression item = Expression.Expr(lcontext);
			m_PositionalValues.Add(item);
		}

		public override void Compile(ByteCode bc)
		{
			bc.Emit_NewTable(m_Shared);
			foreach (KeyValuePair<Expression, Expression> ctorArg in m_CtorArgs)
			{
				ctorArg.Key.Compile(bc);
				ctorArg.Value.Compile(bc);
				bc.Emit_TblInitN();
			}
			for (int i = 0; i < m_PositionalValues.Count; i++)
			{
				m_PositionalValues[i].Compile(bc);
				bc.Emit_TblInitI(i == m_PositionalValues.Count - 1);
			}
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			if (!m_Shared)
			{
				throw new DynamicExpressionException("Dynamic Expressions cannot define new non-prime tables.");
			}
			DynValue dynValue = DynValue.NewPrimeTable();
			Table table = dynValue.Table;
			int num = 0;
			foreach (Expression positionalValue in m_PositionalValues)
			{
				table.Set(++num, positionalValue.Eval(context));
			}
			foreach (KeyValuePair<Expression, Expression> ctorArg in m_CtorArgs)
			{
				table.Set(ctorArg.Key.Eval(context), ctorArg.Value.Eval(context));
			}
			return dynValue;
		}
	}
}
