using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.ProceduralCityGenerator;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Pause
{
	public class VideoSettingsView : SettingsView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static InputField.OnValidateInput _003C_003E9__48_46;

			public static InputField.OnValidateInput _003C_003E9__48_47;

			public static InputField.OnValidateInput _003C_003E9__48_48;

			public static UnityAction<bool> _003C_003E9__48_23;

			internal char _003CStart_003Eb__48_46(string input, int charIndex, char addedChar)
			{
				return SettingsViewUtil.ValidateDigit(addedChar);
			}

			internal char _003CStart_003Eb__48_47(string input, int charIndex, char addedChar)
			{
				return SettingsViewUtil.ValidateDigit(addedChar);
			}

			internal char _003CStart_003Eb__48_48(string input, int charIndex, char addedChar)
			{
				return SettingsViewUtil.ValidateNumber(addedChar);
			}

			internal void _003CStart_003Eb__48_23(bool value)
			{
				GlobalPreferences.BackgroundMaxFps.value = value;
			}
		}

		private Dropdown _drop3d;

		private Dropdown _dropRes;

		private Dropdown _dropTextureQuality;

		private Dropdown _dropAnisotropic;

		private Dropdown _dropAntiAlias;

		private Dropdown _dropVSync;

		private Dropdown _dropRenderMode;

		private Dropdown _dropReflectionsUpdate;

		private Dropdown _dropReflectionsResolution;

		private Dropdown _dropShadowCascades;

		private Dropdown _dropShadowResolution;

		private Dropdown _dropQuality;

		private Dropdown _dropOffScreenUpdate;

		private Toggle _videoMode;

		private Toggle _postProcessToggle;

		private Toggle _fastApproximateAntiAliasingToggle;

		private Toggle _depthOfFieldToggle;

		private Toggle _toggleFps;

		private Toggle _motionBlurToggle;

		private Toggle _smokeToggle;

		private Toggle _backgroundMaxFps;

		private Toggle _aoVolumetric;

		private Toggle _colorHighDefinitionRange;

		private Toggle _colorAcademyColorEncodingSystem;

		private Toggle _colorGrading;

		private Toggle _highDynamicRange;

		private Toggle _shadows;

		private Toggle _softShadows;

		private Toggle _softParticles;

		private ToggleSlider _shakeSlider;

		private InputSlider _colorBrightnessSlider;

		private InputSlider _colorContrastSlider;

		private InputSlider _colorSaturationSlider;

		private InputSlider _fovSlider;

		private InputSlider _fpsSlider;

		private InputSlider _detailDistanceSlider;

		private InputSlider _uiScaleSlider;

		private InputSlider _pixelLightSlider;

		private InputSlider _shadowDistance;

		private ToggleSlider _fog;

		private ToggleSlider _chromaticAberrationSlider;

		private ToggleSlider _bloomSlider;

		private ToggleSlider _vignetteSlider;

		private ToggleSlider _aoToggleSlider;

		private CameraEffectsSettings _cameraEffects;

		private VrCamera _vrCamera;

		private const float MinUiScale = 0.5f;

		private const float MaxUiScale = 1.1f;

		private void Start()
		{
			base.Title = "Video";
			if (mainCamera != null)
			{
				_cameraEffects = mainCamera.GetComponent<CameraEffectsSettings>();
				_vrCamera = mainCamera.GetComponent<VrCamera>();
			}
			AddHeader("Display");
			List<string> list = StringResolutions();
			if (list.Count > 0)
			{
				List<Dropdown.OptionData> options = CreateTextOptions(list.ToArray(), false);
				_dropRes = AddDropdown("Resolution", options);
				_dropRes.value = FindCurrentResolutionListing();
				_dropRes.onValueChanged.AddListener(SetRes);
			}
			_videoMode = AddToggle("Fullscreen", Screen.fullScreen);
			_videoMode.onValueChanged.AddListener(SetFullScreen);
			if ((bool)_vrCamera && _vrCamera.vrSupported)
			{
				List<Dropdown.OptionData> options2 = CreateTextOptions(XRSettings.supportedDevices);
				_drop3d = AddDropdown("3D/VR Mode", options2);
				_drop3d.value = GetVr3dModeIndex();
				_drop3d.onValueChanged.AddListener(_003CStart_003Eb__48_45);
			}
			_fpsSlider = AddInputSlider("Frame Limit", 10f, 300f);
			_fpsSlider.slider.value = GlobalPreferences.Fps.value;
			SetInputSliderInput(_fpsSlider, _fpsSlider.slider.value);
			_fpsSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_0);
			InputField input = _fpsSlider.input;
			input.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(input.onValidateInput, _003C_003Ec._003C_003E9__48_46 ?? (_003C_003Ec._003C_003E9__48_46 = _003C_003Ec._003C_003E9._003CStart_003Eb__48_46));
			_fovSlider = AddInputSlider("Field of View", 45f, 110f);
			_fovSlider.slider.value = GlobalPreferences.Fov.value;
			SetInputSliderInput(_fovSlider, _fovSlider.slider.value);
			_fovSlider.slider.onValueChanged.AddListener(OnFovChanged);
			InputField input2 = _fovSlider.input;
			input2.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(input2.onValidateInput, _003C_003Ec._003C_003E9__48_47 ?? (_003C_003Ec._003C_003E9__48_47 = _003C_003Ec._003C_003E9._003CStart_003Eb__48_47));
			_uiScaleSlider = AddInputSlider("User Interface Scale", 0.5f, 1.1f);
			_uiScaleSlider.input.onEndEdit.AddListener(_003CStart_003Eb__48_1);
			_uiScaleSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_2);
			_uiScaleSlider.slider.value = GlobalPreferences.UIScale.value;
			SetInputSliderInput(_uiScaleSlider, _uiScaleSlider.slider.value, "0.00");
			InputField input3 = _uiScaleSlider.input;
			input3.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(input3.onValidateInput, _003C_003Ec._003C_003E9__48_48 ?? (_003C_003Ec._003C_003E9__48_48 = _003C_003Ec._003C_003E9._003CStart_003Eb__48_48));
			_dropVSync = AddDropdown("Vertical Synchronisation", CreateTextOptions(new string[3] { "None", "Every V Blank", "Every Second V Blank" }));
			_dropVSync.value = QualitySettings.vSyncCount;
			_dropVSync.onValueChanged.AddListener(_003CStart_003Eb__48_3);
			AddHeader("Quality");
			_dropQuality = AddDropdown("Quality Preset", CreateTextOptions(new string[4] { "-", "Performance", "Balanced", "Beautiful" }));
			_dropQuality.onValueChanged.AddListener(SetQualityPreset);
			_dropAnisotropic = AddDropdown("Anisotropic Textures", CreateTextOptions(new string[3] { "Disabled", "Per Texture", "Forced On" }, false));
			_dropAnisotropic.value = GlobalPreferences.AnisotropicTexture.value;
			_dropAnisotropic.onValueChanged.AddListener(_003CStart_003Eb__48_4);
			_detailDistanceSlider = AddInputSlider("Level of Detail Bias", 0.2f, 4.65f);
			_detailDistanceSlider.slider.value = QualitySettings.lodBias;
			SetInputSliderInput(_detailDistanceSlider, _detailDistanceSlider.slider.value, "0.00");
			_detailDistanceSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_5);
			_dropAntiAlias = AddDropdown("Multi Sample Anti-Aliasing", CreateTextOptions(new string[4] { "None", "x2", "x4", "x8" }, false));
			_dropAntiAlias.value = GlobalPreferences.Msaa.value;
			_dropAntiAlias.onValueChanged.AddListener(_003CStart_003Eb__48_6);
			_pixelLightSlider = AddInputSlider("Pixel Light Count", 1f, 64f, true);
			_pixelLightSlider.slider.value = GlobalPreferences.PixelLightCount.value;
			SetInputSliderInput(_pixelLightSlider, _pixelLightSlider.slider.value, "0");
			_pixelLightSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_7);
			_dropTextureQuality = AddDropdown("Texture Quality", CreateTextOptions(new string[4] { "Very High", "High", "Low", "Very Low" }, false));
			_dropTextureQuality.value = GlobalPreferences.TextureQuality.value;
			_dropTextureQuality.onValueChanged.AddListener(_003CStart_003Eb__48_8);
			AddHeader("Effects");
			_fog = AddToggleSlider("Fog", 0.0025f, 0.5f);
			_fog.slider.value = GlobalPreferences.FogPercent.value;
			_fog.slider.onValueChanged.AddListener(_003CStart_003Eb__48_9);
			_fog.toggle.isOn = GlobalPreferences.Fog.value;
			_fog.toggle.onValueChanged.AddListener(_003CStart_003Eb__48_10);
			AddToggle("Exponential Squared", GlobalPreferences.FogExponential.value, _003CStart_003Eb__48_11);
			AddToggle("Fog Covers Sky", GlobalPreferences.FogSkyBox.value, _003CStart_003Eb__48_12);
			_highDynamicRange = AddToggle("High Dynamic Range", GlobalPreferences.HDR.value, _003CStart_003Eb__48_13);
			_smokeToggle = AddToggle("Smoke Particles");
			_smokeToggle.onValueChanged.AddListener(UpdateSmokeToggleChanged);
			_softParticles = AddToggle("Soft Particles", GlobalPreferences.SoftParticles.value, _003CStart_003Eb__48_14);
			AddHeader("Shadows");
			_shadows = AddToggle("Enable Shadows", GlobalPreferences.Shadows.value, _003CStart_003Eb__48_15);
			_dropShadowCascades = AddDropdown("Cascades", CreateTextOptions(new string[3] { "1", "2", "4" }, false));
			_dropShadowCascades.value = GlobalPreferences.ShadowCascade.value;
			_dropShadowCascades.onValueChanged.AddListener(_003CStart_003Eb__48_16);
			_shadowDistance = AddInputSlider("Distance", 100f, 50000f, GlobalPreferences.ShadowDistance.value, "0.00");
			SetInputSliderInput(_shadowDistance, "0.00");
			_shadowDistance.slider.onValueChanged.AddListener(_003CStart_003Eb__48_17);
			_dropShadowResolution = AddDropdown("Resolution", CreateTextOptions(new string[4] { "Low", "Medium", "High", "Very High" }, false));
			_dropShadowResolution.value = GlobalPreferences.ShadowResolution.value;
			_dropShadowResolution.onValueChanged.AddListener(_003CStart_003Eb__48_18);
			_softShadows = AddToggle("Soft Shadows", GlobalPreferences.SoftShadows.value, _003CStart_003Eb__48_19);
			AddHeader("Misc");
			_shakeSlider = AddToggleSlider("Camera Shake", 0f, 10f);
			_shakeSlider.slider.value = GlobalPreferences.CameraShakeMultiplier;
			_shakeSlider.toggle.isOn = GlobalPreferences.CameraShakeEnabled;
			_shakeSlider.slider.onValueChanged.AddListener(OnCameraShakeChanged);
			_shakeSlider.toggle.onValueChanged.AddListener(OnCameraShakeChanged);
			_toggleFps = AddToggle("Display FPS");
			_toggleFps.onValueChanged.AddListener(_003CStart_003Eb__48_20);
			_dropOffScreenUpdate = AddDropdown("Macro Off-screen Update", CreateTextOptions(new string[3] { "Never", "In Pose Mode", "Always" }));
			_dropOffScreenUpdate.value = GlobalPreferences.OffScreenUpdate;
			_dropOffScreenUpdate.onValueChanged.AddListener(_003CStart_003Eb__48_21);
			_dropRenderMode = AddDropdown("Render Path", CreateTextOptions(new string[4] { "Vertex Lit", "Forward", "Deferred Lighting", "Deferred Shading" }, false));
			_dropRenderMode.value = (_cameraEffects ? _cameraEffects.GetRenderingMode() : GlobalPreferences.RenderMode.value);
			_dropRenderMode.onValueChanged.AddListener(_003CStart_003Eb__48_22);
			_backgroundMaxFps = AddToggle("Run full speed when unfocused");
			_backgroundMaxFps.isOn = GlobalPreferences.BackgroundMaxFps.value;
			_backgroundMaxFps.onValueChanged.AddListener(_003C_003Ec._003C_003E9__48_23 ?? (_003C_003Ec._003C_003E9__48_23 = _003C_003Ec._003C_003E9._003CStart_003Eb__48_23));
			AddHeader("Post Processing");
			_postProcessToggle = AddToggle("Enable Post Processing Stack");
			_postProcessToggle.onValueChanged.AddListener(_003CStart_003Eb__48_24);
			_aoToggleSlider = AddToggleSlider("Ambient Occlusion", 0.1f, 4f);
			_aoToggleSlider.toggle.onValueChanged.AddListener(_003CStart_003Eb__48_25);
			_aoToggleSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_26);
			_aoVolumetric = AddToggle("\tMulti Scale Volumetric Obscurance");
			_aoVolumetric.isOn = GlobalPreferences.AmbientOcclusionVolumetric.value;
			_aoVolumetric.onValueChanged.AddListener(_003CStart_003Eb__48_27);
			_bloomSlider = AddToggleSlider("Bloom", 0.1f, 25f);
			_bloomSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_28);
			_bloomSlider.toggle.onValueChanged.AddListener(_003CStart_003Eb__48_29);
			_chromaticAberrationSlider = AddToggleSlider("Chromatic Aberration", 0.1f, 1f);
			_chromaticAberrationSlider.toggle.onValueChanged.AddListener(_003CStart_003Eb__48_30);
			_chromaticAberrationSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_31);
			_colorGrading = AddToggle("Color Grading", GlobalPreferences.ColorGrading.value, _003CStart_003Eb__48_32);
			_colorHighDefinitionRange = AddToggle("\tHigh Definition Range", GlobalPreferences.ColorHDR.value, _003CStart_003Eb__48_33);
			_colorAcademyColorEncodingSystem = AddToggle("\tAcademy Color Encoding System", GlobalPreferences.ColorAces.value, _003CStart_003Eb__48_34);
			_colorBrightnessSlider = AddInputSlider("\tBrightness", -80f, 100f);
			SetInputSliderInput(_colorBrightnessSlider, GlobalPreferences.ColorBrightness.value, "0.00");
			_colorBrightnessSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_35);
			_colorContrastSlider = AddInputSlider("\tContrast", -80f, 100f);
			SetInputSliderInput(_colorContrastSlider, GlobalPreferences.ColorContrast.value, "0.00");
			_colorContrastSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_36);
			_colorSaturationSlider = AddInputSlider("\tSaturation", -100f, 100f);
			SetInputSliderInput(_colorSaturationSlider, GlobalPreferences.ColorSaturation.value, "0.00");
			_colorSaturationSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_37);
			_depthOfFieldToggle = AddToggle("Depth of Field");
			_depthOfFieldToggle.onValueChanged.AddListener(_003CStart_003Eb__48_38);
			_fastApproximateAntiAliasingToggle = AddToggle("Fast Approximate Anti-Aliasing");
			_fastApproximateAntiAliasingToggle.onValueChanged.AddListener(_003CStart_003Eb__48_39);
			_motionBlurToggle = AddToggle("Motion Blur");
			_motionBlurToggle.onValueChanged.AddListener(_003CStart_003Eb__48_40);
			_vignetteSlider = AddToggleSlider("Vignette", 0.2f, 0.5f);
			_vignetteSlider.toggle.onValueChanged.AddListener(_003CStart_003Eb__48_41);
			_vignetteSlider.slider.onValueChanged.AddListener(_003CStart_003Eb__48_42);
			AddHeader("Reflection Probes");
			List<Dropdown.OptionData> options3 = new List<Dropdown.OptionData>
			{
				new Dropdown.OptionData("Disabled"),
				new Dropdown.OptionData("Static"),
				new Dropdown.OptionData("Frequent"),
				new Dropdown.OptionData("Realtime")
			};
			_dropReflectionsUpdate = AddDropdown("Mode", options3);
			_dropReflectionsUpdate.value = GlobalPreferences.ReflectionMode.value;
			_dropReflectionsUpdate.onValueChanged.AddListener(_003CStart_003Eb__48_43);
			List<Dropdown.OptionData> options4 = new List<Dropdown.OptionData>
			{
				new Dropdown.OptionData("16"),
				new Dropdown.OptionData("32"),
				new Dropdown.OptionData("64"),
				new Dropdown.OptionData("128"),
				new Dropdown.OptionData("256"),
				new Dropdown.OptionData("512"),
				new Dropdown.OptionData("1024"),
				new Dropdown.OptionData("2048")
			};
			_dropReflectionsResolution = AddDropdown("Resolution", options4);
			_dropReflectionsResolution.value = GlobalPreferences.ReflectionResolution.value;
			_dropReflectionsResolution.onValueChanged.AddListener(_003CStart_003Eb__48_44);
			AddHeader("Build Information");
			AddReadOnly("Engine Version", Application.unityVersion);
			AddReadOnly("Game Version", Application.version);
			AddReadOnly("Scripting Backend", "Mono");
			AddHeader("System Information");
			AddReadOnly("Platform", SystemInfo.operatingSystem);
			AddReadOnly("Processor", SystemInfo.processorType);
			AddReadOnly("3D Accelerator", SystemInfo.graphicsDeviceName);
			AddReadOnly("Rendering API", SystemInfo.graphicsDeviceVersion);
			AddHeader("System Feature Support");
			AddReadOnly("Async Compute", SystemInfo.supportsAsyncCompute ? "Yes" : "No");
			AddReadOnly("Compute Shader", SystemInfo.supportsComputeShaders ? "Yes" : "No");
			AddReadOnly("Ray Tracing", SystemInfo.supportsRayTracing ? "Yes" : "No");
			UpdateValues();
			SetupFpsSensitivity();
			SetupMultiSampleAntiAliasingSensitivity();
			SetupPostProcessingSensitivity(_postProcessToggle.isOn);
			initialized = true;
		}

		private void SetQualityPreset(int v)
		{
			switch (v)
			{
			case 0:
				return;
			case 1:
			{
				_detailDistanceSlider.slider.value = 0.5f;
				Dropdown dropAnisotropic2 = _dropAnisotropic;
				Dropdown dropReflectionsResolution = _dropReflectionsResolution;
				Dropdown dropReflectionsUpdate = _dropReflectionsUpdate;
				Dropdown dropAntiAlias3 = _dropAntiAlias;
				int num5 = (_dropShadowResolution.value = 0);
				int num7 = (dropAntiAlias3.value = num5);
				int num9 = (dropReflectionsUpdate.value = num7);
				int value = (dropReflectionsResolution.value = num9);
				dropAnisotropic2.value = value;
				Slider slider2 = _pixelLightSlider.slider;
				value = (_dropRenderMode.value = 1);
				slider2.value = value;
				_dropTextureQuality.value = 3;
				Toggle postProcessToggle3 = _postProcessToggle;
				Toggle shadows3 = _shadows;
				Toggle softShadows3 = _softShadows;
				Toggle softParticles2 = _softParticles;
				Toggle smokeToggle3 = _smokeToggle;
				Toggle highDynamicRange3 = _highDynamicRange;
				bool flag2 = (_fog.toggle.isOn = false);
				bool flag4 = (highDynamicRange3.isOn = flag2);
				bool flag6 = (smokeToggle3.isOn = flag4);
				bool flag8 = (softParticles2.isOn = flag6);
				bool flag10 = (softShadows3.isOn = flag8);
				bool isOn = (shadows3.isOn = flag10);
				postProcessToggle3.isOn = isOn;
				break;
			}
			case 2:
			{
				Slider slider = _detailDistanceSlider.slider;
				Dropdown dropAntiAlias2 = _dropAntiAlias;
				Dropdown dropAnisotropic = _dropAnisotropic;
				Dropdown dropTextureQuality = _dropTextureQuality;
				int num5 = (_dropRenderMode.value = 1);
				int num7 = (dropTextureQuality.value = num5);
				int num9 = (dropAnisotropic.value = num7);
				int value = (dropAntiAlias2.value = num9);
				slider.value = value;
				Dropdown dropShadowCascades2 = _dropShadowCascades;
				Dropdown dropShadowResolution = _dropShadowResolution;
				num9 = (_dropReflectionsUpdate.value = 2);
				value = (dropShadowResolution.value = num9);
				dropShadowCascades2.value = value;
				_dropReflectionsResolution.value = 3;
				_pixelLightSlider.slider.value = 4f;
				Toggle postProcessToggle2 = _postProcessToggle;
				Toggle shadows2 = _shadows;
				Toggle smokeToggle2 = _smokeToggle;
				Toggle highDynamicRange2 = _highDynamicRange;
				bool flag6 = (_fog.toggle.isOn = true);
				bool flag8 = (highDynamicRange2.isOn = flag6);
				bool flag10 = (smokeToggle2.isOn = flag8);
				bool isOn = (shadows2.isOn = flag10);
				postProcessToggle2.isOn = isOn;
				Toggle softShadows2 = _softShadows;
				isOn = (_softParticles.isOn = false);
				softShadows2.isOn = isOn;
				break;
			}
			case 3:
			{
				_detailDistanceSlider.slider.value = 2f;
				Dropdown dropAntiAlias = _dropAntiAlias;
				int value = (_dropTextureQuality.value = 0);
				dropAntiAlias.value = value;
				Dropdown dropShadowCascades = _dropShadowCascades;
				value = (_dropAnisotropic.value = 2);
				dropShadowCascades.value = value;
				Dropdown dropRenderMode = _dropRenderMode;
				value = (_dropReflectionsUpdate.value = 3);
				dropRenderMode.value = value;
				_dropShadowResolution.value = 4;
				_dropReflectionsResolution.value = 6;
				_pixelLightSlider.slider.value = 16f;
				Toggle postProcessToggle = _postProcessToggle;
				Toggle shadows = _shadows;
				Toggle softShadows = _softShadows;
				Toggle softParticles = _softParticles;
				Toggle smokeToggle = _smokeToggle;
				Toggle highDynamicRange = _highDynamicRange;
				bool flag2 = (_fog.toggle.isOn = true);
				bool flag4 = (highDynamicRange.isOn = flag2);
				bool flag6 = (smokeToggle.isOn = flag4);
				bool flag8 = (softParticles.isOn = flag6);
				bool flag10 = (softShadows.isOn = flag8);
				bool isOn = (shadows.isOn = flag10);
				postProcessToggle.isOn = isOn;
				break;
			}
			}
			_dropQuality.value = 0;
		}

		private void UpdateSoftParticles(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetSoftParticles(v);
			}
			if (write)
			{
				GlobalPreferences.SoftParticles.value = v;
			}
		}

		private void SetPixelLightCount(float v, bool write = false)
		{
			SetInputSliderInput(_pixelLightSlider, v, "0");
			int num = (int)v;
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetPixelLights(num);
			}
			if (write)
			{
				GlobalPreferences.PixelLightCount.value = num;
			}
		}

		private void SetAnisotropicTexture(int v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetAnisotropicTexture(v);
			}
			if (write)
			{
				GlobalPreferences.AnisotropicTexture.value = v;
			}
		}

		private void UpdateHighDynamicRange(bool value, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetCameraHighDynamicRange(value);
			}
			if (write)
			{
				GlobalPreferences.HDR.value = value;
			}
		}

		private void UpdateAntiAliasingMode(int v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetMultiSampleAntiAliasing(v);
			}
			if (write)
			{
				GlobalPreferences.Msaa.value = v;
			}
		}

		private void OnEnable()
		{
			if (_dropRes != null)
			{
				_dropRes.value = FindCurrentResolutionListing();
			}
		}

		private void OnDisable()
		{
			if (_dropRes != null && _dropRes.options.Count > Screen.resolutions.Length)
			{
				_dropRes.options.RemoveAt(_dropRes.options.Count - 1);
			}
		}

		protected override void UpdateValues()
		{
			if ((bool)_cameraEffects)
			{
				_depthOfFieldToggle.isOn = _cameraEffects.GetDepthOfField();
				_motionBlurToggle.isOn = CameraEffectsSettings.GetMotionBlur();
				_shadowDistance.slider.value = _cameraEffects.GetShadowDistance();
			}
			else
			{
				_aoToggleSlider.slider.value = GlobalPreferences.AmbientOcclusionValue.value;
				_depthOfFieldToggle.isOn = GlobalPreferences.DepthOfField.value;
				_motionBlurToggle.isOn = GlobalPreferences.MotionBlur.value;
				_shadowDistance.slider.value = GlobalPreferences.ShadowDistance.value;
			}
			_detailDistanceSlider.slider.value = GlobalPreferences.ViewDistance.value;
			_smokeToggle.isOn = GlobalPreferences.SmokeEnabled.value;
			_toggleFps.isOn = (mainCamera ? GuiManager.FPSDisplayEnabled : GlobalPreferences.FpsCount.value);
			_postProcessToggle.isOn = GlobalPreferences.PostProcessing.value;
			_aoToggleSlider.toggle.isOn = GlobalPreferences.AmbientOcclusion.value;
			_aoToggleSlider.slider.value = GlobalPreferences.AmbientOcclusionValue.value;
			_bloomSlider.toggle.isOn = GlobalPreferences.Bloom.value;
			_bloomSlider.slider.value = GlobalPreferences.BloomValue.value;
			_colorGrading.isOn = GlobalPreferences.ColorGrading.value;
			_colorBrightnessSlider.slider.value = GlobalPreferences.ColorBrightness.value;
			_colorContrastSlider.slider.value = GlobalPreferences.ColorContrast.value;
			_colorSaturationSlider.slider.value = GlobalPreferences.ColorSaturation.value;
			_chromaticAberrationSlider.toggle.isOn = GlobalPreferences.ChromaticAberration.value;
			_chromaticAberrationSlider.slider.value = GlobalPreferences.ChromaticAberrationValue.value;
			_fastApproximateAntiAliasingToggle.isOn = GlobalPreferences.FastApproximateAntiAliasing.value;
			_vignetteSlider.toggle.isOn = GlobalPreferences.Vignette.value;
			_vignetteSlider.slider.value = GlobalPreferences.VignetteValue.value;
		}

		private void UpdatePostProcessing(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetPostProcessing(v);
			}
			if (write)
			{
				GlobalPreferences.PostProcessing.value = v;
			}
			SetupPostProcessingSensitivity(v);
		}

		private void UpdateFastApproximateAntiAliasing(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetFastApproximateAntiAliasing(v);
			}
			if (write)
			{
				GlobalPreferences.FastApproximateAntiAliasing.value = v;
			}
		}

		private void UpdateBloomSlider(float v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetBloom(v);
			}
			if (write)
			{
				GlobalPreferences.BloomValue.value = v;
			}
		}

		private void UpdateBloomToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetBloom(v);
			}
			if (write)
			{
				GlobalPreferences.Bloom.value = v;
			}
		}

		private void UpdateColorGrading(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorGrading(v);
			}
			if (write)
			{
				GlobalPreferences.ColorGrading.value = v;
			}
			SetupColorGradingSensitivity();
		}

		private void UpdateColorAces(bool v, bool write)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorAcademyColorEncodingSystem(v);
			}
			if (write)
			{
				GlobalPreferences.ColorAces.value = v;
			}
		}

		private void UpdateColorHdr(bool v, bool write)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorHighDefinitionRange(v);
			}
			if (write)
			{
				GlobalPreferences.ColorHDR.value = v;
			}
			SetupColorGradingSensitivity();
		}

		private void UpdateColorBrightness(float v, bool write = false)
		{
			SetInputSliderInput(_colorBrightnessSlider, v, "0.00");
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorBrightness(v);
			}
			if (write)
			{
				GlobalPreferences.ColorBrightness.value = v;
			}
		}

		private void UpdateColorContrast(float v, bool write = false)
		{
			SetInputSliderInput(_colorContrastSlider, v, "0.00");
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorContrast(v);
			}
			if (write)
			{
				GlobalPreferences.ColorContrast.value = v;
			}
		}

		private void UpdateColorSaturation(float v, bool write = false)
		{
			SetInputSliderInput(_colorSaturationSlider, v, "0.00");
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetColorSaturation(v);
			}
			if (write)
			{
				GlobalPreferences.ColorSaturation.value = v;
			}
		}

		private void OnFovChanged(float value)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetFieldOfView(value);
			}
			SetInputSliderInput(_fovSlider, value);
			GlobalPreferences.Fov.value = value;
		}

		private void UpdateFpsSlider(InputSlider i, float value, bool write = false)
		{
			int num2 = (Application.targetFrameRate = (int)value);
			if (write)
			{
				GlobalPreferences.Fps.value = num2;
			}
			SetInputSliderInput(i, num2);
		}

		private void UpdateUiScaleSlider(string s, bool write = false)
		{
			float result;
			if (float.TryParse(s, out result))
			{
				if (result < 0.5f)
				{
					result = 0.5f;
				}
				else if (result > 1.1f)
				{
					result = 1.1f;
				}
				UpdateUiScaleSlider(result, write);
			}
		}

		private void UpdateUiScaleSlider(float value, bool write = false)
		{
			value = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);
			CameraSettings.SetUiScale(value);
			SetInputSliderInput(_uiScaleSlider, value, "0.00");
			if (write)
			{
				GlobalPreferences.UIScale.value = value;
			}
		}

		private void UpdateShowFpsToggle(bool value, bool write = false)
		{
			if ((bool)main)
			{
				main.ShowFPS(value);
			}
			if (write)
			{
				GlobalPreferences.FpsCount.value = value;
			}
		}

		private void UpdateBlurToggle(bool value, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetMotionBlur(value);
			}
			if (write)
			{
				GlobalPreferences.MotionBlur.value = value;
			}
		}

		private void SetTextureQualityLevel(int v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetTextureQuality(v);
			}
			if (write)
			{
				GlobalPreferences.TextureQuality.value = v;
			}
		}

		private void UpdateAoToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetAmbientOcclusion(v);
			}
			if (write)
			{
				GlobalPreferences.AmbientOcclusion.value = v;
			}
			SetupAmbientOcclusionSensitivity();
		}

		private void UpdateAoSlider(float v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetAmbientOcclusion(v);
			}
			if (write)
			{
				GlobalPreferences.AmbientOcclusionValue.value = v;
			}
		}

		private void UpdateAoVolumetric(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetAmbientOcclusionVolumetric(v);
			}
			if (write)
			{
				GlobalPreferences.AmbientOcclusionVolumetric.value = v;
			}
		}

		private void UpdateFogToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetFog(v);
			}
			if (write)
			{
				GlobalPreferences.Fog.value = v;
			}
		}

		private void UpdateFogSkyBoxToggle(bool value, bool write)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetFogSkyBox(value);
			}
			if (write)
			{
				GlobalPreferences.FogSkyBox.value = value;
			}
		}

		private void UpdateFogSlider(float v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetFog(v);
			}
			if (write)
			{
				GlobalPreferences.FogPercent.value = v;
			}
		}

		private void UpdateFogExponential(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetupFogType(v);
			}
			if (write)
			{
				GlobalPreferences.FogExponential.value = v;
			}
		}

		private void UpdateChromaticAberrationToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetChromaticAberration(v);
			}
			if (write)
			{
				GlobalPreferences.ChromaticAberration.value = v;
			}
		}

		private void UpdateChromaticAberrationSlider(float v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetChromaticAberration(v);
			}
			if (write)
			{
				GlobalPreferences.ChromaticAberrationValue.value = v;
			}
		}

		private void UpdateReflectionResolution(int value, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetReflectionResolution(value);
			}
			if (write)
			{
				GlobalPreferences.ReflectionResolution.value = value;
			}
		}

		private void UpdateReflectionMode(int value, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetReflectionMode(value);
			}
			if (write)
			{
				GlobalPreferences.ReflectionMode.value = value;
			}
		}

		private void UpdateShadowToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetShadows(v);
			}
			if (write)
			{
				GlobalPreferences.Shadows.value = v;
			}
		}

		private void UpdateShadowDistanceSlider(float v, bool write = false)
		{
			SetInputSliderInput(_shadowDistance, "0.00");
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetShadows(v);
			}
			if (write)
			{
				GlobalPreferences.ShadowDistance.value = v;
			}
		}

		private void SetShadowCascades(int i, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetShadowCascade(i);
			}
			if (write)
			{
				GlobalPreferences.ShadowCascade.value = i;
			}
		}

		private void SetShadowResolution(int i, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetShadowResolution(i);
			}
			if (write)
			{
				GlobalPreferences.ShadowResolution.value = i;
			}
		}

		private void UpdateVignetteToggle(bool v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetVignette(v);
			}
			if (write)
			{
				GlobalPreferences.Vignette.value = v;
			}
		}

		private void UpdateVignetteSlider(float v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetVignette(v);
			}
			if (write)
			{
				GlobalPreferences.VignetteValue.value = v;
			}
		}

		private void UpdateRenderMode(int v, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetCameraRenderingMode(v);
			}
			if (write)
			{
				GlobalPreferences.RenderMode.value = v;
			}
			SetupMultiSampleAntiAliasingSensitivity();
			SetupHighDynamicRangeSensitivity();
		}

		private void OnOffScreenUpdate(int value, bool write = false)
		{
			if (value > 1)
			{
				UiMessageBox.Create("Setting off-screen updates to 'Always' may result in bad performance as it will render every Macro in the scene even when they can't be seen by the camera. If you are experiencing culling problems with a model then ideally you should fix the model and re-import it.\n\nOnly use the 'Always' setting as a last resort", "Performance Warning").Popup();
			}
			if (!SettingsView.IsMainMenu)
			{
				CameraEffectsSettings.SetupMacroOffScreenUpdate(value);
			}
			if (write)
			{
				GlobalPreferences.OffScreenUpdate.value = value;
			}
		}

		private void SetupFpsSensitivity()
		{
			SettingsView.SetLabelSensitivity(_fpsSlider.slider, _dropVSync.value == 0);
		}

		private void SetupMultiSampleAntiAliasingSensitivity()
		{
			SettingsView.SetLabelSensitivity(_dropAntiAlias, _dropRenderMode.value < 2);
		}

		private void SetupHighDynamicRangeSensitivity()
		{
			bool sensitivity = _dropRenderMode.value > 1 || _postProcessToggle.isOn;
			SettingsView.SetLabelSensitivity(_highDynamicRange, sensitivity);
		}

		private void SetupColorGradingSensitivity()
		{
			bool flag = _postProcessToggle.isOn && _colorGrading.isOn;
			bool flag2 = flag && _colorHighDefinitionRange.isOn;
			SettingsView.SetLabelSensitivity(_colorBrightnessSlider.slider, flag && !flag2);
			SettingsView.SetLabelSensitivity(_colorContrastSlider.slider, flag);
			SettingsView.SetLabelSensitivity(_colorSaturationSlider.slider, flag);
			SettingsView.SetLabelSensitivity(_colorHighDefinitionRange, flag);
			SettingsView.SetLabelSensitivity(_colorAcademyColorEncodingSystem, flag2);
		}

		private void SetupAmbientOcclusionSensitivity()
		{
			bool sensitivity = _postProcessToggle.isOn && _aoToggleSlider.toggle.isOn;
			SettingsView.SetLabelSensitivity(_aoVolumetric, sensitivity);
		}

		private void SetupPostProcessingSensitivity(bool sensitive)
		{
			SettingsView.SetLabelSensitivity(_aoToggleSlider.slider, sensitive);
			SettingsView.SetLabelSensitivity(_bloomSlider.slider, sensitive);
			SettingsView.SetLabelSensitivity(_chromaticAberrationSlider.slider, sensitive);
			SettingsView.SetLabelSensitivity(_colorGrading, sensitive);
			SettingsView.SetLabelSensitivity(_depthOfFieldToggle, sensitive);
			SettingsView.SetLabelSensitivity(_fastApproximateAntiAliasingToggle, sensitive);
			SettingsView.SetLabelSensitivity(_motionBlurToggle, sensitive);
			SettingsView.SetLabelSensitivity(_vignetteSlider.slider, sensitive);
			SetupColorGradingSensitivity();
			SetupAmbientOcclusionSensitivity();
			SetupHighDynamicRangeSensitivity();
		}

		private void UpdateVerticalSynchronisationMode(int a, bool write = false)
		{
			QualitySettings.vSyncCount = a;
			if (write)
			{
				GlobalPreferences.Vsync.value = a;
			}
			SetupFpsSensitivity();
		}

		private void UpdateDistance(float value, bool write = false)
		{
			QualitySettings.lodBias = value;
			SetInputSliderInput(_detailDistanceSlider, value, "0.00");
			if (write)
			{
				GlobalPreferences.ViewDistance.value = value;
			}
		}

		private static void OnCameraShakeChanged(bool value)
		{
			GlobalPreferences.CameraShakeEnabled.value = value;
		}

		private static void OnCameraShakeChanged(float value)
		{
			GlobalPreferences.CameraShakeMultiplier.value = value;
		}

		private static void UpdateSmokeToggleChanged(bool value)
		{
			CityBuilding.smokeEffectEnabled = value;
			GlobalPreferences.SmokeEnabled.value = value;
		}

		private static List<string> StringResolutions()
		{
			return Screen.resolutions.Select(Res2Str).ToList().Distinct()
				.ToList();
		}

		private static Resolution WindowResolution()
		{
			Resolution result = default(Resolution);
			result.width = Screen.width;
			result.height = Screen.height;
			return result;
		}

		private int FindCurrentResolutionListing()
		{
			int num = 0;
			string text = Res2Str(WindowResolution());
			if (_dropRes != null && _dropRes.options != null)
			{
				foreach (Dropdown.OptionData option in _dropRes.options)
				{
					if (string.CompareOrdinal(text, option.text) == 0)
					{
						return num;
					}
					num++;
				}
				_dropRes.options.Add(new Dropdown.OptionData(text));
			}
			return num;
		}

		private static string Res2Str(Resolution res)
		{
			return res.width + "x" + res.height;
		}

		private static Resolution Str2Res(string str)
		{
			string[] array = str.Split('x');
			Resolution result = default(Resolution);
			result.width = int.Parse(array[0]);
			result.height = int.Parse(array[1]);
			return result;
		}

		private void UpdateDepthOfFieldToggle(bool value, bool write = false)
		{
			if ((bool)_cameraEffects)
			{
				_cameraEffects.SetDepthOfField(value);
			}
			if (write)
			{
				GlobalPreferences.DepthOfField.value = value;
			}
		}

		private void SetRes(int value)
		{
			Resolution resolution = Str2Res(_dropRes.options[value].text);
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
		}

		private void SetFullScreen(bool fullScreen)
		{
			Screen.fullScreen = fullScreen;
		}

		private int GetVr3dModeIndex()
		{
			string loadedDeviceName = XRSettings.loadedDeviceName;
			int num = 0;
			int result = 0;
			string[] supportedDevices = XRSettings.supportedDevices;
			foreach (string text in supportedDevices)
			{
				if (loadedDeviceName == text)
				{
					result = num;
				}
				num++;
			}
			return result;
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_45(int value)
		{
			_vrCamera.SelectXR(value);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_0(float v)
		{
			UpdateFpsSlider(_fpsSlider, v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_1(string x)
		{
			UpdateUiScaleSlider(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_2(float x)
		{
			UpdateUiScaleSlider(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_3(int value)
		{
			UpdateVerticalSynchronisationMode(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_4(int value)
		{
			SetAnisotropicTexture(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_5(float value)
		{
			UpdateDistance(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_6(int value)
		{
			UpdateAntiAliasingMode(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_7(float x)
		{
			SetPixelLightCount(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_8(int x)
		{
			SetTextureQualityLevel(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_9(float v)
		{
			UpdateFogSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_10(bool v)
		{
			UpdateFogToggle(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_11(bool x)
		{
			UpdateFogExponential(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_12(bool x)
		{
			UpdateFogSkyBoxToggle(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_13(bool value)
		{
			UpdateHighDynamicRange(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_14(bool x)
		{
			UpdateSoftParticles(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_15(bool x)
		{
			GlobalPreferences.Shadows.value = x;
			UpdateShadowToggle(GlobalPreferences.Shadows.value);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_16(int x)
		{
			SetShadowCascades(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_17(float v)
		{
			UpdateShadowDistanceSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_18(int x)
		{
			SetShadowResolution(x, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_19(bool x)
		{
			GlobalPreferences.SoftShadows.value = x;
			UpdateShadowToggle(GlobalPreferences.Shadows.value);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_20(bool value)
		{
			UpdateShowFpsToggle(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_21(int value)
		{
			OnOffScreenUpdate(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_22(int v)
		{
			UpdateRenderMode(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_24(bool v)
		{
			UpdatePostProcessing(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_25(bool v)
		{
			UpdateAoToggle(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_26(float v)
		{
			UpdateAoSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_27(bool v)
		{
			UpdateAoVolumetric(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_28(float v)
		{
			UpdateBloomSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_29(bool v)
		{
			UpdateBloomToggle(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_30(bool v)
		{
			UpdateChromaticAberrationToggle(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_31(float v)
		{
			UpdateChromaticAberrationSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_32(bool v)
		{
			UpdateColorGrading(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_33(bool v)
		{
			UpdateColorHdr(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_34(bool v)
		{
			UpdateColorAces(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_35(float v)
		{
			UpdateColorBrightness(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_36(float v)
		{
			UpdateColorContrast(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_37(float v)
		{
			UpdateColorSaturation(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_38(bool value)
		{
			UpdateDepthOfFieldToggle(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_39(bool v)
		{
			UpdateFastApproximateAntiAliasing(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_40(bool value)
		{
			UpdateBlurToggle(value, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_41(bool v)
		{
			UpdateVignetteToggle(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_42(float v)
		{
			UpdateVignetteSlider(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_43(int v)
		{
			UpdateReflectionMode(v, true);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__48_44(int v)
		{
			UpdateReflectionResolution(v, true);
		}
	}
}
