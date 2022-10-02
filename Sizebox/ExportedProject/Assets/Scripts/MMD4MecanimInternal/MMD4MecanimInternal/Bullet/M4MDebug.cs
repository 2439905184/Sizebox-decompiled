using System.Diagnostics;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public static class M4MDebug
	{
		[Conditional("MMD4MECANIM_DEBUG")]
		public static void Log(string msg)
		{
			Debug.Log(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void LogWarning(string msg)
		{
			Debug.LogWarning(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void LogError(string msg)
		{
			Debug.LogError(msg);
		}

		[Conditional("MMD4MECANIM_DEBUG")]
		public static void Assert(bool cmp)
		{
			if (!cmp)
			{
				Debug.Break();
			}
		}
	}
}
