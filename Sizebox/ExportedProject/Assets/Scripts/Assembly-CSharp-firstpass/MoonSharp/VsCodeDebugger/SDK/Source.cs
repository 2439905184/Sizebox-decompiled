using System.IO;

namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Source
	{
		public string name { get; private set; }

		public string path { get; private set; }

		public int sourceReference { get; private set; }

		public Source(string name, string path, int sourceReference = 0)
		{
			this.name = name;
			this.path = path;
			this.sourceReference = sourceReference;
		}

		public Source(string path, int sourceReference = 0)
		{
			name = Path.GetFileName(path);
			this.path = path;
			this.sourceReference = sourceReference;
		}
	}
}
