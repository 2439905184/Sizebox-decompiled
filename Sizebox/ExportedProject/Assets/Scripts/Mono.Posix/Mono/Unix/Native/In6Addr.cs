using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map]
	public struct In6Addr : IEquatable<In6Addr>
	{
		private ulong addr0;

		private ulong addr1;

		public unsafe byte this[int index]
		{
			get
			{
				if (index < 0 || index >= 16)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 16");
				}
				fixed (ulong* ptr = &addr0)
				{
					return ((byte*)ptr)[index];
				}
			}
			set
			{
				if (index < 0 || index >= 16)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 16");
				}
				fixed (ulong* ptr = &addr0)
				{
					((byte*)ptr)[index] = value;
				}
			}
		}

		public unsafe In6Addr(byte[] buffer)
		{
			if (buffer.Length != 16)
			{
				throw new ArgumentException("buffer.Length != 16", "buffer");
			}
			addr0 = (addr1 = 0uL);
			fixed (ulong* ptr = &addr0)
			{
				Marshal.Copy(buffer, 0, (IntPtr)ptr, 16);
			}
		}

		public unsafe void CopyFrom(byte[] source, int startIndex)
		{
			fixed (ulong* ptr = &addr0)
			{
				Marshal.Copy(source, startIndex, (IntPtr)ptr, 16);
			}
		}

		public unsafe void CopyTo(byte[] destination, int startIndex)
		{
			fixed (ulong* ptr = &addr0)
			{
				Marshal.Copy((IntPtr)ptr, destination, startIndex, 16);
			}
		}

		public override string ToString()
		{
			return NativeConvert.ToIPAddress(this).ToString();
		}

		public override int GetHashCode()
		{
			return addr0.GetHashCode() ^ addr1.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is In6Addr))
			{
				return false;
			}
			return Equals((In6Addr)obj);
		}

		public bool Equals(In6Addr value)
		{
			if (addr0 == value.addr0)
			{
				return addr1 == value.addr1;
			}
			return false;
		}
	}
}
