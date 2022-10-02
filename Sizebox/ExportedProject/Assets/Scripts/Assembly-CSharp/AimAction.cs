public class AimAction : ShooterAction
{
	private EntityBase target;

	public AimAction(EntityBase target)
	{
		name = "Aim";
		this.target = target;
	}

	public override void StartAction()
	{
		shooterController = agent.gameObject.GetComponent<AIShooterController>();
		shooterController.SetAimTarget(target);
	}

	public override bool IsCompleted()
	{
		return hasStarted;
	}

	public override void Interrupt()
	{
		shooterController.StopAiming();
	}
}
