using System;
using System.Linq;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop
{
	public class StandardEnumUserDataDescriptor : DispatchingUserDataDescriptor
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<object, long> _003C_003E9__23_0;

			public static Func<long, object> _003C_003E9__23_1;

			public static Func<object, long> _003C_003E9__23_2;

			public static Func<long, object> _003C_003E9__23_3;

			public static Func<object, long> _003C_003E9__23_4;

			public static Func<long, object> _003C_003E9__23_5;

			public static Func<object, long> _003C_003E9__23_6;

			public static Func<long, object> _003C_003E9__23_7;

			public static Func<object, ulong> _003C_003E9__24_0;

			public static Func<ulong, object> _003C_003E9__24_1;

			public static Func<object, ulong> _003C_003E9__24_2;

			public static Func<ulong, object> _003C_003E9__24_3;

			public static Func<object, ulong> _003C_003E9__24_4;

			public static Func<ulong, object> _003C_003E9__24_5;

			public static Func<object, ulong> _003C_003E9__24_6;

			public static Func<ulong, object> _003C_003E9__24_7;

			public static Func<ulong, ulong, ulong> _003C_003E9__31_0;

			public static Func<long, long, long> _003C_003E9__31_1;

			public static Func<ulong, ulong, ulong> _003C_003E9__32_0;

			public static Func<long, long, long> _003C_003E9__32_1;

			public static Func<ulong, ulong, ulong> _003C_003E9__33_0;

			public static Func<long, long, long> _003C_003E9__33_1;

			public static Func<ulong, ulong> _003C_003E9__34_0;

			public static Func<long, long> _003C_003E9__34_1;

			public static Func<ulong, ulong, DynValue> _003C_003E9__35_0;

			public static Func<long, long, DynValue> _003C_003E9__35_1;

			public static Func<ulong, ulong, DynValue> _003C_003E9__36_0;

			public static Func<long, long, DynValue> _003C_003E9__36_1;

			internal long _003CCreateSignedConversionFunctions_003Eb__23_0(object o)
			{
				return (sbyte)o;
			}

			internal object _003CCreateSignedConversionFunctions_003Eb__23_1(long o)
			{
				return (sbyte)o;
			}

			internal long _003CCreateSignedConversionFunctions_003Eb__23_2(object o)
			{
				return (short)o;
			}

			internal object _003CCreateSignedConversionFunctions_003Eb__23_3(long o)
			{
				return (short)o;
			}

			internal long _003CCreateSignedConversionFunctions_003Eb__23_4(object o)
			{
				return (int)o;
			}

			internal object _003CCreateSignedConversionFunctions_003Eb__23_5(long o)
			{
				return (int)o;
			}

			internal long _003CCreateSignedConversionFunctions_003Eb__23_6(object o)
			{
				return (long)o;
			}

			internal object _003CCreateSignedConversionFunctions_003Eb__23_7(long o)
			{
				return o;
			}

			internal ulong _003CCreateUnsignedConversionFunctions_003Eb__24_0(object o)
			{
				return (byte)o;
			}

			internal object _003CCreateUnsignedConversionFunctions_003Eb__24_1(ulong o)
			{
				return (byte)o;
			}

			internal ulong _003CCreateUnsignedConversionFunctions_003Eb__24_2(object o)
			{
				return (ushort)o;
			}

			internal object _003CCreateUnsignedConversionFunctions_003Eb__24_3(ulong o)
			{
				return (ushort)o;
			}

			internal ulong _003CCreateUnsignedConversionFunctions_003Eb__24_4(object o)
			{
				return (uint)o;
			}

			internal object _003CCreateUnsignedConversionFunctions_003Eb__24_5(ulong o)
			{
				return (uint)o;
			}

			internal ulong _003CCreateUnsignedConversionFunctions_003Eb__24_6(object o)
			{
				return (ulong)o;
			}

			internal object _003CCreateUnsignedConversionFunctions_003Eb__24_7(ulong o)
			{
				return o;
			}

			internal ulong _003CCallback_Or_003Eb__31_0(ulong v1, ulong v2)
			{
				return v1 | v2;
			}

			internal long _003CCallback_Or_003Eb__31_1(long v1, long v2)
			{
				return v1 | v2;
			}

			internal ulong _003CCallback_And_003Eb__32_0(ulong v1, ulong v2)
			{
				return v1 & v2;
			}

			internal long _003CCallback_And_003Eb__32_1(long v1, long v2)
			{
				return v1 & v2;
			}

			internal ulong _003CCallback_Xor_003Eb__33_0(ulong v1, ulong v2)
			{
				return v1 ^ v2;
			}

			internal long _003CCallback_Xor_003Eb__33_1(long v1, long v2)
			{
				return v1 ^ v2;
			}

			internal ulong _003CCallback_BwNot_003Eb__34_0(ulong v1)
			{
				return ~v1;
			}

			internal long _003CCallback_BwNot_003Eb__34_1(long v1)
			{
				return ~v1;
			}

			internal DynValue _003CCallback_HasAll_003Eb__35_0(ulong v1, ulong v2)
			{
				return DynValue.NewBoolean((v1 & v2) == v2);
			}

			internal DynValue _003CCallback_HasAll_003Eb__35_1(long v1, long v2)
			{
				return DynValue.NewBoolean((v1 & v2) == v2);
			}

			internal DynValue _003CCallback_HasAny_003Eb__36_0(ulong v1, ulong v2)
			{
				return DynValue.NewBoolean((v1 & v2) != 0);
			}

			internal DynValue _003CCallback_HasAny_003Eb__36_1(long v1, long v2)
			{
				return DynValue.NewBoolean((v1 & v2) != 0);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public StandardEnumUserDataDescriptor _003C_003E4__this;

			public Func<long, long, long> operation;

			internal DynValue _003CPerformBinaryOperationS_003Eb__0(long v1, long v2)
			{
				return _003C_003E4__this.CreateValueSigned(operation(v1, v2));
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass28_0
		{
			public StandardEnumUserDataDescriptor _003C_003E4__this;

			public Func<ulong, ulong, ulong> operation;

			internal DynValue _003CPerformBinaryOperationU_003Eb__0(ulong v1, ulong v2)
			{
				return _003C_003E4__this.CreateValueUnsigned(operation(v1, v2));
			}
		}

		private Func<object, ulong> m_EnumToULong;

		private Func<ulong, object> m_ULongToEnum;

		private Func<object, long> m_EnumToLong;

		private Func<long, object> m_LongToEnum;

		public Type UnderlyingType { get; private set; }

		public bool IsUnsigned { get; private set; }

		public bool IsFlags { get; private set; }

		public StandardEnumUserDataDescriptor(Type enumType, string friendlyName = null, string[] names = null, object[] values = null, Type underlyingType = null)
			: base(enumType, friendlyName)
		{
			if (!Framework.Do.IsEnum(enumType))
			{
				throw new ArgumentException("enumType must be an enum!");
			}
			UnderlyingType = underlyingType ?? Enum.GetUnderlyingType(enumType);
			IsUnsigned = UnderlyingType == typeof(byte) || UnderlyingType == typeof(ushort) || UnderlyingType == typeof(uint) || UnderlyingType == typeof(ulong);
			names = names ?? Enum.GetNames(base.Type);
			values = values ?? Enum.GetValues(base.Type).OfType<object>().ToArray();
			FillMemberList(names, values);
		}

		private void FillMemberList(string[] names, object[] values)
		{
			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				DynValue value = UserData.Create(values.GetValue(i), this);
				AddDynValue(name, value);
			}
			Attribute[] customAttributes = Framework.Do.GetCustomAttributes(base.Type, typeof(FlagsAttribute), true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				IsFlags = true;
				AddEnumMethod("flagsAnd", DynValue.NewCallback(Callback_And));
				AddEnumMethod("flagsOr", DynValue.NewCallback(Callback_Or));
				AddEnumMethod("flagsXor", DynValue.NewCallback(Callback_Xor));
				AddEnumMethod("flagsNot", DynValue.NewCallback(Callback_BwNot));
				AddEnumMethod("hasAll", DynValue.NewCallback(Callback_HasAll));
				AddEnumMethod("hasAny", DynValue.NewCallback(Callback_HasAny));
			}
		}

		private void AddEnumMethod(string name, DynValue dynValue)
		{
			if (!HasMember(name))
			{
				AddDynValue(name, dynValue);
			}
			if (!HasMember("__" + name))
			{
				AddDynValue("__" + name, dynValue);
			}
		}

		private long GetValueSigned(DynValue dv)
		{
			CreateSignedConversionFunctions();
			if (dv.Type == DataType.Number)
			{
				return (long)dv.Number;
			}
			if (dv.Type != DataType.UserData || dv.UserData.Descriptor != this || dv.UserData.Object == null)
			{
				throw new ScriptRuntimeException("Enum userdata or number expected, or enum is not of the correct type.");
			}
			return m_EnumToLong(dv.UserData.Object);
		}

		private ulong GetValueUnsigned(DynValue dv)
		{
			CreateUnsignedConversionFunctions();
			if (dv.Type == DataType.Number)
			{
				return (ulong)dv.Number;
			}
			if (dv.Type != DataType.UserData || dv.UserData.Descriptor != this || dv.UserData.Object == null)
			{
				throw new ScriptRuntimeException("Enum userdata or number expected, or enum is not of the correct type.");
			}
			return m_EnumToULong(dv.UserData.Object);
		}

		private DynValue CreateValueSigned(long value)
		{
			CreateSignedConversionFunctions();
			return UserData.Create(m_LongToEnum(value), this);
		}

		private DynValue CreateValueUnsigned(ulong value)
		{
			CreateUnsignedConversionFunctions();
			return UserData.Create(m_ULongToEnum(value), this);
		}

		private void CreateSignedConversionFunctions()
		{
			if (m_EnumToLong != null && m_LongToEnum != null)
			{
				return;
			}
			if (UnderlyingType == typeof(sbyte))
			{
				m_EnumToLong = _003C_003Ec._003C_003E9__23_0 ?? (_003C_003Ec._003C_003E9__23_0 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_0);
				m_LongToEnum = _003C_003Ec._003C_003E9__23_1 ?? (_003C_003Ec._003C_003E9__23_1 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_1);
				return;
			}
			if (UnderlyingType == typeof(short))
			{
				m_EnumToLong = _003C_003Ec._003C_003E9__23_2 ?? (_003C_003Ec._003C_003E9__23_2 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_2);
				m_LongToEnum = _003C_003Ec._003C_003E9__23_3 ?? (_003C_003Ec._003C_003E9__23_3 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_3);
				return;
			}
			if (UnderlyingType == typeof(int))
			{
				m_EnumToLong = _003C_003Ec._003C_003E9__23_4 ?? (_003C_003Ec._003C_003E9__23_4 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_4);
				m_LongToEnum = _003C_003Ec._003C_003E9__23_5 ?? (_003C_003Ec._003C_003E9__23_5 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_5);
				return;
			}
			if (UnderlyingType == typeof(long))
			{
				m_EnumToLong = _003C_003Ec._003C_003E9__23_6 ?? (_003C_003Ec._003C_003E9__23_6 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_6);
				m_LongToEnum = _003C_003Ec._003C_003E9__23_7 ?? (_003C_003Ec._003C_003E9__23_7 = _003C_003Ec._003C_003E9._003CCreateSignedConversionFunctions_003Eb__23_7);
				return;
			}
			throw new ScriptRuntimeException("Unexpected enum underlying type : {0}", UnderlyingType.FullName);
		}

		private void CreateUnsignedConversionFunctions()
		{
			if (m_EnumToULong != null && m_ULongToEnum != null)
			{
				return;
			}
			if (UnderlyingType == typeof(byte))
			{
				m_EnumToULong = _003C_003Ec._003C_003E9__24_0 ?? (_003C_003Ec._003C_003E9__24_0 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_0);
				m_ULongToEnum = _003C_003Ec._003C_003E9__24_1 ?? (_003C_003Ec._003C_003E9__24_1 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_1);
				return;
			}
			if (UnderlyingType == typeof(ushort))
			{
				m_EnumToULong = _003C_003Ec._003C_003E9__24_2 ?? (_003C_003Ec._003C_003E9__24_2 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_2);
				m_ULongToEnum = _003C_003Ec._003C_003E9__24_3 ?? (_003C_003Ec._003C_003E9__24_3 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_3);
				return;
			}
			if (UnderlyingType == typeof(uint))
			{
				m_EnumToULong = _003C_003Ec._003C_003E9__24_4 ?? (_003C_003Ec._003C_003E9__24_4 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_4);
				m_ULongToEnum = _003C_003Ec._003C_003E9__24_5 ?? (_003C_003Ec._003C_003E9__24_5 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_5);
				return;
			}
			if (UnderlyingType == typeof(ulong))
			{
				m_EnumToULong = _003C_003Ec._003C_003E9__24_6 ?? (_003C_003Ec._003C_003E9__24_6 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_6);
				m_ULongToEnum = _003C_003Ec._003C_003E9__24_7 ?? (_003C_003Ec._003C_003E9__24_7 = _003C_003Ec._003C_003E9._003CCreateUnsignedConversionFunctions_003Eb__24_7);
				return;
			}
			throw new ScriptRuntimeException("Unexpected enum underlying type : {0}", UnderlyingType.FullName);
		}

		private DynValue PerformBinaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long, DynValue> operation)
		{
			if (args.Count != 2)
			{
				throw new ScriptRuntimeException("Enum.{0} expects two arguments", funcName);
			}
			long valueSigned = GetValueSigned(args[0]);
			long valueSigned2 = GetValueSigned(args[1]);
			return operation(valueSigned, valueSigned2);
		}

		private DynValue PerformBinaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong, DynValue> operation)
		{
			if (args.Count != 2)
			{
				throw new ScriptRuntimeException("Enum.{0} expects two arguments", funcName);
			}
			ulong valueUnsigned = GetValueUnsigned(args[0]);
			ulong valueUnsigned2 = GetValueUnsigned(args[1]);
			return operation(valueUnsigned, valueUnsigned2);
		}

		private DynValue PerformBinaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long, long> operation)
		{
			_003C_003Ec__DisplayClass27_0 _003C_003Ec__DisplayClass27_ = new _003C_003Ec__DisplayClass27_0();
			_003C_003Ec__DisplayClass27_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass27_.operation = operation;
			return PerformBinaryOperationS(funcName, ctx, args, (Func<long, long, DynValue>)_003C_003Ec__DisplayClass27_._003CPerformBinaryOperationS_003Eb__0);
		}

		private DynValue PerformBinaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong, ulong> operation)
		{
			_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = new _003C_003Ec__DisplayClass28_0();
			_003C_003Ec__DisplayClass28_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass28_.operation = operation;
			return PerformBinaryOperationU(funcName, ctx, args, (Func<ulong, ulong, DynValue>)_003C_003Ec__DisplayClass28_._003CPerformBinaryOperationU_003Eb__0);
		}

		private DynValue PerformUnaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long> operation)
		{
			if (args.Count != 1)
			{
				throw new ScriptRuntimeException("Enum.{0} expects one argument.", funcName);
			}
			long valueSigned = GetValueSigned(args[0]);
			long value = operation(valueSigned);
			return CreateValueSigned(value);
		}

		private DynValue PerformUnaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong> operation)
		{
			if (args.Count != 1)
			{
				throw new ScriptRuntimeException("Enum.{0} expects one argument.", funcName);
			}
			ulong valueUnsigned = GetValueUnsigned(args[0]);
			ulong value = operation(valueUnsigned);
			return CreateValueUnsigned(value);
		}

		internal DynValue Callback_Or(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformBinaryOperationU("or", ctx, args, _003C_003Ec._003C_003E9__31_0 ?? (_003C_003Ec._003C_003E9__31_0 = _003C_003Ec._003C_003E9._003CCallback_Or_003Eb__31_0));
			}
			return PerformBinaryOperationS("or", ctx, args, _003C_003Ec._003C_003E9__31_1 ?? (_003C_003Ec._003C_003E9__31_1 = _003C_003Ec._003C_003E9._003CCallback_Or_003Eb__31_1));
		}

		internal DynValue Callback_And(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformBinaryOperationU("and", ctx, args, _003C_003Ec._003C_003E9__32_0 ?? (_003C_003Ec._003C_003E9__32_0 = _003C_003Ec._003C_003E9._003CCallback_And_003Eb__32_0));
			}
			return PerformBinaryOperationS("and", ctx, args, _003C_003Ec._003C_003E9__32_1 ?? (_003C_003Ec._003C_003E9__32_1 = _003C_003Ec._003C_003E9._003CCallback_And_003Eb__32_1));
		}

		internal DynValue Callback_Xor(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformBinaryOperationU("xor", ctx, args, _003C_003Ec._003C_003E9__33_0 ?? (_003C_003Ec._003C_003E9__33_0 = _003C_003Ec._003C_003E9._003CCallback_Xor_003Eb__33_0));
			}
			return PerformBinaryOperationS("xor", ctx, args, _003C_003Ec._003C_003E9__33_1 ?? (_003C_003Ec._003C_003E9__33_1 = _003C_003Ec._003C_003E9._003CCallback_Xor_003Eb__33_1));
		}

		internal DynValue Callback_BwNot(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformUnaryOperationU("not", ctx, args, _003C_003Ec._003C_003E9__34_0 ?? (_003C_003Ec._003C_003E9__34_0 = _003C_003Ec._003C_003E9._003CCallback_BwNot_003Eb__34_0));
			}
			return PerformUnaryOperationS("not", ctx, args, _003C_003Ec._003C_003E9__34_1 ?? (_003C_003Ec._003C_003E9__34_1 = _003C_003Ec._003C_003E9._003CCallback_BwNot_003Eb__34_1));
		}

		internal DynValue Callback_HasAll(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformBinaryOperationU("hasAll", ctx, args, _003C_003Ec._003C_003E9__35_0 ?? (_003C_003Ec._003C_003E9__35_0 = _003C_003Ec._003C_003E9._003CCallback_HasAll_003Eb__35_0));
			}
			return PerformBinaryOperationS("hasAll", ctx, args, _003C_003Ec._003C_003E9__35_1 ?? (_003C_003Ec._003C_003E9__35_1 = _003C_003Ec._003C_003E9._003CCallback_HasAll_003Eb__35_1));
		}

		internal DynValue Callback_HasAny(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
			{
				return PerformBinaryOperationU("hasAny", ctx, args, _003C_003Ec._003C_003E9__36_0 ?? (_003C_003Ec._003C_003E9__36_0 = _003C_003Ec._003C_003E9._003CCallback_HasAny_003Eb__36_0));
			}
			return PerformBinaryOperationS("hasAny", ctx, args, _003C_003Ec._003C_003E9__36_1 ?? (_003C_003Ec._003C_003E9__36_1 = _003C_003Ec._003C_003E9._003CCallback_HasAny_003Eb__36_1));
		}

		public override bool IsTypeCompatible(Type type, object obj)
		{
			if (obj != null)
			{
				return base.Type == type;
			}
			return base.IsTypeCompatible(type, obj);
		}

		public override DynValue MetaIndex(Script script, object obj, string metaname)
		{
			if (metaname == "__concat" && IsFlags)
			{
				return DynValue.NewCallback(Callback_Or);
			}
			return null;
		}
	}
}
