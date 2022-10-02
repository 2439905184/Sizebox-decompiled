using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Tree.Expressions;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Tree
{
	internal abstract class Statement : NodeBase
	{
		public Statement(ScriptLoadingContext lcontext)
			: base(lcontext)
		{
		}

		protected static Statement CreateStatement(ScriptLoadingContext lcontext, out bool forceLast)
		{
			Token current = lcontext.Lexer.Current;
			forceLast = false;
			switch (current.Type)
			{
			case TokenType.DoubleColon:
				return new LabelStatement(lcontext);
			case TokenType.Goto:
				return new GotoStatement(lcontext);
			case TokenType.SemiColon:
				lcontext.Lexer.Next();
				return new EmptyStatement(lcontext);
			case TokenType.If:
				return new IfStatement(lcontext);
			case TokenType.While:
				return new WhileStatement(lcontext);
			case TokenType.Do:
				return new ScopeBlockStatement(lcontext);
			case TokenType.For:
				return DispatchForLoopStatement(lcontext);
			case TokenType.Repeat:
				return new RepeatStatement(lcontext);
			case TokenType.Function:
				return new FunctionDefinitionStatement(lcontext, false, null);
			case TokenType.Local:
			{
				Token current3 = lcontext.Lexer.Current;
				lcontext.Lexer.Next();
				if (lcontext.Lexer.Current.Type == TokenType.Function)
				{
					return new FunctionDefinitionStatement(lcontext, true, current3);
				}
				return new AssignmentStatement(lcontext, current3);
			}
			case TokenType.Return:
				forceLast = true;
				return new ReturnStatement(lcontext);
			case TokenType.Break:
				return new BreakStatement(lcontext);
			default:
			{
				Token current2 = lcontext.Lexer.Current;
				Expression expression = Expression.PrimaryExp(lcontext);
				FunctionCallExpression functionCallExpression = expression as FunctionCallExpression;
				if (functionCallExpression != null)
				{
					return new FunctionCallStatement(lcontext, functionCallExpression);
				}
				return new AssignmentStatement(lcontext, expression, current2);
			}
			}
		}

		private static Statement DispatchForLoopStatement(ScriptLoadingContext lcontext)
		{
			Token forToken = NodeBase.CheckTokenType(lcontext, TokenType.For);
			Token token = NodeBase.CheckTokenType(lcontext, TokenType.Name);
			if (lcontext.Lexer.Current.Type == TokenType.Op_Assignment)
			{
				return new ForLoopStatement(lcontext, token, forToken);
			}
			return new ForEachLoopStatement(lcontext, token, forToken);
		}
	}
}
