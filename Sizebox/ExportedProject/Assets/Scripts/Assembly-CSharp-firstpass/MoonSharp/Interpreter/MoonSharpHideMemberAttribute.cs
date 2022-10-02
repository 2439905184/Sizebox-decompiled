using System;

namespace MoonSharp.Interpreter
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpHideMemberAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public MoonSharpHideMemberAttribute(string memberName)
		{
			MemberName = memberName;
		}
	}
}
