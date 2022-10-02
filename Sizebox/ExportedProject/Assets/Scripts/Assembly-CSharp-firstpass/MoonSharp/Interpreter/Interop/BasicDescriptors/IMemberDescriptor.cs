namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	public interface IMemberDescriptor
	{
		bool IsStatic { get; }

		string Name { get; }

		MemberDescriptorAccess MemberAccess { get; }

		DynValue GetValue(Script script, object obj);

		void SetValue(Script script, object obj, DynValue value);
	}
}
