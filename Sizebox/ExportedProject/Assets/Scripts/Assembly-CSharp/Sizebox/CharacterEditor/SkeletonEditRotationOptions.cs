using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct SkeletonEditRotationOptions
	{
		public bool effectPairs;

		public bool invertX;

		public bool invertY;

		public bool invertZ;

		public void SetAxes(bool x, bool y, bool z)
		{
			invertX = x;
			invertY = y;
			invertZ = z;
		}
	}
}
