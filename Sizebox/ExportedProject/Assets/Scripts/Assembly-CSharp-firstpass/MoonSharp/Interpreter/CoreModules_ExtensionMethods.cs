namespace MoonSharp.Interpreter
{
	internal static class CoreModules_ExtensionMethods
	{
		public static bool Has(this CoreModules val, CoreModules flag)
		{
			return (val & flag) == flag;
		}
	}
}
