using UnityEngine;

namespace SteeringBehaviors
{
	public class Arrive : SteerBehavior
	{
		private float timeToTarget = 0.1f;

		private float targetRadius
		{
			get
			{
				return agent.scale * 0.2f;
			}
		}

		private float slowRadius
		{
			get
			{
				return agent.scale * 0.5f;
			}
		}

		public Arrive(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			SteeringOutput steeringOutput2 = new SteeringOutput();
			Vector3 vector = target.position - agent.transform.position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude < targetRadius)
			{
				agent.Stop();
				steeringOutput = steeringOutput2;
				return true;
			}
			float num = ((!(magnitude > slowRadius)) ? (agent.MaxSpeed * magnitude / slowRadius) : agent.MaxSpeed);
			Vector3 vector2 = vector;
			vector2.Normalize();
			vector2 *= num;
			steeringOutput2.linear = vector2 - agent.velocity;
			steeringOutput2.linear /= timeToTarget;
			if (steeringOutput2.linear.magnitude > agent.MaxAccel)
			{
				steeringOutput2.linear.Normalize();
				steeringOutput2.linear *= agent.MaxAccel;
			}
			steeringOutput = steeringOutput2;
			return true;
		}
	}
}
