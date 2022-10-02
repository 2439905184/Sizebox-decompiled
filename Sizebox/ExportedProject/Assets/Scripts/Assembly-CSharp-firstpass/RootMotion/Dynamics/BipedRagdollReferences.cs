using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[Serializable]
	public struct BipedRagdollReferences
	{
		public Transform root;

		public Transform hips;

		public Transform spine;

		public Transform chest;

		public Transform head;

		public Transform leftUpperLeg;

		public Transform leftLowerLeg;

		public Transform leftFoot;

		public Transform rightUpperLeg;

		public Transform rightLowerLeg;

		public Transform rightFoot;

		public Transform leftUpperArm;

		public Transform leftLowerArm;

		public Transform leftHand;

		public Transform rightUpperArm;

		public Transform rightLowerArm;

		public Transform rightHand;

		public bool IsValid(ref string msg)
		{
			if (root == null || hips == null || head == null || leftUpperArm == null || leftLowerArm == null || leftHand == null || rightUpperArm == null || rightLowerArm == null || rightHand == null || leftUpperLeg == null || leftLowerLeg == null || leftFoot == null || rightUpperLeg == null || rightLowerLeg == null || rightFoot == null)
			{
				msg = "Invalid References, one or more Transforms missing.";
				return false;
			}
			Transform[] array = new Transform[15]
			{
				root, hips, head, leftUpperArm, leftLowerArm, leftHand, rightUpperArm, rightLowerArm, rightHand, leftUpperLeg,
				leftLowerLeg, leftFoot, rightUpperLeg, rightLowerLeg, rightFoot
			};
			for (int i = 1; i < array.Length; i++)
			{
				if (!IsChildRecursive(array[i], root))
				{
					msg = "Invalid References, " + array[i].name + " is not in the Root's hierarchy.";
					return false;
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (j != k && array[j] == array[k])
					{
						msg = "Invalid References, " + array[j].name + " is represented more than once.";
						return false;
					}
				}
			}
			return true;
		}

		private bool IsChildRecursive(Transform t, Transform parent)
		{
			if (t.parent == parent)
			{
				return true;
			}
			if (t.parent != null)
			{
				return IsChildRecursive(t.parent, parent);
			}
			return false;
		}

		public bool IsEmpty(bool considerRoot)
		{
			if (considerRoot && root != null)
			{
				return false;
			}
			if (hips != null || head != null || spine != null || chest != null || leftUpperArm != null || leftLowerArm != null || leftHand != null || rightUpperArm != null || rightLowerArm != null || rightHand != null || leftUpperLeg != null || leftLowerLeg != null || leftFoot != null || rightUpperLeg != null || rightLowerLeg != null || rightFoot != null)
			{
				return false;
			}
			return true;
		}

		public bool Contains(Transform t, bool ignoreRoot = false)
		{
			if (!ignoreRoot && root == t)
			{
				return true;
			}
			if (hips == t)
			{
				return true;
			}
			if (spine == t)
			{
				return true;
			}
			if (chest == t)
			{
				return true;
			}
			if (leftUpperLeg == t)
			{
				return true;
			}
			if (leftLowerLeg == t)
			{
				return true;
			}
			if (leftFoot == t)
			{
				return true;
			}
			if (rightUpperLeg == t)
			{
				return true;
			}
			if (rightLowerLeg == t)
			{
				return true;
			}
			if (rightFoot == t)
			{
				return true;
			}
			if (leftUpperArm == t)
			{
				return true;
			}
			if (leftLowerArm == t)
			{
				return true;
			}
			if (leftHand == t)
			{
				return true;
			}
			if (rightUpperArm == t)
			{
				return true;
			}
			if (rightLowerArm == t)
			{
				return true;
			}
			if (rightHand == t)
			{
				return true;
			}
			if (head == t)
			{
				return true;
			}
			return false;
		}

		public Transform[] GetRagdollTransforms()
		{
			return new Transform[16]
			{
				hips, spine, chest, head, leftUpperArm, leftLowerArm, leftHand, rightUpperArm, rightLowerArm, rightHand,
				leftUpperLeg, leftLowerLeg, leftFoot, rightUpperLeg, rightLowerLeg, rightFoot
			};
		}

		public static BipedRagdollReferences FromAvatar(Animator animator)
		{
			BipedRagdollReferences result = default(BipedRagdollReferences);
			if (!animator.isHuman)
			{
				return result;
			}
			result.root = animator.transform;
			result.hips = animator.GetBoneTransform(HumanBodyBones.Hips);
			result.spine = animator.GetBoneTransform(HumanBodyBones.Spine);
			result.chest = animator.GetBoneTransform(HumanBodyBones.Chest);
			result.head = animator.GetBoneTransform(HumanBodyBones.Head);
			result.leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			result.leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			result.leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
			result.rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
			result.rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
			result.rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
			result.leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			result.leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			result.leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			result.rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			result.rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			result.rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			return result;
		}

		public static BipedRagdollReferences FromBipedReferences(BipedReferences biped)
		{
			BipedRagdollReferences result = default(BipedRagdollReferences);
			result.root = biped.root;
			result.hips = biped.pelvis;
			if (biped.spine != null && biped.spine.Length != 0)
			{
				result.spine = biped.spine[0];
				if (biped.spine.Length > 1)
				{
					result.chest = biped.spine[biped.spine.Length - 1];
				}
			}
			result.head = biped.head;
			result.leftUpperArm = biped.leftUpperArm;
			result.leftLowerArm = biped.leftForearm;
			result.leftHand = biped.leftHand;
			result.rightUpperArm = biped.rightUpperArm;
			result.rightLowerArm = biped.rightForearm;
			result.rightHand = biped.rightHand;
			result.leftUpperLeg = biped.leftThigh;
			result.leftLowerLeg = biped.leftCalf;
			result.leftFoot = biped.leftFoot;
			result.rightUpperLeg = biped.rightThigh;
			result.rightLowerLeg = biped.rightCalf;
			result.rightFoot = biped.rightFoot;
			return result;
		}
	}
}
