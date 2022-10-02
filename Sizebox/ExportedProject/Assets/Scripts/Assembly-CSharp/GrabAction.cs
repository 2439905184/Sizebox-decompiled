public class GrabAction : AgentAction
{
	private EntityBase target;

	public GrabAction(EntityBase targetToGrab)
	{
		name = "Grab " + targetToGrab.name;
		target = targetToGrab;
	}

	public override void StartAction()
	{
		agent.ik.hand.GrabTarget(target);
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return agent.ik.hand.GrabCompleted();
		}
		return false;
	}

	public override void Interrupt()
	{
		agent.ik.hand.CancelGrab();
	}
}
