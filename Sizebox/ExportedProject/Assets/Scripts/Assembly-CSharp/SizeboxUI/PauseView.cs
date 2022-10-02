using System;
using System.Runtime.CompilerServices;
using Pause;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class PauseView : MonoBehaviour
	{
		private const string MainMenuString = "MainMenu_SBE";

		public GuiManager main;

		private GameController _gameController;

		private GameObject _saveMenu;

		private GameObject _settingsPrefab;

		private Button _buttonPrefab;

		private SettingsView _activeSetting;

		private GameObject _activeGameObject;

		private Button _resume;

		private GridLayoutGroup _buttonLayout;

		public bool CloseIfOpen()
		{
			EventSystem.current.SetSelectedGameObject(null);
			bool result = false;
			if (_saveMenu.activeSelf)
			{
				_saveMenu.SetActive(false);
				result = true;
			}
			if ((bool)_activeSetting)
			{
				_activeSetting.ClosePanel();
				result = true;
			}
			else if ((bool)_activeGameObject)
			{
				UnityEngine.Object.Destroy(_activeGameObject);
				result = true;
			}
			return result;
		}

		private void Awake()
		{
			_buttonPrefab = Resources.Load<Button>("UI/Pause/PauseButton");
			_settingsPrefab = Resources.Load<GameObject>("UI/Pause/SettingsMenu");
			GameObject original = Resources.Load<GameObject>("UI/Pause/SaveMenu");
			_saveMenu = UnityEngine.Object.Instantiate(original, base.transform, false);
			_saveMenu.AddComponent<SaveMenuView>();
			_saveMenu.SetActive(false);
			_buttonLayout = base.gameObject.GetComponentInChildren<GridLayoutGroup>();
			_resume = AddButton("Resume");
			_resume.onClick.AddListener(OnResumeClick);
			AddButton("Restart").onClick.AddListener(OnResetClick);
			AddButton("Save").onClick.AddListener(OnSaveClick);
			AddButton("Settings").onClick.AddListener(OnSettingsClick);
			AddButton("Video").onClick.AddListener(OnVideoClick);
			AddButton("Lighting").onClick.AddListener(OnLightingClick);
			AddButton("Audio").onClick.AddListener(OnAudioClick);
			AddButton("Controls").onClick.AddListener(OnControlsClick);
			AddButton("Main Menu").onClick.AddListener(OnMainMenuClick);
			AddButton("Quit Game").onClick.AddListener(OnQuitClick);
		}

		private void OnEnable()
		{
			Debug.Log("Game paused");
			StateManager.Mouse.Add();
			_resume.Select();
		}

		private void OnDisable()
		{
			Debug.Log("Game resumed");
			StateManager.Mouse.Remove();
		}

		private Button AddButton(string label)
		{
			Button button = UnityEngine.Object.Instantiate(_buttonPrefab, _buttonLayout.transform, false);
			button.gameObject.GetComponent<Text>().text = label;
			return button;
		}

		public void SetGameController(GameController gc)
		{
			_gameController = gc;
		}

		public void OnResumeClick()
		{
			main.ClosePauseMenu();
		}

		private void OnResetClick()
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("All unsaved progress will be lost\nAre you sure you want to restart?", "Restart");
			uiMessageBox.AddButtonsYesNo(SavedScenesManager.Instance.RestartScene);
			uiMessageBox.Popup();
		}

		private void OnSaveClick()
		{
			_saveMenu.SetActive(true);
		}

		private void OnSettingsClick()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/BehaviorMgr/BehaviorManagementMenu"), base.transform, false);
			BehaviorManagerView behaviorManagerView = gameObject.AddComponent<BehaviorManagerView>();
			gameObject.SetActive(false);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false);
			gameObject2.AddComponent<RaygunSettingsView>();
			gameObject2.SetActive(false);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false);
			gameObject3.AddComponent<AIGunSettingsView>();
			gameObject3.SetActive(false);
			GameObject gameObject4 = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false);
			gameObject4.AddComponent<CloudSettingsView>();
			gameObject4.SetActive(false);
			GameObject obj = (_activeGameObject = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false));
			GameSettingsView gameSettingsView = (GameSettingsView)(_activeSetting = obj.AddComponent<GameSettingsView>());
			gameSettingsView.SizeGunSettings = gameObject2;
			gameSettingsView.AiGunSettings = gameObject3;
			gameSettingsView.BehaviourManager = gameObject;
			gameSettingsView.AdvancedClouds = gameObject4;
			behaviorManagerView.Hide = (UnityAction)Delegate.Combine(behaviorManagerView.Hide, new UnityAction(gameSettingsView.OnBehaviorManagerHide));
			obj.SetActive(true);
		}

		private void OnAudioClick()
		{
			GameObject gameObject = (_activeGameObject = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false));
			_activeSetting = gameObject.AddComponent<AudioSettingsView>();
			gameObject.SetActive(true);
		}

		private void OnVideoClick()
		{
			GameObject gameObject = (_activeGameObject = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false));
			_activeSetting = gameObject.AddComponent<VideoSettingsView>();
			_activeSetting.main = main;
			gameObject.SetActive(true);
		}

		private void OnControlsClick()
		{
			GameObject gameObject = (_activeGameObject = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false));
			_activeSetting = gameObject.AddComponent<ControlSettingsView>();
			_activeSetting.main = main;
			gameObject.SetActive(true);
		}

		public void OnCloudsClick()
		{
			GameObject original = Resources.Load<GameObject>("UI/Pause/CloudMenu");
			GameObject gameObject = (_activeGameObject = UnityEngine.Object.Instantiate(original, base.transform, false));
			_activeSetting = gameObject.AddComponent<CloudSettingsView>();
			gameObject.SetActive(true);
		}

		private void OnLightingClick()
		{
			GameObject gameObject = (_activeGameObject = UnityEngine.Object.Instantiate(_settingsPrefab, base.transform, false));
			_activeSetting = gameObject.AddComponent<LightingView>();
			gameObject.SetActive(true);
		}

		private void OnMainMenuClick()
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("All unsaved progress will be lost\nAre you sure you want to return to the main menu?", "Main Menu");
			uiMessageBox.AddButtonsYesNo(_003COnMainMenuClick_003Eb__25_0);
			uiMessageBox.Popup();
		}

		private void OnQuitClick()
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("All unsaved progress will be lost\nAre you sure you want to quit to desktop?", "Quit");
			uiMessageBox.AddButtonsYesNo(Application.Quit);
			uiMessageBox.Popup();
		}

		[CompilerGenerated]
		private void _003COnMainMenuClick_003Eb__25_0()
		{
			_gameController.SetPausedState(false);
			SceneManager.LoadScene("MainMenu_SBE");
		}
	}
}
