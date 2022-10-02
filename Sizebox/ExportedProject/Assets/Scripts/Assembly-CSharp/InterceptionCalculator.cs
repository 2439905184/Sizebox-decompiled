using System.Collections.Generic;
using UnityEngine;

internal class InterceptionCalculator
{
	private class TimePosition
	{
		public float time;

		public Vector2 position;
	}

	[SerializeField]
	private const float SecondsAhead = 5f;

	[SerializeField]
	private const float SecondsRate = 1f;

	public static Vector2 CalculateInterceptionCourse(Vector2 target, Vector2 targetSpeed, Vector2 self, Vector2 selfSpeed)
	{
		List<TimePosition> list = new List<TimePosition>();
		for (float num = 0f; num < 5f; num += 1f)
		{
			TimePosition timePosition = new TimePosition();
			Vector2 position = target + targetSpeed * num;
			timePosition.time = num;
			timePosition.position = position;
			list.Add(timePosition);
		}
		int index = 0;
		float num2 = float.MaxValue;
		float num3 = num2;
		for (int i = 0; i < list.Count; i++)
		{
			TimePosition timePosition2 = list[i];
			for (float num4 = 0f; num4 < 5f; num4 += 1f)
			{
				float num5 = Mathf.Abs(Vector2.Distance(self, timePosition2.position) / 1f - timePosition2.time);
				if (num5 < num2)
				{
					num2 = (num3 = num5);
					index = i;
				}
				else if (num5 > num3)
				{
					return list[index].position;
				}
			}
		}
		return list[index].position;
	}
}
