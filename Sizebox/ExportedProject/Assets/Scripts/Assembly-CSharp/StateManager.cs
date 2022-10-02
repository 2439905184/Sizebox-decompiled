using Pause;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateManager : MonoBehaviour
{
	public static class Mouse
	{
		public static bool IsQuitting;

		private static uint _refVis;

		private static uint RefVis
		{
			get
			{
				return _refVis;
			}
			set
			{
				if (value == _refVis + 1)
				{
					_refVis = value;
				}
				else if (value == _refVis - 1 && _refVis != 0)
				{
					_refVis = value;
				}
				Sync();
			}
		}

		public static void Add()
		{
			RefVis++;
		}

		public static void AddNoSync()
		{
			_refVis++;
		}

		public static void Remove()
		{
			RefVis--;
		}

		public static void Sync()
		{
			if (!IsQuitting)
			{
				Cursor.visible = _refVis != 0;
				Cursor.lockState = ((_refVis == 0) ? CursorLockMode.Locked : CursorLockMode.None);
			}
		}
	}

	public static class Keyboard
	{
		private static uint _typewriters;

		public static bool Ctrl
		{
			get
			{
				return UnityEngine.InputSystem.Keyboard.current.ctrlKey.isPressed;
			}
		}

		public static bool Shift
		{
			get
			{
				return UnityEngine.InputSystem.Keyboard.current.shiftKey.isPressed;
			}
		}

		public static bool userIsTyping
		{
			get
			{
				return _typewriters != 0;
			}
			set
			{
				if (value)
				{
					_typewriters++;
				}
				else if (_typewriters != 0)
				{
					_typewriters--;
				}
				Sync();
			}
		}

		public static void Sync()
		{
			GameController instance = GameController.Instance;
			if ((bool)instance)
			{
				instance.SetTypingState(_typewriters != 0);
			}
		}
	}

	public static StateManager instance
	{
		get
		{
			if (!cachedInstance)
			{
				cachedInstance = new GameObject("State Manager").AddComponent<StateManager>();
				Object.DontDestroyOnLoad(cachedInstance);
			}
			return cachedInstance;
		}
	}

	public static StateManager cachedInstance { get; private set; }

	private void Awake()
	{
		Debug.Log(LogVersion());
		GameSettingsView.OnLogLevelChanged(GlobalPreferences.LogLevel.value);
	}

	public static string LogVersion()
	{
		return "This is Sizebox v" + Version.GetText();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!GlobalPreferences.BackgroundAudio.value)
		{
			if (hasFocus)
			{
				SoundManager.AudioMixer.SetFloat("Master", GlobalPreferences.VolumeMaster.value);
			}
			else
			{
				SoundManager.AudioMixer.SetFloat("Master", -80f);
			}
		}
		if (!GlobalPreferences.BackgroundMaxFps.value && QualitySettings.vSyncCount == 0)
		{
			Application.targetFrameRate = (hasFocus ? GlobalPreferences.Fps.value : 10);
		}
	}

	private void OnApplicationQuit()
	{
		Mouse.IsQuitting = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
