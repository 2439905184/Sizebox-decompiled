using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class SkeletonEditOptions
	{
		public SkeletonEditMovementOptions movement;

		public SkeletonEditRotationOptions rotation;

		public SkeletonEditScalingOptions scaling;

		public static SkeletonEditOptions Default
		{
			get
			{
				return new SkeletonEditOptions
				{
					movement = new SkeletonEditMovementOptions
					{
						effectPairs = true,
						speed = 1f
					},
					rotation = new SkeletonEditRotationOptions
					{
						effectPairs = true
					},
					scaling = new SkeletonEditScalingOptions
					{
						effectPairs = true,
						scaleWithoutChildren = false
					}
				};
			}
		}
	}
}
