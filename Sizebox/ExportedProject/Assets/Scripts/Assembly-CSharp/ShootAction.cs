public class ShootAction : ShooterAction
{
	public ShootAction()
	{
		name = "Shoot";
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		shooterController.StartFiring(false);
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return shooterController.isFiring;
		}
		return false;
	}
}
