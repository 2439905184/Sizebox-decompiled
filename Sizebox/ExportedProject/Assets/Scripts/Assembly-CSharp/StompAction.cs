using UnityEngine;

public class StompAction : AgentAction
{
	private readonly EntityBase _targetEntity;

	private readonly Vector3 _targetPoint;

	private GiantessIK _ik;

	public StompAction(EntityBase target)
	{
		_targetEntity = target;
		name = "Stomp: " + _targetEntity.name;
	}

	public StompAction(Vector3 target)
	{
		_targetPoint = target.ToWorld();
		name = "Stomp: " + _targetPoint;
	}

	public override void StartAction()
	{
		if ((bool)_targetEntity)
		{
			Humanoid humanoid = _targetEntity as Humanoid;
			if ((bool)humanoid && humanoid.IsDead)
			{
				Interrupt();
			}
			else
			{
				agent.ik.FootIk.CrushTarget(_targetEntity);
			}
		}
		else
		{
			agent.ik.FootIk.CrushTarget(_targetPoint);
		}
	}

	public override bool IsCompleted()
	{
		if (hasStarted)
		{
			return agent.ik.FootIk.CrushEnded;
		}
		return false;
	}

	public override void Interrupt()
	{
		agent.ik.FootIk.CancelFootCrush();
	}
}
