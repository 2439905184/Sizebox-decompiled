using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum OpenFlags
	{
		O_RDONLY = 0,
		O_WRONLY = 1,
		O_RDWR = 2,
		O_CREAT = 0x40,
		O_EXCL = 0x80,
		O_NOCTTY = 0x100,
		O_TRUNC = 0x200,
		O_APPEND = 0x400,
		O_NONBLOCK = 0x800,
		O_SYNC = 0x1000,
		O_NOFOLLOW = 0x20000,
		O_DIRECTORY = 0x10000,
		O_DIRECT = 0x4000,
		O_ASYNC = 0x2000,
		O_LARGEFILE = 0x8000,
		O_CLOEXEC = 0x80000,
		O_PATH = 0x200000
	}
}
