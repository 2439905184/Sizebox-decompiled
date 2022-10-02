using System.Collections.Generic;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class ExprListExpression : Expression
	{
		private List<Expression> expressions;

		public ExprListExpression(List<Expression> exps, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			expressions = exps;
		}

		public Expression[] GetExpressions()
		{
			return expressions.ToArray();
		}

		public override void Compile(ByteCode bc)
		{
			foreach (Expression expression in expressions)
			{
				expression.Compile(bc);
			}
			if (expressions.Count > 1)
			{
				bc.Emit_MkTuple(expressions.Count);
			}
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			if (expressions.Count >= 1)
			{
				return expressions[0].Eval(context);
			}
			return DynValue.Void;
		}
	}
}
