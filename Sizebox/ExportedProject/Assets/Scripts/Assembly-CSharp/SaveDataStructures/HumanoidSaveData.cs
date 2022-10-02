namespace SaveDataStructures
{
	public abstract class HumanoidSaveData : EntitySaveData
	{
		public AnimationData Animation;

		public AiSaveData AIData;

		public bool IsPosed;

		public bool IsCustomPosed;

		public CustomPose CustomPose;

		protected HumanoidSaveData(Humanoid human)
			: base(human)
		{
			Animation = new AnimationData(human);
			AIData = human.ai.GetSaveData();
			IsPosed = human.IsPosed;
			IsCustomPosed = (bool)human.ik && human.ik.PoseIkEnabled;
			if (IsCustomPosed)
			{
				CustomPose = human.ik.poseIK.SavePose("tempPose", Animation.name);
			}
		}
	}
}
