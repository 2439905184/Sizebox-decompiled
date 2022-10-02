using UnityEngine;

public static class DebugHelper
{
	private static float startTime;

	public static void StartCron()
	{
		startTime = Time.realtimeSinceStartup;
	}

	public static void LogCron(string msg)
	{
		float num = (Time.realtimeSinceStartup - startTime) * 1000f;
		Debug.Log(msg + ": " + num + " ms");
	}

	public static void LogAndRestart(string msg)
	{
		LogCron(msg);
		StartCron();
	}
}
