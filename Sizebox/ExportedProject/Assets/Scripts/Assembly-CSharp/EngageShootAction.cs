public class EngageShootAction : ShooterAction
{
	private EntityBase target;

	public EngageShootAction(EntityBase target)
	{
		name = "EngageShoot";
		this.target = target;
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		if ((bool)shooterController)
		{
			shooterController.StartSeekFiring(target);
		}
	}

	public override bool IsCompleted()
	{
		return hasStarted;
	}
}
