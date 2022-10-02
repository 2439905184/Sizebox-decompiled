using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class Syscall : Stdlib
	{
		private struct _Dirent
		{
			[ino_t]
			public ulong d_ino;

			[off_t]
			public long d_off;

			public ushort d_reclen;

			public byte d_type;

			public IntPtr d_name;
		}

		[Map]
		private struct _Fstab
		{
			public IntPtr fs_spec;

			public IntPtr fs_file;

			public IntPtr fs_vfstype;

			public IntPtr fs_mntops;

			public IntPtr fs_type;

			public int fs_freq;

			public int fs_passno;

			public IntPtr _fs_buf_;
		}

		[Map]
		private struct _Group
		{
			public IntPtr gr_name;

			public IntPtr gr_passwd;

			[gid_t]
			public uint gr_gid;

			public int _gr_nmem_;

			public IntPtr gr_mem;

			public IntPtr _gr_buf_;
		}

		[Map]
		private struct _Passwd
		{
			public IntPtr pw_name;

			public IntPtr pw_passwd;

			[uid_t]
			public uint pw_uid;

			[gid_t]
			public uint pw_gid;

			public IntPtr pw_gecos;

			public IntPtr pw_dir;

			public IntPtr pw_shell;

			public IntPtr _pw_buf_;
		}

		private struct _pollfd
		{
			public int fd;

			public short events;

			public short revents;
		}

		[Map]
		private struct _Utsname
		{
			public IntPtr sysname;

			public IntPtr nodename;

			public IntPtr release;

			public IntPtr version;

			public IntPtr machine;

			public IntPtr domainname;

			public IntPtr _buf_;
		}

		private delegate long DoReadlinkFun(byte[] target);

		private struct _Msghdr
		{
			public unsafe Iovec* msg_iov;

			public int msg_iovlen;

			public unsafe byte* msg_control;

			public long msg_controllen;

			public int msg_flags;

			public unsafe _Msghdr(Msghdr message, Iovec* ptr_msg_iov, byte* ptr_msg_control)
			{
				if (message.msg_iovlen > message.msg_iov.Length || message.msg_iovlen < 0)
				{
					throw new ArgumentException("message.msg_iovlen > message.msg_iov.Length || message.msg_iovlen < 0", "message");
				}
				msg_iov = ptr_msg_iov;
				msg_iovlen = message.msg_iovlen;
				if (message.msg_control == null && message.msg_controllen != 0L)
				{
					throw new ArgumentException("message.msg_control == null && message.msg_controllen != 0", "message");
				}
				if (message.msg_control != null && message.msg_controllen > message.msg_control.Length)
				{
					throw new ArgumentException("message.msg_controllen > message.msg_control.Length", "message");
				}
				msg_control = ptr_msg_control;
				msg_controllen = message.msg_controllen;
				msg_flags = 0;
			}

			public void Update(Msghdr message)
			{
				message.msg_controllen = msg_controllen;
				message.msg_flags = NativeConvert.ToMessageFlags(msg_flags);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass327_0
		{
			public string path;

			internal long _003Creadlink_003Eb__0(byte[] target)
			{
				return readlink(path, target);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass381_0
		{
			public int dirfd;

			public string pathname;

			internal long _003Creadlinkat_003Eb__0(byte[] target)
			{
				return readlinkat(dirfd, pathname, target);
			}
		}

		internal new const string LIBC = "libc";

		internal static object readdir_lock = new object();

		public static readonly int AT_FDCWD = get_at_fdcwd();

		internal static object fstab_lock = new object();

		internal static object grp_lock = new object();

		internal static object pwd_lock = new object();

		private static object signal_lock = new object();

		public static readonly int L_ctermid = _L_ctermid();

		public static readonly int L_cuserid = _L_cuserid();

		internal static object getlogin_lock = new object();

		public static readonly IntPtr MAP_FAILED = (IntPtr)(-1);

		public static readonly long UTIME_NOW = get_utime_now();

		public static readonly long UTIME_OMIT = get_utime_omit();

		private static object tty_lock = new object();

		internal static object usershell_lock = new object();

		private Syscall()
		{
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setxattr", SetLastError = true)]
		public static extern int setxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size, XattrFlags flags);

		public static int setxattr(string path, string name, byte[] value, ulong size)
		{
			return setxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int setxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return setxattr(path, name, value, (ulong)value.Length, flags);
		}

		public static int setxattr(string path, string name, byte[] value)
		{
			return setxattr(path, name, value, (ulong)value.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lsetxattr", SetLastError = true)]
		public static extern int lsetxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size, XattrFlags flags);

		public static int lsetxattr(string path, string name, byte[] value, ulong size)
		{
			return lsetxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int lsetxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return lsetxattr(path, name, value, (ulong)value.Length, flags);
		}

		public static int lsetxattr(string path, string name, byte[] value)
		{
			return lsetxattr(path, name, value, (ulong)value.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fsetxattr", SetLastError = true)]
		public static extern int fsetxattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size, XattrFlags flags);

		public static int fsetxattr(int fd, string name, byte[] value, ulong size)
		{
			return fsetxattr(fd, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int fsetxattr(int fd, string name, byte[] value, XattrFlags flags)
		{
			return fsetxattr(fd, name, value, (ulong)value.Length, flags);
		}

		public static int fsetxattr(int fd, string name, byte[] value)
		{
			return fsetxattr(fd, name, value, (ulong)value.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getxattr", SetLastError = true)]
		public static extern long getxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size);

		public static long getxattr(string path, string name, byte[] value)
		{
			return getxattr(path, name, value, (ulong)value.Length);
		}

		public static long getxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = getxattr(path, name, value, 0uL);
			if (num <= 0)
			{
				return num;
			}
			value = new byte[num];
			return getxattr(path, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lgetxattr", SetLastError = true)]
		public static extern long lgetxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size);

		public static long lgetxattr(string path, string name, byte[] value)
		{
			return lgetxattr(path, name, value, (ulong)value.Length);
		}

		public static long lgetxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = lgetxattr(path, name, value, 0uL);
			if (num <= 0)
			{
				return num;
			}
			value = new byte[num];
			return lgetxattr(path, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetxattr", SetLastError = true)]
		public static extern long fgetxattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, byte[] value, ulong size);

		public static long fgetxattr(int fd, string name, byte[] value)
		{
			return fgetxattr(fd, name, value, (ulong)value.Length);
		}

		public static long fgetxattr(int fd, string name, out byte[] value)
		{
			value = null;
			long num = fgetxattr(fd, name, value, 0uL);
			if (num <= 0)
			{
				return num;
			}
			value = new byte[num];
			return fgetxattr(fd, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_listxattr", SetLastError = true)]
		public static extern long listxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, byte[] list, ulong size);

		public static long listxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = listxattr(path, null, 0uL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0)
			{
				return (int)num;
			}
			byte[] list = new byte[num];
			long num2 = listxattr(path, list, (ulong)num);
			if (num2 < 0)
			{
				return (int)num2;
			}
			GetValues(list, encoding, out values);
			return 0L;
		}

		public static long listxattr(string path, out string[] values)
		{
			return listxattr(path, UnixEncoding.Instance, out values);
		}

		private static void GetValues(byte[] list, Encoding encoding, out string[] values)
		{
			int num = 0;
			for (int i = 0; i < list.Length; i++)
			{
				if (list[i] == 0)
				{
					num++;
				}
			}
			values = new string[num];
			num = 0;
			int num2 = 0;
			for (int j = 0; j < list.Length; j++)
			{
				if (list[j] == 0)
				{
					values[num++] = encoding.GetString(list, num2, j - num2);
					num2 = j + 1;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_llistxattr", SetLastError = true)]
		public static extern long llistxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, byte[] list, ulong size);

		public static long llistxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = llistxattr(path, null, 0uL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0)
			{
				return (int)num;
			}
			byte[] list = new byte[num];
			long num2 = llistxattr(path, list, (ulong)num);
			if (num2 < 0)
			{
				return (int)num2;
			}
			GetValues(list, encoding, out values);
			return 0L;
		}

		public static long llistxattr(string path, out string[] values)
		{
			return llistxattr(path, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_flistxattr", SetLastError = true)]
		public static extern long flistxattr(int fd, byte[] list, ulong size);

		public static long flistxattr(int fd, Encoding encoding, out string[] values)
		{
			values = null;
			long num = flistxattr(fd, null, 0uL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0)
			{
				return (int)num;
			}
			byte[] list = new byte[num];
			long num2 = flistxattr(fd, list, (ulong)num);
			if (num2 < 0)
			{
				return (int)num2;
			}
			GetValues(list, encoding, out values);
			return 0L;
		}

		public static long flistxattr(int fd, out string[] values)
		{
			return flistxattr(fd, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_removexattr", SetLastError = true)]
		public static extern int removexattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lremovexattr", SetLastError = true)]
		public static extern int lremovexattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fremovexattr", SetLastError = true)]
		public static extern int fremovexattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name);

		[DllImport("libc", SetLastError = true)]
		public static extern IntPtr opendir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name);

		[DllImport("libc", SetLastError = true)]
		public static extern int closedir(IntPtr dir);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_seekdir", SetLastError = true)]
		public static extern int seekdir(IntPtr dir, long offset);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_telldir", SetLastError = true)]
		public static extern long telldir(IntPtr dir);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_rewinddir", SetLastError = true)]
		public static extern int rewinddir(IntPtr dir);

		private static void CopyDirent(Dirent to, ref _Dirent from)
		{
			try
			{
				to.d_ino = from.d_ino;
				to.d_off = from.d_off;
				to.d_reclen = from.d_reclen;
				to.d_type = from.d_type;
				to.d_name = UnixMarshal.PtrToString(from.d_name);
			}
			finally
			{
				Stdlib.free(from.d_name);
				from.d_name = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readdir", SetLastError = true)]
		private static extern int sys_readdir(IntPtr dir, out _Dirent dentry);

		public static Dirent readdir(IntPtr dir)
		{
			int num;
			_Dirent dentry;
			lock (readdir_lock)
			{
				num = sys_readdir(dir, out dentry);
			}
			if (num != 0)
			{
				return null;
			}
			Dirent dirent = new Dirent();
			CopyDirent(dirent, ref dentry);
			return dirent;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readdir_r", SetLastError = true)]
		private static extern int sys_readdir_r(IntPtr dirp, out _Dirent entry, out IntPtr result);

		public static int readdir_r(IntPtr dirp, Dirent entry, out IntPtr result)
		{
			entry.d_ino = 0uL;
			entry.d_off = 0L;
			entry.d_reclen = 0;
			entry.d_type = 0;
			entry.d_name = null;
			_Dirent entry2;
			int num = sys_readdir_r(dirp, out entry2, out result);
			if (num == 0 && result != IntPtr.Zero)
			{
				CopyDirent(entry, ref entry2);
			}
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int dirfd(IntPtr dir);

		[DllImport("libc", SetLastError = true)]
		public static extern IntPtr fdopendir(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_arg", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, long arg);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_arg_int", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, int arg);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_arg_ptr", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, IntPtr ptr);

		public static int fcntl(int fd, FcntlCommand cmd, DirectoryNotifyFlags arg)
		{
			if (cmd != FcntlCommand.F_NOTIFY)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			long arg2 = NativeConvert.FromDirectoryNotifyFlags(arg);
			return fcntl(fd, FcntlCommand.F_NOTIFY, arg2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_lock", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, ref Flock @lock);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_open", SetLastError = true)]
		public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, OpenFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_open_mode", SetLastError = true)]
		public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, OpenFlags flags, FilePermissions mode);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_creat", SetLastError = true)]
		public static extern int creat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, FilePermissions mode);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_fadvise", SetLastError = true)]
		public static extern int posix_fadvise(int fd, long offset, long len, PosixFadviseAdvice advice);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_fallocate", SetLastError = true)]
		public static extern int posix_fallocate(int fd, long offset, ulong len);

		[DllImport("libc", EntryPoint = "openat", SetLastError = true)]
		private static extern int sys_openat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, int flags);

		[DllImport("libc", EntryPoint = "openat", SetLastError = true)]
		private static extern int sys_openat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, int flags, uint mode);

		public static int openat(int dirfd, string pathname, OpenFlags flags)
		{
			int flags2 = NativeConvert.FromOpenFlags(flags);
			return sys_openat(dirfd, pathname, flags2);
		}

		public static int openat(int dirfd, string pathname, OpenFlags flags, FilePermissions mode)
		{
			int flags2 = NativeConvert.FromOpenFlags(flags);
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_openat(dirfd, pathname, flags2, mode2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_get_at_fdcwd", SetLastError = true)]
		private static extern int get_at_fdcwd();

		private static void CopyFstab(Fstab to, ref _Fstab from)
		{
			try
			{
				to.fs_spec = UnixMarshal.PtrToString(from.fs_spec);
				to.fs_file = UnixMarshal.PtrToString(from.fs_file);
				to.fs_vfstype = UnixMarshal.PtrToString(from.fs_vfstype);
				to.fs_mntops = UnixMarshal.PtrToString(from.fs_mntops);
				to.fs_type = UnixMarshal.PtrToString(from.fs_type);
				to.fs_freq = from.fs_freq;
				to.fs_passno = from.fs_passno;
			}
			finally
			{
				Stdlib.free(from._fs_buf_);
				from._fs_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endfsent", SetLastError = true)]
		private static extern int sys_endfsent();

		public static int endfsent()
		{
			lock (fstab_lock)
			{
				return sys_endfsent();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsent", SetLastError = true)]
		private static extern int sys_getfsent(out _Fstab fs);

		public static Fstab getfsent()
		{
			int num;
			_Fstab fs;
			lock (fstab_lock)
			{
				num = sys_getfsent(out fs);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			CopyFstab(fstab, ref fs);
			return fstab;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsfile", SetLastError = true)]
		private static extern int sys_getfsfile([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string mount_point, out _Fstab fs);

		public static Fstab getfsfile(string mount_point)
		{
			int num;
			_Fstab fs;
			lock (fstab_lock)
			{
				num = sys_getfsfile(mount_point, out fs);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			CopyFstab(fstab, ref fs);
			return fstab;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsspec", SetLastError = true)]
		private static extern int sys_getfsspec([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string special_file, out _Fstab fs);

		public static Fstab getfsspec(string special_file)
		{
			int num;
			_Fstab fs;
			lock (fstab_lock)
			{
				num = sys_getfsspec(special_file, out fs);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab = new Fstab();
			CopyFstab(fstab, ref fs);
			return fstab;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setfsent", SetLastError = true)]
		private static extern int sys_setfsent();

		public static int setfsent()
		{
			lock (fstab_lock)
			{
				return sys_setfsent();
			}
		}

		[DllImport("libc", EntryPoint = "getgrouplist", SetLastError = true)]
		private static extern int sys_getgrouplist(string user, uint grp, uint[] groups, ref int ngroups);

		public static Group[] getgrouplist(string username)
		{
			if (username == null)
			{
				throw new ArgumentNullException("username");
			}
			if (username.Trim() == "")
			{
				throw new ArgumentException("Username cannot be empty", "username");
			}
			Passwd passwd = getpwnam(username);
			if (passwd == null)
			{
				throw new ArgumentException(string.Format("User {0} does not exist", username), "username");
			}
			return getgrouplist(passwd);
		}

		public static Group[] getgrouplist(Passwd user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			int ngroups = 8;
			int num = -1;
			uint[] array = null;
			do
			{
				Array.Resize(ref array, ngroups *= 2);
				num = sys_getgrouplist(user.pw_name, user.pw_gid, array, ref ngroups);
			}
			while (num == -1);
			List<Group> list = new List<Group>();
			Group group = null;
			for (int i = 0; i < num; i++)
			{
				group = getgrgid(array[i]);
				if (group != null)
				{
					list.Add(group);
				}
			}
			return list.ToArray();
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setgroups", SetLastError = true)]
		public static extern int setgroups(ulong size, uint[] list);

		public static int setgroups(uint[] list)
		{
			return setgroups((ulong)list.Length, list);
		}

		private static void CopyGroup(Group to, ref _Group from)
		{
			try
			{
				to.gr_gid = from.gr_gid;
				to.gr_name = UnixMarshal.PtrToString(from.gr_name);
				to.gr_passwd = UnixMarshal.PtrToString(from.gr_passwd);
				to.gr_mem = UnixMarshal.PtrToStringArray(from._gr_nmem_, from.gr_mem);
			}
			finally
			{
				Stdlib.free(from.gr_mem);
				Stdlib.free(from._gr_buf_);
				from.gr_mem = IntPtr.Zero;
				from._gr_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrnam", SetLastError = true)]
		private static extern int sys_getgrnam(string name, out _Group group);

		public static Group getgrnam(string name)
		{
			int num;
			_Group group;
			lock (grp_lock)
			{
				num = sys_getgrnam(name, out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			CopyGroup(group2, ref group);
			return group2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrgid", SetLastError = true)]
		private static extern int sys_getgrgid(uint uid, out _Group group);

		public static Group getgrgid(uint uid)
		{
			int num;
			_Group group;
			lock (grp_lock)
			{
				num = sys_getgrgid(uid, out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			CopyGroup(group2, ref group);
			return group2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrnam_r", SetLastError = true)]
		private static extern int sys_getgrnam_r([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, out _Group grbuf, out IntPtr grbufp);

		public static int getgrnam_r(string name, Group grbuf, out Group grbufp)
		{
			grbufp = null;
			_Group grbuf2;
			IntPtr grbufp2;
			int num = sys_getgrnam_r(name, out grbuf2, out grbufp2);
			if (num == 0 && grbufp2 != IntPtr.Zero)
			{
				CopyGroup(grbuf, ref grbuf2);
				grbufp = grbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrgid_r", SetLastError = true)]
		private static extern int sys_getgrgid_r(uint uid, out _Group grbuf, out IntPtr grbufp);

		public static int getgrgid_r(uint uid, Group grbuf, out Group grbufp)
		{
			grbufp = null;
			_Group grbuf2;
			IntPtr grbufp2;
			int num = sys_getgrgid_r(uid, out grbuf2, out grbufp2);
			if (num == 0 && grbufp2 != IntPtr.Zero)
			{
				CopyGroup(grbuf, ref grbuf2);
				grbufp = grbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrent", SetLastError = true)]
		private static extern int sys_getgrent(out _Group grbuf);

		public static Group getgrent()
		{
			int num;
			_Group grbuf;
			lock (grp_lock)
			{
				num = sys_getgrent(out grbuf);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			CopyGroup(group, ref grbuf);
			return group;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setgrent", SetLastError = true)]
		private static extern int sys_setgrent();

		public static int setgrent()
		{
			lock (grp_lock)
			{
				return sys_setgrent();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endgrent", SetLastError = true)]
		private static extern int sys_endgrent();

		public static int endgrent()
		{
			lock (grp_lock)
			{
				return sys_endgrent();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetgrent", SetLastError = true)]
		private static extern int sys_fgetgrent(IntPtr stream, out _Group grbuf);

		public static Group fgetgrent(IntPtr stream)
		{
			int num;
			_Group grbuf;
			lock (grp_lock)
			{
				num = sys_fgetgrent(stream, out grbuf);
			}
			if (num != 0)
			{
				return null;
			}
			Group group = new Group();
			CopyGroup(group, ref grbuf);
			return group;
		}

		private static void CopyPasswd(Passwd to, ref _Passwd from)
		{
			try
			{
				to.pw_name = UnixMarshal.PtrToString(from.pw_name);
				to.pw_passwd = UnixMarshal.PtrToString(from.pw_passwd);
				to.pw_uid = from.pw_uid;
				to.pw_gid = from.pw_gid;
				to.pw_gecos = UnixMarshal.PtrToString(from.pw_gecos);
				to.pw_dir = UnixMarshal.PtrToString(from.pw_dir);
				to.pw_shell = UnixMarshal.PtrToString(from.pw_shell);
			}
			finally
			{
				Stdlib.free(from._pw_buf_);
				from._pw_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwnam", SetLastError = true)]
		private static extern int sys_getpwnam(string name, out _Passwd passwd);

		public static Passwd getpwnam(string name)
		{
			int num;
			_Passwd passwd;
			lock (pwd_lock)
			{
				num = sys_getpwnam(name, out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwuid", SetLastError = true)]
		private static extern int sys_getpwuid(uint uid, out _Passwd passwd);

		public static Passwd getpwuid(uint uid)
		{
			int num;
			_Passwd passwd;
			lock (pwd_lock)
			{
				num = sys_getpwuid(uid, out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwnam_r", SetLastError = true)]
		private static extern int sys_getpwnam_r([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string name, out _Passwd pwbuf, out IntPtr pwbufp);

		public static int getpwnam_r(string name, Passwd pwbuf, out Passwd pwbufp)
		{
			pwbufp = null;
			_Passwd pwbuf2;
			IntPtr pwbufp2;
			int num = sys_getpwnam_r(name, out pwbuf2, out pwbufp2);
			if (num == 0 && pwbufp2 != IntPtr.Zero)
			{
				CopyPasswd(pwbuf, ref pwbuf2);
				pwbufp = pwbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwuid_r", SetLastError = true)]
		private static extern int sys_getpwuid_r(uint uid, out _Passwd pwbuf, out IntPtr pwbufp);

		public static int getpwuid_r(uint uid, Passwd pwbuf, out Passwd pwbufp)
		{
			pwbufp = null;
			_Passwd pwbuf2;
			IntPtr pwbufp2;
			int num = sys_getpwuid_r(uid, out pwbuf2, out pwbufp2);
			if (num == 0 && pwbufp2 != IntPtr.Zero)
			{
				CopyPasswd(pwbuf, ref pwbuf2);
				pwbufp = pwbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwent", SetLastError = true)]
		private static extern int sys_getpwent(out _Passwd pwbuf);

		public static Passwd getpwent()
		{
			int num;
			_Passwd pwbuf;
			lock (pwd_lock)
			{
				num = sys_getpwent(out pwbuf);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			CopyPasswd(passwd, ref pwbuf);
			return passwd;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setpwent", SetLastError = true)]
		private static extern int sys_setpwent();

		public static int setpwent()
		{
			lock (pwd_lock)
			{
				return sys_setpwent();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endpwent", SetLastError = true)]
		private static extern int sys_endpwent();

		public static int endpwent()
		{
			lock (pwd_lock)
			{
				return sys_endpwent();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetpwent", SetLastError = true)]
		private static extern int sys_fgetpwent(IntPtr stream, out _Passwd pwbuf);

		public static Passwd fgetpwent(IntPtr stream)
		{
			int num;
			_Passwd pwbuf;
			lock (pwd_lock)
			{
				num = sys_fgetpwent(stream, out pwbuf);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd = new Passwd();
			CopyPasswd(passwd, ref pwbuf);
			return passwd;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_psignal", SetLastError = true)]
		private static extern int psignal(int sig, string s);

		public static int psignal(Signum sig, string s)
		{
			return psignal(NativeConvert.FromSignum(sig), s);
		}

		[DllImport("libc", EntryPoint = "kill", SetLastError = true)]
		private static extern int sys_kill(int pid, int sig);

		public static int kill(int pid, Signum sig)
		{
			int sig2 = NativeConvert.FromSignum(sig);
			return sys_kill(pid, sig2);
		}

		[DllImport("libc", EntryPoint = "strsignal", SetLastError = true)]
		private static extern IntPtr sys_strsignal(int sig);

		public static string strsignal(Signum sig)
		{
			int sig2 = NativeConvert.FromSignum(sig);
			lock (signal_lock)
			{
				return UnixMarshal.PtrToString(sys_strsignal(sig2));
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_L_ctermid")]
		private static extern int _L_ctermid();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_L_cuserid")]
		private static extern int _L_cuserid();

		[DllImport("libc", EntryPoint = "cuserid", SetLastError = true)]
		private static extern IntPtr sys_cuserid([Out] StringBuilder @string);

		[Obsolete("\"Nobody knows precisely what cuserid() does... DO NOT USE cuserid().\n`string' must hold L_cuserid characters.  Use getlogin_r instead.")]
		public static string cuserid(StringBuilder @string)
		{
			if (@string.Capacity < L_cuserid)
			{
				throw new ArgumentOutOfRangeException("string", "string.Capacity < L_cuserid");
			}
			lock (getlogin_lock)
			{
				return UnixMarshal.PtrToString(sys_cuserid(@string));
			}
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int renameat(int olddirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, int newdirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath);

		[DllImport("libc", SetLastError = true)]
		public static extern int mkstemp(StringBuilder template);

		[DllImport("libc", EntryPoint = "mkdtemp", SetLastError = true)]
		private static extern IntPtr sys_mkdtemp(StringBuilder template);

		public static StringBuilder mkdtemp(StringBuilder template)
		{
			if (sys_mkdtemp(template) == IntPtr.Zero)
			{
				return null;
			}
			return template;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int ttyslot();

		[Obsolete("This is insecure and should not be used", true)]
		public static int setkey(string key)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_strerror_r", SetLastError = true)]
		private static extern int sys_strerror_r(int errnum, [Out] StringBuilder buf, ulong n);

		public static int strerror_r(Errno errnum, StringBuilder buf, ulong n)
		{
			return sys_strerror_r(NativeConvert.FromErrno(errnum), buf, n);
		}

		public static int strerror_r(Errno errnum, StringBuilder buf)
		{
			return strerror_r(errnum, buf, (ulong)buf.Capacity);
		}

		public static int epoll_create(int size)
		{
			return sys_epoll_create(size);
		}

		public static int epoll_create(EpollFlags flags)
		{
			return sys_epoll_create1(flags);
		}

		public static int epoll_ctl(int epfd, EpollOp op, int fd, EpollEvents events)
		{
			EpollEvent ee = default(EpollEvent);
			ee.events = events;
			ee.fd = fd;
			return epoll_ctl(epfd, op, fd, ref ee);
		}

		public static int epoll_wait(int epfd, EpollEvent[] events, int max_events, int timeout)
		{
			if (events.Length < max_events)
			{
				throw new ArgumentOutOfRangeException("events", "Must refer to at least 'max_events' elements.");
			}
			return sys_epoll_wait(epfd, events, max_events, timeout);
		}

		[DllImport("libc", EntryPoint = "epoll_create", SetLastError = true)]
		private static extern int sys_epoll_create(int size);

		[DllImport("libc", EntryPoint = "epoll_create1", SetLastError = true)]
		private static extern int sys_epoll_create1(EpollFlags flags);

		[DllImport("libc", SetLastError = true)]
		public static extern int epoll_ctl(int epfd, EpollOp op, int fd, ref EpollEvent ee);

		[DllImport("libc", EntryPoint = "epoll_wait", SetLastError = true)]
		private static extern int sys_epoll_wait(int epfd, EpollEvent[] ee, int maxevents, int timeout);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_madvise", SetLastError = true)]
		public static extern int posix_madvise(IntPtr addr, ulong len, PosixMadviseAdvice advice);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mmap", SetLastError = true)]
		public static extern IntPtr mmap(IntPtr start, ulong length, MmapProts prot, MmapFlags flags, int fd, long offset);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_munmap", SetLastError = true)]
		public static extern int munmap(IntPtr start, ulong length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mprotect", SetLastError = true)]
		public static extern int mprotect(IntPtr start, ulong len, MmapProts prot);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_msync", SetLastError = true)]
		public static extern int msync(IntPtr start, ulong len, MsyncFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mlock", SetLastError = true)]
		public static extern int mlock(IntPtr start, ulong len);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_munlock", SetLastError = true)]
		public static extern int munlock(IntPtr start, ulong len);

		[DllImport("libc", EntryPoint = "mlockall", SetLastError = true)]
		private static extern int sys_mlockall(int flags);

		public static int mlockall(MlockallFlags flags)
		{
			return sys_mlockall(NativeConvert.FromMlockallFlags(flags));
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int munlockall();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mremap", SetLastError = true)]
		public static extern IntPtr mremap(IntPtr old_address, ulong old_size, ulong new_size, MremapFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mincore", SetLastError = true)]
		public static extern int mincore(IntPtr start, ulong length, byte[] vec);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_remap_file_pages", SetLastError = true)]
		public static extern int remap_file_pages(IntPtr start, ulong size, MmapProts prot, long pgoff, MmapFlags flags);

		[DllImport("libc", EntryPoint = "poll", SetLastError = true)]
		private static extern int sys_poll(_pollfd[] ufds, uint nfds, int timeout);

		public static int poll(Pollfd[] fds, uint nfds, int timeout)
		{
			if (fds.Length < nfds)
			{
				throw new ArgumentOutOfRangeException("fds", "Must refer to at least `nfds' elements");
			}
			_pollfd[] array = new _pollfd[nfds];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].fd = fds[i].fd;
				array[i].events = NativeConvert.FromPollEvents(fds[i].events);
			}
			int result = sys_poll(array, nfds, timeout);
			for (int j = 0; j < array.Length; j++)
			{
				fds[j].revents = NativeConvert.ToPollEvents(array[j].revents);
			}
			return result;
		}

		public static int poll(Pollfd[] fds, int timeout)
		{
			return poll(fds, (uint)fds.Length, timeout);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sendfile", SetLastError = true)]
		public static extern long sendfile(int out_fd, int in_fd, ref long offset, ulong count);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_stat", SetLastError = true)]
		public static extern int stat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string file_name, out Stat buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fstat", SetLastError = true)]
		public static extern int fstat(int filedes, out Stat buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lstat", SetLastError = true)]
		public static extern int lstat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string file_name, out Stat buf);

		[DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
		private static extern int sys_chmod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, uint mode);

		public static int chmod(string path, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_chmod(path, mode2);
		}

		[DllImport("libc", EntryPoint = "fchmod", SetLastError = true)]
		private static extern int sys_fchmod(int filedes, uint mode);

		public static int fchmod(int filedes, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_fchmod(filedes, mode2);
		}

		[DllImport("libc", EntryPoint = "umask", SetLastError = true)]
		private static extern uint sys_umask(uint mask);

		public static FilePermissions umask(FilePermissions mask)
		{
			return NativeConvert.ToFilePermissions(sys_umask(NativeConvert.FromFilePermissions(mask)));
		}

		[DllImport("libc", EntryPoint = "mkdir", SetLastError = true)]
		private static extern int sys_mkdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, uint mode);

		public static int mkdir(string oldpath, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_mkdir(oldpath, mode2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mknod", SetLastError = true)]
		public static extern int mknod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, FilePermissions mode, ulong dev);

		[DllImport("libc", EntryPoint = "mkfifo", SetLastError = true)]
		private static extern int sys_mkfifo([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, uint mode);

		public static int mkfifo(string pathname, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_mkfifo(pathname, mode2);
		}

		[DllImport("libc", EntryPoint = "fchmodat", SetLastError = true)]
		private static extern int sys_fchmodat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, uint mode, int flags);

		public static int fchmodat(int dirfd, string pathname, FilePermissions mode, AtFlags flags)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_fchmodat(dirfd, pathname, mode2, flags2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fstatat", SetLastError = true)]
		public static extern int fstatat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string file_name, out Stat buf, AtFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_get_utime_now", SetLastError = true)]
		private static extern long get_utime_now();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_get_utime_omit", SetLastError = true)]
		private static extern long get_utime_omit();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_futimens", SetLastError = true)]
		private static extern int sys_futimens(int fd, Timespec[] times);

		public static int futimens(int fd, Timespec[] times)
		{
			if (times != null && times.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return sys_futimens(fd, times);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_utimensat", SetLastError = true)]
		private static extern int sys_utimensat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, Timespec[] times, int flags);

		public static int utimensat(int dirfd, string pathname, Timespec[] times, AtFlags flags)
		{
			if (times != null && times.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_utimensat(dirfd, pathname, times, flags2);
		}

		[DllImport("libc", EntryPoint = "mkdirat", SetLastError = true)]
		private static extern int sys_mkdirat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, uint mode);

		public static int mkdirat(int dirfd, string oldpath, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_mkdirat(dirfd, oldpath, mode2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mknodat", SetLastError = true)]
		public static extern int mknodat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, FilePermissions mode, ulong dev);

		[DllImport("libc", EntryPoint = "mkfifoat", SetLastError = true)]
		private static extern int sys_mkfifoat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, uint mode);

		public static int mkfifoat(int dirfd, string pathname, FilePermissions mode)
		{
			uint mode2 = NativeConvert.FromFilePermissions(mode);
			return sys_mkfifoat(dirfd, pathname, mode2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_statvfs", SetLastError = true)]
		public static extern int statvfs([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, out Statvfs buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fstatvfs", SetLastError = true)]
		public static extern int fstatvfs(int fd, out Statvfs buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		public static extern int gettimeofday(out Timeval tv, out Timezone tz);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int gettimeofday(out Timeval tv, IntPtr ignore);

		public static int gettimeofday(out Timeval tv)
		{
			return gettimeofday(out tv, IntPtr.Zero);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int gettimeofday(IntPtr ignore, out Timezone tz);

		public static int gettimeofday(out Timezone tz)
		{
			return gettimeofday(IntPtr.Zero, out tz);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_settimeofday", SetLastError = true)]
		public static extern int settimeofday(ref Timeval tv, ref Timezone tz);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int settimeofday(ref Timeval tv, IntPtr ignore);

		public static int settimeofday(ref Timeval tv)
		{
			return settimeofday(ref tv, IntPtr.Zero);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_utimes", SetLastError = true)]
		private static extern int sys_utimes([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string filename, Timeval[] tvp);

		public static int utimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return sys_utimes(filename, tvp);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lutimes", SetLastError = true)]
		private static extern int sys_lutimes([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string filename, Timeval[] tvp);

		public static int lutimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return sys_lutimes(filename, tvp);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_futimes", SetLastError = true)]
		private static extern int sys_futimes(int fd, Timeval[] tvp);

		public static int futimes(int fd, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return sys_futimes(fd, tvp);
		}

		private static void CopyUtsname(ref Utsname to, ref _Utsname from)
		{
			try
			{
				to = new Utsname();
				to.sysname = UnixMarshal.PtrToString(from.sysname);
				to.nodename = UnixMarshal.PtrToString(from.nodename);
				to.release = UnixMarshal.PtrToString(from.release);
				to.version = UnixMarshal.PtrToString(from.version);
				to.machine = UnixMarshal.PtrToString(from.machine);
				to.domainname = UnixMarshal.PtrToString(from.domainname);
			}
			finally
			{
				Stdlib.free(from._buf_);
				from._buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_uname", SetLastError = true)]
		private static extern int sys_uname(out _Utsname buf);

		public static int uname(out Utsname buf)
		{
			_Utsname buf2;
			int num = sys_uname(out buf2);
			buf = new Utsname();
			if (num == 0)
			{
				CopyUtsname(ref buf, ref buf2);
			}
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int wait(out int status);

		[DllImport("libc", SetLastError = true)]
		private static extern int waitpid(int pid, out int status, int options);

		public static int waitpid(int pid, out int status, WaitOptions options)
		{
			int options2 = NativeConvert.FromWaitOptions(options);
			return waitpid(pid, out status, options2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFEXITED")]
		private static extern int _WIFEXITED(int status);

		public static bool WIFEXITED(int status)
		{
			return _WIFEXITED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WEXITSTATUS")]
		public static extern int WEXITSTATUS(int status);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFSIGNALED")]
		private static extern int _WIFSIGNALED(int status);

		public static bool WIFSIGNALED(int status)
		{
			return _WIFSIGNALED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WTERMSIG")]
		private static extern int _WTERMSIG(int status);

		public static Signum WTERMSIG(int status)
		{
			return NativeConvert.ToSignum(_WTERMSIG(status));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFSTOPPED")]
		private static extern int _WIFSTOPPED(int status);

		public static bool WIFSTOPPED(int status)
		{
			return _WIFSTOPPED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WSTOPSIG")]
		private static extern int _WSTOPSIG(int status);

		public static Signum WSTOPSIG(int status)
		{
			return NativeConvert.ToSignum(_WSTOPSIG(status));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_openlog", SetLastError = true)]
		private static extern int sys_openlog(IntPtr ident, int option, int facility);

		public static int openlog(IntPtr ident, SyslogOptions option, SyslogFacility defaultFacility)
		{
			int option2 = NativeConvert.FromSyslogOptions(option);
			int facility = NativeConvert.FromSyslogFacility(defaultFacility);
			return sys_openlog(ident, option2, facility);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_syslog", SetLastError = true)]
		private static extern int sys_syslog(int priority, string message);

		public static int syslog(SyslogFacility facility, SyslogLevel level, string message)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num2 = NativeConvert.FromSyslogLevel(level);
			return sys_syslog(num | num2, GetSyslogMessage(message));
		}

		public static int syslog(SyslogLevel level, string message)
		{
			return sys_syslog(NativeConvert.FromSyslogLevel(level), GetSyslogMessage(message));
		}

		private static string GetSyslogMessage(string message)
		{
			return UnixMarshal.EscapeFormatString(message, new char[1] { 'm' });
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogFacility, SyslogLevel, string) instead.")]
		public static int syslog(SyslogFacility facility, SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num2 = NativeConvert.FromSyslogLevel(level);
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = num | num2;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.syslog(array);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogLevel, string) instead.")]
		public static int syslog(SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogLevel(level);
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = num;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.syslog(array);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_closelog", SetLastError = true)]
		public static extern int closelog();

		[DllImport("libc", EntryPoint = "setlogmask", SetLastError = true)]
		private static extern int sys_setlogmask(int mask);

		public static int setlogmask(SyslogLevel mask)
		{
			return sys_setlogmask(NativeConvert.FromSyslogLevel(mask));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_nanosleep", SetLastError = true)]
		public static extern int nanosleep(ref Timespec req, ref Timespec rem);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_stime", SetLastError = true)]
		public static extern int stime(ref long t);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_time", SetLastError = true)]
		public static extern long time(out long t);

		[DllImport("libc", EntryPoint = "access", SetLastError = true)]
		private static extern int sys_access([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, int mode);

		public static int access(string pathname, AccessModes mode)
		{
			int mode2 = NativeConvert.FromAccessModes(mode);
			return sys_access(pathname, mode2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lseek", SetLastError = true)]
		private static extern long sys_lseek(int fd, long offset, int whence);

		public static long lseek(int fd, long offset, SeekFlags whence)
		{
			short whence2 = NativeConvert.FromSeekFlags(whence);
			return sys_lseek(fd, offset, whence2);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int close(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_read", SetLastError = true)]
		public static extern long read(int fd, IntPtr buf, ulong count);

		public unsafe static long read(int fd, void* buf, ulong count)
		{
			return read(fd, (IntPtr)buf, count);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_write", SetLastError = true)]
		public static extern long write(int fd, IntPtr buf, ulong count);

		public unsafe static long write(int fd, void* buf, ulong count)
		{
			return write(fd, (IntPtr)buf, count);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pread", SetLastError = true)]
		public static extern long pread(int fd, IntPtr buf, ulong count, long offset);

		public unsafe static long pread(int fd, void* buf, ulong count, long offset)
		{
			return pread(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pwrite", SetLastError = true)]
		public static extern long pwrite(int fd, IntPtr buf, ulong count, long offset);

		public unsafe static long pwrite(int fd, void* buf, ulong count, long offset)
		{
			return pwrite(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pipe", SetLastError = true)]
		public static extern int pipe(out int reading, out int writing);

		public static int pipe(int[] filedes)
		{
			if (filedes == null || filedes.Length != 2)
			{
				return -1;
			}
			int reading;
			int writing;
			int result = pipe(out reading, out writing);
			filedes[0] = reading;
			filedes[1] = writing;
			return result;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern uint alarm(uint seconds);

		[DllImport("libc", SetLastError = true)]
		public static extern uint sleep(uint seconds);

		[DllImport("libc", SetLastError = true)]
		public static extern uint ualarm(uint usecs, uint interval);

		[DllImport("libc", SetLastError = true)]
		public static extern int pause();

		[DllImport("libc", SetLastError = true)]
		public static extern int chown([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int fchown(int fd, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int lchown([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int chdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path);

		[DllImport("libc", SetLastError = true)]
		public static extern int fchdir(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getcwd", SetLastError = true)]
		public static extern IntPtr getcwd([Out] StringBuilder buf, ulong size);

		public static StringBuilder getcwd(StringBuilder buf)
		{
			getcwd(buf, (ulong)buf.Capacity);
			return buf;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int dup(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int dup2(int fd, int fd2);

		[DllImport("libc", SetLastError = true)]
		public static extern int execve([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, string[] argv, string[] envp);

		[DllImport("libc", SetLastError = true)]
		public static extern int fexecve(int fd, string[] argv, string[] envp);

		[DllImport("libc", SetLastError = true)]
		public static extern int execv([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, string[] argv);

		[DllImport("libc", SetLastError = true)]
		public static extern int execvp([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, string[] argv);

		[DllImport("libc", SetLastError = true)]
		public static extern int nice(int inc);

		[DllImport("libc", SetLastError = true)]
		[CLSCompliant(false)]
		public static extern int _exit(int status);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fpathconf", SetLastError = true)]
		public static extern long fpathconf(int filedes, PathconfName name, Errno defaultError);

		public static long fpathconf(int filedes, PathconfName name)
		{
			return fpathconf(filedes, name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pathconf", SetLastError = true)]
		public static extern long pathconf([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, PathconfName name, Errno defaultError);

		public static long pathconf(string path, PathconfName name)
		{
			return pathconf(path, name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sysconf", SetLastError = true)]
		public static extern long sysconf(SysconfName name, Errno defaultError);

		public static long sysconf(SysconfName name)
		{
			return sysconf(name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_confstr", SetLastError = true)]
		public static extern ulong confstr(ConfstrName name, [Out] StringBuilder buf, ulong len);

		[DllImport("libc", SetLastError = true)]
		public static extern int getpid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getppid();

		[DllImport("libc", SetLastError = true)]
		public static extern int setpgid(int pid, int pgid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getpgid(int pid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setpgrp();

		[DllImport("libc", SetLastError = true)]
		public static extern int getpgrp();

		[DllImport("libc", SetLastError = true)]
		public static extern int setsid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getsid(int pid);

		[DllImport("libc", SetLastError = true)]
		public static extern uint getuid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint geteuid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint getgid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint getegid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getgroups(int size, uint[] list);

		public static int getgroups(uint[] list)
		{
			return getgroups(list.Length, list);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int setuid(uint uid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setreuid(uint ruid, uint euid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setregid(uint rgid, uint egid);

		[DllImport("libc", SetLastError = true)]
		public static extern int seteuid(uint euid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setegid(uint uid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setgid(uint gid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getresuid(out uint ruid, out uint euid, out uint suid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getresgid(out uint rgid, out uint egid, out uint sgid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setresuid(uint ruid, uint euid, uint suid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setresgid(uint rgid, uint egid, uint sgid);

		[DllImport("libc", EntryPoint = "ttyname", SetLastError = true)]
		private static extern IntPtr sys_ttyname(int fd);

		public static string ttyname(int fd)
		{
			lock (tty_lock)
			{
				return UnixMarshal.PtrToString(sys_ttyname(fd));
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_ttyname_r", SetLastError = true)]
		public static extern int ttyname_r(int fd, [Out] StringBuilder buf, ulong buflen);

		public static int ttyname_r(int fd, StringBuilder buf)
		{
			return ttyname_r(fd, buf, (ulong)buf.Capacity);
		}

		[DllImport("libc", EntryPoint = "isatty")]
		private static extern int sys_isatty(int fd);

		public static bool isatty(int fd)
		{
			return sys_isatty(fd) == 1;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int link([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath);

		[DllImport("libc", SetLastError = true)]
		public static extern int symlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath);

		private static int ReadlinkIntoStringBuilder(DoReadlinkFun doReadlink, [Out] StringBuilder buf, ulong bufsiz)
		{
			int num;
			long num2;
			checked
			{
				num = (int)bufsiz;
				byte[] array = new byte[num];
				num2 = doReadlink(array);
				if (num2 < 0)
				{
					return (int)num2;
				}
				buf.Length = 0;
				char[] chars = UnixEncoding.Instance.GetChars(array, 0, (int)num2);
				buf.Append(chars, 0, System.Math.Min(num, chars.Length));
			}
			if (num2 == num)
			{
				buf.Append(new string('\0', num - buf.Length));
			}
			return buf.Length;
		}

		public static int readlink(string path, [Out] StringBuilder buf, ulong bufsiz)
		{
			_003C_003Ec__DisplayClass327_0 _003C_003Ec__DisplayClass327_ = new _003C_003Ec__DisplayClass327_0();
			_003C_003Ec__DisplayClass327_.path = path;
			return ReadlinkIntoStringBuilder(_003C_003Ec__DisplayClass327_._003Creadlink_003Eb__0, buf, bufsiz);
		}

		public static int readlink(string path, [Out] StringBuilder buf)
		{
			return readlink(path, buf, (ulong)buf.Capacity);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readlink", SetLastError = true)]
		private static extern long readlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, byte[] buf, ulong bufsiz);

		public static long readlink(string path, byte[] buf)
		{
			return readlink(path, buf, (ulong)buf.LongLength);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int unlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname);

		[DllImport("libc", SetLastError = true)]
		public static extern int rmdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname);

		[DllImport("libc", SetLastError = true)]
		public static extern int tcgetpgrp(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int tcsetpgrp(int fd, int pgrp);

		[DllImport("libc", EntryPoint = "getlogin", SetLastError = true)]
		private static extern IntPtr sys_getlogin();

		public static string getlogin()
		{
			lock (getlogin_lock)
			{
				return UnixMarshal.PtrToString(sys_getlogin());
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getlogin_r", SetLastError = true)]
		public static extern int getlogin_r([Out] StringBuilder name, ulong bufsize);

		public static int getlogin_r(StringBuilder name)
		{
			return getlogin_r(name, (ulong)name.Capacity);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int setlogin(string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gethostname", SetLastError = true)]
		public static extern int gethostname([Out] StringBuilder name, ulong len);

		public static int gethostname(StringBuilder name)
		{
			return gethostname(name, (ulong)name.Capacity);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sethostname", SetLastError = true)]
		public static extern int sethostname(string name, ulong len);

		public static int sethostname(string name)
		{
			return sethostname(name, (ulong)name.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gethostid", SetLastError = true)]
		public static extern long gethostid();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sethostid", SetLastError = true)]
		public static extern int sethostid(long hostid);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getdomainname", SetLastError = true)]
		public static extern int getdomainname([Out] StringBuilder name, ulong len);

		public static int getdomainname(StringBuilder name)
		{
			return getdomainname(name, (ulong)name.Capacity);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setdomainname", SetLastError = true)]
		public static extern int setdomainname(string name, ulong len);

		public static int setdomainname(string name)
		{
			return setdomainname(name, (ulong)name.Length);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int vhangup();

		[DllImport("libc", SetLastError = true)]
		public static extern int revoke([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string file);

		[DllImport("libc", SetLastError = true)]
		public static extern int acct([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string filename);

		[DllImport("libc", EntryPoint = "getusershell", SetLastError = true)]
		private static extern IntPtr sys_getusershell();

		public static string getusershell()
		{
			lock (usershell_lock)
			{
				return UnixMarshal.PtrToString(sys_getusershell());
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setusershell", SetLastError = true)]
		private static extern int sys_setusershell();

		public static int setusershell()
		{
			lock (usershell_lock)
			{
				return sys_setusershell();
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endusershell", SetLastError = true)]
		private static extern int sys_endusershell();

		public static int endusershell()
		{
			lock (usershell_lock)
			{
				return sys_endusershell();
			}
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int chroot([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path);

		[DllImport("libc", SetLastError = true)]
		public static extern int fsync(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int fdatasync(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sync", SetLastError = true)]
		public static extern int sync();

		[DllImport("libc", SetLastError = true)]
		[Obsolete("Dropped in POSIX 1003.1-2001.  Use Syscall.sysconf (SysconfName._SC_PAGESIZE).")]
		public static extern int getpagesize();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_truncate", SetLastError = true)]
		public static extern int truncate([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string path, long length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_ftruncate", SetLastError = true)]
		public static extern int ftruncate(int fd, long length);

		[DllImport("libc", SetLastError = true)]
		public static extern int getdtablesize();

		[DllImport("libc", SetLastError = true)]
		public static extern int brk(IntPtr end_data_segment);

		[DllImport("libc", SetLastError = true)]
		public static extern IntPtr sbrk(IntPtr increment);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lockf", SetLastError = true)]
		public static extern int lockf(int fd, LockfCommand cmd, long len);

		[Obsolete("This is insecure and should not be used", true)]
		public static string crypt(string key, string salt)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[Obsolete("This is insecure and should not be used", true)]
		public static int encrypt(byte[] block, bool decode)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_swab", SetLastError = true)]
		public static extern int swab(IntPtr from, IntPtr to, long n);

		public unsafe static void swab(void* from, void* to, long n)
		{
			swab((IntPtr)from, (IntPtr)to, n);
		}

		[DllImport("libc", EntryPoint = "faccessat", SetLastError = true)]
		private static extern int sys_faccessat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, int mode, int flags);

		public static int faccessat(int dirfd, string pathname, AccessModes mode, AtFlags flags)
		{
			int mode2 = NativeConvert.FromAccessModes(mode);
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_faccessat(dirfd, pathname, mode2, flags2);
		}

		[DllImport("libc", EntryPoint = "fchownat", SetLastError = true)]
		private static extern int sys_fchownat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, uint owner, uint group, int flags);

		public static int fchownat(int dirfd, string pathname, uint owner, uint group, AtFlags flags)
		{
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_fchownat(dirfd, pathname, owner, group, flags2);
		}

		[DllImport("libc", EntryPoint = "linkat", SetLastError = true)]
		private static extern int sys_linkat(int olddirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, int newdirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath, int flags);

		public static int linkat(int olddirfd, string oldpath, int newdirfd, string newpath, AtFlags flags)
		{
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_linkat(olddirfd, oldpath, newdirfd, newpath, flags2);
		}

		public static int readlinkat(int dirfd, string pathname, [Out] StringBuilder buf, ulong bufsiz)
		{
			_003C_003Ec__DisplayClass381_0 _003C_003Ec__DisplayClass381_ = new _003C_003Ec__DisplayClass381_0();
			_003C_003Ec__DisplayClass381_.dirfd = dirfd;
			_003C_003Ec__DisplayClass381_.pathname = pathname;
			return ReadlinkIntoStringBuilder(_003C_003Ec__DisplayClass381_._003Creadlinkat_003Eb__0, buf, bufsiz);
		}

		public static int readlinkat(int dirfd, string pathname, [Out] StringBuilder buf)
		{
			return readlinkat(dirfd, pathname, buf, (ulong)buf.Capacity);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readlinkat", SetLastError = true)]
		private static extern long readlinkat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, byte[] buf, ulong bufsiz);

		public static long readlinkat(int dirfd, string pathname, byte[] buf)
		{
			return readlinkat(dirfd, pathname, buf, (ulong)buf.LongLength);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int symlinkat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string oldpath, int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string newpath);

		[DllImport("libc", EntryPoint = "unlinkat", SetLastError = true)]
		private static extern int sys_unlinkat(int dirfd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string pathname, int flags);

		public static int unlinkat(int dirfd, string pathname, AtFlags flags)
		{
			int flags2 = NativeConvert.FromAtFlags(flags);
			return sys_unlinkat(dirfd, pathname, flags2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_utime", SetLastError = true)]
		private static extern int sys_utime([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Mono.Unix.Native.FileNameMarshaler")] string filename, ref Utimbuf buf, int use_buf);

		public static int utime(string filename, ref Utimbuf buf)
		{
			return sys_utime(filename, ref buf, 1);
		}

		public static int utime(string filename)
		{
			Utimbuf buf = default(Utimbuf);
			return sys_utime(filename, ref buf, 0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readv", SetLastError = true)]
		private static extern long sys_readv(int fd, Iovec[] iov, int iovcnt);

		public static long readv(int fd, Iovec[] iov)
		{
			return sys_readv(fd, iov, iov.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_writev", SetLastError = true)]
		private static extern long sys_writev(int fd, Iovec[] iov, int iovcnt);

		public static long writev(int fd, Iovec[] iov)
		{
			return sys_writev(fd, iov, iov.Length);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_preadv", SetLastError = true)]
		private static extern long sys_preadv(int fd, Iovec[] iov, int iovcnt, long offset);

		public static long preadv(int fd, Iovec[] iov, long offset)
		{
			return sys_preadv(fd, iov, iov.Length, offset);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pwritev", SetLastError = true)]
		private static extern long sys_pwritev(int fd, Iovec[] iov, int iovcnt, long offset);

		public static long pwritev(int fd, Iovec[] iov, long offset)
		{
			return sys_pwritev(fd, iov, iov.Length, offset);
		}

		[DllImport("libc")]
		public static extern uint htonl(uint hostlong);

		[DllImport("libc")]
		public static extern ushort htons(ushort hostshort);

		[DllImport("libc")]
		public static extern uint ntohl(uint netlong);

		[DllImport("libc")]
		public static extern ushort ntohs(ushort netshort);

		[DllImport("libc", EntryPoint = "socket", SetLastError = true)]
		private static extern int sys_socket(int domain, int type, int protocol);

		public static int socket(UnixAddressFamily domain, UnixSocketType type, UnixSocketFlags flags, UnixSocketProtocol protocol)
		{
			int domain2 = NativeConvert.FromUnixAddressFamily(domain);
			int num = NativeConvert.FromUnixSocketType(type);
			int num2 = NativeConvert.FromUnixSocketFlags(flags);
			int protocol2 = ((protocol != 0) ? NativeConvert.FromUnixSocketProtocol(protocol) : 0);
			return sys_socket(domain2, num | num2, protocol2);
		}

		public static int socket(UnixAddressFamily domain, UnixSocketType type, UnixSocketProtocol protocol)
		{
			return socket(domain, type, (UnixSocketFlags)0, protocol);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_socketpair", SetLastError = true)]
		private static extern int sys_socketpair(int domain, int type, int protocol, out int socket1, out int socket2);

		public static int socketpair(UnixAddressFamily domain, UnixSocketType type, UnixSocketFlags flags, UnixSocketProtocol protocol, out int socket1, out int socket2)
		{
			int domain2 = NativeConvert.FromUnixAddressFamily(domain);
			int num = NativeConvert.FromUnixSocketType(type);
			int num2 = NativeConvert.FromUnixSocketFlags(flags);
			int protocol2 = ((protocol != 0) ? NativeConvert.FromUnixSocketProtocol(protocol) : 0);
			return sys_socketpair(domain2, num | num2, protocol2, out socket1, out socket2);
		}

		public static int socketpair(UnixAddressFamily domain, UnixSocketType type, UnixSocketProtocol protocol, out int socket1, out int socket2)
		{
			return socketpair(domain, type, (UnixSocketFlags)0, protocol, out socket1, out socket2);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int sockatmark(int socket);

		[DllImport("libc", SetLastError = true)]
		public static extern int listen(int socket, int backlog);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getsockopt", SetLastError = true)]
		private unsafe static extern int sys_getsockopt(int socket, int level, int option_name, void* option_value, ref long option_len);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getsockopt_timeval", SetLastError = true)]
		private static extern int sys_getsockopt_timeval(int socket, int level, int option_name, out Timeval option_value);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getsockopt_linger", SetLastError = true)]
		private static extern int sys_getsockopt_linger(int socket, int level, int option_name, out Linger option_value);

		public unsafe static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, void* option_value, ref long option_len)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_getsockopt(socket, level2, option_name2, option_value, ref option_len);
		}

		public unsafe static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, IntPtr option_value, ref long option_len)
		{
			return getsockopt(socket, level, option_name, (void*)option_value, ref option_len);
		}

		public unsafe static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, out int option_value)
		{
			long option_len = 4L;
			int num = default(int);
			int num2 = getsockopt(socket, level, option_name, &num, ref option_len);
			if (num2 != -1 && option_len != 4)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				num2 = -1;
			}
			option_value = num;
			return num2;
		}

		public unsafe static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, byte[] option_value, ref long option_len)
		{
			if (option_len > ((option_value != null) ? option_value.Length : 0))
			{
				throw new ArgumentOutOfRangeException("option_len", "option_len > (option_value == null ? 0 : option_value.Length)");
			}
			fixed (byte* option_value2 = option_value)
			{
				return getsockopt(socket, level, option_name, option_value2, ref option_len);
			}
		}

		public static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, out Timeval option_value)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_getsockopt_timeval(socket, level2, option_name2, out option_value);
		}

		public static int getsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, out Linger option_value)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_getsockopt_linger(socket, level2, option_name2, out option_value);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setsockopt", SetLastError = true)]
		private unsafe static extern int sys_setsockopt(int socket, int level, int option_name, void* option_value, long option_len);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setsockopt_timeval", SetLastError = true)]
		private static extern int sys_setsockopt_timeval(int socket, int level, int option_name, ref Timeval option_value);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setsockopt_linger", SetLastError = true)]
		private static extern int sys_setsockopt_linger(int socket, int level, int option_name, ref Linger option_value);

		public unsafe static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, void* option_value, long option_len)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_setsockopt(socket, level2, option_name2, option_value, option_len);
		}

		public unsafe static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, IntPtr option_value, long option_len)
		{
			return setsockopt(socket, level, option_name, (void*)option_value, option_len);
		}

		public unsafe static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, int option_value)
		{
			return setsockopt(socket, level, option_name, &option_value, 4L);
		}

		public unsafe static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, byte[] option_value, long option_len)
		{
			if (option_len > ((option_value != null) ? option_value.Length : 0))
			{
				throw new ArgumentOutOfRangeException("option_len", "option_len > (option_value == null ? 0 : option_value.Length)");
			}
			fixed (byte* option_value2 = option_value)
			{
				return setsockopt(socket, level, option_name, option_value2, option_len);
			}
		}

		public static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, Timeval option_value)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_setsockopt_timeval(socket, level2, option_name2, ref option_value);
		}

		public static int setsockopt(int socket, UnixSocketProtocol level, UnixSocketOptionName option_name, Linger option_value)
		{
			int level2 = NativeConvert.FromUnixSocketProtocol(level);
			int option_name2 = NativeConvert.FromUnixSocketOptionName(option_name);
			return sys_setsockopt_linger(socket, level2, option_name2, ref option_value);
		}

		[DllImport("libc", EntryPoint = "shutdown", SetLastError = true)]
		private static extern int sys_shutdown(int socket, int how);

		public static int shutdown(int socket, ShutdownOption how)
		{
			int how2 = NativeConvert.FromShutdownOption(how);
			return sys_shutdown(socket, how2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_recv", SetLastError = true)]
		private unsafe static extern long sys_recv(int socket, void* buffer, ulong length, int flags);

		public unsafe static long recv(int socket, void* buffer, ulong length, MessageFlags flags)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			return sys_recv(socket, buffer, length, flags2);
		}

		public unsafe static long recv(int socket, IntPtr buffer, ulong length, MessageFlags flags)
		{
			return recv(socket, (void*)buffer, length, flags);
		}

		public unsafe static long recv(int socket, byte[] buffer, ulong length, MessageFlags flags)
		{
			if (length > (ulong)((buffer == null) ? 0 : buffer.LongLength))
			{
				throw new ArgumentOutOfRangeException("length", "length > (buffer == null ? 0 : buffer.LongLength)");
			}
			fixed (byte* buffer2 = buffer)
			{
				return recv(socket, buffer2, length, flags);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_send", SetLastError = true)]
		private unsafe static extern long sys_send(int socket, void* message, ulong length, int flags);

		public unsafe static long send(int socket, void* message, ulong length, MessageFlags flags)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			return sys_send(socket, message, length, flags2);
		}

		public unsafe static long send(int socket, IntPtr message, ulong length, MessageFlags flags)
		{
			return send(socket, (void*)message, length, flags);
		}

		public unsafe static long send(int socket, byte[] message, ulong length, MessageFlags flags)
		{
			if (length > (ulong)((message == null) ? 0 : message.LongLength))
			{
				throw new ArgumentOutOfRangeException("length", "length > (message == null ? 0 : message.LongLength)");
			}
			fixed (byte* message2 = message)
			{
				return send(socket, message2, length, flags);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_bind", SetLastError = true)]
		private unsafe static extern int sys_bind(int socket, _SockaddrHeader* address);

		public unsafe static int bind(int socket, Sockaddr address)
		{
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, false);
					return sys_bind(socket, Sockaddr.GetNative(&sockaddrDynamic, addr));
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_connect", SetLastError = true)]
		private unsafe static extern int sys_connect(int socket, _SockaddrHeader* address);

		public unsafe static int connect(int socket, Sockaddr address)
		{
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, false);
					return sys_connect(socket, Sockaddr.GetNative(&sockaddrDynamic, addr));
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_accept", SetLastError = true)]
		private unsafe static extern int sys_accept(int socket, _SockaddrHeader* address);

		public unsafe static int accept(int socket, Sockaddr address)
		{
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, true);
					int result = sys_accept(socket, Sockaddr.GetNative(&sockaddrDynamic, addr));
					sockaddrDynamic.Update(address);
					return result;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_accept4", SetLastError = true)]
		private unsafe static extern int sys_accept4(int socket, _SockaddrHeader* address, int flags);

		public unsafe static int accept4(int socket, Sockaddr address, UnixSocketFlags flags)
		{
			int flags2 = NativeConvert.FromUnixSocketFlags(flags);
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, true);
					int result = sys_accept4(socket, Sockaddr.GetNative(&sockaddrDynamic, addr), flags2);
					sockaddrDynamic.Update(address);
					return result;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpeername", SetLastError = true)]
		private unsafe static extern int sys_getpeername(int socket, _SockaddrHeader* address);

		public unsafe static int getpeername(int socket, Sockaddr address)
		{
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, true);
					int result = sys_getpeername(socket, Sockaddr.GetNative(&sockaddrDynamic, addr));
					sockaddrDynamic.Update(address);
					return result;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getsockname", SetLastError = true)]
		private unsafe static extern int sys_getsockname(int socket, _SockaddrHeader* address);

		public unsafe static int getsockname(int socket, Sockaddr address)
		{
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, true);
					int result = sys_getsockname(socket, Sockaddr.GetNative(&sockaddrDynamic, addr));
					sockaddrDynamic.Update(address);
					return result;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_recvfrom", SetLastError = true)]
		private unsafe static extern long sys_recvfrom(int socket, void* buffer, ulong length, int flags, _SockaddrHeader* address);

		public unsafe static long recvfrom(int socket, void* buffer, ulong length, MessageFlags flags, Sockaddr address)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, true);
					long result = sys_recvfrom(socket, buffer, length, flags2, Sockaddr.GetNative(&sockaddrDynamic, addr));
					sockaddrDynamic.Update(address);
					return result;
				}
			}
		}

		public unsafe static long recvfrom(int socket, IntPtr buffer, ulong length, MessageFlags flags, Sockaddr address)
		{
			return recvfrom(socket, (void*)buffer, length, flags, address);
		}

		public unsafe static long recvfrom(int socket, byte[] buffer, ulong length, MessageFlags flags, Sockaddr address)
		{
			if (length > (ulong)buffer.LongLength)
			{
				throw new ArgumentOutOfRangeException("length", "length > buffer.LongLength");
			}
			fixed (byte* buffer2 = buffer)
			{
				return recvfrom(socket, buffer2, length, flags, address);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sendto", SetLastError = true)]
		private unsafe static extern long sys_sendto(int socket, void* message, ulong length, int flags, _SockaddrHeader* address);

		public unsafe static long sendto(int socket, void* message, ulong length, MessageFlags flags, Sockaddr address)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			fixed (SockaddrType* addr = &Sockaddr.GetAddress(address).type)
			{
				fixed (byte* data = Sockaddr.GetDynamicData(address))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(address, data, false);
					return sys_sendto(socket, message, length, flags2, Sockaddr.GetNative(&sockaddrDynamic, addr));
				}
			}
		}

		public unsafe static long sendto(int socket, IntPtr message, ulong length, MessageFlags flags, Sockaddr address)
		{
			return sendto(socket, (void*)message, length, flags, address);
		}

		public unsafe static long sendto(int socket, byte[] message, ulong length, MessageFlags flags, Sockaddr address)
		{
			if (length > (ulong)message.LongLength)
			{
				throw new ArgumentOutOfRangeException("length", "length > message.LongLength");
			}
			fixed (byte* message2 = message)
			{
				return sendto(socket, message2, length, flags, address);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_recvmsg", SetLastError = true)]
		private unsafe static extern long sys_recvmsg(int socket, ref _Msghdr message, _SockaddrHeader* msg_name, int flags);

		public unsafe static long recvmsg(int socket, Msghdr message, MessageFlags flags)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			Sockaddr msg_name = message.msg_name;
			fixed (byte* ptr_msg_control = message.msg_control)
			{
				fixed (Iovec* ptr_msg_iov = message.msg_iov)
				{
					_Msghdr message2 = new _Msghdr(message, ptr_msg_iov, ptr_msg_control);
					long result;
					fixed (SockaddrType* addr = &Sockaddr.GetAddress(msg_name).type)
					{
						fixed (byte* data = Sockaddr.GetDynamicData(msg_name))
						{
							_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(msg_name, data, true);
							result = sys_recvmsg(socket, ref message2, Sockaddr.GetNative(&sockaddrDynamic, addr), flags2);
							sockaddrDynamic.Update(msg_name);
						}
					}
					message2.Update(message);
					return result;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sendmsg", SetLastError = true)]
		private unsafe static extern long sys_sendmsg(int socket, ref _Msghdr message, _SockaddrHeader* msg_name, int flags);

		public unsafe static long sendmsg(int socket, Msghdr message, MessageFlags flags)
		{
			int flags2 = NativeConvert.FromMessageFlags(flags);
			Sockaddr msg_name = message.msg_name;
			fixed (byte* ptr_msg_control = message.msg_control)
			{
				fixed (Iovec* ptr_msg_iov = message.msg_iov)
				{
					_Msghdr message2 = new _Msghdr(message, ptr_msg_iov, ptr_msg_control);
					fixed (SockaddrType* addr = &Sockaddr.GetAddress(msg_name).type)
					{
						fixed (byte* data = Sockaddr.GetDynamicData(msg_name))
						{
							_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(msg_name, data, false);
							return sys_sendmsg(socket, ref message2, Sockaddr.GetNative(&sockaddrDynamic, addr), flags2);
						}
					}
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_FIRSTHDR", SetLastError = true)]
		private unsafe static extern long CMSG_FIRSTHDR(byte* msg_control, long msg_controllen);

		public unsafe static long CMSG_FIRSTHDR(Msghdr msgh)
		{
			if (msgh.msg_control == null && msgh.msg_controllen != 0L)
			{
				throw new ArgumentException("msgh.msg_control == null && msgh.msg_controllen != 0", "msgh");
			}
			if (msgh.msg_control != null && msgh.msg_controllen > msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			fixed (byte* msg_control = msgh.msg_control)
			{
				return CMSG_FIRSTHDR(msg_control, msgh.msg_controllen);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_NXTHDR", SetLastError = true)]
		private unsafe static extern long CMSG_NXTHDR(byte* msg_control, long msg_controllen, long cmsg);

		public unsafe static long CMSG_NXTHDR(Msghdr msgh, long cmsg)
		{
			if (msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0 || cmsg + Cmsghdr.Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			fixed (byte* msg_control = msgh.msg_control)
			{
				return CMSG_NXTHDR(msg_control, msgh.msg_controllen, cmsg);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_DATA", SetLastError = true)]
		private unsafe static extern long CMSG_DATA(byte* msg_control, long msg_controllen, long cmsg);

		public unsafe static long CMSG_DATA(Msghdr msgh, long cmsg)
		{
			if (msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0 || cmsg + Cmsghdr.Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			fixed (byte* msg_control = msgh.msg_control)
			{
				return CMSG_DATA(msg_control, msgh.msg_controllen, cmsg);
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_ALIGN", SetLastError = true)]
		public static extern ulong CMSG_ALIGN(ulong length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_SPACE", SetLastError = true)]
		public static extern ulong CMSG_SPACE(ulong length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_CMSG_LEN", SetLastError = true)]
		public static extern ulong CMSG_LEN(ulong length);
	}
}
