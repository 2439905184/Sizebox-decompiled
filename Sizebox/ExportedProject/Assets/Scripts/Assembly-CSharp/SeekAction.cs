using SteeringBehaviors;

public class SeekAction : AgentAction
{
	private float separation;

	private float duration;

	private Kinematic target;

	public SeekAction(Kinematic target, float separation = 0f, float duration = 0f)
	{
		name = "Seek";
		this.target = target;
		this.separation = separation;
		this.duration = duration;
	}

	public override void StartAction()
	{
		agent.Movement.StartSeekBehavior(target, separation, duration);
	}

	public override bool IsCompleted()
	{
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
