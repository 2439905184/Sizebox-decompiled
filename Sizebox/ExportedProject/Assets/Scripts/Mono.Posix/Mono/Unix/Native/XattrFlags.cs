using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum XattrFlags
	{
		XATTR_AUTO = 0,
		XATTR_CREATE = 1,
		XATTR_REPLACE = 2
	}
}
