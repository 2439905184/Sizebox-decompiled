using System;
using System.IO;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class FileHandleOperations
	{
		private FileHandleOperations()
		{
		}

		public static void AdviseFileAccessPattern(int fd, FileAccessPattern pattern, long offset, long len)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.posix_fadvise(fd, offset, len, (PosixFadviseAdvice)pattern));
		}

		public static void AdviseFileAccessPattern(int fd, FileAccessPattern pattern)
		{
			AdviseFileAccessPattern(fd, pattern, 0L, 0L);
		}

		public static void AdviseFileAccessPattern(FileStream file, FileAccessPattern pattern, long offset, long len)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.posix_fadvise(file.Handle.ToInt32(), offset, len, (PosixFadviseAdvice)pattern));
		}

		public static void AdviseFileAccessPattern(FileStream file, FileAccessPattern pattern)
		{
			AdviseFileAccessPattern(file, pattern, 0L, 0L);
		}

		public static void AdviseFileAccessPattern(UnixStream stream, FileAccessPattern pattern, long offset, long len)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.posix_fadvise(stream.Handle, offset, len, (PosixFadviseAdvice)pattern));
		}

		public static void AdviseFileAccessPattern(UnixStream stream, FileAccessPattern pattern)
		{
			AdviseFileAccessPattern(stream, pattern, 0L, 0L);
		}
	}
}
