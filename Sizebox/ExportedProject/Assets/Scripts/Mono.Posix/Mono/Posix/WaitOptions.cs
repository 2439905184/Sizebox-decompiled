using System;

namespace Mono.Posix
{
	[Flags]
	[CLSCompliant(false)]
	[Obsolete("Use Mono.Unix.Native.WaitOptions")]
	public enum WaitOptions
	{
		WNOHANG = 0,
		WUNTRACED = 1
	}
}
