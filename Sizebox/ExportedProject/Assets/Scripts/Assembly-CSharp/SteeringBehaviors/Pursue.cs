using UnityEngine;

namespace SteeringBehaviors
{
	public class Pursue : Seek
	{
		public float maxPrediction = 2f;

		private readonly Kinematic _targetAux;

		public Pursue(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
			_targetAux = target;
			_targetAux = target;
			target = new VectorKinematic(Vector3.zero);
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			float magnitude = (_targetAux.position - agent.transform.position).magnitude;
			float magnitude2 = agent.velocity.magnitude;
			target.position = _targetAux.position;
			return base.GetSteering(out steeringOutput);
		}

		private Vector3 VectorMultScalar(Vector3 vector, float f)
		{
			vector.x *= f;
			vector.y *= f;
			vector.z *= f;
			return vector;
		}
	}
}
