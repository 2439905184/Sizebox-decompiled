using System;
using System.IO;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public abstract class UnixFileSystemInfo
	{
		private Stat stat;

		private string fullPath;

		private string originalPath;

		private bool valid;

		internal const FileSpecialAttributes AllSpecialAttributes = FileSpecialAttributes.SetUserId | FileSpecialAttributes.SetGroupId | FileSpecialAttributes.Sticky;

		internal const FileTypes AllFileTypes = (FileTypes)61440;

		protected string FullPath
		{
			get
			{
				return fullPath;
			}
			set
			{
				if (fullPath != value)
				{
					UnixPath.CheckPath(value);
					valid = false;
					fullPath = value;
				}
			}
		}

		protected string OriginalPath
		{
			get
			{
				return originalPath;
			}
			set
			{
				originalPath = value;
			}
		}

		public virtual string FullName
		{
			get
			{
				return FullPath;
			}
		}

		public abstract string Name { get; }

		public bool Exists
		{
			get
			{
				Refresh(true);
				return valid;
			}
		}

		public long Device
		{
			get
			{
				AssertValid();
				return Convert.ToInt64(stat.st_dev);
			}
		}

		public long Inode
		{
			get
			{
				AssertValid();
				return Convert.ToInt64(stat.st_ino);
			}
		}

		[CLSCompliant(false)]
		public FilePermissions Protection
		{
			get
			{
				AssertValid();
				return stat.st_mode;
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.chmod(FullPath, value));
			}
		}

		public FileTypes FileType
		{
			get
			{
				AssertValid();
				return (FileTypes)(stat.st_mode & FilePermissions.S_IFMT);
			}
		}

		public FileAccessPermissions FileAccessPermissions
		{
			get
			{
				AssertValid();
				return (FileAccessPermissions)(stat.st_mode & FilePermissions.ACCESSPERMS);
			}
			set
			{
				AssertValid();
				int st_mode = (int)stat.st_mode;
				st_mode &= -512;
				st_mode |= (int)value;
				Protection = (FilePermissions)st_mode;
			}
		}

		public FileSpecialAttributes FileSpecialAttributes
		{
			get
			{
				AssertValid();
				return (FileSpecialAttributes)(stat.st_mode & (FilePermissions.S_ISUID | FilePermissions.S_ISGID | FilePermissions.S_ISVTX));
			}
			set
			{
				AssertValid();
				int st_mode = (int)stat.st_mode;
				st_mode &= -3585;
				st_mode |= (int)value;
				Protection = (FilePermissions)st_mode;
			}
		}

		public long LinkCount
		{
			get
			{
				AssertValid();
				return Convert.ToInt64(stat.st_nlink);
			}
		}

		public UnixUserInfo OwnerUser
		{
			get
			{
				AssertValid();
				return new UnixUserInfo(stat.st_uid);
			}
		}

		public long OwnerUserId
		{
			get
			{
				AssertValid();
				return stat.st_uid;
			}
		}

		public UnixGroupInfo OwnerGroup
		{
			get
			{
				AssertValid();
				return new UnixGroupInfo(stat.st_gid);
			}
		}

		public long OwnerGroupId
		{
			get
			{
				AssertValid();
				return stat.st_gid;
			}
		}

		public long DeviceType
		{
			get
			{
				AssertValid();
				return Convert.ToInt64(stat.st_rdev);
			}
		}

		public long Length
		{
			get
			{
				AssertValid();
				return stat.st_size;
			}
		}

		public long BlockSize
		{
			get
			{
				AssertValid();
				return stat.st_blksize;
			}
		}

		public long BlocksAllocated
		{
			get
			{
				AssertValid();
				return stat.st_blocks;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				AssertValid();
				return NativeConvert.ToDateTime(stat.st_atime, stat.st_atime_nsec);
			}
		}

		public DateTime LastAccessTimeUtc
		{
			get
			{
				return LastAccessTime.ToUniversalTime();
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				AssertValid();
				return NativeConvert.ToDateTime(stat.st_mtime, stat.st_mtime_nsec);
			}
		}

		public DateTime LastWriteTimeUtc
		{
			get
			{
				return LastWriteTime.ToUniversalTime();
			}
		}

		public DateTime LastStatusChangeTime
		{
			get
			{
				AssertValid();
				return NativeConvert.ToDateTime(stat.st_ctime, stat.st_ctime_nsec);
			}
		}

		public DateTime LastStatusChangeTimeUtc
		{
			get
			{
				return LastStatusChangeTime.ToUniversalTime();
			}
		}

		public bool IsDirectory
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFDIR);
			}
		}

		public bool IsCharacterDevice
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFCHR);
			}
		}

		public bool IsBlockDevice
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFBLK);
			}
		}

		public bool IsRegularFile
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFREG);
			}
		}

		public bool IsFifo
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFIFO);
			}
		}

		public bool IsSymbolicLink
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFLNK);
			}
		}

		public bool IsSocket
		{
			get
			{
				AssertValid();
				return IsFileType(stat.st_mode, FilePermissions.S_IFSOCK);
			}
		}

		public bool IsSetUser
		{
			get
			{
				AssertValid();
				return IsSet(stat.st_mode, FilePermissions.S_ISUID);
			}
		}

		public bool IsSetGroup
		{
			get
			{
				AssertValid();
				return IsSet(stat.st_mode, FilePermissions.S_ISGID);
			}
		}

		public bool IsSticky
		{
			get
			{
				AssertValid();
				return IsSet(stat.st_mode, FilePermissions.S_ISVTX);
			}
		}

		protected UnixFileSystemInfo(string path)
		{
			UnixPath.CheckPath(path);
			originalPath = path;
			fullPath = UnixPath.GetFullPath(path);
			Refresh(true);
		}

		internal UnixFileSystemInfo(string path, Stat stat)
		{
			originalPath = path;
			fullPath = UnixPath.GetFullPath(path);
			this.stat = stat;
			valid = true;
		}

		private void AssertValid()
		{
			Refresh(false);
			if (!valid)
			{
				throw new InvalidOperationException("Path doesn't exist!");
			}
		}

		internal static bool IsFileType(FilePermissions mode, FilePermissions type)
		{
			return (mode & FilePermissions.S_IFMT) == type;
		}

		internal static bool IsSet(FilePermissions mode, FilePermissions type)
		{
			return (mode & type) == type;
		}

		[CLSCompliant(false)]
		public bool CanAccess(AccessModes mode)
		{
			return Syscall.access(FullPath, mode) == 0;
		}

		public UnixFileSystemInfo CreateLink(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.link(FullName, path));
			return GetFileSystemEntry(path);
		}

		public UnixSymbolicLinkInfo CreateSymbolicLink(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.symlink(FullName, path));
			return new UnixSymbolicLinkInfo(path);
		}

		public abstract void Delete();

		[CLSCompliant(false)]
		public long GetConfigurationValue(PathconfName name)
		{
			long num = Syscall.pathconf(FullPath, name);
			if (num == -1 && Stdlib.GetLastError() != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public void Refresh()
		{
			Refresh(true);
		}

		internal void Refresh(bool force)
		{
			if (!valid || force)
			{
				valid = GetFileStatus(FullPath, out stat);
			}
		}

		protected virtual bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.stat(path, out stat) == 0;
		}

		public void SetLength(long length)
		{
			int num;
			do
			{
				num = Syscall.truncate(FullPath, length);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public virtual void SetOwner(long owner, long group)
		{
			uint owner2 = Convert.ToUInt32(owner);
			uint group2 = Convert.ToUInt32(group);
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.chown(FullPath, owner2, group2));
		}

		public void SetOwner(string owner)
		{
			Passwd passwd = Syscall.getpwnam(owner);
			if (passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "owner");
			}
			uint pw_uid = passwd.pw_uid;
			uint pw_gid = passwd.pw_gid;
			SetOwner(pw_uid, pw_gid);
		}

		public void SetOwner(string owner, string group)
		{
			long owner2 = -1L;
			if (owner != null)
			{
				owner2 = new UnixUserInfo(owner).UserId;
			}
			long group2 = -1L;
			if (group != null)
			{
				group2 = new UnixGroupInfo(group).GroupId;
			}
			SetOwner(owner2, group2);
		}

		public void SetOwner(UnixUserInfo owner)
		{
			long group;
			long owner2 = (group = -1L);
			if (owner != null)
			{
				owner2 = owner.UserId;
				group = owner.GroupId;
			}
			SetOwner(owner2, group);
		}

		public void SetOwner(UnixUserInfo owner, UnixGroupInfo group)
		{
			long group2;
			long owner2 = (group2 = -1L);
			if (owner != null)
			{
				owner2 = owner.UserId;
			}
			if (group != null)
			{
				group2 = owner.GroupId;
			}
			SetOwner(owner2, group2);
		}

		public override string ToString()
		{
			return FullPath;
		}

		public Stat ToStat()
		{
			AssertValid();
			return stat;
		}

		public static UnixFileSystemInfo GetFileSystemEntry(string path)
		{
			UnixFileSystemInfo entry;
			if (TryGetFileSystemEntry(path, out entry))
			{
				return entry;
			}
			UnixMarshal.ThrowExceptionForLastError();
			throw new DirectoryNotFoundException("UnixMarshal.ThrowExceptionForLastError didn't throw?!");
		}

		public static bool TryGetFileSystemEntry(string path, out UnixFileSystemInfo entry)
		{
			Stat buf;
			if (Syscall.lstat(path, out buf) == -1)
			{
				if (Stdlib.GetLastError() == Errno.ENOENT)
				{
					entry = new UnixFileInfo(path);
					return true;
				}
				entry = null;
				return false;
			}
			if (IsFileType(buf.st_mode, FilePermissions.S_IFDIR))
			{
				entry = new UnixDirectoryInfo(path, buf);
			}
			else if (IsFileType(buf.st_mode, FilePermissions.S_IFLNK))
			{
				entry = new UnixSymbolicLinkInfo(path, buf);
			}
			else
			{
				entry = new UnixFileInfo(path, buf);
			}
			return true;
		}
	}
}
