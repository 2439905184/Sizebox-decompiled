using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum MlockallFlags
	{
		MCL_CURRENT = 1,
		MCL_FUTURE = 2
	}
}
