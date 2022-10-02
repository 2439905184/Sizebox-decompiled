using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.DynamicClouds;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class GameController : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField]
	private LocalClient playerPrefab;

	public static GameController Instance;

	public Pathfinder pathfinder;

	public static CloudController cloudController;

	public static List<GameObject> giantessesOnScene = new List<GameObject>();

	public UnityAction<GameMode> onModeChange;

	private EditPlacement editPlacement;

	private GameMode _mode;

	public static float microSpeed = 1f;

	public static float bulletTimeFactor = 0.15f;

	public static bool bulletTimeActive;

	public static bool microsAffectedByBulletTime = true;

	private static float _wantedSpeed = 1f;

	private static float _macroSpeed = 1f;

	private static float _referenceScale = 1f;

	public static bool IsMacroMap = false;

	public static bool playerInputEnabled = false;

	public SharedKnowledge sharedKnowledge;

	private LuaLazyUpdate luaLazyUpdate;

	private LuaLazyFixedUpdate luaLazyFixedUpdate;

	private LuaUpdate luaUpdate;

	private LuaFixedUpdate luaFixedUpdate;

	private LuaManager luaManager;

	private int _lastFrame;

	private Stack<GameMode> _previousModes = new Stack<GameMode>();

	public UnityAction<bool> onPaused;

	public GameMode mode
	{
		get
		{
			return _mode;
		}
		private set
		{
			_mode = value;
			StateManager.Keyboard.Sync();
		}
	}

	public bool allowCustomKeys
	{
		get
		{
			if (!paused)
			{
				if (_mode != GameMode.FreeCam)
				{
					return _mode == GameMode.Play;
				}
				return true;
			}
			return false;
		}
	}

	public static bool FreezeGts
	{
		get
		{
			return _macroSpeed < float.Epsilon;
		}
		set
		{
			_macroSpeed = (value ? 0f : _wantedSpeed);
		}
	}

	public static float macroSpeed
	{
		get
		{
			if (!FreezeGts)
			{
				return _macroSpeed;
			}
			return 0f;
		}
		set
		{
			if (value > 4f)
			{
				_macroSpeed = (_wantedSpeed = 4f);
			}
			else if (value < 0.02f)
			{
				_macroSpeed = (_wantedSpeed = 0.02f);
			}
			else
			{
				_macroSpeed = (_wantedSpeed = value);
			}
		}
	}

	public static float ReferenceScale
	{
		get
		{
			return _referenceScale;
		}
		set
		{
			_referenceScale = ((value > 0f) ? value : 1f);
		}
	}

	public static bool VrMode
	{
		get
		{
			if (XRSettings.enabled)
			{
				return XRSettings.loadedDeviceName != "None";
			}
			return false;
		}
	}

	public static LocalClient LocalClient { get; private set; }

	public bool paused { get; private set; }

	public bool gameStarted { get; private set; }

	public void SetMode()
	{
		SetMode(mode);
	}

	public void SetMode(GameMode newMode)
	{
		mode = newMode;
		if (mode == GameMode.Play && !LocalClient.Player.Entity)
		{
			mode = GameMode.Edit;
		}
		bool flag;
		bool flag2;
		switch (mode)
		{
		case GameMode.Play:
			flag = true;
			flag2 = false;
			break;
		case GameMode.Edit:
			flag = false;
			flag2 = true;
			break;
		case GameMode.FreeCam:
			flag = false;
			flag2 = false;
			break;
		case GameMode.Pause:
			flag = false;
			flag2 = false;
			break;
		case GameMode.Typing:
			flag = false;
			flag2 = false;
			break;
		default:
			flag = false;
			flag2 = false;
			break;
		}
		playerInputEnabled = flag;
		if ((bool)editPlacement)
		{
			editPlacement.enabled = flag2;
		}
		StateManager.Keyboard.Sync();
		UnityAction<GameMode> unityAction = onModeChange;
		if (unityAction != null)
		{
			unityAction(mode);
		}
	}

	private void Awake()
	{
		Instance = this;
		gameStarted = false;
		AssetManager instance = AssetManager.Instance;
		base.gameObject.AddComponent<InputManager>();
		EventManager.Clear();
		pathfinder = new Pathfinder();
		sharedKnowledge = new SharedKnowledge();
		base.gameObject.AddComponent<SoundManager>();
		base.gameObject.AddComponent<SimpleSunController>();
		SetPausedState(false);
		Layers.Initialize();
		Physics.gravity = new Vector3(0f, -9.8f, 0f);
		SetMicrosAffectedByBulletTime(GlobalPreferences.MicrosAffectedByBulletTime.value);
		ReferenceScale = MapSettingInternal.scale;
		IsMacroMap = MapSettingInternal.enableFog;
	}

	private void OnDestroy()
	{
		DisableGameControls();
	}

	public void SetPausedState(bool pause)
	{
		float timeScale = 1f;
		if (pause)
		{
			_previousModes.Push(mode);
			SetMode(GameMode.Pause);
			timeScale = 0f;
		}
		else if (pause != paused && _previousModes.Count > 0)
		{
			SetMode(_previousModes.Pop());
		}
		paused = pause;
		Time.timeScale = timeScale;
		UnityAction<bool> unityAction = onPaused;
		if (unityAction != null)
		{
			unityAction(pause);
		}
	}

	public void SetTypingState(bool isTyping)
	{
		if ((!isTyping || mode != GameMode.Typing) && (isTyping || mode == GameMode.Typing))
		{
			if (isTyping)
			{
				_previousModes.Push(mode);
				SetMode(GameMode.Typing);
			}
			else
			{
				SetMode(_previousModes.Pop());
			}
		}
	}

	private void EnableGameControls()
	{
		InputManager.Instance.EnableControls(InputManager.inputs.Misc.Get(), GameMode.Play);
		InputManager.Instance.EnableControls(InputManager.inputs.Misc.Get(), GameMode.Edit);
		InputManager.Instance.EnableControls(InputManager.inputs.Misc.Get(), GameMode.FreeCam);
		InputManager.Instance.EnableControls(InputManager.inputs.EditMode.Get(), GameMode.Edit);
		InputManager.Instance.EnableControls(InputManager.inputs.EditMode.Get(), GameMode.FreeCam);
	}

	private void DisableGameControls()
	{
		InputManager.Instance.DisableControls(InputManager.inputs.Misc.Get(), GameMode.Play);
		InputManager.Instance.DisableControls(InputManager.inputs.Misc.Get(), GameMode.Edit);
		InputManager.Instance.DisableControls(InputManager.inputs.EditMode.Get(), GameMode.Edit);
	}

	private void Start()
	{
		editPlacement = GuiManager.Instance.EditPlacement;
		LocalClient = UnityEngine.Object.Instantiate(playerPrefab);
		LocalClient.name = "PLAYER";
		luaUpdate = base.gameObject.AddComponent<LuaUpdate>();
		luaFixedUpdate = base.gameObject.AddComponent<LuaFixedUpdate>();
		luaLazyUpdate = base.gameObject.AddComponent<LuaLazyUpdate>();
		luaLazyFixedUpdate = base.gameObject.AddComponent<LuaLazyFixedUpdate>();
		luaManager = base.gameObject.AddComponent<LuaManager>();
		playerInputEnabled = true;
		gameStarted = true;
		EnableGameControls();
		SetMode(GameMode.Edit);
	}

	private void Update()
	{
		if (IOManager.Instance.isLoadingData)
		{
			IOManager.Instance.isLoadingData = false;
			Debug.Log("Loading the data to the scene");
			SavedScenesManager.Instance.ReBuildScene();
		}
	}

	public bool ReloadScript(LuaBehavior behavior)
	{
		return luaManager.ReloadScript(behavior);
	}

	public static void IncreaseSpeed()
	{
		macroSpeed /= 1.2f;
		ObjectManager.Instance.UpdateGiantessSpeeds();
	}

	public static void DecreaseSpeed()
	{
		macroSpeed *= 1.2f;
		ObjectManager.Instance.UpdateGiantessSpeeds();
	}

	public static void ChangeSpeed(float newSpeed)
	{
		macroSpeed = newSpeed;
		ObjectManager.Instance.UpdateGiantessSpeeds();
	}

	public static void ChangeMicroSpeed(float newSpeed)
	{
		microSpeed = newSpeed;
		ObjectManager.Instance.UpdateMicroSpeeds();
	}

	public static void ChangeBulletTimeSpeed(float newSpeed)
	{
		bulletTimeFactor = newSpeed;
		if (bulletTimeActive)
		{
			ObjectManager.Instance.UpdateGiantessSpeeds();
			if (microsAffectedByBulletTime)
			{
				ObjectManager.Instance.UpdateMicroSpeeds();
			}
		}
	}

	public static void SetSlowDown(bool value)
	{
		GlobalPreferences.SlowdownWithSize.value = value;
		ObjectManager.Instance.UpdateGiantessSpeeds();
	}

	public static void SetMicrosAffectedByBulletTime(bool value)
	{
		GlobalPreferences.MicrosAffectedByBulletTime.value = value;
		microsAffectedByBulletTime = value;
	}

	public static float ConvertHumanReadableToScale(string humanReadable, bool nativeFormat = false)
	{
		if (string.IsNullOrEmpty(humanReadable))
		{
			return float.NegativeInfinity;
		}
		char[] anyOf = "1234567890.,' ".ToCharArray();
		int num = humanReadable.IndexOfAny(anyOf);
		if (num < 0)
		{
			return float.NegativeInfinity;
		}
		int num2 = humanReadable.LastIndexOfAny(anyOf) + 1;
		string text = humanReadable.Substring(num, num2 - num);
		float result;
		if (humanReadable.EndsWith("ft") || text.Contains("'"))
		{
			result = Sbox.ConvertFootInchToMeters(text, nativeFormat);
		}
		else
		{
			if (!float.TryParse(text, NumberStyles.Float, nativeFormat ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture, out result))
			{
				return float.NegativeInfinity;
			}
			string text2 = humanReadable.Substring(num2).ToLowerInvariant();
			if (string.IsNullOrEmpty(text2))
			{
				text2 = (GlobalPreferences.ImperialMeasurements.value ? "ft" : "m");
			}
			if (text2.EndsWith("km"))
			{
				result *= 1000f;
			}
			else if (text2.EndsWith("cm"))
			{
				result *= 0.01f;
			}
			else if (text2.EndsWith("mm"))
			{
				result *= 0.001f;
			}
			else if (text2.EndsWith("um") || text2.EndsWith("μm"))
			{
				result *= 1E-06f;
			}
			else if (text2.EndsWith("mi"))
			{
				result *= 1609.344f;
			}
			else if (text2.EndsWith("ft"))
			{
				result *= 0.3048f;
			}
			else if (text2.EndsWith("in"))
			{
				result *= 0.0254f;
			}
			else if (text2.EndsWith("th"))
			{
				result *= 2.54E-05f;
			}
		}
		return Sbox.MeterToScale(result);
	}

	public static string ConvertScaleToHumanReadable(float scale)
	{
		scale = Sbox.ScaleToMeter(scale);
		string text;
		if (GlobalPreferences.ImperialMeasurements.value)
		{
			scale /= 0.3048f;
			if (scale < 1f)
			{
				scale = (float)((double)scale - Math.Truncate(scale)) / 0.08333f;
				if (scale < 0.1f)
				{
					text = " th";
					scale *= 1000f;
				}
				else
				{
					text = " in";
				}
			}
			else
			{
				if (!(scale > 5280f))
				{
					if (scale > 100f)
					{
						text = " ft";
						return Math.Floor(scale).ToString("0") + text;
					}
					text = "\"";
					return string.Concat(str2: ((int)(((double)scale - Math.Truncate(scale)) / 0.08333)).ToString("0"), str0: Math.Floor(scale).ToString("0"), str1: "'", str3: text);
				}
				scale /= 5280f;
				text = " mi";
			}
		}
		else if (scale >= 1000f)
		{
			text = " km";
			scale /= 1000f;
		}
		else if (scale >= 1f)
		{
			text = " m";
		}
		else if (scale >= 0.01f)
		{
			text = " cm";
			scale *= 100f;
		}
		else
		{
			scale *= 1000f;
			if (scale < 0.1f)
			{
				text = " μm";
				scale *= 1000f;
			}
			else
			{
				text = " mm";
			}
		}
		return scale.ToString("F2", CultureInfo.CurrentCulture) + text;
	}

	private void OnGUI()
	{
		int frameCount = Time.frameCount;
		if (frameCount != _lastFrame)
		{
			switch (Event.current.type)
			{
			case EventType.KeyDown:
				EventManager.SendEvent(new KeyDown());
				_lastFrame = frameCount;
				break;
			case EventType.KeyUp:
				EventManager.SendEvent(new KeyUp());
				_lastFrame = frameCount;
				break;
			case EventType.MouseDown:
				EventManager.SendEvent(new MouseDown());
				_lastFrame = frameCount;
				break;
			case EventType.MouseUp:
				EventManager.SendEvent(new MouseUp());
				_lastFrame = frameCount;
				break;
			case EventType.MouseMove:
			case EventType.MouseDrag:
				break;
			}
		}
	}
}
