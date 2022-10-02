using System;

namespace Mono.Unix.Native
{
	[Flags]
	[Map]
	[CLSCompliant(false)]
	public enum MessageFlags
	{
		MSG_OOB = 1,
		MSG_PEEK = 2,
		MSG_DONTROUTE = 4,
		MSG_CTRUNC = 8,
		MSG_PROXY = 0x10,
		MSG_TRUNC = 0x20,
		MSG_DONTWAIT = 0x40,
		MSG_EOR = 0x80,
		MSG_WAITALL = 0x100,
		MSG_FIN = 0x200,
		MSG_SYN = 0x400,
		MSG_CONFIRM = 0x800,
		MSG_RST = 0x1000,
		MSG_ERRQUEUE = 0x2000,
		MSG_NOSIGNAL = 0x4000,
		MSG_MORE = 0x8000,
		MSG_WAITFORONE = 0x10000,
		MSG_FASTOPEN = 0x20000000,
		MSG_CMSG_CLOEXEC = 0x40000000
	}
}
