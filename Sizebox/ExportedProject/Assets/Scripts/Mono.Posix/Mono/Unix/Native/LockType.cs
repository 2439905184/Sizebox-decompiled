using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum LockType : short
	{
		F_RDLCK = 0,
		F_WRLCK = 1,
		F_UNLCK = 2
	}
}
