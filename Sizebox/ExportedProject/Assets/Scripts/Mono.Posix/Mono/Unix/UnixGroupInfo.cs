using System;
using System.Collections;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixGroupInfo
	{
		private Group group;

		public string GroupName
		{
			get
			{
				return group.gr_name;
			}
		}

		public string Password
		{
			get
			{
				return group.gr_passwd;
			}
		}

		public long GroupId
		{
			get
			{
				return group.gr_gid;
			}
		}

		public UnixGroupInfo(string group)
		{
			this.group = new Group();
			Group grbufp;
			if (Syscall.getgrnam_r(group, this.group, out grbufp) != 0 || grbufp == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group name"), "group");
			}
		}

		public UnixGroupInfo(long group)
		{
			this.group = new Group();
			Group grbufp;
			if (Syscall.getgrgid_r(Convert.ToUInt32(group), this.group, out grbufp) != 0 || grbufp == null)
			{
				throw new ArgumentException(Locale.GetText("invalid group id"), "group");
			}
		}

		public UnixGroupInfo(Group group)
		{
			this.group = CopyGroup(group);
		}

		private static Group CopyGroup(Group group)
		{
			return new Group
			{
				gr_gid = group.gr_gid,
				gr_mem = group.gr_mem,
				gr_name = group.gr_name,
				gr_passwd = group.gr_passwd
			};
		}

		public UnixUserInfo[] GetMembers()
		{
			ArrayList arrayList = new ArrayList(group.gr_mem.Length);
			for (int i = 0; i < group.gr_mem.Length; i++)
			{
				try
				{
					arrayList.Add(new UnixUserInfo(group.gr_mem[i]));
				}
				catch (ArgumentException)
				{
				}
			}
			return (UnixUserInfo[])arrayList.ToArray(typeof(UnixUserInfo));
		}

		public string[] GetMemberNames()
		{
			return (string[])group.gr_mem.Clone();
		}

		public override int GetHashCode()
		{
			return group.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return group.Equals(((UnixGroupInfo)obj).group);
		}

		public override string ToString()
		{
			return group.ToString();
		}

		public Group ToGroup()
		{
			return CopyGroup(group);
		}

		public static UnixGroupInfo[] GetLocalGroups()
		{
			ArrayList arrayList = new ArrayList();
			lock (Syscall.grp_lock)
			{
				if (Syscall.setgrent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					Group group;
					while ((group = Syscall.getgrent()) != null)
					{
						arrayList.Add(new UnixGroupInfo(group));
					}
					if (Stdlib.GetLastError() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endgrent();
				}
			}
			return (UnixGroupInfo[])arrayList.ToArray(typeof(UnixGroupInfo));
		}
	}
}
