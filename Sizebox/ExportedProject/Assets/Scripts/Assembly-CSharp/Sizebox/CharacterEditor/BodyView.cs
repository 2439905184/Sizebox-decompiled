namespace Sizebox.CharacterEditor
{
	public class BodyView : BaseSkeletonHandleView
	{
		public override string DataId
		{
			get
			{
				return "body";
			}
		}

		protected override string DATA_FILE_NAME
		{
			get
			{
				return "bodyview.dat";
			}
		}

		protected override void LoadDefaultKeys(CharacterEditor targetEditor)
		{
			base.Keys.Clear();
			foreach (EditBone bone in targetEditor.SkeletonEdit.Skeleton.Bones)
			{
				string transformKey = targetEditor.GetTransformKey(bone.RealTransform);
				if (transformKey != null)
				{
					base.Keys.Add(transformKey);
				}
			}
		}
	}
}
