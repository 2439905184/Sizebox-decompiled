using SteeringBehaviors;
using UnityEngine;

public class FleeAction : AgentAction
{
	private float endTime;

	private Kinematic target;

	public FleeAction(Kinematic target, float time = 0f)
	{
		name = "Flee";
		this.target = target;
		if (time > 0f)
		{
			endTime = Time.time + time;
		}
	}

	public override void StartAction()
	{
		agent.Movement.StartFlee(target);
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
