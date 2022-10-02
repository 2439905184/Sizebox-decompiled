using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	public class PropertyTableAssigner<T> : IPropertyTableAssigner
	{
		private PropertyTableAssigner m_InternalAssigner;

		public PropertyTableAssigner(params string[] expectedMissingProperties)
		{
			m_InternalAssigner = new PropertyTableAssigner(typeof(T), expectedMissingProperties);
		}

		public void AddExpectedMissingProperty(string name)
		{
			m_InternalAssigner.AddExpectedMissingProperty(name);
		}

		public void AssignObject(T obj, Table data)
		{
			m_InternalAssigner.AssignObject(obj, data);
		}

		public PropertyTableAssigner GetTypeUnsafeAssigner()
		{
			return m_InternalAssigner;
		}

		public void SetSubassignerForType(Type propertyType, IPropertyTableAssigner assigner)
		{
			m_InternalAssigner.SetSubassignerForType(propertyType, assigner);
		}

		public void SetSubassigner<SubassignerType>(PropertyTableAssigner<SubassignerType> assigner)
		{
			m_InternalAssigner.SetSubassignerForType(typeof(SubassignerType), assigner);
		}

		void IPropertyTableAssigner.AssignObjectUnchecked(object o, Table data)
		{
			AssignObject((T)o, data);
		}
	}
	public class PropertyTableAssigner : IPropertyTableAssigner
	{
		private Type m_Type;

		private Dictionary<string, PropertyInfo> m_PropertyMap = new Dictionary<string, PropertyInfo>();

		private Dictionary<Type, IPropertyTableAssigner> m_SubAssigners = new Dictionary<Type, IPropertyTableAssigner>();

		public PropertyTableAssigner(Type type, params string[] expectedMissingProperties)
		{
			m_Type = type;
			if (Framework.Do.IsValueType(m_Type))
			{
				throw new ArgumentException("Type cannot be a value type.");
			}
			foreach (string key in expectedMissingProperties)
			{
				m_PropertyMap.Add(key, null);
			}
			PropertyInfo[] properties = Framework.Do.GetProperties(m_Type);
			foreach (PropertyInfo propertyInfo in properties)
			{
				foreach (MoonSharpPropertyAttribute item in propertyInfo.GetCustomAttributes(true).OfType<MoonSharpPropertyAttribute>())
				{
					string text = item.Name ?? propertyInfo.Name;
					if (m_PropertyMap.ContainsKey(text))
					{
						throw new ArgumentException(string.Format("Type {0} has two definitions for MoonSharp property {1}", m_Type.FullName, text));
					}
					m_PropertyMap.Add(text, propertyInfo);
				}
			}
		}

		public void AddExpectedMissingProperty(string name)
		{
			m_PropertyMap.Add(name, null);
		}

		private bool TryAssignProperty(object obj, string name, DynValue value)
		{
			if (m_PropertyMap.ContainsKey(name))
			{
				PropertyInfo propertyInfo = m_PropertyMap[name];
				if (propertyInfo != null)
				{
					object obj2;
					if (value.Type == DataType.Table && m_SubAssigners.ContainsKey(propertyInfo.PropertyType))
					{
						IPropertyTableAssigner propertyTableAssigner = m_SubAssigners[propertyInfo.PropertyType];
						obj2 = Activator.CreateInstance(propertyInfo.PropertyType);
						propertyTableAssigner.AssignObjectUnchecked(obj2, value.Table);
					}
					else
					{
						obj2 = ScriptToClrConversions.DynValueToObjectOfType(value, propertyInfo.PropertyType, null, false);
					}
					Framework.Do.GetSetMethod(propertyInfo).Invoke(obj, new object[1] { obj2 });
				}
				return true;
			}
			return false;
		}

		private void AssignProperty(object obj, string name, DynValue value)
		{
			if (TryAssignProperty(obj, name, value) || TryAssignProperty(obj, DescriptorHelpers.UpperFirstLetter(name), value) || TryAssignProperty(obj, DescriptorHelpers.Camelify(name), value) || TryAssignProperty(obj, DescriptorHelpers.UpperFirstLetter(DescriptorHelpers.Camelify(name)), value))
			{
				return;
			}
			throw new ScriptRuntimeException("Invalid property {0}", name);
		}

		public void AssignObject(object obj, Table data)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("Object is null");
			}
			if (!Framework.Do.IsInstanceOfType(m_Type, obj))
			{
				throw new ArgumentException(string.Format("Invalid type of object : got '{0}', expected {1}", obj.GetType().FullName, m_Type.FullName));
			}
			foreach (TablePair pair in data.Pairs)
			{
				if (pair.Key.Type != DataType.String)
				{
					throw new ScriptRuntimeException("Invalid property of type {0}", pair.Key.Type.ToErrorTypeString());
				}
				AssignProperty(obj, pair.Key.String, pair.Value);
			}
		}

		public void SetSubassignerForType(Type propertyType, IPropertyTableAssigner assigner)
		{
			if (Framework.Do.IsAbstract(propertyType) || Framework.Do.IsGenericType(propertyType) || Framework.Do.IsInterface(propertyType) || Framework.Do.IsValueType(propertyType))
			{
				throw new ArgumentException("propertyType must be a concrete, reference type");
			}
			m_SubAssigners[propertyType] = assigner;
		}

		void IPropertyTableAssigner.AssignObjectUnchecked(object obj, Table data)
		{
			AssignObject(obj, data);
		}
	}
}
