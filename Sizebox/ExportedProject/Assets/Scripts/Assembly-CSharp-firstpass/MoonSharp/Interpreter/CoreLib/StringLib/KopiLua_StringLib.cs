using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop.LuaStateInterop;

namespace MoonSharp.Interpreter.CoreLib.StringLib
{
	internal class KopiLua_StringLib : LuaBase
	{
		public class MatchState
		{
			public class capture_
			{
				public CharPtr init;

				public int len;
			}

			public int matchdepth;

			public CharPtr src_init;

			public CharPtr src_end;

			public LuaState L;

			public int level;

			public capture_[] capture = new capture_[32];

			public MatchState()
			{
				for (int i = 0; i < 32; i++)
				{
					capture[i] = new capture_();
				}
			}
		}

		private class GMatchAuxData
		{
			public CharPtr S;

			public CharPtr P;

			public uint LS;

			public uint POS;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass29_0
		{
			public ScriptExecutionContext executionContext;

			internal int _003Cgmatch_aux_2_003Eb__0(LuaState L)
			{
				return gmatch_aux(L, (GMatchAuxData)executionContext.AdditionalData);
			}
		}

		public const int LUA_MAXCAPTURES = 32;

		public const int CAP_UNFINISHED = -1;

		public const int CAP_POSITION = -2;

		public const int MAXCCALLS = 200;

		public const char L_ESC = '%';

		public const string SPECIALS = "^$*+?.([%-";

		public const int MAX_ITEM = 512;

		public const string FLAGS = "-+ #0";

		public static readonly int MAX_FORMAT = "-+ #0".Length + 1 + ("l".Length + 1) + 10;

		private static int posrelat(int pos, uint len)
		{
			if (pos < 0)
			{
				pos += (int)(len + 1);
			}
			if (pos < 0)
			{
				return 0;
			}
			return pos;
		}

		private static int check_capture(MatchState ms, int l)
		{
			l -= 49;
			if (l < 0 || l >= ms.level || ms.capture[l].len == -1)
			{
				return LuaBase.LuaLError(ms.L, "invalid capture index {0}", l + 1);
			}
			return l;
		}

		private static int capture_to_close(MatchState ms)
		{
			int level = ms.level;
			for (level--; level >= 0; level--)
			{
				if (ms.capture[level].len == -1)
				{
					return level;
				}
			}
			return LuaBase.LuaLError(ms.L, "invalid pattern capture");
		}

		private static CharPtr classend(MatchState ms, CharPtr p)
		{
			p = new CharPtr(p);
			char c = p[0];
			p = p.next();
			switch (c)
			{
			case '%':
				if (p[0] == '\0')
				{
					LuaBase.LuaLError(ms.L, "malformed pattern (ends with " + LuaBase.LUA_QL("%") + ")");
				}
				return p + 1;
			case '[':
				if (p[0] == '^')
				{
					p = p.next();
				}
				do
				{
					if (p[0] == '\0')
					{
						LuaBase.LuaLError(ms.L, "malformed pattern (missing " + LuaBase.LUA_QL("]") + ")");
					}
					c = p[0];
					p = p.next();
					if (c == '%' && p[0] != 0)
					{
						p = p.next();
					}
				}
				while (p[0] != ']');
				return p + 1;
			default:
				return p;
			}
		}

		private static int match_class(char c, char cl)
		{
			bool flag;
			switch (LuaBase.tolower(cl))
			{
			case 'a':
				flag = LuaBase.isalpha(c);
				break;
			case 'c':
				flag = LuaBase.iscntrl(c);
				break;
			case 'd':
				flag = LuaBase.isdigit(c);
				break;
			case 'l':
				flag = LuaBase.islower(c);
				break;
			case 'p':
				flag = LuaBase.ispunct(c);
				break;
			case 's':
				flag = LuaBase.isspace(c);
				break;
			case 'g':
				flag = LuaBase.isgraph(c);
				break;
			case 'u':
				flag = LuaBase.isupper(c);
				break;
			case 'w':
				flag = LuaBase.isalnum(c);
				break;
			case 'x':
				flag = LuaBase.isxdigit(c);
				break;
			case 'z':
				flag = c == '\0';
				break;
			default:
				if (cl != c)
				{
					return 0;
				}
				return 1;
			}
			if (!LuaBase.islower(cl))
			{
				if (flag)
				{
					return 0;
				}
				return 1;
			}
			if (!flag)
			{
				return 0;
			}
			return 1;
		}

		private static int matchbracketclass(int c, CharPtr p, CharPtr ec)
		{
			int num = 1;
			if (p[1] == '^')
			{
				num = 0;
				p = p.next();
			}
			while ((p = p.next()) < ec)
			{
				if (p == '%')
				{
					p = p.next();
					if (match_class((char)c, p[0]) != 0)
					{
						return num;
					}
				}
				else if (p[1] == '-' && p + 2 < ec)
				{
					p += 2;
					if ((byte)p[-2] <= c && c <= (byte)p[0])
					{
						return num;
					}
				}
				else if ((byte)p[0] == c)
				{
					return num;
				}
			}
			if (num != 0)
			{
				return 0;
			}
			return 1;
		}

		private static int singlematch(int c, CharPtr p, CharPtr ep)
		{
			switch (p[0])
			{
			case '.':
				return 1;
			case '%':
				return match_class((char)c, p[1]);
			case '[':
				return matchbracketclass(c, p, ep - 1);
			default:
				if ((byte)p[0] != c)
				{
					return 0;
				}
				return 1;
			}
		}

		private static CharPtr matchbalance(MatchState ms, CharPtr s, CharPtr p)
		{
			if (p[0] == '\0' || p[1] == '\0')
			{
				LuaBase.LuaLError(ms.L, "unbalanced pattern");
			}
			if (s[0] != p[0])
			{
				return null;
			}
			int num = p[0];
			int num2 = p[1];
			int num3 = 1;
			while ((s = s.next()) < ms.src_end)
			{
				if (s[0] == num2)
				{
					if (--num3 == 0)
					{
						return s + 1;
					}
				}
				else if (s[0] == num)
				{
					num3++;
				}
			}
			return null;
		}

		private static CharPtr max_expand(MatchState ms, CharPtr s, CharPtr p, CharPtr ep)
		{
			int i;
			for (i = 0; s + i < ms.src_end && singlematch((byte)s[i], p, ep) != 0; i++)
			{
			}
			while (i >= 0)
			{
				CharPtr charPtr = match(ms, s + i, ep + 1);
				if (charPtr != null)
				{
					return charPtr;
				}
				i--;
			}
			return null;
		}

		private static CharPtr min_expand(MatchState ms, CharPtr s, CharPtr p, CharPtr ep)
		{
			while (true)
			{
				CharPtr charPtr = match(ms, s, ep + 1);
				if (charPtr != null)
				{
					return charPtr;
				}
				if (!(s < ms.src_end) || singlematch((byte)s[0], p, ep) == 0)
				{
					break;
				}
				s = s.next();
			}
			return null;
		}

		private static CharPtr start_capture(MatchState ms, CharPtr s, CharPtr p, int what)
		{
			int level = ms.level;
			if (level >= 32)
			{
				LuaBase.LuaLError(ms.L, "too many captures");
			}
			ms.capture[level].init = s;
			ms.capture[level].len = what;
			ms.level = level + 1;
			CharPtr charPtr = match(ms, s, p);
			if (charPtr == null)
			{
				ms.level--;
			}
			return charPtr;
		}

		private static CharPtr end_capture(MatchState ms, CharPtr s, CharPtr p)
		{
			int num = capture_to_close(ms);
			ms.capture[num].len = s - ms.capture[num].init;
			CharPtr charPtr = match(ms, s, p);
			if (charPtr == null)
			{
				ms.capture[num].len = -1;
			}
			return charPtr;
		}

		private static CharPtr match_capture(MatchState ms, CharPtr s, int l)
		{
			l = check_capture(ms, l);
			uint len = (uint)ms.capture[l].len;
			if ((uint)(ms.src_end - s) >= len && LuaBase.memcmp(ms.capture[l].init, s, len) == 0)
			{
				return s + len;
			}
			return null;
		}

		private static CharPtr match(MatchState ms, CharPtr s, CharPtr p)
		{
			s = new CharPtr(s);
			p = new CharPtr(p);
			if (ms.matchdepth-- == 0)
			{
				LuaBase.LuaLError(ms.L, "pattern too complex");
			}
			while (true)
			{
				switch (p[0])
				{
				case '(':
					if (p[1] == ')')
					{
						return start_capture(ms, s, p + 2, -2);
					}
					return start_capture(ms, s, p + 1, -1);
				case ')':
					return end_capture(ms, s, p + 1);
				case '%':
				{
					switch (p[1])
					{
					case 'b':
						s = matchbalance(ms, s, p + 2);
						if (s == null)
						{
							return null;
						}
						p += 4;
						continue;
					case 'f':
					{
						p += 2;
						if (p[0] != '[')
						{
							LuaBase.LuaLError(ms.L, "missing " + LuaBase.LUA_QL("[") + " after " + LuaBase.LUA_QL("%f") + " in pattern");
						}
						CharPtr charPtr = classend(ms, p);
						if (matchbracketclass((byte)((!(s == ms.src_init)) ? s[-1] : '\0'), p, charPtr - 1) != 0 || matchbracketclass((byte)s[0], p, charPtr - 1) == 0)
						{
							return null;
						}
						p = charPtr;
						continue;
					}
					}
					if (LuaBase.isdigit(p[1]))
					{
						s = match_capture(ms, s, (byte)p[1]);
						if (s == null)
						{
							return null;
						}
						p += 2;
						continue;
					}
					CharPtr charPtr2 = classend(ms, p);
					int num = ((s < ms.src_end && singlematch((byte)s[0], p, charPtr2) != 0) ? 1 : 0);
					switch (charPtr2[0])
					{
					case '?':
					{
						CharPtr result;
						if (num != 0 && (result = match(ms, s + 1, charPtr2 + 1)) != null)
						{
							return result;
						}
						p = charPtr2 + 1;
						continue;
					}
					case '*':
						return max_expand(ms, s, p, charPtr2);
					case '+':
						if (num == 0)
						{
							return null;
						}
						return max_expand(ms, s + 1, p, charPtr2);
					case '-':
						return min_expand(ms, s, p, charPtr2);
					}
					if (num == 0)
					{
						return null;
					}
					s = s.next();
					p = charPtr2;
					continue;
				}
				case '\0':
					return s;
				case '$':
					if (p[1] == '\0')
					{
						if (!(s == ms.src_end))
						{
							return null;
						}
						return s;
					}
					break;
				}
				CharPtr charPtr3 = classend(ms, p);
				int num2 = ((s < ms.src_end && singlematch((byte)s[0], p, charPtr3) != 0) ? 1 : 0);
				switch (charPtr3[0])
				{
				case '?':
				{
					CharPtr result2;
					if (num2 != 0 && (result2 = match(ms, s + 1, charPtr3 + 1)) != null)
					{
						return result2;
					}
					p = charPtr3 + 1;
					continue;
				}
				case '*':
					return max_expand(ms, s, p, charPtr3);
				case '+':
					if (num2 == 0)
					{
						return null;
					}
					return max_expand(ms, s + 1, p, charPtr3);
				case '-':
					return min_expand(ms, s, p, charPtr3);
				}
				if (num2 == 0)
				{
					return null;
				}
				s = s.next();
				p = charPtr3;
			}
		}

		private static CharPtr lmemfind(CharPtr s1, uint l1, CharPtr s2, uint l2)
		{
			if (l2 == 0)
			{
				return s1;
			}
			if (l2 > l1)
			{
				return null;
			}
			l2--;
			l1 -= l2;
			CharPtr charPtr;
			while (l1 != 0 && (charPtr = LuaBase.memchr(s1, s2[0], l1)) != null)
			{
				charPtr = charPtr.next();
				if (LuaBase.memcmp(charPtr, s2 + 1, l2) == 0)
				{
					return charPtr - 1;
				}
				l1 -= (uint)(charPtr - s1);
				s1 = charPtr;
			}
			return null;
		}

		private static void push_onecapture(MatchState ms, int i, CharPtr s, CharPtr e)
		{
			if (i >= ms.level)
			{
				if (i == 0)
				{
					LuaBase.LuaPushLString(ms.L, s, (uint)(e - s));
				}
				else
				{
					LuaBase.LuaLError(ms.L, "invalid capture index");
				}
				return;
			}
			int len = ms.capture[i].len;
			if (len == -1)
			{
				LuaBase.LuaLError(ms.L, "unfinished capture");
			}
			if (len == -2)
			{
				LuaBase.LuaPushInteger(ms.L, ms.capture[i].init - ms.src_init + 1);
			}
			else
			{
				LuaBase.LuaPushLString(ms.L, ms.capture[i].init, (uint)len);
			}
		}

		private static int push_captures(MatchState ms, CharPtr s, CharPtr e)
		{
			int num = ((ms.level == 0 && s != null) ? 1 : ms.level);
			LuaBase.LuaLCheckStack(ms.L, num, "too many captures");
			for (int i = 0; i < num; i++)
			{
				push_onecapture(ms, i, s, e);
			}
			return num;
		}

		private static int str_find_aux(LuaState L, int find)
		{
			uint l;
			CharPtr charPtr = LuaBase.LuaLCheckLString(L, 1, out l);
			uint l2;
			CharPtr charPtr2 = PatchPattern(LuaBase.LuaLCheckLString(L, 2, out l2));
			int num = posrelat(LuaBase.LuaLOptInteger(L, 3, 1), l) - 1;
			if (num < 0)
			{
				num = 0;
			}
			else if ((uint)num > l)
			{
				num = (int)l;
			}
			if (find != 0 && (LuaBase.LuaToBoolean(L, 4) != 0 || LuaBase.strpbrk(charPtr2, "^$*+?.([%-") == null))
			{
				CharPtr charPtr3 = lmemfind(charPtr + num, (uint)(l - num), charPtr2, l2);
				if (charPtr3 != null)
				{
					LuaBase.LuaPushInteger(L, charPtr3 - charPtr + 1);
					LuaBase.LuaPushInteger(L, (int)(charPtr3 - charPtr + l2));
					return 2;
				}
			}
			else
			{
				MatchState matchState = new MatchState();
				int num2 = 0;
				if (charPtr2[0] == '^')
				{
					charPtr2 = charPtr2.next();
					num2 = 1;
				}
				CharPtr charPtr4 = charPtr + num;
				matchState.L = L;
				matchState.matchdepth = 200;
				matchState.src_init = charPtr;
				matchState.src_end = charPtr + l;
				do
				{
					matchState.level = 0;
					matchState.matchdepth = 200;
					CharPtr charPtr5;
					if ((charPtr5 = match(matchState, charPtr4, charPtr2)) != null)
					{
						if (find != 0)
						{
							LuaBase.LuaPushInteger(L, charPtr4 - charPtr + 1);
							LuaBase.LuaPushInteger(L, charPtr5 - charPtr);
							return push_captures(matchState, null, null) + 2;
						}
						return push_captures(matchState, charPtr4, charPtr5);
					}
				}
				while ((charPtr4 = charPtr4.next()) <= matchState.src_end && num2 == 0);
			}
			LuaBase.LuaPushNil(L);
			return 1;
		}

		public static int str_find(LuaState L)
		{
			return str_find_aux(L, 1);
		}

		public static int str_match(LuaState L)
		{
			return str_find_aux(L, 0);
		}

		private static int gmatch_aux(LuaState L, GMatchAuxData auxdata)
		{
			MatchState matchState = new MatchState();
			uint lS = auxdata.LS;
			CharPtr s = auxdata.S;
			CharPtr p = auxdata.P;
			matchState.L = L;
			matchState.matchdepth = 200;
			matchState.src_init = s;
			matchState.src_end = s + lS;
			CharPtr charPtr = s + auxdata.POS;
			while (charPtr <= matchState.src_end)
			{
				matchState.level = 0;
				matchState.matchdepth = 200;
				CharPtr charPtr2;
				if ((charPtr2 = match(matchState, charPtr, p)) != null)
				{
					int num = charPtr2 - s;
					if (charPtr2 == charPtr)
					{
						num++;
					}
					auxdata.POS = (uint)num;
					return push_captures(matchState, charPtr, charPtr2);
				}
				charPtr = charPtr.next();
			}
			return 0;
		}

		private static DynValue gmatch_aux_2(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			_003C_003Ec__DisplayClass29_0 _003C_003Ec__DisplayClass29_ = new _003C_003Ec__DisplayClass29_0();
			_003C_003Ec__DisplayClass29_.executionContext = executionContext;
			return _003C_003Ec__DisplayClass29_.executionContext.EmulateClassicCall(args, "gmatch", _003C_003Ec__DisplayClass29_._003Cgmatch_aux_2_003Eb__0);
		}

		public static int str_gmatch(LuaState L)
		{
			CallbackFunction callbackFunction = new CallbackFunction(gmatch_aux_2, "gmatch");
			string @string = LuaBase.ArgAsType(L, 1, DataType.String).String;
			string str = PatchPattern(LuaBase.ArgAsType(L, 2, DataType.String).String);
			callbackFunction.AdditionalData = new GMatchAuxData
			{
				S = new CharPtr(@string),
				P = new CharPtr(str),
				LS = (uint)@string.Length,
				POS = 0u
			};
			L.Push(DynValue.NewCallback(callbackFunction));
			return 1;
		}

		private static int gfind_nodef(LuaState L)
		{
			return LuaBase.LuaLError(L, LuaBase.LUA_QL("string.gfind") + " was renamed to " + LuaBase.LUA_QL("string.gmatch"));
		}

		private static void add_s(MatchState ms, LuaLBuffer b, CharPtr s, CharPtr e)
		{
			uint l;
			CharPtr charPtr = LuaBase.LuaToLString(ms.L, 3, out l);
			for (uint num = 0u; num < l; num++)
			{
				if (charPtr[num] != '%')
				{
					LuaBase.LuaLAddChar(b, charPtr[num]);
					continue;
				}
				num++;
				if (!LuaBase.isdigit(charPtr[num]))
				{
					if (charPtr[num] != '%')
					{
						LuaBase.LuaLError(ms.L, "invalid use of '%' in replacement string");
					}
					LuaBase.LuaLAddChar(b, charPtr[num]);
				}
				else if (charPtr[num] == '0')
				{
					LuaBase.LuaLAddLString(b, s, (uint)(e - s));
				}
				else
				{
					push_onecapture(ms, charPtr[num] - 49, s, e);
					LuaBase.LuaLAddValue(b);
				}
			}
		}

		private static void add_value(MatchState ms, LuaLBuffer b, CharPtr s, CharPtr e)
		{
			LuaState l = ms.L;
			switch (LuaBase.LuaType(l, 3))
			{
			case 3:
			case 4:
				add_s(ms, b, s, e);
				return;
			case 6:
			{
				LuaBase.LuaPushValue(l, 3);
				int nargs = push_captures(ms, s, e);
				LuaBase.LuaCall(l, nargs, 1);
				break;
			}
			case 5:
				push_onecapture(ms, 0, s, e);
				LuaBase.LuaGetTable(l, 3);
				break;
			}
			if (LuaBase.LuaToBoolean(l, -1) == 0)
			{
				LuaBase.LuaPop(l, 1);
				LuaBase.LuaPushLString(l, s, (uint)(e - s));
			}
			else if (LuaBase.LuaIsString(l, -1) == 0)
			{
				LuaBase.LuaLError(l, "invalid replacement value (a {0})", LuaBase.LuaLTypeName(l, -1));
			}
			LuaBase.LuaLAddValue(b);
		}

		public static int str_gsub(LuaState L)
		{
			uint l;
			CharPtr charPtr = LuaBase.LuaLCheckLString(L, 1, out l);
			CharPtr charPtr2 = PatchPattern(LuaBase.LuaLCheckStringStr(L, 2));
			int num = LuaBase.LuaType(L, 3);
			int num2 = LuaBase.LuaLOptInt(L, 4, (int)(l + 1));
			int num3 = 0;
			if (charPtr2[0] == '^')
			{
				charPtr2 = charPtr2.next();
				num3 = 1;
			}
			int num4 = 0;
			MatchState matchState = new MatchState();
			LuaLBuffer b = new LuaLBuffer(L);
			LuaBase.LuaLArgCheck(L, num == 3 || num == 4 || num == 6 || num == 5 || num == 7, 3, "string/function/table expected");
			LuaBase.LuaLBuffInit(L, b);
			matchState.L = L;
			matchState.matchdepth = 200;
			matchState.src_init = charPtr;
			matchState.src_end = charPtr + l;
			while (num4 < num2)
			{
				matchState.level = 0;
				matchState.matchdepth = 200;
				CharPtr charPtr3 = match(matchState, charPtr, charPtr2);
				if (charPtr3 != null)
				{
					num4++;
					add_value(matchState, b, charPtr, charPtr3);
				}
				if (charPtr3 != null && charPtr3 > charPtr)
				{
					charPtr = charPtr3;
				}
				else
				{
					if (!(charPtr < matchState.src_end))
					{
						break;
					}
					char p = charPtr[0];
					charPtr = charPtr.next();
					LuaBase.LuaLAddChar(b, p);
				}
				if (num3 != 0)
				{
					break;
				}
			}
			LuaBase.LuaLAddLString(b, charPtr, (uint)(matchState.src_end - charPtr));
			LuaBase.LuaLPushResult(b);
			LuaBase.LuaPushInteger(L, num4);
			return 2;
		}

		private static void addquoted(LuaState L, LuaLBuffer b, int arg)
		{
			uint l;
			CharPtr charPtr = LuaBase.LuaLCheckLString(L, arg, out l);
			LuaBase.LuaLAddChar(b, '"');
			while (l-- != 0)
			{
				switch (charPtr[0])
				{
				case '\n':
				case '"':
				case '\\':
					LuaBase.LuaLAddChar(b, '\\');
					LuaBase.LuaLAddChar(b, charPtr[0]);
					break;
				case '\r':
					LuaBase.LuaLAddLString(b, "\\r", 2u);
					break;
				default:
					if (charPtr[0] < '\u0010')
					{
						bool flag = false;
						if (l >= 1 && char.IsNumber(charPtr[1]))
						{
							flag = true;
						}
						if (flag)
						{
							LuaBase.LuaLAddString(b, string.Format("\\{0:000}", (int)charPtr[0]));
						}
						else
						{
							LuaBase.LuaLAddString(b, string.Format("\\{0}", (int)charPtr[0]));
						}
					}
					else
					{
						LuaBase.LuaLAddChar(b, charPtr[0]);
					}
					break;
				}
				charPtr = charPtr.next();
			}
			LuaBase.LuaLAddChar(b, '"');
		}

		private static CharPtr scanformat(LuaState L, CharPtr strfrmt, CharPtr form)
		{
			CharPtr charPtr = strfrmt;
			while (charPtr[0] != 0 && LuaBase.strchr("-+ #0", charPtr[0]) != null)
			{
				charPtr = charPtr.next();
			}
			if ((uint)(charPtr - strfrmt) >= "-+ #0".Length + 1)
			{
				LuaBase.LuaLError(L, "invalid format (repeated flags)");
			}
			if (LuaBase.isdigit((byte)charPtr[0]))
			{
				charPtr = charPtr.next();
			}
			if (LuaBase.isdigit((byte)charPtr[0]))
			{
				charPtr = charPtr.next();
			}
			if (charPtr[0] == '.')
			{
				charPtr = charPtr.next();
				if (LuaBase.isdigit((byte)charPtr[0]))
				{
					charPtr = charPtr.next();
				}
				if (LuaBase.isdigit((byte)charPtr[0]))
				{
					charPtr = charPtr.next();
				}
			}
			if (LuaBase.isdigit((byte)charPtr[0]))
			{
				LuaBase.LuaLError(L, "invalid format (width or precision too long)");
			}
			form[0] = '%';
			form = form.next();
			LuaBase.strncpy(form, strfrmt, charPtr - strfrmt + 1);
			form += charPtr - strfrmt + 1;
			form[0] = '\0';
			return charPtr;
		}

		private static void addintlen(CharPtr form)
		{
			uint num = (uint)LuaBase.strlen(form);
			char value = form[num - 1];
			LuaBase.strcpy(form + num - 1, "l");
			form[num + ("l".Length + 1) - 2] = value;
			form[num + ("l".Length + 1) - 1] = '\0';
		}

		public static int str_format(LuaState L)
		{
			int num = LuaBase.LuaGetTop(L);
			int num2 = 1;
			uint l;
			CharPtr charPtr = LuaBase.LuaLCheckLString(L, num2, out l);
			CharPtr charPtr2 = charPtr + l;
			LuaLBuffer b = new LuaLBuffer(L);
			LuaBase.LuaLBuffInit(L, b);
			while (charPtr < charPtr2)
			{
				if (charPtr[0] != '%')
				{
					LuaBase.LuaLAddChar(b, charPtr[0]);
					charPtr = charPtr.next();
					continue;
				}
				if (charPtr[1] == '%')
				{
					LuaBase.LuaLAddChar(b, charPtr[0]);
					charPtr += 2;
					continue;
				}
				charPtr = charPtr.next();
				CharPtr charPtr3 = new char[MAX_FORMAT];
				CharPtr charPtr4 = new char[512];
				if (++num2 > num)
				{
					LuaBase.LuaLArgError(L, num2, "no value");
				}
				charPtr = scanformat(L, charPtr, charPtr3);
				char c = charPtr[0];
				charPtr = charPtr.next();
				switch (c)
				{
				case 'c':
					LuaBase.sprintf(charPtr4, charPtr3, (int)LuaBase.LuaLCheckNumber(L, num2));
					break;
				case 'd':
				case 'i':
					addintlen(charPtr3);
					LuaBase.sprintf(charPtr4, charPtr3, (long)LuaBase.LuaLCheckNumber(L, num2));
					break;
				case 'X':
				case 'o':
				case 'u':
				case 'x':
					addintlen(charPtr3);
					LuaBase.sprintf(charPtr4, charPtr3, (ulong)LuaBase.LuaLCheckNumber(L, num2));
					break;
				case 'E':
				case 'G':
				case 'e':
				case 'f':
				case 'g':
					LuaBase.sprintf(charPtr4, charPtr3, LuaBase.LuaLCheckNumber(L, num2));
					break;
				case 'q':
					addquoted(L, b, num2);
					continue;
				case 's':
				{
					uint l2;
					CharPtr charPtr5 = LuaBase.LuaLCheckLString(L, num2, out l2);
					if (LuaBase.strchr(charPtr3, '.') == null && l2 >= 100)
					{
						LuaBase.LuaPushValue(L, num2);
						LuaBase.LuaLAddValue(b);
						continue;
					}
					LuaBase.sprintf(charPtr4, charPtr3, charPtr5);
					break;
				}
				default:
					return LuaBase.LuaLError(L, "invalid option " + LuaBase.LUA_QL("%" + c) + " to " + LuaBase.LUA_QL("format"), charPtr[-1]);
				}
				LuaBase.LuaLAddLString(b, charPtr4, (uint)LuaBase.strlen(charPtr4));
			}
			LuaBase.LuaLPushResult(b);
			return 1;
		}

		private static string PatchPattern(string charPtr)
		{
			return charPtr.Replace("\0", "%z");
		}
	}
}
