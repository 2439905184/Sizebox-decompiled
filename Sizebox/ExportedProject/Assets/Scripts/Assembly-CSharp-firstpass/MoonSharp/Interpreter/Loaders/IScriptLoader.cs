using System;

namespace MoonSharp.Interpreter.Loaders
{
	public interface IScriptLoader
	{
		object LoadFile(string file, Table globalContext);

		[Obsolete("This serves almost no purpose. Kept here just to preserve backward compatibility.")]
		string ResolveFileName(string filename, Table globalContext);

		string ResolveModuleName(string modname, Table globalContext);
	}
}
