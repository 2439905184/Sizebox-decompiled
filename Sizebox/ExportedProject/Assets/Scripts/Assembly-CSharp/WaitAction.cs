using UnityEngine;

public class WaitAction : AgentAction
{
	private float startTime;

	private float duration;

	public WaitAction(float duration)
	{
		name = "Wait";
		this.duration = duration;
	}

	public override void StartAction()
	{
		startTime = Time.time;
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return Time.time > startTime + duration;
		}
		return false;
	}
}
