using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public class StdioFileStream : Stream
	{
		public static readonly IntPtr InvalidFileStream = IntPtr.Zero;

		public static readonly IntPtr StandardInput = Stdlib.stdin;

		public static readonly IntPtr StandardOutput = Stdlib.stdout;

		public static readonly IntPtr StandardError = Stdlib.stderr;

		private bool canSeek;

		private bool canRead;

		private bool canWrite;

		private bool owner = true;

		private IntPtr file = InvalidFileStream;

		public IntPtr Handle
		{
			get
			{
				AssertNotDisposed();
				GC.KeepAlive(this);
				return file;
			}
		}

		public override bool CanRead
		{
			get
			{
				return canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return canSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return canWrite;
			}
		}

		public override long Length
		{
			get
			{
				AssertNotDisposed();
				if (!CanSeek)
				{
					throw new NotSupportedException("File Stream doesn't support seeking");
				}
				long num = Stdlib.ftell(file);
				if (num == -1)
				{
					throw new NotSupportedException("Unable to obtain current file position");
				}
				UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fseek(file, 0L, SeekFlags.SEEK_END));
				long num2 = Stdlib.ftell(file);
				if (num2 == -1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fseek(file, num, SeekFlags.SEEK_SET));
				GC.KeepAlive(this);
				return num2;
			}
		}

		public override long Position
		{
			get
			{
				AssertNotDisposed();
				if (!CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				long num = Stdlib.ftell(file);
				if (num == -1)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				GC.KeepAlive(this);
				return num;
			}
			set
			{
				AssertNotDisposed();
				Seek(value, SeekOrigin.Begin);
			}
		}

		public StdioFileStream(IntPtr fileStream)
			: this(fileStream, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, bool ownsHandle)
		{
			InitStream(fileStream, ownsHandle);
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access)
			: this(fileStream, access, true)
		{
		}

		public StdioFileStream(IntPtr fileStream, FileAccess access, bool ownsHandle)
		{
			InitStream(fileStream, ownsHandle);
			InitCanReadWrite(access);
		}

		public StdioFileStream(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InitStream(Fopen(path, "rb"), true);
		}

		public StdioFileStream(string path, string mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InitStream(Fopen(path, mode), true);
		}

		public StdioFileStream(string path, FileMode mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InitStream(Fopen(path, ToFopenMode(path, mode)), true);
		}

		public StdioFileStream(string path, FileAccess access)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InitStream(Fopen(path, ToFopenMode(path, access)), true);
			InitCanReadWrite(access);
		}

		public StdioFileStream(string path, FileMode mode, FileAccess access)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InitStream(Fopen(path, ToFopenMode(path, mode, access)), true);
			InitCanReadWrite(access);
		}

		private static IntPtr Fopen(string path, string mode)
		{
			if (path.Length == 0)
			{
				throw new ArgumentException("path");
			}
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			IntPtr intPtr = Stdlib.fopen(path, mode);
			if (intPtr == IntPtr.Zero)
			{
				throw new DirectoryNotFoundException("path", UnixMarshal.CreateExceptionForLastError());
			}
			return intPtr;
		}

		private void InitStream(IntPtr fileStream, bool ownsHandle)
		{
			if (InvalidFileStream == fileStream)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			file = fileStream;
			owner = ownsHandle;
			try
			{
				if ((long)Stdlib.fseek(file, 0L, SeekFlags.SEEK_CUR) != -1)
				{
					canSeek = true;
				}
				Stdlib.fread(IntPtr.Zero, 0uL, 0uL, file);
				if (Stdlib.ferror(file) == 0)
				{
					canRead = true;
				}
				Stdlib.fwrite(IntPtr.Zero, 0uL, 0uL, file);
				if (Stdlib.ferror(file) == 0)
				{
					canWrite = true;
				}
				Stdlib.clearerr(file);
			}
			catch (Exception)
			{
				throw new ArgumentException(Locale.GetText("Invalid file stream"), "fileStream");
			}
			GC.KeepAlive(this);
		}

		private void InitCanReadWrite(FileAccess access)
		{
			canRead = canRead && (access == FileAccess.Read || access == FileAccess.ReadWrite);
			canWrite = canWrite && (access == FileAccess.Write || access == FileAccess.ReadWrite);
		}

		private static string ToFopenMode(string file, FileMode mode)
		{
			string result = NativeConvert.ToFopenMode(mode);
			AssertFileMode(file, mode);
			return result;
		}

		private static string ToFopenMode(string file, FileAccess access)
		{
			return NativeConvert.ToFopenMode(access);
		}

		private static string ToFopenMode(string file, FileMode mode, FileAccess access)
		{
			string result = NativeConvert.ToFopenMode(mode, access);
			bool flag = AssertFileMode(file, mode);
			if (mode == FileMode.OpenOrCreate && access == FileAccess.Read && !flag)
			{
				result = "w+b";
			}
			return result;
		}

		private static bool AssertFileMode(string file, FileMode mode)
		{
			bool flag = FileExists(file);
			if (mode == FileMode.CreateNew && flag)
			{
				throw new IOException("File exists and FileMode.CreateNew specified");
			}
			if ((mode == FileMode.Open || mode == FileMode.Truncate) && !flag)
			{
				throw new FileNotFoundException("File doesn't exist and FileMode.Open specified", file);
			}
			return flag;
		}

		private static bool FileExists(string file)
		{
			IntPtr intPtr = Stdlib.fopen(file, "r");
			bool result = intPtr != IntPtr.Zero;
			if (intPtr != IntPtr.Zero)
			{
				Stdlib.fclose(intPtr);
			}
			return result;
		}

		private void AssertNotDisposed()
		{
			if (file == InvalidFileStream)
			{
				throw new ObjectDisposedException("Invalid File Stream");
			}
			GC.KeepAlive(this);
		}

		public void SaveFilePosition(FilePosition pos)
		{
			AssertNotDisposed();
			UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fgetpos(file, pos));
			GC.KeepAlive(this);
		}

		public void RestoreFilePosition(FilePosition pos)
		{
			AssertNotDisposed();
			if (pos == null)
			{
				throw new ArgumentNullException("value");
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Stdlib.fsetpos(file, pos));
			GC.KeepAlive(this);
		}

		public override void Flush()
		{
			AssertNotDisposed();
			if (Stdlib.fflush(file) != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			GC.KeepAlive(this);
		}

		public unsafe override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			AssertNotDisposed();
			AssertValidBuffer(buffer, offset, count);
			if (!CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			ulong num;
			fixed (byte* ptr = &buffer[offset])
			{
				num = Stdlib.fread(ptr, 1uL, (ulong)count, file);
			}
			if (num != (ulong)count && Stdlib.ferror(file) != 0)
			{
				throw new IOException();
			}
			GC.KeepAlive(this);
			return (int)num;
		}

		private void AssertValidBuffer(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("would overrun buffer");
			}
		}

		public void Rewind()
		{
			AssertNotDisposed();
			Stdlib.rewind(file);
			GC.KeepAlive(this);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			AssertNotDisposed();
			if (!CanSeek)
			{
				throw new NotSupportedException("The File Stream does not support seeking");
			}
			SeekFlags seekFlags = SeekFlags.SEEK_CUR;
			switch (origin)
			{
			case SeekOrigin.Begin:
				seekFlags = SeekFlags.SEEK_SET;
				break;
			case SeekOrigin.Current:
				seekFlags = SeekFlags.SEEK_CUR;
				break;
			case SeekOrigin.End:
				seekFlags = SeekFlags.SEEK_END;
				break;
			default:
				throw new ArgumentException("origin");
			}
			if (Stdlib.fseek(file, offset, seekFlags) != 0)
			{
				throw new IOException("Unable to seek", UnixMarshal.CreateExceptionForLastError());
			}
			long num = Stdlib.ftell(file);
			if (num == -1)
			{
				throw new IOException("Unable to get current file position", UnixMarshal.CreateExceptionForLastError());
			}
			GC.KeepAlive(this);
			return num;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("ANSI C doesn't provide a way to truncate a file");
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			AssertNotDisposed();
			AssertValidBuffer(buffer, offset, count);
			if (!CanWrite)
			{
				throw new NotSupportedException("File Stream does not support writing");
			}
			ulong num;
			fixed (byte* ptr = &buffer[offset])
			{
				num = Stdlib.fwrite(ptr, 1uL, (ulong)count, file);
			}
			if (num != (ulong)count)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			GC.KeepAlive(this);
		}

		~StdioFileStream()
		{
			Close();
		}

		public override void Close()
		{
			if (file == InvalidFileStream)
			{
				return;
			}
			if (owner)
			{
				if (Stdlib.fclose(file) != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
			}
			else
			{
				Flush();
			}
			file = InvalidFileStream;
			canRead = false;
			canSeek = false;
			canWrite = false;
			GC.SuppressFinalize(this);
			GC.KeepAlive(this);
		}
	}
}
