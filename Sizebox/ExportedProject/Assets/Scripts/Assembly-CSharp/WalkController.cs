using UnityEngine;

public class WalkController : MonoBehaviour
{
	public CustomSteer customSteer;

	public void Initialize(MovementCharacter agent)
	{
		customSteer = new CustomSteer(agent, null);
	}

	public void MoveTowards(Vector3 destination)
	{
	}

	public void MoveLocalDirection(Vector3 direction)
	{
	}

	public void MoveWorldDirection(Vector3 direction)
	{
		customSteer.SetLinearSteering(direction);
	}
}
