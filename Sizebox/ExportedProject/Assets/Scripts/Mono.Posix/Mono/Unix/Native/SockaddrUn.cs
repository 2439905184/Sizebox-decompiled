using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class SockaddrUn : Sockaddr, IEquatable<SockaddrUn>
	{
		private static readonly int sizeof_sun_path = get_sizeof_sun_path();

		public UnixAddressFamily sun_family
		{
			get
			{
				return base.sa_family;
			}
			set
			{
				base.sa_family = value;
			}
		}

		public byte[] sun_path { get; set; }

		public long sun_path_len { get; set; }

		public bool IsLinuxAbstractNamespace
		{
			get
			{
				if (sun_path_len > 0)
				{
					return sun_path[0] == 0;
				}
				return false;
			}
		}

		public string Path
		{
			get
			{
				int num = (IsLinuxAbstractNamespace ? 1 : 0);
				int i;
				for (i = 0; num + i < sun_path_len && sun_path[num + i] != 0; i++)
				{
				}
				return UnixEncoding.Instance.GetString(sun_path, num, i);
			}
		}

		internal override byte[] DynamicData()
		{
			return sun_path;
		}

		internal override long GetDynamicLength()
		{
			return sun_path_len;
		}

		internal override void SetDynamicLength(long value)
		{
			sun_path_len = value;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_SockaddrUn_get_sizeof_sun_path", SetLastError = true)]
		private static extern int get_sizeof_sun_path();

		public SockaddrUn()
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			sun_path = new byte[sizeof_sun_path];
			sun_path_len = 0L;
		}

		public SockaddrUn(int size)
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			sun_path = new byte[size];
			sun_path_len = 0L;
		}

		public SockaddrUn(string path, bool linuxAbstractNamespace = false)
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			byte[] bytes = UnixEncoding.Instance.GetBytes(path);
			if (linuxAbstractNamespace)
			{
				sun_path = new byte[1 + bytes.Length];
				Array.Copy(bytes, 0, sun_path, 1, bytes.Length);
			}
			else
			{
				sun_path = bytes;
			}
			sun_path_len = sun_path.Length;
		}

		public override string ToString()
		{
			return string.Format("{{sa_family={0}, sun_path=\"{1}{2}\"}}", base.sa_family, IsLinuxAbstractNamespace ? "\\0" : "", Path);
		}

		public new static SockaddrUn FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrUn sockaddrUn = new SockaddrUn((int)storage.data_len);
			storage.CopyTo(sockaddrUn);
			return sockaddrUn;
		}

		public override int GetHashCode()
		{
			return sun_family.GetHashCode() ^ IsLinuxAbstractNamespace.GetHashCode() ^ Path.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SockaddrUn))
			{
				return false;
			}
			return Equals((SockaddrUn)obj);
		}

		public bool Equals(SockaddrUn value)
		{
			if (value == null)
			{
				return false;
			}
			if (sun_family == value.sun_family && IsLinuxAbstractNamespace == value.IsLinuxAbstractNamespace)
			{
				return Path == value.Path;
			}
			return false;
		}
	}
}
