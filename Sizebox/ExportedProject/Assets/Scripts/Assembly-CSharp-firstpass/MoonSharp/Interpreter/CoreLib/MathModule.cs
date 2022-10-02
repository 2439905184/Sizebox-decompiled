using System;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Interop;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "math")]
	public class MathModule
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<double, double> _003C_003E9__9_0;

			public static Func<double, double> _003C_003E9__10_0;

			public static Func<double, double> _003C_003E9__11_0;

			public static Func<double, double> _003C_003E9__12_0;

			public static Func<double, double, double> _003C_003E9__13_0;

			public static Func<double, double> _003C_003E9__14_0;

			public static Func<double, double> _003C_003E9__15_0;

			public static Func<double, double> _003C_003E9__16_0;

			public static Func<double, double> _003C_003E9__17_0;

			public static Func<double, double> _003C_003E9__18_0;

			public static Func<double, double> _003C_003E9__19_0;

			public static Func<double, double, double> _003C_003E9__20_0;

			public static Func<double, double, double> _003C_003E9__22_0;

			public static Func<double, double, double> _003C_003E9__23_0;

			public static Func<double, double, double> _003C_003E9__24_0;

			public static Func<double, double, double> _003C_003E9__25_0;

			public static Func<double, double, double> _003C_003E9__27_0;

			public static Func<double, double> _003C_003E9__28_0;

			public static Func<double, double> _003C_003E9__31_0;

			public static Func<double, double> _003C_003E9__32_0;

			public static Func<double, double> _003C_003E9__33_0;

			public static Func<double, double> _003C_003E9__34_0;

			public static Func<double, double> _003C_003E9__35_0;

			internal double _003Cabs_003Eb__9_0(double d)
			{
				return Math.Abs(d);
			}

			internal double _003Cacos_003Eb__10_0(double d)
			{
				return Math.Acos(d);
			}

			internal double _003Casin_003Eb__11_0(double d)
			{
				return Math.Asin(d);
			}

			internal double _003Catan_003Eb__12_0(double d)
			{
				return Math.Atan(d);
			}

			internal double _003Catan2_003Eb__13_0(double d1, double d2)
			{
				return Math.Atan2(d1, d2);
			}

			internal double _003Cceil_003Eb__14_0(double d)
			{
				return Math.Ceiling(d);
			}

			internal double _003Ccos_003Eb__15_0(double d)
			{
				return Math.Cos(d);
			}

			internal double _003Ccosh_003Eb__16_0(double d)
			{
				return Math.Cosh(d);
			}

			internal double _003Cdeg_003Eb__17_0(double d)
			{
				return d * 180.0 / Math.PI;
			}

			internal double _003Cexp_003Eb__18_0(double d)
			{
				return Math.Exp(d);
			}

			internal double _003Cfloor_003Eb__19_0(double d)
			{
				return Math.Floor(d);
			}

			internal double _003Cfmod_003Eb__20_0(double d1, double d2)
			{
				return Math.IEEERemainder(d1, d2);
			}

			internal double _003Cldexp_003Eb__22_0(double d1, double d2)
			{
				return d1 * Math.Pow(2.0, d2);
			}

			internal double _003Clog_003Eb__23_0(double d1, double d2)
			{
				return Math.Log(d1, d2);
			}

			internal double _003Cmax_003Eb__24_0(double d1, double d2)
			{
				return Math.Max(d1, d2);
			}

			internal double _003Cmin_003Eb__25_0(double d1, double d2)
			{
				return Math.Min(d1, d2);
			}

			internal double _003Cpow_003Eb__27_0(double d1, double d2)
			{
				return Math.Pow(d1, d2);
			}

			internal double _003Crad_003Eb__28_0(double d)
			{
				return d * Math.PI / 180.0;
			}

			internal double _003Csin_003Eb__31_0(double d)
			{
				return Math.Sin(d);
			}

			internal double _003Csinh_003Eb__32_0(double d)
			{
				return Math.Sinh(d);
			}

			internal double _003Csqrt_003Eb__33_0(double d)
			{
				return Math.Sqrt(d);
			}

			internal double _003Ctan_003Eb__34_0(double d)
			{
				return Math.Tan(d);
			}

			internal double _003Ctanh_003Eb__35_0(double d)
			{
				return Math.Tanh(d);
			}
		}

		[MoonSharpModuleConstant]
		public const double pi = Math.PI;

		[MoonSharpModuleConstant]
		public const double huge = double.MaxValue;

		private static Random GetRandom(Script s)
		{
			return (s.Registry.Get("F61E3AA7247D4D1EB7A45430B0C8C9BB_MATH_RANDOM").UserData.Object as AnonWrapper<Random>).Value;
		}

		private static void SetRandom(Script s, Random random)
		{
			DynValue value = UserData.Create(new AnonWrapper<Random>(random));
			s.Registry.Set("F61E3AA7247D4D1EB7A45430B0C8C9BB_MATH_RANDOM", value);
		}

		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			SetRandom(globalTable.OwnerScript, new Random());
		}

		private static DynValue exec1(CallbackArguments args, string funcName, Func<double, double> func)
		{
			DynValue dynValue = args.AsType(0, funcName, DataType.Number);
			return DynValue.NewNumber(func(dynValue.Number));
		}

		private static DynValue exec2(CallbackArguments args, string funcName, Func<double, double, double> func)
		{
			DynValue dynValue = args.AsType(0, funcName, DataType.Number);
			DynValue dynValue2 = args.AsType(1, funcName, DataType.Number);
			return DynValue.NewNumber(func(dynValue.Number, dynValue2.Number));
		}

		private static DynValue exec2n(CallbackArguments args, string funcName, double defVal, Func<double, double, double> func)
		{
			DynValue dynValue = args.AsType(0, funcName, DataType.Number);
			DynValue dynValue2 = args.AsType(1, funcName, DataType.Number, true);
			return DynValue.NewNumber(func(dynValue.Number, dynValue2.IsNil() ? defVal : dynValue2.Number));
		}

		private static DynValue execaccum(CallbackArguments args, string funcName, Func<double, double, double> func)
		{
			double num = double.NaN;
			if (args.Count == 0)
			{
				throw new ScriptRuntimeException("bad argument #1 to '{0}' (number expected, got no value)", funcName);
			}
			for (int i = 0; i < args.Count; i++)
			{
				DynValue dynValue = args.AsType(i, funcName, DataType.Number);
				num = ((i != 0) ? func(num, dynValue.Number) : dynValue.Number);
			}
			return DynValue.NewNumber(num);
		}

		[MoonSharpModuleMethod]
		public static DynValue abs(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "abs", _003C_003Ec._003C_003E9__9_0 ?? (_003C_003Ec._003C_003E9__9_0 = _003C_003Ec._003C_003E9._003Cabs_003Eb__9_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue acos(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "acos", _003C_003Ec._003C_003E9__10_0 ?? (_003C_003Ec._003C_003E9__10_0 = _003C_003Ec._003C_003E9._003Cacos_003Eb__10_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue asin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "asin", _003C_003Ec._003C_003E9__11_0 ?? (_003C_003Ec._003C_003E9__11_0 = _003C_003Ec._003C_003E9._003Casin_003Eb__11_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue atan(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "atan", _003C_003Ec._003C_003E9__12_0 ?? (_003C_003Ec._003C_003E9__12_0 = _003C_003Ec._003C_003E9._003Catan_003Eb__12_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue atan2(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "atan2", _003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003Catan2_003Eb__13_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue ceil(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "ceil", _003C_003Ec._003C_003E9__14_0 ?? (_003C_003Ec._003C_003E9__14_0 = _003C_003Ec._003C_003E9._003Cceil_003Eb__14_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue cos(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "cos", _003C_003Ec._003C_003E9__15_0 ?? (_003C_003Ec._003C_003E9__15_0 = _003C_003Ec._003C_003E9._003Ccos_003Eb__15_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue cosh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "cosh", _003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003Ccosh_003Eb__16_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue deg(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "deg", _003C_003Ec._003C_003E9__17_0 ?? (_003C_003Ec._003C_003E9__17_0 = _003C_003Ec._003C_003E9._003Cdeg_003Eb__17_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue exp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "exp", _003C_003Ec._003C_003E9__18_0 ?? (_003C_003Ec._003C_003E9__18_0 = _003C_003Ec._003C_003E9._003Cexp_003Eb__18_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue floor(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "floor", _003C_003Ec._003C_003E9__19_0 ?? (_003C_003Ec._003C_003E9__19_0 = _003C_003Ec._003C_003E9._003Cfloor_003Eb__19_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue fmod(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "fmod", _003C_003Ec._003C_003E9__20_0 ?? (_003C_003Ec._003C_003E9__20_0 = _003C_003Ec._003C_003E9._003Cfmod_003Eb__20_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue frexp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			long num = BitConverter.DoubleToInt64Bits(args.AsType(0, "frexp", DataType.Number).Number);
			bool flag = num < 0;
			int num2 = (int)((num >> 52) & 0x7FF);
			long num3 = num & 0xFFFFFFFFFFFFFL;
			if (num2 == 0)
			{
				num2++;
			}
			else
			{
				num3 |= 0x10000000000000L;
			}
			num2 -= 1075;
			if (num3 == 0L)
			{
				return DynValue.NewTuple(DynValue.NewNumber(0.0), DynValue.NewNumber(0.0));
			}
			while ((num3 & 1) == 0L)
			{
				num3 >>= 1;
				num2++;
			}
			double num4 = num3;
			double num5 = num2;
			while (num4 >= 1.0)
			{
				num4 /= 2.0;
				num5 += 1.0;
			}
			if (flag)
			{
				num4 = 0.0 - num4;
			}
			return DynValue.NewTuple(DynValue.NewNumber(num4), DynValue.NewNumber(num5));
		}

		[MoonSharpModuleMethod]
		public static DynValue ldexp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "ldexp", _003C_003Ec._003C_003E9__22_0 ?? (_003C_003Ec._003C_003E9__22_0 = _003C_003Ec._003C_003E9._003Cldexp_003Eb__22_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue log(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2n(args, "log", Math.E, _003C_003Ec._003C_003E9__23_0 ?? (_003C_003Ec._003C_003E9__23_0 = _003C_003Ec._003C_003E9._003Clog_003Eb__23_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue max(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return execaccum(args, "max", _003C_003Ec._003C_003E9__24_0 ?? (_003C_003Ec._003C_003E9__24_0 = _003C_003Ec._003C_003E9._003Cmax_003Eb__24_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue min(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return execaccum(args, "min", _003C_003Ec._003C_003E9__25_0 ?? (_003C_003Ec._003C_003E9__25_0 = _003C_003Ec._003C_003E9._003Cmin_003Eb__25_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue modf(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "modf", DataType.Number);
			return DynValue.NewTuple(DynValue.NewNumber(Math.Floor(dynValue.Number)), DynValue.NewNumber(dynValue.Number - Math.Floor(dynValue.Number)));
		}

		[MoonSharpModuleMethod]
		public static DynValue pow(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "pow", _003C_003Ec._003C_003E9__27_0 ?? (_003C_003Ec._003C_003E9__27_0 = _003C_003Ec._003C_003E9._003Cpow_003Eb__27_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue rad(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "rad", _003C_003Ec._003C_003E9__28_0 ?? (_003C_003Ec._003C_003E9__28_0 = _003C_003Ec._003C_003E9._003Crad_003Eb__28_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue random(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "random", DataType.Number, true);
			DynValue dynValue2 = args.AsType(1, "random", DataType.Number, true);
			Random random = GetRandom(executionContext.GetScript());
			double num;
			if (dynValue.IsNil() && dynValue2.IsNil())
			{
				num = random.NextDouble();
			}
			else
			{
				int num2 = (dynValue2.IsNil() ? 1 : ((int)dynValue2.Number));
				int num3 = (int)dynValue.Number;
				num = ((num2 >= num3) ? ((double)random.Next(num3, num2 + 1)) : ((double)random.Next(num2, num3 + 1)));
			}
			return DynValue.NewNumber(num);
		}

		[MoonSharpModuleMethod]
		public static DynValue randomseed(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue dynValue = args.AsType(0, "randomseed", DataType.Number);
			SetRandom(executionContext.GetScript(), new Random((int)dynValue.Number));
			return DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue sin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sin", _003C_003Ec._003C_003E9__31_0 ?? (_003C_003Ec._003C_003E9__31_0 = _003C_003Ec._003C_003E9._003Csin_003Eb__31_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue sinh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sinh", _003C_003Ec._003C_003E9__32_0 ?? (_003C_003Ec._003C_003E9__32_0 = _003C_003Ec._003C_003E9._003Csinh_003Eb__32_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue sqrt(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sqrt", _003C_003Ec._003C_003E9__33_0 ?? (_003C_003Ec._003C_003E9__33_0 = _003C_003Ec._003C_003E9._003Csqrt_003Eb__33_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue tan(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "tan", _003C_003Ec._003C_003E9__34_0 ?? (_003C_003Ec._003C_003E9__34_0 = _003C_003Ec._003C_003E9._003Ctan_003Eb__34_0));
		}

		[MoonSharpModuleMethod]
		public static DynValue tanh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "tanh", _003C_003Ec._003C_003E9__35_0 ?? (_003C_003Ec._003C_003E9__35_0 = _003C_003Ec._003C_003E9._003Ctanh_003Eb__35_0));
		}
	}
}
