using System.IO;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class SkeletonPresetsView : BasePresetsView
	{
		[Header("Skeleton References")]
		[SerializeField]
		protected BaseSkeletonHandleView handleView;

		protected override void LoadFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				SkeletonPresetData skeletonPresetData = JsonUtility.FromJson<SkeletonPresetData>(File.ReadAllText(filePath));
				if (skeletonPresetData == null || !skeletonPresetData.Validate())
				{
					OnLoadFail();
				}
				else
				{
					handleManager.TargetEditor.SkeletonEdit.LoadPreset(skeletonPresetData, handleView.DataId);
				}
			}
		}

		protected override void SaveFile(string fileName)
		{
			EnsureFolder(folderPath);
			string value = JsonUtility.ToJson(new SkeletonPresetData(handleView.DataId, handleManager.TargetEditor));
			StreamWriter streamWriter = new StreamWriter(folderPath + Path.DirectorySeparatorChar + fileName + PRESET_EXTENSION);
			streamWriter.Write(value);
			streamWriter.Close();
		}
	}
}
