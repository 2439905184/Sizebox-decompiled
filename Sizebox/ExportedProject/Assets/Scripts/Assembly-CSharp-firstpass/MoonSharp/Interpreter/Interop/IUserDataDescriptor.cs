using System;

namespace MoonSharp.Interpreter.Interop
{
	public interface IUserDataDescriptor
	{
		string Name { get; }

		Type Type { get; }

		DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing);

		bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing);

		string AsString(object obj);

		DynValue MetaIndex(Script script, object obj, string metaname);

		bool IsTypeCompatible(Type type, object obj);
	}
}
