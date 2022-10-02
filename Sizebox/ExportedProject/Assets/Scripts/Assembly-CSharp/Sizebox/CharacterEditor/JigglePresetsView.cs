using System.IO;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class JigglePresetsView : BasePresetsView
	{
		protected override void LoadFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				JigglePresetData jigglePresetData = JsonUtility.FromJson<JigglePresetData>(File.ReadAllText(filePath));
				if (jigglePresetData == null)
				{
					OnLoadFail();
				}
				else
				{
					handleManager.TargetEditor.JiggleEdit.LoadPreset(jigglePresetData);
				}
			}
		}

		protected override void SaveFile(string fileName)
		{
			EnsureFolder(folderPath);
			string value = JsonUtility.ToJson(new JigglePresetData(handleManager.TargetEditor));
			StreamWriter streamWriter = new StreamWriter(folderPath + Path.DirectorySeparatorChar + fileName + PRESET_EXTENSION);
			streamWriter.Write(value);
			streamWriter.Close();
		}
	}
}
