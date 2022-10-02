using System;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop.Converters
{
	internal static class ScriptToClrConversions
	{
		internal const int WEIGHT_MAX_VALUE = 100;

		internal const int WEIGHT_CUSTOM_CONVERTER_MATCH = 100;

		internal const int WEIGHT_EXACT_MATCH = 100;

		internal const int WEIGHT_STRING_TO_STRINGBUILDER = 99;

		internal const int WEIGHT_STRING_TO_CHAR = 98;

		internal const int WEIGHT_NIL_TO_NULLABLE = 100;

		internal const int WEIGHT_NIL_TO_REFTYPE = 100;

		internal const int WEIGHT_VOID_WITH_DEFAULT = 50;

		internal const int WEIGHT_VOID_WITHOUT_DEFAULT = 25;

		internal const int WEIGHT_NIL_WITH_DEFAULT = 25;

		internal const int WEIGHT_BOOL_TO_STRING = 5;

		internal const int WEIGHT_NUMBER_TO_STRING = 50;

		internal const int WEIGHT_NUMBER_TO_ENUM = 90;

		internal const int WEIGHT_USERDATA_TO_STRING = 5;

		internal const int WEIGHT_TABLE_CONVERSION = 90;

		internal const int WEIGHT_NUMBER_DOWNCAST = 99;

		internal const int WEIGHT_NO_MATCH = 0;

		internal const int WEIGHT_NO_EXTRA_PARAMS_BONUS = 100;

		internal const int WEIGHT_EXTRA_PARAMS_MALUS = 2;

		internal const int WEIGHT_BYREF_BONUSMALUS = -10;

		internal const int WEIGHT_VARARGS_MALUS = 1;

		internal const int WEIGHT_VARARGS_EMPTY = 40;

		internal static object DynValueToObject(DynValue value)
		{
			Func<DynValue, object> scriptToClrCustomConversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(value.Type, typeof(object));
			if (scriptToClrCustomConversion != null)
			{
				object obj = scriptToClrCustomConversion(value);
				if (obj != null)
				{
					return obj;
				}
			}
			switch (value.Type)
			{
			case DataType.Nil:
			case DataType.Void:
				return null;
			case DataType.Boolean:
				return value.Boolean;
			case DataType.Number:
				return value.Number;
			case DataType.String:
				return value.String;
			case DataType.Function:
				return value.Function;
			case DataType.Table:
				return value.Table;
			case DataType.Tuple:
				return value.Tuple;
			case DataType.UserData:
				if (value.UserData.Object != null)
				{
					return value.UserData.Object;
				}
				if (value.UserData.Descriptor != null)
				{
					return value.UserData.Descriptor.Type;
				}
				return null;
			case DataType.ClrFunction:
				return value.Callback;
			default:
				throw ScriptRuntimeException.ConvertObjectFailed(value.Type);
			}
		}

		internal static object DynValueToObjectOfType(DynValue value, Type desiredType, object defaultValue, bool isOptional)
		{
			if (desiredType.IsByRef)
			{
				desiredType = desiredType.GetElementType();
			}
			Func<DynValue, object> scriptToClrCustomConversion = Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(value.Type, desiredType);
			if (scriptToClrCustomConversion != null)
			{
				object obj = scriptToClrCustomConversion(value);
				if (obj != null)
				{
					return obj;
				}
			}
			if (desiredType == typeof(DynValue))
			{
				return value;
			}
			if (desiredType == typeof(object))
			{
				return DynValueToObject(value);
			}
			StringConversions.StringSubtype stringSubtype = StringConversions.GetStringSubtype(desiredType);
			string text = null;
			Type underlyingType = Nullable.GetUnderlyingType(desiredType);
			Type type = null;
			if (underlyingType != null)
			{
				type = desiredType;
				desiredType = underlyingType;
			}
			switch (value.Type)
			{
			case DataType.Void:
				if (isOptional)
				{
					return defaultValue;
				}
				if (!Framework.Do.IsValueType(desiredType) || type != null)
				{
					return null;
				}
				break;
			case DataType.Nil:
				if (Framework.Do.IsValueType(desiredType))
				{
					if (type != null)
					{
						return null;
					}
					if (isOptional)
					{
						return defaultValue;
					}
					break;
				}
				return null;
			case DataType.Boolean:
				if (desiredType == typeof(bool))
				{
					return value.Boolean;
				}
				if (stringSubtype != 0)
				{
					text = value.Boolean.ToString();
				}
				break;
			case DataType.Number:
				if (Framework.Do.IsEnum(desiredType))
				{
					return NumericConversions.DoubleToType(Enum.GetUnderlyingType(desiredType), value.Number);
				}
				if (NumericConversions.NumericTypes.Contains(desiredType))
				{
					return NumericConversions.DoubleToType(desiredType, value.Number);
				}
				if (stringSubtype != 0)
				{
					text = value.Number.ToString();
				}
				break;
			case DataType.String:
				if (stringSubtype != 0)
				{
					text = value.String;
				}
				break;
			case DataType.Function:
				if (desiredType == typeof(Closure))
				{
					return value.Function;
				}
				if (desiredType == typeof(ScriptFunctionDelegate))
				{
					return value.Function.GetDelegate();
				}
				break;
			case DataType.ClrFunction:
				if (desiredType == typeof(CallbackFunction))
				{
					return value.Callback;
				}
				if (desiredType == typeof(Func<ScriptExecutionContext, CallbackArguments, DynValue>))
				{
					return value.Callback.ClrCallback;
				}
				break;
			case DataType.UserData:
				if (value.UserData.Object != null)
				{
					object @object = value.UserData.Object;
					IUserDataDescriptor descriptor = value.UserData.Descriptor;
					if (descriptor.IsTypeCompatible(desiredType, @object))
					{
						return @object;
					}
					if (stringSubtype != 0)
					{
						text = descriptor.AsString(@object);
					}
				}
				break;
			case DataType.Table:
			{
				if (desiredType == typeof(Table) || Framework.Do.IsAssignableFrom(desiredType, typeof(Table)))
				{
					return value.Table;
				}
				object obj2 = TableConversions.ConvertTableToType(value.Table, desiredType);
				if (obj2 != null)
				{
					return obj2;
				}
				break;
			}
			}
			if (stringSubtype != 0 && text != null)
			{
				return StringConversions.ConvertString(stringSubtype, text, desiredType, value.Type);
			}
			throw ScriptRuntimeException.ConvertObjectFailed(value.Type, desiredType);
		}

		internal static int DynValueToObjectOfTypeWeight(DynValue value, Type desiredType, bool isOptional)
		{
			if (desiredType.IsByRef)
			{
				desiredType = desiredType.GetElementType();
			}
			if (Script.GlobalOptions.CustomConverters.GetScriptToClrCustomConversion(value.Type, desiredType) != null)
			{
				return 100;
			}
			if (desiredType == typeof(DynValue))
			{
				return 100;
			}
			if (desiredType == typeof(object))
			{
				return 100;
			}
			StringConversions.StringSubtype stringSubtype = StringConversions.GetStringSubtype(desiredType);
			Type underlyingType = Nullable.GetUnderlyingType(desiredType);
			Type type = null;
			if (underlyingType != null)
			{
				type = desiredType;
				desiredType = underlyingType;
			}
			switch (value.Type)
			{
			case DataType.Void:
				if (isOptional)
				{
					return 50;
				}
				if (!Framework.Do.IsValueType(desiredType) || type != null)
				{
					return 25;
				}
				break;
			case DataType.Nil:
				if (Framework.Do.IsValueType(desiredType))
				{
					if (type != null)
					{
						return 100;
					}
					if (isOptional)
					{
						return 25;
					}
					break;
				}
				return 100;
			case DataType.Boolean:
				if (desiredType == typeof(bool))
				{
					return 100;
				}
				if (stringSubtype != 0)
				{
					return 5;
				}
				break;
			case DataType.Number:
				if (Framework.Do.IsEnum(desiredType))
				{
					return 90;
				}
				if (NumericConversions.NumericTypes.Contains(desiredType))
				{
					return GetNumericTypeWeight(desiredType);
				}
				if (stringSubtype != 0)
				{
					return 50;
				}
				break;
			case DataType.String:
				switch (stringSubtype)
				{
				case StringConversions.StringSubtype.String:
					return 100;
				case StringConversions.StringSubtype.StringBuilder:
					return 99;
				case StringConversions.StringSubtype.Char:
					return 98;
				}
				break;
			case DataType.Function:
				if (desiredType == typeof(Closure))
				{
					return 100;
				}
				if (desiredType == typeof(ScriptFunctionDelegate))
				{
					return 100;
				}
				break;
			case DataType.ClrFunction:
				if (desiredType == typeof(CallbackFunction))
				{
					return 100;
				}
				if (desiredType == typeof(Func<ScriptExecutionContext, CallbackArguments, DynValue>))
				{
					return 100;
				}
				break;
			case DataType.UserData:
				if (value.UserData.Object != null)
				{
					object @object = value.UserData.Object;
					if (value.UserData.Descriptor.IsTypeCompatible(desiredType, @object))
					{
						return 100;
					}
					if (stringSubtype != 0)
					{
						return 5;
					}
				}
				break;
			case DataType.Table:
				if (desiredType == typeof(Table) || Framework.Do.IsAssignableFrom(desiredType, typeof(Table)))
				{
					return 100;
				}
				if (TableConversions.CanConvertTableToType(value.Table, desiredType))
				{
					return 90;
				}
				break;
			}
			return 0;
		}

		private static int GetNumericTypeWeight(Type desiredType)
		{
			if (desiredType == typeof(double) || desiredType == typeof(decimal))
			{
				return 100;
			}
			return 99;
		}
	}
}
