using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class SaveDialog : MonoBehaviour
	{
		private GameController gameController;

		private UnityAction<string> callback;

		[SerializeField]
		private Button saveButton;

		[SerializeField]
		private Button cancelButton;

		[SerializeField]
		private InputField nameInput;

		private void Awake()
		{
			gameController = GameController.Instance;
			saveButton.onClick.AddListener(OnSave);
			cancelButton.onClick.AddListener(OnCancel);
		}

		public void Open(UnityAction<string> saveCallback, string name = null)
		{
			base.gameObject.SetActive(true);
			callback = saveCallback;
			if (name != null)
			{
				nameInput.text = name;
			}
		}

		private void OnSave()
		{
			if (callback != null && !(nameInput.text == "") && nameInput.text != null)
			{
				callback(nameInput.text);
				base.gameObject.SetActive(false);
			}
		}

		private void OnCancel()
		{
			base.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			StateManager.Keyboard.userIsTyping = true;
		}

		private void OnDisable()
		{
			callback = null;
			nameInput.text = "";
			StateManager.Keyboard.userIsTyping = false;
		}
	}
}
