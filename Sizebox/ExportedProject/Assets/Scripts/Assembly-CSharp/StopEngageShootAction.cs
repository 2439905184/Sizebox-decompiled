public class StopEngageShootAction : ShooterAction
{
	public StopEngageShootAction()
	{
		name = "StopEngageShoot";
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		shooterController.StopSeekFiring();
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
