using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class CameraEffectsSettings : CameraSettings
{
	private float _shadowDistance = 200f;

	public float defaultNearPlane = 0.01f;

	private float farPlaneRatio = 20000f;

	public Color fogColor = Color.white;

	public PostProcessLayer ppLayer;

	private PostProcessProfile _ppProfile;

	public PostProcessVolume ppVolume;

	private ReflectionProbe _reflectionProbe;

	private ReflectionProbe _distanceReflectionProbe;

	private Transform _primaryCameraTransform;

	public Camera farCamera;

	private AmbientOcclusion _ambientOcclusion;

	private UnityEngine.Rendering.PostProcessing.Bloom _bloom;

	private ChromaticAberration _chromaticAberration;

	private ColorGrading _colorGrading;

	private UnityEngine.Rendering.PostProcessing.DepthOfField _depthOfField;

	private UnityEngine.Rendering.PostProcessing.MotionBlur _motionBlur;

	private Vignette _vignette;

	private void InitEffects()
	{
		_ppProfile.TryGetSettings<AmbientOcclusion>(out _ambientOcclusion);
		_ppProfile.TryGetSettings<UnityEngine.Rendering.PostProcessing.Bloom>(out _bloom);
		_ppProfile.TryGetSettings<ColorGrading>(out _colorGrading);
		_ppProfile.TryGetSettings<ChromaticAberration>(out _chromaticAberration);
		_ppProfile.TryGetSettings<UnityEngine.Rendering.PostProcessing.DepthOfField>(out _depthOfField);
		_ppProfile.TryGetSettings<UnityEngine.Rendering.PostProcessing.MotionBlur>(out _motionBlur);
		_ppProfile.TryGetSettings<Vignette>(out _vignette);
	}

	private void LoadEffects()
	{
		_reflectionProbe = GetComponent<ReflectionProbe>();
		_distanceReflectionProbe = base.transform.Find("Far Camera").GetComponent<ReflectionProbe>();
		SetAmbientOcclusion(GlobalPreferences.AmbientOcclusion.value);
		SetAmbientOcclusion(GlobalPreferences.AmbientOcclusionValue.value);
		SetAmbientOcclusionVolumetric(GlobalPreferences.AmbientOcclusionVolumetric.value);
		SetBloom(GlobalPreferences.Bloom.value);
		SetBloom(GlobalPreferences.BloomValue.value);
		SetColorGrading(GlobalPreferences.ColorGrading.value);
		SetColorAcademyColorEncodingSystem(GlobalPreferences.ColorAces.value);
		SetColorHighDefinitionRange(GlobalPreferences.ColorHDR.value);
		SetColorBrightness(GlobalPreferences.ColorBrightness.value);
		SetColorContrast(GlobalPreferences.ColorContrast.value);
		SetColorSaturation(GlobalPreferences.ColorSaturation.value);
		SetChromaticAberration(GlobalPreferences.ChromaticAberration.value);
		SetChromaticAberration(GlobalPreferences.ChromaticAberrationValue.value);
		SetDepthOfField(GlobalPreferences.DepthOfField.value);
		SetFog(GlobalPreferences.Fog.value);
		SetFog(GlobalPreferences.FogPercent.value);
		SetMotionBlur(GlobalPreferences.MotionBlur.value);
		SetReflectionMode(GlobalPreferences.ReflectionMode.value);
		SetReflectionResolution(GlobalPreferences.ReflectionResolution.value);
		SetVignette(GlobalPreferences.Vignette.value);
		SetVignette(GlobalPreferences.VignetteValue.value);
	}

	protected override void Awake()
	{
		base.Awake();
		_primaryCameraTransform = primaryCamera.transform.parent;
		QualitySettings.lodBias = GlobalPreferences.ViewDistance.value;
		ppLayer = primaryCamera.GetComponent<PostProcessLayer>();
		ppVolume = primaryCamera.GetComponent<PostProcessVolume>();
		_ppProfile = ppVolume.profile;
		InitEffects();
		LoadEffects();
		SetTextureQuality(GlobalPreferences.TextureQuality.value);
		SetPixelLights(GlobalPreferences.PixelLightCount.value);
		SetFastApproximateAntiAliasing(GlobalPreferences.FastApproximateAntiAliasing.value);
		SetMultiSampleAntiAliasing(GlobalPreferences.Msaa.value);
		SetPostProcessing(GlobalPreferences.PostProcessing.value);
		SetCameraRenderingMode(GlobalPreferences.RenderMode.value);
		SetCameraHighDynamicRange(GlobalPreferences.HDR.value);
		SetShadows(GlobalPreferences.Shadows.value);
		SetShadows(GlobalPreferences.ShadowDistance.value);
		SetShadowCascade(GlobalPreferences.ShadowCascade.value);
		SetShadowResolution(GlobalPreferences.ShadowResolution.value);
		SetFog(GlobalPreferences.Fog.value);
		SetFieldOfView(GlobalPreferences.Fov.value);
	}

	public void LateUpdate()
	{
		UpdateEffectsRealtime();
	}

	public override void SetMultiSampleAntiAliasing(int v)
	{
		base.SetMultiSampleAntiAliasing(v);
		farCamera.allowMSAA = QualitySettings.antiAliasing > 0;
	}

	private void UpdateEffectsRealtime()
	{
		float y = _primaryCameraTransform.localScale.y;
		primaryCamera.nearClipPlane = defaultNearPlane * y;
		primaryCamera.farClipPlane = primaryCamera.nearClipPlane * farPlaneRatio;
		float num = primaryCamera.farClipPlane - 10f;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		farCamera.nearClipPlane = num;
		if (farCamera.nearClipPlane > farCamera.farClipPlane)
		{
			farCamera.farClipPlane = farCamera.nearClipPlane + 10f;
		}
		QualitySettings.shadowDistance = _shadowDistance * y;
		QualitySettings.shadowNearPlaneOffset = _shadowDistance * 0.01f * y;
		AdjustDepthOfField();
	}

	public void SetChromaticAberration(bool value)
	{
		if ((bool)_chromaticAberration)
		{
			_chromaticAberration.active = value;
		}
	}

	public void SetChromaticAberration(float value)
	{
		if ((bool)_chromaticAberration)
		{
			_chromaticAberration.intensity.value = value;
		}
	}

	public void SetBloom(bool value)
	{
		if ((bool)_bloom)
		{
			_bloom.active = value;
			_bloom.intensity.overrideState = true;
		}
	}

	public void SetBloom(float value)
	{
		if ((bool)_bloom)
		{
			_bloom.intensity.value = value;
		}
	}

	private void UpdateGlobalFogProperties(GlobalFog globalFogComponent)
	{
		globalFogComponent.enabled = true;
		globalFogComponent.fogShader = Shader.Find("Hidden/GlobalFog");
		globalFogComponent.useRadialDistance = true;
		globalFogComponent.distanceFog = true;
		globalFogComponent.excludeFarPixels = true;
		globalFogComponent.heightFog = false;
	}

	public void SetupFogType(bool exponential)
	{
		RenderSettings.fogMode = (exponential ? FogMode.ExponentialSquared : FogMode.Exponential);
		UpdateFog(GlobalPreferences.FogPercent.value);
	}

	private void GetFogComponents(out GlobalFog fogFar, out GlobalFog fogNear)
	{
		fogFar = farCamera.GetComponent<GlobalFog>();
		fogNear = primaryCamera.GetComponent<GlobalFog>();
	}

	private void SetupFog(bool value)
	{
		RenderSettings.fog = MapSettingInternal.enableFog && value;
		RenderSettings.fogColor = fogColor;
		SetupFogType(GlobalPreferences.FogExponential.value);
		GlobalFog fogFar;
		GlobalFog fogNear;
		GetFogComponents(out fogFar, out fogNear);
		if (RenderSettings.fog)
		{
			if (!fogFar)
			{
				fogFar = farCamera.gameObject.AddComponent<GlobalFog>();
			}
			if (!fogNear)
			{
				fogNear = primaryCamera.gameObject.AddComponent<GlobalFog>();
			}
			UpdateGlobalFogProperties(fogFar);
			UpdateGlobalFogProperties(fogNear);
			SetFogSkyBox(fogFar, GlobalPreferences.FogSkyBox.value);
		}
		else
		{
			if ((bool)fogFar)
			{
				Object.Destroy(fogFar);
			}
			if ((bool)fogNear)
			{
				Object.Destroy(fogNear);
			}
		}
	}

	public void SetFogSkyBox(bool value)
	{
		GlobalFog component = farCamera.GetComponent<GlobalFog>();
		if ((bool)component)
		{
			SetFogSkyBox(component, value);
		}
	}

	private void SetFogSkyBox(GlobalFog far, bool value)
	{
		far.excludeFarPixels = !value;
	}

	private void UpdateFog(float value)
	{
		RenderSettings.fogDensity = 0.01f * (value - 0.01f);
	}

	public void SetFog(bool value)
	{
		SetupFog(value);
		if (value)
		{
			UpdateFog(GlobalPreferences.FogPercent.value);
		}
	}

	public void SetFog(float value)
	{
		UpdateFog(value);
	}

	public void SetColorGrading(bool value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.active = value;
		}
	}

	public void SetColorHighDefinitionRange(bool value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.gradingMode.Override(value ? GradingMode.HighDefinitionRange : GradingMode.LowDefinitionRange);
		}
	}

	public void SetColorAcademyColorEncodingSystem(bool value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.tonemapper.Override((!value) ? Tonemapper.Neutral : Tonemapper.ACES);
		}
	}

	public void SetColorBrightness(float value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.brightness.value = value;
		}
	}

	public void SetColorContrast(float value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.contrast.value = value;
		}
	}

	public void SetColorSaturation(float value)
	{
		if ((bool)_colorGrading)
		{
			_colorGrading.saturation.value = value;
		}
	}

	public void SetVignette(bool value)
	{
		if ((bool)_vignette)
		{
			_vignette.active = value;
		}
	}

	public void SetVignette(float value)
	{
		if ((bool)_vignette)
		{
			_vignette.intensity.value = value;
		}
	}

	public void SetDepthOfField(bool value)
	{
		if ((bool)_depthOfField)
		{
			_depthOfField.active = value;
			_depthOfField.focalLength.value = 55f;
			_depthOfField.aperture.value = 15f;
		}
	}

	public bool GetDepthOfField()
	{
		if ((bool)_depthOfField)
		{
			return _depthOfField.active;
		}
		return false;
	}

	public void SetAmbientOcclusion(bool value)
	{
		if ((bool)_ambientOcclusion)
		{
			_ambientOcclusion.active = value;
		}
	}

	public void SetAmbientOcclusion(float value)
	{
		if ((bool)_ambientOcclusion)
		{
			_ambientOcclusion.intensity.value = value;
		}
	}

	public void SetAmbientOcclusionVolumetric(bool value)
	{
		if ((bool)_ambientOcclusion)
		{
			if (value && SystemInfo.supportsComputeShaders)
			{
				_ambientOcclusion.mode.Override(AmbientOcclusionMode.MultiScaleVolumetricObscurance);
			}
			else
			{
				_ambientOcclusion.mode.Override(AmbientOcclusionMode.ScalableAmbientObscurance);
			}
		}
	}

	public float GetShadowDistance()
	{
		return _shadowDistance;
	}

	public void SetReflectionResolution(int value)
	{
		int resolution = 16;
		switch (value)
		{
		case 1:
			resolution = 32;
			break;
		case 2:
			resolution = 64;
			break;
		case 3:
			resolution = 128;
			break;
		case 4:
			resolution = 256;
			break;
		case 5:
			resolution = 512;
			break;
		case 6:
			resolution = 1024;
			break;
		case 7:
			resolution = 2048;
			break;
		}
		_distanceReflectionProbe.resolution = resolution;
		if (_distanceReflectionProbe.enabled)
		{
			_distanceReflectionProbe.RenderProbe();
		}
		_reflectionProbe.resolution = resolution;
	}

	public void SetReflectionMode(int value)
	{
		_reflectionProbe.timeSlicingMode = ((value <= 2) ? ReflectionProbeTimeSlicingMode.IndividualFaces : ReflectionProbeTimeSlicingMode.NoTimeSlicing);
		_reflectionProbe.enabled = value > 1;
		_distanceReflectionProbe.enabled = value > 0;
		if (value == 1)
		{
			_distanceReflectionProbe.RenderProbe();
		}
	}

	public void SetFieldOfView(float degrees)
	{
		primaryCamera.fieldOfView = degrees;
		farCamera.fieldOfView = degrees;
	}

	public void SetShadows(float distance)
	{
		_shadowDistance = distance;
	}

	public void SetShadowCascade(int i)
	{
		switch (i)
		{
		default:
			i = 0;
			break;
		case 1:
			i = 2;
			break;
		case 2:
			i = 4;
			break;
		}
		QualitySettings.shadowCascades = i;
	}

	public void SetShadowResolution(int i)
	{
		switch (i)
		{
		default:
			QualitySettings.shadowResolution = ShadowResolution.Low;
			break;
		case 1:
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			break;
		case 2:
			QualitySettings.shadowResolution = ShadowResolution.High;
			break;
		case 3:
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			break;
		}
	}

	public bool GetShadowsValue()
	{
		return GlobalPreferences.Shadows.value;
	}

	public void SetPostProcessing(bool value)
	{
		if (value)
		{
			ppLayer.enabled = true;
			ppVolume.enabled = true;
		}
		else
		{
			ppLayer.enabled = false;
			ppVolume.enabled = false;
		}
	}

	public void SetFastApproximateAntiAliasing(bool value)
	{
		ppLayer.antialiasingMode = (value ? PostProcessLayer.Antialiasing.FastApproximateAntialiasing : PostProcessLayer.Antialiasing.None);
	}

	public void SetShadows(bool value)
	{
		if (value)
		{
			QualitySettings.shadows = ((!GlobalPreferences.SoftShadows.value) ? ShadowQuality.HardOnly : ShadowQuality.All);
		}
		else
		{
			QualitySettings.shadows = ShadowQuality.Disable;
		}
	}

	public void SetMotionBlur(bool value)
	{
		if ((bool)_motionBlur)
		{
			_motionBlur.active = value;
		}
	}

	public static bool GetMotionBlur()
	{
		return GlobalPreferences.MotionBlur.value;
	}

	public int GetRenderingMode()
	{
		if ((bool)primaryCamera)
		{
			switch (primaryCamera.actualRenderingPath)
			{
			case RenderingPath.VertexLit:
				return 0;
			case RenderingPath.Forward:
				return 1;
			case RenderingPath.DeferredLighting:
				return 2;
			case RenderingPath.DeferredShading:
				return 3;
			default:
				return GlobalPreferences.RenderMode.value;
			}
		}
		return GlobalPreferences.RenderMode.value;
	}

	public void SetCameraHighDynamicRange(bool value)
	{
		Camera camera = farCamera;
		bool allowHDR = (primaryCamera.allowHDR = value);
		camera.allowHDR = allowHDR;
	}

	public void SetCameraRenderingMode(int renderMode)
	{
		RenderingPath renderingPath;
		switch (renderMode)
		{
		case 0:
			renderingPath = RenderingPath.VertexLit;
			break;
		default:
			renderingPath = RenderingPath.Forward;
			break;
		case 2:
			renderingPath = RenderingPath.DeferredLighting;
			break;
		case 3:
			renderingPath = RenderingPath.DeferredShading;
			break;
		}
		farCamera.renderingPath = renderingPath;
		primaryCamera.renderingPath = renderingPath;
		SetupFog(GlobalPreferences.Fog.value);
	}

	private void AdjustDepthOfField()
	{
		if ((bool)_depthOfField && _depthOfField.active)
		{
			RaycastHit hitInfo;
			float num = (Physics.Raycast(primaryCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hitInfo, primaryCamera.farClipPlane * 0.5f) ? hitInfo.distance : 1.5f);
			if (num > 1.7f)
			{
				num = 1.7f;
			}
			else if (num < 0.7f)
			{
				num = 0.7f;
			}
			num = Mathf.Lerp(_depthOfField.focusDistance.value, num, Time.fixedDeltaTime);
			_depthOfField.focusDistance.value = num;
		}
	}

	public void SetTextureQuality(int i)
	{
		QualitySettings.masterTextureLimit = i;
	}

	public void SetPixelLights(int i)
	{
		i = ((primaryCamera.renderingPath != RenderingPath.Forward && primaryCamera.renderingPath != 0) ? Mathf.Clamp(i, 1, 64) : Mathf.Clamp(i, 1, 4));
		QualitySettings.pixelLightCount = i;
	}

	public void SetAnisotropicTexture(int i)
	{
		switch (i)
		{
		default:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			break;
		case 1:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
			break;
		case 2:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			break;
		}
	}

	public void SetSoftParticles(bool v)
	{
		QualitySettings.softParticles = v;
	}

	public static void SetupMacroOffScreenUpdate()
	{
		SetupMacroOffScreenUpdate(GlobalPreferences.OffScreenUpdate.value);
	}

	public static void SetupMacroOffScreenUpdate(int setting)
	{
		Giantess[] array = Object.FindObjectsOfType<Giantess>();
		if (setting != 1)
		{
			bool updateWhenOffScreen = setting > 1;
			Giantess[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].updateWhenOffScreen = updateWhenOffScreen;
			}
		}
		else
		{
			Giantess[] array2 = array;
			foreach (Giantess obj in array2)
			{
				obj.updateWhenOffScreen = obj.IsPosed;
			}
		}
	}
}
