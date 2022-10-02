using UnityEngine;

public class WanderAction : AgentAction
{
	private float endTime;

	public WanderAction(float timeLimit = 0f)
	{
		name = "Wander";
		if (timeLimit > 0f)
		{
			endTime = Time.time + timeLimit;
		}
	}

	public override void StartAction()
	{
		agent.Movement.StartWanderBehavior();
	}

	public override bool IsCompleted()
	{
		if (endTime > 0f && Time.time > endTime)
		{
			Interrupt();
			return hasStarted;
		}
		if (hasStarted)
		{
			return !agent.Movement.move;
		}
		return false;
	}

	public override void Interrupt()
	{
		agent.Movement.Stop();
	}
}
