using UnityEngine;
using UnityEngine.UI;

public class UiMessageBox : UiPopup
{
	public Text message;

	protected void SetMessage(string text)
	{
		message.text = text;
	}

	public override void Popup()
	{
		if (string.IsNullOrEmpty(message.text))
		{
			Close();
		}
		base.Popup();
	}

	private static UiMessageBox MessageBoxExists(string message, string title)
	{
		UiMessageBox[] array = Object.FindObjectsOfType<UiMessageBox>();
		foreach (UiMessageBox uiMessageBox in array)
		{
			if (uiMessageBox.message.text == message && uiMessageBox.title.text == title)
			{
				return uiMessageBox;
			}
		}
		return null;
	}

	private string GetLogMessage()
	{
		return (string.IsNullOrEmpty(title.text) ? "Message: " : (title.text + ": ")) + message.text;
	}

	public void ToLog()
	{
		Debug.Log(GetLogMessage());
	}

	public void ToLogWarning()
	{
		Debug.LogWarning(GetLogMessage());
	}

	public void ToLogError()
	{
		Debug.LogError(GetLogMessage());
	}

	public static UiMessageBox Create(string message = null, string title = null)
	{
		UiMessageBox uiMessageBox = MessageBoxExists(message, title);
		if (uiMessageBox != null)
		{
			return uiMessageBox;
		}
		Transform parent = GameObject.FindWithTag("MainCanvas").transform;
		uiMessageBox = Object.Instantiate(Resources.Load<GameObject>("UI/UiMessageBox"), parent, false).GetComponent<UiMessageBox>();
		uiMessageBox.SetMessage(message);
		if (!string.IsNullOrEmpty(title))
		{
			uiMessageBox.title.text = title;
		}
		return uiMessageBox;
	}
}
