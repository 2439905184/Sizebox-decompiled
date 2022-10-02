using UnityEngine;

namespace SteeringBehaviors
{
	public class VectorKinematic : Kinematic
	{
		private Vector3 _virtualPosition;

		public VectorKinematic(Vector3 position)
		{
			_virtualPosition = CenterOrigin.WorldToVirtual(position);
		}

		protected override Vector3 GetPosition()
		{
			return CenterOrigin.VirtualToWorld(_virtualPosition);
		}

		protected override void SetPosition(Vector3 newPosition)
		{
			_virtualPosition = CenterOrigin.WorldToVirtual(newPosition);
		}

		public override bool TryGetPosition(out Vector3 pos)
		{
			pos = GetPosition();
			return true;
		}
	}
}
