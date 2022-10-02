using SteeringBehaviors;
using UnityEngine;

public class CustomSteer : SteerBehavior
{
	public CustomSteer(MovementCharacter agent, Kinematic target)
		: base(agent, target)
	{
		weight = 1f;
	}

	public override bool GetSteering(out SteeringOutput steeringOutput)
	{
		pSteering.linear.y = 0f;
		pSteering.linear.Normalize();
		float maxAccel = agent.MaxAccel;
		pSteering.linear.x *= maxAccel;
		pSteering.linear.y *= maxAccel;
		pSteering.linear.z *= maxAccel;
		steeringOutput = pSteering;
		return true;
	}

	public void SetLinearSteering(Vector3 direction)
	{
		pSteering.linear = direction;
	}
}
