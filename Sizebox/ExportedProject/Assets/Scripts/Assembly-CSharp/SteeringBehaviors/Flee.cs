namespace SteeringBehaviors
{
	public class Flee : SteerBehavior
	{
		public Flee(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			if ((bool)target)
			{
				pSteering.linear = agent.transform.position - target.position;
				pSteering.linear.y = 0f;
				pSteering.linear.Normalize();
				pSteering.linear *= agent.MaxAccel;
				steeringOutput = pSteering;
				return true;
			}
			pSteering.Reset();
			steeringOutput = null;
			return false;
		}
	}
}
