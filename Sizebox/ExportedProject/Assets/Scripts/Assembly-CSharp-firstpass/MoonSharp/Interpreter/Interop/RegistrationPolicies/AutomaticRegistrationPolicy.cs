using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	public class AutomaticRegistrationPolicy : DefaultRegistrationPolicy
	{
		public override bool AllowTypeAutoRegistration(Type type)
		{
			return true;
		}
	}
}
