using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiPopup : MonoBehaviour
{
	private GameObject _popupButton;

	public Text title;

	public Transform messageArea;

	public Transform buttonGrid;

	private void Awake()
	{
		_popupButton = Resources.Load<GameObject>("UI/Button/PopupButton");
		StateManager.Mouse.AddNoSync();
		base.gameObject.SetActive(false);
	}

	public void AddButtonsYesNo(UnityAction yesAction, UnityAction noAction = null)
	{
		AddButton("Yes", yesAction);
		AddButton("No", noAction);
	}

	public void AddButtonsOkCancel(UnityAction okAction, UnityAction cancelAction = null)
	{
		AddButton("OK", okAction);
		AddButton("Cancel", cancelAction);
	}

	public void AddButton(string text, UnityAction unityAction = null)
	{
		Button component = Object.Instantiate(_popupButton, buttonGrid, false).GetComponent<Button>();
		component.GetComponentInChildren<Text>().text = text;
		component.onClick.AddListener(unityAction ?? new UnityAction(Close));
	}

	public virtual void Popup()
	{
		if (string.IsNullOrEmpty(title.text))
		{
			title.text = "Message";
		}
		if (buttonGrid.GetComponentsInChildren<Button>().Length == 0)
		{
			AddButton("OK", Close);
		}
		base.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		StateManager.Mouse.Add();
	}

	private void OnDisable()
	{
		StateManager.Mouse.Remove();
	}

	public virtual void Close()
	{
		Object.Destroy(base.gameObject);
	}

	public static void CloseAll(UiPopup[] uiPopup)
	{
		for (int i = 0; i < uiPopup.Length; i++)
		{
			uiPopup[i].Close();
		}
	}

	public static UiPopup Create()
	{
		Transform parent = GameObject.FindWithTag("MainCanvas").transform;
		return Object.Instantiate(Resources.Load<GameObject>("UI/UiPopup"), parent, false).GetComponent<UiPopup>();
	}
}
