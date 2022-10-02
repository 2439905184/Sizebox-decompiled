using System;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.Interop.Converters
{
	internal static class NumericConversions
	{
		internal static readonly HashSet<Type> NumericTypes;

		internal static readonly Type[] NumericTypesOrdered;

		static NumericConversions()
		{
			NumericTypesOrdered = new Type[11]
			{
				typeof(double),
				typeof(decimal),
				typeof(float),
				typeof(long),
				typeof(int),
				typeof(short),
				typeof(sbyte),
				typeof(ulong),
				typeof(uint),
				typeof(ushort),
				typeof(byte)
			};
			NumericTypes = new HashSet<Type>(NumericTypesOrdered);
		}

		internal static object DoubleToType(Type type, double d)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;
			if (type == typeof(double))
			{
				return d;
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)d;
			}
			if (type == typeof(byte))
			{
				return (byte)d;
			}
			if (type == typeof(short))
			{
				return (short)d;
			}
			if (type == typeof(ushort))
			{
				return (ushort)d;
			}
			if (type == typeof(int))
			{
				return (int)d;
			}
			if (type == typeof(uint))
			{
				return (uint)d;
			}
			if (type == typeof(long))
			{
				return (long)d;
			}
			if (type == typeof(ulong))
			{
				return (ulong)d;
			}
			if (type == typeof(float))
			{
				return (float)d;
			}
			if (type == typeof(decimal))
			{
				return (decimal)d;
			}
			return d;
		}

		internal static double TypeToDouble(Type type, object d)
		{
			if (type == typeof(double))
			{
				return (double)d;
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)d;
			}
			if (type == typeof(byte))
			{
				return (int)(byte)d;
			}
			if (type == typeof(short))
			{
				return (short)d;
			}
			if (type == typeof(ushort))
			{
				return (int)(ushort)d;
			}
			if (type == typeof(int))
			{
				return (int)d;
			}
			if (type == typeof(uint))
			{
				return (uint)d;
			}
			if (type == typeof(long))
			{
				return (long)d;
			}
			if (type == typeof(ulong))
			{
				return (ulong)d;
			}
			if (type == typeof(float))
			{
				return (float)d;
			}
			if (type == typeof(decimal))
			{
				return (double)(decimal)d;
			}
			return (double)d;
		}
	}
}
