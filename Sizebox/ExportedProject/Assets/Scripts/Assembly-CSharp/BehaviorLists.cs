using System.Collections.Generic;
using UnityEngine;

public class BehaviorLists
{
	public static BehaviorLists Instance;

	private List<IBehavior> _behaviorList;

	public static void Initialize()
	{
		Instance = new BehaviorLists();
	}

	public void AddBehavior(IBehavior behavior)
	{
		if (_behaviorList == null)
		{
			_behaviorList = new List<IBehavior>();
		}
		_behaviorList.Add(behavior);
	}

	public IBehavior GetBehavior(string behaviorName)
	{
		for (int i = 0; i < _behaviorList.Count; i++)
		{
			if (_behaviorList[i].GetName() == behaviorName)
			{
				return _behaviorList[i];
			}
		}
		Debug.LogError("Behavior " + behaviorName + " not found");
		return null;
	}

	public List<IBehavior> FindBehaviors(EntityBase agent, EntityBase target)
	{
		return FindBehaviors(FindDefs(agent), FindDefs(target, agent));
	}

	public List<IBehavior> FindBehaviors(List<TargetType> agentDefs, List<TargetType> targetDefs)
	{
		List<IBehavior> list = new List<IBehavior>();
		for (int i = 0; i < _behaviorList.Count; i++)
		{
			IBehavior behavior = _behaviorList[i];
			if (behavior.IsEnabled() && EntityMatch(agentDefs, behavior.GetAgentDef()) && !IsExcluded(agentDefs, behavior.GetAgentDef()) && EntityMatch(targetDefs, behavior.GetTargetDef()) && !IsExcluded(targetDefs, behavior.GetTargetDef()))
			{
				list.Add(behavior);
			}
		}
		return list;
	}

	public List<IBehavior> GetAllBehaviors()
	{
		List<IBehavior> list = new List<IBehavior>();
		list.AddRange(_behaviorList);
		return list;
	}

	public List<TargetType> FindDefs(EntityBase a, EntityBase b = null)
	{
		List<TargetType> list = new List<TargetType>();
		if (!a)
		{
			list.Add(TargetType.None);
			return list;
		}
		if (a == b)
		{
			list.Add(TargetType.Oneself);
		}
		if (a.isHumanoid)
		{
			list.Add(TargetType.Humanoid);
		}
		if (a.isPlayer)
		{
			list.Add(TargetType.Player);
		}
		if (a.isMicro)
		{
			list.Add(TargetType.Micro);
		}
		if (a.isGiantess)
		{
			list.Add(TargetType.Giantess);
		}
		return list;
	}

	private bool EntityMatch(List<TargetType> defs, EntityDef entityDef)
	{
		if (entityDef.include.Contains(TargetType.None))
		{
			return true;
		}
		for (int i = 0; i < defs.Count; i++)
		{
			for (int j = 0; j < entityDef.include.Count; j++)
			{
				TargetType targetType = entityDef.include[j];
				if (defs[i] == targetType)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsExcluded(List<TargetType> defs, EntityDef entityDef)
	{
		for (int i = 0; i < defs.Count; i++)
		{
			for (int j = 0; j < entityDef.exclude.Count; j++)
			{
				TargetType targetType = entityDef.exclude[j];
				if (defs[i] == targetType)
				{
					return true;
				}
			}
		}
		return false;
	}
}
