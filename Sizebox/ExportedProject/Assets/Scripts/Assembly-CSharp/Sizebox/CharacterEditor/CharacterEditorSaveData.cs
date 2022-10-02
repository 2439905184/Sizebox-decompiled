using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class CharacterEditorSaveData
	{
		public SkeletonEditSaveData skeletonData;

		public MaterialEditSaveData materialData;

		public JiggleEditSaveData jiggleData;
	}
}
