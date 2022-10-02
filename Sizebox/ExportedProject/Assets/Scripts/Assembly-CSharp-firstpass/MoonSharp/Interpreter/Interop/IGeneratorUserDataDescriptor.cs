using System;

namespace MoonSharp.Interpreter.Interop
{
	public interface IGeneratorUserDataDescriptor : IUserDataDescriptor
	{
		IUserDataDescriptor Generate(Type type);
	}
}
