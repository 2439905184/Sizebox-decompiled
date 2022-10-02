using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "table")]
	public class TableModule
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{
			public ScriptExecutionContext executionContext;

			public DynValue lt;

			internal int _003Csort_003Eb__0(DynValue a, DynValue b)
			{
				return SortComparer(executionContext, a, b, lt);
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue unpack(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "unpack", DataType.Table);
			DynValue dynValue2 = args.AsType(1, "unpack", DataType.Number, true);
			DynValue dynValue3 = args.AsType(2, "unpack", DataType.Number, true);
			int num = (dynValue2.IsNil() ? 1 : ((int)dynValue2.Number));
			int num2 = (dynValue3.IsNil() ? GetTableLength(executionContext, dynValue) : ((int)dynValue3.Number));
			Table table = dynValue.Table;
			DynValue[] array = new DynValue[num2 - num + 1];
			int num3 = 0;
			for (int i = num; i <= num2; i++)
			{
				array[num3++] = table.Get(i);
			}
			return DynValue.NewTuple(array);
		}

		[MoonSharpModuleMethod]
		public static DynValue pack(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Table table = new Table(executionContext.GetScript());
			DynValue result = DynValue.NewTable(table);
			for (int i = 0; i < args.Count; i++)
			{
				table.Set(i + 1, args[i]);
			}
			table.Set("n", DynValue.NewNumber(args.Count));
			return result;
		}

		[MoonSharpModuleMethod]
		public static DynValue sort(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
			_003C_003Ec__DisplayClass2_.executionContext = executionContext;
			DynValue dynValue = args.AsType(0, "sort", DataType.Table);
			_003C_003Ec__DisplayClass2_.lt = args[1];
			if (_003C_003Ec__DisplayClass2_.lt.Type != DataType.Function && _003C_003Ec__DisplayClass2_.lt.Type != DataType.ClrFunction && _003C_003Ec__DisplayClass2_.lt.IsNotNil())
			{
				args.AsType(1, "sort", DataType.Function, true);
			}
			int tableLength = GetTableLength(_003C_003Ec__DisplayClass2_.executionContext, dynValue);
			List<DynValue> list = new List<DynValue>();
			for (int i = 1; i <= tableLength; i++)
			{
				list.Add(dynValue.Table.Get(i));
			}
			try
			{
				list.Sort(_003C_003Ec__DisplayClass2_._003Csort_003Eb__0);
			}
			catch (InvalidOperationException ex)
			{
				if (ex.InnerException is ScriptRuntimeException)
				{
					throw ex.InnerException;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				dynValue.Table.Set(j + 1, list[j]);
			}
			return dynValue;
		}

		private static int SortComparer(ScriptExecutionContext executionContext, DynValue a, DynValue b, DynValue lt)
		{
			if (lt == null || lt.IsNil())
			{
				lt = executionContext.GetBinaryMetamethod(a, b, "__lt");
				if (lt == null || lt.IsNil())
				{
					if (a.Type == DataType.Number && b.Type == DataType.Number)
					{
						return a.Number.CompareTo(b.Number);
					}
					if (a.Type == DataType.String && b.Type == DataType.String)
					{
						return a.String.CompareTo(b.String);
					}
					throw ScriptRuntimeException.CompareInvalidType(a, b);
				}
				return LuaComparerToClrComparer(executionContext.GetScript().Call(lt, a, b), executionContext.GetScript().Call(lt, b, a));
			}
			return LuaComparerToClrComparer(executionContext.GetScript().Call(lt, a, b), executionContext.GetScript().Call(lt, b, a));
		}

		private static int LuaComparerToClrComparer(DynValue dynValue1, DynValue dynValue2)
		{
			bool flag = dynValue1.CastToBool();
			bool flag2 = dynValue2.CastToBool();
			if (flag && !flag2)
			{
				return -1;
			}
			if (flag2 && !flag)
			{
				return 1;
			}
			if (flag || flag2)
			{
				throw new ScriptRuntimeException("invalid order function for sorting");
			}
			return 0;
		}

		[MoonSharpModuleMethod]
		public static DynValue insert(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "table.insert", DataType.Table);
			DynValue dynValue2 = args[1];
			DynValue dynValue3 = args[2];
			if (args.Count > 3)
			{
				throw new ScriptRuntimeException("wrong number of arguments to 'insert'");
			}
			int tableLength = GetTableLength(executionContext, dynValue);
			Table table = dynValue.Table;
			if (dynValue3.IsNil())
			{
				dynValue3 = dynValue2;
				dynValue2 = DynValue.NewNumber(tableLength + 1);
			}
			if (dynValue2.Type != DataType.Number)
			{
				throw ScriptRuntimeException.BadArgument(1, "table.insert", DataType.Number, dynValue2.Type, false);
			}
			int num = (int)dynValue2.Number;
			if (num > tableLength + 1 || num < 1)
			{
				throw new ScriptRuntimeException("bad argument #2 to 'insert' (position out of bounds)");
			}
			for (int num2 = tableLength; num2 >= num; num2--)
			{
				table.Set(num2 + 1, table.Get(num2));
			}
			table.Set(num, dynValue3);
			return dynValue;
		}

		[MoonSharpModuleMethod]
		public static DynValue remove(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "table.remove", DataType.Table);
			DynValue dynValue2 = args.AsType(1, "table.remove", DataType.Number, true);
			DynValue result = DynValue.Nil;
			if (args.Count > 2)
			{
				throw new ScriptRuntimeException("wrong number of arguments to 'remove'");
			}
			int tableLength = GetTableLength(executionContext, dynValue);
			Table table = dynValue.Table;
			int num = (dynValue2.IsNil() ? tableLength : ((int)dynValue2.Number));
			if (num >= tableLength + 1 || (num < 1 && tableLength > 0))
			{
				throw new ScriptRuntimeException("bad argument #1 to 'remove' (position out of bounds)");
			}
			for (int i = num; i <= tableLength; i++)
			{
				if (i == num)
				{
					result = table.Get(i);
				}
				table.Set(i, table.Get(i + 1));
			}
			return result;
		}

		[MoonSharpModuleMethod]
		public static DynValue concat(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "concat", DataType.Table);
			DynValue dynValue2 = args.AsType(1, "concat", DataType.String, true);
			DynValue dynValue3 = args.AsType(2, "concat", DataType.Number, true);
			DynValue dynValue4 = args.AsType(3, "concat", DataType.Number, true);
			Table table = dynValue.Table;
			string value = (dynValue2.IsNil() ? "" : dynValue2.String);
			int num = (dynValue3.IsNilOrNan() ? 1 : ((int)dynValue3.Number));
			int num2 = ((!dynValue4.IsNilOrNan()) ? ((int)dynValue4.Number) : GetTableLength(executionContext, dynValue));
			if (num2 < num)
			{
				return DynValue.NewString(string.Empty);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = num; i <= num2; i++)
			{
				DynValue dynValue5 = table.Get(i);
				if (dynValue5.Type != DataType.Number && dynValue5.Type != DataType.String)
				{
					throw new ScriptRuntimeException("invalid value ({1}) at index {0} in table for 'concat'", i, dynValue5.Type.ToLuaTypeString());
				}
				string value2 = dynValue5.ToPrintString();
				if (i != num)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Append(value2);
			}
			return DynValue.NewString(stringBuilder.ToString());
		}

		private static int GetTableLength(ScriptExecutionContext executionContext, DynValue vlist)
		{
			DynValue metamethod = executionContext.GetMetamethod(vlist, "__len");
			if (metamethod != null)
			{
				double? num = executionContext.GetScript().Call(metamethod, vlist).CastToNumber();
				if (!num.HasValue)
				{
					throw new ScriptRuntimeException("object length is not a number");
				}
				return (int)num.Value;
			}
			return vlist.Table.Length;
		}
	}
}
