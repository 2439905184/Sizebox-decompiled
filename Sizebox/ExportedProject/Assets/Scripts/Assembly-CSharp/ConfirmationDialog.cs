using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
	private GameController gameController;

	[SerializeField]
	private Button yesButton;

	[SerializeField]
	private Button noButton;

	[SerializeField]
	private Text messageText;

	private UnityAction yesCallback;

	private UnityAction noCallback;

	private void Awake()
	{
		gameController = GameController.Instance;
		yesButton.onClick.AddListener(OnYes);
		noButton.onClick.AddListener(OnNo);
	}

	public void Open(UnityAction yesCallback, UnityAction noCallback, string message)
	{
		this.yesCallback = yesCallback;
		this.noCallback = noCallback;
		messageText.text = message;
		base.gameObject.SetActive(true);
	}

	private void OnYes()
	{
		if (yesCallback != null)
		{
			yesCallback();
		}
		base.gameObject.SetActive(false);
	}

	private void OnNo()
	{
		if (noCallback != null)
		{
			noCallback();
		}
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		StateManager.Keyboard.userIsTyping = true;
	}

	private void OnDisable()
	{
		StateManager.Keyboard.userIsTyping = false;
	}
}
