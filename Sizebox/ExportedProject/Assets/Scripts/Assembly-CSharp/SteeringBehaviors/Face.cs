using UnityEngine;

namespace SteeringBehaviors
{
	public class Face : Align
	{
		public Face(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			Vector3 vector = target.position - agent.transform.position;
			if (vector.magnitude > 0f)
			{
				targetOrientation = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (Mathf.Abs(MapToRange(agent.orientation - targetOrientation)) < targetRadius)
				{
					agent.Stop();
				}
			}
			return base.GetSteering(out steeringOutput);
		}
	}
}
