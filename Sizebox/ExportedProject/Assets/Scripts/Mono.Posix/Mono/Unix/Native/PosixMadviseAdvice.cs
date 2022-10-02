using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum PosixMadviseAdvice
	{
		POSIX_MADV_NORMAL = 0,
		POSIX_MADV_RANDOM = 1,
		POSIX_MADV_SEQUENTIAL = 2,
		POSIX_MADV_WILLNEED = 3,
		POSIX_MADV_DONTNEED = 4
	}
}
