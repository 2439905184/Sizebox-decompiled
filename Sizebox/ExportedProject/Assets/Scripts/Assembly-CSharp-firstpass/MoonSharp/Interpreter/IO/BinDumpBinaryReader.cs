using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.IO
{
	public class BinDumpBinaryReader : BinaryReader
	{
		private List<string> m_Strings = new List<string>();

		public BinDumpBinaryReader(Stream s)
			: base(s)
		{
		}

		public BinDumpBinaryReader(Stream s, Encoding e)
			: base(s, e)
		{
		}

		public override int ReadInt32()
		{
			sbyte b = base.ReadSByte();
			switch (b)
			{
			case sbyte.MaxValue:
				return base.ReadInt16();
			case 126:
				return base.ReadInt32();
			default:
				return b;
			}
		}

		public override uint ReadUInt32()
		{
			byte b = base.ReadByte();
			switch (b)
			{
			case 127:
				return base.ReadUInt16();
			case 126:
				return base.ReadUInt32();
			default:
				return b;
			}
		}

		public override string ReadString()
		{
			int num = ReadInt32();
			if (num < m_Strings.Count)
			{
				return m_Strings[num];
			}
			if (num == m_Strings.Count)
			{
				string text = base.ReadString();
				m_Strings.Add(text);
				return text;
			}
			throw new IOException("string map failure");
		}
	}
}
