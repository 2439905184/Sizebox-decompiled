using UnityEngine;

public class BEAction : AgentAction
{
	private float startTime;

	private float duration;

	private float speed;

	private bool stop;

	private Giantess giantess;

	public BEAction(float speed, float duration = 0f)
	{
		name = "Breast Expansion: " + speed;
		this.speed = speed;
		this.duration = duration;
		stop = duration == 0f;
	}

	public override void StartAction()
	{
		startTime = Time.time;
		giantess = agent.GetComponent<Giantess>();
		giantess.StartBreastExpansion();
		giantess.SetBeSpeed(speed);
	}

	public override void UpdateAction()
	{
		if (!stop && Time.time > startTime + duration)
		{
			giantess.SetBeSpeed(0f);
			stop = true;
		}
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return stop;
		}
		return false;
	}

	public override void Interrupt()
	{
		if (!stop)
		{
			giantess.SetBeSpeed(speed);
		}
	}
}
