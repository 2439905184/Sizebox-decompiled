using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class IndexExpression : Expression, IVariable
	{
		private Expression m_BaseExp;

		private Expression m_IndexExp;

		private string m_Name;

		public IndexExpression(Expression baseExp, Expression indexExp, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			m_BaseExp = baseExp;
			m_IndexExp = indexExp;
		}

		public IndexExpression(Expression baseExp, string name, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			m_BaseExp = baseExp;
			m_Name = name;
		}

		public override void Compile(ByteCode bc)
		{
			m_BaseExp.Compile(bc);
			if (m_Name != null)
			{
				bc.Emit_Index(DynValue.NewString(m_Name), true);
			}
			else if (m_IndexExp is LiteralExpression)
			{
				LiteralExpression literalExpression = (LiteralExpression)m_IndexExp;
				bc.Emit_Index(literalExpression.Value);
			}
			else
			{
				m_IndexExp.Compile(bc);
				bc.Emit_Index(null, false, m_IndexExp is ExprListExpression);
			}
		}

		public void CompileAssignment(ByteCode bc, int stackofs, int tupleidx)
		{
			m_BaseExp.Compile(bc);
			if (m_Name != null)
			{
				bc.Emit_IndexSet(stackofs, tupleidx, DynValue.NewString(m_Name), true);
			}
			else if (m_IndexExp is LiteralExpression)
			{
				LiteralExpression literalExpression = (LiteralExpression)m_IndexExp;
				bc.Emit_IndexSet(stackofs, tupleidx, literalExpression.Value);
			}
			else
			{
				m_IndexExp.Compile(bc);
				bc.Emit_IndexSet(stackofs, tupleidx, null, false, m_IndexExp is ExprListExpression);
			}
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			DynValue dynValue = m_BaseExp.Eval(context).ToScalar();
			DynValue dynValue2 = ((m_IndexExp != null) ? m_IndexExp.Eval(context).ToScalar() : DynValue.NewString(m_Name));
			if (dynValue.Type != DataType.Table)
			{
				throw new DynamicExpressionException("Attempt to index non-table.");
			}
			if (dynValue2.IsNilOrNan())
			{
				throw new DynamicExpressionException("Attempt to index with nil or nan key.");
			}
			return dynValue.Table.Get(dynValue2) ?? DynValue.Nil;
		}
	}
}
