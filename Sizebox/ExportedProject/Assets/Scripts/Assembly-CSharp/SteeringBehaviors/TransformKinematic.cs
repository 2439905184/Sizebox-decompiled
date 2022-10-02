using UnityEngine;

namespace SteeringBehaviors
{
	public class TransformKinematic : Kinematic
	{
		public readonly Transform transform;

		public TransformKinematic(Transform transform)
		{
			this.transform = transform;
		}

		protected override Vector3 GetPosition()
		{
			return transform.position;
		}

		public override bool TryGetPosition(out Vector3 pos)
		{
			if ((bool)transform)
			{
				pos = transform.position;
				return true;
			}
			pos = Vector3.zero;
			return false;
		}

		protected override bool ToBool()
		{
			return transform;
		}
	}
}
