using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct SkeletonDataMap
	{
		public string key;

		public SkeletonEditBones bone;

		public SkeletonDataMap(string key, SkeletonEditBones bone)
		{
			this.key = key;
			this.bone = bone;
		}
	}
}
