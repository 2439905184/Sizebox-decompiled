namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Breakpoint
	{
		public bool verified { get; private set; }

		public int line { get; private set; }

		public Breakpoint(bool verified, int line)
		{
			this.verified = verified;
			this.line = line;
		}
	}
}
