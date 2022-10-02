using System;

public class BehaviorScore
{
	public readonly ScoreType Type;

	public readonly float Value;

	public BehaviorScore(string t, float val)
	{
		Type = (ScoreType)Enum.Parse(typeof(ScoreType), t);
		Value = val;
	}
}
