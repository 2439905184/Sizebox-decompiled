using System;

namespace Mono.Unix.Native
{
	[Map]
	internal struct _SockaddrDynamic
	{
		public SockaddrType type;

		public UnixAddressFamily sa_family;

		public unsafe byte* data;

		public long len;

		public unsafe _SockaddrDynamic(Sockaddr address, byte* data, bool useMaxLength)
		{
			if (data == null)
			{
				this = default(_SockaddrDynamic);
				return;
			}
			byte[] array = address.DynamicData();
			type = address.type & (SockaddrType)(-32769);
			sa_family = address.sa_family;
			this.data = data;
			if (useMaxLength)
			{
				len = array.Length;
				return;
			}
			len = address.GetDynamicLength();
			if (len >= 0 && len <= array.Length)
			{
				return;
			}
			throw new ArgumentException("len < 0 || len > dynData.Length", "address");
		}

		public unsafe void Update(Sockaddr address)
		{
			if (data != null)
			{
				address.sa_family = sa_family;
				address.SetDynamicLength(len);
			}
		}
	}
}
