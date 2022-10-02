using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	public interface IRegistrationPolicy
	{
		IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor);

		bool AllowTypeAutoRegistration(Type type);
	}
}
