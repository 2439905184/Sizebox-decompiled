namespace MoonSharp.Interpreter
{
	public enum InteropAccessMode
	{
		Reflection = 0,
		LazyOptimized = 1,
		Preoptimized = 2,
		BackgroundOptimized = 3,
		Hardwired = 4,
		HideMembers = 5,
		NoReflectionAllowed = 6,
		Default = 7
	}
}
