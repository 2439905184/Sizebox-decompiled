using System;

namespace MoonSharp.Interpreter.Interop.LuaStateInterop
{
	public class LuaBase
	{
		protected const int LUA_TNONE = -1;

		protected const int LUA_TNIL = 0;

		protected const int LUA_TBOOLEAN = 1;

		protected const int LUA_TLIGHTUSERDATA = 2;

		protected const int LUA_TNUMBER = 3;

		protected const int LUA_TSTRING = 4;

		protected const int LUA_TTABLE = 5;

		protected const int LUA_TFUNCTION = 6;

		protected const int LUA_TUSERDATA = 7;

		protected const int LUA_TTHREAD = 8;

		protected const int LUA_MULTRET = -1;

		protected const string LUA_INTFRMLEN = "l";

		protected static DynValue GetArgument(LuaState L, int pos)
		{
			return L.At(pos);
		}

		protected static DynValue ArgAsType(LuaState L, int pos, DataType type, bool allowNil = false)
		{
			return GetArgument(L, pos).CheckType(L.FunctionName, type, pos - 1, allowNil ? (TypeValidationFlags.AllowNil | TypeValidationFlags.AutoConvert) : TypeValidationFlags.AutoConvert);
		}

		protected static int LuaType(LuaState L, int p)
		{
			switch (GetArgument(L, p).Type)
			{
			case DataType.Void:
				return -1;
			case DataType.Nil:
				return 0;
			case DataType.Boolean:
				return 0;
			case DataType.Number:
				return 3;
			case DataType.String:
				return 4;
			case DataType.Function:
				return 6;
			case DataType.Table:
				return 5;
			case DataType.UserData:
				return 7;
			case DataType.Thread:
				return 8;
			case DataType.ClrFunction:
				return 6;
			default:
				throw new ScriptRuntimeException("Can't call LuaType on any type");
			}
		}

		protected static string LuaLCheckLString(LuaState L, int argNum, out uint l)
		{
			string @string = ArgAsType(L, argNum, DataType.String).String;
			l = (uint)@string.Length;
			return @string;
		}

		protected static void LuaPushInteger(LuaState L, int val)
		{
			L.Push(DynValue.NewNumber(val));
		}

		protected static int LuaToBoolean(LuaState L, int p)
		{
			if (!GetArgument(L, p).CastToBool())
			{
				return 0;
			}
			return 1;
		}

		protected static string LuaToLString(LuaState luaState, int p, out uint l)
		{
			return LuaLCheckLString(luaState, p, out l);
		}

		protected static string LuaToString(LuaState luaState, int p)
		{
			uint l;
			return LuaLCheckLString(luaState, p, out l);
		}

		protected static void LuaLAddValue(LuaLBuffer b)
		{
			b.StringBuilder.Append(b.LuaState.Pop().ToPrintString());
		}

		protected static void LuaLAddLString(LuaLBuffer b, CharPtr s, uint p)
		{
			b.StringBuilder.Append(s.ToString((int)p));
		}

		protected static void LuaLAddString(LuaLBuffer b, string s)
		{
			b.StringBuilder.Append(s.ToString());
		}

		protected static int LuaLOptInteger(LuaState L, int pos, int def)
		{
			DynValue dynValue = ArgAsType(L, pos, DataType.Number, true);
			if (dynValue.IsNil())
			{
				return def;
			}
			return (int)dynValue.Number;
		}

		protected static int LuaLCheckInteger(LuaState L, int pos)
		{
			return (int)ArgAsType(L, pos, DataType.Number).Number;
		}

		protected static void LuaLArgCheck(LuaState L, bool condition, int argNum, string message)
		{
			if (!condition)
			{
				LuaLArgError(L, argNum, message);
			}
		}

		protected static int LuaLCheckInt(LuaState L, int argNum)
		{
			return LuaLCheckInteger(L, argNum);
		}

		protected static int LuaGetTop(LuaState L)
		{
			return L.Count;
		}

		protected static int LuaLError(LuaState luaState, string message, params object[] args)
		{
			throw new ScriptRuntimeException(message, args);
		}

		protected static void LuaLAddChar(LuaLBuffer b, char p)
		{
			b.StringBuilder.Append(p);
		}

		protected static void LuaLBuffInit(LuaState L, LuaLBuffer b)
		{
		}

		protected static void LuaPushLiteral(LuaState L, string literalString)
		{
			L.Push(DynValue.NewString(literalString));
		}

		protected static void LuaLPushResult(LuaLBuffer b)
		{
			LuaPushLiteral(b.LuaState, b.StringBuilder.ToString());
		}

		protected static void LuaPushLString(LuaState L, CharPtr s, uint len)
		{
			string str = s.ToString((int)len);
			L.Push(DynValue.NewString(str));
		}

		protected static void LuaLCheckStack(LuaState L, int n, string message)
		{
		}

		protected static string LUA_QL(string p)
		{
			return "'" + p + "'";
		}

		protected static void LuaPushNil(LuaState L)
		{
			L.Push(DynValue.Nil);
		}

		protected static void LuaAssert(bool p)
		{
		}

		protected static string LuaLTypeName(LuaState L, int p)
		{
			return L.At(p).Type.ToErrorTypeString();
		}

		protected static int LuaIsString(LuaState L, int p)
		{
			DynValue dynValue = L.At(p);
			if (dynValue.Type != DataType.String && dynValue.Type != DataType.Number)
			{
				return 0;
			}
			return 1;
		}

		protected static void LuaPop(LuaState L, int p)
		{
			for (int i = 0; i < p; i++)
			{
				L.Pop();
			}
		}

		protected static void LuaGetTable(LuaState L, int p)
		{
			DynValue key = L.Pop();
			DynValue dynValue = L.At(p);
			if (dynValue.Type != DataType.Table)
			{
				throw new NotImplementedException();
			}
			DynValue v = dynValue.Table.Get(key);
			L.Push(v);
		}

		protected static int LuaLOptInt(LuaState L, int pos, int def)
		{
			return LuaLOptInteger(L, pos, def);
		}

		protected static CharPtr LuaLCheckString(LuaState L, int p)
		{
			uint l;
			return LuaLCheckLString(L, p, out l);
		}

		protected static string LuaLCheckStringStr(LuaState L, int p)
		{
			uint l;
			return LuaLCheckLString(L, p, out l);
		}

		protected static void LuaLArgError(LuaState L, int arg, string p)
		{
			throw ScriptRuntimeException.BadArgument(arg - 1, L.FunctionName, p);
		}

		protected static double LuaLCheckNumber(LuaState L, int pos)
		{
			return ArgAsType(L, pos, DataType.Number).Number;
		}

		protected static void LuaPushValue(LuaState L, int arg)
		{
			DynValue v = L.At(arg);
			L.Push(v);
		}

		protected static void LuaCall(LuaState L, int nargs, int nresults = -1)
		{
			DynValue[] topArray = L.GetTopArray(nargs);
			L.Discard(nargs);
			DynValue func = L.Pop();
			DynValue dynValue = L.ExecutionContext.Call(func, topArray);
			switch (nresults)
			{
			case -1:
				nresults = ((dynValue.Type != DataType.Tuple) ? 1 : dynValue.Tuple.Length);
				break;
			case 0:
				return;
			}
			DynValue[] array = ((dynValue.Type == DataType.Tuple) ? dynValue.Tuple : new DynValue[1] { dynValue });
			int num = 0;
			int num2 = 0;
			while (num2 < array.Length && num < nresults)
			{
				L.Push(array[num2]);
				num2++;
				num++;
			}
			while (num < nresults)
			{
				L.Push(DynValue.Nil);
			}
		}

		protected static int memcmp(CharPtr ptr1, CharPtr ptr2, uint size)
		{
			return memcmp(ptr1, ptr2, (int)size);
		}

		protected static int memcmp(CharPtr ptr1, CharPtr ptr2, int size)
		{
			for (int i = 0; i < size; i++)
			{
				if (ptr1[i] != ptr2[i])
				{
					if (ptr1[i] < ptr2[i])
					{
						return -1;
					}
					return 1;
				}
			}
			return 0;
		}

		protected static CharPtr memchr(CharPtr ptr, char c, uint count)
		{
			for (uint num = 0u; num < count; num++)
			{
				if (ptr[num] == c)
				{
					return new CharPtr(ptr.chars, (int)(ptr.index + num));
				}
			}
			return null;
		}

		protected static CharPtr strpbrk(CharPtr str, CharPtr charset)
		{
			for (int i = 0; str[i] != 0; i++)
			{
				for (int j = 0; charset[j] != 0; j++)
				{
					if (str[i] == charset[j])
					{
						return new CharPtr(str.chars, str.index + i);
					}
				}
			}
			return null;
		}

		protected static bool isalpha(char c)
		{
			return char.IsLetter(c);
		}

		protected static bool iscntrl(char c)
		{
			return char.IsControl(c);
		}

		protected static bool isdigit(char c)
		{
			return char.IsDigit(c);
		}

		protected static bool islower(char c)
		{
			return char.IsLower(c);
		}

		protected static bool ispunct(char c)
		{
			return char.IsPunctuation(c);
		}

		protected static bool isspace(char c)
		{
			if (c != ' ')
			{
				if (c >= '\t')
				{
					return c <= '\r';
				}
				return false;
			}
			return true;
		}

		protected static bool isupper(char c)
		{
			return char.IsUpper(c);
		}

		protected static bool isalnum(char c)
		{
			return char.IsLetterOrDigit(c);
		}

		protected static bool isxdigit(char c)
		{
			return "0123456789ABCDEFabcdef".IndexOf(c) >= 0;
		}

		protected static bool isgraph(char c)
		{
			if (!char.IsControl(c))
			{
				return !char.IsWhiteSpace(c);
			}
			return false;
		}

		protected static bool isalpha(int c)
		{
			return char.IsLetter((char)c);
		}

		protected static bool iscntrl(int c)
		{
			return char.IsControl((char)c);
		}

		protected static bool isdigit(int c)
		{
			return char.IsDigit((char)c);
		}

		protected static bool islower(int c)
		{
			return char.IsLower((char)c);
		}

		protected static bool ispunct(int c)
		{
			if ((ushort)c != 32)
			{
				return !isalnum((char)c);
			}
			return false;
		}

		protected static bool isspace(int c)
		{
			if ((ushort)c != 32)
			{
				if ((ushort)c >= 9)
				{
					return (ushort)c <= 13;
				}
				return false;
			}
			return true;
		}

		protected static bool isupper(int c)
		{
			return char.IsUpper((char)c);
		}

		protected static bool isalnum(int c)
		{
			return char.IsLetterOrDigit((char)c);
		}

		protected static bool isgraph(int c)
		{
			if (!char.IsControl((char)c))
			{
				return !char.IsWhiteSpace((char)c);
			}
			return false;
		}

		protected static char tolower(char c)
		{
			return char.ToLower(c);
		}

		protected static char toupper(char c)
		{
			return char.ToUpper(c);
		}

		protected static char tolower(int c)
		{
			return char.ToLower((char)c);
		}

		protected static char toupper(int c)
		{
			return char.ToUpper((char)c);
		}

		protected static CharPtr strchr(CharPtr str, char c)
		{
			for (int i = str.index; str.chars[i] != 0; i++)
			{
				if (str.chars[i] == c)
				{
					return new CharPtr(str.chars, i);
				}
			}
			return null;
		}

		protected static CharPtr strcpy(CharPtr dst, CharPtr src)
		{
			int i;
			for (i = 0; src[i] != 0; i++)
			{
				dst[i] = src[i];
			}
			dst[i] = '\0';
			return dst;
		}

		protected static CharPtr strncpy(CharPtr dst, CharPtr src, int length)
		{
			int i;
			for (i = 0; src[i] != 0 && i < length; i++)
			{
				dst[i] = src[i];
			}
			while (i < length)
			{
				dst[i++] = '\0';
			}
			return dst;
		}

		protected static int strlen(CharPtr str)
		{
			int i;
			for (i = 0; str[i] != 0; i++)
			{
			}
			return i;
		}

		public static void sprintf(CharPtr buffer, CharPtr str, params object[] argv)
		{
			string text = Tools.sprintf(str.ToString(), argv);
			strcpy(buffer, text);
		}
	}
}
