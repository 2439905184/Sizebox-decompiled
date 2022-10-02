using System;
using System.Collections.Generic;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class SkeletonEditSliderSet
	{
		public string name = "New Set";

		public List<SkeletonEditSliderObject> sliders = new List<SkeletonEditSliderObject>();
	}
}
