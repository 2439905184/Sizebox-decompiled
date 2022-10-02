namespace SteeringBehaviors
{
	public class Align : SteerBehavior
	{
		protected float targetRadius = 2f;

		private float slowRadius = 5f;

		private float timeToTarget = 0.1f;

		protected float targetOrientation;

		protected Align(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			float rotation = targetOrientation - agent.orientation;
			rotation = MapToRange(rotation);
			float num = rotation;
			if (rotation < 0f)
			{
				num = 0f - rotation;
			}
			if (num < targetRadius)
			{
				pSteering.angular = 0f;
				steeringOutput = pSteering;
				return true;
			}
			float num2 = ((!(num > slowRadius)) ? (agent.MaxRotation * num / slowRadius) : agent.MaxRotation);
			num2 *= rotation / num;
			pSteering.angular = num2 - agent.rotation;
			pSteering.angular /= timeToTarget;
			float num3 = pSteering.angular;
			if (pSteering.angular < 0f)
			{
				num3 = 0f - pSteering.angular;
			}
			float maxAngularAccel = agent.MaxAngularAccel;
			if (num3 > maxAngularAccel)
			{
				pSteering.angular /= num3;
				pSteering.angular *= maxAngularAccel;
			}
			steeringOutput = pSteering;
			return true;
		}
	}
}
