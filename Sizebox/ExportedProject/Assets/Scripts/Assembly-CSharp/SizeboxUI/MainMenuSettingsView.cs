using Pause;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class MainMenuSettingsView : MonoBehaviour
	{
		private SettingsView _activeSetting;

		[SerializeField]
		private Canvas menuUi;

		[SerializeField]
		private Button pauseButtonPrefab;

		[SerializeField]
		private GameObject settingsMenuPrefab;

		private GridLayoutGroup _buttonLayout;

		private void Update()
		{
			if ((Keyboard.current.escapeKey.wasPressedThisFrame || Input.GetButtonDown(ButtonInput.EscapeAlt)) && !CloseIfOpen())
			{
				CloseSettings();
			}
		}

		private bool CloseIfOpen()
		{
			bool result = false;
			if ((bool)_activeSetting)
			{
				_activeSetting.ClosePanel();
				result = true;
			}
			return result;
		}

		private void Start()
		{
			_buttonLayout = base.gameObject.GetComponentInChildren<GridLayoutGroup>();
			AddButton("Settings").onClick.AddListener(OnSettingsClick);
			AddButton("Controls").onClick.AddListener(OnControlsClick);
			AddButton("Content").onClick.AddListener(OnContentClick);
			AddButton("Video").onClick.AddListener(OnVideoClick);
			AddButton("Audio").onClick.AddListener(OnAudioClick);
			Button button = AddButton("Back");
			button.onClick.AddListener(CloseSettings);
			button.Select();
		}

		private Button AddButton(string label)
		{
			Button button = Object.Instantiate(pauseButtonPrefab, _buttonLayout.transform);
			button.gameObject.GetComponent<Text>().text = label;
			return button;
		}

		private void OnSettingsClick()
		{
			CloseIfOpen();
			GameObject gameObject = Object.Instantiate(settingsMenuPrefab, base.transform);
			RaygunSettingsView raygunSettingsView = gameObject.AddComponent<RaygunSettingsView>();
			gameObject.SetActive(false);
			GameObject gameObject2 = Object.Instantiate(settingsMenuPrefab, base.transform);
			AIGunSettingsView aIGunSettingsView = gameObject2.AddComponent<AIGunSettingsView>();
			gameObject2.SetActive(false);
			SettingsView settingsView = (_activeSetting = Object.Instantiate(settingsMenuPrefab, base.transform).AddComponent<GameSettingsView>());
			GameObject gameObject3;
			(gameObject3 = settingsView.gameObject).GetComponent<GameSettingsView>().SizeGunSettings = gameObject;
			settingsView.GetComponent<GameSettingsView>().AiGunSettings = gameObject2;
			gameObject3.SetActive(true);
			raygunSettingsView._gameSettings = gameObject3;
			aIGunSettingsView._gameSettings = gameObject3;
		}

		private void OnAudioClick()
		{
			CloseIfOpen();
			(_activeSetting = Object.Instantiate(settingsMenuPrefab, base.transform).AddComponent<AudioSettingsView>()).gameObject.SetActive(true);
		}

		private void OnContentClick()
		{
			CloseIfOpen();
			(_activeSetting = Object.Instantiate(settingsMenuPrefab, base.transform).AddComponent<ContentSettingsView>()).gameObject.SetActive(true);
		}

		private void OnVideoClick()
		{
			CloseIfOpen();
			(_activeSetting = Object.Instantiate(settingsMenuPrefab, base.transform).AddComponent<VideoSettingsView>()).gameObject.SetActive(true);
		}

		private void OnControlsClick()
		{
			CloseIfOpen();
			(_activeSetting = Object.Instantiate(settingsMenuPrefab, base.transform).AddComponent<ControlSettingsView>()).gameObject.SetActive(true);
		}

		private void CloseSettings()
		{
			CloseIfOpen();
			menuUi.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}
	}
}
