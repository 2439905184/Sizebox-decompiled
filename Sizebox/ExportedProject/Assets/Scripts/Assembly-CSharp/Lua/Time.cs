using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Time
	{
		public static float deltaTime
		{
			get
			{
				return UnityEngine.Time.deltaTime;
			}
		}

		public static float fixedDeltaTime
		{
			get
			{
				return UnityEngine.Time.fixedDeltaTime;
			}
		}

		public static float frameCount
		{
			get
			{
				return UnityEngine.Time.frameCount;
			}
		}

		public static float time
		{
			get
			{
				return UnityEngine.Time.time;
			}
		}

		public static float timeSinceLevelLoad
		{
			get
			{
				return UnityEngine.Time.timeSinceLevelLoad;
			}
		}
	}
}
