using System.Diagnostics;
using UnityEngine;

namespace MMD4Mecanim
{
	public static class M4MDebug
	{
		[Conditional("MMD4MECANIM_DEBUG")]
		public static void Log(string msg)
		{
			UnityEngine.Debug.Log(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void LogWarning(string msg)
		{
			UnityEngine.Debug.LogWarning(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void LogError(string msg)
		{
			UnityEngine.Debug.LogError(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void Assert(bool cmp)
		{
			if (!cmp)
			{
				UnityEngine.Debug.Break();
			}
		}
	}
}
