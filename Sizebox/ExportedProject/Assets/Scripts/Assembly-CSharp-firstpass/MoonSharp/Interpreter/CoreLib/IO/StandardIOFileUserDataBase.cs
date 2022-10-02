using System.IO;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	internal class StandardIOFileUserDataBase : StreamFileUserDataBase
	{
		protected override string Close()
		{
			return "cannot close standard file";
		}

		public static StandardIOFileUserDataBase CreateInputStream(Stream stream)
		{
			StandardIOFileUserDataBase standardIOFileUserDataBase = new StandardIOFileUserDataBase();
			standardIOFileUserDataBase.Initialize(stream, new StreamReader(stream), null);
			return standardIOFileUserDataBase;
		}

		public static StandardIOFileUserDataBase CreateOutputStream(Stream stream)
		{
			StandardIOFileUserDataBase standardIOFileUserDataBase = new StandardIOFileUserDataBase();
			standardIOFileUserDataBase.Initialize(stream, null, new StreamWriter(stream));
			return standardIOFileUserDataBase;
		}
	}
}
