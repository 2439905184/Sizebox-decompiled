using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class SkeletonPresetData
	{
		public string hashCode;

		public BoneData[] boneData;

		public SkeletonDataMap[] skeletonMap;

		public SkeletonPresetData(string dataId, CharacterEditor editor)
		{
			hashCode = editor.SkeletonHashCode;
			Dictionary<string, EditBone> boneMap = editor.SkeletonEdit.BoneMap;
			List<BoneData> list = new List<BoneData>();
			BoneData item = default(BoneData);
			foreach (string key in boneMap.Keys)
			{
				BoneTransformData data = boneMap[key].GetData(dataId);
				if (!(data == BoneTransformData.Default))
				{
					item.key = key;
					item.data = data;
					list.Add(item);
				}
			}
			boneData = list.ToArray();
			skeletonMap = editor.SkeletonEdit.Skeleton.GenerateDataMap();
		}

		public bool Validate()
		{
			if (boneData == null || hashCode == null)
			{
				return false;
			}
			return true;
		}
	}
}
