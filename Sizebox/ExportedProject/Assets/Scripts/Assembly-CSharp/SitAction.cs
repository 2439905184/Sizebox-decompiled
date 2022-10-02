using UnityEngine;

public class SitAction : AgentAction
{
	private Vector3 target;

	private GiantessIK ik;

	public SitAction(Vector3 target)
	{
		name = "Sit: " + target;
		this.target = target;
	}

	public override void StartAction()
	{
		ik = agent.GetComponent<GiantessIK>();
		ik.SetButtTarget(target);
	}

	public override bool IsCompleted()
	{
		return hasStarted;
	}

	public override void Interrupt()
	{
		ik.CancelButtTarget();
	}
}
