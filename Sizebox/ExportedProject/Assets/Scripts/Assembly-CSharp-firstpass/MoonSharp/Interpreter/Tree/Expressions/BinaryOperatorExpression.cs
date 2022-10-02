using System;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class BinaryOperatorExpression : Expression
	{
		[Flags]
		private enum Operator
		{
			NotAnOperator = 0,
			Or = 1,
			And = 2,
			Less = 4,
			Greater = 8,
			LessOrEqual = 0x10,
			GreaterOrEqual = 0x20,
			NotEqual = 0x40,
			Equal = 0x80,
			StrConcat = 0x100,
			Add = 0x200,
			Sub = 0x400,
			Mul = 0x1000,
			Div = 0x2000,
			Mod = 0x4000,
			Power = 0x8000
		}

		private class Node
		{
			public Expression Expr;

			public Operator Op;

			public Node Prev;

			public Node Next;
		}

		private class LinkedList
		{
			public Node Nodes;

			public Node Last;

			public Operator OperatorMask;
		}

		private const Operator POWER = Operator.Power;

		private const Operator MUL_DIV_MOD = Operator.Mul | Operator.Div | Operator.Mod;

		private const Operator ADD_SUB = Operator.Add | Operator.Sub;

		private const Operator STRCAT = Operator.StrConcat;

		private const Operator COMPARES = Operator.Less | Operator.Greater | Operator.LessOrEqual | Operator.GreaterOrEqual | Operator.NotEqual | Operator.Equal;

		private const Operator LOGIC_AND = Operator.And;

		private const Operator LOGIC_OR = Operator.Or;

		private Expression m_Exp1;

		private Expression m_Exp2;

		private Operator m_Operator;

		public static object BeginOperatorChain()
		{
			return new LinkedList();
		}

		public static void AddExpressionToChain(object chain, Expression exp)
		{
			LinkedList list = (LinkedList)chain;
			Node node = new Node
			{
				Expr = exp
			};
			AddNode(list, node);
		}

		public static void AddOperatorToChain(object chain, Token op)
		{
			LinkedList list = (LinkedList)chain;
			Node node = new Node
			{
				Op = ParseBinaryOperator(op)
			};
			AddNode(list, node);
		}

		public static Expression CommitOperatorChain(object chain, ScriptLoadingContext lcontext)
		{
			return CreateSubTree((LinkedList)chain, lcontext);
		}

		public static Expression CreatePowerExpression(Expression op1, Expression op2, ScriptLoadingContext lcontext)
		{
			return new BinaryOperatorExpression(op1, op2, Operator.Power, lcontext);
		}

		private static void AddNode(LinkedList list, Node node)
		{
			list.OperatorMask |= node.Op;
			if (list.Nodes == null)
			{
				list.Nodes = (list.Last = node);
				return;
			}
			list.Last.Next = node;
			node.Prev = list.Last;
			list.Last = node;
		}

		private static Expression CreateSubTree(LinkedList list, ScriptLoadingContext lcontext)
		{
			Operator operatorMask = list.OperatorMask;
			Node node = list.Nodes;
			if ((operatorMask & Operator.Power) != 0)
			{
				node = PrioritizeRightAssociative(node, lcontext, Operator.Power);
			}
			if ((operatorMask & (Operator.Mul | Operator.Div | Operator.Mod)) != 0)
			{
				node = PrioritizeLeftAssociative(node, lcontext, Operator.Mul | Operator.Div | Operator.Mod);
			}
			if ((operatorMask & (Operator.Add | Operator.Sub)) != 0)
			{
				node = PrioritizeLeftAssociative(node, lcontext, Operator.Add | Operator.Sub);
			}
			if ((operatorMask & Operator.StrConcat) != 0)
			{
				node = PrioritizeRightAssociative(node, lcontext, Operator.StrConcat);
			}
			if ((operatorMask & (Operator.Less | Operator.Greater | Operator.LessOrEqual | Operator.GreaterOrEqual | Operator.NotEqual | Operator.Equal)) != 0)
			{
				node = PrioritizeLeftAssociative(node, lcontext, Operator.Less | Operator.Greater | Operator.LessOrEqual | Operator.GreaterOrEqual | Operator.NotEqual | Operator.Equal);
			}
			if ((operatorMask & Operator.And) != 0)
			{
				node = PrioritizeLeftAssociative(node, lcontext, Operator.And);
			}
			if ((operatorMask & Operator.Or) != 0)
			{
				node = PrioritizeLeftAssociative(node, lcontext, Operator.Or);
			}
			if (node.Next != null || node.Prev != null)
			{
				throw new InternalErrorException("Expression reduction didn't work! - 1");
			}
			if (node.Expr == null)
			{
				throw new InternalErrorException("Expression reduction didn't work! - 2");
			}
			return node.Expr;
		}

		private static Node PrioritizeLeftAssociative(Node nodes, ScriptLoadingContext lcontext, Operator operatorsToFind)
		{
			for (Node node = nodes; node != null; node = node.Next)
			{
				Operator op = node.Op;
				if ((op & operatorsToFind) != 0)
				{
					node.Op = Operator.NotAnOperator;
					node.Expr = new BinaryOperatorExpression(node.Prev.Expr, node.Next.Expr, op, lcontext);
					node.Prev = node.Prev.Prev;
					node.Next = node.Next.Next;
					if (node.Next != null)
					{
						node.Next.Prev = node;
					}
					if (node.Prev != null)
					{
						node.Prev.Next = node;
					}
					else
					{
						nodes = node;
					}
				}
			}
			return nodes;
		}

		private static Node PrioritizeRightAssociative(Node nodes, ScriptLoadingContext lcontext, Operator operatorsToFind)
		{
			Node node = nodes;
			while (node.Next != null)
			{
				node = node.Next;
			}
			for (Node node2 = node; node2 != null; node2 = node2.Prev)
			{
				Operator op = node2.Op;
				if ((op & operatorsToFind) != 0)
				{
					node2.Op = Operator.NotAnOperator;
					node2.Expr = new BinaryOperatorExpression(node2.Prev.Expr, node2.Next.Expr, op, lcontext);
					node2.Prev = node2.Prev.Prev;
					node2.Next = node2.Next.Next;
					if (node2.Next != null)
					{
						node2.Next.Prev = node2;
					}
					if (node2.Prev != null)
					{
						node2.Prev.Next = node2;
					}
					else
					{
						nodes = node2;
					}
				}
			}
			return nodes;
		}

		private static Operator ParseBinaryOperator(Token token)
		{
			switch (token.Type)
			{
			case TokenType.Or:
				return Operator.Or;
			case TokenType.And:
				return Operator.And;
			case TokenType.Op_LessThan:
				return Operator.Less;
			case TokenType.Op_GreaterThan:
				return Operator.Greater;
			case TokenType.Op_LessThanEqual:
				return Operator.LessOrEqual;
			case TokenType.Op_GreaterThanEqual:
				return Operator.GreaterOrEqual;
			case TokenType.Op_NotEqual:
				return Operator.NotEqual;
			case TokenType.Op_Equal:
				return Operator.Equal;
			case TokenType.Op_Concat:
				return Operator.StrConcat;
			case TokenType.Op_Add:
				return Operator.Add;
			case TokenType.Op_MinusOrSub:
				return Operator.Sub;
			case TokenType.Op_Mul:
				return Operator.Mul;
			case TokenType.Op_Div:
				return Operator.Div;
			case TokenType.Op_Mod:
				return Operator.Mod;
			case TokenType.Op_Pwr:
				return Operator.Power;
			default:
				throw new InternalErrorException("Unexpected binary operator '{0}'", token.Text);
			}
		}

		private BinaryOperatorExpression(Expression exp1, Expression exp2, Operator op, ScriptLoadingContext lcontext)
			: base(lcontext)
		{
			m_Exp1 = exp1;
			m_Exp2 = exp2;
			m_Operator = op;
		}

		private static bool ShouldInvertBoolean(Operator op)
		{
			if (op != Operator.NotEqual && op != Operator.GreaterOrEqual)
			{
				return op == Operator.Greater;
			}
			return true;
		}

		private static OpCode OperatorToOpCode(Operator op)
		{
			switch (op)
			{
			case Operator.Less:
			case Operator.GreaterOrEqual:
				return OpCode.Less;
			case Operator.Greater:
			case Operator.LessOrEqual:
				return OpCode.LessEq;
			case Operator.NotEqual:
			case Operator.Equal:
				return OpCode.Eq;
			case Operator.StrConcat:
				return OpCode.Concat;
			case Operator.Add:
				return OpCode.Add;
			case Operator.Sub:
				return OpCode.Sub;
			case Operator.Mul:
				return OpCode.Mul;
			case Operator.Div:
				return OpCode.Div;
			case Operator.Mod:
				return OpCode.Mod;
			case Operator.Power:
				return OpCode.Power;
			default:
				throw new InternalErrorException("Unsupported operator {0}", op);
			}
		}

		public override void Compile(ByteCode bc)
		{
			m_Exp1.Compile(bc);
			if (m_Operator == Operator.Or)
			{
				Instruction instruction = bc.Emit_Jump(OpCode.JtOrPop, -1);
				m_Exp2.Compile(bc);
				instruction.NumVal = bc.GetJumpPointForNextInstruction();
				return;
			}
			if (m_Operator == Operator.And)
			{
				Instruction instruction2 = bc.Emit_Jump(OpCode.JfOrPop, -1);
				m_Exp2.Compile(bc);
				instruction2.NumVal = bc.GetJumpPointForNextInstruction();
				return;
			}
			if (m_Exp2 != null)
			{
				m_Exp2.Compile(bc);
			}
			bc.Emit_Operator(OperatorToOpCode(m_Operator));
			if (ShouldInvertBoolean(m_Operator))
			{
				bc.Emit_Operator(OpCode.Not);
			}
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			DynValue dynValue = m_Exp1.Eval(context).ToScalar();
			if (m_Operator == Operator.Or)
			{
				if (dynValue.CastToBool())
				{
					return dynValue;
				}
				return m_Exp2.Eval(context).ToScalar();
			}
			if (m_Operator == Operator.And)
			{
				if (!dynValue.CastToBool())
				{
					return dynValue;
				}
				return m_Exp2.Eval(context).ToScalar();
			}
			DynValue dynValue2 = m_Exp2.Eval(context).ToScalar();
			if ((m_Operator & (Operator.Less | Operator.Greater | Operator.LessOrEqual | Operator.GreaterOrEqual | Operator.NotEqual | Operator.Equal)) != 0)
			{
				return DynValue.NewBoolean(EvalComparison(dynValue, dynValue2, m_Operator));
			}
			if (m_Operator == Operator.StrConcat)
			{
				string text = dynValue.CastToString();
				string text2 = dynValue2.CastToString();
				if (text == null || text2 == null)
				{
					throw new DynamicExpressionException("Attempt to perform concatenation on non-strings.");
				}
				return DynValue.NewString(text + text2);
			}
			return DynValue.NewNumber(EvalArithmetic(dynValue, dynValue2));
		}

		private double EvalArithmetic(DynValue v1, DynValue v2)
		{
			double? num = v1.CastToNumber();
			double? num2 = v2.CastToNumber();
			if (!num.HasValue || !num2.HasValue)
			{
				throw new DynamicExpressionException("Attempt to perform arithmetic on non-numbers.");
			}
			double value = num.Value;
			double value2 = num2.Value;
			switch (m_Operator)
			{
			case Operator.Add:
				return value + value2;
			case Operator.Sub:
				return value - value2;
			case Operator.Mul:
				return value * value2;
			case Operator.Div:
				return value / value2;
			case Operator.Mod:
			{
				double num3 = Math.IEEERemainder(value, value2);
				if (num3 < 0.0)
				{
					num3 += value2;
				}
				return num3;
			}
			default:
				throw new DynamicExpressionException("Unsupported operator {0}", m_Operator);
			}
		}

		private bool EvalComparison(DynValue l, DynValue r, Operator op)
		{
			switch (op)
			{
			case Operator.Less:
				if (l.Type == DataType.Number && r.Type == DataType.Number)
				{
					return l.Number < r.Number;
				}
				if (l.Type == DataType.String && r.Type == DataType.String)
				{
					return l.String.CompareTo(r.String) < 0;
				}
				throw new DynamicExpressionException("Attempt to compare non-numbers, non-strings.");
			case Operator.LessOrEqual:
				if (l.Type == DataType.Number && r.Type == DataType.Number)
				{
					return l.Number <= r.Number;
				}
				if (l.Type == DataType.String && r.Type == DataType.String)
				{
					return l.String.CompareTo(r.String) <= 0;
				}
				throw new DynamicExpressionException("Attempt to compare non-numbers, non-strings.");
			case Operator.Equal:
				if (r == l)
				{
					return true;
				}
				if (r.Type != l.Type)
				{
					if ((l.Type == DataType.Nil && r.Type == DataType.Void) || (l.Type == DataType.Void && r.Type == DataType.Nil))
					{
						return true;
					}
					return false;
				}
				return r.Equals(l);
			case Operator.Greater:
				return !EvalComparison(l, r, Operator.LessOrEqual);
			case Operator.GreaterOrEqual:
				return !EvalComparison(l, r, Operator.Less);
			case Operator.NotEqual:
				return !EvalComparison(l, r, Operator.Equal);
			default:
				throw new DynamicExpressionException("Unsupported operator {0}", op);
			}
		}
	}
}
