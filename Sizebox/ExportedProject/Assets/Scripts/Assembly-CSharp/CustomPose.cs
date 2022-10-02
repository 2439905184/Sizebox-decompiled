using System;
using System.Collections.Generic;

[Serializable]
public class CustomPose : Pose
{
	public string animationName;

	public List<PoseBoneData> boneData;

	public List<PoseTargetData> targetData;

	public override bool IsCustom
	{
		get
		{
			return true;
		}
	}

	public CustomPose(string poseName, string animationName, List<PoseBoneData> boneData, List<PoseTargetData> targetData)
		: base(null)
	{
		name = poseName;
		this.animationName = animationName;
		this.boneData = boneData;
		this.targetData = targetData;
	}
}
