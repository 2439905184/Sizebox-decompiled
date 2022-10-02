using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISelectFolder : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public UISelectFolder _003C_003E4__this;

		public string entry;

		internal void _003CAddListEntry_003Eb__0()
		{
			_003C_003E4__this.OnFolderClick(entry);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass23_0
	{
		public UISelectFolder _003C_003E4__this;

		public UiEntryBox uiEntryBox;

		internal void _003COnNewFolder_003Eb__0()
		{
			_003C_003E4__this.OnCreateFolder(uiEntryBox);
		}
	}

	public string path;

	public Transform listCanvas;

	public InputField input;

	public UnityAction<UISelectFolder> selected;

	public List<string> subDirectories;

	public Button selectButton;

	public Button cancelButton;

	public Button folderButton;

	private Button _buttonPrefab;

	private string _userString;

	private string _execString;

	private string _homeDirectory;

	private string _viewFolderPath;

	private List<GameObject> _children;

	private void Awake()
	{
		_buttonPrefab = Resources.Load<Button>("UI/Button/SceneItem");
	}

	private void Start()
	{
		_execString = Application.productName;
		_userString = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Environment.UserName.ToLower());
		_homeDirectory = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		_children = new List<GameObject>();
		folderButton.onClick.AddListener(_003CStart_003Eb__15_0);
		input.onEndEdit.AddListener(OnEntryUpdate);
		selectButton.onClick.AddListener(OnSelectedClick);
		_viewFolderPath = path;
		PopulateList(_viewFolderPath);
		EventSystem current;
		(current = EventSystem.current).SetSelectedGameObject(input.gameObject);
		input.OnPointerClick(new PointerEventData(current));
	}

	private void OnSelectedClick()
	{
		if (string.IsNullOrEmpty(input.text))
		{
			path = input.text;
			selected(this);
		}
		else if (Directory.Exists(input.text))
		{
			path = input.text;
			selected(this);
		}
		else
		{
			UiMessageBox.Create("Folder '" + input.text + "' doesn't exist", "Directory does not exist").Popup();
		}
	}

	private void ClearList()
	{
		Button[] componentsInChildren = listCanvas.GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	private void AddListEntry(string entry)
	{
		_003C_003Ec__DisplayClass18_0 _003C_003Ec__DisplayClass18_ = new _003C_003Ec__DisplayClass18_0();
		_003C_003Ec__DisplayClass18_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass18_.entry = entry;
		Button button = UnityEngine.Object.Instantiate(_buttonPrefab, listCanvas.transform, false);
		button.GetComponentInChildren<Text>().text = _003C_003Ec__DisplayClass18_.entry;
		button.onClick.AddListener(_003C_003Ec__DisplayClass18_._003CAddListEntry_003Eb__0);
	}

	private void OnFolderClick(string entry)
	{
		if (!SpecialSelection(entry))
		{
			if (string.IsNullOrEmpty(entry) || (entry == ".." && Path.GetPathRoot(path) == path))
			{
				PopulateList(string.Empty);
			}
			else
			{
				PopulateList(Path.GetFullPath(Path.Combine(_viewFolderPath, entry)));
			}
		}
	}

	private bool SpecialSelection(string entry, bool anywhere = false)
	{
		if (anywhere || string.IsNullOrEmpty(_viewFolderPath))
		{
			string text = entry.ToLower();
			if (text == _execString.ToLower())
			{
				PopulateList(Path.GetFullPath(IOManager.GetApplicationDirectory()));
				return true;
			}
			if (text == _userString.ToLower())
			{
				PopulateList(Path.GetFullPath(_homeDirectory));
				return true;
			}
		}
		return false;
	}

	private void OnEntryUpdate(string entry)
	{
		if (string.IsNullOrEmpty(entry) && !string.IsNullOrEmpty(_viewFolderPath))
		{
			PopulateList(string.Empty);
		}
		else if (!(entry == _viewFolderPath) && !SpecialSelection(entry, true) && Directory.Exists(entry))
		{
			PopulateList(entry);
		}
	}

	private void OnCreateFolder(UiEntryBox uiEntryBox)
	{
		if (string.IsNullOrEmpty(uiEntryBox.inputField.text))
		{
			UiMessageBox.Create("No folder name was entered", "Error").Popup();
			return;
		}
		string entry = Path.Combine(_viewFolderPath, uiEntryBox.inputField.text);
		if (Directory.Exists(entry))
		{
			UiMessageBox.Create("Folder '" + uiEntryBox.inputField.text + "' already exists", "Error").Popup();
			return;
		}
		Sbox.CreateSubDirectory(_viewFolderPath, uiEntryBox.inputField.text);
		if (Directory.Exists(entry))
		{
			OnFolderClick(entry);
		}
		uiEntryBox.Close();
	}

	private GameObject OnNewFolder()
	{
		_003C_003Ec__DisplayClass23_0 _003C_003Ec__DisplayClass23_ = new _003C_003Ec__DisplayClass23_0();
		_003C_003Ec__DisplayClass23_._003C_003E4__this = this;
		if (string.IsNullOrEmpty(input.text))
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("You must be browsing a directory before you can create a folder.", "Error");
			uiMessageBox.Popup();
			return uiMessageBox.gameObject;
		}
		if (!Directory.Exists(input.text))
		{
			UiMessageBox uiMessageBox2 = UiMessageBox.Create("Current path '" + input.text + "' doesn't exist.", "Error");
			uiMessageBox2.Popup();
			return uiMessageBox2.gameObject;
		}
		_003C_003Ec__DisplayClass23_.uiEntryBox = UiEntryBox.Create("Enter a name for the new folder", "New Folder", "Folder name...");
		_003C_003Ec__DisplayClass23_.uiEntryBox.AddButtonsOkCancel(_003C_003Ec__DisplayClass23_._003COnNewFolder_003Eb__0);
		_003C_003Ec__DisplayClass23_.uiEntryBox.Popup();
		return _003C_003Ec__DisplayClass23_.uiEntryBox.gameObject;
	}

	private void PopulateList(string directoryPath)
	{
		ClearList();
		if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
		{
			AddListEntry(_execString);
			if (!string.IsNullOrEmpty(_homeDirectory))
			{
				AddListEntry(_userString);
			}
			string[] logicalDrives = Environment.GetLogicalDrives();
			foreach (string entry in logicalDrives)
			{
				AddListEntry(entry);
			}
			path = "";
			input.text = "";
			_viewFolderPath = "";
			return;
		}
		try
		{
			AddListEntry("..");
			string[] logicalDrives = Directory.GetDirectories(directoryPath);
			foreach (string text in logicalDrives)
			{
				if ((File.GetAttributes(text) & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					AddListEntry(text.Substring(text.LastIndexOf(Path.DirectorySeparatorChar) + 1));
				}
			}
			path = directoryPath;
			input.text = directoryPath;
			_viewFolderPath = directoryPath;
		}
		catch (Exception ex)
		{
			Debug.LogError("Unable to open '" + directoryPath + "' " + ex);
		}
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (_children == null)
		{
			return;
		}
		foreach (GameObject child in _children)
		{
			UnityEngine.Object.Destroy(child);
		}
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__15_0()
	{
		_children.Add(OnNewFolder());
	}
}
