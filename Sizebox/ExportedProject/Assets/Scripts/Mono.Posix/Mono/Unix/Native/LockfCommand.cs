using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum LockfCommand
	{
		F_ULOCK = 0,
		F_LOCK = 1,
		F_TLOCK = 2,
		F_TEST = 3
	}
}
