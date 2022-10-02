using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter.Tree
{
	internal abstract class Expression : NodeBase
	{
		public Expression(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
		}

		public virtual string GetFriendlyDebugName()
		{
			return null;
		}

		public abstract DynValue Eval(ScriptExecutionContext context);

		public virtual SymbolRef FindDynamic(ScriptExecutionContext context)
		{
			return null;
		}

		internal static List<Expression> ExprListAfterFirstExpr(ScriptLoadingContext lcontext, Expression expr1)
		{
			List<Expression> list = new List<Expression>();
			list.Add(expr1);
			while (lcontext.Lexer.Current.Type == TokenType.Comma)
			{
				lcontext.Lexer.Next();
				list.Add(Expr(lcontext));
			}
			return list;
		}

		internal static List<Expression> ExprList(ScriptLoadingContext lcontext)
		{
			List<Expression> list = new List<Expression>();
			while (true)
			{
				list.Add(Expr(lcontext));
				if (lcontext.Lexer.Current.Type != TokenType.Comma)
				{
					break;
				}
				lcontext.Lexer.Next();
			}
			return list;
		}

		internal static Expression Expr(ScriptLoadingContext lcontext)
		{
			return SubExpr(lcontext, true);
		}

		internal static Expression SubExpr(ScriptLoadingContext lcontext, bool isPrimary)
		{
			Expression expression = null;
			Token current = lcontext.Lexer.Current;
			if (current.IsUnaryOperator())
			{
				lcontext.Lexer.Next();
				expression = SubExpr(lcontext, false);
				Token unaryOpToken = current;
				current = lcontext.Lexer.Current;
				if (isPrimary && current.Type == TokenType.Op_Pwr)
				{
					List<Expression> list = new List<Expression>();
					list.Add(expression);
					while (isPrimary && current.Type == TokenType.Op_Pwr)
					{
						lcontext.Lexer.Next();
						list.Add(SubExpr(lcontext, false));
						current = lcontext.Lexer.Current;
					}
					expression = list[list.Count - 1];
					for (int num = list.Count - 2; num >= 0; num--)
					{
						expression = BinaryOperatorExpression.CreatePowerExpression(list[num], expression, lcontext);
					}
				}
				expression = new UnaryOperatorExpression(lcontext, expression, unaryOpToken);
			}
			else
			{
				expression = SimpleExp(lcontext);
			}
			current = lcontext.Lexer.Current;
			if (isPrimary && current.IsBinaryOperator())
			{
				object chain = BinaryOperatorExpression.BeginOperatorChain();
				BinaryOperatorExpression.AddExpressionToChain(chain, expression);
				while (current.IsBinaryOperator())
				{
					BinaryOperatorExpression.AddOperatorToChain(chain, current);
					lcontext.Lexer.Next();
					Expression exp = SubExpr(lcontext, false);
					BinaryOperatorExpression.AddExpressionToChain(chain, exp);
					current = lcontext.Lexer.Current;
				}
				expression = BinaryOperatorExpression.CommitOperatorChain(chain, lcontext);
			}
			return expression;
		}

		internal static Expression SimpleExp(ScriptLoadingContext lcontext)
		{
			Token current = lcontext.Lexer.Current;
			switch (current.Type)
			{
			case TokenType.False:
			case TokenType.Nil:
			case TokenType.True:
			case TokenType.String:
			case TokenType.String_Long:
			case TokenType.Number:
			case TokenType.Number_HexFloat:
			case TokenType.Number_Hex:
				return new LiteralExpression(lcontext, current);
			case TokenType.VarArgs:
				return new SymbolRefExpression(current, lcontext);
			case TokenType.Brk_Open_Curly:
			case TokenType.Brk_Open_Curly_Shared:
				return new TableConstructor(lcontext, current.Type == TokenType.Brk_Open_Curly_Shared);
			case TokenType.Function:
				lcontext.Lexer.Next();
				return new FunctionDefinitionExpression(lcontext, false, false);
			case TokenType.Lambda:
				return new FunctionDefinitionExpression(lcontext, false, true);
			default:
				return PrimaryExp(lcontext);
			}
		}

		internal static Expression PrimaryExp(ScriptLoadingContext lcontext)
		{
			Expression expression = PrefixExp(lcontext);
			while (true)
			{
				Token current = lcontext.Lexer.Current;
				Token thisCallName = null;
				switch (current.Type)
				{
				case TokenType.Dot:
				{
					lcontext.Lexer.Next();
					Token token = NodeBase.CheckTokenType(lcontext, TokenType.Name);
					expression = new IndexExpression(expression, token.Text, lcontext);
					break;
				}
				case TokenType.Brk_Open_Square:
				{
					Token current2 = lcontext.Lexer.Current;
					lcontext.Lexer.Next();
					Expression expression2 = Expr(lcontext);
					if (lcontext.Lexer.Current.Type == TokenType.Comma)
					{
						expression2 = new ExprListExpression(ExprListAfterFirstExpr(lcontext, expression2), lcontext);
					}
					NodeBase.CheckMatch(lcontext, current2, TokenType.Brk_Close_Square, "]");
					expression = new IndexExpression(expression, expression2, lcontext);
					break;
				}
				case TokenType.Colon:
					lcontext.Lexer.Next();
					thisCallName = NodeBase.CheckTokenType(lcontext, TokenType.Name);
					goto case TokenType.Brk_Open_Curly;
				case TokenType.Brk_Open_Curly:
				case TokenType.Brk_Open_Round:
				case TokenType.String:
				case TokenType.String_Long:
				case TokenType.Brk_Open_Curly_Shared:
					expression = new FunctionCallExpression(lcontext, expression, thisCallName);
					break;
				default:
					return expression;
				}
			}
		}

		private static Expression PrefixExp(ScriptLoadingContext lcontext)
		{
			Token current = lcontext.Lexer.Current;
			switch (current.Type)
			{
			case TokenType.Brk_Open_Round:
			{
				lcontext.Lexer.Next();
				Expression exp = Expr(lcontext);
				exp = new AdjustmentExpression(lcontext, exp);
				NodeBase.CheckMatch(lcontext, current, TokenType.Brk_Close_Round, ")");
				return exp;
			}
			case TokenType.Name:
				return new SymbolRefExpression(current, lcontext);
			default:
				throw new SyntaxErrorException(current, "unexpected symbol near '{0}'", current.Text)
				{
					IsPrematureStreamTermination = (current.Type == TokenType.Eof)
				};
			}
		}
	}
}
