using System.IO;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class MaterialPresetsView : BasePresetsView
	{
		protected override void LoadFile(string filePath)
		{
			if ((bool)handleManager.TargetEditor && File.Exists(filePath))
			{
				MaterialEdit materialEdit = handleManager.TargetEditor.MaterialEdit;
				MaterialDataSet materialData = JsonUtility.FromJson<MaterialDataSet>(File.ReadAllText(filePath));
				materialEdit.LoadPreset(materialData);
			}
		}

		protected override void SaveFile(string fileName)
		{
			if ((bool)handleManager.TargetEditor)
			{
				string value = JsonUtility.ToJson(handleManager.TargetEditor.MaterialEdit.SavePreset());
				StreamWriter streamWriter = new StreamWriter(folderPath + Path.DirectorySeparatorChar + fileName + PRESET_EXTENSION);
				streamWriter.Write(value);
				streamWriter.Close();
			}
		}
	}
}
