using SteeringBehaviors;

public class PursueAction : AgentAction
{
	private Kinematic target;

	public PursueAction(Kinematic target)
	{
		name = "Pursue";
		this.target = target;
	}

	public override void StartAction()
	{
		agent.Movement.StartPursueBehavior(target);
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
