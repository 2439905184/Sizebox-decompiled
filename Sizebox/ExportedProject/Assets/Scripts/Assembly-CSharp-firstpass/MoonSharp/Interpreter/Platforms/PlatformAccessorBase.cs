using System;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	public abstract class PlatformAccessorBase : IPlatformAccessor
	{
		public abstract string GetPlatformNamePrefix();

		public string GetPlatformName()
		{
			string text = null;
			text = (PlatformAutoDetector.IsRunningOnUnity ? (PlatformAutoDetector.IsUnityNative ? ("unity." + GetUnityPlatformName().ToLower() + "." + GetUnityRuntimeName()) : ((!PlatformAutoDetector.IsRunningOnMono) ? "unity.dll.unknown" : "unity.dll.mono")) : ((!PlatformAutoDetector.IsRunningOnMono) ? "dotnet" : "mono"));
			if (PlatformAutoDetector.IsPortableFramework)
			{
				text += ".portable";
			}
			text = ((!PlatformAutoDetector.IsRunningOnClr4) ? (text + ".clr2") : (text + ".clr4"));
			if (PlatformAutoDetector.IsRunningOnAOT)
			{
				text += ".aot";
			}
			return GetPlatformNamePrefix() + "." + text;
		}

		private string GetUnityRuntimeName()
		{
			return "mono";
		}

		private string GetUnityPlatformName()
		{
			return "WIN";
		}

		public abstract void DefaultPrint(string content);

		[Obsolete("Replace with DefaultInput(string)")]
		public virtual string DefaultInput()
		{
			return null;
		}

		public virtual string DefaultInput(string prompt)
		{
			return DefaultInput();
		}

		public abstract Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode);

		public abstract Stream IO_GetStandardStream(StandardFileType type);

		public abstract string IO_OS_GetTempFilename();

		public abstract void OS_ExitFast(int exitCode);

		public abstract bool OS_FileExists(string file);

		public abstract void OS_FileDelete(string file);

		public abstract void OS_FileMove(string src, string dst);

		public abstract int OS_Execute(string cmdline);

		public abstract CoreModules FilterSupportedCoreModules(CoreModules module);

		public abstract string GetEnvironmentVariable(string envvarname);

		public virtual bool IsRunningOnAOT()
		{
			return PlatformAutoDetector.IsRunningOnAOT;
		}
	}
}
