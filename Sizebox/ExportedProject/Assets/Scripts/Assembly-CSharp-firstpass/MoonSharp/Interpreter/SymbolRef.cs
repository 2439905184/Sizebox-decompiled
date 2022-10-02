using System.Collections.Generic;
using System.IO;

namespace MoonSharp.Interpreter
{
	public class SymbolRef
	{
		private static SymbolRef s_DefaultEnv = new SymbolRef
		{
			i_Type = SymbolRefType.DefaultEnv
		};

		internal SymbolRefType i_Type;

		internal SymbolRef i_Env;

		internal int i_Index;

		internal string i_Name;

		public SymbolRefType Type
		{
			get
			{
				return i_Type;
			}
		}

		public int Index
		{
			get
			{
				return i_Index;
			}
		}

		public string Name
		{
			get
			{
				return i_Name;
			}
		}

		public SymbolRef Environment
		{
			get
			{
				return i_Env;
			}
		}

		public static SymbolRef DefaultEnv
		{
			get
			{
				return s_DefaultEnv;
			}
		}

		public static SymbolRef Global(string name, SymbolRef envSymbol)
		{
			return new SymbolRef
			{
				i_Index = -1,
				i_Type = SymbolRefType.Global,
				i_Env = envSymbol,
				i_Name = name
			};
		}

		internal static SymbolRef Local(string name, int index)
		{
			return new SymbolRef
			{
				i_Index = index,
				i_Type = SymbolRefType.Local,
				i_Name = name
			};
		}

		internal static SymbolRef Upvalue(string name, int index)
		{
			return new SymbolRef
			{
				i_Index = index,
				i_Type = SymbolRefType.Upvalue,
				i_Name = name
			};
		}

		public override string ToString()
		{
			if (i_Type == SymbolRefType.DefaultEnv)
			{
				return "(default _ENV)";
			}
			if (i_Type == SymbolRefType.Global)
			{
				return string.Format("{2} : {0} / {1}", i_Type, i_Env, i_Name);
			}
			return string.Format("{2} : {0}[{1}]", i_Type, i_Index, i_Name);
		}

		internal void WriteBinary(BinaryWriter bw)
		{
			bw.Write((byte)i_Type);
			bw.Write(i_Index);
			bw.Write(i_Name);
		}

		internal static SymbolRef ReadBinary(BinaryReader br)
		{
			return new SymbolRef
			{
				i_Type = (SymbolRefType)br.ReadByte(),
				i_Index = br.ReadInt32(),
				i_Name = br.ReadString()
			};
		}

		internal void WriteBinaryEnv(BinaryWriter bw, Dictionary<SymbolRef, int> symbolMap)
		{
			if (i_Env != null)
			{
				bw.Write(symbolMap[i_Env]);
			}
			else
			{
				bw.Write(-1);
			}
		}

		internal void ReadBinaryEnv(BinaryReader br, SymbolRef[] symbolRefs)
		{
			int num = br.ReadInt32();
			if (num >= 0)
			{
				i_Env = symbolRefs[num];
			}
		}
	}
}
