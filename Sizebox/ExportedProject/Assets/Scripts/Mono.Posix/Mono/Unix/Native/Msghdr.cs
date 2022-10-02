using System;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class Msghdr
	{
		public Sockaddr msg_name;

		public Iovec[] msg_iov;

		public int msg_iovlen;

		public byte[] msg_control;

		public long msg_controllen;

		public MessageFlags msg_flags;
	}
}
