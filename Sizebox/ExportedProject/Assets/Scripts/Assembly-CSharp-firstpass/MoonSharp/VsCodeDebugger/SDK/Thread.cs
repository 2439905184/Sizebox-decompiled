namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Thread
	{
		public int id { get; private set; }

		public string name { get; private set; }

		public Thread(int id, string name)
		{
			this.id = id;
			if (name == null || name.Length == 0)
			{
				this.name = string.Format("Thread #{0}", id);
			}
			else
			{
				this.name = name;
			}
		}
	}
}
