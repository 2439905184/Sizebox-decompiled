public interface IPosable : IAnimated, IEntity, IGameObject
{
	bool IsPosed { get; }

	void SetPoseMode(bool enabled);

	void SetPose(string poseName);

	void SetPoseIk(bool enabled);

	void LoadPose(CustomPose customPose);

	void CreateFingerPosers();

	void DestroyFingerPosers();

	void ShowFingerPosers(bool visible);
}
