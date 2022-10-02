namespace MoonSharp.VsCodeDebugger.SDK
{
	public class Capabilities : ResponseBody
	{
		public bool supportsConfigurationDoneRequest;

		public bool supportsFunctionBreakpoints;

		public bool supportsConditionalBreakpoints;

		public bool supportsEvaluateForHovers;

		public object[] exceptionBreakpointFilters;
	}
}
