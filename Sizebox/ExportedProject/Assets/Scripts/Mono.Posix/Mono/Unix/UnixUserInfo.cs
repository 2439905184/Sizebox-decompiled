using System;
using System.Collections;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixUserInfo
	{
		private Passwd passwd;

		public string UserName
		{
			get
			{
				return passwd.pw_name;
			}
		}

		public string Password
		{
			get
			{
				return passwd.pw_passwd;
			}
		}

		public long UserId
		{
			get
			{
				return passwd.pw_uid;
			}
		}

		public UnixGroupInfo Group
		{
			get
			{
				return new UnixGroupInfo(passwd.pw_gid);
			}
		}

		public long GroupId
		{
			get
			{
				return passwd.pw_gid;
			}
		}

		public string GroupName
		{
			get
			{
				return Group.GroupName;
			}
		}

		public string RealName
		{
			get
			{
				return passwd.pw_gecos;
			}
		}

		public string HomeDirectory
		{
			get
			{
				return passwd.pw_dir;
			}
		}

		public string ShellProgram
		{
			get
			{
				return passwd.pw_shell;
			}
		}

		public UnixUserInfo(string user)
		{
			passwd = new Passwd();
			Passwd pwbufp;
			if (Syscall.getpwnam_r(user, passwd, out pwbufp) != 0 || pwbufp == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "user");
			}
		}

		[CLSCompliant(false)]
		public UnixUserInfo(uint user)
		{
			passwd = new Passwd();
			Passwd pwbufp;
			if (Syscall.getpwuid_r(user, passwd, out pwbufp) != 0 || pwbufp == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(long user)
		{
			passwd = new Passwd();
			Passwd pwbufp;
			if (Syscall.getpwuid_r(Convert.ToUInt32(user), passwd, out pwbufp) != 0 || pwbufp == null)
			{
				throw new ArgumentException(Locale.GetText("invalid user id"), "user");
			}
		}

		public UnixUserInfo(Passwd passwd)
		{
			this.passwd = CopyPasswd(passwd);
		}

		private static Passwd CopyPasswd(Passwd pw)
		{
			return new Passwd
			{
				pw_name = pw.pw_name,
				pw_passwd = pw.pw_passwd,
				pw_uid = pw.pw_uid,
				pw_gid = pw.pw_gid,
				pw_gecos = pw.pw_gecos,
				pw_dir = pw.pw_dir,
				pw_shell = pw.pw_shell
			};
		}

		public override int GetHashCode()
		{
			return passwd.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return passwd.Equals(((UnixUserInfo)obj).passwd);
		}

		public override string ToString()
		{
			return passwd.ToString();
		}

		public static UnixUserInfo GetRealUser()
		{
			return new UnixUserInfo(GetRealUserId());
		}

		public static long GetRealUserId()
		{
			return Syscall.getuid();
		}

		public static string GetLoginName()
		{
			StringBuilder stringBuilder = new StringBuilder(4);
			int num;
			do
			{
				stringBuilder.Capacity *= 2;
				num = Syscall.getlogin_r(stringBuilder, (ulong)stringBuilder.Capacity);
			}
			while (num == -1 && Stdlib.GetLastError() == Errno.ERANGE);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return stringBuilder.ToString();
		}

		public Passwd ToPasswd()
		{
			return CopyPasswd(passwd);
		}

		public static UnixUserInfo[] GetLocalUsers()
		{
			ArrayList arrayList = new ArrayList();
			lock (Syscall.pwd_lock)
			{
				if (Syscall.setpwent() != 0)
				{
					UnixMarshal.ThrowExceptionForLastError();
				}
				try
				{
					Passwd passwd;
					while ((passwd = Syscall.getpwent()) != null)
					{
						arrayList.Add(new UnixUserInfo(passwd));
					}
					if (Stdlib.GetLastError() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
				}
				finally
				{
					Syscall.endpwent();
				}
			}
			return (UnixUserInfo[])arrayList.ToArray(typeof(UnixUserInfo));
		}
	}
}
