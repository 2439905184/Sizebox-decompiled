using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MoonSharp.Interpreter
{
	public static class LinqHelpers
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass0_0<T>
		{
			public DataType type;

			internal bool _003CConvert_003Eb__0(DynValue v)
			{
				return v.Type == type;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec__0<T>
		{
			public static readonly _003C_003Ec__0<T> _003C_003E9 = new _003C_003Ec__0<T>();

			public static Func<DynValue, T> _003C_003E9__0_1;

			internal T _003CConvert_003Eb__0_1(DynValue v)
			{
				return v.ToObject<T>();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0
		{
			public DataType type;

			internal bool _003COfDataType_003Eb__0(DynValue v)
			{
				return v.Type == type;
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<DynValue, object> _003C_003E9__2_0;

			internal object _003CAsObjects_003Eb__2_0(DynValue v)
			{
				return v.ToObject();
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec__3<T>
		{
			public static readonly _003C_003Ec__3<T> _003C_003E9 = new _003C_003Ec__3<T>();

			public static Func<DynValue, T> _003C_003E9__3_0;

			internal T _003CAsObjects_003Eb__3_0(DynValue v)
			{
				return v.ToObject<T>();
			}
		}

		public static IEnumerable<T> Convert<T>(this IEnumerable<DynValue> enumerable, DataType type)
		{
			_003C_003Ec__DisplayClass0_0<T> _003C_003Ec__DisplayClass0_ = new _003C_003Ec__DisplayClass0_0<T>();
			_003C_003Ec__DisplayClass0_.type = type;
			return enumerable.Where(_003C_003Ec__DisplayClass0_._003CConvert_003Eb__0).Select(_003C_003Ec__0<T>._003C_003E9__0_1 ?? (_003C_003Ec__0<T>._003C_003E9__0_1 = _003C_003Ec__0<T>._003C_003E9._003CConvert_003Eb__0_1));
		}

		public static IEnumerable<DynValue> OfDataType(this IEnumerable<DynValue> enumerable, DataType type)
		{
			_003C_003Ec__DisplayClass1_0 _003C_003Ec__DisplayClass1_ = new _003C_003Ec__DisplayClass1_0();
			_003C_003Ec__DisplayClass1_.type = type;
			return enumerable.Where(_003C_003Ec__DisplayClass1_._003COfDataType_003Eb__0);
		}

		public static IEnumerable<object> AsObjects(this IEnumerable<DynValue> enumerable)
		{
			return enumerable.Select(_003C_003Ec._003C_003E9__2_0 ?? (_003C_003Ec._003C_003E9__2_0 = _003C_003Ec._003C_003E9._003CAsObjects_003Eb__2_0));
		}

		public static IEnumerable<T> AsObjects<T>(this IEnumerable<DynValue> enumerable)
		{
			return enumerable.Select(_003C_003Ec__3<T>._003C_003E9__3_0 ?? (_003C_003Ec__3<T>._003C_003E9__3_0 = _003C_003Ec__3<T>._003C_003E9._003CAsObjects_003Eb__3_0));
		}
	}
}
