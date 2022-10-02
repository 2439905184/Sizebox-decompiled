using UnityEngine;

namespace SteeringBehaviors
{
	public class LookWhereYouAreGoing : Align
	{
		private readonly SteeringOutput _zeroSteering;

		public LookWhereYouAreGoing(MovementCharacter agent)
			: base(agent, null)
		{
			_zeroSteering = new SteeringOutput();
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			if (agent.velocity.sqrMagnitude == 0f)
			{
				steeringOutput = _zeroSteering;
				return true;
			}
			targetOrientation = Mathf.Atan2(agent.velocity.x, agent.velocity.z) * 57.29578f;
			return base.GetSteering(out steeringOutput);
		}
	}
}
