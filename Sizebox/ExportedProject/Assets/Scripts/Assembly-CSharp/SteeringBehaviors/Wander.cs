using UnityEngine;

namespace SteeringBehaviors
{
	public class Wander : Seek
	{
		private float offset = 2f;

		private float radius = 1f;

		private float rate = 30f;

		private float _wanderOrientation;

		private float Offset
		{
			get
			{
				return offset * agent.scale;
			}
		}

		private float Radius
		{
			get
			{
				return radius * agent.scale;
			}
		}

		public Wander(MovementCharacter agent)
			: base(agent, null)
		{
			target = new VectorKinematic(agent.transform.position);
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			_wanderOrientation += RandomBinomial() * rate;
			float orientation = _wanderOrientation + agent.orientation;
			Vector3 vector = OrientationToVector(agent.orientation);
			Vector3 position = Offset * vector + agent.transform.position;
			position += OrientationToVector(orientation) * Radius;
			target.position = position;
			return base.GetSteering(out steeringOutput);
		}
	}
}
