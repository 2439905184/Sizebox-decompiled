using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop
{
	public class CompositeUserDataDescriptor : IUserDataDescriptor
	{
		private List<IUserDataDescriptor> m_Descriptors;

		private Type m_Type;

		public IList<IUserDataDescriptor> Descriptors
		{
			get
			{
				return m_Descriptors;
			}
		}

		public string Name
		{
			get
			{
				return "^" + m_Type.FullName;
			}
		}

		public Type Type
		{
			get
			{
				return m_Type;
			}
		}

		public CompositeUserDataDescriptor(List<IUserDataDescriptor> descriptors, Type type)
		{
			m_Descriptors = descriptors;
			m_Type = type;
		}

		public DynValue Index(Script script, object obj, DynValue index, bool isNameIndex)
		{
			foreach (IUserDataDescriptor descriptor in m_Descriptors)
			{
				DynValue dynValue = descriptor.Index(script, obj, index, isNameIndex);
				if (dynValue != null)
				{
					return dynValue;
				}
			}
			return null;
		}

		public bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isNameIndex)
		{
			foreach (IUserDataDescriptor descriptor in m_Descriptors)
			{
				if (descriptor.SetIndex(script, obj, index, value, isNameIndex))
				{
					return true;
				}
			}
			return false;
		}

		public string AsString(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return obj.ToString();
		}

		public DynValue MetaIndex(Script script, object obj, string metaname)
		{
			foreach (IUserDataDescriptor descriptor in m_Descriptors)
			{
				DynValue dynValue = descriptor.MetaIndex(script, obj, metaname);
				if (dynValue != null)
				{
					return dynValue;
				}
			}
			return null;
		}

		public bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}
	}
}
