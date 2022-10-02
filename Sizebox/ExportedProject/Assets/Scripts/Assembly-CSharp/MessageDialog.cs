using UnityEngine;
using UnityEngine.UI;

public class MessageDialog : MonoBehaviour
{
	[SerializeField]
	private Button okButton;

	[SerializeField]
	private Text messageText;

	private void Awake()
	{
		okButton.onClick.AddListener(OnOk);
	}

	public void Open(string message)
	{
		messageText.text = message;
		base.gameObject.SetActive(true);
	}

	private void OnOk()
	{
		base.gameObject.SetActive(false);
	}
}
