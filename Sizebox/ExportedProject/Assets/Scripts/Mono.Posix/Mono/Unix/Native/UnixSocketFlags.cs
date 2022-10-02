using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum UnixSocketFlags
	{
		SOCK_CLOEXEC = 0x80000,
		SOCK_NONBLOCK = 0x800
	}
}
