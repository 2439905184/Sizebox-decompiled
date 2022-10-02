using System;
using System.Net.Sockets;

namespace Mono.Posix
{
	[Obsolete("Use Mono.Unix.PeerCred")]
	public class PeerCred
	{
		private const int so_peercred = 10001;

		private PeerCredData data;

		public int ProcessID
		{
			get
			{
				return data.pid;
			}
		}

		public int UserID
		{
			get
			{
				return data.uid;
			}
		}

		public int GroupID
		{
			get
			{
				return data.gid;
			}
		}

		public PeerCred(Socket sock)
		{
			if (sock.AddressFamily != AddressFamily.Unix)
			{
				throw new ArgumentException("Only Unix sockets are supported", "sock");
			}
			data = (PeerCredData)sock.GetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)10001);
		}
	}
}
