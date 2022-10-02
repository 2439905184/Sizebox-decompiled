namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "dynamic")]
	public class DynamicModule
	{
		private class DynamicExprWrapper
		{
			public DynamicExpression Expr;
		}

		public static void MoonSharpInit(Table globalTable, Table stringTable)
		{
			UserData.RegisterType<DynamicExprWrapper>(InteropAccessMode.HideMembers);
		}

		[MoonSharpModuleMethod]
		public static DynValue eval(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				if (args[0].Type == DataType.UserData)
				{
					UserData userData = args[0].UserData;
					if (userData.Object is DynamicExprWrapper)
					{
						return ((DynamicExprWrapper)userData.Object).Expr.Evaluate(executionContext);
					}
					throw ScriptRuntimeException.BadArgument(0, "dynamic.eval", "A userdata was passed, but was not a previously prepared expression.");
				}
				DynValue dynValue = args.AsType(0, "dynamic.eval", DataType.String);
				return executionContext.GetScript().CreateDynamicExpression(dynValue.String).Evaluate(executionContext);
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue prepare(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				DynValue dynValue = args.AsType(0, "dynamic.prepare", DataType.String);
				DynamicExpression expr = executionContext.GetScript().CreateDynamicExpression(dynValue.String);
				return UserData.Create(new DynamicExprWrapper
				{
					Expr = expr
				});
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}
	}
}
