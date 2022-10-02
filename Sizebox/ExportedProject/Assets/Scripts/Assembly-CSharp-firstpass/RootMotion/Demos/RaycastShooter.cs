using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class RaycastShooter : MonoBehaviour
	{
		public LayerMask layers;

		public float unpin = 10f;

		public float force = 10f;

		public ParticleSystem blood;

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo, 100f, layers))
			{
				MuscleCollisionBroadcaster component = hitInfo.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
				if (component != null)
				{
					component.Hit(unpin, ray.direction * force, hitInfo.point);
					blood.transform.position = hitInfo.point;
					blood.transform.rotation = Quaternion.LookRotation(-ray.direction);
					blood.Emit(5);
				}
			}
		}
	}
}
