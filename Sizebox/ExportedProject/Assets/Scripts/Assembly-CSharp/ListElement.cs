using UnityEngine;
using UnityEngine.UI;

public class ListElement : MonoBehaviour
{
	[SerializeField]
	private Text textGui;

	[SerializeField]
	private Button button;

	public string Text
	{
		get
		{
			return textGui.text;
		}
		set
		{
			textGui.text = value;
		}
	}

	public Button.ButtonClickedEvent onClick
	{
		get
		{
			return button.onClick;
		}
	}
}
