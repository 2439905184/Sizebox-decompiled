using System;
using System.Collections;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixEnvironment
	{
		public static string CurrentDirectory
		{
			get
			{
				return UnixDirectoryInfo.GetCurrentDirectory();
			}
			set
			{
				UnixDirectoryInfo.SetCurrentDirectory(value);
			}
		}

		public static string MachineName
		{
			get
			{
				Utsname buf;
				if (Syscall.uname(out buf) != 0)
				{
					throw UnixMarshal.CreateExceptionForLastError();
				}
				return buf.nodename;
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.sethostname(value));
			}
		}

		public static string UserName
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		public static UnixGroupInfo RealGroup
		{
			get
			{
				return new UnixGroupInfo(RealGroupId);
			}
		}

		public static long RealGroupId
		{
			get
			{
				return Syscall.getgid();
			}
		}

		public static UnixUserInfo RealUser
		{
			get
			{
				return new UnixUserInfo(RealUserId);
			}
		}

		public static long RealUserId
		{
			get
			{
				return Syscall.getuid();
			}
		}

		public static UnixGroupInfo EffectiveGroup
		{
			get
			{
				return new UnixGroupInfo(EffectiveGroupId);
			}
			set
			{
				EffectiveGroupId = value.GroupId;
			}
		}

		public static long EffectiveGroupId
		{
			get
			{
				return Syscall.getegid();
			}
			set
			{
				Syscall.setegid(Convert.ToUInt32(value));
			}
		}

		public static UnixUserInfo EffectiveUser
		{
			get
			{
				return new UnixUserInfo(EffectiveUserId);
			}
			set
			{
				EffectiveUserId = value.UserId;
			}
		}

		public static long EffectiveUserId
		{
			get
			{
				return Syscall.geteuid();
			}
			set
			{
				Syscall.seteuid(Convert.ToUInt32(value));
			}
		}

		public static string Login
		{
			get
			{
				return UnixUserInfo.GetRealUser().UserName;
			}
		}

		private UnixEnvironment()
		{
		}

		[CLSCompliant(false)]
		public static long GetConfigurationValue(SysconfName name)
		{
			long num = Syscall.sysconf(name);
			if (num == -1 && Stdlib.GetLastError() != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		[CLSCompliant(false)]
		public static string GetConfigurationString(ConfstrName name)
		{
			ulong num = Syscall.confstr(name, null, 0uL);
			if (num == ulong.MaxValue)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			if (num == 0L)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder((int)num + 1);
			num = Syscall.confstr(name, stringBuilder, num);
			if (num == ulong.MaxValue)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return stringBuilder.ToString();
		}

		public static void SetNiceValue(int inc)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.nice(inc));
		}

		public static int CreateSession()
		{
			int num = Syscall.setsid();
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return num;
		}

		public static void SetProcessGroup()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setpgrp());
		}

		public static int GetProcessGroup()
		{
			return Syscall.getpgrp();
		}

		public static UnixGroupInfo[] GetSupplementaryGroups()
		{
			uint[] array = _GetSupplementaryGroupIds();
			UnixGroupInfo[] array2 = new UnixGroupInfo[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new UnixGroupInfo(array[i]);
			}
			return array2;
		}

		private static uint[] _GetSupplementaryGroupIds()
		{
			int num = Syscall.getgroups(0, new uint[0]);
			if (num == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			uint[] array = new uint[num];
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.getgroups(array));
			return array;
		}

		public static void SetSupplementaryGroups(UnixGroupInfo[] groups)
		{
			uint[] array = new uint[groups.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToUInt32(groups[i].GroupId);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setgroups(array));
		}

		public static long[] GetSupplementaryGroupIds()
		{
			uint[] array = _GetSupplementaryGroupIds();
			long[] array2 = new long[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i];
			}
			return array2;
		}

		public static void SetSupplementaryGroupIds(long[] list)
		{
			uint[] array = new uint[list.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToUInt32(list[i]);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setgroups(array));
		}

		public static int GetParentProcessId()
		{
			return Syscall.getppid();
		}

		public static UnixProcess GetParentProcess()
		{
			return new UnixProcess(GetParentProcessId());
		}

		public static string[] GetUserShells()
		{
			ArrayList arrayList = new ArrayList();
			lock (Syscall.usershell_lock)
			{
				try
				{
					if (Syscall.setusershell() != 0)
					{
						UnixMarshal.ThrowExceptionForLastError();
					}
					string value;
					while ((value = Syscall.getusershell()) != null)
					{
						arrayList.Add(value);
					}
				}
				finally
				{
					Syscall.endusershell();
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
	}
}
