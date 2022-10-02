using System.Collections.Generic;
using AI;
using UnityEngine;

public abstract class DecisionMaker
{
	protected readonly AIController AI;

	protected readonly Humanoid Entity;

	protected DecisionMaker(Humanoid entity)
	{
		Entity = entity;
		AI = entity.ai;
	}

	public abstract void Execute();

	protected LuaBehavior ChooseBehavior(List<IBehavior> behaviors)
	{
		if (behaviors == null || behaviors.Count == 0)
		{
			return null;
		}
		Dictionary<LuaBehavior, float> dictionary = new Dictionary<LuaBehavior, float>();
		float num = 0f;
		foreach (LuaBehavior behavior in behaviors)
		{
			float num2 = 0f;
			if (behavior.Scores != null)
			{
				foreach (BehaviorScore score in behavior.Scores)
				{
					num2 += EvaluateScore(score);
				}
			}
			num2 = Mathf.Max(num2, 0f);
			dictionary.Add(behavior, num2);
			num += num2;
		}
		if (num == 0f)
		{
			return (LuaBehavior)behaviors[Random.Range(0, behaviors.Count)];
		}
		float num3 = Random.value * num;
		float num4 = 0f;
		foreach (LuaBehavior behavior2 in behaviors)
		{
			num4 += dictionary[behavior2];
			if (num4 > num3)
			{
				return behavior2;
			}
		}
		return null;
	}

	private float EvaluateScore(BehaviorScore score)
	{
		float num = 1f;
		switch (score.Type)
		{
		case ScoreType.Normal:
			num = AI.mentalState.normal;
			break;
		case ScoreType.Afraid:
			num = AI.mentalState.fear;
			break;
		case ScoreType.Curious:
			num = AI.mentalState.curiosity;
			break;
		case ScoreType.Hostile:
			num = AI.mentalState.hostile;
			break;
		}
		return num * score.Value;
	}

	protected Vector3 GetRandomPoint()
	{
		Vector2 vector = Random.insideUnitCircle * (Entity.Scale * 20f);
		return Entity.transform.position + new Vector3(vector.x, 0f, vector.y);
	}
}
