using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixSymbolicLinkInfo : UnixFileSystemInfo
	{
		public override string Name
		{
			get
			{
				return UnixPath.GetFileName(base.FullPath);
			}
		}

		[Obsolete("Use GetContents()")]
		public UnixFileSystemInfo Contents
		{
			get
			{
				return GetContents();
			}
		}

		public string ContentsPath
		{
			get
			{
				return UnixPath.ReadLink(base.FullPath);
			}
		}

		public bool HasContents
		{
			get
			{
				return UnixPath.TryReadLink(base.FullPath) != null;
			}
		}

		public UnixSymbolicLinkInfo(string path)
			: base(path)
		{
		}

		internal UnixSymbolicLinkInfo(string path, Stat stat)
			: base(path, stat)
		{
		}

		public UnixFileSystemInfo GetContents()
		{
			return UnixFileSystemInfo.GetFileSystemEntry(UnixPath.Combine(UnixPath.GetDirectoryName(base.FullPath), ContentsPath));
		}

		public void CreateSymbolicLinkTo(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.symlink(path, FullName));
		}

		public void CreateSymbolicLinkTo(UnixFileSystemInfo path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.symlink(path.FullName, FullName));
		}

		public override void Delete()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.unlink(base.FullPath));
			Refresh();
		}

		public override void SetOwner(long owner, long group)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.lchown(base.FullPath, Convert.ToUInt32(owner), Convert.ToUInt32(group)));
		}

		protected override bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.lstat(path, out stat) == 0;
		}
	}
}
