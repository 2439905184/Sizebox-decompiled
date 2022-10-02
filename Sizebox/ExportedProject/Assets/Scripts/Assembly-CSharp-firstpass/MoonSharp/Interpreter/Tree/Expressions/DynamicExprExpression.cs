using System;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class DynamicExprExpression : Expression
	{
		private Expression m_Exp;

		public DynamicExprExpression(Expression exp, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			lcontext.Anonymous = true;
			m_Exp = exp;
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			return m_Exp.Eval(context);
		}

		public override void Compile(ByteCode bc)
		{
			throw new InvalidOperationException();
		}

		public override SymbolRef FindDynamic(ScriptExecutionContext context)
		{
			return m_Exp.FindDynamic(context);
		}
	}
}
