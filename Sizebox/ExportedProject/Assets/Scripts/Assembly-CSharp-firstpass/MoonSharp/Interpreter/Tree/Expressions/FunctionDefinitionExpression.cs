using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Tree.Expressions
{
	internal class FunctionDefinitionExpression : Expression, IClosureBuilder
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<int> _003C_003E9__21_0;

			internal int _003CCompile_003Eb__21_0()
			{
				return 0;
			}
		}

		private SymbolRef[] m_ParamNames;

		private Statement m_Statement;

		private RuntimeScopeFrame m_StackFrame;

		private List<SymbolRef> m_Closure = new List<SymbolRef>();

		private bool m_HasVarArgs;

		private Instruction m_ClosureInstruction;

		private bool m_UsesGlobalEnv;

		private SymbolRef m_Env;

		private SourceRef m_Begin;

		private SourceRef m_End;

		public FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool usesGlobalEnv)
			: this(lcontext, false, usesGlobalEnv, false)
		{
		}

		public FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool pushSelfParam, bool isLambda)
			: this(lcontext, pushSelfParam, false, isLambda)
		{
		}

		private FunctionDefinitionExpression(ScriptLoadingContext lcontext, bool pushSelfParam, bool usesGlobalEnv, bool isLambda)
			: base(lcontext)
		{
			if (m_UsesGlobalEnv = usesGlobalEnv)
			{
				NodeBase.CheckTokenType(lcontext, TokenType.Function);
			}
			Token token = NodeBase.CheckTokenType(lcontext, isLambda ? TokenType.Lambda : TokenType.Brk_Open_Round);
			List<string> paramnames = BuildParamList(lcontext, pushSelfParam, token, isLambda);
			m_Begin = token.GetSourceRefUpTo(lcontext.Lexer.Current);
			lcontext.Scope.PushFunction(this, m_HasVarArgs);
			if (m_UsesGlobalEnv)
			{
				m_Env = lcontext.Scope.DefineLocal("_ENV");
			}
			else
			{
				lcontext.Scope.ForceEnvUpValue();
			}
			m_ParamNames = DefineArguments(paramnames, lcontext);
			if (isLambda)
			{
				m_Statement = CreateLambdaBody(lcontext);
			}
			else
			{
				m_Statement = CreateBody(lcontext);
			}
			m_StackFrame = lcontext.Scope.PopFunction();
			lcontext.Source.Refs.Add(m_Begin);
			lcontext.Source.Refs.Add(m_End);
		}

		private Statement CreateLambdaBody(ScriptLoadingContext lcontext)
		{
			Token current = lcontext.Lexer.Current;
			Expression e = Expression.Expr(lcontext);
			Token current2 = lcontext.Lexer.Current;
			SourceRef sourceRefUpTo = current.GetSourceRefUpTo(current2);
			return new ReturnStatement(lcontext, e, sourceRefUpTo);
		}

		private Statement CreateBody(ScriptLoadingContext lcontext)
		{
			Statement result = new CompositeStatement(lcontext);
			if (lcontext.Lexer.Current.Type != TokenType.End)
			{
				throw new SyntaxErrorException(lcontext.Lexer.Current, "'end' expected near '{0}'", lcontext.Lexer.Current.Text)
				{
					IsPrematureStreamTermination = (lcontext.Lexer.Current.Type == TokenType.Eof)
				};
			}
			m_End = lcontext.Lexer.Current.GetSourceRef();
			lcontext.Lexer.Next();
			return result;
		}

		private List<string> BuildParamList(ScriptLoadingContext lcontext, bool pushSelfParam, Token openBracketToken, bool isLambda)
		{
			TokenType tokenType = (isLambda ? TokenType.Lambda : TokenType.Brk_Close_Round);
			List<string> list = new List<string>();
			if (pushSelfParam)
			{
				list.Add("self");
			}
			while (lcontext.Lexer.Current.Type != tokenType)
			{
				Token current = lcontext.Lexer.Current;
				if (current.Type == TokenType.Name)
				{
					list.Add(current.Text);
				}
				else if (current.Type == TokenType.VarArgs)
				{
					m_HasVarArgs = true;
					list.Add("...");
				}
				else
				{
					NodeBase.UnexpectedTokenType(current);
				}
				lcontext.Lexer.Next();
				current = lcontext.Lexer.Current;
				if (current.Type == TokenType.Comma)
				{
					lcontext.Lexer.Next();
					continue;
				}
				NodeBase.CheckMatch(lcontext, openBracketToken, tokenType, isLambda ? "|" : ")");
				break;
			}
			if (lcontext.Lexer.Current.Type == tokenType)
			{
				lcontext.Lexer.Next();
			}
			return list;
		}

		private SymbolRef[] DefineArguments(List<string> paramnames, ScriptLoadingContext lcontext)
		{
			HashSet<string> hashSet = new HashSet<string>();
			SymbolRef[] array = new SymbolRef[paramnames.Count];
			for (int num = paramnames.Count - 1; num >= 0; num--)
			{
				if (!hashSet.Add(paramnames[num]))
				{
					paramnames[num] = paramnames[num] + "@" + num;
				}
				array[num] = lcontext.Scope.DefineLocal(paramnames[num]);
			}
			return array;
		}

		public SymbolRef CreateUpvalue(BuildTimeScope scope, SymbolRef symbol)
		{
			for (int i = 0; i < m_Closure.Count; i++)
			{
				if (m_Closure[i].i_Name == symbol.i_Name)
				{
					return SymbolRef.Upvalue(symbol.i_Name, i);
				}
			}
			m_Closure.Add(symbol);
			if (m_ClosureInstruction != null)
			{
				m_ClosureInstruction.SymbolList = m_Closure.ToArray();
			}
			return SymbolRef.Upvalue(symbol.i_Name, m_Closure.Count - 1);
		}

		public override DynValue Eval(ScriptExecutionContext context)
		{
			throw new DynamicExpressionException("Dynamic Expressions cannot define new functions.");
		}

		public int CompileBody(ByteCode bc, string friendlyName)
		{
			string funcName = friendlyName ?? ("<" + m_Begin.FormatLocation(bc.Script, true) + ">");
			bc.PushSourceRef(m_Begin);
			Instruction instruction = bc.Emit_Jump(OpCode.Jump, -1);
			Instruction instruction2 = bc.Emit_Meta(funcName, OpCodeMetadataType.FunctionEntrypoint);
			int jumpPointForLastInstruction = bc.GetJumpPointForLastInstruction();
			bc.Emit_BeginFn(m_StackFrame);
			bc.LoopTracker.Loops.Push(new LoopBoundary());
			int jumpPointForLastInstruction2 = bc.GetJumpPointForLastInstruction();
			if (m_UsesGlobalEnv)
			{
				bc.Emit_Load(SymbolRef.Upvalue("_ENV", 0));
				bc.Emit_Store(m_Env, 0, 0);
				bc.Emit_Pop();
			}
			if (m_ParamNames.Length != 0)
			{
				bc.Emit_Args(m_ParamNames);
			}
			m_Statement.Compile(bc);
			bc.PopSourceRef();
			bc.PushSourceRef(m_End);
			bc.Emit_Ret(0);
			bc.LoopTracker.Loops.Pop();
			instruction.NumVal = bc.GetJumpPointForNextInstruction();
			instruction2.NumVal = bc.GetJumpPointForLastInstruction() - jumpPointForLastInstruction;
			bc.PopSourceRef();
			return jumpPointForLastInstruction2;
		}

		public int Compile(ByteCode bc, Func<int> afterDecl, string friendlyName)
		{
			using (bc.EnterSource(m_Begin))
			{
				SymbolRef[] symbols = m_Closure.ToArray();
				m_ClosureInstruction = bc.Emit_Closure(symbols, bc.GetJumpPointForNextInstruction());
				int num = afterDecl();
				m_ClosureInstruction.NumVal += 2 + num;
			}
			return CompileBody(bc, friendlyName);
		}

		public override void Compile(ByteCode bc)
		{
			Compile(bc, _003C_003Ec._003C_003E9__21_0 ?? (_003C_003Ec._003C_003E9__21_0 = _003C_003Ec._003C_003E9._003CCompile_003Eb__21_0), null);
		}
	}
}
