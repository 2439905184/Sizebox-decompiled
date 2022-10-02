using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class LuaPlayerRaygun
	{
		private PlayerRaygun raygun;

		public bool firingEnabled
		{
			get
			{
				return raygun.GetScriptEnableRaygun();
			}
			set
			{
				raygun.SetScriptEnableRaygun(value);
			}
		}

		[MoonSharpHidden]
		public LuaPlayerRaygun(global::Player player)
		{
			raygun = player.GetRaygun();
		}

		public void SetGrowEnergyColor(int r, int g, int b)
		{
			raygun.SetScriptGrowColor(r, g, b);
		}

		public void ResetGrowEnergyColor()
		{
			raygun.ClearScriptGrowColor();
		}

		public void SetShrinkEnergyColor(int r, int g, int b)
		{
			raygun.SetScriptShrinkColor(r, g, b);
		}

		public void ResetShrinkEnergyColor()
		{
			raygun.ClearScriptGrowColor();
		}
	}
}
