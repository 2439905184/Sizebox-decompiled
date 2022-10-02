using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.VsCodeDebugger.SDK
{
	internal class Utilities
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0
		{
			public bool underscoredOnly;

			public Type type;

			public object variables;

			internal string _003CExpandVariables_003Eb__0(Match match)
			{
				string value = match.Groups[1].Value;
				if (!underscoredOnly || value.StartsWith("_"))
				{
					PropertyInfo property = Framework.Do.GetProperty(type, value);
					if (property != null)
					{
						return property.GetValue(variables, null).ToString();
					}
					return "{" + value + ": not found}";
				}
				return match.Groups[0].Value;
			}
		}

		private static readonly Regex VARIABLE = new Regex("\\{(\\w+)\\}");

		public static string ExpandVariables(string format, object variables, bool underscoredOnly = true)
		{
			_003C_003Ec__DisplayClass1_0 _003C_003Ec__DisplayClass1_ = new _003C_003Ec__DisplayClass1_0();
			_003C_003Ec__DisplayClass1_.underscoredOnly = underscoredOnly;
			_003C_003Ec__DisplayClass1_.variables = variables;
			if (_003C_003Ec__DisplayClass1_.variables == null)
			{
				_003C_003Ec__DisplayClass1_.variables = new _003C_003Ef__AnonymousType7();
			}
			_003C_003Ec__DisplayClass1_.type = _003C_003Ec__DisplayClass1_.variables.GetType();
			return VARIABLE.Replace(format, _003C_003Ec__DisplayClass1_._003CExpandVariables_003Eb__0);
		}

		public static string MakeRelativePath(string dirPath, string absPath)
		{
			if (!dirPath.EndsWith("/"))
			{
				dirPath += "/";
			}
			if (absPath.StartsWith(dirPath))
			{
				return absPath.Replace(dirPath, "");
			}
			return absPath;
		}
	}
}
