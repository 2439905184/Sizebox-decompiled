using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	public class DefaultRegistrationPolicy : IRegistrationPolicy
	{
		public IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor)
		{
			if (newDescriptor == null)
			{
				return null;
			}
			return oldDescriptor ?? newDescriptor;
		}

		public virtual bool AllowTypeAutoRegistration(Type type)
		{
			return false;
		}
	}
}
