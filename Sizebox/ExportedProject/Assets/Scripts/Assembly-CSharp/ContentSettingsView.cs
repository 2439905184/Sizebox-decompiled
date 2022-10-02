using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Pause;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContentSettingsView : SettingsView
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_0
	{
		public ContentSettingsView _003C_003E4__this;

		public UISelectFolder selectFolder;

		public InputField inputField;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_1
	{
		public UiMessageBox subdirectoryPrompt;

		public _003C_003Ec__DisplayClass17_0 CS_0024_003C_003E8__locals1;

		internal void _003CSelectFolder_003Eb__0()
		{
			CS_0024_003C_003E8__locals1._003C_003E4__this.CreateAndSelectFolder(subdirectoryPrompt, CS_0024_003C_003E8__locals1.selectFolder, CS_0024_003C_003E8__locals1.inputField, CS_0024_003C_003E8__locals1.selectFolder.subDirectories);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public ContentSettingsView _003C_003E4__this;

		public InputField inputField;

		internal void _003CCreateFolderSelect_003Eb__0(UISelectFolder f)
		{
			_003C_003E4__this.SelectFolder(f, inputField);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public ContentSettingsView _003C_003E4__this;

		public UISelectFolder selectFolder;

		public List<string> allFolders;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_1
	{
		public UiMessageBox messageBox1;

		public _003C_003Ec__DisplayClass20_0 CS_0024_003C_003E8__locals1;

		internal void _003CSetupAllPaths_003Eb__0()
		{
			CS_0024_003C_003E8__locals1._003C_003E4__this.CreateAllFolders(messageBox1, CS_0024_003C_003E8__locals1.selectFolder, CS_0024_003C_003E8__locals1.allFolders);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_2
	{
		public UiPopup[] messageBoxes;

		internal void _003CSetupAllPaths_003Eb__1()
		{
			UiPopup.CloseAll(messageBoxes);
		}
	}

	private PathInput _models;

	private PathInput _scenes;

	private PathInput _script;

	private PathInput _sounds;

	private PathInput _data;

	private PathInput _screenshots;

	private PathInput _game;

	private PathInput _user;

	private Button _setup;

	private readonly List<string> _modelSubDir = new List<string> { "Giantess", "MaleNPC", "FemaleNPC", "Objects" };

	private readonly List<string> _dataSubDir = new List<string> { "Saves", "Config", "Character" };

	private GameObject _folderSelect;

	private List<GameObject> _children;

	private void OnDisable()
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

	private void CreateAndSelectFolder(UiMessageBox uiMessageBox, UISelectFolder selectFolder, InputField inputField, List<string> subDirectories)
	{
		Sbox.CreateSubDirectory(selectFolder.path, subDirectories);
		inputField.text = selectFolder.path;
		selectFolder.Close();
		uiMessageBox.Close();
	}

	private bool DirectoryIsEmpty(string directory, bool directoryOnly = false, List<string> subDirectories = null)
	{
		if (!directoryOnly && Directory.GetFiles(directory).Length != 0)
		{
			return false;
		}
		int num = Directory.GetDirectories(directory).Length;
		if (num == 0)
		{
			return true;
		}
		int num2 = 0;
		if (subDirectories != null)
		{
			foreach (string subDirectory in subDirectories)
			{
				if (Directory.Exists(Path.Combine(directory, subDirectory)))
				{
					num2++;
				}
			}
			if (num2 == subDirectories.Count)
			{
				return true;
			}
		}
		if (num > num2)
		{
			return false;
		}
		return true;
	}

	private bool AllDirectoriesExists(string directory, List<string> subDirectories)
	{
		foreach (string subDirectory in subDirectories)
		{
			if (!Directory.Exists(Path.Combine(directory, subDirectory)))
			{
				return false;
			}
		}
		return true;
	}

	private void SelectFolder(UISelectFolder selectFolder, InputField inputField)
	{
		_003C_003Ec__DisplayClass17_0 _003C_003Ec__DisplayClass17_ = new _003C_003Ec__DisplayClass17_0();
		_003C_003Ec__DisplayClass17_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass17_.selectFolder = selectFolder;
		_003C_003Ec__DisplayClass17_.inputField = inputField;
		if (string.IsNullOrEmpty(_003C_003Ec__DisplayClass17_.selectFolder.path))
		{
			_003C_003Ec__DisplayClass17_.inputField.text = string.Empty;
			_003C_003Ec__DisplayClass17_.selectFolder.Close();
		}
		else if (Directory.Exists(_003C_003Ec__DisplayClass17_.selectFolder.path))
		{
			if (_003C_003Ec__DisplayClass17_.selectFolder.subDirectories != null && !AllDirectoriesExists(_003C_003Ec__DisplayClass17_.selectFolder.path, _003C_003Ec__DisplayClass17_.selectFolder.subDirectories))
			{
				_003C_003Ec__DisplayClass17_1 _003C_003Ec__DisplayClass17_2 = new _003C_003Ec__DisplayClass17_1();
				_003C_003Ec__DisplayClass17_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass17_;
				_003C_003Ec__DisplayClass17_2.subdirectoryPrompt = UiMessageBox.Create();
				_003C_003Ec__DisplayClass17_2.subdirectoryPrompt.message.text = "A folder structure within '" + _003C_003Ec__DisplayClass17_2.CS_0024_003C_003E8__locals1.selectFolder.path + "' is required but not all folders exist.\n\n Would you like Sizebox to create them and continue?";
				_003C_003Ec__DisplayClass17_2.subdirectoryPrompt.title.text = "Subdirectories";
				_003C_003Ec__DisplayClass17_2.subdirectoryPrompt.AddButtonsYesNo(_003C_003Ec__DisplayClass17_2._003CSelectFolder_003Eb__0);
				_003C_003Ec__DisplayClass17_2.subdirectoryPrompt.Popup();
				_children.Add(_003C_003Ec__DisplayClass17_2.subdirectoryPrompt.gameObject);
			}
			else
			{
				_003C_003Ec__DisplayClass17_.inputField.text = _003C_003Ec__DisplayClass17_.selectFolder.path;
				_003C_003Ec__DisplayClass17_.selectFolder.Close();
			}
		}
		else
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("Unknown Error while creating subdirectories", "Unforeseen error");
			uiMessageBox.ToLogError();
			uiMessageBox.Popup();
		}
	}

	public GameObject CreateFolderSelect(InputField inputField, List<string> subDirectories = null)
	{
		_003C_003Ec__DisplayClass18_0 _003C_003Ec__DisplayClass18_ = new _003C_003Ec__DisplayClass18_0();
		_003C_003Ec__DisplayClass18_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass18_.inputField = inputField;
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCanvas");
		GameObject obj = UnityEngine.Object.Instantiate(_folderSelect, gameObject.transform, false);
		UISelectFolder component = obj.GetComponent<UISelectFolder>();
		component.path = _003C_003Ec__DisplayClass18_.inputField.text;
		component.subDirectories = subDirectories;
		component.selected = (UnityAction<UISelectFolder>)Delegate.Combine(component.selected, new UnityAction<UISelectFolder>(_003C_003Ec__DisplayClass18_._003CCreateFolderSelect_003Eb__0));
		component.cancelButton.onClick.AddListener(component.Close);
		return obj;
	}

	public GameObject CreateFolderSelectAll()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCanvas");
		GameObject obj = UnityEngine.Object.Instantiate(_folderSelect, gameObject.transform, false);
		UISelectFolder component = obj.GetComponent<UISelectFolder>();
		component.selected = (UnityAction<UISelectFolder>)Delegate.Combine(component.selected, new UnityAction<UISelectFolder>(SetupAllPaths));
		component.cancelButton.onClick.AddListener(component.Close);
		return obj;
	}

	private void SetupAllPaths(UISelectFolder selectFolder)
	{
		_003C_003Ec__DisplayClass20_0 _003C_003Ec__DisplayClass20_ = new _003C_003Ec__DisplayClass20_0();
		_003C_003Ec__DisplayClass20_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass20_.selectFolder = selectFolder;
		if (string.IsNullOrEmpty(_003C_003Ec__DisplayClass20_.selectFolder.input.text))
		{
			InputField inputField = _models.inputField;
			InputField inputField2 = _data.inputField;
			InputField inputField3 = _scenes.inputField;
			InputField inputField4 = _script.inputField;
			InputField inputField5 = _sounds.inputField;
			string text = (_screenshots.inputField.text = string.Empty);
			string text3 = (inputField5.text = text);
			string text5 = (inputField4.text = text3);
			string text7 = (inputField3.text = text5);
			string text9 = (inputField2.text = text7);
			inputField.text = text9;
			_003C_003Ec__DisplayClass20_.selectFolder.Close();
			return;
		}
		_003C_003Ec__DisplayClass20_.allFolders = new List<string> { "Scenes", "Scripts", "Sounds", "Screenshots" };
		_003C_003Ec__DisplayClass20_.allFolders.AddRange(_modelSubDir);
		_003C_003Ec__DisplayClass20_.allFolders.AddRange(_dataSubDir);
		if (!AllDirectoriesExists(_003C_003Ec__DisplayClass20_.selectFolder.path, _003C_003Ec__DisplayClass20_.allFolders))
		{
			_003C_003Ec__DisplayClass20_1 _003C_003Ec__DisplayClass20_2 = new _003C_003Ec__DisplayClass20_1();
			_003C_003Ec__DisplayClass20_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass20_;
			_003C_003Ec__DisplayClass20_2.messageBox1 = UiMessageBox.Create();
			_003C_003Ec__DisplayClass20_2.messageBox1.message.text = "A folder structure within '" + _003C_003Ec__DisplayClass20_2.CS_0024_003C_003E8__locals1.selectFolder.path + "' is required but not all folders exist.\n\n Would you like Sizebox to create them and continue?";
			_003C_003Ec__DisplayClass20_2.messageBox1.title.text = "Subdirectories";
			_003C_003Ec__DisplayClass20_2.messageBox1.AddButtonsYesNo(_003C_003Ec__DisplayClass20_2._003CSetupAllPaths_003Eb__0);
			_003C_003Ec__DisplayClass20_2.messageBox1.Popup();
			_children.Add(_003C_003Ec__DisplayClass20_2.messageBox1.gameObject);
			if (!DirectoryIsEmpty(_003C_003Ec__DisplayClass20_2.CS_0024_003C_003E8__locals1.selectFolder.path))
			{
				_003C_003Ec__DisplayClass20_2 _003C_003Ec__DisplayClass20_3 = new _003C_003Ec__DisplayClass20_2();
				UiMessageBox uiMessageBox = UiMessageBox.Create();
				uiMessageBox.message.text = "Folder '" + _003C_003Ec__DisplayClass20_2.CS_0024_003C_003E8__locals1.selectFolder.path + "' is not empty.\n\n Are you sure you want to continue?";
				uiMessageBox.title.text = "Non Empty directory";
				_003C_003Ec__DisplayClass20_3.messageBoxes = new UiPopup[2] { _003C_003Ec__DisplayClass20_2.messageBox1, uiMessageBox };
				uiMessageBox.AddButtonsYesNo(null, _003C_003Ec__DisplayClass20_3._003CSetupAllPaths_003Eb__1);
				uiMessageBox.Popup();
				_children.Add(uiMessageBox.gameObject);
			}
		}
		else
		{
			SelectAllFolders(_003C_003Ec__DisplayClass20_.selectFolder);
		}
	}

	private void CreateAllFolders(UiMessageBox uiMessageBox, UISelectFolder selectFolder, List<string> subDirectories)
	{
		Sbox.CreateSubDirectory(selectFolder.path, subDirectories);
		SelectAllFolders(selectFolder);
		uiMessageBox.Close();
	}

	private void SelectAllFolders(UISelectFolder selectFolder)
	{
		string path = selectFolder.path;
		List<string> subDirectories = new List<string> { "Saves", "Character", "Config" };
		List<string> subDirectories2 = new List<string> { "Giantess", "Objects", "FemaleNPC", "MaleNPC" };
		if (FolderOk(path, subDirectories))
		{
			_data.inputField.text = path;
		}
		if (FolderOk(path, subDirectories2))
		{
			_models.inputField.text = path;
		}
		path = Path.Combine(selectFolder.path, "Scenes");
		if (FolderOk(path))
		{
			_scenes.inputField.text = path;
		}
		path = Path.Combine(selectFolder.path, "Scripts");
		if (FolderOk(path))
		{
			_script.inputField.text = path;
		}
		path = Path.Combine(selectFolder.path, "Sounds");
		if (FolderOk(path))
		{
			_sounds.inputField.text = path;
		}
		path = Path.Combine(selectFolder.path, "Screenshots");
		if (FolderOk(path))
		{
			_screenshots.inputField.text = path;
		}
		selectFolder.Close();
	}

	private bool FolderOk(string path)
	{
		if (Directory.Exists(path))
		{
			return true;
		}
		UiMessageBox.Create("Couldn't set directory '" + path + "'").Popup();
		return false;
	}

	private bool FolderOk(string path, List<string> subDirectories)
	{
		int num = 0;
		foreach (string subDirectory in subDirectories)
		{
			string text = Path.Combine(path, subDirectory);
			if (!Directory.Exists(text))
			{
				num++;
				UiMessageBox.Create("Couldn't find directory '" + text + "' required in '" + path + "'").Popup();
			}
		}
		return num == 0;
	}

	private void UpdateRestartPath(string path, StringStored save, bool restartRequired = true)
	{
		UpdatePath(path, save);
		if (restartRequired)
		{
			UiMessageBox uiMessageBox = UiMessageBox.Create("You will need to restart Sizebox for this change to take effect", "Restart Required");
			uiMessageBox.Popup();
			_children.Add(uiMessageBox.gameObject);
		}
	}

	private void UpdatePath(string path, StringStored save)
	{
		save.value = path;
		if (IOManager.Initialized)
		{
			IOManager.Instance.RecheckLivePaths();
		}
	}

	private void Start()
	{
		_folderSelect = Resources.Load<GameObject>("UI/UISelectFolder");
		_children = new List<GameObject>();
		base.Title = "Content";
		AddHeader("Custom");
		_models = AddPath("Models", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathModel, "-path-models"), true);
		_models.viewButton.onClick.AddListener(_003CStart_003Eb__27_0);
		_models.editButton.onClick.AddListener(_003CStart_003Eb__27_1);
		_models.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_2);
		_scenes = AddPath("Scenes", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScene, "-path-scenes", "Scenes"), true);
		_scenes.viewButton.onClick.AddListener(_003CStart_003Eb__27_3);
		_scenes.editButton.onClick.AddListener(_003CStart_003Eb__27_4);
		_scenes.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_5);
		_script = AddPath("Scripts", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScript, "-path-scripts", "Scripts"), true);
		_script.viewButton.onClick.AddListener(_003CStart_003Eb__27_6);
		_script.editButton.onClick.AddListener(_003CStart_003Eb__27_7);
		_script.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_8);
		_sounds = AddPath("Sounds", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathSound, "-path-sounds", "Sounds"), true);
		_sounds.viewButton.onClick.AddListener(_003CStart_003Eb__27_9);
		_sounds.editButton.onClick.AddListener(_003CStart_003Eb__27_10);
		_sounds.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_11);
		_data = AddPath("Data", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathData, "-path-data"), true);
		_data.viewButton.onClick.AddListener(_003CStart_003Eb__27_12);
		_data.editButton.onClick.AddListener(_003CStart_003Eb__27_13);
		_data.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_14);
		_screenshots = AddPath("Screenshots", Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScreenshot, "-path-screenshots", "Screenshots"), true);
		_screenshots.viewButton.onClick.AddListener(_003CStart_003Eb__27_15);
		_screenshots.editButton.onClick.AddListener(_003CStart_003Eb__27_16);
		_screenshots.inputField.onValueChanged.AddListener(_003CStart_003Eb__27_17);
		_setup = AddButton("Set all...");
		_setup.onClick.AddListener(_003CStart_003Eb__27_18);
		AddHeader("Default");
		_game = AddPath("Game path", IOManager.GetApplicationDirectory());
		_game.viewButton.onClick.AddListener(_003CStart_003Eb__27_19);
		_user = AddPath("User path", IOManager.GetUserDirectory());
		_user.viewButton.onClick.AddListener(_003CStart_003Eb__27_20);
		initialized = true;
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_0()
	{
		Sbox.OsViewFolder(_models.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_1()
	{
		_children.Add(CreateFolderSelect(_models.inputField, _modelSubDir));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_2(string x)
	{
		UpdateRestartPath(x, GlobalPreferences.PathModel, IOManager.Initialized);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_3()
	{
		Sbox.OsViewFolder(_scenes.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_4()
	{
		_children.Add(CreateFolderSelect(_scenes.inputField));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_5(string x)
	{
		UpdateRestartPath(x, GlobalPreferences.PathScene, SceneLoader.Initialized);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_6()
	{
		Sbox.OsViewFolder(_script.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_7()
	{
		_children.Add(CreateFolderSelect(_script.inputField));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_8(string x)
	{
		UpdateRestartPath(x, GlobalPreferences.PathScript, LuaManager.Initialized);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_9()
	{
		Sbox.OsViewFolder(_sounds.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_10()
	{
		_children.Add(CreateFolderSelect(_sounds.inputField));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_11(string x)
	{
		UpdateRestartPath(x, GlobalPreferences.PathSound, IOManager.Initialized);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_12()
	{
		Sbox.OsViewFolder(_data.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_13()
	{
		_children.Add(CreateFolderSelect(_data.inputField, _dataSubDir));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_14(string x)
	{
		UpdatePath(x, GlobalPreferences.PathData);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_15()
	{
		Sbox.OsViewFolder(_screenshots.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_16()
	{
		_children.Add(CreateFolderSelect(_screenshots.inputField));
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_17(string x)
	{
		UpdatePath(x, GlobalPreferences.PathScreenshot);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_18()
	{
		_children.Add(CreateFolderSelectAll());
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_19()
	{
		Sbox.OsViewFolder(_game.inputField.text);
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__27_20()
	{
		Sbox.OsViewFolder(_user.inputField.text);
	}
}
