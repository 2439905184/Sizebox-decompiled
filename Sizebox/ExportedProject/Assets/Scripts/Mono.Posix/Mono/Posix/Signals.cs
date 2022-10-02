using System;

namespace Mono.Posix
{
	[CLSCompliant(false)]
	[Obsolete("Use Mono.Unix.Native.Signum")]
	public enum Signals
	{
		SIGHUP = 0,
		SIGINT = 1,
		SIGQUIT = 2,
		SIGILL = 3,
		SIGTRAP = 4,
		SIGABRT = 5,
		SIGBUS = 6,
		SIGFPE = 7,
		SIGKILL = 8,
		SIGUSR1 = 9,
		SIGSEGV = 10,
		SIGUSR2 = 11,
		SIGPIPE = 12,
		SIGALRM = 13,
		SIGTERM = 14,
		SIGCHLD = 15,
		SIGCONT = 16,
		SIGSTOP = 17,
		SIGTSTP = 18,
		SIGTTIN = 19,
		SIGTTOU = 20,
		SIGURG = 21,
		SIGXCPU = 22,
		SIGXFSZ = 23,
		SIGVTALRM = 24,
		SIGPROF = 25,
		SIGWINCH = 26,
		SIGIO = 27,
		SIGSYS = 28
	}
}
