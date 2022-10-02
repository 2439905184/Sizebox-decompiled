using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiEntryBox : UiMessageBox
{
	public InputField inputField;

	public override void Popup()
	{
		if (string.IsNullOrEmpty(message.text))
		{
			Close();
		}
		base.Popup();
		EventSystem current;
		(current = EventSystem.current).SetSelectedGameObject(inputField.gameObject);
		inputField.OnPointerClick(new PointerEventData(current));
	}

	public void SetInput(string text)
	{
		inputField.text = text;
	}

	public void SetInputPlaceholder(string text)
	{
		inputField.placeholder.GetComponent<Text>().text = text;
	}

	public static UiEntryBox Create(string message = null, string title = null, string placeholder = null)
	{
		Transform parent = GameObject.FindWithTag("MainCanvas").transform;
		UiEntryBox component = Object.Instantiate(Resources.Load<GameObject>("UI/UiEntryBox"), parent, false).GetComponent<UiEntryBox>();
		component.SetMessage(message);
		if (!string.IsNullOrEmpty(title))
		{
			component.title.text = title;
		}
		if (!string.IsNullOrEmpty(placeholder))
		{
			component.SetInputPlaceholder(placeholder);
		}
		return component;
	}
}
