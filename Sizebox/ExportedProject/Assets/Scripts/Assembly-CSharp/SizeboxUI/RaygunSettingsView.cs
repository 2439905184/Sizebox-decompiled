using System;
using System.Runtime.CompilerServices;
using Pause;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class RaygunSettingsView : SettingsView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static UnityAction<bool> _003C_003E9__64_0;

			public static UnityAction<float> _003C_003E9__64_15;

			public static UnityAction<float> _003C_003E9__65_0;

			public static UnityAction<int> _003C_003E9__65_1;

			public static UnityAction<float> _003C_003E9__65_2;

			public static UnityAction<float> _003C_003E9__65_3;

			public static UnityAction<float> _003C_003E9__65_4;

			public static UnityAction<bool> _003C_003E9__65_5;

			public static UnityAction<float> _003C_003E9__65_6;

			public static UnityAction<float> _003C_003E9__66_0;

			public static UnityAction<float> _003C_003E9__66_1;

			public static UnityAction<bool> _003C_003E9__66_2;

			public static UnityAction<float> _003C_003E9__66_3;

			public static UnityAction<float> _003C_003E9__67_0;

			public static UnityAction<float> _003C_003E9__67_1;

			public static UnityAction<bool> _003C_003E9__67_2;

			internal void _003CSetupGeneralTab_003Eb__64_0(bool v)
			{
				GlobalPreferences.RaygunScriptMode.value = v;
			}

			internal void _003CSetupGeneralTab_003Eb__64_15(float v)
			{
				GlobalPreferences.AuxiliaryFadeDelay.value = v;
			}

			internal void _003CSetupProjectileTab_003Eb__65_0(float v)
			{
				GlobalPreferences.ProjectileEffectMultiplier.value = v / 4f;
			}

			internal void _003CSetupProjectileTab_003Eb__65_1(int v)
			{
				GlobalPreferences.ProjectileEffectMode.value = v;
			}

			internal void _003CSetupProjectileTab_003Eb__65_2(float v)
			{
				GlobalPreferences.PlayerProjectileSpeed.value = v / 4f;
			}

			internal void _003CSetupProjectileTab_003Eb__65_3(float v)
			{
				GlobalPreferences.ProjectileChargeRate.value = v / 2f;
			}

			internal void _003CSetupProjectileTab_003Eb__65_4(float v)
			{
				GlobalPreferences.PlayerProjectileLifetime.value = (int)v;
			}

			internal void _003CSetupProjectileTab_003Eb__65_5(bool v)
			{
				GlobalPreferences.PlayerProjectileImpactParticles.value = v;
			}

			internal void _003CSetupProjectileTab_003Eb__65_6(float v)
			{
				GlobalPreferences.PlayerProjectileImpactParticlesSizeMult.value = v;
			}

			internal void _003CSetupLaserTab_003Eb__66_0(float v)
			{
				GlobalPreferences.LaserEffectMultiplier.value = v / 4f;
			}

			internal void _003CSetupLaserTab_003Eb__66_1(float v)
			{
				GlobalPreferences.LaserWidth.value = v / 2f;
			}

			internal void _003CSetupLaserTab_003Eb__66_2(bool v)
			{
				GlobalPreferences.LaserImpactParticles.value = v;
			}

			internal void _003CSetupLaserTab_003Eb__66_3(float v)
			{
				GlobalPreferences.LaserImpactParticlesSizeMult.value = v;
			}

			internal void _003CSetupSonicTab_003Eb__67_0(float v)
			{
				GlobalPreferences.SonicEffectMultiplier.value = v / 4f;
			}

			internal void _003CSetupSonicTab_003Eb__67_1(float v)
			{
				GlobalPreferences.SonicWidth.value = v / 2f;
			}

			internal void _003CSetupSonicTab_003Eb__67_2(bool v)
			{
				GlobalPreferences.SonicTagging.value = v;
			}
		}

		private Transform panelTransform;

		private GameObject settingsTab;

		private Button generalTabButton;

		private Button projectileTabButton;

		private Button laserTabButton;

		private Button sonicTabButton;

		private GameObject gameSettings;

		private Toggle scriptMode;

		private Slider aimTargetDist;

		private Slider growColorR;

		private Slider growColorG;

		private Slider growColorB;

		private Slider shrinkColorR;

		private Slider shrinkColorG;

		private Slider shrinkColorB;

		private Slider crosshairColorR;

		private Slider crosshairColorG;

		private Slider crosshairColorB;

		private Slider auxiliaryColorR;

		private Slider auxiliaryColorG;

		private Slider auxiliaryColorB;

		private ToggleSlider auxiliaryFade;

		private Dropdown crosshairImage;

		private string[] crosshairs = new string[10] { "Standard 1", "Standard 2", "Standard 3", "Standard 4", "Standard 5", "Standard 6", "Scifi 1", "Scifi 2", "Arcane 1", "Arcane 2" };

		private Dropdown crosshairOutline;

		private string[] crosshairOutlines = new string[6] { "No Outline", "Black Outline", "White Outline", "Inverse Color Outline", "Darker color Outline", "Lighter Color Outline" };

		private Dropdown polarityBarLocation;

		private string[] polarityBarLocations = new string[3] { "Center", "Bottom-right", "Bottom-left" };

		private Dropdown firingModeBarLocation;

		private string[] firingModeBarLocations = new string[4] { "Center-top", "Center-bottom", "Bottom-right", "Bottom-left" };

		private Slider crosshairScale;

		private Slider auxiliaryScale;

		private Slider projectileEffectMult;

		private Dropdown projectileEffectMode;

		private string[] projectileEffectModes = new string[2] { "Spurt", "Instant" };

		private Slider projectileChargeRate;

		private Slider projectileLifetime;

		private Slider projectileSpeedMult;

		private Toggle projectileImpactParticles;

		private Slider projectileImpactParticlesSizeMult;

		private Toggle projectileGtsMask;

		private Toggle projectileMicroMask;

		private Toggle projectileObjectMask;

		private Slider laserEffectMult;

		private Slider laserWidthMult;

		private Toggle laserImpactParticles;

		private Slider laserImpactParticlesSizeMult;

		private Toggle laserGtsMask;

		private Toggle laserMicroMask;

		private Toggle laserObjectMask;

		private Slider sonicEffectMult;

		private Slider sonicWidthMult;

		private Toggle sonicTagging;

		private Toggle sonicGtsMask;

		private Toggle sonicMicroMask;

		private Toggle sonicObjectMask;

		public GameObject _gameSettings
		{
			set
			{
				gameSettings = value;
			}
		}

		private void Start()
		{
			Button componentInChildren = GetComponentInChildren<Button>();
			componentInChildren.onClick.RemoveAllListeners();
			componentInChildren.onClick.AddListener(_003CStart_003Eb__58_0);
			panelTransform = base.transform.Find("Decoration/Panel");
			settingsTab = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Pause/RaygunSettingsTabs"));
			settingsTab.transform.SetParent(gridGroup.transform, false);
			generalTabButton = settingsTab.transform.Find("GeneralButton").gameObject.GetComponent<Button>();
			generalTabButton.interactable = false;
			projectileTabButton = settingsTab.transform.Find("ProjectileButton").gameObject.GetComponent<Button>();
			laserTabButton = settingsTab.transform.Find("LaserButton").gameObject.GetComponent<Button>();
			sonicTabButton = settingsTab.transform.Find("SonicButton").gameObject.GetComponent<Button>();
			generalTabButton.onClick.AddListener(_003CStart_003Eb__58_1);
			projectileTabButton.onClick.AddListener(_003CStart_003Eb__58_2);
			laserTabButton.onClick.AddListener(_003CStart_003Eb__58_3);
			sonicTabButton.onClick.AddListener(_003CStart_003Eb__58_4);
			OnGeneralTabPressed();
			initialized = true;
		}

		protected override void UpdateValues()
		{
			if (!projectileTabButton.interactable)
			{
				projectileSpeedMult.value = GlobalPreferences.PlayerProjectileSpeed.value * 4f;
			}
			if (!laserTabButton.interactable)
			{
				laserWidthMult.value = GlobalPreferences.LaserWidth.value * 2f;
			}
			if (!sonicTabButton.interactable)
			{
				sonicWidthMult.value = GlobalPreferences.SonicWidth.value * 2f;
			}
		}

		private void OnGeneralTabPressed()
		{
			generalTabButton.interactable = false;
			projectileTabButton.interactable = true;
			laserTabButton.interactable = true;
			sonicTabButton.interactable = true;
			ClearSettingsPanel();
			SetupGeneralTab();
		}

		private void OnProjectileTabPressed()
		{
			generalTabButton.interactable = true;
			projectileTabButton.interactable = false;
			laserTabButton.interactable = true;
			sonicTabButton.interactable = true;
			ClearSettingsPanel();
			SetupProjectileTab();
		}

		private void OnLaserTabPressed()
		{
			generalTabButton.interactable = true;
			projectileTabButton.interactable = true;
			laserTabButton.interactable = false;
			sonicTabButton.interactable = true;
			ClearSettingsPanel();
			SetupLaserTab();
		}

		private void OnSonicTabPressed()
		{
			generalTabButton.interactable = true;
			projectileTabButton.interactable = true;
			laserTabButton.interactable = true;
			sonicTabButton.interactable = false;
			ClearSettingsPanel();
			SetupSonicTab();
		}

		private void SetupGeneralTab()
		{
			AddHeader("");
			scriptMode = AddToggle("Script Mode");
			scriptMode.isOn = GlobalPreferences.RaygunScriptMode.value;
			scriptMode.onValueChanged.AddListener(_003C_003Ec._003C_003E9__64_0 ?? (_003C_003Ec._003C_003E9__64_0 = _003C_003Ec._003C_003E9._003CSetupGeneralTab_003Eb__64_0));
			aimTargetDist = AddSlider("Player Position", -2f, 2f);
			aimTargetDist.value = GlobalPreferences.AimTargetDist.value;
			aimTargetDist.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_1);
			AddHeader("Growing Energy Color");
			growColorR = AddSlider("R", 0f, 1f);
			growColorR.value = GlobalPreferences.GrowColorR.value;
			growColorR.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_2);
			growColorG = AddSlider("G", 0f, 1f);
			growColorG.value = GlobalPreferences.GrowColorG.value;
			growColorG.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_3);
			growColorB = AddSlider("B", 0f, 1f);
			growColorB.value = GlobalPreferences.GrowColorB.value;
			growColorB.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_4);
			AddHeader("Shrinking Energy Color");
			shrinkColorR = AddSlider("R", 0f, 1f);
			shrinkColorR.value = GlobalPreferences.ShrinkColorR.value;
			shrinkColorR.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_5);
			shrinkColorG = AddSlider("G", 0f, 1f);
			shrinkColorG.value = GlobalPreferences.ShrinkColorG.value;
			shrinkColorG.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_6);
			shrinkColorB = AddSlider("B", 0f, 1f);
			shrinkColorB.value = GlobalPreferences.ShrinkColorB.value;
			shrinkColorB.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_7);
			AddHeader("Crosshair Settings");
			crosshairColorR = AddSlider("R", 0f, 1f);
			crosshairColorR.value = GlobalPreferences.CrossHairColorR.value;
			crosshairColorR.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_8);
			crosshairColorG = AddSlider("G", 0f, 1f);
			crosshairColorG.value = GlobalPreferences.CrossHairColorG.value;
			crosshairColorG.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_9);
			crosshairColorB = AddSlider("B", 0f, 1f);
			crosshairColorB.value = GlobalPreferences.CrossHairColorB.value;
			crosshairColorB.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_10);
			crosshairImage = AddDropdown("Crosshair Image", CreateTextOptions(crosshairs, false));
			crosshairImage.value = GlobalPreferences.CrossHairImage.value;
			crosshairImage.onValueChanged.AddListener(SetCrosshairImage);
			crosshairOutline = AddDropdown("Crosshair Outline", CreateTextOptions(crosshairOutlines, false));
			crosshairOutline.value = GlobalPreferences.CrossHairOutline.value;
			crosshairOutline.onValueChanged.AddListener(SetCrosshairOutline);
			crosshairScale = AddSlider("Crosshair Scale", 0.1f, 1.5f);
			crosshairScale.value = GlobalPreferences.UiCrossHairScale.value;
			crosshairScale.onValueChanged.AddListener(OnCrosshairScaleChanged);
			AddHeader("Auxiliary UI Settings");
			auxiliaryColorR = AddSlider("R", 0f, 1f);
			auxiliaryColorR.value = GlobalPreferences.AuxiliaryUIColorR.value;
			auxiliaryColorR.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_11);
			auxiliaryColorG = AddSlider("G", 0f, 1f);
			auxiliaryColorG.value = GlobalPreferences.AuxiliaryUIColorG.value;
			auxiliaryColorG.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_12);
			auxiliaryColorB = AddSlider("B", 0f, 1f);
			auxiliaryColorB.value = GlobalPreferences.AuxiliaryUIColorB.value;
			auxiliaryColorB.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_13);
			auxiliaryFade = AddToggleSlider("Auxiliary Fade", 0.5f, 4f);
			auxiliaryFade.toggle.isOn = GlobalPreferences.AuxiliaryFade.value;
			auxiliaryFade.toggle.onValueChanged.AddListener(_003CSetupGeneralTab_003Eb__64_14);
			auxiliaryFade.slider.value = GlobalPreferences.AuxiliaryFadeDelay.value;
			auxiliaryFade.slider.onValueChanged.AddListener(_003C_003Ec._003C_003E9__64_15 ?? (_003C_003Ec._003C_003E9__64_15 = _003C_003Ec._003C_003E9._003CSetupGeneralTab_003Eb__64_15));
			polarityBarLocation = AddDropdown("Polarity Bar Location", CreateTextOptions(polarityBarLocations, false));
			polarityBarLocation.value = GlobalPreferences.PolarityBarLocation.value;
			polarityBarLocation.onValueChanged.AddListener(SetPolarityBarLocation);
			firingModeBarLocation = AddDropdown("Firing Mode Bar Location", CreateTextOptions(firingModeBarLocations, false));
			firingModeBarLocation.value = GlobalPreferences.FiringModeBarLocation.value;
			firingModeBarLocation.onValueChanged.AddListener(SetFiringModeBarLocation);
			auxiliaryScale = AddSlider("Auxiliary UI Scale", 0.1f, 1.5f);
			auxiliaryScale.value = GlobalPreferences.UIAuxiliaryScale.value;
			auxiliaryScale.onValueChanged.AddListener(OnAuxiliaryScaleChanged);
		}

		private void SetupProjectileTab()
		{
			AddHeader("");
			projectileEffectMult = AddSlider("Effect Multiplier", 1f, 16f);
			projectileEffectMult.value = GlobalPreferences.ProjectileEffectMultiplier.value * 4f;
			projectileEffectMult.wholeNumbers = true;
			projectileEffectMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_0 ?? (_003C_003Ec._003C_003E9__65_0 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_0));
			projectileEffectMode = AddDropdown("Growth Mode", CreateTextOptions(projectileEffectModes, false));
			projectileEffectMode.value = GlobalPreferences.ProjectileEffectMode.value;
			projectileEffectMode.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_1 ?? (_003C_003Ec._003C_003E9__65_1 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_1));
			projectileSpeedMult = AddSlider("Projectile Speed", 1f, 12f);
			projectileSpeedMult.wholeNumbers = true;
			projectileSpeedMult.value = GlobalPreferences.PlayerProjectileSpeed.value * 4f;
			projectileSpeedMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_2 ?? (_003C_003Ec._003C_003E9__65_2 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_2));
			projectileChargeRate = AddSlider("Charge Rate", 1f, 8f);
			projectileChargeRate.wholeNumbers = true;
			projectileChargeRate.value = GlobalPreferences.ProjectileChargeRate.value * 2f;
			projectileChargeRate.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_3 ?? (_003C_003Ec._003C_003E9__65_3 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_3));
			projectileLifetime = AddSlider("Projectile Lifetime", 3f, 10f);
			projectileLifetime.value = GlobalPreferences.PlayerProjectileLifetime.value;
			projectileLifetime.wholeNumbers = true;
			projectileLifetime.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_4 ?? (_003C_003Ec._003C_003E9__65_4 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_4));
			projectileImpactParticles = AddToggle("Display Impact VFX");
			projectileImpactParticles.isOn = GlobalPreferences.PlayerProjectileImpactParticles.value;
			projectileImpactParticles.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_5 ?? (_003C_003Ec._003C_003E9__65_5 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_5));
			projectileImpactParticlesSizeMult = AddSlider("Impact VFX Size", 0.1f, 2f);
			projectileImpactParticlesSizeMult.value = GlobalPreferences.PlayerProjectileImpactParticlesSizeMult.value;
			projectileImpactParticlesSizeMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__65_6 ?? (_003C_003Ec._003C_003E9__65_6 = _003C_003Ec._003C_003E9._003CSetupProjectileTab_003Eb__65_6));
			projectileGtsMask = AddToggle("Affect GTS");
			projectileGtsMask.isOn = GlobalPreferences.PlayerProjectileGtsMask.value;
			projectileGtsMask.onValueChanged.AddListener(_003CSetupProjectileTab_003Eb__65_7);
			projectileMicroMask = AddToggle("Affect Micros");
			projectileMicroMask.isOn = GlobalPreferences.PlayerProjectileMicroMask.value;
			projectileMicroMask.onValueChanged.AddListener(_003CSetupProjectileTab_003Eb__65_8);
			projectileObjectMask = AddToggle("Affect Objects");
			projectileObjectMask.isOn = GlobalPreferences.PlayerProjectileObjectMask.value;
			projectileObjectMask.onValueChanged.AddListener(_003CSetupProjectileTab_003Eb__65_9);
		}

		private void SetupLaserTab()
		{
			AddHeader("");
			laserEffectMult = AddSlider("Effect Multiplier", 1f, 16f);
			laserEffectMult.value = GlobalPreferences.LaserEffectMultiplier.value * 4f;
			laserEffectMult.wholeNumbers = true;
			laserEffectMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__66_0 ?? (_003C_003Ec._003C_003E9__66_0 = _003C_003Ec._003C_003E9._003CSetupLaserTab_003Eb__66_0));
			laserWidthMult = AddSlider("Laser Width", 1f, 30f);
			laserWidthMult.wholeNumbers = true;
			laserWidthMult.value = GlobalPreferences.LaserWidth.value * 2f;
			laserWidthMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__66_1 ?? (_003C_003Ec._003C_003E9__66_1 = _003C_003Ec._003C_003E9._003CSetupLaserTab_003Eb__66_1));
			laserImpactParticles = AddToggle("Display Impact VFX");
			laserImpactParticles.isOn = GlobalPreferences.LaserImpactParticles.value;
			laserImpactParticles.onValueChanged.AddListener(_003C_003Ec._003C_003E9__66_2 ?? (_003C_003Ec._003C_003E9__66_2 = _003C_003Ec._003C_003E9._003CSetupLaserTab_003Eb__66_2));
			laserImpactParticlesSizeMult = AddSlider("Impact VFX Size", 0.1f, 2f);
			laserImpactParticlesSizeMult.value = GlobalPreferences.LaserImpactParticlesSizeMult.value;
			laserImpactParticlesSizeMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__66_3 ?? (_003C_003Ec._003C_003E9__66_3 = _003C_003Ec._003C_003E9._003CSetupLaserTab_003Eb__66_3));
			laserGtsMask = AddToggle("Affect GTS");
			laserGtsMask.isOn = GlobalPreferences.LaserGtsMask.value;
			laserGtsMask.onValueChanged.AddListener(_003CSetupLaserTab_003Eb__66_4);
			laserMicroMask = AddToggle("Affect Micros");
			laserMicroMask.isOn = GlobalPreferences.LaserMicroMask.value;
			laserMicroMask.onValueChanged.AddListener(_003CSetupLaserTab_003Eb__66_5);
			laserObjectMask = AddToggle("Affect Objects");
			laserObjectMask.isOn = GlobalPreferences.LaserObjectMask.value;
			laserObjectMask.onValueChanged.AddListener(_003CSetupLaserTab_003Eb__66_6);
		}

		private void SetupSonicTab()
		{
			AddHeader("");
			sonicEffectMult = AddSlider("Effect Multiplier", 1f, 16f);
			sonicEffectMult.value = GlobalPreferences.SonicEffectMultiplier.value * 4f;
			sonicEffectMult.wholeNumbers = true;
			sonicEffectMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__67_0 ?? (_003C_003Ec._003C_003E9__67_0 = _003C_003Ec._003C_003E9._003CSetupSonicTab_003Eb__67_0));
			sonicWidthMult = AddSlider("Sonic Width", 1f, 6f);
			sonicWidthMult.wholeNumbers = true;
			sonicWidthMult.value = GlobalPreferences.SonicWidth.value * 2f;
			sonicWidthMult.onValueChanged.AddListener(_003C_003Ec._003C_003E9__67_1 ?? (_003C_003Ec._003C_003E9__67_1 = _003C_003Ec._003C_003E9._003CSetupSonicTab_003Eb__67_1));
			sonicTagging = AddToggle("Continous Tagging");
			sonicTagging.isOn = GlobalPreferences.SonicTagging.value;
			sonicTagging.onValueChanged.AddListener(_003C_003Ec._003C_003E9__67_2 ?? (_003C_003Ec._003C_003E9__67_2 = _003C_003Ec._003C_003E9._003CSetupSonicTab_003Eb__67_2));
			sonicGtsMask = AddToggle("Affect GTS");
			sonicGtsMask.isOn = GlobalPreferences.SonicGtsMask.value;
			sonicGtsMask.onValueChanged.AddListener(_003CSetupSonicTab_003Eb__67_3);
			sonicMicroMask = AddToggle("Affect Micros");
			sonicMicroMask.isOn = GlobalPreferences.SonicMicroMask.value;
			sonicMicroMask.onValueChanged.AddListener(_003CSetupSonicTab_003Eb__67_4);
			sonicObjectMask = AddToggle("Affect Objects");
			sonicObjectMask.isOn = GlobalPreferences.SonicObjectMask.value;
			sonicObjectMask.onValueChanged.AddListener(_003CSetupSonicTab_003Eb__67_5);
		}

		private void ClearSettingsPanel()
		{
			for (int i = 1; i < panelTransform.childCount; i++)
			{
				UnityEngine.Object.Destroy(panelTransform.GetChild(i).gameObject);
			}
		}

		private new void ClosePanel()
		{
			gameSettings.SetActive(true);
			base.gameObject.SetActive(false);
		}

		private void SetAimTargetDist(float v)
		{
			GlobalPreferences.AimTargetDist.value = v;
		}

		private void OnGrowColorChanged()
		{
			GlobalPreferences.GrowColorR.value = growColorR.value;
			GlobalPreferences.GrowColorG.value = growColorG.value;
			GlobalPreferences.GrowColorB.value = growColorB.value;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshColors();
			}
		}

		private void OnShrinkColorChanged()
		{
			GlobalPreferences.ShrinkColorR.value = shrinkColorR.value;
			GlobalPreferences.ShrinkColorG.value = shrinkColorG.value;
			GlobalPreferences.ShrinkColorB.value = shrinkColorB.value;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshColors();
			}
		}

		private void OnCrosshairColorChanged()
		{
			GlobalPreferences.CrossHairColorR.value = crosshairColorR.value;
			GlobalPreferences.CrossHairColorG.value = crosshairColorG.value;
			GlobalPreferences.CrossHairColorB.value = crosshairColorB.value;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void SetCrosshairImage(int v)
		{
			GlobalPreferences.CrossHairImage.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void SetCrosshairOutline(int v)
		{
			GlobalPreferences.CrossHairOutline.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void OnCrosshairScaleChanged(float v)
		{
			GlobalPreferences.UiCrossHairScale.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void OnAuxiliaryColorChanged()
		{
			GlobalPreferences.AuxiliaryUIColorR.value = auxiliaryColorR.value;
			GlobalPreferences.AuxiliaryUIColorG.value = auxiliaryColorG.value;
			GlobalPreferences.AuxiliaryUIColorB.value = auxiliaryColorB.value;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.ChangeAuxiliaryUIColor(new Color(auxiliaryColorR.value, auxiliaryColorG.value, auxiliaryColorB.value));
			}
		}

		private void OnToggleFade(bool v)
		{
			GlobalPreferences.AuxiliaryFade.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.ResetUIFade();
			}
		}

		private void SetPolarityBarLocation(int v)
		{
			GlobalPreferences.PolarityBarLocation.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void SetFiringModeBarLocation(int v)
		{
			GlobalPreferences.FiringModeBarLocation.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void OnAuxiliaryScaleChanged(float v)
		{
			GlobalPreferences.UIAuxiliaryScale.value = v;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.RefreshUI();
			}
		}

		private void OnProjectileMaskChanged()
		{
			GlobalPreferences.PlayerProjectileGtsMask.value = projectileGtsMask.isOn;
			GlobalPreferences.PlayerProjectileMicroMask.value = projectileMicroMask.isOn;
			GlobalPreferences.PlayerProjectileObjectMask.value = projectileObjectMask.isOn;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.SetupProjectileMask();
			}
		}

		private void OnLaserMaskChanged()
		{
			GlobalPreferences.LaserGtsMask.value = laserGtsMask.isOn;
			GlobalPreferences.LaserMicroMask.value = laserMicroMask.isOn;
			GlobalPreferences.LaserObjectMask.value = laserObjectMask.isOn;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.SetupLaserMask();
			}
		}

		private void OnSonicMaskChanged()
		{
			GlobalPreferences.SonicGtsMask.value = sonicGtsMask.isOn;
			GlobalPreferences.SonicMicroMask.value = sonicMicroMask.isOn;
			GlobalPreferences.SonicObjectMask.value = sonicObjectMask.isOn;
			if (PlayerRaygun.instance != null)
			{
				PlayerRaygun.instance.SetupSonicMask();
			}
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__58_0()
		{
			ClosePanel();
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__58_1()
		{
			OnGeneralTabPressed();
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__58_2()
		{
			OnProjectileTabPressed();
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__58_3()
		{
			OnLaserTabPressed();
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__58_4()
		{
			OnSonicTabPressed();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_1(float v)
		{
			SetAimTargetDist(v);
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_2(float v)
		{
			OnGrowColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_3(float v)
		{
			OnGrowColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_4(float v)
		{
			OnGrowColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_5(float v)
		{
			OnShrinkColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_6(float v)
		{
			OnShrinkColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_7(float v)
		{
			OnShrinkColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_8(float v)
		{
			OnCrosshairColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_9(float v)
		{
			OnCrosshairColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_10(float v)
		{
			OnCrosshairColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_11(float v)
		{
			OnAuxiliaryColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_12(float v)
		{
			OnAuxiliaryColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_13(float v)
		{
			OnAuxiliaryColorChanged();
		}

		[CompilerGenerated]
		private void _003CSetupGeneralTab_003Eb__64_14(bool v)
		{
			OnToggleFade(v);
		}

		[CompilerGenerated]
		private void _003CSetupProjectileTab_003Eb__65_7(bool v)
		{
			OnProjectileMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupProjectileTab_003Eb__65_8(bool v)
		{
			OnProjectileMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupProjectileTab_003Eb__65_9(bool v)
		{
			OnProjectileMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupLaserTab_003Eb__66_4(bool v)
		{
			OnLaserMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupLaserTab_003Eb__66_5(bool v)
		{
			OnLaserMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupLaserTab_003Eb__66_6(bool v)
		{
			OnLaserMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupSonicTab_003Eb__67_3(bool v)
		{
			OnSonicMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupSonicTab_003Eb__67_4(bool v)
		{
			OnSonicMaskChanged();
		}

		[CompilerGenerated]
		private void _003CSetupSonicTab_003Eb__67_5(bool v)
		{
			OnSonicMaskChanged();
		}
	}
}
