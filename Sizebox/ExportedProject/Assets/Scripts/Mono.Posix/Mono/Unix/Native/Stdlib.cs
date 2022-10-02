using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	public class Stdlib
	{
		internal const string LIBC = "msvcrt";

		internal const string MPH = "MonoPosixHelper";

		private static bool versionCheckPerformed;

		private static readonly IntPtr _SIG_DFL;

		private static readonly IntPtr _SIG_ERR;

		private static readonly IntPtr _SIG_IGN;

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_DFL;

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_ERR;

		[CLSCompliant(false)]
		public static readonly SignalHandler SIG_IGN;

		[CLSCompliant(false)]
		public static readonly int _IOFBF;

		[CLSCompliant(false)]
		public static readonly int _IOLBF;

		[CLSCompliant(false)]
		public static readonly int _IONBF;

		[CLSCompliant(false)]
		public static readonly int BUFSIZ;

		[CLSCompliant(false)]
		public static readonly int EOF;

		[CLSCompliant(false)]
		public static readonly int FOPEN_MAX;

		[CLSCompliant(false)]
		public static readonly int FILENAME_MAX;

		[CLSCompliant(false)]
		public static readonly int L_tmpnam;

		public static readonly IntPtr stderr;

		public static readonly IntPtr stdin;

		public static readonly IntPtr stdout;

		[CLSCompliant(false)]
		public static readonly int TMP_MAX;

		private static object tmpnam_lock;

		[CLSCompliant(false)]
		public static readonly int EXIT_FAILURE;

		[CLSCompliant(false)]
		public static readonly int EXIT_SUCCESS;

		[CLSCompliant(false)]
		public static readonly int MB_CUR_MAX;

		[CLSCompliant(false)]
		public static readonly int RAND_MAX;

		private static object strerror_lock;

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_VersionString")]
		private static extern IntPtr VersionStringPtr();

		internal static void VersionCheck()
		{
			if (!versionCheckPerformed)
			{
				string text = "MonoProject-2015-12-1";
				string text2 = Marshal.PtrToStringAnsi(VersionStringPtr());
				if (text != text2)
				{
					throw new Exception("Mono.Posix assembly loaded with a different version (\"" + text + "\") than MonoPosixHelper (\"" + text2 + "\"). You may need to reinstall Mono.Posix.");
				}
				versionCheckPerformed = true;
			}
		}

		static Stdlib()
		{
			versionCheckPerformed = false;
			_SIG_DFL = GetDefaultSignal();
			_SIG_ERR = GetErrorSignal();
			_SIG_IGN = GetIgnoreSignal();
			SIG_DFL = _DefaultHandler;
			SIG_ERR = _ErrorHandler;
			SIG_IGN = _IgnoreHandler;
			_IOFBF = GetFullyBuffered();
			_IOLBF = GetLineBuffered();
			_IONBF = GetNonBuffered();
			BUFSIZ = GetBufferSize();
			EOF = GetEOF();
			FOPEN_MAX = GetFopenMax();
			FILENAME_MAX = GetFilenameMax();
			L_tmpnam = GetTmpnamLength();
			stderr = GetStandardError();
			stdin = GetStandardInput();
			stdout = GetStandardOutput();
			TMP_MAX = GetTmpMax();
			tmpnam_lock = new object();
			EXIT_FAILURE = GetExitFailure();
			EXIT_SUCCESS = GetExitSuccess();
			MB_CUR_MAX = GetMbCurMax();
			RAND_MAX = GetRandMax();
			strerror_lock = new object();
			VersionCheck();
		}

		internal Stdlib()
		{
		}

		public static Errno GetLastError()
		{
			int value = Marshal.GetLastWin32Error();
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				value = _GetLastError();
			}
			return NativeConvert.ToErrno(value);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_GetLastError")]
		private static extern int _GetLastError();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SetLastError")]
		private static extern void SetLastError(int error);

		protected static void SetLastError(Errno error)
		{
			SetLastError(NativeConvert.FromErrno(error));
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_InvokeSignalHandler")]
		internal static extern void InvokeSignalHandler(int signum, IntPtr handler);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_DFL")]
		private static extern IntPtr GetDefaultSignal();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_ERR")]
		private static extern IntPtr GetErrorSignal();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_SIG_IGN")]
		private static extern IntPtr GetIgnoreSignal();

		private static void _ErrorHandler(int signum)
		{
			Console.Error.WriteLine("Error handler invoked for signum " + signum + ".  Don't do that.");
		}

		private static void _DefaultHandler(int signum)
		{
			Console.Error.WriteLine("Default handler invoked for signum " + signum + ".  Don't do that.");
		}

		private static void _IgnoreHandler(int signum)
		{
			Console.Error.WriteLine("Ignore handler invoked for signum " + signum + ".  Don't do that.");
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "signal", SetLastError = true)]
		private static extern IntPtr sys_signal(int signum, SignalHandler handler);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "signal", SetLastError = true)]
		private static extern IntPtr sys_signal(int signum, IntPtr handler);

		[CLSCompliant(false)]
		[Obsolete("This is not safe; use Mono.Unix.UnixSignal for signal delivery or SetSignalAction()")]
		public static SignalHandler signal(Signum signum, SignalHandler handler)
		{
			int signum2 = NativeConvert.FromSignum(signum);
			Delegate[] invocationList = handler.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Marshal.Prelink(invocationList[i].Method);
			}
			IntPtr handler2 = ((handler == SIG_DFL) ? sys_signal(signum2, _SIG_DFL) : ((handler == SIG_ERR) ? sys_signal(signum2, _SIG_ERR) : ((!(handler == SIG_IGN)) ? sys_signal(signum2, handler) : sys_signal(signum2, _SIG_IGN))));
			return TranslateHandler(handler2);
		}

		private static SignalHandler TranslateHandler(IntPtr handler)
		{
			if (handler == _SIG_DFL)
			{
				return SIG_DFL;
			}
			if (handler == _SIG_ERR)
			{
				return SIG_ERR;
			}
			if (handler == _SIG_IGN)
			{
				return SIG_IGN;
			}
			return (SignalHandler)Marshal.GetDelegateForFunctionPointer(handler, typeof(SignalHandler));
		}

		public static int SetSignalAction(Signum signal, SignalAction action)
		{
			return SetSignalAction(NativeConvert.FromSignum(signal), action);
		}

		public static int SetSignalAction(RealTimeSignum rts, SignalAction action)
		{
			return SetSignalAction(NativeConvert.FromRealTimeSignum(rts), action);
		}

		private static int SetSignalAction(int signum, SignalAction action)
		{
			IntPtr zero = IntPtr.Zero;
			switch (action)
			{
			case SignalAction.Default:
				zero = _SIG_DFL;
				break;
			case SignalAction.Ignore:
				zero = _SIG_IGN;
				break;
			case SignalAction.Error:
				zero = _SIG_ERR;
				break;
			default:
				throw new ArgumentException("Invalid action value.", "action");
			}
			if (sys_signal(signum, zero) == _SIG_ERR)
			{
				return -1;
			}
			return 0;
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "raise")]
		private static extern int sys_raise(int sig);

		[CLSCompliant(false)]
		public static int raise(Signum sig)
		{
			return sys_raise(NativeConvert.FromSignum(sig));
		}

		public static int raise(RealTimeSignum rts)
		{
			return sys_raise(NativeConvert.FromRealTimeSignum(rts));
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IOFBF")]
		private static extern int GetFullyBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IOLBF")]
		private static extern int GetLineBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib__IONBF")]
		private static extern int GetNonBuffered();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_BUFSIZ")]
		private static extern int GetBufferSize();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_CreateFilePosition")]
		internal static extern IntPtr CreateFilePosition();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_DumpFilePosition")]
		internal static extern int DumpFilePosition(StringBuilder buf, HandleRef handle, int len);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EOF")]
		private static extern int GetEOF();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_FILENAME_MAX")]
		private static extern int GetFilenameMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_FOPEN_MAX")]
		private static extern int GetFopenMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_L_tmpnam")]
		private static extern int GetTmpnamLength();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stdin")]
		private static extern IntPtr GetStandardInput();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stdout")]
		private static extern IntPtr GetStandardOutput();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_stderr")]
		private static extern IntPtr GetStandardError();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_TMP_MAX")]
		private static extern int GetTmpMax();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int remove([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string filename);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int rename([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_tmpfile", SetLastError = true)]
		public static extern IntPtr tmpfile();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tmpnam", SetLastError = true)]
		private static extern IntPtr sys_tmpnam(StringBuilder s);

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam(StringBuilder s)
		{
			if (s != null && s.Capacity < L_tmpnam)
			{
				throw new ArgumentOutOfRangeException("s", "s.Capacity < L_tmpnam");
			}
			lock (tmpnam_lock)
			{
				return UnixMarshal.PtrToString(sys_tmpnam(s));
			}
		}

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam()
		{
			lock (tmpnam_lock)
			{
				return UnixMarshal.PtrToString(sys_tmpnam(null));
			}
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fclose", SetLastError = true)]
		public static extern int fclose(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fflush", SetLastError = true)]
		public static extern int fflush(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fopen", SetLastError = true)]
		public static extern IntPtr fopen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, string mode);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_freopen", SetLastError = true)]
		public static extern IntPtr freopen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, string mode, IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_setbuf", SetLastError = true)]
		public static extern int setbuf(IntPtr stream, IntPtr buf);

		[CLSCompliant(false)]
		public unsafe static int setbuf(IntPtr stream, byte* buf)
		{
			return setbuf(stream, (IntPtr)buf);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_setvbuf", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern int setvbuf(IntPtr stream, IntPtr buf, int mode, ulong size);

		[CLSCompliant(false)]
		public unsafe static int setvbuf(IntPtr stream, byte* buf, int mode, ulong size)
		{
			return setvbuf(stream, (IntPtr)buf, mode, size);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fprintf")]
		private static extern int sys_fprintf(IntPtr stream, string format, string message);

		public static int fprintf(IntPtr stream, string message)
		{
			return sys_fprintf(stream, "%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse fprintf (IntPtr, string) instead.")]
		public static int fprintf(IntPtr stream, string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = stream;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.fprintf(array);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "printf")]
		private static extern int sys_printf(string format, string message);

		public static int printf(string message)
		{
			return sys_printf("%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse printf (string) instead.")]
		public static int printf(string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 1)];
			array[0] = format;
			Array.Copy(parameters, 0, array, 1, parameters.Length);
			return (int)XPrintfFunctions.printf(array);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_snprintf")]
		private static extern int sys_snprintf(StringBuilder s, ulong n, string format, string message);

		[CLSCompliant(false)]
		public static int snprintf(StringBuilder s, ulong n, string message)
		{
			if (n > (ulong)s.Capacity)
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			return sys_snprintf(s, n, "%s", message);
		}

		public static int snprintf(StringBuilder s, string message)
		{
			return sys_snprintf(s, (ulong)s.Capacity, "%s", message);
		}

		[CLSCompliant(false)]
		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		public static int snprintf(StringBuilder s, ulong n, string format, params object[] parameters)
		{
			if (n > (ulong)s.Capacity)
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			object[] array = new object[checked(parameters.Length + 3)];
			array[0] = s;
			array[1] = n;
			array[2] = format;
			Array.Copy(parameters, 0, array, 3, parameters.Length);
			return (int)XPrintfFunctions.snprintf(array);
		}

		[CLSCompliant(false)]
		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		public static int snprintf(StringBuilder s, string format, params object[] parameters)
		{
			object[] array = new object[checked(parameters.Length + 3)];
			array[0] = s;
			array[1] = (ulong)s.Capacity;
			array[2] = format;
			Array.Copy(parameters, 0, array, 3, parameters.Length);
			return (int)XPrintfFunctions.snprintf(array);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fgetc", SetLastError = true)]
		public static extern int fgetc(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fgets", SetLastError = true)]
		private static extern IntPtr sys_fgets(StringBuilder sb, int size, IntPtr stream);

		public static StringBuilder fgets(StringBuilder sb, int size, IntPtr stream)
		{
			if (sys_fgets(sb, size, stream) == IntPtr.Zero)
			{
				return null;
			}
			return sb;
		}

		public static StringBuilder fgets(StringBuilder sb, IntPtr stream)
		{
			return fgets(sb, sb.Capacity, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fputc", SetLastError = true)]
		public static extern int fputc(int c, IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fputs", SetLastError = true)]
		public static extern int fputs(string s, IntPtr stream);

		public static int getc(IntPtr stream)
		{
			return fgetc(stream);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int getchar();

		public static int putc(int c, IntPtr stream)
		{
			return fputc(c, stream);
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int putchar(int c);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern int puts(string s);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_ungetc", SetLastError = true)]
		public static extern int ungetc(int c, IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fread", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern ulong fread(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public unsafe static ulong fread(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return fread((IntPtr)ptr, size, nmemb, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fread", SetLastError = true)]
		private static extern ulong sys_fread([Out] byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (ulong)ptr.Length)
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return sys_fread(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, IntPtr stream)
		{
			return fread(ptr, 1uL, (ulong)ptr.Length, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fwrite", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern ulong fwrite(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public unsafe static ulong fwrite(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return fwrite((IntPtr)ptr, size, nmemb, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fwrite", SetLastError = true)]
		private static extern ulong sys_fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (ulong)ptr.Length)
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return sys_fwrite(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, IntPtr stream)
		{
			return fwrite(ptr, 1uL, (ulong)ptr.Length, stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fgetpos", SetLastError = true)]
		private static extern int sys_fgetpos(IntPtr stream, HandleRef pos);

		public static int fgetpos(IntPtr stream, FilePosition pos)
		{
			return sys_fgetpos(stream, pos.Handle);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fseek", SetLastError = true)]
		private static extern int sys_fseek(IntPtr stream, long offset, int origin);

		[CLSCompliant(false)]
		public static int fseek(IntPtr stream, long offset, SeekFlags origin)
		{
			int origin2 = NativeConvert.FromSeekFlags(origin);
			return sys_fseek(stream, offset, origin2);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_fsetpos", SetLastError = true)]
		private static extern int sys_fsetpos(IntPtr stream, HandleRef pos);

		public static int fsetpos(IntPtr stream, FilePosition pos)
		{
			return sys_fsetpos(stream, pos.Handle);
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_ftell", SetLastError = true)]
		public static extern long ftell(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_rewind", SetLastError = true)]
		public static extern int rewind(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_clearerr", SetLastError = true)]
		public static extern int clearerr(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_feof", SetLastError = true)]
		public static extern int feof(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_ferror", SetLastError = true)]
		public static extern int ferror(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_perror", SetLastError = true)]
		private static extern int perror(string s, int err);

		public static int perror(string s)
		{
			return perror(s, Marshal.GetLastWin32Error());
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EXIT_FAILURE")]
		private static extern int GetExitFailure();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_EXIT_SUCCESS")]
		private static extern int GetExitSuccess();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_MB_CUR_MAX")]
		private static extern int GetMbCurMax();

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_RAND_MAX")]
		private static extern int GetRandMax();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern int rand();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		[CLSCompliant(false)]
		public static extern void srand(uint seed);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_calloc", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern IntPtr calloc(ulong nmemb, ulong size);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_free")]
		public static extern void free(IntPtr ptr);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_malloc", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern IntPtr malloc(ulong size);

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_realloc", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern IntPtr realloc(IntPtr ptr, ulong size);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void abort();

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		public static extern void exit(int status);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
		[CLSCompliant(false)]
		public static extern void _Exit(int status);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "getenv")]
		private static extern IntPtr sys_getenv(string name);

		public static string getenv(string name)
		{
			return UnixMarshal.PtrToString(sys_getenv(name));
		}

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		[CLSCompliant(false)]
		public static extern int system(string @string);

		[DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "strerror", SetLastError = true)]
		private static extern IntPtr sys_strerror(int errnum);

		[CLSCompliant(false)]
		public static string strerror(Errno errnum)
		{
			int errnum2 = NativeConvert.FromErrno(errnum);
			lock (strerror_lock)
			{
				return UnixMarshal.PtrToString(sys_strerror(errnum2));
			}
		}

		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_Stdlib_strlen", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern ulong strlen(IntPtr s);
	}
}
