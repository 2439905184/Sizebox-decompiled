public class GlobalPreferences
{
	private static GlobalPreferences _instance;

	public static readonly BoolStored ClothPhysics = Instance._enableCloth;

	private readonly StringStored _model = new StringStored("Model Path", string.Empty);

	private readonly StringStored _sound = new StringStored("Sound Path", string.Empty);

	private readonly StringStored _scene = new StringStored("Scene Path", string.Empty);

	private readonly StringStored _script = new StringStored("Script Path", string.Empty);

	private readonly StringStored _screenshot = new StringStored("Screenshot Path", string.Empty);

	private readonly StringStored _data = new StringStored("Data Path", string.Empty);

	private readonly IntStored _textureQuality = new IntStored("TextureQuality", 0);

	private readonly IntStored _pixelLightCount = new IntStored("PixelLightCount", 4);

	private readonly IntStored _anisotropic = new IntStored("AnisotropicTexture", 2);

	private readonly IntStored _msaa = new IntStored("AntiAlias", 0);

	private readonly IntStored _fps = new IntStored("Fps", 60);

	private readonly IntStored _vsync = new IntStored("Vsync", 0);

	private readonly BoolStored _shadows = new BoolStored("Shadows", true);

	private readonly IntStored _shadowCascade = new IntStored("ShadowCascade", 2);

	private readonly IntStored _shadowResolution = new IntStored("ShadowResolution", 3);

	private readonly BoolStored _softShadows = new BoolStored("SoftShadows", true);

	private readonly BoolStored _softParticles = new BoolStored("SoftParticles", true);

	private readonly BoolStored _fpsCount = new BoolStored("FpsCount", false);

	private readonly BoolStored _fog = new BoolStored("Fog", true);

	private readonly BoolStored _fogExponential = new BoolStored("FogExponential", false);

	private readonly BoolStored _fogSkyBox = new BoolStored("Fog Skybox", false);

	private readonly FloatStored _fogDistance = new FloatStored("FogPercent", 0.1f);

	private readonly FloatStored _fov = new FloatStored("Fov", 65f);

	private readonly FloatStored _shadowDistance = new FloatStored("ShadowDistance", 200f);

	private readonly FloatStored _viewDistance = new FloatStored("viewDistance", 1f);

	private readonly BoolStored _cameraShakeEnabled = new BoolStored("cameraShakeEnabled", true);

	private readonly FloatStored _cameraShakeMultiplier = new FloatStored("ShakeMultiplier", 1f);

	private readonly BoolStored _smokeEnabled = new BoolStored("SmokeEnabled", true);

	private readonly FloatStored _uiScale = new FloatStored("UIScalePercent", 1f);

	private readonly BoolStored _backgroundMaxFps = new BoolStored("BackgroundMaxFps", false);

	private readonly IntStored _offScreenUpdate = new IntStored("OffScreenUpdate", 1);

	private readonly BoolStored _enablePostProcess = new BoolStored("PostProcessing", true);

	private readonly BoolStored _ambientOcclusion = new BoolStored("AmbientOcclusion", false);

	private readonly FloatStored _ambientOcclusionValue = new FloatStored("AmbientOcclusionValue", 1f);

	private readonly BoolStored _ambientOcclusionVolumetric = new BoolStored("AmbientOcclusionVolumetric", false);

	private readonly BoolStored _bloom = new BoolStored("Bloom", true);

	private readonly FloatStored _bloomValue = new FloatStored("BloomValue", 1f);

	private readonly BoolStored _chromaticAberration = new BoolStored("ChromaticAberration", false);

	private readonly FloatStored _chromaticAberrationValue = new FloatStored("ChromaticAberrationValue", 0.2f);

	private readonly BoolStored _colorGrading = new BoolStored("ColorGain", true);

	private readonly BoolStored _colorHDR = new BoolStored("ColorHDR", true);

	private readonly BoolStored _colorAces = new BoolStored("ColorACES", true);

	private readonly FloatStored _colorBrightness = new FloatStored("ColorBrightness", 0f);

	private readonly FloatStored _colorContrast = new FloatStored("ColorContrast", 0f);

	private readonly FloatStored _colorSaturation = new FloatStored("ColorSaturation", 0f);

	private readonly BoolStored _depthOfField = new BoolStored("DepthOfField", false);

	private readonly BoolStored _fastApproximateAntiAliasing = new BoolStored("FastApproximateAntiAliasing", true);

	private readonly BoolStored _motionBlur = new BoolStored("MotionBlur", false);

	private readonly IntStored _reflectionMode = new IntStored("ReflectionMode", 1);

	private readonly IntStored _reflectionResolution = new IntStored("ReflectionResolution", 1);

	private readonly BoolStored _vignette = new BoolStored("Vignette", false);

	private readonly FloatStored _vignetteValue = new FloatStored("VignetteValue", 0.5f);

	private readonly BoolStored _backgroundAudio = new BoolStored("BackgroundAudio", true);

	private readonly FloatStored _volMaster = new FloatStored("VolMaster", 0f);

	private readonly FloatStored _volBackground = new FloatStored("VolBackground", 0f);

	private readonly FloatStored _volDestruction = new FloatStored("VolDestruction", 0f);

	private readonly FloatStored _volEffect = new FloatStored("VolEffect", 0f);

	private readonly FloatStored _volMacro = new FloatStored("VolMacro", 0f);

	private readonly FloatStored _volMicro = new FloatStored("VolMicro", 0f);

	private readonly FloatStored _volMusic = new FloatStored("VolMusic", -20f);

	private readonly FloatStored _volVoice = new FloatStored("VolVoice", 0f);

	private readonly FloatStored _volWindA = new FloatStored("VolWindA", 0f);

	private readonly FloatStored _volWindG = new FloatStored("VolWindG", 0f);

	private readonly FloatStored _volRaygun = new FloatStored("VolRaygun", 0f);

	private readonly FloatStored _volAIGuns = new FloatStored("VolAIGuns", 0f);

	private readonly BoolStored _slOpenOnEditor = new BoolStored("SL_OpenOnEditor", true);

	private readonly BoolStored _slSelObjOnEntryClick = new BoolStored("SL_SelObjOnEntryClick", true);

	private readonly BoolStored _backgroundPause = new BoolStored("BackgroundPause", true);

	private readonly BoolStored _lookAtPlayer = new BoolStored("LookAtPlayer", true);

	private readonly BoolStored _slowdownWithSize = new BoolStored("SlowdownWithSize", true);

	private readonly BoolStored _microsAffectedByBulletTime = new BoolStored("MicrosAffectedByBulletTime", true);

	private readonly BoolStored _ignorePlayer = new BoolStored("IgnorePlayer", false);

	private readonly BoolStored _bloodEnabled = new BoolStored("BloodEnabled", true);

	private readonly BoolStored _crushPlayerEnabled = new BoolStored("CrushPlayerEnabled", true);

	private readonly BoolStored _crushNpcEnabled = new BoolStored("CrushNpcEnabled", true);

	private readonly BoolStored _playerCrushingEnabled = new BoolStored("PlayerCrushingEnabled", true);

	private readonly BoolStored _npcMicroCrushingEnabled = new BoolStored("NpcMicroCrushingEnabled", true);

	private readonly BoolStored _npcGtsCrushingEnabled = new BoolStored("NpcGtsCrushingEnabled", true);

	private readonly BoolStored _crushStick = new BoolStored("CrushStick", false);

	private readonly FloatStored _crushStickChance = new FloatStored("CrushStickChance", 0.2f);

	private readonly FloatStored _crushStickDuration = new FloatStored("CrushStickDuration", 0.4f);

	private readonly FloatStored _microDurability = new FloatStored("MicroDurability", 0.5f);

	private readonly FloatStored _microSpawnSize = new FloatStored("MicroSpawnSize", 1f);

	private readonly FloatStored _crushSurviveChance = new FloatStored("CrushSurviveChance", 0.2f);

	private readonly FloatStored _crushFlatness = new FloatStored("CrushFlatness", 1f);

	private readonly FloatStored _stompSpeed = new FloatStored("StompSpeed", 1f);

	private readonly BoolStored _ragDollEnabled = new BoolStored("RagDollEnabled", true);

	private readonly IntStored _logLevel = new IntStored("LogLevel", 1);

	private readonly FloatStored _macroColliderUpdateDistance = new FloatStored("MacroColliderUpdateDistance", 350f);

	private readonly IntStored _meshCheckLimit = new IntStored("MeshCheckLimit", 100);

	private readonly IntStored _meshUpdateLimit = new IntStored("MeshUpdateLimit", 5);

	private readonly BoolStored _useAdvancedCollision = new BoolStored("UseAdvancedCollision", false);

	private readonly FloatStored _microRepositionSpeed = new FloatStored("MicroRepositionSpeed", 90f);

	private readonly BoolStored _bloodGrows = new BoolStored("BloodGrows", false);

	private readonly BoolStored _bloodShrinks = new BoolStored("BloodShrinks", true);

	private readonly FloatStored _bodyDuration = new FloatStored("BodyDuration", 120f);

	private readonly FloatStored _bloodDuration = new FloatStored("BloodDuration", 240f);

	private readonly FloatStored _mouseSensibility = new FloatStored("Mouse Sensitivity", 1f);

	private readonly BoolStored _cameraEdgeScroll = new BoolStored("Camera Edge Scrolling", false);

	private readonly BoolStored _gtsAI = new BoolStored("GtsAI", false);

	private readonly BoolStored _microAI = new BoolStored("MicroAI", true);

	private readonly BoolStored _scriptAuxLogging = new BoolStored("ScriptAuxLogging", false);

	private readonly BoolStored _hairPhysics = new BoolStored("HairPhysics", true);

	private readonly BoolStored _breastPhysics = new BoolStored("BreastPhysics", true);

	private readonly BoolStored _jigglePhysics = new BoolStored("JigglePhysics", true);

	private readonly BoolStored _blinkEnabled = new BoolStored("BlinkEnabled", true);

	private readonly BoolStored _useGrounder = new BoolStored("UseGrounder", false);

	private readonly BoolStored _secondaryDestruction = new BoolStored("SecondaryDestruction", false);

	private readonly BoolStored _keepDebris = new BoolStored("KeepDebris", false);

	private readonly BoolStored _lowEndCities = new BoolStored("LowEndCities", true);

	private readonly IntStored _cityPopulation = new IntStored("CityPopulation", 20);

	private readonly FloatStored _microSpeed = new FloatStored("MicroSpeed", 1f);

	private readonly FloatStored _bulletTimeFactor = new FloatStored("BulletTimeFactor", 0.15f);

	private readonly IntStored _lazyBatch = new IntStored("LazyBatch", 5);

	private readonly BoolStored _debrisCanCrush = new BoolStored("DebrisCanCrush", true);

	private readonly BoolStored _imperialMeasurements = new BoolStored("ImperialMeasurements", false);

	private readonly BoolStored _gtsBuildingDestruction = new BoolStored("GtsBuildingDestruction", true);

	private readonly BoolStored _microPlayerBuildingDestruction = new BoolStored("MicroPlayerBuildingDestruction", true);

	private readonly BoolStored _gtsPlayerBuildingDestruction = new BoolStored("GtsPlayerBuildingDestruction", true);

	private readonly BoolStored _enableCloth = new BoolStored("EnableCloth", false);

	private readonly FloatStored _growColorR = new FloatStored("GrowColorR", 0f);

	private readonly FloatStored _growColorG = new FloatStored("GrowColorG", 1f);

	private readonly FloatStored _growColorB = new FloatStored("GrowColorB", 0f);

	private readonly FloatStored _shrinkColorR = new FloatStored("ShrinkColorR", 1f);

	private readonly FloatStored _shrinkColorG = new FloatStored("ShrinkColorG", 0f);

	private readonly FloatStored _shrinkColorB = new FloatStored("ShrinkColorB", 1f);

	private readonly FloatStored _crossHairColorR = new FloatStored("CrossHairColorR", 1f);

	private readonly FloatStored _crossHairColorG = new FloatStored("CrossHairColorG", 1f);

	private readonly FloatStored _crossHairColorB = new FloatStored("CrossHairColorB", 1f);

	private readonly FloatStored _auxiliaryUIColorR = new FloatStored("AuxiliaryUIColorR", 1f);

	private readonly FloatStored _auxiliaryUIColorG = new FloatStored("AuxiliaryUIColorG", 1f);

	private readonly FloatStored _auxiliaryUIColorB = new FloatStored("AuxiliaryUIColorB", 1f);

	private readonly BoolStored _auxiliaryFade = new BoolStored("AuxiliaryFade", true);

	private readonly FloatStored _auxiliaryFadeDelay = new FloatStored("AuxiliaryFadeDelay", 1.5f);

	private readonly FloatStored _aimTargetDist = new FloatStored("AimTargetDist", -0.75f);

	private readonly IntStored _crossHairImage = new IntStored("CrossHairImage", 2);

	private readonly IntStored _crossHairOutline = new IntStored("CrossHairOutline", 1);

	private readonly IntStored _polarityBarLocation = new IntStored("PolarityBarLocation", 1);

	private readonly IntStored _firingModeBarLocation = new IntStored("FiringModeBarLocation", 2);

	private readonly FloatStored _uiCrossHairScale = new FloatStored("UICrossHairScale", 0.5f);

	private readonly FloatStored _uiAuxiliaryScale = new FloatStored("UIAuxiliaryScale", 0.5f);

	private readonly BoolStored _raygunScriptMode = new BoolStored("RaygunScriptMode", false);

	private readonly FloatStored _projectileEffectMultiplier = new FloatStored("ProjectileEffectMultiplier", 1f);

	private readonly IntStored _projectileEffectMode = new IntStored("ProjectileEffectMode", 0);

	private readonly FloatStored _projectileSpurtDuration = new FloatStored("ProjectileSpurtDuration", 5f);

	private readonly FloatStored _playerProjectileSpeed = new FloatStored("PlayerProjectileSpeed", 2f);

	private readonly FloatStored _projectileChargeRate = new FloatStored("ProjectileChargeRate", 1f);

	private readonly IntStored _playerProjectileLifetime = new IntStored("PlayerProjectileLifetime", 6);

	private readonly BoolStored _playerProjectileImpactParticles = new BoolStored("PlayerProjectileImpactParticles", true);

	private readonly FloatStored _playerProjectileImpactParticlesSizeMult = new FloatStored("PlayerProjectileImpactParticlesSizeMult", 1f);

	private readonly BoolStored _playerProjectileGtsMask = new BoolStored("PlayerProjectileGtsMask", true);

	private readonly BoolStored _playerProjectileMicroMask = new BoolStored("PlayerProjectileMicroMask", true);

	private readonly BoolStored _playerProjectileObjectMask = new BoolStored("PlayerProjectileObjectMask", true);

	private readonly FloatStored _laserEffectMultiplier = new FloatStored("LaserEffectMultiplier", 1f);

	private readonly FloatStored _laserWidth = new FloatStored("LaserWidth", 2f);

	private readonly BoolStored _laserImpactParticles = new BoolStored("LaserImpactParticles", true);

	private readonly FloatStored _laserImpactParticlesSizeMult = new FloatStored("LaserImpactParticlesSizeMult", 1f);

	private readonly BoolStored _laserGtsMask = new BoolStored("LaserGtsMask", true);

	private readonly BoolStored _laserMicroMask = new BoolStored("LaserMicroMask", true);

	private readonly BoolStored _laserObjectMask = new BoolStored("LaserObjectMask", true);

	private readonly FloatStored _sonicEffectMultiplier = new FloatStored("SonicEffectMultiplier", 1f);

	private readonly FloatStored _sonicWidth = new FloatStored("SonicWidth", 2f);

	private readonly BoolStored _sonicTagging = new BoolStored("SonicTagging", false);

	private readonly BoolStored _sonicGtsMask = new BoolStored("SonicGtsMask", true);

	private readonly BoolStored _sonicMicroMask = new BoolStored("SonicMicroMask", true);

	private readonly BoolStored _sonicObjectMask = new BoolStored("SonicObjectMask", true);

	private readonly BoolStored _aiProjectileGtsMask = new BoolStored("AIProjectileGtsMask", true);

	private readonly BoolStored _aiProjectileMicroMask = new BoolStored("AIProjectileMicroMask", true);

	private readonly BoolStored _aiProjectileObjectMask = new BoolStored("AIProjectileObjectMask", true);

	private readonly BoolStored _aiProjectilePlayerMask = new BoolStored("AIProjectilePlayerMask", true);

	private readonly BoolStored _aiAccurateShooting = new BoolStored("AIAccurateShooting", true);

	private readonly IntStored _aiInaccuracyFactor = new IntStored("AIInaccuracyFactor", 5);

	private readonly BoolStored _aiPredictiveAiming = new BoolStored("AIPredictiveAiming", false);

	private readonly BoolStored _aiRandomIntervals = new BoolStored("AIRandomIntervals", false);

	private readonly BoolStored _aiBurstFire = new BoolStored("AIBurstFire", false);

	private readonly FloatStored _aiProjectileSpeed = new FloatStored("AIProjectileSpeed", 2f);

	private readonly IntStored _aiProjectileLifetime = new IntStored("AIProjectileLifetime", 6);

	private readonly BoolStored _aiProjectileImpactParticles = new BoolStored("AIProjectileImpactParticles", true);

	private readonly FloatStored _aiProjectileImpactParticlesSizeMult = new FloatStored("AIProjectileImpactParticlesSizeMult", 1f);

	private readonly FloatStored _aiRaygunColorR = new FloatStored("AIRaygunColorR", 1f);

	private readonly FloatStored _aiRaygunColorG = new FloatStored("AIRaygunColorG", 0f);

	private readonly FloatStored _aiRaygunColorB = new FloatStored("AIRaygunColorB", 0f);

	private readonly FloatStored _aiSmgColorR = new FloatStored("AiSmgColorR", 1f);

	private readonly FloatStored _aiSmgColorG = new FloatStored("AiSmgColorG", 1f);

	private readonly FloatStored _aiSmgColorB = new FloatStored("AiSmgColorB", 0f);

	private readonly FloatStored _clampWeight = new FloatStored("ClampWeight", 0.5f);

	private readonly FloatStored _clampWeightHead = new FloatStored("ClampWeightHead", 0.6f);

	private readonly FloatStored _clampWeightEyes = new FloatStored("ClampWeightEyes", 0.7f);

	private readonly IntStored _clampSmoothing = new IntStored("ClampSmoothing", 2);

	private readonly FloatStored _bloodSize = new FloatStored("BloodSize", 0.25f);

	private readonly BoolStored _cameraRtsMode = new BoolStored("CameraRtsMode", true);

	private readonly IntStored _renderMode = new IntStored("RenderMode", 1);

	private readonly BoolStored _hdr = new BoolStored("HighDynamicRange", true);

	private static GlobalPreferences Instance
	{
		get
		{
			return _instance ?? (_instance = new GlobalPreferences());
		}
	}

	public static StringStored PathModel
	{
		get
		{
			return Instance._model;
		}
	}

	public static StringStored PathSound
	{
		get
		{
			return Instance._sound;
		}
	}

	public static StringStored PathScript
	{
		get
		{
			return Instance._script;
		}
	}

	public static StringStored PathScene
	{
		get
		{
			return Instance._scene;
		}
	}

	public static StringStored PathScreenshot
	{
		get
		{
			return Instance._screenshot;
		}
	}

	public static StringStored PathData
	{
		get
		{
			return Instance._data;
		}
	}

	public static IntStored TextureQuality
	{
		get
		{
			return Instance._textureQuality;
		}
	}

	public static IntStored PixelLightCount
	{
		get
		{
			return Instance._pixelLightCount;
		}
	}

	public static IntStored AnisotropicTexture
	{
		get
		{
			return Instance._anisotropic;
		}
	}

	public static IntStored Msaa
	{
		get
		{
			return Instance._msaa;
		}
	}

	public static IntStored Fps
	{
		get
		{
			return Instance._fps;
		}
	}

	public static IntStored Vsync
	{
		get
		{
			return Instance._vsync;
		}
	}

	public static FloatStored AmbientOcclusionValue
	{
		get
		{
			return Instance._ambientOcclusionValue;
		}
	}

	public static BoolStored PostProcessing
	{
		get
		{
			return Instance._enablePostProcess;
		}
	}

	public static BoolStored AmbientOcclusion
	{
		get
		{
			return Instance._ambientOcclusion;
		}
	}

	public static BoolStored AmbientOcclusionVolumetric
	{
		get
		{
			return Instance._ambientOcclusionVolumetric;
		}
	}

	public static BoolStored SoftParticles
	{
		get
		{
			return Instance._softParticles;
		}
	}

	public static BoolStored Shadows
	{
		get
		{
			return Instance._shadows;
		}
	}

	public static IntStored ShadowCascade
	{
		get
		{
			return Instance._shadowCascade;
		}
	}

	public static IntStored ShadowResolution
	{
		get
		{
			return Instance._shadowResolution;
		}
	}

	public static BoolStored SoftShadows
	{
		get
		{
			return Instance._softShadows;
		}
	}

	public static BoolStored FpsCount
	{
		get
		{
			return Instance._fpsCount;
		}
	}

	public static BoolStored DepthOfField
	{
		get
		{
			return Instance._depthOfField;
		}
	}

	public static BoolStored FastApproximateAntiAliasing
	{
		get
		{
			return Instance._fastApproximateAntiAliasing;
		}
	}

	public static BoolStored Fog
	{
		get
		{
			return Instance._fog;
		}
	}

	public static BoolStored FogSkyBox
	{
		get
		{
			return Instance._fogSkyBox;
		}
	}

	public static BoolStored FogExponential
	{
		get
		{
			return Instance._fogExponential;
		}
	}

	public static FloatStored FogPercent
	{
		get
		{
			return Instance._fogDistance;
		}
	}

	public static FloatStored Fov
	{
		get
		{
			return Instance._fov;
		}
	}

	public static BoolStored ChromaticAberration
	{
		get
		{
			return Instance._chromaticAberration;
		}
	}

	public static FloatStored ChromaticAberrationValue
	{
		get
		{
			return Instance._chromaticAberrationValue;
		}
	}

	public static BoolStored ColorGrading
	{
		get
		{
			return Instance._colorGrading;
		}
	}

	public static BoolStored HDR
	{
		get
		{
			return Instance._hdr;
		}
	}

	public static BoolStored ColorHDR
	{
		get
		{
			return Instance._colorHDR;
		}
	}

	public static BoolStored ColorAces
	{
		get
		{
			return Instance._colorAces;
		}
	}

	public static FloatStored ColorBrightness
	{
		get
		{
			return Instance._colorBrightness;
		}
	}

	public static FloatStored ColorContrast
	{
		get
		{
			return Instance._colorContrast;
		}
	}

	public static FloatStored ColorSaturation
	{
		get
		{
			return Instance._colorSaturation;
		}
	}

	public static BoolStored Bloom
	{
		get
		{
			return Instance._bloom;
		}
	}

	public static FloatStored BloomValue
	{
		get
		{
			return Instance._bloomValue;
		}
	}

	public static FloatStored ViewDistance
	{
		get
		{
			return Instance._viewDistance;
		}
	}

	public static FloatStored ShadowDistance
	{
		get
		{
			return Instance._shadowDistance;
		}
	}

	public static BoolStored Vignette
	{
		get
		{
			return Instance._vignette;
		}
	}

	public static FloatStored VignetteValue
	{
		get
		{
			return Instance._vignetteValue;
		}
	}

	public static BoolStored CameraShakeEnabled
	{
		get
		{
			return Instance._cameraShakeEnabled;
		}
	}

	public static FloatStored CameraShakeMultiplier
	{
		get
		{
			return Instance._cameraShakeMultiplier;
		}
	}

	public static BoolStored BackgroundMaxFps
	{
		get
		{
			return Instance._backgroundMaxFps;
		}
	}

	public static BoolStored MotionBlur
	{
		get
		{
			return Instance._motionBlur;
		}
	}

	public static IntStored ReflectionResolution
	{
		get
		{
			return Instance._reflectionResolution;
		}
	}

	public static IntStored ReflectionMode
	{
		get
		{
			return Instance._reflectionMode;
		}
	}

	public static BoolStored SmokeEnabled
	{
		get
		{
			return Instance._smokeEnabled;
		}
	}

	public static FloatStored UIScale
	{
		get
		{
			return Instance._uiScale;
		}
	}

	public static IntStored OffScreenUpdate
	{
		get
		{
			return Instance._offScreenUpdate;
		}
	}

	public static BoolStored SlOpenOnEditor
	{
		get
		{
			return Instance._slOpenOnEditor;
		}
	}

	public static BoolStored SlSelObjOnEntryClick
	{
		get
		{
			return Instance._slSelObjOnEntryClick;
		}
	}

	public static BoolStored BackgroundPause
	{
		get
		{
			return Instance._backgroundPause;
		}
	}

	public static BoolStored LookAtPlayer
	{
		get
		{
			return Instance._lookAtPlayer;
		}
	}

	public static BoolStored SlowdownWithSize
	{
		get
		{
			return Instance._slowdownWithSize;
		}
	}

	public static BoolStored MicrosAffectedByBulletTime
	{
		get
		{
			return Instance._microsAffectedByBulletTime;
		}
	}

	public static BoolStored IgnorePlayer
	{
		get
		{
			return Instance._ignorePlayer;
		}
	}

	public static BoolStored BloodEnabled
	{
		get
		{
			return Instance._bloodEnabled;
		}
	}

	public static BoolStored CrushPlayerEnabled
	{
		get
		{
			return Instance._crushPlayerEnabled;
		}
	}

	public static BoolStored CrushNpcEnabled
	{
		get
		{
			return Instance._crushNpcEnabled;
		}
	}

	public static BoolStored PlayerCrushingEnabled
	{
		get
		{
			return Instance._playerCrushingEnabled;
		}
	}

	public static BoolStored NpcMicroCrushingEnabled
	{
		get
		{
			return Instance._npcMicroCrushingEnabled;
		}
	}

	public static BoolStored NpcGtsCrushingEnabled
	{
		get
		{
			return Instance._npcGtsCrushingEnabled;
		}
	}

	public static BoolStored CrushStick
	{
		get
		{
			return Instance._crushStick;
		}
	}

	public static FloatStored CrushStickChance
	{
		get
		{
			return Instance._crushStickChance;
		}
	}

	public static FloatStored CrushStickDuration
	{
		get
		{
			return Instance._crushStickDuration;
		}
	}

	public static FloatStored MicroDurability
	{
		get
		{
			return Instance._microDurability;
		}
	}

	public static FloatStored MicroSpawnSize
	{
		get
		{
			return Instance._microSpawnSize;
		}
	}

	public static FloatStored CrushSurviveChance
	{
		get
		{
			return Instance._crushSurviveChance;
		}
	}

	public static FloatStored CrushFlatness
	{
		get
		{
			return Instance._crushFlatness;
		}
	}

	public static FloatStored StompSpeed
	{
		get
		{
			return Instance._stompSpeed;
		}
	}

	public static BoolStored RagDollEnabled
	{
		get
		{
			return Instance._ragDollEnabled;
		}
	}

	public static IntStored LogLevel
	{
		get
		{
			return Instance._logLevel;
		}
	}

	public static FloatStored MacroColliderUpdateDistance
	{
		get
		{
			return Instance._macroColliderUpdateDistance;
		}
	}

	public static IntStored MeshCheckLimit
	{
		get
		{
			return Instance._meshCheckLimit;
		}
	}

	public static IntStored MeshUpdateLimit
	{
		get
		{
			return Instance._meshUpdateLimit;
		}
	}

	public static BoolStored UseAdvancedCollision
	{
		get
		{
			return Instance._useAdvancedCollision;
		}
	}

	public static FloatStored MicroRepositionSpeed
	{
		get
		{
			return Instance._microRepositionSpeed;
		}
	}

	public static BoolStored BloodGrows
	{
		get
		{
			return Instance._bloodGrows;
		}
	}

	public static BoolStored BloodShrinks
	{
		get
		{
			return Instance._bloodShrinks;
		}
	}

	public static FloatStored BodyDuration
	{
		get
		{
			return Instance._bodyDuration;
		}
	}

	public static FloatStored BloodDuration
	{
		get
		{
			return Instance._bloodDuration;
		}
	}

	public static FloatStored MouseSensibility
	{
		get
		{
			return Instance._mouseSensibility;
		}
	}

	public static BoolStored CameraEdgeScroll
	{
		get
		{
			return Instance._cameraEdgeScroll;
		}
	}

	public static BoolStored GtsAI
	{
		get
		{
			return Instance._gtsAI;
		}
	}

	public static BoolStored MicroAI
	{
		get
		{
			return Instance._microAI;
		}
	}

	public static BoolStored ScriptAuxLogging
	{
		get
		{
			return Instance._scriptAuxLogging;
		}
	}

	public static BoolStored HairPhysics
	{
		get
		{
			return Instance._hairPhysics;
		}
	}

	public static BoolStored BreastPhysics
	{
		get
		{
			return Instance._breastPhysics;
		}
	}

	public static BoolStored JigglePhysics
	{
		get
		{
			return Instance._jigglePhysics;
		}
	}

	public static BoolStored BlinkEnabled
	{
		get
		{
			return Instance._blinkEnabled;
		}
	}

	public static BoolStored UseGrounder
	{
		get
		{
			return Instance._useGrounder;
		}
	}

	public static BoolStored SecondaryDestruction
	{
		get
		{
			return Instance._secondaryDestruction;
		}
	}

	public static BoolStored KeepDebris
	{
		get
		{
			return Instance._keepDebris;
		}
	}

	public static BoolStored LowEndCities
	{
		get
		{
			return Instance._lowEndCities;
		}
	}

	public static IntStored CityPopulation
	{
		get
		{
			return Instance._cityPopulation;
		}
	}

	public static FloatStored MicroSpeed
	{
		get
		{
			return Instance._microSpeed;
		}
	}

	public static FloatStored BulletTimeFactor
	{
		get
		{
			return Instance._bulletTimeFactor;
		}
	}

	public static IntStored LazyBatch
	{
		get
		{
			return Instance._lazyBatch;
		}
	}

	public static BoolStored DebrisCanCrush
	{
		get
		{
			return Instance._debrisCanCrush;
		}
	}

	public static BoolStored ImperialMeasurements
	{
		get
		{
			return Instance._imperialMeasurements;
		}
	}

	public static BoolStored GtsBuildingDestruction
	{
		get
		{
			return Instance._gtsBuildingDestruction;
		}
	}

	public static BoolStored MicroPlayerBuildingDestruction
	{
		get
		{
			return Instance._microPlayerBuildingDestruction;
		}
	}

	public static BoolStored GtsPlayerBuildingDestruction
	{
		get
		{
			return Instance._gtsPlayerBuildingDestruction;
		}
	}

	public static FloatStored GrowColorR
	{
		get
		{
			return Instance._growColorR;
		}
	}

	public static FloatStored GrowColorG
	{
		get
		{
			return Instance._growColorG;
		}
	}

	public static FloatStored GrowColorB
	{
		get
		{
			return Instance._growColorB;
		}
	}

	public static FloatStored ShrinkColorR
	{
		get
		{
			return Instance._shrinkColorR;
		}
	}

	public static FloatStored ShrinkColorG
	{
		get
		{
			return Instance._shrinkColorG;
		}
	}

	public static FloatStored ShrinkColorB
	{
		get
		{
			return Instance._shrinkColorB;
		}
	}

	public static FloatStored CrossHairColorR
	{
		get
		{
			return Instance._crossHairColorR;
		}
	}

	public static FloatStored CrossHairColorG
	{
		get
		{
			return Instance._crossHairColorG;
		}
	}

	public static FloatStored CrossHairColorB
	{
		get
		{
			return Instance._crossHairColorB;
		}
	}

	public static FloatStored AuxiliaryUIColorR
	{
		get
		{
			return Instance._auxiliaryUIColorR;
		}
	}

	public static FloatStored AuxiliaryUIColorG
	{
		get
		{
			return Instance._auxiliaryUIColorG;
		}
	}

	public static FloatStored AuxiliaryUIColorB
	{
		get
		{
			return Instance._auxiliaryUIColorB;
		}
	}

	public static BoolStored AuxiliaryFade
	{
		get
		{
			return Instance._auxiliaryFade;
		}
	}

	public static FloatStored AuxiliaryFadeDelay
	{
		get
		{
			return Instance._auxiliaryFadeDelay;
		}
	}

	public static FloatStored AimTargetDist
	{
		get
		{
			return Instance._aimTargetDist;
		}
	}

	public static IntStored CrossHairImage
	{
		get
		{
			return Instance._crossHairImage;
		}
	}

	public static IntStored CrossHairOutline
	{
		get
		{
			return Instance._crossHairOutline;
		}
	}

	public static IntStored PolarityBarLocation
	{
		get
		{
			return Instance._polarityBarLocation;
		}
	}

	public static IntStored FiringModeBarLocation
	{
		get
		{
			return Instance._firingModeBarLocation;
		}
	}

	public static FloatStored UiCrossHairScale
	{
		get
		{
			return Instance._uiCrossHairScale;
		}
	}

	public static FloatStored UIAuxiliaryScale
	{
		get
		{
			return Instance._uiAuxiliaryScale;
		}
	}

	public static BoolStored RaygunScriptMode
	{
		get
		{
			return Instance._raygunScriptMode;
		}
	}

	public static FloatStored ProjectileEffectMultiplier
	{
		get
		{
			return Instance._projectileEffectMultiplier;
		}
	}

	public static IntStored ProjectileEffectMode
	{
		get
		{
			return Instance._projectileEffectMode;
		}
	}

	public static FloatStored ProjectileSpurtDuration
	{
		get
		{
			return Instance._projectileSpurtDuration;
		}
	}

	public static FloatStored PlayerProjectileSpeed
	{
		get
		{
			return Instance._playerProjectileSpeed;
		}
	}

	public static FloatStored ProjectileChargeRate
	{
		get
		{
			return Instance._projectileChargeRate;
		}
	}

	public static IntStored PlayerProjectileLifetime
	{
		get
		{
			return Instance._playerProjectileLifetime;
		}
	}

	public static BoolStored PlayerProjectileImpactParticles
	{
		get
		{
			return Instance._playerProjectileImpactParticles;
		}
	}

	public static FloatStored PlayerProjectileImpactParticlesSizeMult
	{
		get
		{
			return Instance._playerProjectileImpactParticlesSizeMult;
		}
	}

	public static BoolStored PlayerProjectileGtsMask
	{
		get
		{
			return Instance._playerProjectileGtsMask;
		}
	}

	public static BoolStored PlayerProjectileMicroMask
	{
		get
		{
			return Instance._playerProjectileMicroMask;
		}
	}

	public static BoolStored PlayerProjectileObjectMask
	{
		get
		{
			return Instance._playerProjectileObjectMask;
		}
	}

	public static FloatStored LaserEffectMultiplier
	{
		get
		{
			return Instance._laserEffectMultiplier;
		}
	}

	public static FloatStored LaserWidth
	{
		get
		{
			return Instance._laserWidth;
		}
	}

	public static BoolStored LaserImpactParticles
	{
		get
		{
			return Instance._laserImpactParticles;
		}
	}

	public static FloatStored LaserImpactParticlesSizeMult
	{
		get
		{
			return Instance._laserImpactParticlesSizeMult;
		}
	}

	public static BoolStored LaserGtsMask
	{
		get
		{
			return Instance._laserGtsMask;
		}
	}

	public static BoolStored LaserMicroMask
	{
		get
		{
			return Instance._laserMicroMask;
		}
	}

	public static BoolStored LaserObjectMask
	{
		get
		{
			return Instance._laserObjectMask;
		}
	}

	public static FloatStored SonicEffectMultiplier
	{
		get
		{
			return Instance._sonicEffectMultiplier;
		}
	}

	public static FloatStored SonicWidth
	{
		get
		{
			return Instance._sonicWidth;
		}
	}

	public static BoolStored SonicTagging
	{
		get
		{
			return Instance._sonicTagging;
		}
	}

	public static BoolStored SonicGtsMask
	{
		get
		{
			return Instance._sonicGtsMask;
		}
	}

	public static BoolStored SonicMicroMask
	{
		get
		{
			return Instance._sonicMicroMask;
		}
	}

	public static BoolStored SonicObjectMask
	{
		get
		{
			return Instance._sonicObjectMask;
		}
	}

	public static BoolStored AIProjectileGtsMask
	{
		get
		{
			return Instance._aiProjectileGtsMask;
		}
	}

	public static BoolStored AIProjectileMicroMask
	{
		get
		{
			return Instance._aiProjectileMicroMask;
		}
	}

	public static BoolStored AIProjectileObjectMask
	{
		get
		{
			return Instance._aiProjectileObjectMask;
		}
	}

	public static BoolStored AIProjectilePlayerMask
	{
		get
		{
			return Instance._aiProjectilePlayerMask;
		}
	}

	public static BoolStored AIAccurateShooting
	{
		get
		{
			return Instance._aiAccurateShooting;
		}
	}

	public static IntStored AIInaccuracyFactor
	{
		get
		{
			return Instance._aiInaccuracyFactor;
		}
	}

	public static BoolStored AIPredictiveAiming
	{
		get
		{
			return Instance._aiPredictiveAiming;
		}
	}

	public static BoolStored AIRandomIntervals
	{
		get
		{
			return Instance._aiRandomIntervals;
		}
	}

	public static BoolStored AIBurstFire
	{
		get
		{
			return Instance._aiBurstFire;
		}
	}

	public static FloatStored AIProjectileSpeed
	{
		get
		{
			return Instance._aiProjectileSpeed;
		}
	}

	public static IntStored AIProjectileLifetime
	{
		get
		{
			return Instance._aiProjectileLifetime;
		}
	}

	public static BoolStored AIProjectileImpactParticles
	{
		get
		{
			return Instance._aiProjectileImpactParticles;
		}
	}

	public static FloatStored AIProjectileImpactParticlesSizeMult
	{
		get
		{
			return Instance._aiProjectileImpactParticlesSizeMult;
		}
	}

	public static FloatStored AIRaygunColorR
	{
		get
		{
			return Instance._aiRaygunColorR;
		}
	}

	public static FloatStored AIRaygunColorG
	{
		get
		{
			return Instance._aiRaygunColorG;
		}
	}

	public static FloatStored AIRaygunColorB
	{
		get
		{
			return Instance._aiRaygunColorB;
		}
	}

	public static FloatStored AiSmgColorR
	{
		get
		{
			return Instance._aiSmgColorR;
		}
	}

	public static FloatStored AiSmgColorG
	{
		get
		{
			return Instance._aiSmgColorG;
		}
	}

	public static FloatStored AiSmgColorB
	{
		get
		{
			return Instance._aiSmgColorB;
		}
	}

	public static FloatStored ClampWeight
	{
		get
		{
			return Instance._clampWeight;
		}
	}

	public static FloatStored ClampWeightHead
	{
		get
		{
			return Instance._clampWeightHead;
		}
	}

	public static FloatStored ClampWeightEyes
	{
		get
		{
			return Instance._clampWeightEyes;
		}
	}

	public static IntStored ClampSmoothing
	{
		get
		{
			return Instance._clampSmoothing;
		}
	}

	public static FloatStored VolumeMaster
	{
		get
		{
			return Instance._volMaster;
		}
	}

	public static FloatStored VolumeBackground
	{
		get
		{
			return Instance._volBackground;
		}
	}

	public static FloatStored VolumeDestruction
	{
		get
		{
			return Instance._volDestruction;
		}
	}

	public static FloatStored VolumeEffect
	{
		get
		{
			return Instance._volEffect;
		}
	}

	public static FloatStored VolumeMacro
	{
		get
		{
			return Instance._volMacro;
		}
	}

	public static FloatStored VolumeMicro
	{
		get
		{
			return Instance._volMicro;
		}
	}

	public static FloatStored VolumeMusic
	{
		get
		{
			return Instance._volMusic;
		}
	}

	public static FloatStored VolumeVoice
	{
		get
		{
			return Instance._volVoice;
		}
	}

	public static FloatStored VolumeWindA
	{
		get
		{
			return Instance._volWindA;
		}
	}

	public static FloatStored VolumeWindG
	{
		get
		{
			return Instance._volWindG;
		}
	}

	public static FloatStored VolumeRaygun
	{
		get
		{
			return Instance._volRaygun;
		}
	}

	public static FloatStored VolumeAIGuns
	{
		get
		{
			return Instance._volAIGuns;
		}
	}

	public static BoolStored BackgroundAudio
	{
		get
		{
			return Instance._backgroundAudio;
		}
	}

	public static FloatStored BloodSize
	{
		get
		{
			return Instance._bloodSize;
		}
	}

	public static BoolStored CameraRtsMode
	{
		get
		{
			return Instance._cameraRtsMode;
		}
	}

	public static IntStored RenderMode
	{
		get
		{
			return Instance._renderMode;
		}
	}

	public static void Refresh()
	{
		_instance = new GlobalPreferences();
	}
}
