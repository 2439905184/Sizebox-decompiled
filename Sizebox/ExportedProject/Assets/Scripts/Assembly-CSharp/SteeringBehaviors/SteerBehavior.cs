using System;
using UnityEngine;

namespace SteeringBehaviors
{
	public class SteerBehavior
	{
		public Kinematic target;

		public readonly MovementCharacter agent;

		public float weight = 1f;

		protected readonly SteeringOutput pSteering;

		protected SteerBehavior(MovementCharacter agent, Kinematic target)
		{
			this.agent = agent;
			this.target = target;
			pSteering = new SteeringOutput();
		}

		~SteerBehavior()
		{
			if ((bool)agent)
			{
				MovementCharacter component = agent.GetComponent<MovementCharacter>();
				if ((bool)component)
				{
					component.RemoveBehaviour(this);
				}
			}
		}

		public virtual bool GetSteering(out SteeringOutput steeringOutput)
		{
			steeringOutput = pSteering;
			return pSteering != null;
		}

		protected float MapToRange(float rotation)
		{
			rotation %= 360f;
			if (rotation > 180f)
			{
				rotation -= 360f;
			}
			else if (rotation < -180f)
			{
				rotation += 360f;
			}
			return rotation;
		}

		protected Vector3 OrientationToVector(float orientation)
		{
			Vector3 result = default(Vector3);
			result.x = Mathf.Sin(orientation * ((float)Math.PI / 180f));
			result.z = Mathf.Cos(orientation * ((float)Math.PI / 180f));
			return result;
		}

		protected float RandomBinomial()
		{
			return UnityEngine.Random.Range(0f, 1f) - UnityEngine.Random.Range(0f, 1f);
		}
	}
}
