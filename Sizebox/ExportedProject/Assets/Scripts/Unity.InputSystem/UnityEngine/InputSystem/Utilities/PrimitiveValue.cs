using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.InputSystem.Utilities
{
	[StructLayout(LayoutKind.Explicit)]
	public struct PrimitiveValue : IEquatable<PrimitiveValue>, IConvertible
	{
		[FieldOffset(0)]
		private TypeCode m_Type;

		[FieldOffset(4)]
		private bool m_BoolValue;

		[FieldOffset(4)]
		private char m_CharValue;

		[FieldOffset(4)]
		private byte m_ByteValue;

		[FieldOffset(4)]
		private sbyte m_SByteValue;

		[FieldOffset(4)]
		private short m_ShortValue;

		[FieldOffset(4)]
		private ushort m_UShortValue;

		[FieldOffset(4)]
		private int m_IntValue;

		[FieldOffset(4)]
		private uint m_UIntValue;

		[FieldOffset(4)]
		private long m_LongValue;

		[FieldOffset(4)]
		private ulong m_ULongValue;

		[FieldOffset(4)]
		private float m_FloatValue;

		[FieldOffset(4)]
		private double m_DoubleValue;

		public TypeCode type
		{
			get
			{
				return m_Type;
			}
		}

		public bool isEmpty
		{
			get
			{
				return type == TypeCode.Empty;
			}
		}

		public PrimitiveValue(bool value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Boolean;
			m_BoolValue = value;
		}

		public PrimitiveValue(char value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Char;
			m_CharValue = value;
		}

		public PrimitiveValue(byte value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Byte;
			m_ByteValue = value;
		}

		public PrimitiveValue(sbyte value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.SByte;
			m_SByteValue = value;
		}

		public PrimitiveValue(short value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Int16;
			m_ShortValue = value;
		}

		public PrimitiveValue(ushort value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.UInt16;
			m_UShortValue = value;
		}

		public PrimitiveValue(int value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Int32;
			m_IntValue = value;
		}

		public PrimitiveValue(uint value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.UInt32;
			m_UIntValue = value;
		}

		public PrimitiveValue(long value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Int64;
			m_LongValue = value;
		}

		public PrimitiveValue(ulong value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.UInt64;
			m_ULongValue = value;
		}

		public PrimitiveValue(float value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Single;
			m_FloatValue = value;
		}

		public PrimitiveValue(double value)
		{
			this = default(PrimitiveValue);
			m_Type = TypeCode.Double;
			m_DoubleValue = value;
		}

		public PrimitiveValue ConvertTo(TypeCode type)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				return ToBoolean();
			case TypeCode.Char:
				return ToChar();
			case TypeCode.Byte:
				return ToByte();
			case TypeCode.SByte:
				return ToSByte();
			case TypeCode.Int16:
				return ToInt16();
			case TypeCode.Int32:
				return ToInt32();
			case TypeCode.Int64:
				return ToInt64();
			case TypeCode.UInt16:
				return ToInt16();
			case TypeCode.UInt32:
				return ToInt32();
			case TypeCode.UInt64:
				return ToInt64();
			case TypeCode.Single:
				return ToSingle();
			case TypeCode.Double:
				return ToDouble();
			case TypeCode.Empty:
				return default(PrimitiveValue);
			default:
				throw new ArgumentException(string.Format("Don't know how to convert PrimitiveValue to '{0}'", type), "type");
			}
		}

		public unsafe bool Equals(PrimitiveValue other)
		{
			if (m_Type != other.m_Type)
			{
				return false;
			}
			void* ptr = UnsafeUtility.AddressOf(ref m_DoubleValue);
			void* ptr2 = UnsafeUtility.AddressOf(ref other.m_DoubleValue);
			return UnsafeUtility.MemCmp(ptr, ptr2, 8L) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			object obj2;
			if ((obj2 = obj) is PrimitiveValue)
			{
				PrimitiveValue other = (PrimitiveValue)obj2;
				return Equals(other);
			}
			if (obj is bool || obj is char || obj is byte || obj is sbyte || obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is float || obj is double)
			{
				return Equals(FromObject(obj));
			}
			return false;
		}

		public static bool operator ==(PrimitiveValue left, PrimitiveValue right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(PrimitiveValue left, PrimitiveValue right)
		{
			return !left.Equals(right);
		}

		public unsafe override int GetHashCode()
		{
			fixed (double* ptr = &m_DoubleValue)
			{
				return (m_Type.GetHashCode() * 397) ^ ptr->GetHashCode();
			}
		}

		public override string ToString()
		{
			switch (type)
			{
			case TypeCode.Boolean:
				if (!m_BoolValue)
				{
					return "false";
				}
				return "true";
			case TypeCode.Char:
				return "'" + m_CharValue + "'";
			case TypeCode.Byte:
				return m_ByteValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.SByte:
				return m_SByteValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int16:
				return m_ShortValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt16:
				return m_UShortValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int32:
				return m_IntValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt32:
				return m_UIntValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Int64:
				return m_LongValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.UInt64:
				return m_ULongValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Single:
				return m_FloatValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			case TypeCode.Double:
				return m_DoubleValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			default:
				return string.Empty;
			}
		}

		public static PrimitiveValue FromString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return default(PrimitiveValue);
			}
			if (value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PrimitiveValue(true);
			}
			if (value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PrimitiveValue(false);
			}
			double result;
			if ((value.Contains('.') || value.Contains("e") || value.Contains("E") || value.Contains("infinity", StringComparison.InvariantCultureIgnoreCase)) && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return new PrimitiveValue(result);
			}
			long result2;
			if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2))
			{
				return new PrimitiveValue(result2);
			}
			if (value.IndexOf("0x", StringComparison.InvariantCultureIgnoreCase) != -1)
			{
				string text = value.TrimStart();
				if (text.StartsWith("0x"))
				{
					text = text.Substring(2);
				}
				long result3;
				if (long.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result3))
				{
					return new PrimitiveValue(result3);
				}
			}
			throw new NotImplementedException();
		}

		public TypeCode GetTypeCode()
		{
			return type;
		}

		public bool ToBoolean(IFormatProvider provider = null)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				return m_BoolValue;
			case TypeCode.Char:
				return m_CharValue != '\0';
			case TypeCode.Byte:
				return m_ByteValue != 0;
			case TypeCode.SByte:
				return m_SByteValue != 0;
			case TypeCode.Int16:
				return m_ShortValue != 0;
			case TypeCode.UInt16:
				return m_UShortValue != 0;
			case TypeCode.Int32:
				return m_IntValue != 0;
			case TypeCode.UInt32:
				return m_UIntValue != 0;
			case TypeCode.Int64:
				return m_LongValue != 0;
			case TypeCode.UInt64:
				return m_ULongValue != 0;
			case TypeCode.Single:
				return !Mathf.Approximately(m_FloatValue, 0f);
			case TypeCode.Double:
				return NumberHelpers.Approximately(m_DoubleValue, 0.0);
			default:
				return false;
			}
		}

		public byte ToByte(IFormatProvider provider = null)
		{
			return (byte)ToInt64(provider);
		}

		public char ToChar(IFormatProvider provider = null)
		{
			switch (type)
			{
			case TypeCode.Char:
				return m_CharValue;
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return (char)ToInt64(provider);
			default:
				return '\0';
			}
		}

		public DateTime ToDateTime(IFormatProvider provider = null)
		{
			throw new NotSupportedException("Converting PrimitiveValue to DateTime");
		}

		public decimal ToDecimal(IFormatProvider provider = null)
		{
			return new decimal(ToDouble(provider));
		}

		public double ToDouble(IFormatProvider provider = null)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				if (m_BoolValue)
				{
					return 1.0;
				}
				return 0.0;
			case TypeCode.Char:
				return (int)m_CharValue;
			case TypeCode.Byte:
				return (int)m_ByteValue;
			case TypeCode.SByte:
				return m_SByteValue;
			case TypeCode.Int16:
				return m_ShortValue;
			case TypeCode.UInt16:
				return (int)m_UShortValue;
			case TypeCode.Int32:
				return m_IntValue;
			case TypeCode.UInt32:
				return m_UIntValue;
			case TypeCode.Int64:
				return m_LongValue;
			case TypeCode.UInt64:
				return m_ULongValue;
			case TypeCode.Single:
				return m_FloatValue;
			case TypeCode.Double:
				return m_DoubleValue;
			default:
				return 0.0;
			}
		}

		public short ToInt16(IFormatProvider provider = null)
		{
			return (short)ToInt64(provider);
		}

		public int ToInt32(IFormatProvider provider = null)
		{
			return (int)ToInt64(provider);
		}

		public long ToInt64(IFormatProvider provider = null)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				if (m_BoolValue)
				{
					return 1L;
				}
				return 0L;
			case TypeCode.Char:
				return m_CharValue;
			case TypeCode.Byte:
				return m_ByteValue;
			case TypeCode.SByte:
				return m_SByteValue;
			case TypeCode.Int16:
				return m_ShortValue;
			case TypeCode.UInt16:
				return m_UShortValue;
			case TypeCode.Int32:
				return m_IntValue;
			case TypeCode.UInt32:
				return m_UIntValue;
			case TypeCode.Int64:
				return m_LongValue;
			case TypeCode.UInt64:
				return (long)m_ULongValue;
			case TypeCode.Single:
				return (long)m_FloatValue;
			case TypeCode.Double:
				return (long)m_DoubleValue;
			default:
				return 0L;
			}
		}

		public sbyte ToSByte(IFormatProvider provider = null)
		{
			return (sbyte)ToInt64(provider);
		}

		public float ToSingle(IFormatProvider provider = null)
		{
			return (float)ToDouble(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return ToString();
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		public ushort ToUInt16(IFormatProvider provider = null)
		{
			return (ushort)ToUInt64();
		}

		public uint ToUInt32(IFormatProvider provider = null)
		{
			return (uint)ToUInt64();
		}

		public ulong ToUInt64(IFormatProvider provider = null)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				if (m_BoolValue)
				{
					return 1uL;
				}
				return 0uL;
			case TypeCode.Char:
				return m_CharValue;
			case TypeCode.Byte:
				return m_ByteValue;
			case TypeCode.SByte:
				return (ulong)m_SByteValue;
			case TypeCode.Int16:
				return (ulong)m_ShortValue;
			case TypeCode.UInt16:
				return m_UShortValue;
			case TypeCode.Int32:
				return (ulong)m_IntValue;
			case TypeCode.UInt32:
				return m_UIntValue;
			case TypeCode.Int64:
				return (ulong)m_LongValue;
			case TypeCode.UInt64:
				return m_ULongValue;
			case TypeCode.Single:
				return (ulong)m_FloatValue;
			case TypeCode.Double:
				return (ulong)m_DoubleValue;
			default:
				return 0uL;
			}
		}

		public object ToObject()
		{
			switch (m_Type)
			{
			case TypeCode.Boolean:
				return m_BoolValue;
			case TypeCode.Char:
				return m_CharValue;
			case TypeCode.Byte:
				return m_ByteValue;
			case TypeCode.SByte:
				return m_SByteValue;
			case TypeCode.Int16:
				return m_ShortValue;
			case TypeCode.UInt16:
				return m_UShortValue;
			case TypeCode.Int32:
				return m_IntValue;
			case TypeCode.UInt32:
				return m_UIntValue;
			case TypeCode.Int64:
				return m_LongValue;
			case TypeCode.UInt64:
				return m_ULongValue;
			case TypeCode.Single:
				return m_FloatValue;
			case TypeCode.Double:
				return m_DoubleValue;
			default:
				return null;
			}
		}

		public static PrimitiveValue From<TValue>(TValue value) where TValue : struct
		{
			Type type = typeof(TValue);
			if (type.IsEnum)
			{
				type = type.GetEnumUnderlyingType();
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return new PrimitiveValue(Convert.ToBoolean(value));
			case TypeCode.Char:
				return new PrimitiveValue(Convert.ToChar(value));
			case TypeCode.Byte:
				return new PrimitiveValue(Convert.ToByte(value));
			case TypeCode.SByte:
				return new PrimitiveValue(Convert.ToSByte(value));
			case TypeCode.Int16:
				return new PrimitiveValue(Convert.ToInt16(value));
			case TypeCode.Int32:
				return new PrimitiveValue(Convert.ToInt32(value));
			case TypeCode.Int64:
				return new PrimitiveValue(Convert.ToInt64(value));
			case TypeCode.UInt16:
				return new PrimitiveValue(Convert.ToUInt16(value));
			case TypeCode.UInt32:
				return new PrimitiveValue(Convert.ToUInt32(value));
			case TypeCode.UInt64:
				return new PrimitiveValue(Convert.ToUInt64(value));
			case TypeCode.Single:
				return new PrimitiveValue(Convert.ToSingle(value));
			case TypeCode.Double:
				return new PrimitiveValue(Convert.ToDouble(value));
			default:
				throw new ArgumentException(string.Format("Cannot convert value '{0}' of type '{1}' to PrimitiveValue", value, typeof(TValue).Name), "value");
			}
		}

		public static PrimitiveValue FromObject(object value)
		{
			if (value == null)
			{
				return default(PrimitiveValue);
			}
			string value2;
			if ((value2 = value as string) != null)
			{
				return FromString(value2);
			}
			object obj;
			if ((obj = value) is bool)
			{
				bool value3 = (bool)obj;
				return new PrimitiveValue(value3);
			}
			if ((obj = value) is char)
			{
				char value4 = (char)obj;
				return new PrimitiveValue(value4);
			}
			if ((obj = value) is byte)
			{
				byte value5 = (byte)obj;
				return new PrimitiveValue(value5);
			}
			if ((obj = value) is sbyte)
			{
				sbyte value6 = (sbyte)obj;
				return new PrimitiveValue(value6);
			}
			if ((obj = value) is short)
			{
				short value7 = (short)obj;
				return new PrimitiveValue(value7);
			}
			if ((obj = value) is ushort)
			{
				ushort value8 = (ushort)obj;
				return new PrimitiveValue(value8);
			}
			if ((obj = value) is int)
			{
				int value9 = (int)obj;
				return new PrimitiveValue(value9);
			}
			if ((obj = value) is uint)
			{
				uint value10 = (uint)obj;
				return new PrimitiveValue(value10);
			}
			if ((obj = value) is long)
			{
				long value11 = (long)obj;
				return new PrimitiveValue(value11);
			}
			if ((obj = value) is ulong)
			{
				ulong value12 = (ulong)obj;
				return new PrimitiveValue(value12);
			}
			if ((obj = value) is float)
			{
				float value13 = (float)obj;
				return new PrimitiveValue(value13);
			}
			if ((obj = value) is double)
			{
				double value14 = (double)obj;
				return new PrimitiveValue(value14);
			}
			if (value is Enum)
			{
				switch (Type.GetTypeCode(value.GetType().GetEnumUnderlyingType()))
				{
				case TypeCode.Byte:
					return new PrimitiveValue((byte)value);
				case TypeCode.SByte:
					return new PrimitiveValue((sbyte)value);
				case TypeCode.Int16:
					return new PrimitiveValue((short)value);
				case TypeCode.Int32:
					return new PrimitiveValue((int)value);
				case TypeCode.Int64:
					return new PrimitiveValue((long)value);
				case TypeCode.UInt16:
					return new PrimitiveValue((ushort)value);
				case TypeCode.UInt32:
					return new PrimitiveValue((uint)value);
				case TypeCode.UInt64:
					return new PrimitiveValue((ulong)value);
				}
			}
			throw new ArgumentException(string.Format("Cannot convert '{0}' to primitive value", value), "value");
		}

		public static implicit operator PrimitiveValue(bool value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(char value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(byte value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(sbyte value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(short value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(ushort value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(int value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(uint value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(long value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(ulong value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(float value)
		{
			return new PrimitiveValue(value);
		}

		public static implicit operator PrimitiveValue(double value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromBoolean(bool value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromChar(char value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromByte(byte value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromSByte(sbyte value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt16(short value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt16(ushort value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt32(int value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt32(uint value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromInt64(long value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromUInt64(ulong value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromSingle(float value)
		{
			return new PrimitiveValue(value);
		}

		public static PrimitiveValue FromDouble(double value)
		{
			return new PrimitiveValue(value);
		}
	}
}
