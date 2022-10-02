using System;

namespace Mono.Unix.Native
{
	[Map("struct iovec")]
	public struct Iovec
	{
		public IntPtr iov_base;

		[CLSCompliant(false)]
		public ulong iov_len;
	}
}
