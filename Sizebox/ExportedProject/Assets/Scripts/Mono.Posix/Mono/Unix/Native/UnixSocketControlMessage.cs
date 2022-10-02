using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum UnixSocketControlMessage
	{
		SCM_RIGHTS = 1,
		SCM_CREDENTIALS = 2
	}
}
