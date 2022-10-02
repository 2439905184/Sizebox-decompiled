using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[StructLayout(LayoutKind.Sequential)]
	[Map("struct sockaddr_in")]
	[CLSCompliant(false)]
	public sealed class SockaddrIn : Sockaddr, IEquatable<SockaddrIn>
	{
		public ushort sin_port;

		public InAddr sin_addr;

		public UnixAddressFamily sin_family
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

		public SockaddrIn()
			: base(SockaddrType.SockaddrIn, UnixAddressFamily.AF_INET)
		{
		}

		public override string ToString()
		{
			return string.Format("{{sin_family={0}, sin_port=htons({1}), sin_addr={2}}}", base.sa_family, Syscall.ntohs(sin_port), sin_addr);
		}

		public new static SockaddrIn FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrIn sockaddrIn = new SockaddrIn();
			storage.CopyTo(sockaddrIn);
			return sockaddrIn;
		}

		public override int GetHashCode()
		{
			return sin_family.GetHashCode() ^ sin_port.GetHashCode() ^ sin_addr.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SockaddrIn))
			{
				return false;
			}
			return Equals((SockaddrIn)obj);
		}

		public bool Equals(SockaddrIn value)
		{
			if (value == null)
			{
				return false;
			}
			if (sin_family == value.sin_family && sin_port == value.sin_port)
			{
				return sin_addr.Equals(value.sin_addr);
			}
			return false;
		}
	}
}
