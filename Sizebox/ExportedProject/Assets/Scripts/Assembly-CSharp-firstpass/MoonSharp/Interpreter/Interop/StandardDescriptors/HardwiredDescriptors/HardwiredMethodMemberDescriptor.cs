using System.Collections.Generic;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop.StandardDescriptors.HardwiredDescriptors
{
	public abstract class HardwiredMethodMemberDescriptor : FunctionMemberDescriptorBase
	{
		public override DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			this.CheckAccess(MemberDescriptorAccess.CanExecute, obj);
			List<int> outParams = null;
			object[] pars = base.BuildArgumentList(script, obj, context, args, out outParams);
			object obj2 = Invoke(script, obj, pars, CalcArgsCount(pars));
			return DynValue.FromObject(script, obj2);
		}

		private int CalcArgsCount(object[] pars)
		{
			int num = pars.Length;
			for (int i = 0; i < pars.Length; i++)
			{
				if (base.Parameters[i].HasDefaultValue && pars[i] is DefaultValue)
				{
					num--;
				}
			}
			return num;
		}

		protected abstract object Invoke(Script script, object obj, object[] pars, int argscount);
	}
}
