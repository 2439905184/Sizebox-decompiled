using MoonSharp.Interpreter.Tree.Expressions;

namespace MoonSharp.Interpreter
{
	public class DynamicExpression : IScriptPrivateResource
	{
		private DynamicExprExpression m_Exp;

		private DynValue m_Constant;

		public readonly string ExpressionCode;

		public Script OwnerScript { get; private set; }

		internal DynamicExpression(Script S, string strExpr, DynamicExprExpression expr)
		{
			ExpressionCode = strExpr;
			OwnerScript = S;
			m_Exp = expr;
		}

		internal DynamicExpression(Script S, string strExpr, DynValue constant)
		{
			ExpressionCode = strExpr;
			OwnerScript = S;
			m_Constant = constant;
		}

		public DynValue Evaluate(ScriptExecutionContext context = null)
		{
			context = context ?? OwnerScript.CreateDynamicExecutionContext();
			this.CheckScriptOwnership(context.GetScript());
			if (m_Constant != null)
			{
				return m_Constant;
			}
			return m_Exp.Eval(context);
		}

		public SymbolRef FindSymbol(ScriptExecutionContext context)
		{
			this.CheckScriptOwnership(context.GetScript());
			if (m_Exp != null)
			{
				return m_Exp.FindDynamic(context);
			}
			return null;
		}

		public bool IsConstant()
		{
			return m_Constant != null;
		}

		public override int GetHashCode()
		{
			return ExpressionCode.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			DynamicExpression dynamicExpression = obj as DynamicExpression;
			if (dynamicExpression == null)
			{
				return false;
			}
			return dynamicExpression.ExpressionCode == ExpressionCode;
		}
	}
}
