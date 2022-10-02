using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum PathconfName
	{
		_PC_LINK_MAX = 0,
		_PC_MAX_CANON = 1,
		_PC_MAX_INPUT = 2,
		_PC_NAME_MAX = 3,
		_PC_PATH_MAX = 4,
		_PC_PIPE_BUF = 5,
		_PC_CHOWN_RESTRICTED = 6,
		_PC_NO_TRUNC = 7,
		_PC_VDISABLE = 8,
		_PC_SYNC_IO = 9,
		_PC_ASYNC_IO = 10,
		_PC_PRIO_IO = 11,
		_PC_SOCK_MAXBUF = 12,
		_PC_FILESIZEBITS = 13,
		_PC_REC_INCR_XFER_SIZE = 14,
		_PC_REC_MAX_XFER_SIZE = 15,
		_PC_REC_MIN_XFER_SIZE = 16,
		_PC_REC_XFER_ALIGN = 17,
		_PC_ALLOC_SIZE_MIN = 18,
		_PC_SYMLINK_MAX = 19,
		_PC_2_SYMLINKS = 20
	}
}
