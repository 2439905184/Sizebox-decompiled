using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class UnixEndPoint : EndPoint
	{
		private string filename;

		public string Filename
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
			}
		}

		public override AddressFamily AddressFamily
		{
			get
			{
				return AddressFamily.Unix;
			}
		}

		public UnixEndPoint(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (filename == "")
			{
				throw new ArgumentException("Cannot be empty.", "filename");
			}
			this.filename = filename;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			if (socketAddress.Size == 2)
			{
				return new UnixEndPoint("a")
				{
					filename = ""
				};
			}
			int num = socketAddress.Size - 2;
			byte[] array = new byte[num];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = socketAddress[i + 2];
				if (array[i] == 0)
				{
					num = i;
					break;
				}
			}
			return new UnixEndPoint(Encoding.Default.GetString(array, 0, num));
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.Default.GetBytes(filename);
			SocketAddress socketAddress = new SocketAddress(AddressFamily, 2 + bytes.Length + 1);
			for (int i = 0; i < bytes.Length; i++)
			{
				socketAddress[2 + i] = bytes[i];
			}
			socketAddress[2 + bytes.Length] = 0;
			return socketAddress;
		}

		public override string ToString()
		{
			return filename;
		}

		public override int GetHashCode()
		{
			return filename.GetHashCode();
		}

		public override bool Equals(object o)
		{
			UnixEndPoint unixEndPoint = o as UnixEndPoint;
			if (unixEndPoint == null)
			{
				return false;
			}
			return unixEndPoint.filename == filename;
		}
	}
}
