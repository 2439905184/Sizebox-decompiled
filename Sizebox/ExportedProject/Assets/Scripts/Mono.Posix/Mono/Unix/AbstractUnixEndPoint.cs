using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Unix
{
	[Serializable]
	public class AbstractUnixEndPoint : EndPoint
	{
		private string path;

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		public override AddressFamily AddressFamily
		{
			get
			{
				return AddressFamily.Unix;
			}
		}

		public AbstractUnixEndPoint(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path == "")
			{
				throw new ArgumentException("Cannot be empty.", "path");
			}
			this.path = path;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			byte[] array = new byte[socketAddress.Size - 2 - 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = socketAddress[3 + i];
			}
			return new AbstractUnixEndPoint(Encoding.Default.GetString(array));
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.Default.GetBytes(path);
			SocketAddress socketAddress = new SocketAddress(AddressFamily, 3 + bytes.Length);
			socketAddress[2] = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				socketAddress[i + 2 + 1] = bytes[i];
			}
			return socketAddress;
		}

		public override string ToString()
		{
			return path;
		}

		public override int GetHashCode()
		{
			return path.GetHashCode();
		}

		public override bool Equals(object o)
		{
			AbstractUnixEndPoint abstractUnixEndPoint = o as AbstractUnixEndPoint;
			if (abstractUnixEndPoint == null)
			{
				return false;
			}
			return abstractUnixEndPoint.path == path;
		}
	}
}
