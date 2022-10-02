using System;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop
{
	public class StandardGenericsUserDataDescriptor : IUserDataDescriptor, IGeneratorUserDataDescriptor
	{
		public InteropAccessMode AccessMode { get; private set; }

		public string Name { get; private set; }

		public Type Type { get; private set; }

		public StandardGenericsUserDataDescriptor(Type type, InteropAccessMode accessMode)
		{
			if (accessMode == InteropAccessMode.NoReflectionAllowed)
			{
				throw new ArgumentException("Can't create a StandardGenericsUserDataDescriptor under a NoReflectionAllowed access mode");
			}
			AccessMode = accessMode;
			Type = type;
			Name = "@@" + type.FullName;
		}

		public DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing)
		{
			return null;
		}

		public bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing)
		{
			return false;
		}

		public string AsString(object obj)
		{
			return obj.ToString();
		}

		public DynValue MetaIndex(Script script, object obj, string metaname)
		{
			return null;
		}

		public bool IsTypeCompatible(Type type, object obj)
		{
			return Framework.Do.IsInstanceOfType(type, obj);
		}

		public IUserDataDescriptor Generate(Type type)
		{
			if (UserData.IsTypeRegistered(type))
			{
				return null;
			}
			if (Framework.Do.IsGenericTypeDefinition(type))
			{
				return null;
			}
			return UserData.RegisterType(type, AccessMode);
		}
	}
}
