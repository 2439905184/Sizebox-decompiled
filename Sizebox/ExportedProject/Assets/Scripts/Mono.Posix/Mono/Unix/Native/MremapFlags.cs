using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum MremapFlags : ulong
	{
		MREMAP_MAYMOVE = 1uL
	}
}
