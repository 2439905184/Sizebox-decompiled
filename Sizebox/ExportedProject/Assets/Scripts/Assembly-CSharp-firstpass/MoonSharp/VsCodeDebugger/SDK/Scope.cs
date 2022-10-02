namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Scope
	{
		public string name { get; private set; }

		public int variablesReference { get; private set; }

		public bool expensive { get; private set; }

		public Scope(string name, int variablesReference, bool expensive = false)
		{
			this.name = name;
			this.variablesReference = variablesReference;
			this.expensive = expensive;
		}
	}
}
