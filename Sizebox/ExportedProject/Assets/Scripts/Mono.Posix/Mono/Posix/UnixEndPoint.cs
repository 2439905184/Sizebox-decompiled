using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Posix
{
	[Serializable]
	[Obsolete("Use Mono.Unix.UnixEndPoint")]
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
			byte[] array = new byte[socketAddress.Size - 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = socketAddress[i + 2];
			}
			return new UnixEndPoint(Encoding.Default.GetString(array));
		}

		public override SocketAddress Serialize()
		{
			byte[] bytes = Encoding.Default.GetBytes(filename);
			SocketAddress socketAddress = new SocketAddress(AddressFamily, bytes.Length + 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				socketAddress[i + 2] = bytes[i];
			}
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
