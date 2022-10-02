using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.Diagnostics;
using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;
using MoonSharp.Interpreter.Tree.Expressions;
using MoonSharp.Interpreter.Tree.Statements;

namespace MoonSharp.Interpreter.Tree.Fast_Interface
{
	internal static class Loader_Fast
	{
		internal static DynamicExprExpression LoadDynamicExpr(Script script, SourceCode source)
		{
			ScriptLoadingContext scriptLoadingContext = CreateLoadingContext(script, source);
			try
			{
				scriptLoadingContext.IsDynamicExpression = true;
				scriptLoadingContext.Anonymous = true;
				Expression exp;
				using (script.PerformanceStats.StartStopwatch(PerformanceCounter.AstCreation))
				{
					exp = Expression.Expr(scriptLoadingContext);
				}
				return new DynamicExprExpression(exp, scriptLoadingContext);
			}
			catch (SyntaxErrorException ex)
			{
				ex.DecorateMessage(script);
				ex.Rethrow();
				throw;
			}
		}

		private static ScriptLoadingContext CreateLoadingContext(Script script, SourceCode source)
		{
			return new ScriptLoadingContext(script)
			{
				Scope = new BuildTimeScope(),
				Source = source,
				Lexer = new Lexer(source.SourceID, source.Code, true)
			};
		}

		internal static int LoadChunk(Script script, SourceCode source, ByteCode bytecode)
		{
			ScriptLoadingContext lcontext = CreateLoadingContext(script, source);
			try
			{
				Statement statement;
				using (script.PerformanceStats.StartStopwatch(PerformanceCounter.AstCreation))
				{
					statement = new ChunkStatement(lcontext);
				}
				int result = -1;
				using (script.PerformanceStats.StartStopwatch(PerformanceCounter.Compilation))
				{
					using (bytecode.EnterSource(null))
					{
						bytecode.Emit_Nop(string.Format("Begin chunk {0}", source.Name));
						result = bytecode.GetJumpPointForLastInstruction();
						statement.Compile(bytecode);
						bytecode.Emit_Nop(string.Format("End chunk {0}", source.Name));
					}
				}
				return result;
			}
			catch (SyntaxErrorException ex)
			{
				ex.DecorateMessage(script);
				ex.Rethrow();
				throw;
			}
		}

		internal static int LoadFunction(Script script, SourceCode source, ByteCode bytecode, bool usesGlobalEnv)
		{
			ScriptLoadingContext lcontext = CreateLoadingContext(script, source);
			try
			{
				FunctionDefinitionExpression functionDefinitionExpression;
				using (script.PerformanceStats.StartStopwatch(PerformanceCounter.AstCreation))
				{
					functionDefinitionExpression = new FunctionDefinitionExpression(lcontext, usesGlobalEnv);
				}
				int result = -1;
				using (script.PerformanceStats.StartStopwatch(PerformanceCounter.Compilation))
				{
					using (bytecode.EnterSource(null))
					{
						bytecode.Emit_Nop(string.Format("Begin function {0}", source.Name));
						result = functionDefinitionExpression.CompileBody(bytecode, source.Name);
						bytecode.Emit_Nop(string.Format("End function {0}", source.Name));
					}
				}
				return result;
			}
			catch (SyntaxErrorException ex)
			{
				ex.DecorateMessage(script);
				ex.Rethrow();
				throw;
			}
		}
	}
}
