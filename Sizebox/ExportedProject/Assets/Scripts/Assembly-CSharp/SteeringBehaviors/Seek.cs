using UnityEngine;

namespace SteeringBehaviors
{
	public class Seek : SteerBehavior
	{
		private readonly float _startTime;

		private readonly float _separationLimit;

		private readonly float _duration;

		public Seek(MovementCharacter agent, Kinematic target, float separation = 0f, float duration = 0f)
			: base(agent, target)
		{
			weight = 1.5f;
			_startTime = Time.time;
			_separationLimit = separation;
			_duration = duration;
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			TransformKinematic transformKinematic = target as TransformKinematic;
			if ((bool)transformKinematic)
			{
				Transform transform = ((transformKinematic != null) ? transformKinematic.transform : null);
				if (!transform)
				{
					steeringOutput = pSteering;
					return false;
				}
				Micro component = transform.GetComponent<Micro>();
				if (!component || component.isCrushed)
				{
					agent.Stop();
					steeringOutput = pSteering;
					return true;
				}
			}
			Vector3 position = agent.transform.position;
			Vector3 pos;
			if (!target.TryGetPosition(out pos))
			{
				agent.Stop();
				steeringOutput = pSteering;
				return true;
			}
			pSteering.linear.x = pos.x - position.x;
			pSteering.linear.y = 0f;
			pSteering.linear.z = pos.z - position.z;
			pSteering.linear.Normalize();
			float maxAccel = agent.MaxAccel;
			pSteering.linear.x *= maxAccel;
			pSteering.linear.y *= maxAccel;
			pSteering.linear.z *= maxAccel;
			TransformKinematic transformKinematic2;
			if (_duration > 0f && Time.time > _startTime + _duration)
			{
				agent.Stop();
			}
			else if (_separationLimit > -1f && (transformKinematic2 = target as TransformKinematic) != null)
			{
				Vector3 vector = transformKinematic2.position - agent.transform.position;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				GameObject gameObject = transformKinematic2.transform.gameObject;
				CapsuleCollider component2 = gameObject.GetComponent<CapsuleCollider>();
				if (!component2)
				{
					component2 = gameObject.GetComponent<Giantess>().gtsMovement.GetComponent<CapsuleCollider>();
				}
				CapsuleCollider component3 = agent.gameObject.GetComponent<CapsuleCollider>();
				float a = component3.radius * component3.transform.lossyScale.y;
				float b = component2.radius * component2.transform.lossyScale.y;
				float num = Mathf.Max(a, b);
				float num2 = Mathf.Min(a, b);
				float num3 = num;
				if (magnitude < num + num2)
				{
					num3 += num2;
				}
				if ((magnitude - num - num2) / num3 < _separationLimit)
				{
					agent.Stop();
				}
			}
			steeringOutput = pSteering;
			return true;
		}
	}
}
