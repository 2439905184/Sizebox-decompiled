using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public struct InAddr : IEquatable<InAddr>
	{
		public uint s_addr;

		public unsafe byte this[int index]
		{
			get
			{
				if (index < 0 || index >= 4)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 4");
				}
				fixed (uint* ptr = &s_addr)
				{
					return ((byte*)ptr)[index];
				}
			}
			set
			{
				if (index < 0 || index >= 4)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 4");
				}
				fixed (uint* ptr = &s_addr)
				{
					((byte*)ptr)[index] = value;
				}
			}
		}

		public unsafe InAddr(byte b0, byte b1, byte b2, byte b3)
		{
			s_addr = 0u;
			fixed (uint* ptr = &s_addr)
			{
				byte* ptr2 = (byte*)ptr;
				*ptr2 = b0;
				ptr2[1] = b1;
				ptr2[2] = b2;
				ptr2[3] = b3;
			}
		}

		public unsafe InAddr(byte[] buffer)
		{
			if (buffer.Length != 4)
			{
				throw new ArgumentException("buffer.Length != 4", "buffer");
			}
			s_addr = 0u;
			fixed (uint* ptr = &s_addr)
			{
				Marshal.Copy(buffer, 0, (IntPtr)ptr, 4);
			}
		}

		public unsafe void CopyFrom(byte[] source, int startIndex)
		{
			fixed (uint* ptr = &s_addr)
			{
				Marshal.Copy(source, startIndex, (IntPtr)ptr, 4);
			}
		}

		public unsafe void CopyTo(byte[] destination, int startIndex)
		{
			fixed (uint* ptr = &s_addr)
			{
				Marshal.Copy((IntPtr)ptr, destination, startIndex, 4);
			}
		}

		public override string ToString()
		{
			return NativeConvert.ToIPAddress(this).ToString();
		}

		public override int GetHashCode()
		{
			return s_addr.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is InAddr))
			{
				return false;
			}
			return Equals((InAddr)obj);
		}

		public bool Equals(InAddr value)
		{
			return s_addr == value.s_addr;
		}
	}
}
