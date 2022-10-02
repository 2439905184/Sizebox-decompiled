using System;

namespace MoonSharp.Interpreter.Interop
{
	public interface IUserDataMemberDescriptor
	{
		string Name { get; }

		Type Type { get; }

		UserDataMemberType MemberType { get; }

		bool IsStatic { get; }

		DynValue GetValue(Script script, object obj);

		bool SetValue(Script script, object obj, DynValue value);

		void Optimize();
	}
}
