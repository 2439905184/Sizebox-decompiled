using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	public interface IPlatformAccessor
	{
		CoreModules FilterSupportedCoreModules(CoreModules module);

		string GetEnvironmentVariable(string envvarname);

		bool IsRunningOnAOT();

		string GetPlatformName();

		void DefaultPrint(string content);

		string DefaultInput(string prompt);

		Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode);

		Stream IO_GetStandardStream(StandardFileType type);

		string IO_OS_GetTempFilename();

		void OS_ExitFast(int exitCode);

		bool OS_FileExists(string file);

		void OS_FileDelete(string file);

		void OS_FileMove(string src, string dst);

		int OS_Execute(string cmdline);
	}
}
