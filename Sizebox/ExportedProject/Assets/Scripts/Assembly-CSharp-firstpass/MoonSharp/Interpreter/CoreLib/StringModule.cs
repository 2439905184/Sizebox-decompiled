using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using MoonSharp.Interpreter.CoreLib.StringLib;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "string")]
	public class StringModule
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<int, int> _003C_003E9__4_0;

			public static Func<int, int> _003C_003E9__5_0;

			internal int _003Cbyte_003Eb__4_0(int i)
			{
				return Unicode2Ascii(i);
			}

			internal int _003Cunicode_003Eb__5_0(int i)
			{
				return i;
			}
		}

		public const string BASE64_DUMP_HEADER = "MoonSharp_dump_b64::";

		public static void MoonSharpInit(Table globalTable, Table stringTable)
		{
			Table table = new Table(globalTable.OwnerScript);
			table.Set("__index", DynValue.NewTable(stringTable));
			globalTable.OwnerScript.SetTypeMetatable(DataType.String, table);
		}

		[MoonSharpModuleMethod]
		public static DynValue dump(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue function = args.AsType(0, "dump", DataType.Function);
			try
			{
				byte[] inArray;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					executionContext.GetScript().Dump(function, memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					inArray = memoryStream.ToArray();
				}
				string text = Convert.ToBase64String(inArray);
				return DynValue.NewString("MoonSharp_dump_b64::" + text);
			}
			catch (Exception ex)
			{
				throw new ScriptRuntimeException(ex.Message);
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue @char(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			StringBuilder stringBuilder = new StringBuilder(args.Count);
			for (int i = 0; i < args.Count; i++)
			{
				DynValue dynValue = args[i];
				double num = 0.0;
				if (dynValue.Type == DataType.String)
				{
					double? num2 = dynValue.CastToNumber();
					if (!num2.HasValue)
					{
						args.AsType(i, "char", DataType.Number);
					}
					else
					{
						num = num2.Value;
					}
				}
				else
				{
					args.AsType(i, "char", DataType.Number);
					num = dynValue.Number;
				}
				stringBuilder.Append((char)num);
			}
			return DynValue.NewString(stringBuilder.ToString());
		}

		[MoonSharpModuleMethod]
		public static DynValue @byte(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args.AsType(0, "byte", DataType.String);
			DynValue vi = args.AsType(1, "byte", DataType.Number, true);
			DynValue vj = args.AsType(2, "byte", DataType.Number, true);
			return PerformByteLike(vs, vi, vj, _003C_003Ec._003C_003E9__4_0 ?? (_003C_003Ec._003C_003E9__4_0 = _003C_003Ec._003C_003E9._003Cbyte_003Eb__4_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue unicode(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args.AsType(0, "unicode", DataType.String);
			DynValue vi = args.AsType(1, "unicode", DataType.Number, true);
			DynValue vj = args.AsType(2, "unicode", DataType.Number, true);
			return PerformByteLike(vs, vi, vj, _003C_003Ec._003C_003E9__5_0 ?? (_003C_003Ec._003C_003E9__5_0 = _003C_003Ec._003C_003E9._003Cunicode_003Eb__5_0));
		}

		private static int Unicode2Ascii(int i)
		{
			if (i >= 0 && i < 255)
			{
				return i;
			}
			return 63;
		}

		private static DynValue PerformByteLike(DynValue vs, DynValue vi, DynValue vj, Func<int, int> filter)
		{
			string text = StringRange.FromLuaRange(vi, vj).ApplyToString(vs.String);
			int length = text.Length;
			DynValue[] array = new DynValue[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = DynValue.NewNumber(filter(text[i]));
			}
			return DynValue.NewTuple(array);
		}

		private static int? AdjustIndex(string s, DynValue vi, int defval)
		{
			if (vi.IsNil())
			{
				return defval;
			}
			int num = (int)Math.Round(vi.Number, 0);
			if (num == 0)
			{
				return null;
			}
			if (num > 0)
			{
				return num - 1;
			}
			return s.Length - num;
		}

		[MoonSharpModuleMethod]
		public static DynValue len(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewNumber(args.AsType(0, "len", DataType.String).String.Length);
		}

		[MoonSharpModuleMethod]
		public static DynValue match(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "match", KopiLua_StringLib.str_match);
		}

		[MoonSharpModuleMethod]
		public static DynValue gmatch(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "gmatch", KopiLua_StringLib.str_gmatch);
		}

		[MoonSharpModuleMethod]
		public static DynValue gsub(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "gsub", KopiLua_StringLib.str_gsub);
		}

		[MoonSharpModuleMethod]
		public static DynValue find(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "find", KopiLua_StringLib.str_find);
		}

		[MoonSharpModuleMethod]
		public static DynValue lower(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewString(args.AsType(0, "lower", DataType.String).String.ToLower());
		}

		[MoonSharpModuleMethod]
		public static DynValue upper(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewString(args.AsType(0, "upper", DataType.String).String.ToUpper());
		}

		[MoonSharpModuleMethod]
		public static DynValue rep(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "rep", DataType.String);
			DynValue dynValue2 = args.AsType(1, "rep", DataType.Number);
			DynValue dynValue3 = args.AsType(2, "rep", DataType.String, true);
			if (string.IsNullOrEmpty(dynValue.String) || dynValue2.Number < 1.0)
			{
				return DynValue.NewString("");
			}
			string text = (dynValue3.IsNotNil() ? dynValue3.String : null);
			int num = (int)dynValue2.Number;
			StringBuilder stringBuilder = new StringBuilder(dynValue.String.Length * num);
			for (int i = 0; i < num; i++)
			{
				if (i != 0 && text != null)
				{
					stringBuilder.Append(text);
				}
				stringBuilder.Append(dynValue.String);
			}
			return DynValue.NewString(stringBuilder.ToString());
		}

		[MoonSharpModuleMethod]
		public static DynValue format(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "format", KopiLua_StringLib.str_format);
		}

		[MoonSharpModuleMethod]
		public static DynValue reverse(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "reverse", DataType.String);
			if (string.IsNullOrEmpty(dynValue.String))
			{
				return DynValue.NewString("");
			}
			char[] array = dynValue.String.ToCharArray();
			Array.Reverse((Array)array);
			return DynValue.NewString(new string(array));
		}

		[MoonSharpModuleMethod]
		public static DynValue sub(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "sub", DataType.String);
			DynValue start = args.AsType(1, "sub", DataType.Number, true);
			DynValue end = args.AsType(2, "sub", DataType.Number, true);
			return DynValue.NewString(StringRange.FromLuaRange(start, end, -1).ApplyToString(dynValue.String));
		}

		[MoonSharpModuleMethod]
		public static DynValue startsWith(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "startsWith", DataType.String, true);
			DynValue dynValue2 = args.AsType(1, "startsWith", DataType.String, true);
			if (dynValue.IsNil() || dynValue2.IsNil())
			{
				return DynValue.False;
			}
			return DynValue.NewBoolean(dynValue.String.StartsWith(dynValue2.String));
		}

		[MoonSharpModuleMethod]
		public static DynValue endsWith(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "endsWith", DataType.String, true);
			DynValue dynValue2 = args.AsType(1, "endsWith", DataType.String, true);
			if (dynValue.IsNil() || dynValue2.IsNil())
			{
				return DynValue.False;
			}
			return DynValue.NewBoolean(dynValue.String.EndsWith(dynValue2.String));
		}

		[MoonSharpModuleMethod]
		public static DynValue contains(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "contains", DataType.String, true);
			DynValue dynValue2 = args.AsType(1, "contains", DataType.String, true);
			if (dynValue.IsNil() || dynValue2.IsNil())
			{
				return DynValue.False;
			}
			return DynValue.NewBoolean(dynValue.String.Contains(dynValue2.String));
		}
	}
}
