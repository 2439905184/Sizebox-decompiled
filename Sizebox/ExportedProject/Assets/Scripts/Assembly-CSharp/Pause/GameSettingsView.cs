using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pause
{
	public class GameSettingsView : SettingsView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static UnityAction<bool> _003C_003E9__35_1;

			public static UnityAction<bool> _003C_003E9__35_2;

			public static UnityAction<bool> _003C_003E9__35_3;

			public static UnityAction<bool> _003C_003E9__35_4;

			public static UnityAction<bool> _003C_003E9__35_9;

			public static UnityAction<bool> _003C_003E9__35_10;

			public static UnityAction<bool> _003C_003E9__35_11;

			public static UnityAction<bool> _003C_003E9__35_12;

			public static UnityAction<bool> _003C_003E9__35_13;

			public static UnityAction<bool> _003C_003E9__35_14;

			public static UnityAction<bool> _003C_003E9__35_15;

			public static UnityAction<bool> _003C_003E9__35_16;

			public static UnityAction<bool> _003C_003E9__35_17;

			public static UnityAction<bool> _003C_003E9__35_18;

			public static UnityAction<bool> _003C_003E9__35_19;

			public static UnityAction<bool> _003C_003E9__35_20;

			public static UnityAction<bool> _003C_003E9__35_21;

			public static UnityAction<bool> _003C_003E9__35_22;

			public static UnityAction<float> _003C_003E9__35_23;

			public static UnityAction<float> _003C_003E9__35_24;

			public static UnityAction<float> _003C_003E9__35_25;

			public static UnityAction<float> _003C_003E9__35_28;

			public static UnityAction<int> _003C_003E9__35_29;

			public static UnityAction<bool> _003C_003E9__35_30;

			public static UnityAction<bool> _003C_003E9__35_31;

			public static UnityAction<bool> _003C_003E9__35_32;

			public static UnityAction<bool> _003C_003E9__35_33;

			public static UnityAction<bool> _003C_003E9__35_34;

			public static UnityAction<bool> _003C_003E9__35_35;

			public static UnityAction<bool> _003C_003E9__35_40;

			public static UnityAction<float> _003C_003E9__35_41;

			public static UnityAction<bool> _003C_003E9__35_45;

			public static UnityAction<bool> _003C_003E9__35_46;

			public static UnityAction<bool> _003C_003E9__35_47;

			public static UnityAction<bool> _003C_003E9__35_48;

			public static UnityAction<bool> _003C_003E9__35_49;

			public static InputField.OnValidateInput _003C_003E9__35_53;

			public static InputField.OnValidateInput _003C_003E9__35_54;

			public static UnityAction<bool> _003C_003E9__35_52;

			internal void _003CStart_003Eb__35_1(bool value)
			{
				GlobalPreferences.GtsAI.value = value;
			}

			internal void _003CStart_003Eb__35_2(bool value)
			{
				GlobalPreferences.MicroAI.value = value;
			}

			internal void _003CStart_003Eb__35_3(bool value)
			{
				GlobalPreferences.IgnorePlayer.value = value;
			}

			internal void _003CStart_003Eb__35_4(bool value)
			{
				GlobalPreferences.LookAtPlayer.value = value;
			}

			internal void _003CStart_003Eb__35_9(bool value)
			{
				GlobalPreferences.LowEndCities.value = value;
			}

			internal void _003CStart_003Eb__35_10(bool value)
			{
				GlobalPreferences.GtsBuildingDestruction.value = value;
			}

			internal void _003CStart_003Eb__35_11(bool value)
			{
				GlobalPreferences.GtsPlayerBuildingDestruction.value = value;
			}

			internal void _003CStart_003Eb__35_12(bool value)
			{
				GlobalPreferences.MicroPlayerBuildingDestruction.value = value;
			}

			internal void _003CStart_003Eb__35_13(bool value)
			{
				GlobalPreferences.KeepDebris.value = value;
			}

			internal void _003CStart_003Eb__35_14(bool value)
			{
				GlobalPreferences.SecondaryDestruction.value = value;
			}

			internal void _003CStart_003Eb__35_15(bool value)
			{
				GlobalPreferences.DebrisCanCrush.value = value;
			}

			internal void _003CStart_003Eb__35_16(bool value)
			{
				GlobalPreferences.RagDollEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_17(bool value)
			{
				GlobalPreferences.CrushPlayerEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_18(bool value)
			{
				GlobalPreferences.CrushNpcEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_19(bool value)
			{
				GlobalPreferences.NpcGtsCrushingEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_20(bool value)
			{
				GlobalPreferences.NpcMicroCrushingEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_21(bool value)
			{
				GlobalPreferences.PlayerCrushingEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_22(bool value)
			{
				GlobalPreferences.CrushStick.value = value;
			}

			internal void _003CStart_003Eb__35_23(float value)
			{
				GlobalPreferences.CrushStickChance.value = value;
			}

			internal void _003CStart_003Eb__35_24(float value)
			{
				GlobalPreferences.StompSpeed.value = value;
			}

			internal void _003CStart_003Eb__35_25(float value)
			{
				GlobalPreferences.CrushStickDuration.value = value;
			}

			internal void _003CStart_003Eb__35_28(float value)
			{
				GlobalPreferences.CrushSurviveChance.value = value;
			}

			internal void _003CStart_003Eb__35_29(int x)
			{
				OnLogLevelChanged(x, true);
			}

			internal void _003CStart_003Eb__35_30(bool value)
			{
				GlobalPreferences.ScriptAuxLogging.value = value;
			}

			internal void _003CStart_003Eb__35_31(bool value)
			{
				GlobalPreferences.BreastPhysics.value = value;
			}

			internal void _003CStart_003Eb__35_32(bool value)
			{
				GlobalPreferences.HairPhysics.value = value;
			}

			internal void _003CStart_003Eb__35_33(bool value)
			{
				GlobalPreferences.BreastPhysics.value = value;
			}

			internal void _003CStart_003Eb__35_34(bool value)
			{
				GlobalPreferences.ClothPhysics.value = value;
			}

			internal void _003CStart_003Eb__35_35(bool value)
			{
				GlobalPreferences.UseGrounder.value = value;
				FootIK[] array = UnityEngine.Object.FindObjectsOfType<FootIK>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].EnableIk = value;
				}
			}

			internal void _003CStart_003Eb__35_40(bool value)
			{
				GlobalPreferences.UseAdvancedCollision.value = value;
				if (value)
				{
					UiMessageBox.Create("This feature is \n<color=#ff0000ff>very</color>\ndemanding.\n\nIf you experience poor performance, please disable this feature.", "Performance Alert").Popup();
				}
			}

			internal void _003CStart_003Eb__35_41(float value)
			{
				GlobalPreferences.MacroColliderUpdateDistance.value = value;
			}

			internal void _003CStart_003Eb__35_45(bool value)
			{
				GlobalPreferences.SlOpenOnEditor.value = value;
			}

			internal void _003CStart_003Eb__35_46(bool value)
			{
				GlobalPreferences.SlSelObjOnEntryClick.value = value;
			}

			internal void _003CStart_003Eb__35_47(bool value)
			{
				GlobalPreferences.BackgroundPause.value = value;
			}

			internal void _003CStart_003Eb__35_48(bool value)
			{
				GlobalPreferences.BloodEnabled.value = value;
			}

			internal void _003CStart_003Eb__35_49(bool value)
			{
				GlobalPreferences.CameraEdgeScroll.value = value;
			}

			internal char _003CStart_003Eb__35_53(string input, int charIndex, char addedChar)
			{
				return SettingsViewUtil.ValidateDigit(addedChar);
			}

			internal char _003CStart_003Eb__35_54(string input, int charIndex, char addedChar)
			{
				return SettingsViewUtil.ValidateDigit(addedChar);
			}

			internal void _003CStart_003Eb__35_52(bool value)
			{
				GlobalPreferences.ImperialMeasurements.value = value;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass43_0
		{
			public GameSettingsView _003C_003E4__this;

			public UiMessageBox messageBox;

			internal void _003COnResetAllClicked_003Eb__0()
			{
				PlayerPrefs.DeleteAll();
				GlobalPreferences.Refresh();
				_003C_003E4__this.UpdateValues();
				messageBox.Close();
			}
		}

		private Slider _stompSpeed;

		private Slider _crushStickDuration;

		private InputSlider _microDurability;

		private InputSlider _microSpawnSize;

		private Slider _crushSurviveChance;

		private Slider _speedSlider;

		private InputSlider _microSpeedSlider;

		private InputSlider _bulletTimeSpeedSlider;

		private InputSlider _cityPopulation;

		private InputSlider _lazyBatch;

		private InputSlider _clampWeight;

		private InputSlider _clampWeightHead;

		private InputSlider _clampWeightEyes;

		private InputSlider _clampSmoothing;

		private ToggleSlider _crushStickChance;

		private Slider _macroColliderUpdateDistance;

		private InputSlider _meshCheckLimit;

		private InputSlider _meshUpdateLimit;

		private InputSlider _microRepositionSpeed;

		private Dropdown _logLevel;

		private Button _behaviorManager;

		private Button _resetHeadIk;

		private Button _resetAll;

		private GameObject _sizeGunSettings;

		private GameObject _aiGunSettings;

		private GameObject _behaviourManager;

		private GameObject _advancedClouds;

		public GameObject SizeGunSettings
		{
			set
			{
				_sizeGunSettings = value;
			}
		}

		public GameObject AiGunSettings
		{
			set
			{
				_aiGunSettings = value;
			}
		}

		public GameObject BehaviourManager
		{
			set
			{
				_behaviourManager = value;
			}
		}

		public GameObject AdvancedClouds
		{
			set
			{
				_advancedClouds = value;
			}
		}

		private void Start()
		{
			AddButton("Size Gun Settings").onClick.AddListener(OnSizeGunSettingsClick);
			AddButton("AI Gun Settings").onClick.AddListener(OnAIGunSettingsClick);
			AddButton("Advanced Clouds").onClick.AddListener(OnAdvancedCloudsClick);
			AddHeader("Behaviour");
			_behaviorManager = AddButton("Behavior Manager...");
			_behaviorManager.onClick.AddListener(OnBehaviorManagerClicked);
			AddToggle("Enable Blinking", GlobalPreferences.BlinkEnabled.value, _003CStart_003Eb__35_0);
			AddToggle("Enable Giantess AI", GlobalPreferences.GtsAI.value, _003C_003Ec._003C_003E9__35_1 ?? (_003C_003Ec._003C_003E9__35_1 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_1));
			AddToggle("Enable Micro AI", GlobalPreferences.MicroAI.value, _003C_003Ec._003C_003E9__35_2 ?? (_003C_003Ec._003C_003E9__35_2 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_2));
			AddToggle("Ignore player", GlobalPreferences.IgnorePlayer.value, _003C_003Ec._003C_003E9__35_3 ?? (_003C_003Ec._003C_003E9__35_3 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_3));
			AddToggle("Look At Player", GlobalPreferences.LookAtPlayer.value, _003C_003Ec._003C_003E9__35_4 ?? (_003C_003Ec._003C_003E9__35_4 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_4));
			AddHeader("Head IK");
			_clampWeight = AddInputSlider("Body weight", 0f, 1f);
			_clampWeight.slider.onValueChanged.AddListener(_003CStart_003Eb__35_5);
			ClampWeightChanged(GlobalPreferences.ClampWeight.value);
			_clampWeightHead = AddInputSlider("Head weight", 0f, 1f);
			_clampWeightHead.slider.onValueChanged.AddListener(_003CStart_003Eb__35_6);
			ClampWeightHeadChanged(GlobalPreferences.ClampWeightHead.value);
			_clampWeightEyes = AddInputSlider("Eyes weight", 0f, 1f);
			_clampWeightEyes.slider.onValueChanged.AddListener(_003CStart_003Eb__35_7);
			ClampWeightEyesChanged(GlobalPreferences.ClampWeightEyes.value);
			_clampSmoothing = AddInputSlider("Smoothing", 0f, 2f, true);
			_clampSmoothing.slider.onValueChanged.AddListener(_003CStart_003Eb__35_8);
			ClampSmoothingChanged(GlobalPreferences.ClampSmoothing.value);
			_resetHeadIk = AddButton("Reset Head IK Values");
			_resetHeadIk.onClick.AddListener(ResetHeadIk);
			AddHeader("City Destruction");
			AddToggle("Always Low End", GlobalPreferences.LowEndCities.value, _003C_003Ec._003C_003E9__35_9 ?? (_003C_003Ec._003C_003E9__35_9 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_9));
			AddToggle("Enable for Macro NPC", GlobalPreferences.GtsBuildingDestruction.value, _003C_003Ec._003C_003E9__35_10 ?? (_003C_003Ec._003C_003E9__35_10 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_10));
			AddToggle("Enable for Macro Player", GlobalPreferences.GtsPlayerBuildingDestruction.value, _003C_003Ec._003C_003E9__35_11 ?? (_003C_003Ec._003C_003E9__35_11 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_11));
			AddToggle("Enable for Micro Player", GlobalPreferences.MicroPlayerBuildingDestruction.value, _003C_003Ec._003C_003E9__35_12 ?? (_003C_003Ec._003C_003E9__35_12 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_12));
			AddToggle("Keep Building Debris", GlobalPreferences.KeepDebris.value, _003C_003Ec._003C_003E9__35_13 ?? (_003C_003Ec._003C_003E9__35_13 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_13));
			AddToggle("Secondary destruction", GlobalPreferences.SecondaryDestruction.value, _003C_003Ec._003C_003E9__35_14 ?? (_003C_003Ec._003C_003E9__35_14 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_14));
			AddToggle("Debris Chunks Can Crush Micros", GlobalPreferences.DebrisCanCrush.value, _003C_003Ec._003C_003E9__35_15 ?? (_003C_003Ec._003C_003E9__35_15 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_15));
			AddHeader("Crushing");
			AddToggle("Enable Ragdolls", GlobalPreferences.RagDollEnabled.value, _003C_003Ec._003C_003E9__35_16 ?? (_003C_003Ec._003C_003E9__35_16 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_16));
			AddToggle("Allow Player Micro to be Crushed", GlobalPreferences.CrushPlayerEnabled.value, _003C_003Ec._003C_003E9__35_17 ?? (_003C_003Ec._003C_003E9__35_17 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_17));
			AddToggle("Allow NPC Micros to be Crushed", GlobalPreferences.CrushNpcEnabled.value, _003C_003Ec._003C_003E9__35_18 ?? (_003C_003Ec._003C_003E9__35_18 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_18));
			AddToggle("Allow NPC Macros to Crush", GlobalPreferences.NpcGtsCrushingEnabled.value, _003C_003Ec._003C_003E9__35_19 ?? (_003C_003Ec._003C_003E9__35_19 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_19));
			AddToggle("Allow NPC Micros to Crush", GlobalPreferences.NpcMicroCrushingEnabled.value, _003C_003Ec._003C_003E9__35_20 ?? (_003C_003Ec._003C_003E9__35_20 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_20));
			AddToggle("Allow Player to Crush", GlobalPreferences.PlayerCrushingEnabled.value, _003C_003Ec._003C_003E9__35_21 ?? (_003C_003Ec._003C_003E9__35_21 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_21));
			_crushStickChance = AddToggleSlider("Stick Chance", 0f, 1f);
			_crushStickChance.toggle.isOn = GlobalPreferences.CrushStick.value;
			_crushStickChance.slider.value = GlobalPreferences.CrushStickChance.value;
			_crushStickChance.toggle.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_22 ?? (_003C_003Ec._003C_003E9__35_22 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_22));
			_crushStickChance.slider.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_23 ?? (_003C_003Ec._003C_003E9__35_23 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_23));
			_stompSpeed = AddSlider("Stomp Speed", 0.1f, 3f);
			_stompSpeed.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_24 ?? (_003C_003Ec._003C_003E9__35_24 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_24));
			_crushStickDuration = AddSlider("Stuck Duration", 0f, 1f);
			_crushStickDuration.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_25 ?? (_003C_003Ec._003C_003E9__35_25 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_25));
			_microDurability = AddInputSlider("Micro Durability", 0f, 1f);
			_microDurability.slider.onValueChanged.AddListener(_003CStart_003Eb__35_26);
			MicroDurabilityChanged(GlobalPreferences.MicroDurability.value);
			_microSpawnSize = AddInputSlider("Micro Hot Key Spawn Size", 0.01f, 1f);
			_microSpawnSize.slider.onValueChanged.AddListener(_003CStart_003Eb__35_27);
			MicroSpawnSizeChanged(GlobalPreferences.MicroSpawnSize.value);
			_crushSurviveChance = AddSlider("Stuck Survivability", 0f, 1f);
			_crushSurviveChance.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_28 ?? (_003C_003Ec._003C_003E9__35_28 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_28));
			AddHeader("Debug");
			_logLevel = AddDropdown("Log Level", new List<Dropdown.OptionData>
			{
				new Dropdown.OptionData("Log"),
				new Dropdown.OptionData("Warning"),
				new Dropdown.OptionData("Error")
			});
			_logLevel.value = GlobalPreferences.LogLevel.value;
			_logLevel.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_29 ?? (_003C_003Ec._003C_003E9__35_29 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_29));
			AddToggle("Auxiliary Lua Debug Logging", GlobalPreferences.ScriptAuxLogging.value, _003C_003Ec._003C_003E9__35_30 ?? (_003C_003Ec._003C_003E9__35_30 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_30));
			AddHeader("Dynamic Bone Physics");
			AddToggle("Breast Rig Physics", GlobalPreferences.BreastPhysics.value, _003C_003Ec._003C_003E9__35_31 ?? (_003C_003Ec._003C_003E9__35_31 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_31));
			AddToggle("Hair Rig Physics", GlobalPreferences.HairPhysics.value, _003C_003Ec._003C_003E9__35_32 ?? (_003C_003Ec._003C_003E9__35_32 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_32));
			AddToggle("Jiggle Rig Physics", GlobalPreferences.BreastPhysics.value, _003C_003Ec._003C_003E9__35_33 ?? (_003C_003Ec._003C_003E9__35_33 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_33));
			AddToggle("Cloth Physics", GlobalPreferences.ClothPhysics.value, _003C_003Ec._003C_003E9__35_34 ?? (_003C_003Ec._003C_003E9__35_34 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_34));
			AddToggle("IK Foot Placement", GlobalPreferences.UseGrounder.value, _003C_003Ec._003C_003E9__35_35 ?? (_003C_003Ec._003C_003E9__35_35 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_35));
			AddHeader("Game Speed");
			_speedSlider = AddSlider("Giantess Speed", 0.02f, 4f);
			if (GameController.Instance != null)
			{
				_speedSlider.onValueChanged.AddListener(GameController.ChangeSpeed);
			}
			_microSpeedSlider = AddInputSlider("Micro Speed", 0.1f, 4f);
			_microSpeedSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__35_36);
			MicroSpeedChanged(GlobalPreferences.MicroSpeed.value);
			_bulletTimeSpeedSlider = AddInputSlider("Bullet Time Speed", 0.01f, 0.5f);
			_bulletTimeSpeedSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__35_37);
			BulletTimeSpeedChanged(GlobalPreferences.BulletTimeFactor.value);
			AddToggle("Giantess Speed Affected by Scale", GlobalPreferences.SlowdownWithSize.value, _003CStart_003Eb__35_38);
			AddToggle("Micros Affected by Bullet Time", GlobalPreferences.MicrosAffectedByBulletTime.value, _003CStart_003Eb__35_39);
			AddHeader("Advanced Collision");
			AddToggle("Use Advanced Collision", GlobalPreferences.UseAdvancedCollision.value, _003C_003Ec._003C_003E9__35_40 ?? (_003C_003Ec._003C_003E9__35_40 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_40));
			_macroColliderUpdateDistance = AddSlider("Update Distance", 1f, 600f);
			_macroColliderUpdateDistance.onValueChanged.AddListener(_003C_003Ec._003C_003E9__35_41 ?? (_003C_003Ec._003C_003E9__35_41 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_41));
			_meshCheckLimit = AddInputSlider("Bone Check Limit", 1f, 200f, true);
			_meshCheckLimit.slider.onValueChanged.AddListener(_003CStart_003Eb__35_42);
			MeshCheckLimitChanged(GlobalPreferences.MeshCheckLimit.value);
			_meshUpdateLimit = AddInputSlider("Collider Update Limit", 1f, 30f, true);
			_meshUpdateLimit.slider.onValueChanged.AddListener(_003CStart_003Eb__35_43);
			MeshUpdateLimitChanged(GlobalPreferences.MeshUpdateLimit.value);
			_microRepositionSpeed = AddInputSlider("Micro Reposition Speed", 0f, 200f);
			_microRepositionSpeed.slider.onValueChanged.AddListener(_003CStart_003Eb__35_44);
			MicroRepositionSpeedChanged(GlobalPreferences.MicroRepositionSpeed.value);
			AddHeader("Editor");
			AddToggle("Scene List Auto-Open", GlobalPreferences.SlOpenOnEditor.value, _003C_003Ec._003C_003E9__35_45 ?? (_003C_003Ec._003C_003E9__35_45 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_45));
			AddToggle("Scene List Auto-Select", GlobalPreferences.SlSelObjOnEntryClick.value, _003C_003Ec._003C_003E9__35_46 ?? (_003C_003Ec._003C_003E9__35_46 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_46));
			AddHeader("Misc");
			AddToggle("Automatically Pause When Unfocused", GlobalPreferences.BackgroundPause.value, _003C_003Ec._003C_003E9__35_47 ?? (_003C_003Ec._003C_003E9__35_47 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_47));
			AddToggle("Blood", GlobalPreferences.BloodEnabled.value, _003C_003Ec._003C_003E9__35_48 ?? (_003C_003Ec._003C_003E9__35_48 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_48));
			AddToggle("Camera Edge Scrolling", GlobalPreferences.CameraEdgeScroll.value, _003C_003Ec._003C_003E9__35_49 ?? (_003C_003Ec._003C_003E9__35_49 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_49));
			_lazyBatch = AddInputSlider("Lazy Batch", 1f, 50f);
			_lazyBatch.slider.onValueChanged.AddListener(_003CStart_003Eb__35_50);
			InputField input = _lazyBatch.input;
			input.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(input.onValidateInput, _003C_003Ec._003C_003E9__35_53 ?? (_003C_003Ec._003C_003E9__35_53 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_53));
			LazyBatchChanged(GlobalPreferences.LazyBatch.value);
			_cityPopulation = AddInputSlider("Street Population", 5f, 500f);
			_cityPopulation.slider.onValueChanged.AddListener(_003CStart_003Eb__35_51);
			InputField input2 = _cityPopulation.input;
			input2.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(input2.onValidateInput, _003C_003Ec._003C_003E9__35_54 ?? (_003C_003Ec._003C_003E9__35_54 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_54));
			CityPopulationChanged(GlobalPreferences.CityPopulation.value);
			AddToggle("Imperial Measurements", GlobalPreferences.ImperialMeasurements.value, _003C_003Ec._003C_003E9__35_52 ?? (_003C_003Ec._003C_003E9__35_52 = _003C_003Ec._003C_003E9._003CStart_003Eb__35_52));
			if (SettingsView.IsMainMenu)
			{
				_resetAll = AddButton("Reset All Settings");
				_resetAll.onClick.AddListener(OnResetAllClicked);
			}
			UpdateValues();
			initialized = true;
		}

		public static void OnLogLevelChanged(int value, bool write = false)
		{
			LogType filterLogType = LogType.Warning;
			switch (value)
			{
			case 0:
				filterLogType = LogType.Log;
				break;
			case 3:
				filterLogType = LogType.Error;
				break;
			}
			Debug.unityLogger.filterLogType = filterLogType;
			if (write)
			{
				GlobalPreferences.LogLevel.value = value;
			}
		}

		private void OnSlowToggleChanged(bool value, bool write = false)
		{
			if (write)
			{
				GlobalPreferences.SlowdownWithSize.value = value;
			}
			if (!SettingsView.IsMainMenu)
			{
				GameController.SetSlowDown(value);
			}
		}

		private void OnMicrosEffectedByBulletTime(bool value, bool write = false)
		{
			if (write)
			{
				GlobalPreferences.MicrosAffectedByBulletTime.value = value;
			}
			if (!SettingsView.IsMainMenu)
			{
				GameController.microsAffectedByBulletTime = value;
				ObjectManager.Instance.UpdateMicroSpeeds();
			}
		}

		private void OnAdvancedCloudsClick()
		{
			if (!_advancedClouds)
			{
				UiMessageBox.Create("You must be in-game to use Advanced Clouds!", "Not available").Popup();
				return;
			}
			base.gameObject.SetActive(false);
			_advancedClouds.SetActive(true);
		}

		private void OnBehaviorManagerClicked()
		{
			if (!_behaviourManager)
			{
				UiMessageBox.Create("You must be in-game to use the behaviour manager!", "Not available").Popup();
				return;
			}
			base.gameObject.SetActive(false);
			_behaviourManager.SetActive(true);
		}

		public void OnBehaviorManagerHide()
		{
			base.gameObject.SetActive(true);
		}

		private void OnGtsBlink(bool value, bool write = false)
		{
			if (value)
			{
				StartCoroutine(Humanoid.StartBlinkingRoutines());
			}
			if (write)
			{
				GlobalPreferences.BlinkEnabled.value = value;
			}
		}

		private void OnResetAllClicked()
		{
			_003C_003Ec__DisplayClass43_0 _003C_003Ec__DisplayClass43_ = new _003C_003Ec__DisplayClass43_0();
			_003C_003Ec__DisplayClass43_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass43_.messageBox = UiMessageBox.Create("Are you sure!?\n\nYou will lose\n[ ALL ]\nof your settings.", "Reset All Settings");
			_003C_003Ec__DisplayClass43_.messageBox.AddButtonsYesNo(_003C_003Ec__DisplayClass43_._003COnResetAllClicked_003Eb__0);
			_003C_003Ec__DisplayClass43_.messageBox.Popup();
		}

		private void ResetHeadIk()
		{
			GlobalPreferences.ClampWeight.value = 0.5f;
			GlobalPreferences.ClampWeightHead.value = 0.6f;
			GlobalPreferences.ClampWeightEyes.value = 0.7f;
			GlobalPreferences.ClampSmoothing.value = 2;
			_clampWeight.slider.value = 0.5f;
			_clampWeightHead.slider.value = 0.6f;
			_clampWeightEyes.slider.value = 0.7f;
			_clampSmoothing.slider.value = 2f;
		}

		protected override void UpdateValues()
		{
			_stompSpeed.value = GlobalPreferences.StompSpeed.value;
			_crushStickDuration.value = GlobalPreferences.CrushStickDuration.value;
			_microDurability.slider.value = GlobalPreferences.MicroDurability.value;
			_microSpawnSize.slider.value = GlobalPreferences.MicroSpawnSize.value;
			_crushSurviveChance.value = GlobalPreferences.CrushSurviveChance.value;
			_macroColliderUpdateDistance.value = GlobalPreferences.MacroColliderUpdateDistance.value;
			_meshCheckLimit.slider.value = GlobalPreferences.MeshCheckLimit.value;
			_meshUpdateLimit.slider.value = GlobalPreferences.MeshUpdateLimit.value;
			_microRepositionSpeed.slider.value = GlobalPreferences.MicroRepositionSpeed.value;
			_lazyBatch.slider.value = GlobalPreferences.LazyBatch.value;
			_cityPopulation.slider.value = GlobalPreferences.CityPopulation.value;
			_microSpeedSlider.slider.value = GlobalPreferences.MicroSpeed.value;
			_bulletTimeSpeedSlider.slider.value = GlobalPreferences.BulletTimeFactor.value;
			_clampWeight.slider.value = GlobalPreferences.ClampWeight.value;
			_clampWeightHead.slider.value = GlobalPreferences.ClampWeightHead.value;
			_clampWeightEyes.slider.value = GlobalPreferences.ClampWeightEyes.value;
			_clampSmoothing.slider.value = GlobalPreferences.ClampSmoothing.value;
			_speedSlider.value = GameController.macroSpeed;
		}

		private void CityPopulationChanged(float value, bool write = false)
		{
			int num = (int)value;
			SetInputSliderInput(_cityPopulation, num);
			if (write)
			{
				GlobalPreferences.CityPopulation.value = num;
			}
		}

		private void MicroSpeedChanged(float value, bool write = false)
		{
			_microSpeedSlider.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (!SettingsView.IsMainMenu)
			{
				GameController.ChangeMicroSpeed(value);
			}
			if (write)
			{
				GlobalPreferences.MicroSpeed.value = value;
			}
		}

		private void BulletTimeSpeedChanged(float value, bool write = false)
		{
			_bulletTimeSpeedSlider.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.BulletTimeFactor.value = value;
				GameController.ChangeBulletTimeSpeed(value);
			}
		}

		private void MicroDurabilityChanged(float value, bool write = false)
		{
			_microDurability.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MicroDurability.value = value;
			}
		}

		private void MicroSpawnSizeChanged(float value, bool write = false)
		{
			_microSpawnSize.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MicroSpawnSize.value = value;
			}
		}

		private void MeshCheckLimitChanged(float value, bool write = false)
		{
			float num = (int)value;
			_meshCheckLimit.input.text = num.ToString("0", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MeshCheckLimit.value = (int)num;
			}
		}

		private void MeshUpdateLimitChanged(float value, bool write = false)
		{
			float num = (int)value;
			_meshUpdateLimit.input.text = num.ToString("0", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MeshUpdateLimit.value = (int)num;
			}
		}

		private void MicroRepositionSpeedChanged(float value, bool write = false)
		{
			_microRepositionSpeed.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MicroRepositionSpeed.value = value;
			}
		}

		private void ClampWeightChanged(float value, bool write = false)
		{
			float value2 = value;
			_clampWeight.input.text = value2.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.ClampWeight.value = value2;
			}
		}

		private void ClampWeightHeadChanged(float value, bool write = false)
		{
			float value2 = value;
			_clampWeightHead.input.text = value2.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.ClampWeightHead.value = value2;
			}
		}

		private void ClampWeightEyesChanged(float value, bool write = false)
		{
			float value2 = value;
			_clampWeightEyes.input.text = value2.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.ClampWeightEyes.value = value2;
			}
		}

		private void ClampSmoothingChanged(float value, bool write = false)
		{
			float num = (int)value;
			_clampSmoothing.input.text = num.ToString("0", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.ClampSmoothing.value = (int)num;
			}
		}

		private void LazyBatchChanged(float value, bool write = false)
		{
			int num = (int)value;
			SetInputSliderInput(_lazyBatch, num);
			if (write)
			{
				GlobalPreferences.LazyBatch.value = num;
			}
		}

		private void OnSizeGunSettingsClick()
		{
			_sizeGunSettings.SetActive(true);
			base.gameObject.SetActive(false);
		}

		private void OnAIGunSettingsClick()
		{
			_aiGunSettings.SetActive(true);
			base.gameObject.SetActive(false);
		}

		public override void ClosePanel()
		{
			UnityEngine.Object.Destroy(_behaviourManager);
			UnityEngine.Object.Destroy(_sizeGunSettings);
			UnityEngine.Object.Destroy(_aiGunSettings);
			UnityEngine.Object.Destroy(_advancedClouds);
			base.ClosePanel();
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_0(bool value)
		{
			OnGtsBlink(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_5(float value)
		{
			ClampWeightChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_6(float value)
		{
			ClampWeightHeadChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_7(float value)
		{
			ClampWeightEyesChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_8(float value)
		{
			ClampSmoothingChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_26(float value)
		{
			MicroDurabilityChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_27(float value)
		{
			MicroSpawnSizeChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_36(float value)
		{
			MicroSpeedChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_37(float value)
		{
			BulletTimeSpeedChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_38(bool value)
		{
			OnSlowToggleChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_39(bool value)
		{
			OnMicrosEffectedByBulletTime(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_42(float value)
		{
			MeshCheckLimitChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_43(float value)
		{
			MeshUpdateLimitChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_44(float value)
		{
			MicroRepositionSpeedChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_50(float value)
		{
			LazyBatchChanged(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__35_51(float value)
		{
			CityPopulationChanged(value, true);
		}
	}
}
