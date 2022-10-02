using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public abstract class BasePresetsView : BaseView
	{
		[Header("Prefabs")]
		[SerializeField]
		protected PresetsFileEntryGui fileEntryPrefab;

		[Header("References")]
		[SerializeField]
		protected HandleManager handleManager;

		[SerializeField]
		protected RectTransform fileEntryParent;

		[SerializeField]
		protected SaveDialog saveDialog;

		[SerializeField]
		protected ConfirmationDialog confirmationDialog;

		[SerializeField]
		protected MessageDialog messageDialog;

		[Header("Buttons")]
		[SerializeField]
		protected Button loadButton;

		[SerializeField]
		protected Button saveButton;

		[SerializeField]
		protected Button deleteButton;

		[Header("Preset Settings")]
		[SerializeField]
		protected string presetFolder = "";

		[SerializeField]
		protected string PRESET_EXTENSION = ".preset";

		[SerializeField]
		protected string LOAD_FAIL_MESSAGE = "The selected preset could not be loaded.";

		[SerializeField]
		protected string DELETE_CONFIRMATION_MESSAGE = "Are you sure you want to delete this preset?";

		protected List<PresetsFileEntryGui> fileEntries = new List<PresetsFileEntryGui>();

		protected string folderPath;

		protected PresetsFileEntryGui targetFile;

		protected override void Awake()
		{
			base.Awake();
			loadButton.onClick.AddListener(ExecuteLoad);
			saveButton.onClick.AddListener(OnSave);
			deleteButton.onClick.AddListener(OnDeleteFile);
		}

		protected override void OnEnable()
		{
			if ((bool)handleManager.TargetEditor)
			{
				folderPath = handleManager.TargetEditor.FolderPath + Path.DirectorySeparatorChar + presetFolder;
				LoadFolderEntries(folderPath);
			}
		}

		public void SetTarget(PresetsFileEntryGui target)
		{
			if ((bool)targetFile)
			{
				targetFile.Selected = false;
			}
			targetFile = target;
			target.Selected = true;
		}

		protected void EnsureFolder(string folderPath)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}

		private void LoadFolderEntries(string folderPath)
		{
			RemoveFileEntries();
			EnsureFolder(folderPath);
			string[] files = Directory.GetFiles(folderPath);
			foreach (string filePath in files)
			{
				LoadFileEntry(filePath);
			}
		}

		private void LoadFileEntry(string filePath)
		{
			if (Path.GetExtension(filePath) == PRESET_EXTENSION && File.Exists(filePath))
			{
				PresetsFileEntryGui presetsFileEntryGui = Object.Instantiate(fileEntryPrefab, fileEntryParent);
				fileEntries.Add(presetsFileEntryGui);
				presetsFileEntryGui.Initialize(filePath);
			}
		}

		private void OnDeleteFile()
		{
			if ((bool)targetFile)
			{
				confirmationDialog.Open(DeleteTargetFile, null, DELETE_CONFIRMATION_MESSAGE);
			}
		}

		private void DeleteTargetFile()
		{
			if ((bool)targetFile)
			{
				DeleteFile(targetFile.FilePath);
			}
			LoadFolderEntries(folderPath);
		}

		private void DeleteFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private void RemoveFileEntries()
		{
			foreach (PresetsFileEntryGui fileEntry in fileEntries)
			{
				Object.Destroy(fileEntry.gameObject);
			}
			fileEntries.Clear();
		}

		private void OnSave()
		{
			string text = null;
			if ((bool)targetFile)
			{
				text = Path.GetFileNameWithoutExtension(targetFile.FilePath);
			}
			saveDialog.Open(ExecuteSave, text);
		}

		private void ExecuteSave(string fileName)
		{
			SaveFile(fileName);
			LoadFolderEntries(folderPath);
		}

		private void ExecuteLoad()
		{
			if ((bool)handleManager.TargetEditor && (bool)targetFile)
			{
				LoadFile(targetFile.FilePath);
			}
		}

		protected void OnLoadFail()
		{
			messageDialog.Open(LOAD_FAIL_MESSAGE);
		}

		protected abstract void SaveFile(string fileName);

		protected abstract void LoadFile(string filePath);
	}
}
