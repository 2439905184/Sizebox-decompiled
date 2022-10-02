using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[StructLayout(LayoutKind.Sequential)]
	[CLSCompliant(false)]
	public class Sockaddr
	{
		internal SockaddrType type;

		internal UnixAddressFamily _sa_family;

		private static Sockaddr nullSockaddr = new Sockaddr();

		public UnixAddressFamily sa_family
		{
			get
			{
				return _sa_family;
			}
			set
			{
				_sa_family = value;
			}
		}

		public Sockaddr()
		{
			type = SockaddrType.Sockaddr;
			sa_family = UnixAddressFamily.AF_UNSPEC;
		}

		internal Sockaddr(SockaddrType type, UnixAddressFamily sa_family)
		{
			this.type = type;
			this.sa_family = sa_family;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Sockaddr_GetNativeSize", SetLastError = true)]
		private unsafe static extern int GetNativeSize(_SockaddrHeader* address, out long size);

		internal unsafe long GetNativeSize()
		{
			long size;
			fixed (SockaddrType* addr = &GetAddress(this).type)
			{
				fixed (byte* data = GetDynamicData(this))
				{
					_SockaddrDynamic sockaddrDynamic = new _SockaddrDynamic(this, data, false);
					if (GetNativeSize(GetNative(&sockaddrDynamic, addr), out size) != 0)
					{
						throw new ArgumentException("Failed to get size of native struct", "this");
					}
				}
			}
			return size;
		}

		internal static Sockaddr GetAddress(Sockaddr address)
		{
			if (address == null)
			{
				return nullSockaddr;
			}
			return address;
		}

		internal unsafe static _SockaddrHeader* GetNative(_SockaddrDynamic* dyn, SockaddrType* addr)
		{
			if (dyn->data != null)
			{
				return (_SockaddrHeader*)dyn;
			}
			fixed (SockaddrType* ptr = &nullSockaddr.type)
			{
				if (addr == ptr)
				{
					return null;
				}
			}
			return (_SockaddrHeader*)addr;
		}

		internal static byte[] GetDynamicData(Sockaddr addr)
		{
			if (addr == null)
			{
				return null;
			}
			return addr.DynamicData();
		}

		internal virtual byte[] DynamicData()
		{
			return null;
		}

		internal virtual long GetDynamicLength()
		{
			throw new NotImplementedException();
		}

		internal virtual void SetDynamicLength(long value)
		{
			throw new NotImplementedException();
		}

		public SockaddrStorage ToSockaddrStorage()
		{
			SockaddrStorage sockaddrStorage = new SockaddrStorage((int)GetNativeSize());
			sockaddrStorage.SetTo(this);
			return sockaddrStorage;
		}

		public static Sockaddr FromSockaddrStorage(SockaddrStorage storage)
		{
			Sockaddr sockaddr = new Sockaddr();
			storage.CopyTo(sockaddr);
			return sockaddr;
		}
	}
}
