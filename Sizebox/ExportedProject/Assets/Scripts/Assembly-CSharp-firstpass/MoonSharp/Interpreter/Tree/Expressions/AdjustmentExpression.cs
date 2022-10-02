using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class AdjustmentExpression : Expression
	{
		private Expression expression;

		public AdjustmentExpression(ScriptLoadingContext lcontext, Expression exp)
			: base(lcontext)
		{
			expression = exp;
		}

		public override void Compile(ByteCode bc)
		{
			expression.Compile(bc);
			bc.Emit_Scalar();
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			return expression.Eval(context).ToScalar();
		}
	}
}
