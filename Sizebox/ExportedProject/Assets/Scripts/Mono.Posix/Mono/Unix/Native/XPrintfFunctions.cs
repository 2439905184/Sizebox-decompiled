namespace Mono.Unix.Native
{
	internal class XPrintfFunctions
	{
		internal delegate object XPrintf(object[] parameters);

		internal static XPrintf printf;

		internal static XPrintf fprintf;

		internal static XPrintf snprintf;

		internal static XPrintf syslog;

		static XPrintfFunctions()
		{
			printf = new CdeclFunction("msvcrt", "printf", typeof(int)).Invoke;
			fprintf = new CdeclFunction("msvcrt", "fprintf", typeof(int)).Invoke;
			snprintf = new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_snprintf", typeof(int)).Invoke;
			syslog = new CdeclFunction("MonoPosixHelper", "Mono_Posix_Stdlib_syslog2", typeof(int)).Invoke;
		}
	}
}
