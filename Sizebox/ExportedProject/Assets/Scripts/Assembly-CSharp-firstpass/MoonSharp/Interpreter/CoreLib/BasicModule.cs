using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule]
	public class BasicModule
	{
		[MoonSharpModuleMethod]
		public static DynValue type(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args.Count < 1)
			{
				throw ScriptRuntimeException.BadArgumentValueExpected(0, "type");
			}
			return DynValue.NewString(args[0].Type.ToLuaTypeString());
		}

		[MoonSharpModuleMethod]
		public static DynValue assert(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0];
			DynValue dynValue2 = args[1];
			if (!dynValue.CastToBool())
			{
				if (dynValue2.IsNil())
				{
					throw new ScriptRuntimeException("assertion failed!");
				}
				throw new ScriptRuntimeException(dynValue2.ToPrintString());
			}
			return DynValue.NewTupleNested(args.GetArray());
		}

		[MoonSharpModuleMethod]
		public static DynValue collectgarbage(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			switch (args[0].CastToString())
			{
			case null:
			case "collect":
			case "restart":
				GC.Collect(2, GCCollectionMode.Forced);
				break;
			}
			return DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue error(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			throw new ScriptRuntimeException(args.AsType(0, "error", DataType.String).String);
		}

		[MoonSharpModuleMethod]
		public static DynValue tostring(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args.Count < 1)
			{
				throw ScriptRuntimeException.BadArgumentValueExpected(0, "tostring");
			}
			DynValue dynValue = args[0];
			DynValue metamethodTailCall = executionContext.GetMetamethodTailCall(dynValue, "__tostring", dynValue);
			if (metamethodTailCall == null || metamethodTailCall.IsNil())
			{
				return DynValue.NewString(dynValue.ToPrintString());
			}
			metamethodTailCall.TailCallData.Continuation = new CallbackFunction(__tostring_continuation, "__tostring");
			return metamethodTailCall;
		}

		private static DynValue __tostring_continuation(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args[0].ToScalar();
			if (dynValue.IsNil())
			{
				return dynValue;
			}
			if (dynValue.Type != DataType.String)
			{
				throw new ScriptRuntimeException("'tostring' must return a string");
			}
			return dynValue;
		}

		[MoonSharpModuleMethod]
		public static DynValue select(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type == DataType.String && args[0].String == "#")
			{
				if (args[args.Count - 1].Type == DataType.Tuple)
				{
					return DynValue.NewNumber(args.Count - 1 + args[args.Count - 1].Tuple.Length);
				}
				return DynValue.NewNumber(args.Count - 1);
			}
			int num = (int)args.AsType(0, "select", DataType.Number).Number;
			List<DynValue> list = new List<DynValue>();
			if (num > 0)
			{
				for (int i = num; i < args.Count; i++)
				{
					list.Add(args[i]);
				}
			}
			else
			{
				if (num >= 0)
				{
					throw ScriptRuntimeException.BadArgumentIndexOutOfRange("select", 0);
				}
				num = args.Count + num;
				if (num < 1)
				{
					throw ScriptRuntimeException.BadArgumentIndexOutOfRange("select", 0);
				}
				for (int j = num; j < args.Count; j++)
				{
					list.Add(args[j]);
				}
			}
			return DynValue.NewTupleNested(list.ToArray());
		}

		[MoonSharpModuleMethod]
		public static DynValue tonumber(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args.Count < 1)
			{
				throw ScriptRuntimeException.BadArgumentValueExpected(0, "tonumber");
			}
			DynValue dynValue = args[0];
			DynValue dynValue2 = args.AsType(1, "tonumber", DataType.Number, true);
			if (dynValue2.IsNil())
			{
				if (dynValue.Type == DataType.Number)
				{
					return dynValue;
				}
				if (dynValue.Type != DataType.String)
				{
					return DynValue.Nil;
				}
				double result;
				if (double.TryParse(dynValue.String, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					return DynValue.NewNumber(result);
				}
				return DynValue.Nil;
			}
			DynValue dynValue3 = ((args[0].Type == DataType.Number) ? DynValue.NewString(args[0].Number.ToString(CultureInfo.InvariantCulture)) : args.AsType(0, "tonumber", DataType.String));
			int num = (int)dynValue2.Number;
			uint num2 = 0u;
			switch (num)
			{
			case 2:
			case 8:
			case 10:
			case 16:
				num2 = Convert.ToUInt32(dynValue3.String.Trim(), num);
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 9:
			{
				string text = dynValue3.String.Trim();
				for (int i = 0; i < text.Length; i++)
				{
					int num3 = text[i] - 48;
					if (num3 < 0 || num3 >= num)
					{
						throw new ScriptRuntimeException("bad argument #1 to 'tonumber' (invalid character)");
					}
					num2 = (uint)((int)(num2 * num) + num3);
				}
				break;
			}
			default:
				throw new ScriptRuntimeException("bad argument #2 to 'tonumber' (base out of range)");
			}
			return DynValue.NewNumber(num2);
		}

		[MoonSharpModuleMethod]
		public static DynValue print(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Count && !args[i].IsVoid(); i++)
			{
				if (i != 0)
				{
					stringBuilder.Append('\t');
				}
				stringBuilder.Append(args.AsStringUsingMeta(executionContext, i, "print"));
			}
			executionContext.GetScript().Options.DebugPrint(stringBuilder.ToString());
			return DynValue.Nil;
		}
	}
}
