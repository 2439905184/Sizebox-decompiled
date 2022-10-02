using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map("struct cmsghdr")]
	[CLSCompliant(false)]
	public struct Cmsghdr
	{
		public long cmsg_len;

		public UnixSocketProtocol cmsg_level;

		public UnixSocketControlMessage cmsg_type;

		private static readonly int size = getsize();

		public static int Size
		{
			get
			{
				return size;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Cmsghdr_getsize", SetLastError = true)]
		private static extern int getsize();

		public unsafe static Cmsghdr ReadFromBuffer(Msghdr msgh, long cmsg)
		{
			if (msgh == null)
			{
				throw new ArgumentNullException("msgh");
			}
			if (msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0 || cmsg + Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			Cmsghdr destination;
			fixed (byte* ptr = msgh.msg_control)
			{
				if (!NativeConvert.TryCopy((IntPtr)(ptr + cmsg), out destination))
				{
					throw new ArgumentException("Failed to convert from native struct", "buffer");
				}
			}
			if (NativeConvert.FromUnixSocketProtocol(destination.cmsg_level) == NativeConvert.FromUnixSocketProtocol(UnixSocketProtocol.SOL_SOCKET))
			{
				destination.cmsg_level = UnixSocketProtocol.SOL_SOCKET;
			}
			return destination;
		}

		public unsafe void WriteToBuffer(Msghdr msgh, long cmsg)
		{
			if (msgh == null)
			{
				throw new ArgumentNullException("msgh");
			}
			if (msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0 || cmsg + Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			fixed (byte* ptr = msgh.msg_control)
			{
				if (!NativeConvert.TryCopy(ref this, (IntPtr)(ptr + cmsg)))
				{
					throw new ArgumentException("Failed to convert to native struct", "buffer");
				}
			}
		}
	}
}
