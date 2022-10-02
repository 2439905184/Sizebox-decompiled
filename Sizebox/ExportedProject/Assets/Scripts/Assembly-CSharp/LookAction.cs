public class LookAction : AgentAction
{
	private readonly EntityBase _target;

	public LookAction(EntityBase targetToLook)
	{
		if (targetToLook == null)
		{
			name = "Look Default";
		}
		else
		{
			name = "Look at " + targetToLook.name;
		}
		_target = targetToLook;
	}

	public override void StartAction()
	{
		if (agent.isGiantess)
		{
			if ((bool)agent.ik && agent.ik.head != null)
			{
				agent.ik.head.LookAt(_target);
			}
		}
		else if (agent.isMicro)
		{
			AnimatedMicroNPC animatedMicroNPC = agent as AnimatedMicroNPC;
			if ((bool)animatedMicroNPC)
			{
				animatedMicroNPC.LookAt(_target);
			}
		}
	}

	public override bool IsCompleted()
	{
		return hasStarted;
	}
}
