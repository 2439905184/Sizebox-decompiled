public class PoseAction : AgentAction
{
	private string pose;

	public PoseAction(string pose)
	{
		this.pose = pose;
		name = "Set Pose: " + this.pose;
	}

	public override void StartAction()
	{
		agent.animationManager.PlayAnimation(pose, true);
	}

	public override bool IsCompleted()
	{
		return hasStarted;
	}
}
