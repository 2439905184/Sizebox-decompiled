using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.IO
{
	public class BinDumpBinaryWriter : BinaryWriter
	{
		private Dictionary<string, int> m_StringMap = new Dictionary<string, int>();

		public BinDumpBinaryWriter(Stream s)
			: base(s)
		{
		}

		public BinDumpBinaryWriter(Stream s, Encoding e)
			: base(s, e)
		{
		}

		public override void Write(uint value)
		{
			byte b = (byte)value;
			if (b == value && b != 127 && b != 126)
			{
				base.Write(b);
				return;
			}
			ushort num = (ushort)value;
			if (num == value)
			{
				base.Write((byte)127);
				base.Write(num);
			}
			else
			{
				base.Write((byte)126);
				base.Write(value);
			}
		}

		public override void Write(int value)
		{
			sbyte b = (sbyte)value;
			if (b == value && b != sbyte.MaxValue && b != 126)
			{
				base.Write(b);
				return;
			}
			short num = (short)value;
			if (num == value)
			{
				base.Write(sbyte.MaxValue);
				base.Write(num);
			}
			else
			{
				base.Write((sbyte)126);
				base.Write(value);
			}
		}

		public override void Write(string value)
		{
			int value2;
			if (m_StringMap.TryGetValue(value, out value2))
			{
				Write(m_StringMap[value]);
				return;
			}
			value2 = m_StringMap.Count;
			m_StringMap[value] = value2;
			Write(value2);
			base.Write(value);
		}
	}
}
