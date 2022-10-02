using System;
using System.Collections;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class GuiManager : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField]
		private Canvas mainCanvasPrefab;

		[Header("Required References")]
		[SerializeField]
		private InterfaceControl interfaceControl;

		[SerializeField]
		private EditPlacement editPlacement;

		[SerializeField]
		private TransformGizmo gizmo;

		private GameController _gameController;

		private FPSDisplay _fpsDisplay;

		public static bool FPSDisplayEnabled = true;

		private EditPlacement _placement;

		private Canvas _mainCanvas;

		private Toast _screenShotToast;

		private Coroutine _screenShotRoutine;

		private CanvasScaler _scaler;

		public static GuiManager Instance { get; private set; }

		public GameObject MainCanvasGameObject { get; private set; }

		public InGameConsole Console { get; private set; }

		public EditView EditMode { get; private set; }

		public PauseView Pause { get; private set; }

		public InterfaceControl InterfaceControl
		{
			get
			{
				return interfaceControl;
			}
		}

		public EditPlacement EditPlacement
		{
			get
			{
				return editPlacement;
			}
		}

		public TransformGizmo TransformGizmo
		{
			get
			{
				return gizmo;
			}
		}

		public Vector2 GetCanvasScaleVector()
		{
			Vector2 referenceResolution = _scaler.referenceResolution;
			Transform transform = _mainCanvas.transform;
			return new Vector2(((RectTransform)transform).sizeDelta.x / referenceResolution.x, ((RectTransform)transform).sizeDelta.y / referenceResolution.y);
		}

		public Vector2 GetCanvasReferenceResolution()
		{
			return Vector2.Scale(_scaler.referenceResolution, GetCanvasScaleVector());
		}

		public void ShowFPS(bool show)
		{
			FPSDisplayEnabled = show;
			GlobalPreferences.FpsCount.value = show;
			if (!show)
			{
				_fpsDisplay.enabled = false;
			}
			if (show && !_gameController.paused)
			{
				_fpsDisplay.enabled = true;
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			_gameController = GameController.Instance;
			GameController instance = GameController.Instance;
			instance.onModeChange = (UnityAction<GameMode>)Delegate.Combine(instance.onModeChange, new UnityAction<GameMode>(OnModeChange));
			FPSDisplayEnabled = GlobalPreferences.FpsCount.value;
			MainCanvasGameObject = UnityEngine.Object.Instantiate(mainCanvasPrefab.gameObject);
			_mainCanvas = MainCanvasGameObject.GetComponent<Canvas>();
			_scaler = MainCanvasGameObject.GetComponent<CanvasScaler>();
			CameraSettings.SetUiScale(GlobalPreferences.UIScale.value);
			_fpsDisplay = MainCanvasGameObject.AddComponent<FPSDisplay>();
			if (!FPSDisplayEnabled)
			{
				_fpsDisplay.enabled = false;
			}
			Console = MainCanvasGameObject.GetComponentInChildren<InGameConsole>();
			Console.visible = false;
			EditMode = MainCanvasGameObject.GetComponentInChildren<EditView>();
			EditMode.gameObject.SetActive(true);
			Pause = MainCanvasGameObject.GetComponentInChildren<PauseView>();
			Pause.SetGameController(_gameController);
			Pause.main = this;
			Pause.gameObject.SetActive(false);
			ToastInternal.UpdateToastCanvas(_mainCanvas.transform.Find("Toast Holder"));
			_screenShotToast = new Toast("_Screenshot");
			base.gameObject.AddComponent<MainInput>();
			GameController.Instance.SetMode(GameMode.Edit);
			StateManager.Mouse.Sync();
			StateManager.Keyboard.Sync();
		}

		private void OnModeChange(GameMode mode)
		{
			if ((bool)EditMode)
			{
				switch (mode)
				{
				case GameMode.Edit:
					EditMode.gameObject.SetActive(true);
					break;
				case GameMode.Play:
				case GameMode.FreeCam:
					EditMode.gameObject.SetActive(false);
					break;
				}
			}
		}

		public void ToggleEditMode()
		{
			GameMode gameMode = ((!EditMode.gameObject.activeSelf) ? GameMode.Edit : GameMode.Play);
			if (gameMode == GameMode.Play && !LocalClient.Instance.Player.Entity)
			{
				gameMode = GameMode.FreeCam;
			}
			_gameController.SetMode(gameMode);
		}

		public void TogglePauseMenu()
		{
			if (Pause.gameObject.activeSelf)
			{
				if (!Pause.CloseIfOpen())
				{
					Pause.OnResumeClick();
				}
			}
			else
			{
				OpenPauseMenu();
			}
		}

		public void OpenPauseMenu()
		{
			if (!Pause.gameObject.activeSelf)
			{
				_gameController.SetPausedState(true);
				SoundManager.Instance.SetPaused(true);
				Pause.gameObject.SetActive(true);
				_fpsDisplay.enabled = false;
			}
		}

		public void ClosePauseMenu()
		{
			_gameController.SetPausedState(false);
			SoundManager.Instance.SetPaused(false);
			Pause.gameObject.SetActive(false);
			_fpsDisplay.enabled = FPSDisplayEnabled;
		}

		public void TakeScreenshot()
		{
			if (_screenShotRoutine == null)
			{
				_screenShotRoutine = StartCoroutine(ScreenshotRoutine());
			}
		}

		private IEnumerator ScreenshotRoutine()
		{
			SetGizmo(false);
			MainCanvasGameObject.SetActive(false);
			string fullPath;
			string filename;
			IOManager.Instance.GetScreenshotFileName(out fullPath, out filename);
			ScreenCapture.CaptureScreenshot(fullPath);
			Debug.Log("Taken Screenshot: " + filename);
			yield return null;
			MainCanvasGameObject.SetActive(true);
			SetGizmo(true);
			_screenShotToast.Print(filename);
			yield return null;
			_screenShotRoutine = null;
		}

		private void SetGizmo(bool value)
		{
			if (!_placement)
			{
				_placement = GetComponent<EditPlacement>();
			}
			if ((bool)_placement)
			{
				value = value && _gameController.mode == GameMode.Edit;
				_placement.enabled = value;
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (GlobalPreferences.BackgroundPause.value && !hasFocus)
			{
				OpenPauseMenu();
			}
		}
	}
}
