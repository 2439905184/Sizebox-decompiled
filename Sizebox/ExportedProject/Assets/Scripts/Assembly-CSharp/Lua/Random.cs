using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public static class Random
	{
		public static Vector3 insideUnitCircle
		{
			get
			{
				return new Vector3(UnityEngine.Random.insideUnitCircle);
			}
		}

		public static Vector3 insideUnitSphere
		{
			get
			{
				return new Vector3(UnityEngine.Random.insideUnitSphere);
			}
		}

		public static Vector3 onUnitSphere
		{
			get
			{
				return new Vector3(UnityEngine.Random.onUnitSphere);
			}
		}

		public static Quaternion rotation
		{
			get
			{
				return new Quaternion(UnityEngine.Random.rotation);
			}
		}

		public static Quaternion rotationUniform
		{
			get
			{
				return new Quaternion(UnityEngine.Random.rotationUniform);
			}
		}

		public static float value
		{
			get
			{
				return UnityEngine.Random.value;
			}
		}

		public static void InitState(int seed)
		{
			UnityEngine.Random.InitState(seed);
		}

		public static float Range(int min, int max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static float Range(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}
	}
}
