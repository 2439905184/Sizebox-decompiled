using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class HairView : BaseSkeletonHandleView
	{
		public override string DataId
		{
			get
			{
				return "hair";
			}
		}

		protected override string DATA_FILE_NAME
		{
			get
			{
				return "hairview.dat";
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
		}
	}
}
