using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	public enum EpollFlags
	{
		EPOLL_CLOEXEC = 0x1E8480,
		EPOLL_NONBLOCK = 0xFA0
	}
}
