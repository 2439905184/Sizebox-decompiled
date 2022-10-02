using UnityEngine;

namespace SteeringBehaviors
{
	public class SteeringOutput
	{
		public float angular;

		public Vector3 linear;

		public bool overrideUpdateReset;

		public SteeringOutput()
		{
			angular = 0f;
			linear = default(Vector3);
		}

		public void Reset()
		{
			angular = 0f;
			linear = Vector3.zero;
		}
	}
}
