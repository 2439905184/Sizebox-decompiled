using System;
using System.Collections;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Serialization
{
	public static class ObjectValueConverter
	{
		public static DynValue SerializeObjectToDynValue(Script script, object o, DynValue valueForNulls = null)
		{
			if (o == null)
			{
				return valueForNulls ?? DynValue.Nil;
			}
			DynValue dynValue = ClrToScriptConversions.TryObjectToTrivialDynValue(script, o);
			if (dynValue != null)
			{
				return dynValue;
			}
			if (o is Enum)
			{
				return DynValue.NewNumber(NumericConversions.TypeToDouble(Enum.GetUnderlyingType(o.GetType()), o));
			}
			Table table = new Table(script);
			IEnumerable enumerable = o as IEnumerable;
			if (enumerable != null)
			{
				foreach (object item in enumerable)
				{
					table.Append(SerializeObjectToDynValue(script, item, valueForNulls));
				}
			}
			else
			{
				Type type = o.GetType();
				PropertyInfo[] properties = Framework.Do.GetProperties(type);
				foreach (PropertyInfo propertyInfo in properties)
				{
					MethodInfo getMethod = Framework.Do.GetGetMethod(propertyInfo);
					object o2 = getMethod.Invoke(getMethod.IsStatic ? null : o, null);
					table.Set(propertyInfo.Name, SerializeObjectToDynValue(script, o2, valueForNulls));
				}
			}
			return DynValue.NewTable(table);
		}
	}
}
