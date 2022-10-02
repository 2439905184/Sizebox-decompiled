using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class SettingsViewUtil
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static MatchEvaluator _003C_003E9__0_0;

		internal string _003CCapitalize_003Eb__0_0(Match m)
		{
			return m.Value.ToUpper();
		}
	}

	public static string Capitalize(string str)
	{
		return Regex.Replace(str, "^[a-z]", _003C_003Ec._003C_003E9__0_0 ?? (_003C_003Ec._003C_003E9__0_0 = _003C_003Ec._003C_003E9._003CCapitalize_003Eb__0_0));
	}

	public static char ValidateNumber(char c)
	{
		if (!char.IsDigit(c) && c != '.')
		{
			c = '\0';
		}
		return c;
	}

	public static char ValidateDigit(char c)
	{
		if (!char.IsDigit(c))
		{
			c = '\0';
		}
		return c;
	}
}
