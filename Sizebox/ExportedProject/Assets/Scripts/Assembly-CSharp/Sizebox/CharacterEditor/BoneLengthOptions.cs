using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct BoneLengthOptions
	{
		public BoneLengthModes mode;

		public bool effectChildren;

		public bool effectSibling;
	}
}
