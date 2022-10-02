using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Bones
	{
		private readonly Animator _animator;

		public Transform head
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.Head);
			}
		}

		public Transform hips
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.Hips);
			}
		}

		public Transform spine
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.Spine);
			}
		}

		public Transform leftUpperArm
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftUpperArm);
			}
		}

		public Transform leftLowerArm
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftLowerArm);
			}
		}

		public Transform leftHand
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftHand);
			}
		}

		public Transform rightUpperArm
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightUpperArm);
			}
		}

		public Transform rightLowerArm
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightLowerArm);
			}
		}

		public Transform rightHand
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightHand);
			}
		}

		public Transform rightUpperLeg
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightUpperLeg);
			}
		}

		public Transform rightLowerLeg
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightLowerLeg);
			}
		}

		public Transform rightFoot
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.RightFoot);
			}
		}

		public Transform leftUpperLeg
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			}
		}

		public Transform leftLowerLeg
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			}
		}

		public Transform leftFoot
		{
			get
			{
				return GetBoneTransform(HumanBodyBones.LeftFoot);
			}
		}

		[MoonSharpHidden]
		public Bones(Animator animator)
		{
			if (animator == null)
			{
				Debug.LogError("No animator to create body bones.");
			}
			_animator = animator;
		}

		private Transform GetBoneTransform(HumanBodyBones humanBodyBone)
		{
			UnityEngine.Transform boneTransform = _animator.GetBoneTransform(humanBodyBone);
			if ((bool)boneTransform)
			{
				return new Transform(boneTransform);
			}
			return null;
		}

		public Transform[] GetBonesByName(string name, bool partial = true)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			List<UnityEngine.Transform> list = _animator.transform.FindAllRecursive(name, partial);
			if (list.Count < 1)
			{
				return null;
			}
			Transform[] array = new Transform[list.Count];
			uint num = 0u;
			foreach (UnityEngine.Transform item in list)
			{
				array[num] = new Transform(item);
				num++;
			}
			return array;
		}

		public Transform GetBoneByName(string name, bool partial = true)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			UnityEngine.Transform transform = _animator.transform.FindRecursive(name, partial);
			if ((bool)transform)
			{
				return new Transform(transform);
			}
			return null;
		}
	}
}
