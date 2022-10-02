using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Screen
	{
		public static bool fullScreen
		{
			get
			{
				return UnityEngine.Screen.fullScreen;
			}
		}

		public static int height
		{
			get
			{
				return UnityEngine.Screen.height;
			}
		}

		public static int width
		{
			get
			{
				return UnityEngine.Screen.width;
			}
		}
	}
}
