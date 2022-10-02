using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
	private enum SceneState
	{
		Reset = 0,
		PreLoad = 1,
		Load = 2,
		Unload = 3,
		PostLoad = 4,
		Ready = 5,
		Run = 6,
		Count = 7
	}

	private delegate void UpdateDelegate();

	public Text loadingText;

	private static MainController _mainController;

	private string _currentSceneName;

	private string _nextSceneName;

	private AsyncOperation _resourceUnloadTask;

	private AsyncOperation _sceneLoadTask;

	private SceneState _sceneState;

	private UpdateDelegate[] _updateDelegates;

	public void Reset()
	{
		if (_mainController != null)
		{
			_mainController.ResetScene();
		}
	}

	private void ResetScene()
	{
		_sceneState = SceneState.Reset;
	}

	public static void SwitchScene(string nextSceneName)
	{
		if (_mainController != null && _mainController._currentSceneName != nextSceneName)
		{
			_mainController._nextSceneName = nextSceneName;
		}
	}

	protected void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_mainController = this;
		_updateDelegates = new UpdateDelegate[7];
		_updateDelegates[0] = UpdateSceneReset;
		_updateDelegates[1] = UpdateScenePreload;
		_updateDelegates[2] = UpdateSceneLoad;
		_updateDelegates[3] = UpdateSceneUnload;
		_updateDelegates[4] = UpdateScenePostLoad;
		_updateDelegates[5] = UpdateSceneReady;
		_updateDelegates[6] = UpdateSceneRun;
		_nextSceneName = "MainMenu_SBE";
		_sceneState = SceneState.Reset;
		if (Sbox.GetProcessFlag("-reset-all-settings"))
		{
			PlayerPrefs.DeleteAll();
		}
		if (Sbox.GetProcessFlag("-no-toast"))
		{
			UiMessageBoxQueue uiMessageBoxQueue = UiMessageBoxQueue.Create("You have started the game with Toast notifications disabled.\n\nThis means you wont be notified when the game runs into an error. Please do not report unexpected behaviour in this session", "Toasts Disabled");
			uiMessageBoxQueue.ToLogWarning();
			UiMessageBoxQueue.Enqueue(uiMessageBoxQueue);
		}
		StateManager instance = StateManager.instance;
	}

	protected void Update()
	{
		if (_updateDelegates[(int)_sceneState] != null)
		{
			_updateDelegates[(int)_sceneState]();
		}
		if ((bool)loadingText)
		{
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1f));
		}
	}

	protected void OnDestroy()
	{
		if (_updateDelegates != null)
		{
			for (int i = 0; i < 7; i++)
			{
				_updateDelegates[i] = null;
			}
			_updateDelegates = null;
		}
		_mainController = null;
	}

	private void UpdateSceneReset()
	{
		GC.Collect();
		_sceneState = SceneState.PreLoad;
	}

	private void UpdateScenePreload()
	{
		_sceneLoadTask = SceneManager.LoadSceneAsync(_nextSceneName);
		_sceneState = SceneState.Load;
	}

	private void UpdateSceneLoad()
	{
		if (_sceneLoadTask.isDone)
		{
			_sceneState = SceneState.Unload;
		}
	}

	private void UpdateSceneUnload()
	{
		if (_resourceUnloadTask == null)
		{
			_resourceUnloadTask = Resources.UnloadUnusedAssets();
		}
		else if (_resourceUnloadTask.isDone)
		{
			_resourceUnloadTask = null;
			_sceneState = SceneState.PostLoad;
		}
	}

	private void UpdateScenePostLoad()
	{
		_currentSceneName = _nextSceneName;
		_sceneState = SceneState.Ready;
	}

	private void UpdateSceneReady()
	{
		GC.Collect();
		_sceneState = SceneState.Run;
	}

	private void UpdateSceneRun()
	{
		if (_currentSceneName != _nextSceneName)
		{
			_sceneState = SceneState.Reset;
		}
	}
}
