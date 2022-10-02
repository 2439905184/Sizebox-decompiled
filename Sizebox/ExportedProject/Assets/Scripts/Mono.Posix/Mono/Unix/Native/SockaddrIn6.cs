using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[StructLayout(LayoutKind.Sequential)]
	[Map("struct sockaddr_in6")]
	[CLSCompliant(false)]
	public sealed class SockaddrIn6 : Sockaddr, IEquatable<SockaddrIn6>
	{
		public ushort sin6_port;

		public uint sin6_flowinfo;

		public In6Addr sin6_addr;

		public uint sin6_scope_id;

		public UnixAddressFamily sin6_family
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

		public SockaddrIn6()
			: base(SockaddrType.SockaddrIn6, UnixAddressFamily.AF_INET6)
		{
		}

		public override string ToString()
		{
			return string.Format("{{sin6_family={0}, sin6_port=htons({1}), sin6_flowinfo={2}, sin6_addr={3}, sin6_scope_id={4}}}", base.sa_family, Syscall.ntohs(sin6_port), sin6_flowinfo, sin6_addr, sin6_scope_id);
		}

		public new static SockaddrIn6 FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrIn6 sockaddrIn = new SockaddrIn6();
			storage.CopyTo(sockaddrIn);
			return sockaddrIn;
		}

		public override int GetHashCode()
		{
			return sin6_family.GetHashCode() ^ sin6_port.GetHashCode() ^ sin6_flowinfo.GetHashCode() ^ sin6_addr.GetHashCode() ^ sin6_scope_id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SockaddrIn6))
			{
				return false;
			}
			return Equals((SockaddrIn6)obj);
		}

		public bool Equals(SockaddrIn6 value)
		{
			if (value == null)
			{
				return false;
			}
			if (sin6_family == value.sin6_family && sin6_port == value.sin6_port && sin6_flowinfo == value.sin6_flowinfo && sin6_addr.Equals(value.sin6_addr))
			{
				return sin6_scope_id == value.sin6_scope_id;
			}
			return false;
		}
	}
}
