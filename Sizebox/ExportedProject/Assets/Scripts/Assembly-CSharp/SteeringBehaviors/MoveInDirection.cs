namespace SteeringBehaviors
{
	public class MoveInDirection : SteerBehavior
	{
		public MoveInDirection(MovementCharacter agent, VectorKinematic target)
			: base(agent, target)
		{
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			weight = 0f;
			pSteering.overrideUpdateReset = true;
			pSteering.linear = target.position;
			pSteering.linear.y = 0f;
			pSteering.linear.Normalize();
			float maxAccel = agent.MaxAccel;
			pSteering.linear.x *= maxAccel;
			pSteering.linear.y *= maxAccel;
			pSteering.linear.z *= maxAccel;
			steeringOutput = pSteering;
			return true;
		}
	}
}
