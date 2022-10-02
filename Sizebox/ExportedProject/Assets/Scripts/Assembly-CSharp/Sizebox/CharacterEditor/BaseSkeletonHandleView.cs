using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public abstract class BaseSkeletonHandleView : BaseView
	{
		private List<string> _Keys = new List<string>();

		[SerializeField]
		private HandleManager handleManager;

		public abstract string DataId { get; }

		protected abstract string DATA_FILE_NAME { get; }

		public List<string> Keys
		{
			get
			{
				return _Keys;
			}
			private set
			{
				_Keys = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateKeys();
		}

		public void UpdateKeys()
		{
			CharacterEditor targetEditor = handleManager.TargetEditor;
			if ((bool)targetEditor && !LoadKeys(targetEditor, DATA_FILE_NAME))
			{
				LoadDefaultKeys(targetEditor);
			}
		}

		public void SaveKeys(CharacterEditor editor)
		{
			string folderPath = editor.FolderPath;
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string path = folderPath + DATA_FILE_NAME;
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Create(path);
			binaryFormatter.Serialize(fileStream, Keys.ToArray());
			fileStream.Close();
		}

		private bool LoadKeys(CharacterEditor editor, string fileName)
		{
			string path = editor.FolderPath + fileName;
			if (File.Exists(path))
			{
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					FileStream fileStream = File.Open(path, FileMode.Open);
					Keys = new List<string>((string[])binaryFormatter.Deserialize(fileStream));
					fileStream.Close();
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		protected abstract void LoadDefaultKeys(CharacterEditor targetEditor);
	}
}
