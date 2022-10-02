using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiMessageBoxQueue
{
	private struct ButtonData
	{
		public string Text;

		public UnityAction Action;
	}

	private readonly List<ButtonData> _buttons = new List<ButtonData>();

	private string _message;

	private string _title;

	private static readonly Stack<UiMessageBoxQueue> Queue = new Stack<UiMessageBoxQueue>();

	public static UiMessageBoxQueue Create(string message = null, string title = null)
	{
		return new UiMessageBoxQueue
		{
			_message = message,
			_title = title
		};
	}

	private void Popup()
	{
		UiMessageBox uiMessageBox = UiMessageBox.Create(_message, _title);
		foreach (ButtonData button in _buttons)
		{
			uiMessageBox.AddButton(button.Text, button.Action);
		}
		uiMessageBox.Popup();
	}

	private string GetLogMessage()
	{
		return (string.IsNullOrEmpty(_title) ? "Message: " : (_title + ": ")) + _message;
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

	public void AddButton(string text, UnityAction unityAction = null)
	{
		ButtonData buttonData = default(ButtonData);
		buttonData.Text = text;
		buttonData.Action = unityAction;
		ButtonData item = buttonData;
		_buttons.Add(item);
	}

	public static void Enqueue(UiMessageBoxQueue uiMessageBox)
	{
		Queue.Push(uiMessageBox);
	}

	public static void Enqueue(string message = null, string title = null)
	{
		Enqueue(Create(message, title));
	}

	public static void Flush()
	{
		while (Queue.Count > 0)
		{
			Queue.Pop().Popup();
		}
	}
}
