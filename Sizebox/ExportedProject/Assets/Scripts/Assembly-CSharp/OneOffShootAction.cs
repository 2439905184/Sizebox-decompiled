public class OneOffShootAction : ShooterAction
{
	public OneOffShootAction()
	{
		name = "OneOffShot";
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		shooterController.Fire();
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return !shooterController.isFiring;
		}
		return false;
	}
}
