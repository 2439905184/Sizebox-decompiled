using SteeringBehaviors;

public class FaceAction : AgentAction
{
	private Kinematic target;

	public FaceAction(Kinematic target)
	{
		name = "Face to " + target.position;
		this.target = target;
	}

	public override void StartAction()
	{
		agent.Movement.StartFace(target);
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
