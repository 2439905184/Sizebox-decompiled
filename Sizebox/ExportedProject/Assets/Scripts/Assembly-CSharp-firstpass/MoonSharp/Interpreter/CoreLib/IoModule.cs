using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.CoreLib.IO;
using MoonSharp.Interpreter.Platforms;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "io")]
	public class IoModule
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<DynValue, DynValue> _003C_003E9__14_0;

			internal DynValue _003Clines_003Eb__14_0(DynValue s)
			{
				return s;
			}
		}

		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			UserData.RegisterType<FileUserDataBase>(InteropAccessMode.Default, "file");
			Table table = new Table(ioTable.OwnerScript);
			DynValue value = DynValue.NewCallback(new CallbackFunction(__index_callback, "__index_callback"));
			table.Set("__index", value);
			ioTable.MetaTable = table;
			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdIn, globalTable.OwnerScript.Options.Stdin);
			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdOut, globalTable.OwnerScript.Options.Stdout);
			SetStandardFile(globalTable.OwnerScript, StandardFileType.StdErr, globalTable.OwnerScript.Options.Stderr);
		}

		private static DynValue __index_callback(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			switch (args[1].CastToString())
			{
			case "stdin":
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdIn);
			case "stdout":
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdOut);
			case "stderr":
				return GetStandardFile(executionContext.GetScript(), StandardFileType.StdErr);
			default:
				return DynValue.Nil;
			}
		}

		private static DynValue GetStandardFile(Script S, StandardFileType file)
		{
			return S.Registry.Get("853BEAAF298648839E2C99D005E1DF94_STD_" + file);
		}

		private static void SetStandardFile(Script S, StandardFileType file, Stream optionsStream)
		{
			Table registry = S.Registry;
			optionsStream = optionsStream ?? Script.GlobalOptions.Platform.IO_GetStandardStream(file);
			FileUserDataBase fileUserDataBase = null;
			registry.Set(value: UserData.Create((file != 0) ? StandardIOFileUserDataBase.CreateOutputStream(optionsStream) : StandardIOFileUserDataBase.CreateInputStream(optionsStream)), key: "853BEAAF298648839E2C99D005E1DF94_STD_" + file);
		}

		private static FileUserDataBase GetDefaultFile(ScriptExecutionContext executionContext, StandardFileType file)
		{
			DynValue dynValue = executionContext.GetScript().Registry.Get("853BEAAF298648839E2C99D005E1DF94_" + file);
			if (dynValue.IsNil())
			{
				dynValue = GetStandardFile(executionContext.GetScript(), file);
			}
			return dynValue.CheckUserDataType<FileUserDataBase>("getdefaultfile(" + file.ToString() + ")");
		}

		private static void SetDefaultFile(ScriptExecutionContext executionContext, StandardFileType file, FileUserDataBase fileHandle)
		{
			SetDefaultFile(executionContext.GetScript(), file, fileHandle);
		}

		internal static void SetDefaultFile(Script script, StandardFileType file, FileUserDataBase fileHandle)
		{
			script.Registry.Set("853BEAAF298648839E2C99D005E1DF94_" + file, UserData.Create(fileHandle));
		}

		public static void SetDefaultFile(Script script, StandardFileType file, Stream stream)
		{
			if (file == StandardFileType.StdIn)
			{
				SetDefaultFile(script, file, StandardIOFileUserDataBase.CreateInputStream(stream));
			}
			else
			{
				SetDefaultFile(script, file, StandardIOFileUserDataBase.CreateOutputStream(stream));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue close(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return (args.AsUserData<FileUserDataBase>(0, "close", true) ?? GetDefaultFile(executionContext, StandardFileType.StdOut)).close(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue flush(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			(args.AsUserData<FileUserDataBase>(0, "close", true) ?? GetDefaultFile(executionContext, StandardFileType.StdOut)).flush();
			return DynValue.True;
		}

		[MoonSharpModuleMethod]
		public static DynValue input(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return HandleDefaultStreamSetter(executionContext, args, StandardFileType.StdIn);
		}

		[MoonSharpModuleMethod]
		public static DynValue output(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return HandleDefaultStreamSetter(executionContext, args, StandardFileType.StdOut);
		}

		private static DynValue HandleDefaultStreamSetter(ScriptExecutionContext executionContext, CallbackArguments args, StandardFileType defaultFiles)
		{
			if (args.Count == 0 || args[0].IsNil())
			{
				return UserData.Create(GetDefaultFile(executionContext, defaultFiles));
			}
			FileUserDataBase fileUserDataBase = null;
			if (args[0].Type == DataType.String || args[0].Type == DataType.Number)
			{
				string filename = args[0].CastToString();
				fileUserDataBase = Open(executionContext, filename, GetUTF8Encoding(), (defaultFiles == StandardFileType.StdIn) ? "r" : "w");
			}
			else
			{
				fileUserDataBase = args.AsUserData<FileUserDataBase>(0, (defaultFiles == StandardFileType.StdIn) ? "input" : "output");
			}
			SetDefaultFile(executionContext, defaultFiles, fileUserDataBase);
			return UserData.Create(fileUserDataBase);
		}

		private static Encoding GetUTF8Encoding()
		{
			return new UTF8Encoding(false);
		}

		[MoonSharpModuleMethod]
		public static DynValue lines(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string @string = args.AsType(0, "lines", DataType.String).String;
			try
			{
				List<DynValue> list = new List<DynValue>();
				using (Stream stream = Script.GlobalOptions.Platform.IO_OpenFile(executionContext.GetScript(), @string, null, "r"))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						while (!streamReader.EndOfStream)
						{
							string str = streamReader.ReadLine();
							list.Add(DynValue.NewString(str));
						}
					}
				}
				list.Add(DynValue.Nil);
				return DynValue.FromObject(executionContext.GetScript(), list.Select(_003C_003Ec._003C_003E9__14_0 ?? (_003C_003Ec._003C_003E9__14_0 = _003C_003Ec._003C_003E9._003Clines_003Eb__14_0)));
			}
			catch (Exception ex)
			{
				throw new ScriptRuntimeException(IoExceptionToLuaMessage(ex, @string));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue open(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string @string = args.AsType(0, "open", DataType.String).String;
			DynValue dynValue = args.AsType(1, "open", DataType.String, true);
			DynValue dynValue2 = args.AsType(2, "open", DataType.String, true);
			string text = (dynValue.IsNil() ? "r" : dynValue.String);
			if (text.Replace("+", "").Replace("r", "").Replace("a", "")
				.Replace("w", "")
				.Replace("b", "")
				.Replace("t", "")
				.Length > 0)
			{
				throw ScriptRuntimeException.BadArgument(1, "open", "invalid mode");
			}
			try
			{
				string text2 = (dynValue2.IsNil() ? null : dynValue2.String);
				Encoding encoding = null;
				bool flag = Framework.Do.StringContainsChar(text, 'b');
				if (text2 == "binary")
				{
					flag = true;
					encoding = new BinaryEncoding();
				}
				else if (text2 == null)
				{
					encoding = (flag ? new BinaryEncoding() : GetUTF8Encoding());
				}
				else
				{
					if (flag)
					{
						throw new ScriptRuntimeException("Can't specify encodings other than nil or 'binary' for binary streams.");
					}
					encoding = Encoding.GetEncoding(text2);
				}
				return UserData.Create(Open(executionContext, @string, encoding, text));
			}
			catch (Exception ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(IoExceptionToLuaMessage(ex, @string)));
			}
		}

		public static string IoExceptionToLuaMessage(Exception ex, string filename)
		{
			if (ex is FileNotFoundException)
			{
				return string.Format("{0}: No such file or directory", filename);
			}
			return ex.Message;
		}

		[MoonSharpModuleMethod]
		public static DynValue type(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.UserData)
			{
				return DynValue.Nil;
			}
			FileUserDataBase fileUserDataBase = args[0].UserData.Object as FileUserDataBase;
			if (fileUserDataBase == null)
			{
				return DynValue.Nil;
			}
			if (fileUserDataBase.isopen())
			{
				return DynValue.NewString("file");
			}
			return DynValue.NewString("closed file");
		}

		[MoonSharpModuleMethod]
		public static DynValue read(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return GetDefaultFile(executionContext, StandardFileType.StdIn).read(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue write(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return GetDefaultFile(executionContext, StandardFileType.StdOut).write(executionContext, args);
		}

		[MoonSharpModuleMethod]
		public static DynValue tmpfile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			string filename = Script.GlobalOptions.Platform.IO_OS_GetTempFilename();
			return UserData.Create(Open(executionContext, filename, GetUTF8Encoding(), "w"));
		}

		private static FileUserDataBase Open(ScriptExecutionContext executionContext, string filename, Encoding encoding, string mode)
		{
			return new FileUserData(executionContext.GetScript(), filename, encoding, mode);
		}
	}
}
