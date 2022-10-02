public class AgentAction
{
	public string name = "Unknown Action";

	public bool priority;

	public Humanoid agent;

	protected bool hasStarted;

	public virtual bool CanRunOnHumanoid(Humanoid humanoid)
	{
		if (humanoid != null)
		{
			return humanoid.ActionManager != null;
		}
		return false;
	}

	public virtual bool CanInterrupt()
	{
		return false;
	}

	public virtual bool CanDoBoth(AgentAction anotherAction)
	{
		return false;
	}

	public virtual bool IsCompleted()
	{
		return hasStarted;
	}

	public virtual void Execute()
	{
		if (!hasStarted)
		{
			agent.SetPoseMode(false);
			StartAction();
			hasStarted = true;
		}
		UpdateAction();
	}

	public virtual void FixedExecute()
	{
		if (!hasStarted)
		{
			StartAction();
			hasStarted = true;
		}
		FixedUpdateAction();
	}

	public virtual void StartAction()
	{
	}

	public virtual void UpdateAction()
	{
	}

	public virtual void FixedUpdateAction()
	{
	}

	public virtual void Interrupt()
	{
	}
}
