using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum PosixFadviseAdvice
	{
		POSIX_FADV_NORMAL = 0,
		POSIX_FADV_RANDOM = 1,
		POSIX_FADV_SEQUENTIAL = 2,
		POSIX_FADV_WILLNEED = 3,
		POSIX_FADV_DONTNEED = 4,
		POSIX_FADV_NOREUSE = 5
	}
}
