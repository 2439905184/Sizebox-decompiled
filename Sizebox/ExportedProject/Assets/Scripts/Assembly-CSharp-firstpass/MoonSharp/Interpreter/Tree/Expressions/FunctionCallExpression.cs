using System.Collections.Generic;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class FunctionCallExpression : Expression
	{
		private List<Expression> m_Arguments;

		private Expression m_Function;

		private string m_Name;

		private string m_DebugErr;

		internal SourceRef SourceRef { get; private set; }

		public FunctionCallExpression(ScriptLoadingContext lcontext, Expression function, Token thisCallName)
			: base(lcontext)
		{
			Token token = thisCallName ?? lcontext.Lexer.Current;
			m_Name = ((thisCallName != null) ? thisCallName.Text : null);
			m_DebugErr = function.GetFriendlyDebugName();
			m_Function = function;
			switch (lcontext.Lexer.Current.Type)
			{
			case TokenType.Brk_Open_Round:
			{
				Token current = lcontext.Lexer.Current;
				lcontext.Lexer.Next();
				Token current2 = lcontext.Lexer.Current;
				if (current2.Type == TokenType.Brk_Close_Round)
				{
					m_Arguments = new List<Expression>();
					SourceRef = token.GetSourceRef(current2);
					lcontext.Lexer.Next();
				}
				else
				{
					m_Arguments = Expression.ExprList(lcontext);
					SourceRef = token.GetSourceRef(NodeBase.CheckMatch(lcontext, current, TokenType.Brk_Close_Round, ")"));
				}
				break;
			}
			case TokenType.String:
			case TokenType.String_Long:
			{
				m_Arguments = new List<Expression>();
				Expression item = new LiteralExpression(lcontext, lcontext.Lexer.Current);
				m_Arguments.Add(item);
				SourceRef = token.GetSourceRef(lcontext.Lexer.Current);
				break;
			}
			case TokenType.Brk_Open_Curly:
			case TokenType.Brk_Open_Curly_Shared:
				m_Arguments = new List<Expression>();
				m_Arguments.Add(new TableConstructor(lcontext, lcontext.Lexer.Current.Type == TokenType.Brk_Open_Curly_Shared));
				SourceRef = token.GetSourceRefUpTo(lcontext.Lexer.Current);
				break;
			default:
				throw new SyntaxErrorException(lcontext.Lexer.Current, "function arguments expected")
				{
					IsPrematureStreamTermination = (lcontext.Lexer.Current.Type == TokenType.Eof)
				};
			}
		}

		public override void Compile(ByteCode bc)
		{
			m_Function.Compile(bc);
			int num = m_Arguments.Count;
			if (!string.IsNullOrEmpty(m_Name))
			{
				bc.Emit_Copy(0);
				bc.Emit_Index(DynValue.NewString(m_Name), true);
				bc.Emit_Swap(0, 1);
				num++;
			}
			for (int i = 0; i < m_Arguments.Count; i++)
			{
				m_Arguments[i].Compile(bc);
			}
			if (!string.IsNullOrEmpty(m_Name))
			{
				bc.Emit_ThisCall(num, m_DebugErr);
			}
			else
			{
				bc.Emit_Call(num, m_DebugErr);
			}
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			throw new DynamicExpressionException("Dynamic Expressions cannot call functions.");
		}
	}
}
