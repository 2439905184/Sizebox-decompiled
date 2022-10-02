using UnityEngine;

public abstract class ObstacleDetector : EntityComponent
{
	public abstract bool CheckObstacle();

	public abstract Vector3 GetPoint();

	public abstract Vector3 GetNormal();
}
