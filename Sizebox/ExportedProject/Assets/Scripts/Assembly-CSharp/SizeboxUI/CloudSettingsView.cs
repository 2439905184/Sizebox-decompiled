using System.Runtime.CompilerServices;
using Pause;
using UnityEngine;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class CloudSettingsView : SettingsView
	{
		private Button _spawnButton;

		private Button _clearButton;

		private Toggle _useSimpleClouds;

		private Toggle _overcast;

		private InputSlider _altitude;

		private InputSlider _length;

		private InputSlider _width;

		private InputSlider _height;

		private Slider _scale;

		private Toggle _receiveActiveShadows;

		private Toggle _enablePhysics;

		private Toggle _volumetricCloudCompression;

		private Toggle _disableElasticity;

		private Toggle _lockAltitude;

		private Slider _gtsAirCompression;

		private Toggle _lodEnabled;

		private InputSlider _lodRange;

		private InputSlider _cloudCover;

		private InputSlider _spawnRange;

		private Toggle _spawnAdvancedClouds;

		private Toggle _deleteAdvancedClouds;

		private void Start()
		{
			base.Title = "Advanced Clouds";
			UiMessageBox.Create("Advanced Clouds might become very taxing on your machine, tinker with care!", "Experimental feature").Popup();
			_useSimpleClouds = AddToggle("Simple Clouds");
			_useSimpleClouds.onValueChanged.AddListener(OnUseSimpleCloudsChanged);
			_overcast = AddToggle("Overcast");
			_overcast.onValueChanged.AddListener(OnOvercastChanged);
			_altitude = AddInputSlider("Altitude", 0f, 20f);
			_altitude.slider.onValueChanged.AddListener(OnAltitudeChanged);
			_length = AddInputSlider("Length", 5f, 50f, true);
			_length.slider.onValueChanged.AddListener(_003CStart_003Eb__21_0);
			_width = AddInputSlider("Width", 5f, 50f, true);
			_width.slider.onValueChanged.AddListener(_003CStart_003Eb__21_1);
			_height = AddInputSlider("Height", 1f, 10f, true);
			_height.slider.onValueChanged.AddListener(_003CStart_003Eb__21_2);
			_scale = AddSlider("Scale", 1f, 40000f);
			_scale.onValueChanged.AddListener(OnScaleChanged);
			_receiveActiveShadows = AddToggle("Receive Active Shadows");
			_receiveActiveShadows.onValueChanged.AddListener(OnReceiveActiveShadowsChanged);
			_enablePhysics = AddToggle("Enable Physics");
			_enablePhysics.onValueChanged.AddListener(OnEnablePhysicsChanged);
			_volumetricCloudCompression = AddToggle("Volumetric Cloud Compression");
			_volumetricCloudCompression.onValueChanged.AddListener(OnVolumetricCloudCompressionChanged);
			_disableElasticity = AddToggle("Disable Elasticity");
			_disableElasticity.onValueChanged.AddListener(OnDisableElasticityChanged);
			_lockAltitude = AddToggle("Lock Altitude");
			_lockAltitude.onValueChanged.AddListener(OnLockAltitudeChanged);
			_gtsAirCompression = AddSlider("GTS Air Compression", 1f, 7f);
			_gtsAirCompression.onValueChanged.AddListener(OnMacroAirCompressionChanged);
			_lodEnabled = AddToggle("LOD Enabled");
			_lodEnabled.onValueChanged.AddListener(OnLevelOfDetailEnabledChanged);
			_lodRange = AddInputSlider("LOD Mid Range", 1500f, 150000f);
			_lodRange.slider.onValueChanged.AddListener(OnLevelOfDetailMidRangeChanged);
			_cloudCover = AddInputSlider("Cloud Cover", 1f, 10f, true);
			_cloudCover.slider.onValueChanged.AddListener(_003CStart_003Eb__21_3);
			_spawnRange = AddInputSlider("Spawn Range", 0f, 5f, true);
			_spawnRange.slider.onValueChanged.AddListener(_003CStart_003Eb__21_4);
			_spawnButton = AddButton("Spawn Clouds");
			_spawnButton.onClick.AddListener(Spawn);
			_clearButton = AddButton("Clear Clouds");
			_clearButton.onClick.AddListener(Clear);
			UpdateValues();
			initialized = true;
		}

		private void Spawn()
		{
			GameController.cloudController.Populate = true;
		}

		private void Clear()
		{
			GameController.cloudController.WipeClouds = true;
		}

		private void OnUseSimpleCloudsChanged(bool val)
		{
			GameController.cloudController.UseSimpleClouds = val;
		}

		private void OnOvercastChanged(bool val)
		{
			GameController.cloudController.Overcast = val;
		}

		private void OnAltitudeChanged(float val)
		{
			GameController.cloudController.CloudAltitude = val;
			SetInputSliderInput(_altitude, "0");
		}

		private void OnLengthChanged(int val)
		{
			GameController.cloudController.CloudSpawnAreaRandomX = val;
			SetInputSliderInput(_length);
		}

		private void OnWidthChanged(int val)
		{
			GameController.cloudController.CloudSpawnAreaRandomZ = val;
			SetInputSliderInput(_width);
		}

		private void OnHeightChanged(int val)
		{
			GameController.cloudController.CloudSpawnAreaRandomHieght = val;
			SetInputSliderInput(_height);
		}

		private void OnScaleChanged(float val)
		{
			GameController.cloudController.CloudScale = val;
		}

		private void OnReceiveActiveShadowsChanged(bool val)
		{
			GameController.cloudController.ActiveShadows = val;
		}

		private void OnEnablePhysicsChanged(bool val)
		{
			GameController.cloudController.CloudPhysics = val;
		}

		private void OnVolumetricCloudCompressionChanged(bool val)
		{
			GameController.cloudController.CloudCompression = val;
		}

		private void OnDisableElasticityChanged(bool val)
		{
			GameController.cloudController.DisableCloudElastisity = val;
		}

		private void OnLockAltitudeChanged(bool val)
		{
			GameController.cloudController.LockCloudTileElevation = val;
		}

		private void OnMacroAirCompressionChanged(float val)
		{
			GameController.cloudController.GtsAirComressionFactor = val;
		}

		private void OnLevelOfDetailEnabledChanged(bool val)
		{
			GameController.cloudController.CloudLODEnabled = val;
		}

		private void OnLevelOfDetailMidRangeChanged(float val)
		{
			GameController.cloudController.MidDistanceLOD = val;
			SetInputSliderInput(_lodRange);
		}

		private void OnCloudCoverChanged(int val)
		{
			GameController.cloudController.SkyFill = val;
			SetInputSliderInput(_cloudCover);
		}

		private void OnSpawnRangeChanged(int val)
		{
			GameController.cloudController.CloudHubSpawnRange = val;
			SetInputSliderInput(_spawnRange, "0");
		}

		protected override void UpdateValues()
		{
			_useSimpleClouds.isOn = GameController.cloudController.UseSimpleClouds;
			_overcast.isOn = GameController.cloudController.Overcast;
			_altitude.slider.value = GameController.cloudController.CloudAltitude;
			_length.slider.value = GameController.cloudController.CloudSpawnAreaRandomX;
			_width.slider.value = GameController.cloudController.CloudSpawnAreaRandomZ;
			_height.slider.value = GameController.cloudController.CloudSpawnAreaRandomHieght;
			_scale.value = GameController.cloudController.CloudScale;
			_receiveActiveShadows.isOn = GameController.cloudController.ActiveShadows;
			_enablePhysics.isOn = GameController.cloudController.CloudPhysics;
			_volumetricCloudCompression.isOn = GameController.cloudController.CloudCompression;
			_disableElasticity.isOn = GameController.cloudController.DisableCloudElastisity;
			_lockAltitude.isOn = GameController.cloudController.LockCloudTileElevation;
			_gtsAirCompression.value = GameController.cloudController.GtsAirComressionFactor;
			_lodEnabled.isOn = GameController.cloudController.CloudLODEnabled;
			_lodRange.slider.value = GameController.cloudController.MidDistanceLOD;
			_cloudCover.slider.value = GameController.cloudController.SkyFill;
			_spawnRange.slider.value = GameController.cloudController.CloudHubSpawnRange;
			SetInputSliderInput(_altitude, "0");
			SetInputSliderInput(_length);
			SetInputSliderInput(_width);
			SetInputSliderInput(_height);
			SetInputSliderInput(_lodRange);
			SetInputSliderInput(_cloudCover);
			SetInputSliderInput(_spawnRange, "0");
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__21_0(float v)
		{
			OnLengthChanged(Mathf.CeilToInt(v));
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__21_1(float v)
		{
			OnWidthChanged(Mathf.CeilToInt(v));
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__21_2(float v)
		{
			OnHeightChanged(Mathf.CeilToInt(v));
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__21_3(float v)
		{
			OnCloudCoverChanged(Mathf.CeilToInt(v));
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__21_4(float v)
		{
			OnSpawnRangeChanged(Mathf.CeilToInt(v));
		}
	}
}
