using System;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	public class LimitedPlatformAccessor : PlatformAccessorBase
	{
		public override string GetEnvironmentVariable(string envvarname)
		{
			return null;
		}

		public override CoreModules FilterSupportedCoreModules(CoreModules module)
		{
			return module & ~(CoreModules.OS_System | CoreModules.IO);
		}

		public override Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override Stream IO_GetStandardStream(StandardFileType type)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override string IO_OS_GetTempFilename()
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override void OS_ExitFast(int exitCode)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override bool OS_FileExists(string file)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override void OS_FileDelete(string file)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override void OS_FileMove(string src, string dst)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override int OS_Execute(string cmdline)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		public override string GetPlatformNamePrefix()
		{
			return "limited";
		}

		public override void DefaultPrint(string content)
		{
		}
	}
}
