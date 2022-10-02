using System;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop;

namespace MoonSharp.Interpreter
{
	internal class AutoDescribingUserDataDescriptor : IUserDataDescriptor
	{
		private string m_FriendlyName;

		private Type m_Type;

		public string Name
		{
			get
			{
				return m_FriendlyName;
			}
		}

		public Type Type
		{
			get
			{
				return m_Type;
			}
		}

		public AutoDescribingUserDataDescriptor(Type type, string friendlyName)
		{
			m_FriendlyName = friendlyName;
			m_Type = type;
		}

		public DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing)
		{
			IUserDataType userDataType = obj as IUserDataType;
			if (userDataType != null)
			{
				return userDataType.Index(script, index, isDirectIndexing);
			}
			return null;
		}

		public bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing)
		{
			IUserDataType userDataType = obj as IUserDataType;
			if (userDataType != null)
			{
				return userDataType.SetIndex(script, index, value, isDirectIndexing);
			}
			return false;
		}

		public string AsString(object obj)
		{
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}

		public DynValue MetaIndex(Script script, object obj, string metaname)
		{
			IUserDataType userDataType = obj as IUserDataType;
			if (userDataType != null)
			{
				return userDataType.MetaIndex(script, metaname);
			}
			return null;
		}

		public bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}
	}
}
