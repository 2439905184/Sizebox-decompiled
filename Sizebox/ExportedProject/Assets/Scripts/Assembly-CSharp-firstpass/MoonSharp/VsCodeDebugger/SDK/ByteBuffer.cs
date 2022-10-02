using System;
using System.Text;

namespace MoonSharp.VsCodeDebugger.SDK
{
	internal class ByteBuffer
	{
		private byte[] _buffer;

		public int Length
		{
			get
			{
				return _buffer.Length;
			}
		}

		public ByteBuffer()
		{
			_buffer = new byte[0];
		}

		public string GetString(Encoding enc)
		{
			return enc.GetString(_buffer);
		}

		public void Append(byte[] b, int length)
		{
			byte[] array = new byte[_buffer.Length + length];
			Buffer.BlockCopy(_buffer, 0, array, 0, _buffer.Length);
			Buffer.BlockCopy(b, 0, array, _buffer.Length, length);
			_buffer = array;
		}

		public byte[] RemoveFirst(int n)
		{
			byte[] array = new byte[n];
			Buffer.BlockCopy(_buffer, 0, array, 0, n);
			byte[] array2 = new byte[_buffer.Length - n];
			Buffer.BlockCopy(_buffer, n, array2, 0, _buffer.Length - n);
			_buffer = array2;
			return array;
		}
	}
}
