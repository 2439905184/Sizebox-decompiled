using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditSkeleton
	{
		public EditBone head;

		public EditBone neck;

		public EditBone leftShoulder;

		public EditBone leftUpperArm;

		public EditBone leftLowerArm;

		public EditBone leftHand;

		public EditBone rightShoulder;

		public EditBone rightUpperArm;

		public EditBone rightLowerArm;

		public EditBone rightHand;

		public EditBone upperSpine;

		public EditBone midSpine;

		public EditBone lowerSpine;

		public EditBone leftBreast;

		public EditBone rightBreast;

		public EditBone hips;

		public EditBone leftUpperLeg;

		public EditBone leftLowerLeg;

		public EditBone leftFoot;

		public EditBone rightUpperLeg;

		public EditBone rightLowerLeg;

		public EditBone rightFoot;

		public List<EditBone> Bones { get; private set; }

		public void Initialize(Animator animator)
		{
			Bones = new List<EditBone>();
			if ((bool)animator && animator.isHuman)
			{
				FindLowerBody(animator);
				FindSpine(animator);
				FindArms(animator);
				FindBreasts(animator);
				if ((bool)head)
				{
					Bones.Add(head);
				}
				if ((bool)neck)
				{
					Bones.Add(neck);
				}
				if ((bool)leftShoulder)
				{
					Bones.Add(leftShoulder);
				}
				if ((bool)leftUpperArm)
				{
					Bones.Add(leftUpperArm);
				}
				if ((bool)leftLowerArm)
				{
					Bones.Add(leftLowerArm);
				}
				if ((bool)leftHand)
				{
					Bones.Add(leftHand);
				}
				if ((bool)rightShoulder)
				{
					Bones.Add(rightShoulder);
				}
				if ((bool)rightUpperArm)
				{
					Bones.Add(rightUpperArm);
				}
				if ((bool)rightLowerArm)
				{
					Bones.Add(rightLowerArm);
				}
				if ((bool)rightHand)
				{
					Bones.Add(rightHand);
				}
				if ((bool)leftBreast)
				{
					Bones.Add(leftBreast);
				}
				if ((bool)rightBreast)
				{
					Bones.Add(rightBreast);
				}
				if ((bool)hips)
				{
					Bones.Add(hips);
				}
				if ((bool)lowerSpine)
				{
					Bones.Add(lowerSpine);
				}
				if ((bool)midSpine)
				{
					Bones.Add(midSpine);
				}
				if ((bool)upperSpine)
				{
					Bones.Add(upperSpine);
				}
				if ((bool)leftUpperLeg)
				{
					Bones.Add(leftUpperLeg);
				}
				if ((bool)leftLowerLeg)
				{
					Bones.Add(leftLowerLeg);
				}
				if ((bool)leftFoot)
				{
					Bones.Add(leftFoot);
				}
				if ((bool)rightUpperLeg)
				{
					Bones.Add(rightUpperLeg);
				}
				if ((bool)rightLowerLeg)
				{
					Bones.Add(rightLowerLeg);
				}
				if ((bool)rightFoot)
				{
					Bones.Add(rightFoot);
				}
			}
		}

		public void GeneratePairings(SkeletonEdit editor)
		{
			editor.CreatePair(leftShoulder, rightShoulder);
			editor.CreatePair(leftUpperArm, rightUpperArm);
			editor.CreatePair(leftLowerArm, rightLowerArm);
			editor.CreatePair(leftHand, rightHand);
			editor.CreatePair(leftUpperLeg, rightUpperLeg);
			editor.CreatePair(leftLowerLeg, rightLowerLeg);
			editor.CreatePair(leftFoot, rightFoot);
			editor.CreatePair(leftBreast, rightBreast);
		}

		private void FindSpine(Animator animator)
		{
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
			hips = GetEditBone(boneTransform);
			Transform inTransform;
			Transform middleSpine;
			Transform inTransform2;
			FindSpineTransforms(animator, out inTransform, out middleSpine, out inTransform2);
			lowerSpine = GetEditBone(inTransform2);
			midSpine = GetEditBone(middleSpine);
			upperSpine = GetEditBone(inTransform);
			Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.Neck);
			neck = GetEditBone(boneTransform2);
			Transform boneTransform3 = animator.GetBoneTransform(HumanBodyBones.Head);
			head = GetEditBone(boneTransform3);
		}

		private void FindSpineTransforms(Animator animator, out Transform upperSpine, out Transform middleSpine, out Transform lowerSpine)
		{
			EditBone componentInParent = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponentInParent<EditBone>();
			upperSpine = null;
			middleSpine = null;
			lowerSpine = null;
			Transform transform = null;
			EditBone editBone = null;
			if ((bool)animator.GetBoneTransform(HumanBodyBones.Neck))
			{
				transform = animator.GetBoneTransform(HumanBodyBones.Neck);
			}
			else
			{
				if (!animator.GetBoneTransform(HumanBodyBones.Head))
				{
					return;
				}
				transform = animator.GetBoneTransform(HumanBodyBones.Head);
			}
			editBone = transform.GetComponentInParent<EditBone>();
			EditBone parentBone = editBone.ParentBone;
			int num = 0;
			while (parentBone != componentInParent)
			{
				num++;
				parentBone = parentBone.ParentBone;
			}
			switch (num)
			{
			case 2:
				middleSpine = editBone.ParentBone.RealTransform;
				lowerSpine = editBone.ParentBone.ParentBone.RealTransform;
				break;
			case 3:
				upperSpine = editBone.ParentBone.RealTransform;
				middleSpine = editBone.ParentBone.ParentBone.RealTransform;
				lowerSpine = editBone.ParentBone.ParentBone.ParentBone.RealTransform;
				break;
			default:
			{
				upperSpine = editBone.ParentBone.RealTransform;
				int n = num / 2;
				middleSpine = GetNthParent(editBone, n).RealTransform;
				lowerSpine = GetNthParent(editBone, num).RealTransform;
				break;
			}
			}
		}

		private static EditBone GetNthParent(EditBone bone, int n)
		{
			if (n <= 0)
			{
				return bone;
			}
			EditBone parentBone = bone.ParentBone;
			for (int i = 1; i < n; i++)
			{
				parentBone = parentBone.ParentBone;
			}
			return parentBone;
		}

		private void FindArms(Animator animator)
		{
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
			Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
			Transform boneTransform3 = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			Transform boneTransform4 = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
			Transform boneTransform5 = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			Transform boneTransform6 = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
			Transform boneTransform7 = animator.GetBoneTransform(HumanBodyBones.LeftHand);
			Transform boneTransform8 = animator.GetBoneTransform(HumanBodyBones.RightHand);
			leftShoulder = GetEditBone(boneTransform);
			rightShoulder = GetEditBone(boneTransform2);
			leftUpperArm = GetEditBone(boneTransform3);
			rightUpperArm = GetEditBone(boneTransform4);
			leftLowerArm = GetEditBone(boneTransform5);
			rightLowerArm = GetEditBone(boneTransform6);
			leftHand = GetEditBone(boneTransform7);
			rightHand = GetEditBone(boneTransform8);
		}

		private void FindLowerBody(Animator animator)
		{
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			Transform boneTransform3 = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			Transform boneTransform4 = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			Transform boneTransform5 = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			Transform boneTransform6 = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			if ((bool)boneTransform && (bool)boneTransform2)
			{
				EditBone editBone = GetEditBone(boneTransform);
				EditBone editBone2 = GetEditBone(boneTransform2);
				leftUpperLeg = editBone;
				rightUpperLeg = editBone2;
			}
			if ((bool)boneTransform3 && (bool)boneTransform4)
			{
				EditBone editBone = GetEditBone(boneTransform3);
				EditBone editBone2 = GetEditBone(boneTransform4);
				leftLowerLeg = editBone;
				rightLowerLeg = editBone2;
			}
			if ((bool)boneTransform5 && (bool)boneTransform6)
			{
				EditBone editBone = GetEditBone(boneTransform5);
				EditBone editBone2 = GetEditBone(boneTransform6);
				leftFoot = editBone;
				rightFoot = editBone2;
			}
		}

		private void FindBreasts(Animator animator)
		{
			Transform inTransform;
			Transform inTransform2;
			BoneUtil.FindBreastBones(animator, out inTransform, out inTransform2);
			leftBreast = GetEditBone(inTransform);
			rightBreast = GetEditBone(inTransform2);
		}

		private static EditBone GetEditBone(Transform inTransform)
		{
			if (!inTransform)
			{
				return null;
			}
			EditBone componentInParent = inTransform.GetComponentInParent<EditBone>();
			if (!inTransform.GetComponent<EditBone>() && (bool)componentInParent && componentInParent.RealTransform == inTransform)
			{
				return componentInParent;
			}
			return null;
		}

		public SkeletonDataMap[] GenerateDataMap()
		{
			List<SkeletonDataMap> list = new List<SkeletonDataMap>();
			if ((bool)head)
			{
				list.Add(new SkeletonDataMap(head.Key, SkeletonEditBones.Head));
			}
			if ((bool)neck)
			{
				list.Add(new SkeletonDataMap(neck.Key, SkeletonEditBones.Neck));
			}
			if ((bool)leftShoulder)
			{
				list.Add(new SkeletonDataMap(leftShoulder.Key, SkeletonEditBones.LeftShoulder));
			}
			if ((bool)rightShoulder)
			{
				list.Add(new SkeletonDataMap(rightShoulder.Key, SkeletonEditBones.RightShoulder));
			}
			if ((bool)leftUpperArm)
			{
				list.Add(new SkeletonDataMap(leftUpperArm.Key, SkeletonEditBones.LeftUpperArm));
			}
			if ((bool)leftLowerArm)
			{
				list.Add(new SkeletonDataMap(leftLowerArm.Key, SkeletonEditBones.LeftLowerArm));
			}
			if ((bool)leftHand)
			{
				list.Add(new SkeletonDataMap(leftHand.Key, SkeletonEditBones.LeftHand));
			}
			if ((bool)rightUpperArm)
			{
				list.Add(new SkeletonDataMap(rightUpperArm.Key, SkeletonEditBones.RightUpperArm));
			}
			if ((bool)rightLowerArm)
			{
				list.Add(new SkeletonDataMap(rightLowerArm.Key, SkeletonEditBones.RightLowerArm));
			}
			if ((bool)rightHand)
			{
				list.Add(new SkeletonDataMap(rightHand.Key, SkeletonEditBones.RightHand));
			}
			if ((bool)leftUpperLeg)
			{
				list.Add(new SkeletonDataMap(leftUpperLeg.Key, SkeletonEditBones.LeftUpperLeg));
			}
			if ((bool)leftLowerLeg)
			{
				list.Add(new SkeletonDataMap(leftLowerLeg.Key, SkeletonEditBones.LeftLowerLeg));
			}
			if ((bool)leftFoot)
			{
				list.Add(new SkeletonDataMap(leftFoot.Key, SkeletonEditBones.LeftFoot));
			}
			if ((bool)rightUpperLeg)
			{
				list.Add(new SkeletonDataMap(rightUpperLeg.Key, SkeletonEditBones.RightUpperLeg));
			}
			if ((bool)rightLowerLeg)
			{
				list.Add(new SkeletonDataMap(rightLowerLeg.Key, SkeletonEditBones.RightLowerLeg));
			}
			if ((bool)rightFoot)
			{
				list.Add(new SkeletonDataMap(rightFoot.Key, SkeletonEditBones.RightFoot));
			}
			if ((bool)upperSpine)
			{
				list.Add(new SkeletonDataMap(upperSpine.Key, SkeletonEditBones.UpperSpine));
			}
			if ((bool)midSpine)
			{
				list.Add(new SkeletonDataMap(midSpine.Key, SkeletonEditBones.MidSpine));
			}
			if ((bool)lowerSpine)
			{
				list.Add(new SkeletonDataMap(lowerSpine.Key, SkeletonEditBones.LowerSpine));
			}
			if ((bool)hips)
			{
				list.Add(new SkeletonDataMap(hips.Key, SkeletonEditBones.Hips));
			}
			if ((bool)leftBreast)
			{
				list.Add(new SkeletonDataMap(leftBreast.Key, SkeletonEditBones.LeftBreast));
			}
			if ((bool)rightBreast)
			{
				list.Add(new SkeletonDataMap(rightBreast.Key, SkeletonEditBones.RightBreast));
			}
			return list.ToArray();
		}

		public void SetData(SkeletonEditBones bone, string dataId, BoneTransformData transformation)
		{
			switch (bone)
			{
			case SkeletonEditBones.Head:
				if ((bool)head)
				{
					head.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.Neck:
				if ((bool)neck)
				{
					neck.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftShoulder:
				if ((bool)leftShoulder)
				{
					leftShoulder.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftUpperArm:
				if ((bool)leftUpperArm)
				{
					leftUpperArm.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftLowerArm:
				if ((bool)leftLowerArm)
				{
					leftLowerArm.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftHand:
				if ((bool)leftHand)
				{
					leftHand.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightShoulder:
				if ((bool)rightShoulder)
				{
					rightShoulder.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightUpperArm:
				if ((bool)rightUpperArm)
				{
					rightUpperArm.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightLowerArm:
				if ((bool)rightLowerArm)
				{
					rightLowerArm.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightHand:
				if ((bool)rightHand)
				{
					rightHand.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftBreast:
				if ((bool)leftBreast)
				{
					leftBreast.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightBreast:
				if ((bool)leftBreast)
				{
					leftBreast.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.UpperSpine:
				if ((bool)upperSpine)
				{
					upperSpine.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.MidSpine:
				if ((bool)midSpine)
				{
					midSpine.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LowerSpine:
				if ((bool)lowerSpine)
				{
					lowerSpine.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.Hips:
				if ((bool)hips)
				{
					hips.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftUpperLeg:
				if ((bool)leftUpperLeg)
				{
					leftUpperLeg.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftLowerLeg:
				if ((bool)leftLowerLeg)
				{
					leftLowerLeg.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.LeftFoot:
				if ((bool)leftFoot)
				{
					leftFoot.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightUpperLeg:
				if ((bool)rightUpperLeg)
				{
					rightUpperLeg.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightLowerLeg:
				if ((bool)rightLowerLeg)
				{
					rightLowerLeg.SetData(dataId, transformation);
				}
				break;
			case SkeletonEditBones.RightFoot:
				if ((bool)rightFoot)
				{
					rightFoot.SetData(dataId, transformation);
				}
				break;
			}
		}
	}
}
