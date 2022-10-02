using System;

namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	[Flags]
	public enum MemberDescriptorAccess
	{
		CanRead = 1,
		CanWrite = 2,
		CanExecute = 4
	}
}
