using System;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct SkeletonEditSliderData
	{
		[Header("Target Bone")]
		public SkeletonEditBones bone;

		public SkeletonEditBones secondary;

		[Header("Basic Transformations")]
		public Vector3 movement;

		public Vector3 rotation;

		public Vector3 scaling;

		[Header("Advanced Transformations")]
		public float boneLength;

		public float hipHeight;

		[Header("Options")]
		public SkeletonEditOptions options;

		public BoneLengthOptions boneLengthOptions;
	}
}
