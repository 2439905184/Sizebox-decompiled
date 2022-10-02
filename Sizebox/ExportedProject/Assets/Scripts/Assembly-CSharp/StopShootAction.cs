public class StopShootAction : ShooterAction
{
	public StopShootAction()
	{
		name = "StopShoot";
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		shooterController.StopFiring();
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
