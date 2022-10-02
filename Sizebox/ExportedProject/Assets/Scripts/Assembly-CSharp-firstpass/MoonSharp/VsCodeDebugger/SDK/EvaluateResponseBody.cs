namespace MoonSharp.VsCodeDebugger.SDK
{
	public class EvaluateResponseBody : ResponseBody
	{
		public string result { get; private set; }

		public string type { get; set; }

		public int variablesReference { get; private set; }

		public EvaluateResponseBody(string value, int reff = 0)
		{
			result = value;
			variablesReference = reff;
		}
	}
}
