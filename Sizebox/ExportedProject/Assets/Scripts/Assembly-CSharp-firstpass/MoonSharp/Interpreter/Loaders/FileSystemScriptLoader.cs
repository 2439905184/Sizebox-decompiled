using System.IO;

namespace MoonSharp.Interpreter.Loaders
{
	public class FileSystemScriptLoader : ScriptLoaderBase
	{
		public override bool ScriptFileExists(string name)
		{
			return File.Exists(name);
		}

		public override object LoadFile(string file, Table globalContext)
		{
			return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
	}
}
