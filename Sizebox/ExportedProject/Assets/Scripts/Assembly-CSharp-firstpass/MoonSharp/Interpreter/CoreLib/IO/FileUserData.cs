using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.CoreLib.IO
{
	internal class FileUserData : StreamFileUserDataBase
	{
		public FileUserData(Script script, string filename, Encoding encoding, string mode)
		{
			Stream stream = Script.GlobalOptions.Platform.IO_OpenFile(script, filename, encoding, mode);
			StreamReader reader = (stream.CanRead ? new StreamReader(stream, encoding) : null);
			StreamWriter writer = (stream.CanWrite ? new StreamWriter(stream, encoding) : null);
			Initialize(stream, reader, writer);
		}
	}
}
