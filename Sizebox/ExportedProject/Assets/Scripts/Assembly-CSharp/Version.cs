using UnityEngine;

public class Version
{
	public const uint Api = 3u;

	public static string GetText()
	{
		return Application.version;
	}

	public static int GetMajorNumber()
	{
		int result;
		if (int.TryParse(Application.version.Substring(0, Application.version.IndexOf('.')), out result))
		{
			return result;
		}
		return -1;
	}

	public static int GetMinorNumber()
	{
		int num = Application.version.IndexOf('.') + 1;
		int num2 = Application.version.IndexOf(' ') - 1;
		if (num2 < 1)
		{
			num2 = Application.version.Length - num;
		}
		int result;
		if (int.TryParse(Application.version.Substring(num, num2), out result))
		{
			return result;
		}
		return -1;
	}
}
