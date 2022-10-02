using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	[Serializable]
	public class StageData
	{
		public string scene;

		public Sprite thumbnail;
	}

	private int _activeScene;

	private List<SceneLoader.SceneInfo> _listStages;

	private SceneLoader _sceneLoader;

	[Header("Required References")]
	public Canvas mainCanvas;

	public Canvas loadUI;

	public Canvas menuUI;

	public Canvas sceneUI;

	public Canvas settingsUI;

	[Header("Buttons")]
	[SerializeField]
	private Button startButton;

	[SerializeField]
	private Button loadButton;

	[SerializeField]
	private Button settingsButton;

	[SerializeField]
	private Button exitButton;

	private void Awake()
	{
		startButton.onClick.AddListener(OnStartClick);
		loadButton.onClick.AddListener(OnLoadClick);
		settingsButton.onClick.AddListener(OnSettingsClick);
		exitButton.onClick.AddListener(OnExitClick);
		StateManager.Mouse.Add();
		_sceneLoader = UnityEngine.Object.FindObjectOfType<SceneLoader>();
		if (!_sceneLoader)
		{
			Debug.LogError("Can't find the Scene Loader on the scene.");
		}
		_listStages = _sceneLoader.GetSceneList();
		_activeScene = 0;
		CameraSettings.SetUiScale(GlobalPreferences.UIScale.value);
		StateManager instance = StateManager.instance;
	}

	private void Start()
	{
		InputManager.inputs.Interface.Enable();
		ToastInternal.UpdateToastCanvas();
		UiMessageBoxQueue.Flush();
		SoundManager.InitializeMixers();
		SoundManager.LoadMixerLevels();
	}

	public void OnDestroy()
	{
		InputManager.inputs.Interface.Disable();
		StateManager.Mouse.Remove();
	}

	private void OnStartClick()
	{
		sceneUI.gameObject.SetActive(true);
		menuUI.gameObject.SetActive(false);
	}

	private void OnLoadClick()
	{
		loadUI.gameObject.SetActive(true);
		menuUI.gameObject.SetActive(false);
	}

	private void OnSettingsClick()
	{
		settingsUI.gameObject.SetActive(true);
		menuUI.gameObject.SetActive(false);
	}

	private void OnExitClick()
	{
		Application.Quit();
	}

	public void SwitchNextScene()
	{
		_activeScene = (_listStages.Count + (_activeScene + 1)) % _listStages.Count;
	}

	public void SwitchPreviousScene()
	{
		_activeScene = (_listStages.Count + (_activeScene - 1)) % _listStages.Count;
	}

	public SceneLoader.SceneInfo GetActiveSceneData()
	{
		return _listStages[_activeScene];
	}
}
