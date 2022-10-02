using System.Net.Sockets;
using Mono.Unix;

namespace Mono.Remoting.Channels.Unix
{
	internal class ReusableUnixClient : UnixClient
	{
		public bool IsAlive
		{
			get
			{
				return !base.Client.Poll(0, SelectMode.SelectRead);
			}
		}

		public ReusableUnixClient(string path)
			: base(path)
		{
		}
	}
}
