public class SizeChangeAction : AgentAction
{
	private ResizeManager sizeChanger;

	private ResizeManager.Resizer resizer;

	public bool async = true;

	public SizeChangeAction(float factor)
		: this(factor, 1f, true)
	{
	}

	public SizeChangeAction(float factor, float duration)
		: this(factor, duration, false)
	{
	}

	private SizeChangeAction(float factor, float duration, bool loop)
	{
		name = "SizeChange: " + factor;
		priority = true;
		resizer = new ResizeManager.Resizer(duration, factor, loop);
	}

	public override bool CanDoBoth(AgentAction anotherAction)
	{
		return async;
	}

	public override void StartAction()
	{
		sizeChanger = agent.GetComponent<ResizeManager>() ?? agent.gameObject.AddComponent<ResizeManager>();
		sizeChanger.AddResizer(resizer);
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return resizer.IsCompleted();
		}
		return false;
	}

	public override void Interrupt()
	{
		resizer.Interrupt();
	}
}
