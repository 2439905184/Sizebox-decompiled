using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class SockaddrStorage : Sockaddr, IEquatable<SockaddrStorage>
	{
		private static readonly int default_size = get_size();

		public byte[] data { get; set; }

		public long data_len { get; set; }

		internal override byte[] DynamicData()
		{
			return data;
		}

		internal override long GetDynamicLength()
		{
			return data_len;
		}

		internal override void SetDynamicLength(long value)
		{
			data_len = value;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_SockaddrStorage_get_size", SetLastError = true)]
		private static extern int get_size();

		public SockaddrStorage()
			: base((SockaddrType)32769, UnixAddressFamily.AF_UNSPEC)
		{
			data = new byte[default_size];
			data_len = 0L;
		}

		public SockaddrStorage(int size)
			: base((SockaddrType)32769, UnixAddressFamily.AF_UNSPEC)
		{
			data = new byte[size];
			data_len = 0L;
		}

		public unsafe void SetTo(Sockaddr address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			long nativeSize = address.GetNativeSize();
			if (nativeSize > data.Length)
			{
				data = new byte[nativeSize];
			}
			fixed (byte* ptr = data)
			{
				if (!NativeConvert.TryCopy(address, (IntPtr)ptr))
				{
					throw new ArgumentException("Failed to convert to native struct", "address");
				}
			}
			data_len = nativeSize;
			base.sa_family = address.sa_family;
		}

		public unsafe void CopyTo(Sockaddr address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data_len < 0 || data_len > data.Length)
			{
				throw new ArgumentException("data_len < 0 || data_len > data.Length", "this");
			}
			fixed (byte* ptr = data)
			{
				if (!NativeConvert.TryCopy((IntPtr)ptr, data_len, address))
				{
					throw new ArgumentException("Failed to convert from native struct", "this");
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{{sa_family={0}, data_len={1}, data=(", base.sa_family, data_len);
			for (int i = 0; i < data_len; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(data[i].ToString("x2"));
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			int num = 4660;
			for (int i = 0; i < data_len; i++)
			{
				num += i ^ data[i];
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SockaddrStorage))
			{
				return false;
			}
			return Equals((SockaddrStorage)obj);
		}

		public bool Equals(SockaddrStorage value)
		{
			if (value == null)
			{
				return false;
			}
			if (data_len != value.data_len)
			{
				return false;
			}
			for (int i = 0; i < data_len; i++)
			{
				if (data[i] != value.data[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
