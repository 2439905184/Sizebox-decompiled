using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixProcess
	{
		private int pid;

		public int Id
		{
			get
			{
				return pid;
			}
		}

		public bool HasExited
		{
			get
			{
				return Syscall.WIFEXITED(GetProcessStatus());
			}
		}

		public int ExitCode
		{
			get
			{
				if (!HasExited)
				{
					throw new InvalidOperationException(Locale.GetText("Process hasn't exited"));
				}
				return Syscall.WEXITSTATUS(GetProcessStatus());
			}
		}

		public bool HasSignaled
		{
			get
			{
				return Syscall.WIFSIGNALED(GetProcessStatus());
			}
		}

		public Signum TerminationSignal
		{
			get
			{
				if (!HasSignaled)
				{
					throw new InvalidOperationException(Locale.GetText("Process wasn't terminated by a signal"));
				}
				return Syscall.WTERMSIG(GetProcessStatus());
			}
		}

		public bool HasStopped
		{
			get
			{
				return Syscall.WIFSTOPPED(GetProcessStatus());
			}
		}

		public Signum StopSignal
		{
			get
			{
				if (!HasStopped)
				{
					throw new InvalidOperationException(Locale.GetText("Process isn't stopped"));
				}
				return Syscall.WSTOPSIG(GetProcessStatus());
			}
		}

		public int ProcessGroupId
		{
			get
			{
				return Syscall.getpgid(pid);
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setpgid(pid, value));
			}
		}

		public int SessionId
		{
			get
			{
				int num = Syscall.getsid(pid);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
				return num;
			}
		}

		internal UnixProcess(int pid)
		{
			this.pid = pid;
		}

		private int GetProcessStatus()
		{
			int status;
			int num = Syscall.waitpid(pid, out status, WaitOptions.WNOHANG | WaitOptions.WUNTRACED);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return num;
		}

		public static UnixProcess GetCurrentProcess()
		{
			return new UnixProcess(GetCurrentProcessId());
		}

		public static int GetCurrentProcessId()
		{
			return Syscall.getpid();
		}

		public void Kill()
		{
			Signal(Signum.SIGKILL);
		}

		[CLSCompliant(false)]
		public void Signal(Signum signal)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.kill(pid, signal));
		}

		public void WaitForExit()
		{
			int num;
			do
			{
				int status;
				num = Syscall.waitpid(pid, out status, (WaitOptions)0);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}
	}
}
