using SteeringBehaviors;

public class ArriveAction : AgentAction
{
	private readonly Kinematic _target;

	public ArriveAction(Kinematic target)
	{
		name = "Arrive to " + target.position;
		_target = target;
	}

	public override void StartAction()
	{
		agent.Movement.StartArriveBehavior(_target);
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
