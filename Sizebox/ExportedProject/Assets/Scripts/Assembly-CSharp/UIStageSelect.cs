using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIStageSelect : MonoBehaviour
{
	[Header("Required Reference")]
	[SerializeField]
	private MenuController menu;

	[SerializeField]
	private Canvas menuUI;

	[SerializeField]
	private Image stageImage;

	[SerializeField]
	private Text stageText;

	[Header("Buttons")]
	[SerializeField]
	private Button nextStageButton;

	[SerializeField]
	private Button prevStageButton;

	[SerializeField]
	private Button okButton;

	[SerializeField]
	private Button backButton;

	[Header("Scripts")]
	[SerializeField]
	private UILoadingStage uiLoadingStage;

	private SceneLoader _sceneLoader;

	private SceneLoader.SceneInfo _actualStage;

	private void Awake()
	{
		nextStageButton.onClick.AddListener(OnNextScene);
		prevStageButton.onClick.AddListener(OnPreviousScene);
		okButton.onClick.AddListener(OnOk);
		backButton.onClick.AddListener(OnBack);
	}

	private void OnEnable()
	{
		InputManager.inputs.Interface.Back.performed += OnBackKey;
		InputManager.inputs.Interface.Move.performed += OnArrowKeys;
		InputManager.inputs.Interface.Select.performed += OnSelectKey;
	}

	private void OnSelectKey(InputAction.CallbackContext obj)
	{
		OnOk();
	}

	private void OnBackKey(InputAction.CallbackContext obj)
	{
		OnBack();
	}

	private void OnArrowKeys(InputAction.CallbackContext obj)
	{
		Vector2 vector = obj.ReadValue<Vector2>();
		if ((double)vector.x > 0.1)
		{
			OnNextScene();
		}
		else if ((double)vector.x < -0.1)
		{
			OnPreviousScene();
		}
	}

	private void OnDestroy()
	{
		InputManager.inputs.Interface.Back.performed -= OnBackKey;
		InputManager.inputs.Interface.Move.performed -= OnArrowKeys;
		InputManager.inputs.Interface.Select.performed -= OnSelectKey;
	}

	private void Start()
	{
		_sceneLoader = Object.FindObjectOfType<SceneLoader>();
		ShowStage();
	}

	private void OnNextScene()
	{
		menu.SwitchNextScene();
		ShowStage();
	}

	private void OnPreviousScene()
	{
		menu.SwitchPreviousScene();
		ShowStage();
	}

	private void ShowStage()
	{
		_actualStage = menu.GetActiveSceneData();
		stageImage.sprite = _actualStage.Thumbnail;
		stageText.text = _actualStage.Scene;
	}

	private void OnBack()
	{
		base.gameObject.SetActive(false);
		menuUI.gameObject.SetActive(true);
		Button componentInChildren = menuUI.GetComponentInChildren<Button>();
		if ((object)componentInChildren != null)
		{
			componentInChildren.Select();
		}
	}

	private void OnOk()
	{
		AsyncOperation asyncOperation = _sceneLoader.LoadScene(_actualStage);
		if (asyncOperation == null)
		{
			UiMessageBox.Create(string.Concat("Unable to load level '", _actualStage, "'."), "Error Loading Level").Popup();
			return;
		}
		uiLoadingStage.enabled = true;
		uiLoadingStage.AsyncOperation = asyncOperation;
		uiLoadingStage.IsLoading = true;
		backButton.gameObject.SetActive(false);
		okButton.gameObject.SetActive(false);
		nextStageButton.gameObject.SetActive(false);
		prevStageButton.gameObject.SetActive(false);
	}
}
