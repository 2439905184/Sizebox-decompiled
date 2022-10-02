using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class JigglePresetData
	{
		public List<DynamicBoneData> jiggleBones;

		public JigglePresetData(CharacterEditor editor)
		{
			List<DynamicBone> jiggleSources = editor.JiggleEdit.GetJiggleSources();
			jiggleBones = new List<DynamicBoneData>();
			foreach (DynamicBone item2 in jiggleSources)
			{
				DynamicBoneData item = new DynamicBoneData(item2, editor);
				if (item.boneKey != null)
				{
					jiggleBones.Add(item);
				}
			}
		}
	}
}
