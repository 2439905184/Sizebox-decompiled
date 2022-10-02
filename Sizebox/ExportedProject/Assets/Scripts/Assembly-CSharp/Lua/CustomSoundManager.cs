using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public static class CustomSoundManager
	{
		public static void SetPlayerRaygunArmingSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunArmingSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunArmingSound(clip);
			}
		}

		public static void SetPlayerRaygunDisarmingSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunDisarmingSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunDisarmingSound(clip);
			}
		}

		public static void SetPlayerRaygunModeSwitchSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunModeSwitchSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunModeSwitchSound(clip);
			}
		}

		public static void SetPlayerRaygunUtilitySFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunUtilitySound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunUtilitySound(clip);
			}
		}

		public static void SetPlayerRaygunPolaritySFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunPolaritySound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunPolaritySound(clip);
			}
		}

		public static void SetPlayerRaygunProjectileFireSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunProjFireSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunProjFireSound(clip);
			}
		}

		public static void SetPlayerRaygunProjectileImpactSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunProjImpactSounds();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunProjImpactSound(clip);
			}
		}

		public static void SetPlayerRaygunLaserSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunLaserSustainSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunLaserSustainSound(clip);
			}
		}

		public static void SetPlayerRaygunSonicFireSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunSonicFireSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunSonicFireSound(clip);
			}
		}

		public static void SetPlayerRaygunSonicSustainSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetPlayerRaygunSonicSustainSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetPlayerRaygunSonicSustainSound(clip);
			}
		}

		public static void SetNpcRaygunProjectileFireSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetNpcRaygunProjFireSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetNpcRaygunProjFireSound(clip);
			}
		}

		public static void SetNpcRaygunProjectileImpactSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetNpcRaygunProjImpactSounds();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetNpcRaygunProjImpactSound(clip);
			}
		}

		public static void SetNpcSmgProjectileFireSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetNpcSmgProjFireSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetNpcSmgProjFireSound(clip);
			}
		}

		public static void SetNpcSmgProjectileImpactSFX(string clip)
		{
			if (clip == null)
			{
				SoundManager.Instance.ResetNpcSmgProjImpactSound();
			}
			else if (clip != "")
			{
				SoundManager.Instance.SetNpcSmgProjImpactSound(clip);
			}
		}
	}
}
