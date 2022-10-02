using System;

namespace Mono.Unix.Native
{
	public struct Stat : IEquatable<Stat>
	{
		[CLSCompliant(false)]
		[dev_t]
		public ulong st_dev;

		[CLSCompliant(false)]
		[ino_t]
		public ulong st_ino;

		[CLSCompliant(false)]
		public FilePermissions st_mode;

		[NonSerialized]
		private uint _padding_;

		[CLSCompliant(false)]
		[nlink_t]
		public ulong st_nlink;

		[CLSCompliant(false)]
		[uid_t]
		public uint st_uid;

		[CLSCompliant(false)]
		[gid_t]
		public uint st_gid;

		[CLSCompliant(false)]
		[dev_t]
		public ulong st_rdev;

		[off_t]
		public long st_size;

		[blksize_t]
		public long st_blksize;

		[blkcnt_t]
		public long st_blocks;

		[time_t]
		public long st_atime;

		[time_t]
		public long st_mtime;

		[time_t]
		public long st_ctime;

		public long st_atime_nsec;

		public long st_mtime_nsec;

		public long st_ctime_nsec;

		public Timespec st_atim
		{
			get
			{
				Timespec result = default(Timespec);
				result.tv_sec = st_atime;
				result.tv_nsec = st_atime_nsec;
				return result;
			}
			set
			{
				st_atime = value.tv_sec;
				st_atime_nsec = value.tv_nsec;
			}
		}

		public Timespec st_mtim
		{
			get
			{
				Timespec result = default(Timespec);
				result.tv_sec = st_mtime;
				result.tv_nsec = st_mtime_nsec;
				return result;
			}
			set
			{
				st_mtime = value.tv_sec;
				st_mtime_nsec = value.tv_nsec;
			}
		}

		public Timespec st_ctim
		{
			get
			{
				Timespec result = default(Timespec);
				result.tv_sec = st_ctime;
				result.tv_nsec = st_ctime_nsec;
				return result;
			}
			set
			{
				st_ctime = value.tv_sec;
				st_ctime_nsec = value.tv_nsec;
			}
		}

		public override int GetHashCode()
		{
			return st_dev.GetHashCode() ^ st_ino.GetHashCode() ^ st_mode.GetHashCode() ^ st_nlink.GetHashCode() ^ st_uid.GetHashCode() ^ st_gid.GetHashCode() ^ st_rdev.GetHashCode() ^ st_size.GetHashCode() ^ st_blksize.GetHashCode() ^ st_blocks.GetHashCode() ^ st_atime.GetHashCode() ^ st_mtime.GetHashCode() ^ st_ctime.GetHashCode() ^ st_atime_nsec.GetHashCode() ^ st_mtime_nsec.GetHashCode() ^ st_ctime_nsec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Stat stat = (Stat)obj;
			if (stat.st_dev == st_dev && stat.st_ino == st_ino && stat.st_mode == st_mode && stat.st_nlink == st_nlink && stat.st_uid == st_uid && stat.st_gid == st_gid && stat.st_rdev == st_rdev && stat.st_size == st_size && stat.st_blksize == st_blksize && stat.st_blocks == st_blocks && stat.st_atime == st_atime && stat.st_mtime == st_mtime && stat.st_ctime == st_ctime && stat.st_atime_nsec == st_atime_nsec && stat.st_mtime_nsec == st_mtime_nsec)
			{
				return stat.st_ctime_nsec == st_ctime_nsec;
			}
			return false;
		}

		public bool Equals(Stat value)
		{
			if (value.st_dev == st_dev && value.st_ino == st_ino && value.st_mode == st_mode && value.st_nlink == st_nlink && value.st_uid == st_uid && value.st_gid == st_gid && value.st_rdev == st_rdev && value.st_size == st_size && value.st_blksize == st_blksize && value.st_blocks == st_blocks && value.st_atime == st_atime && value.st_mtime == st_mtime && value.st_ctime == st_ctime && value.st_atime_nsec == st_atime_nsec && value.st_mtime_nsec == st_mtime_nsec)
			{
				return value.st_ctime_nsec == st_ctime_nsec;
			}
			return false;
		}

		public static bool operator ==(Stat lhs, Stat rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Stat lhs, Stat rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
}
