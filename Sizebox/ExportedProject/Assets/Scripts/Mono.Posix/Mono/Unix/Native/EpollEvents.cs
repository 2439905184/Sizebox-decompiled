using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	[CLSCompliant(false)]
	public enum EpollEvents : uint
	{
		EPOLLIN = 1u,
		EPOLLPRI = 2u,
		EPOLLOUT = 4u,
		EPOLLRDNORM = 0x40u,
		EPOLLRDBAND = 0x80u,
		EPOLLWRNORM = 0x100u,
		EPOLLWRBAND = 0x200u,
		EPOLLMSG = 0x400u,
		EPOLLERR = 8u,
		EPOLLHUP = 0x10u,
		EPOLLRDHUP = 0x2000u,
		EPOLLONESHOT = 0x40000000u,
		EPOLLET = 0x80000000u
	}
}
