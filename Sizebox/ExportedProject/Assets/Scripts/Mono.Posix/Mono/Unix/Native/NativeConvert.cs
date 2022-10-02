using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class NativeConvert
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static readonly DateTime LocalUnixEpoch = new DateTime(1970, 1, 1);

		public static readonly TimeSpan LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.UtcNow);

		private static readonly string[][] fopen_modes = new string[6][]
		{
			new string[3] { "Can't Read+Create", "wb", "w+b" },
			new string[3] { "Can't Read+Create", "wb", "w+b" },
			new string[3] { "rb", "wb", "r+b" },
			new string[3] { "rb", "wb", "r+b" },
			new string[3] { "Cannot Truncate and Read", "wb", "w+b" },
			new string[3] { "Cannot Append and Read", "ab", "a+b" }
		};

		private const string LIB = "MonoPosixHelper";

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromRealTimeSignum")]
		private static extern int FromRealTimeSignum(int offset, out int rval);

		public static int FromRealTimeSignum(RealTimeSignum sig)
		{
			int rval;
			if (FromRealTimeSignum(sig.Offset, out rval) == -1)
			{
				ThrowArgumentException(sig.Offset);
			}
			return rval;
		}

		public static RealTimeSignum ToRealTimeSignum(int offset)
		{
			return new RealTimeSignum(offset);
		}

		public static FilePermissions FromOctalPermissionString(string value)
		{
			return ToFilePermissions(Convert.ToUInt32(value, 8));
		}

		public static string ToOctalPermissionString(FilePermissions value)
		{
			string text = Convert.ToString((int)value & -61441, 8);
			return new string('0', 4 - text.Length) + text;
		}

		public static FilePermissions FromUnixPermissionString(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length != 9 && value.Length != 10)
			{
				throw new ArgumentException("value", "must contain 9 or 10 characters");
			}
			int num = 0;
			FilePermissions filePermissions = (FilePermissions)0u;
			if (value.Length == 10)
			{
				filePermissions |= GetUnixPermissionDevice(value[num]);
				num++;
			}
			filePermissions |= GetUnixPermissionGroup(value[num++], FilePermissions.S_IRUSR, value[num++], FilePermissions.S_IWUSR, value[num++], FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			filePermissions |= GetUnixPermissionGroup(value[num++], FilePermissions.S_IRGRP, value[num++], FilePermissions.S_IWGRP, value[num++], FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			return filePermissions | GetUnixPermissionGroup(value[num++], FilePermissions.S_IROTH, value[num++], FilePermissions.S_IWOTH, value[num++], FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
		}

		private static FilePermissions GetUnixPermissionDevice(char value)
		{
			switch (value)
			{
			case 'd':
				return FilePermissions.S_IFDIR;
			case 'c':
				return FilePermissions.S_IFCHR;
			case 'b':
				return FilePermissions.S_IFBLK;
			case '-':
				return FilePermissions.S_IFREG;
			case 'p':
				return FilePermissions.S_IFIFO;
			case 'l':
				return FilePermissions.S_IFLNK;
			case 's':
				return FilePermissions.S_IFSOCK;
			default:
				throw new ArgumentException("value", "invalid device specification: " + value);
			}
		}

		private static FilePermissions GetUnixPermissionGroup(char read, FilePermissions readb, char write, FilePermissions writeb, char exec, FilePermissions execb, char xboth, char xbitonly, FilePermissions xbit)
		{
			FilePermissions filePermissions = (FilePermissions)0u;
			if (read == 'r')
			{
				filePermissions |= readb;
			}
			if (write == 'w')
			{
				filePermissions |= writeb;
			}
			if (exec == 'x')
			{
				filePermissions |= execb;
			}
			else if (exec == xbitonly)
			{
				filePermissions |= xbit;
			}
			else if (exec == xboth)
			{
				filePermissions |= execb | xbit;
			}
			return filePermissions;
		}

		public static string ToUnixPermissionString(FilePermissions value)
		{
			char[] array = new char[10] { '-', '-', '-', '-', '-', '-', '-', '-', '-', '-' };
			bool flag = true;
			switch (value & FilePermissions.S_IFMT)
			{
			case FilePermissions.S_IFDIR:
				array[0] = 'd';
				break;
			case FilePermissions.S_IFCHR:
				array[0] = 'c';
				break;
			case FilePermissions.S_IFBLK:
				array[0] = 'b';
				break;
			case FilePermissions.S_IFREG:
				array[0] = '-';
				break;
			case FilePermissions.S_IFIFO:
				array[0] = 'p';
				break;
			case FilePermissions.S_IFLNK:
				array[0] = 'l';
				break;
			case FilePermissions.S_IFSOCK:
				array[0] = 's';
				break;
			default:
				flag = false;
				break;
			}
			SetUnixPermissionGroup(value, array, 1, FilePermissions.S_IRUSR, FilePermissions.S_IWUSR, FilePermissions.S_IXUSR, 's', 'S', FilePermissions.S_ISUID);
			SetUnixPermissionGroup(value, array, 4, FilePermissions.S_IRGRP, FilePermissions.S_IWGRP, FilePermissions.S_IXGRP, 's', 'S', FilePermissions.S_ISGID);
			SetUnixPermissionGroup(value, array, 7, FilePermissions.S_IROTH, FilePermissions.S_IWOTH, FilePermissions.S_IXOTH, 't', 'T', FilePermissions.S_ISVTX);
			if (!flag)
			{
				return new string(array, 1, 9);
			}
			return new string(array);
		}

		private static void SetUnixPermissionGroup(FilePermissions value, char[] access, int index, FilePermissions read, FilePermissions write, FilePermissions exec, char both, char setonly, FilePermissions setxbit)
		{
			if (UnixFileSystemInfo.IsSet(value, read))
			{
				access[index] = 'r';
			}
			if (UnixFileSystemInfo.IsSet(value, write))
			{
				access[index + 1] = 'w';
			}
			access[index + 2] = GetSymbolicMode(value, exec, both, setonly, setxbit);
		}

		private static char GetSymbolicMode(FilePermissions value, FilePermissions xbit, char both, char setonly, FilePermissions setxbit)
		{
			bool flag = UnixFileSystemInfo.IsSet(value, xbit);
			bool flag2 = UnixFileSystemInfo.IsSet(value, setxbit);
			if (flag && flag2)
			{
				return both;
			}
			if (flag2)
			{
				return setonly;
			}
			if (flag)
			{
				return 'x';
			}
			return '-';
		}

		public static DateTime ToDateTime(long time)
		{
			return FromTimeT(time);
		}

		public static DateTime ToDateTime(long time, long nanoTime)
		{
			return FromTimeT(time).AddMilliseconds(nanoTime / 1000);
		}

		public static long FromDateTime(DateTime time)
		{
			return ToTimeT(time);
		}

		public static DateTime FromTimeT(long time)
		{
			return UnixEpoch.AddSeconds(time).ToLocalTime();
		}

		public static long ToTimeT(DateTime time)
		{
			if (time.Kind == DateTimeKind.Unspecified)
			{
				throw new ArgumentException("DateTimeKind.Unspecified is not supported. Use Local or Utc times.", "time");
			}
			if (time.Kind == DateTimeKind.Local)
			{
				time = time.ToUniversalTime();
			}
			return (long)(time - UnixEpoch).TotalSeconds;
		}

		public static OpenFlags ToOpenFlags(FileMode mode, FileAccess access)
		{
			OpenFlags openFlags = OpenFlags.O_RDONLY;
			switch (mode)
			{
			case FileMode.CreateNew:
				openFlags = OpenFlags.O_CREAT | OpenFlags.O_EXCL;
				break;
			case FileMode.Create:
				openFlags = OpenFlags.O_CREAT | OpenFlags.O_TRUNC;
				break;
			case FileMode.OpenOrCreate:
				openFlags = OpenFlags.O_CREAT;
				break;
			case FileMode.Truncate:
				openFlags = OpenFlags.O_TRUNC;
				break;
			case FileMode.Append:
				openFlags = OpenFlags.O_APPEND;
				break;
			default:
				throw new ArgumentException(Locale.GetText("Unsupported mode value"), "mode");
			case FileMode.Open:
				break;
			}
			int rval;
			if (TryFromOpenFlags(OpenFlags.O_LARGEFILE, out rval))
			{
				openFlags |= OpenFlags.O_LARGEFILE;
			}
			switch (access)
			{
			case FileAccess.Read:
				return openFlags | OpenFlags.O_RDONLY;
			case FileAccess.Write:
				return openFlags | OpenFlags.O_WRONLY;
			case FileAccess.ReadWrite:
				return openFlags | OpenFlags.O_RDWR;
			default:
				throw new ArgumentOutOfRangeException(Locale.GetText("Unsupported access value"), "access");
			}
		}

		public static string ToFopenMode(FileAccess access)
		{
			switch (access)
			{
			case FileAccess.Read:
				return "rb";
			case FileAccess.Write:
				return "wb";
			case FileAccess.ReadWrite:
				return "r+b";
			default:
				throw new ArgumentOutOfRangeException("access");
			}
		}

		public static string ToFopenMode(FileMode mode)
		{
			switch (mode)
			{
			case FileMode.CreateNew:
			case FileMode.Create:
				return "w+b";
			case FileMode.Open:
			case FileMode.OpenOrCreate:
				return "r+b";
			case FileMode.Truncate:
				return "w+b";
			case FileMode.Append:
				return "a+b";
			default:
				throw new ArgumentOutOfRangeException("mode");
			}
		}

		public static string ToFopenMode(FileMode mode, FileAccess access)
		{
			int num = -1;
			int num2 = -1;
			switch (mode)
			{
			case FileMode.CreateNew:
				num = 0;
				break;
			case FileMode.Create:
				num = 1;
				break;
			case FileMode.Open:
				num = 2;
				break;
			case FileMode.OpenOrCreate:
				num = 3;
				break;
			case FileMode.Truncate:
				num = 4;
				break;
			case FileMode.Append:
				num = 5;
				break;
			}
			switch (access)
			{
			case FileAccess.Read:
				num2 = 0;
				break;
			case FileAccess.Write:
				num2 = 1;
				break;
			case FileAccess.ReadWrite:
				num2 = 2;
				break;
			}
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException("mode");
			}
			if (num2 == -1)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			string text = fopen_modes[num][num2];
			if (text[0] != 'r' && text[0] != 'w' && text[0] != 'a')
			{
				throw new ArgumentException(text);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromStat")]
		private static extern int FromStat(ref Stat source, IntPtr destination);

		public static bool TryCopy(ref Stat source, IntPtr destination)
		{
			return FromStat(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToStat")]
		private static extern int ToStat(IntPtr source, out Stat destination);

		public static bool TryCopy(IntPtr source, out Stat destination)
		{
			return ToStat(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromStatvfs")]
		private static extern int FromStatvfs(ref Statvfs source, IntPtr destination);

		public static bool TryCopy(ref Statvfs source, IntPtr destination)
		{
			return FromStatvfs(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToStatvfs")]
		private static extern int ToStatvfs(IntPtr source, out Statvfs destination);

		public static bool TryCopy(IntPtr source, out Statvfs destination)
		{
			return ToStatvfs(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromInAddr")]
		private static extern int FromInAddr(ref InAddr source, IntPtr destination);

		public static bool TryCopy(ref InAddr source, IntPtr destination)
		{
			return FromInAddr(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToInAddr")]
		private static extern int ToInAddr(IntPtr source, out InAddr destination);

		public static bool TryCopy(IntPtr source, out InAddr destination)
		{
			return ToInAddr(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromIn6Addr")]
		private static extern int FromIn6Addr(ref In6Addr source, IntPtr destination);

		public static bool TryCopy(ref In6Addr source, IntPtr destination)
		{
			return FromIn6Addr(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToIn6Addr")]
		private static extern int ToIn6Addr(IntPtr source, out In6Addr destination);

		public static bool TryCopy(IntPtr source, out In6Addr destination)
		{
			return ToIn6Addr(source, out destination) == 0;
		}

		public static InAddr ToInAddr(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.AddressFamily != AddressFamily.InterNetwork)
			{
				throw new ArgumentException("address", "address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork");
			}
			return new InAddr(address.GetAddressBytes());
		}

		public static IPAddress ToIPAddress(InAddr address)
		{
			byte[] array = new byte[4];
			address.CopyTo(array, 0);
			return new IPAddress(array);
		}

		public static In6Addr ToIn6Addr(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("address", "address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6");
			}
			return new In6Addr(address.GetAddressBytes());
		}

		public static IPAddress ToIPAddress(In6Addr address)
		{
			byte[] array = new byte[16];
			address.CopyTo(array, 0);
			return new IPAddress(array);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSockaddr")]
		private unsafe static extern int FromSockaddr(_SockaddrHeader* source, IntPtr destination);

		public unsafe static bool TryCopy(Sockaddr source, IntPtr destination)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			byte[] dynamicData = Sockaddr.GetDynamicData(source);
			if (source.type == (SockaddrType)32769)
			{
				Marshal.Copy(dynamicData, 0, destination, (int)source.GetDynamicLength());
				return true;
			}
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(source).type)
			{
				fixed (byte* data = dynamicData)
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(source, data, false);
					return FromSockaddr(Sockaddr.GetNative(&sockaddrDynamic, addr), destination) == 0;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSockaddr")]
		private unsafe static extern int ToSockaddr(IntPtr source, long size, _SockaddrHeader* destination);

		public unsafe static bool TryCopy(IntPtr source, long size, Sockaddr destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			byte[] dynamicData = Sockaddr.GetDynamicData(destination);
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(destination).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(destination))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(destination, data, true);
					int num = ToSockaddr(source, size, Sockaddr.GetNative(&sockaddrDynamic, addr));
					sockaddrDynamic.Update(destination);
					if (num == 0 && destination.type == (SockaddrType)32769)
					{
						Marshal.Copy(source, dynamicData, 0, (int)destination.GetDynamicLength());
					}
					return num == 0;
				}
			}
		}

		private NativeConvert()
		{
		}

		private static void ThrowArgumentException(object value)
		{
			throw new ArgumentOutOfRangeException("value", value, Locale.GetText("Current platform doesn't support this value."));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromAccessModes")]
		private static extern int FromAccessModes(AccessModes value, out int rval);

		public static bool TryFromAccessModes(AccessModes value, out int rval)
		{
			return FromAccessModes(value, out rval) == 0;
		}

		public static int FromAccessModes(AccessModes value)
		{
			int rval;
			if (FromAccessModes(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToAccessModes")]
		private static extern int ToAccessModes(int value, out AccessModes rval);

		public static bool TryToAccessModes(int value, out AccessModes rval)
		{
			return ToAccessModes(value, out rval) == 0;
		}

		public static AccessModes ToAccessModes(int value)
		{
			AccessModes rval;
			if (ToAccessModes(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromAtFlags")]
		private static extern int FromAtFlags(AtFlags value, out int rval);

		public static bool TryFromAtFlags(AtFlags value, out int rval)
		{
			return FromAtFlags(value, out rval) == 0;
		}

		public static int FromAtFlags(AtFlags value)
		{
			int rval;
			if (FromAtFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToAtFlags")]
		private static extern int ToAtFlags(int value, out AtFlags rval);

		public static bool TryToAtFlags(int value, out AtFlags rval)
		{
			return ToAtFlags(value, out rval) == 0;
		}

		public static AtFlags ToAtFlags(int value)
		{
			AtFlags rval;
			if (ToAtFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromCmsghdr")]
		private static extern int FromCmsghdr(ref Cmsghdr source, IntPtr destination);

		public static bool TryCopy(ref Cmsghdr source, IntPtr destination)
		{
			return FromCmsghdr(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToCmsghdr")]
		private static extern int ToCmsghdr(IntPtr source, out Cmsghdr destination);

		public static bool TryCopy(IntPtr source, out Cmsghdr destination)
		{
			return ToCmsghdr(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromConfstrName")]
		private static extern int FromConfstrName(ConfstrName value, out int rval);

		public static bool TryFromConfstrName(ConfstrName value, out int rval)
		{
			return FromConfstrName(value, out rval) == 0;
		}

		public static int FromConfstrName(ConfstrName value)
		{
			int rval;
			if (FromConfstrName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToConfstrName")]
		private static extern int ToConfstrName(int value, out ConfstrName rval);

		public static bool TryToConfstrName(int value, out ConfstrName rval)
		{
			return ToConfstrName(value, out rval) == 0;
		}

		public static ConfstrName ToConfstrName(int value)
		{
			ConfstrName rval;
			if (ToConfstrName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromDirectoryNotifyFlags")]
		private static extern int FromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval);

		public static bool TryFromDirectoryNotifyFlags(DirectoryNotifyFlags value, out int rval)
		{
			return FromDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static int FromDirectoryNotifyFlags(DirectoryNotifyFlags value)
		{
			int rval;
			if (FromDirectoryNotifyFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToDirectoryNotifyFlags")]
		private static extern int ToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval);

		public static bool TryToDirectoryNotifyFlags(int value, out DirectoryNotifyFlags rval)
		{
			return ToDirectoryNotifyFlags(value, out rval) == 0;
		}

		public static DirectoryNotifyFlags ToDirectoryNotifyFlags(int value)
		{
			DirectoryNotifyFlags rval;
			if (ToDirectoryNotifyFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromEpollEvents")]
		private static extern int FromEpollEvents(EpollEvents value, out uint rval);

		public static bool TryFromEpollEvents(EpollEvents value, out uint rval)
		{
			return FromEpollEvents(value, out rval) == 0;
		}

		public static uint FromEpollEvents(EpollEvents value)
		{
			uint rval;
			if (FromEpollEvents(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToEpollEvents")]
		private static extern int ToEpollEvents(uint value, out EpollEvents rval);

		public static bool TryToEpollEvents(uint value, out EpollEvents rval)
		{
			return ToEpollEvents(value, out rval) == 0;
		}

		public static EpollEvents ToEpollEvents(uint value)
		{
			EpollEvents rval;
			if (ToEpollEvents(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromEpollFlags")]
		private static extern int FromEpollFlags(EpollFlags value, out int rval);

		public static bool TryFromEpollFlags(EpollFlags value, out int rval)
		{
			return FromEpollFlags(value, out rval) == 0;
		}

		public static int FromEpollFlags(EpollFlags value)
		{
			int rval;
			if (FromEpollFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToEpollFlags")]
		private static extern int ToEpollFlags(int value, out EpollFlags rval);

		public static bool TryToEpollFlags(int value, out EpollFlags rval)
		{
			return ToEpollFlags(value, out rval) == 0;
		}

		public static EpollFlags ToEpollFlags(int value)
		{
			EpollFlags rval;
			if (ToEpollFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromErrno")]
		private static extern int FromErrno(Errno value, out int rval);

		public static bool TryFromErrno(Errno value, out int rval)
		{
			return FromErrno(value, out rval) == 0;
		}

		public static int FromErrno(Errno value)
		{
			int rval;
			if (FromErrno(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToErrno")]
		private static extern int ToErrno(int value, out Errno rval);

		public static bool TryToErrno(int value, out Errno rval)
		{
			return ToErrno(value, out rval) == 0;
		}

		public static Errno ToErrno(int value)
		{
			Errno rval;
			if (ToErrno(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFcntlCommand")]
		private static extern int FromFcntlCommand(FcntlCommand value, out int rval);

		public static bool TryFromFcntlCommand(FcntlCommand value, out int rval)
		{
			return FromFcntlCommand(value, out rval) == 0;
		}

		public static int FromFcntlCommand(FcntlCommand value)
		{
			int rval;
			if (FromFcntlCommand(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFcntlCommand")]
		private static extern int ToFcntlCommand(int value, out FcntlCommand rval);

		public static bool TryToFcntlCommand(int value, out FcntlCommand rval)
		{
			return ToFcntlCommand(value, out rval) == 0;
		}

		public static FcntlCommand ToFcntlCommand(int value)
		{
			FcntlCommand rval;
			if (ToFcntlCommand(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFilePermissions")]
		private static extern int FromFilePermissions(FilePermissions value, out uint rval);

		public static bool TryFromFilePermissions(FilePermissions value, out uint rval)
		{
			return FromFilePermissions(value, out rval) == 0;
		}

		public static uint FromFilePermissions(FilePermissions value)
		{
			uint rval;
			if (FromFilePermissions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFilePermissions")]
		private static extern int ToFilePermissions(uint value, out FilePermissions rval);

		public static bool TryToFilePermissions(uint value, out FilePermissions rval)
		{
			return ToFilePermissions(value, out rval) == 0;
		}

		public static FilePermissions ToFilePermissions(uint value)
		{
			FilePermissions rval;
			if (ToFilePermissions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFlock")]
		private static extern int FromFlock(ref Flock source, IntPtr destination);

		public static bool TryCopy(ref Flock source, IntPtr destination)
		{
			return FromFlock(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToFlock")]
		private static extern int ToFlock(IntPtr source, out Flock destination);

		public static bool TryCopy(IntPtr source, out Flock destination)
		{
			return ToFlock(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromIovec")]
		private static extern int FromIovec(ref Iovec source, IntPtr destination);

		public static bool TryCopy(ref Iovec source, IntPtr destination)
		{
			return FromIovec(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToIovec")]
		private static extern int ToIovec(IntPtr source, out Iovec destination);

		public static bool TryCopy(IntPtr source, out Iovec destination)
		{
			return ToIovec(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromLinger")]
		private static extern int FromLinger(ref Linger source, IntPtr destination);

		public static bool TryCopy(ref Linger source, IntPtr destination)
		{
			return FromLinger(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToLinger")]
		private static extern int ToLinger(IntPtr source, out Linger destination);

		public static bool TryCopy(IntPtr source, out Linger destination)
		{
			return ToLinger(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromLockType")]
		private static extern int FromLockType(LockType value, out short rval);

		public static bool TryFromLockType(LockType value, out short rval)
		{
			return FromLockType(value, out rval) == 0;
		}

		public static short FromLockType(LockType value)
		{
			short rval;
			if (FromLockType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToLockType")]
		private static extern int ToLockType(short value, out LockType rval);

		public static bool TryToLockType(short value, out LockType rval)
		{
			return ToLockType(value, out rval) == 0;
		}

		public static LockType ToLockType(short value)
		{
			LockType rval;
			if (ToLockType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromLockfCommand")]
		private static extern int FromLockfCommand(LockfCommand value, out int rval);

		public static bool TryFromLockfCommand(LockfCommand value, out int rval)
		{
			return FromLockfCommand(value, out rval) == 0;
		}

		public static int FromLockfCommand(LockfCommand value)
		{
			int rval;
			if (FromLockfCommand(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToLockfCommand")]
		private static extern int ToLockfCommand(int value, out LockfCommand rval);

		public static bool TryToLockfCommand(int value, out LockfCommand rval)
		{
			return ToLockfCommand(value, out rval) == 0;
		}

		public static LockfCommand ToLockfCommand(int value)
		{
			LockfCommand rval;
			if (ToLockfCommand(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMessageFlags")]
		private static extern int FromMessageFlags(MessageFlags value, out int rval);

		public static bool TryFromMessageFlags(MessageFlags value, out int rval)
		{
			return FromMessageFlags(value, out rval) == 0;
		}

		public static int FromMessageFlags(MessageFlags value)
		{
			int rval;
			if (FromMessageFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMessageFlags")]
		private static extern int ToMessageFlags(int value, out MessageFlags rval);

		public static bool TryToMessageFlags(int value, out MessageFlags rval)
		{
			return ToMessageFlags(value, out rval) == 0;
		}

		public static MessageFlags ToMessageFlags(int value)
		{
			MessageFlags rval;
			if (ToMessageFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMlockallFlags")]
		private static extern int FromMlockallFlags(MlockallFlags value, out int rval);

		public static bool TryFromMlockallFlags(MlockallFlags value, out int rval)
		{
			return FromMlockallFlags(value, out rval) == 0;
		}

		public static int FromMlockallFlags(MlockallFlags value)
		{
			int rval;
			if (FromMlockallFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMlockallFlags")]
		private static extern int ToMlockallFlags(int value, out MlockallFlags rval);

		public static bool TryToMlockallFlags(int value, out MlockallFlags rval)
		{
			return ToMlockallFlags(value, out rval) == 0;
		}

		public static MlockallFlags ToMlockallFlags(int value)
		{
			MlockallFlags rval;
			if (ToMlockallFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMmapFlags")]
		private static extern int FromMmapFlags(MmapFlags value, out int rval);

		public static bool TryFromMmapFlags(MmapFlags value, out int rval)
		{
			return FromMmapFlags(value, out rval) == 0;
		}

		public static int FromMmapFlags(MmapFlags value)
		{
			int rval;
			if (FromMmapFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMmapFlags")]
		private static extern int ToMmapFlags(int value, out MmapFlags rval);

		public static bool TryToMmapFlags(int value, out MmapFlags rval)
		{
			return ToMmapFlags(value, out rval) == 0;
		}

		public static MmapFlags ToMmapFlags(int value)
		{
			MmapFlags rval;
			if (ToMmapFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMmapProts")]
		private static extern int FromMmapProts(MmapProts value, out int rval);

		public static bool TryFromMmapProts(MmapProts value, out int rval)
		{
			return FromMmapProts(value, out rval) == 0;
		}

		public static int FromMmapProts(MmapProts value)
		{
			int rval;
			if (FromMmapProts(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMmapProts")]
		private static extern int ToMmapProts(int value, out MmapProts rval);

		public static bool TryToMmapProts(int value, out MmapProts rval)
		{
			return ToMmapProts(value, out rval) == 0;
		}

		public static MmapProts ToMmapProts(int value)
		{
			MmapProts rval;
			if (ToMmapProts(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMountFlags")]
		private static extern int FromMountFlags(MountFlags value, out ulong rval);

		public static bool TryFromMountFlags(MountFlags value, out ulong rval)
		{
			return FromMountFlags(value, out rval) == 0;
		}

		public static ulong FromMountFlags(MountFlags value)
		{
			ulong rval;
			if (FromMountFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMountFlags")]
		private static extern int ToMountFlags(ulong value, out MountFlags rval);

		public static bool TryToMountFlags(ulong value, out MountFlags rval)
		{
			return ToMountFlags(value, out rval) == 0;
		}

		public static MountFlags ToMountFlags(ulong value)
		{
			MountFlags rval;
			if (ToMountFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMremapFlags")]
		private static extern int FromMremapFlags(MremapFlags value, out ulong rval);

		public static bool TryFromMremapFlags(MremapFlags value, out ulong rval)
		{
			return FromMremapFlags(value, out rval) == 0;
		}

		public static ulong FromMremapFlags(MremapFlags value)
		{
			ulong rval;
			if (FromMremapFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMremapFlags")]
		private static extern int ToMremapFlags(ulong value, out MremapFlags rval);

		public static bool TryToMremapFlags(ulong value, out MremapFlags rval)
		{
			return ToMremapFlags(value, out rval) == 0;
		}

		public static MremapFlags ToMremapFlags(ulong value)
		{
			MremapFlags rval;
			if (ToMremapFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromMsyncFlags")]
		private static extern int FromMsyncFlags(MsyncFlags value, out int rval);

		public static bool TryFromMsyncFlags(MsyncFlags value, out int rval)
		{
			return FromMsyncFlags(value, out rval) == 0;
		}

		public static int FromMsyncFlags(MsyncFlags value)
		{
			int rval;
			if (FromMsyncFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToMsyncFlags")]
		private static extern int ToMsyncFlags(int value, out MsyncFlags rval);

		public static bool TryToMsyncFlags(int value, out MsyncFlags rval)
		{
			return ToMsyncFlags(value, out rval) == 0;
		}

		public static MsyncFlags ToMsyncFlags(int value)
		{
			MsyncFlags rval;
			if (ToMsyncFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromOpenFlags")]
		private static extern int FromOpenFlags(OpenFlags value, out int rval);

		public static bool TryFromOpenFlags(OpenFlags value, out int rval)
		{
			return FromOpenFlags(value, out rval) == 0;
		}

		public static int FromOpenFlags(OpenFlags value)
		{
			int rval;
			if (FromOpenFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToOpenFlags")]
		private static extern int ToOpenFlags(int value, out OpenFlags rval);

		public static bool TryToOpenFlags(int value, out OpenFlags rval)
		{
			return ToOpenFlags(value, out rval) == 0;
		}

		public static OpenFlags ToOpenFlags(int value)
		{
			OpenFlags rval;
			if (ToOpenFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPathconfName")]
		private static extern int FromPathconfName(PathconfName value, out int rval);

		public static bool TryFromPathconfName(PathconfName value, out int rval)
		{
			return FromPathconfName(value, out rval) == 0;
		}

		public static int FromPathconfName(PathconfName value)
		{
			int rval;
			if (FromPathconfName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPathconfName")]
		private static extern int ToPathconfName(int value, out PathconfName rval);

		public static bool TryToPathconfName(int value, out PathconfName rval)
		{
			return ToPathconfName(value, out rval) == 0;
		}

		public static PathconfName ToPathconfName(int value)
		{
			PathconfName rval;
			if (ToPathconfName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPollEvents")]
		private static extern int FromPollEvents(PollEvents value, out short rval);

		public static bool TryFromPollEvents(PollEvents value, out short rval)
		{
			return FromPollEvents(value, out rval) == 0;
		}

		public static short FromPollEvents(PollEvents value)
		{
			short rval;
			if (FromPollEvents(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPollEvents")]
		private static extern int ToPollEvents(short value, out PollEvents rval);

		public static bool TryToPollEvents(short value, out PollEvents rval)
		{
			return ToPollEvents(value, out rval) == 0;
		}

		public static PollEvents ToPollEvents(short value)
		{
			PollEvents rval;
			if (ToPollEvents(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPollfd")]
		private static extern int FromPollfd(ref Pollfd source, IntPtr destination);

		public static bool TryCopy(ref Pollfd source, IntPtr destination)
		{
			return FromPollfd(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPollfd")]
		private static extern int ToPollfd(IntPtr source, out Pollfd destination);

		public static bool TryCopy(IntPtr source, out Pollfd destination)
		{
			return ToPollfd(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPosixFadviseAdvice")]
		private static extern int FromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval);

		public static bool TryFromPosixFadviseAdvice(PosixFadviseAdvice value, out int rval)
		{
			return FromPosixFadviseAdvice(value, out rval) == 0;
		}

		public static int FromPosixFadviseAdvice(PosixFadviseAdvice value)
		{
			int rval;
			if (FromPosixFadviseAdvice(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPosixFadviseAdvice")]
		private static extern int ToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval);

		public static bool TryToPosixFadviseAdvice(int value, out PosixFadviseAdvice rval)
		{
			return ToPosixFadviseAdvice(value, out rval) == 0;
		}

		public static PosixFadviseAdvice ToPosixFadviseAdvice(int value)
		{
			PosixFadviseAdvice rval;
			if (ToPosixFadviseAdvice(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromPosixMadviseAdvice")]
		private static extern int FromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval);

		public static bool TryFromPosixMadviseAdvice(PosixMadviseAdvice value, out int rval)
		{
			return FromPosixMadviseAdvice(value, out rval) == 0;
		}

		public static int FromPosixMadviseAdvice(PosixMadviseAdvice value)
		{
			int rval;
			if (FromPosixMadviseAdvice(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToPosixMadviseAdvice")]
		private static extern int ToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval);

		public static bool TryToPosixMadviseAdvice(int value, out PosixMadviseAdvice rval)
		{
			return ToPosixMadviseAdvice(value, out rval) == 0;
		}

		public static PosixMadviseAdvice ToPosixMadviseAdvice(int value)
		{
			PosixMadviseAdvice rval;
			if (ToPosixMadviseAdvice(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSeekFlags")]
		private static extern int FromSeekFlags(SeekFlags value, out short rval);

		public static bool TryFromSeekFlags(SeekFlags value, out short rval)
		{
			return FromSeekFlags(value, out rval) == 0;
		}

		public static short FromSeekFlags(SeekFlags value)
		{
			short rval;
			if (FromSeekFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSeekFlags")]
		private static extern int ToSeekFlags(short value, out SeekFlags rval);

		public static bool TryToSeekFlags(short value, out SeekFlags rval)
		{
			return ToSeekFlags(value, out rval) == 0;
		}

		public static SeekFlags ToSeekFlags(short value)
		{
			SeekFlags rval;
			if (ToSeekFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromShutdownOption")]
		private static extern int FromShutdownOption(ShutdownOption value, out int rval);

		public static bool TryFromShutdownOption(ShutdownOption value, out int rval)
		{
			return FromShutdownOption(value, out rval) == 0;
		}

		public static int FromShutdownOption(ShutdownOption value)
		{
			int rval;
			if (FromShutdownOption(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToShutdownOption")]
		private static extern int ToShutdownOption(int value, out ShutdownOption rval);

		public static bool TryToShutdownOption(int value, out ShutdownOption rval)
		{
			return ToShutdownOption(value, out rval) == 0;
		}

		public static ShutdownOption ToShutdownOption(int value)
		{
			ShutdownOption rval;
			if (ToShutdownOption(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSignum")]
		private static extern int FromSignum(Signum value, out int rval);

		public static bool TryFromSignum(Signum value, out int rval)
		{
			return FromSignum(value, out rval) == 0;
		}

		public static int FromSignum(Signum value)
		{
			int rval;
			if (FromSignum(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSignum")]
		private static extern int ToSignum(int value, out Signum rval);

		public static bool TryToSignum(int value, out Signum rval)
		{
			return ToSignum(value, out rval) == 0;
		}

		public static Signum ToSignum(int value)
		{
			Signum rval;
			if (ToSignum(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSockaddrIn")]
		private static extern int FromSockaddrIn(SockaddrIn source, IntPtr destination);

		public static bool TryCopy(SockaddrIn source, IntPtr destination)
		{
			return FromSockaddrIn(source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSockaddrIn")]
		private static extern int ToSockaddrIn(IntPtr source, SockaddrIn destination);

		public static bool TryCopy(IntPtr source, SockaddrIn destination)
		{
			return ToSockaddrIn(source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSockaddrIn6")]
		private static extern int FromSockaddrIn6(SockaddrIn6 source, IntPtr destination);

		public static bool TryCopy(SockaddrIn6 source, IntPtr destination)
		{
			return FromSockaddrIn6(source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSockaddrIn6")]
		private static extern int ToSockaddrIn6(IntPtr source, SockaddrIn6 destination);

		public static bool TryCopy(IntPtr source, SockaddrIn6 destination)
		{
			return ToSockaddrIn6(source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSockaddrType")]
		private static extern int FromSockaddrType(SockaddrType value, out int rval);

		internal static bool TryFromSockaddrType(SockaddrType value, out int rval)
		{
			return FromSockaddrType(value, out rval) == 0;
		}

		internal static int FromSockaddrType(SockaddrType value)
		{
			int rval;
			if (FromSockaddrType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSockaddrType")]
		private static extern int ToSockaddrType(int value, out SockaddrType rval);

		internal static bool TryToSockaddrType(int value, out SockaddrType rval)
		{
			return ToSockaddrType(value, out rval) == 0;
		}

		internal static SockaddrType ToSockaddrType(int value)
		{
			SockaddrType rval;
			if (ToSockaddrType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSysconfName")]
		private static extern int FromSysconfName(SysconfName value, out int rval);

		public static bool TryFromSysconfName(SysconfName value, out int rval)
		{
			return FromSysconfName(value, out rval) == 0;
		}

		public static int FromSysconfName(SysconfName value)
		{
			int rval;
			if (FromSysconfName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSysconfName")]
		private static extern int ToSysconfName(int value, out SysconfName rval);

		public static bool TryToSysconfName(int value, out SysconfName rval)
		{
			return ToSysconfName(value, out rval) == 0;
		}

		public static SysconfName ToSysconfName(int value)
		{
			SysconfName rval;
			if (ToSysconfName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogFacility")]
		private static extern int FromSyslogFacility(SyslogFacility value, out int rval);

		public static bool TryFromSyslogFacility(SyslogFacility value, out int rval)
		{
			return FromSyslogFacility(value, out rval) == 0;
		}

		public static int FromSyslogFacility(SyslogFacility value)
		{
			int rval;
			if (FromSyslogFacility(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogFacility")]
		private static extern int ToSyslogFacility(int value, out SyslogFacility rval);

		public static bool TryToSyslogFacility(int value, out SyslogFacility rval)
		{
			return ToSyslogFacility(value, out rval) == 0;
		}

		public static SyslogFacility ToSyslogFacility(int value)
		{
			SyslogFacility rval;
			if (ToSyslogFacility(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogLevel")]
		private static extern int FromSyslogLevel(SyslogLevel value, out int rval);

		public static bool TryFromSyslogLevel(SyslogLevel value, out int rval)
		{
			return FromSyslogLevel(value, out rval) == 0;
		}

		public static int FromSyslogLevel(SyslogLevel value)
		{
			int rval;
			if (FromSyslogLevel(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogLevel")]
		private static extern int ToSyslogLevel(int value, out SyslogLevel rval);

		public static bool TryToSyslogLevel(int value, out SyslogLevel rval)
		{
			return ToSyslogLevel(value, out rval) == 0;
		}

		public static SyslogLevel ToSyslogLevel(int value)
		{
			SyslogLevel rval;
			if (ToSyslogLevel(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSyslogOptions")]
		private static extern int FromSyslogOptions(SyslogOptions value, out int rval);

		public static bool TryFromSyslogOptions(SyslogOptions value, out int rval)
		{
			return FromSyslogOptions(value, out rval) == 0;
		}

		public static int FromSyslogOptions(SyslogOptions value)
		{
			int rval;
			if (FromSyslogOptions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSyslogOptions")]
		private static extern int ToSyslogOptions(int value, out SyslogOptions rval);

		public static bool TryToSyslogOptions(int value, out SyslogOptions rval)
		{
			return ToSyslogOptions(value, out rval) == 0;
		}

		public static SyslogOptions ToSyslogOptions(int value)
		{
			SyslogOptions rval;
			if (ToSyslogOptions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimespec")]
		private static extern int FromTimespec(ref Timespec source, IntPtr destination);

		public static bool TryCopy(ref Timespec source, IntPtr destination)
		{
			return FromTimespec(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimespec")]
		private static extern int ToTimespec(IntPtr source, out Timespec destination);

		public static bool TryCopy(IntPtr source, out Timespec destination)
		{
			return ToTimespec(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimeval")]
		private static extern int FromTimeval(ref Timeval source, IntPtr destination);

		public static bool TryCopy(ref Timeval source, IntPtr destination)
		{
			return FromTimeval(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimeval")]
		private static extern int ToTimeval(IntPtr source, out Timeval destination);

		public static bool TryCopy(IntPtr source, out Timeval destination)
		{
			return ToTimeval(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromTimezone")]
		private static extern int FromTimezone(ref Timezone source, IntPtr destination);

		public static bool TryCopy(ref Timezone source, IntPtr destination)
		{
			return FromTimezone(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToTimezone")]
		private static extern int ToTimezone(IntPtr source, out Timezone destination);

		public static bool TryCopy(IntPtr source, out Timezone destination)
		{
			return ToTimezone(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixAddressFamily")]
		private static extern int FromUnixAddressFamily(UnixAddressFamily value, out int rval);

		public static bool TryFromUnixAddressFamily(UnixAddressFamily value, out int rval)
		{
			return FromUnixAddressFamily(value, out rval) == 0;
		}

		public static int FromUnixAddressFamily(UnixAddressFamily value)
		{
			int rval;
			if (FromUnixAddressFamily(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixAddressFamily")]
		private static extern int ToUnixAddressFamily(int value, out UnixAddressFamily rval);

		public static bool TryToUnixAddressFamily(int value, out UnixAddressFamily rval)
		{
			return ToUnixAddressFamily(value, out rval) == 0;
		}

		public static UnixAddressFamily ToUnixAddressFamily(int value)
		{
			UnixAddressFamily rval;
			if (ToUnixAddressFamily(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixSocketControlMessage")]
		private static extern int FromUnixSocketControlMessage(UnixSocketControlMessage value, out int rval);

		public static bool TryFromUnixSocketControlMessage(UnixSocketControlMessage value, out int rval)
		{
			return FromUnixSocketControlMessage(value, out rval) == 0;
		}

		public static int FromUnixSocketControlMessage(UnixSocketControlMessage value)
		{
			int rval;
			if (FromUnixSocketControlMessage(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixSocketControlMessage")]
		private static extern int ToUnixSocketControlMessage(int value, out UnixSocketControlMessage rval);

		public static bool TryToUnixSocketControlMessage(int value, out UnixSocketControlMessage rval)
		{
			return ToUnixSocketControlMessage(value, out rval) == 0;
		}

		public static UnixSocketControlMessage ToUnixSocketControlMessage(int value)
		{
			UnixSocketControlMessage rval;
			if (ToUnixSocketControlMessage(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixSocketFlags")]
		private static extern int FromUnixSocketFlags(UnixSocketFlags value, out int rval);

		public static bool TryFromUnixSocketFlags(UnixSocketFlags value, out int rval)
		{
			return FromUnixSocketFlags(value, out rval) == 0;
		}

		public static int FromUnixSocketFlags(UnixSocketFlags value)
		{
			int rval;
			if (FromUnixSocketFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixSocketFlags")]
		private static extern int ToUnixSocketFlags(int value, out UnixSocketFlags rval);

		public static bool TryToUnixSocketFlags(int value, out UnixSocketFlags rval)
		{
			return ToUnixSocketFlags(value, out rval) == 0;
		}

		public static UnixSocketFlags ToUnixSocketFlags(int value)
		{
			UnixSocketFlags rval;
			if (ToUnixSocketFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixSocketOptionName")]
		private static extern int FromUnixSocketOptionName(UnixSocketOptionName value, out int rval);

		public static bool TryFromUnixSocketOptionName(UnixSocketOptionName value, out int rval)
		{
			return FromUnixSocketOptionName(value, out rval) == 0;
		}

		public static int FromUnixSocketOptionName(UnixSocketOptionName value)
		{
			int rval;
			if (FromUnixSocketOptionName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixSocketOptionName")]
		private static extern int ToUnixSocketOptionName(int value, out UnixSocketOptionName rval);

		public static bool TryToUnixSocketOptionName(int value, out UnixSocketOptionName rval)
		{
			return ToUnixSocketOptionName(value, out rval) == 0;
		}

		public static UnixSocketOptionName ToUnixSocketOptionName(int value)
		{
			UnixSocketOptionName rval;
			if (ToUnixSocketOptionName(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixSocketProtocol")]
		private static extern int FromUnixSocketProtocol(UnixSocketProtocol value, out int rval);

		public static bool TryFromUnixSocketProtocol(UnixSocketProtocol value, out int rval)
		{
			return FromUnixSocketProtocol(value, out rval) == 0;
		}

		public static int FromUnixSocketProtocol(UnixSocketProtocol value)
		{
			int rval;
			if (FromUnixSocketProtocol(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixSocketProtocol")]
		private static extern int ToUnixSocketProtocol(int value, out UnixSocketProtocol rval);

		public static bool TryToUnixSocketProtocol(int value, out UnixSocketProtocol rval)
		{
			return ToUnixSocketProtocol(value, out rval) == 0;
		}

		public static UnixSocketProtocol ToUnixSocketProtocol(int value)
		{
			UnixSocketProtocol rval;
			if (ToUnixSocketProtocol(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUnixSocketType")]
		private static extern int FromUnixSocketType(UnixSocketType value, out int rval);

		public static bool TryFromUnixSocketType(UnixSocketType value, out int rval)
		{
			return FromUnixSocketType(value, out rval) == 0;
		}

		public static int FromUnixSocketType(UnixSocketType value)
		{
			int rval;
			if (FromUnixSocketType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUnixSocketType")]
		private static extern int ToUnixSocketType(int value, out UnixSocketType rval);

		public static bool TryToUnixSocketType(int value, out UnixSocketType rval)
		{
			return ToUnixSocketType(value, out rval) == 0;
		}

		public static UnixSocketType ToUnixSocketType(int value)
		{
			UnixSocketType rval;
			if (ToUnixSocketType(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromUtimbuf")]
		private static extern int FromUtimbuf(ref Utimbuf source, IntPtr destination);

		public static bool TryCopy(ref Utimbuf source, IntPtr destination)
		{
			return FromUtimbuf(ref source, destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToUtimbuf")]
		private static extern int ToUtimbuf(IntPtr source, out Utimbuf destination);

		public static bool TryCopy(IntPtr source, out Utimbuf destination)
		{
			return ToUtimbuf(source, out destination) == 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromWaitOptions")]
		private static extern int FromWaitOptions(WaitOptions value, out int rval);

		public static bool TryFromWaitOptions(WaitOptions value, out int rval)
		{
			return FromWaitOptions(value, out rval) == 0;
		}

		public static int FromWaitOptions(WaitOptions value)
		{
			int rval;
			if (FromWaitOptions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToWaitOptions")]
		private static extern int ToWaitOptions(int value, out WaitOptions rval);

		public static bool TryToWaitOptions(int value, out WaitOptions rval)
		{
			return ToWaitOptions(value, out rval) == 0;
		}

		public static WaitOptions ToWaitOptions(int value)
		{
			WaitOptions rval;
			if (ToWaitOptions(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromXattrFlags")]
		private static extern int FromXattrFlags(XattrFlags value, out int rval);

		public static bool TryFromXattrFlags(XattrFlags value, out int rval)
		{
			return FromXattrFlags(value, out rval) == 0;
		}

		public static int FromXattrFlags(XattrFlags value)
		{
			int rval;
			if (FromXattrFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToXattrFlags")]
		private static extern int ToXattrFlags(int value, out XattrFlags rval);

		public static bool TryToXattrFlags(int value, out XattrFlags rval)
		{
			return ToXattrFlags(value, out rval) == 0;
		}

		public static XattrFlags ToXattrFlags(int value)
		{
			XattrFlags rval;
			if (ToXattrFlags(value, out rval) == -1)
			{
				ThrowArgumentException(value);
			}
			return rval;
		}
	}
}
