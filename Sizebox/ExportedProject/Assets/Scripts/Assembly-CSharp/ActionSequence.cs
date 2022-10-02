public class ActionSequence : AgentAction
{
	protected AgentAction[] actions;

	private int activeIndex;

	private bool initialized;

	public override bool CanInterrupt()
	{
		return actions[0].CanInterrupt();
	}

	public override bool CanDoBoth(AgentAction anotherAction)
	{
		AgentAction[] array = actions;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CanDoBoth(anotherAction))
			{
				return false;
			}
		}
		return true;
	}

	public override bool IsCompleted()
	{
		return activeIndex >= actions.Length;
	}

	public override void Execute()
	{
		if (!initialized)
		{
			AgentAction[] array = actions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].agent = agent;
			}
			initialized = true;
		}
		priority = actions[activeIndex].priority;
		actions[activeIndex].Execute();
		if (actions[activeIndex].IsCompleted())
		{
			activeIndex++;
		}
	}
}
