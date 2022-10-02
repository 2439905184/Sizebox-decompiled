using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SaveMenuView : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public string filename;

		public SaveMenuView _003C_003E4__this;

		internal void _003CReloadSavedFiles_003Eb__0()
		{
			_003C_003E4__this.OnClickElement(filename);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass12_0
	{
		public SaveMenuView _003C_003E4__this;

		public string filename;

		internal void _003CSaveFile_003Eb__0()
		{
			_003C_003E4__this.WriteFile(filename);
			_003C_003E4__this._overwritePrompt.Close();
			_003C_003E4__this.Close();
		}
	}

	private InputField _inputField;

	private Button _saveButton;

	private bool _initialized;

	private Button _buttonPrefab;

	private Transform _listCanvas;

	private UiMessageBox _overwritePrompt;

	private void Initialize()
	{
		_inputField = GetComponentInChildren<InputField>();
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		_saveButton = componentsInChildren[0];
		_saveButton.onClick.AddListener(SaveFile);
		componentsInChildren[1].onClick.AddListener(Close);
		_buttonPrefab = Resources.Load<Button>("UI/Button/SceneItem");
		_listCanvas = GetComponentInChildren<GridLayoutGroup>().transform;
		_initialized = true;
	}

	private void ReloadSavedFiles()
	{
		string[] listSavedFiles = IOManager.Instance.GetListSavedFiles();
		Button[] componentsInChildren = _listCanvas.GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
		string[] array = listSavedFiles;
		foreach (string filename in array)
		{
			_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
			_003C_003Ec__DisplayClass7_._003C_003E4__this = this;
			Button button = Object.Instantiate(_buttonPrefab, _listCanvas.transform, false);
			Text componentInChildren = button.GetComponentInChildren<Text>();
			_003C_003Ec__DisplayClass7_.filename = filename;
			_003C_003Ec__DisplayClass7_.filename = _003C_003Ec__DisplayClass7_.filename.Replace(".json", "");
			componentInChildren.text = _003C_003Ec__DisplayClass7_.filename;
			button.onClick.AddListener(_003C_003Ec__DisplayClass7_._003CReloadSavedFiles_003Eb__0);
		}
	}

	private void Update()
	{
		if (Keyboard.current.enterKey.wasReleasedThisFrame)
		{
			SaveFile();
		}
	}

	private void OnEnable()
	{
		if (!_initialized)
		{
			Initialize();
		}
		ReloadSavedFiles();
		_inputField.text = "";
		_inputField.ActivateInputField();
	}

	private void OnClickElement(string filename)
	{
		_inputField.text = filename;
	}

	private void Close()
	{
		base.gameObject.SetActive(false);
	}

	private void SaveFile()
	{
		_003C_003Ec__DisplayClass12_0 _003C_003Ec__DisplayClass12_ = new _003C_003Ec__DisplayClass12_0();
		_003C_003Ec__DisplayClass12_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass12_.filename = _inputField.text;
		if (string.IsNullOrWhiteSpace(_003C_003Ec__DisplayClass12_.filename))
		{
			if (!_overwritePrompt)
			{
				_overwritePrompt = UiMessageBox.Create("Please enter a file name to save as", "No filename");
				_overwritePrompt.Popup();
			}
			return;
		}
		if (!_003C_003Ec__DisplayClass12_.filename.EndsWith(".json"))
		{
			_003C_003Ec__DisplayClass12_.filename += ".json";
		}
		if (IOManager.Instance.SaveExists(_003C_003Ec__DisplayClass12_.filename))
		{
			if (!_overwritePrompt)
			{
				_overwritePrompt = UiMessageBox.Create("A file with this name already exists. Are you sure you want to overwrite it?", "Overwrite");
				_overwritePrompt.AddButtonsYesNo(_003C_003Ec__DisplayClass12_._003CSaveFile_003Eb__0);
				_overwritePrompt.Popup();
			}
		}
		else
		{
			WriteFile(_003C_003Ec__DisplayClass12_.filename);
			Close();
		}
	}

	private void WriteFile(string filename)
	{
		Debug.Log("Saving scene as " + filename);
		SavedScenesManager.Instance.SaveScene(filename);
	}
}
