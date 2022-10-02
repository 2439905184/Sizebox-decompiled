namespace MoonSharp.Interpreter.Interop
{
	public interface IUserDataType
	{
		DynValue Index(Script script, DynValue index, bool isDirectIndexing);

		bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing);

		DynValue MetaIndex(Script script, string metaname);
	}
}
