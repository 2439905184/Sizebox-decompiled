using System;
using System.Runtime.CompilerServices;
using Pause;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class AIGunSettingsView : SettingsView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static UnityAction<bool> _003C_003E9__22_0;

			public static UnityAction<bool> _003C_003E9__22_1;

			public static UnityAction<bool> _003C_003E9__22_2;

			public static UnityAction<bool> _003C_003E9__22_3;

			public static UnityAction<bool> _003C_003E9__22_4;

			public static UnityAction<float> _003C_003E9__22_5;

			public static UnityAction<bool> _003C_003E9__22_6;

			public static UnityAction<bool> _003C_003E9__22_7;

			public static UnityAction<bool> _003C_003E9__22_8;

			public static UnityAction<float> _003C_003E9__22_9;

			public static UnityAction<float> _003C_003E9__22_10;

			public static UnityAction<bool> _003C_003E9__22_11;

			public static UnityAction<float> _003C_003E9__22_12;

			public static UnityAction<float> _003C_003E9__22_13;

			internal void _003CStart_003Eb__22_0(bool value)
			{
				GlobalPreferences.AIProjectileGtsMask.value = value;
			}

			internal void _003CStart_003Eb__22_1(bool value)
			{
				GlobalPreferences.AIProjectileMicroMask.value = value;
			}

			internal void _003CStart_003Eb__22_2(bool value)
			{
				GlobalPreferences.AIProjectileObjectMask.value = value;
			}

			internal void _003CStart_003Eb__22_3(bool value)
			{
				GlobalPreferences.AIProjectilePlayerMask.value = value;
			}

			internal void _003CStart_003Eb__22_4(bool value)
			{
				GlobalPreferences.AIAccurateShooting.value = value;
			}

			internal void _003CStart_003Eb__22_5(float value)
			{
				GlobalPreferences.AIInaccuracyFactor.value = (int)value;
			}

			internal void _003CStart_003Eb__22_6(bool value)
			{
				GlobalPreferences.AIPredictiveAiming.value = value;
			}

			internal void _003CStart_003Eb__22_7(bool value)
			{
				GlobalPreferences.AIRandomIntervals.value = value;
			}

			internal void _003CStart_003Eb__22_8(bool value)
			{
				GlobalPreferences.AIBurstFire.value = value;
			}

			internal void _003CStart_003Eb__22_9(float v)
			{
				GlobalPreferences.AIProjectileSpeed.value = v / 4f;
			}

			internal void _003CStart_003Eb__22_10(float v)
			{
				GlobalPreferences.AIProjectileLifetime.value = (int)v;
			}

			internal void _003CStart_003Eb__22_11(bool v)
			{
				GlobalPreferences.AIProjectileImpactParticles.value = v;
			}

			internal void _003CStart_003Eb__22_12(float v)
			{
				GlobalPreferences.AIProjectileImpactParticlesSizeMult.value = v;
			}

			internal void _003CStart_003Eb__22_13(float value)
			{
				GlobalPreferences.AIRaygunColorR.value = value;
			}
		}

		private Toggle projectileGtsMask;

		private Toggle projectileMicroMask;

		private Toggle projectileObjectMask;

		private Toggle projectilePlayerMask;

		private Slider raygunColorR;

		private Slider raygunColorG;

		private Slider raygunColorB;

		private Slider smgColorR;

		private Slider smgColorG;

		private Slider smgColorB;

		private Toggle accurateAim;

		private Slider inaccuracyFactor;

		private Toggle predictiveAim;

		private Toggle randomIntervals;

		private Toggle burstFire;

		private Slider projectileLifetime;

		private Slider projectileSpeedMult;

		private Toggle projectileImpactParticles;

		private Slider projectileImpactParticlesSizeMult;

		private GameObject gameSettings;

		public GameObject _gameSettings
		{
			set
			{
				gameSettings = value;
			}
		}

		private void Start()
		{
			AddHeader("AI Projectile Collision");
			projectileGtsMask = AddToggle("Collide with GTS");
			projectileGtsMask.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_0 ?? (_003C_003Ec._003C_003E9__22_0 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_0));
			projectileMicroMask = AddToggle("Collide with Micros");
			projectileMicroMask.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_1 ?? (_003C_003Ec._003C_003E9__22_1 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_1));
			projectileObjectMask = AddToggle("Collide with Objects");
			projectileObjectMask.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_2 ?? (_003C_003Ec._003C_003E9__22_2 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_2));
			projectilePlayerMask = AddToggle("Collide with Player");
			projectilePlayerMask.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_3 ?? (_003C_003Ec._003C_003E9__22_3 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_3));
			AddHeader("Default values");
			accurateAim = AddToggle("Accurate Shooting (No Spread)");
			accurateAim.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_4 ?? (_003C_003Ec._003C_003E9__22_4 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_4));
			inaccuracyFactor = AddSlider("Inaccuracy Factor", 1f, 30f);
			inaccuracyFactor.wholeNumbers = true;
			inaccuracyFactor.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_5 ?? (_003C_003Ec._003C_003E9__22_5 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_5));
			predictiveAim = AddToggle("Predictive Aiming (Not Perfect)");
			predictiveAim.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_6 ?? (_003C_003Ec._003C_003E9__22_6 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_6));
			randomIntervals = AddToggle("Random Intervals Added (Prevent Syncing)");
			randomIntervals.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_7 ?? (_003C_003Ec._003C_003E9__22_7 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_7));
			burstFire = AddToggle("Burst Fire By Default");
			burstFire.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_8 ?? (_003C_003Ec._003C_003E9__22_8 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_8));
			projectileSpeedMult = AddSlider("Projectile Speed", 1f, 12f);
			projectileSpeedMult.wholeNumbers = true;
			projectileSpeedMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_9 ?? (_003C_003Ec._003C_003E9__22_9 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_9));
			projectileLifetime = AddSlider("Projectile Lifetime", 3f, 10f);
			projectileLifetime.wholeNumbers = true;
			projectileLifetime.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_10 ?? (_003C_003Ec._003C_003E9__22_10 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_10));
			projectileImpactParticles = AddToggle("Display Impact VFX");
			projectileImpactParticles.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_11 ?? (_003C_003Ec._003C_003E9__22_11 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_11));
			projectileImpactParticlesSizeMult = AddSlider("Impact VFX Size", 0.1f, 2f);
			projectileImpactParticlesSizeMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_12 ?? (_003C_003Ec._003C_003E9__22_12 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_12));
			AddHeader("AI Raygun Projectile Color");
			raygunColorR = AddSlider("R", 0f, 1f);
			raygunColorR.onValueChanged.AddListener(_003C_003Ec._003C_003E9__22_13 ?? (_003C_003Ec._003C_003E9__22_13 = _003C_003Ec._003C_003E9._003CStart_003Eb__22_13));
			raygunColorG = AddSlider("G", 0f, 1f);
			raygunColorG.value = GlobalPreferences.AIRaygunColorG.value;
			raygunColorB = AddSlider("B", 0f, 1f);
			raygunColorB.value = GlobalPreferences.AIRaygunColorB.value;
			AddHeader("AI SMG Projectile Color");
			smgColorR = AddSlider("R", 0f, 1f);
			smgColorR.value = GlobalPreferences.AiSmgColorR.value;
			smgColorG = AddSlider("G", 0f, 1f);
			smgColorG.value = GlobalPreferences.AiSmgColorG.value;
			smgColorB = AddSlider("B", 0f, 1f);
			smgColorB.value = GlobalPreferences.AiSmgColorB.value;
			UpdateValues();
			initialized = true;
		}

		protected override void UpdateValues()
		{
			projectileGtsMask.isOn = GlobalPreferences.AIProjectileGtsMask.value;
			projectileMicroMask.isOn = GlobalPreferences.AIProjectileMicroMask.value;
			projectileObjectMask.isOn = GlobalPreferences.AIProjectileObjectMask.value;
			projectilePlayerMask.isOn = GlobalPreferences.AIProjectilePlayerMask.value;
			accurateAim.isOn = GlobalPreferences.AIAccurateShooting.value;
			inaccuracyFactor.value = GlobalPreferences.AIInaccuracyFactor.value;
			predictiveAim.isOn = GlobalPreferences.AIPredictiveAiming.value;
			randomIntervals.isOn = GlobalPreferences.AIRandomIntervals.value;
			burstFire.isOn = GlobalPreferences.AIBurstFire.value;
			projectileLifetime.value = GlobalPreferences.AIProjectileLifetime.value;
			projectileSpeedMult.value = GlobalPreferences.AIProjectileSpeed.value * 4f;
			projectileImpactParticles.isOn = GlobalPreferences.AIProjectileImpactParticles.value;
			projectileImpactParticlesSizeMult.value = GlobalPreferences.AIProjectileImpactParticlesSizeMult.value;
			raygunColorR.value = GlobalPreferences.AIRaygunColorR.value;
			raygunColorG.value = GlobalPreferences.AIRaygunColorG.value;
			raygunColorB.value = GlobalPreferences.AIRaygunColorB.value;
			smgColorR.value = GlobalPreferences.AiSmgColorR.value;
			smgColorG.value = GlobalPreferences.AiSmgColorG.value;
			smgColorB.value = GlobalPreferences.AiSmgColorB.value;
		}
	}
}
