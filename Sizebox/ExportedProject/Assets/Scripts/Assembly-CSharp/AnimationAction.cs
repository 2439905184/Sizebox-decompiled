using UnityEngine;

public class AnimationAction : AgentAction
{
	private readonly string _animation;

	private readonly bool _smoothTransition;

	private readonly bool _waitToComplete;

	public AnimationAction(string animation, bool smooth = true, bool waitToComplete = false)
	{
		if (animation == "")
		{
			Debug.LogError("Animation name is empty");
		}
		_waitToComplete = waitToComplete;
		_animation = animation;
		name = "Play Animation: " + animation;
		_smoothTransition = smooth;
	}

	public override void StartAction()
	{
		agent.animationManager.PlayAnimation(_animation, false, !_smoothTransition);
	}

	public override bool IsCompleted()
	{
		if (_waitToComplete)
		{
			if (hasStarted)
			{
				return agent.animationManager.AnimationHasFinished();
			}
			return false;
		}
		return hasStarted;
	}
}
