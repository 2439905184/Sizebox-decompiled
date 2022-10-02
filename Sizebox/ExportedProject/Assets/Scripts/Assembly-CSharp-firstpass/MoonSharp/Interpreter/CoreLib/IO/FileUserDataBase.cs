using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	internal abstract class FileUserDataBase : RefIdObject
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<DynValue, DynValue> _003C_003E9__0_0;

			internal DynValue _003Clines_003Eb__0_0(DynValue s)
			{
				return s;
			}
		}

		public DynValue lines(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			List<DynValue> list = new List<DynValue>();
			DynValue dynValue = null;
			do
			{
				dynValue = read(executionContext, args);
				list.Add(dynValue);
			}
			while (dynValue.IsNotNil());
			return DynValue.FromObject(executionContext.GetScript(), list.Select(_003C_003Ec._003C_003E9__0_0 ?? (_003C_003Ec._003C_003E9__0_0 = _003C_003Ec._003C_003E9._003Clines_003Eb__0_0)));
		}

		public DynValue read(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args.Count == 0)
			{
				string text = ReadLine();
				if (text == null)
				{
					return DynValue.Nil;
				}
				text = text.TrimEnd('\n', '\r');
				return DynValue.NewString(text);
			}
			List<DynValue> list = new List<DynValue>();
			for (int i = 0; i < args.Count; i++)
			{
				DynValue item;
				if (args[i].Type == DataType.Number)
				{
					if (Eof())
					{
						return DynValue.Nil;
					}
					int p = (int)args[i].Number;
					item = DynValue.NewString(ReadBuffer(p));
				}
				else
				{
					string @string = args.AsType(i, "read", DataType.String).String;
					if (Eof())
					{
						item = (@string.StartsWith("*a") ? DynValue.NewString("") : DynValue.Nil);
					}
					else if (@string.StartsWith("*n"))
					{
						double? num = ReadNumber();
						item = ((!num.HasValue) ? DynValue.Nil : DynValue.NewNumber(num.Value));
					}
					else if (@string.StartsWith("*a"))
					{
						item = DynValue.NewString(ReadToEnd());
					}
					else if (@string.StartsWith("*l"))
					{
						item = DynValue.NewString(ReadLine().TrimEnd('\n', '\r'));
					}
					else
					{
						if (!@string.StartsWith("*L"))
						{
							throw ScriptRuntimeException.BadArgument(i, "read", "invalid option");
						}
						item = DynValue.NewString(ReadLine().TrimEnd('\n', '\r') + "\n");
					}
				}
				list.Add(item);
			}
			return DynValue.NewTuple(list.ToArray());
		}

		public DynValue write(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				for (int i = 0; i < args.Count; i++)
				{
					string @string = args.AsType(i, "write", DataType.String).String;
					Write(@string);
				}
				return UserData.Create(this);
			}
			catch (ScriptRuntimeException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex2.Message));
			}
		}

		public DynValue close(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				string text = Close();
				if (text == null)
				{
					return DynValue.True;
				}
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(text));
			}
			catch (ScriptRuntimeException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex2.Message));
			}
		}

		private double? ReadNumber()
		{
			string text = "";
			while (!Eof())
			{
				char c = Peek();
				if (char.IsWhiteSpace(c))
				{
					ReadBuffer(1);
					continue;
				}
				if (!IsNumericChar(c, text))
				{
					break;
				}
				ReadBuffer(1);
				text += c;
			}
			double result;
			if (double.TryParse(text, out result))
			{
				return result;
			}
			return null;
		}

		private bool IsNumericChar(char c, string numAsFar)
		{
			if (char.IsDigit(c))
			{
				return true;
			}
			switch (c)
			{
			case '-':
				return numAsFar.Length == 0;
			case '.':
				return !Framework.Do.StringContainsChar(numAsFar, '.');
			case 'E':
			case 'e':
				if (!Framework.Do.StringContainsChar(numAsFar, 'E'))
				{
					return !Framework.Do.StringContainsChar(numAsFar, 'e');
				}
				return false;
			default:
				return false;
			}
		}

		protected abstract bool Eof();

		protected abstract string ReadLine();

		protected abstract string ReadBuffer(int p);

		protected abstract string ReadToEnd();

		protected abstract char Peek();

		protected abstract void Write(string value);

		protected internal abstract bool isopen();

		protected abstract string Close();

		public abstract bool flush();

		public abstract long seek(string whence, long offset);

		public abstract bool setvbuf(string mode);

		public override string ToString()
		{
			if (isopen())
			{
				return string.Format("file ({0:X8})", base.ReferenceID);
			}
			return "file (closed)";
		}
	}
}
