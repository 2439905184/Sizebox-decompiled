using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	public class PermanentRegistrationPolicy : IRegistrationPolicy
	{
		public IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor)
		{
			return oldDescriptor ?? newDescriptor;
		}

		public bool AllowTypeAutoRegistration(Type type)
		{
			return false;
		}
	}
}
