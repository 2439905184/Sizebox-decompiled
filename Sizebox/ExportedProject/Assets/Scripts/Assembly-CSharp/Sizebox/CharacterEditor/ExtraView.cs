using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class ExtraView : BaseSkeletonHandleView
	{
		public override string DataId
		{
			get
			{
				return "extra";
			}
		}

		protected override string DATA_FILE_NAME
		{
			get
			{
				return "extraview.dat";
			}
		}

		protected override void LoadDefaultKeys(CharacterEditor targetEditor)
		{
			base.Keys.Clear();
			Animator animator = targetEditor.Animator;
			if (!animator)
			{
				return;
			}
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
			if ((bool)boneTransform)
			{
				EditBone[] componentsInChildren = boneTransform.GetComponentsInChildren<EditBone>();
				foreach (EditBone editBone in componentsInChildren)
				{
					base.Keys.Add(editBone.Key);
				}
			}
			Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.LeftHand);
			if ((bool)boneTransform2)
			{
				EditBone[] componentsInChildren = boneTransform2.GetComponentsInChildren<EditBone>();
				foreach (EditBone editBone2 in componentsInChildren)
				{
					base.Keys.Add(editBone2.Key);
				}
			}
			boneTransform2 = animator.GetBoneTransform(HumanBodyBones.RightHand);
			if ((bool)boneTransform2)
			{
				EditBone[] componentsInChildren = boneTransform2.GetComponentsInChildren<EditBone>();
				foreach (EditBone editBone3 in componentsInChildren)
				{
					base.Keys.Add(editBone3.Key);
				}
			}
		}
	}
}
