using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class World
	{
		public float gravity
		{
			get
			{
				return 0f - Physics.gravity.y;
			}
			set
			{
				Physics.gravity = UnityEngine.Vector3.down * value;
				Gravity.GravityDensity = value;
			}
		}
	}
}
