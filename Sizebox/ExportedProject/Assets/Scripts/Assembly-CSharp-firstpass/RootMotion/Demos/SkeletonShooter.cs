using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class SkeletonShooter : MonoBehaviour
	{
		public MuscleRemoveMode removeMuscleMode;

		public LayerMask layers;

		public float unpin = 10f;

		public float force = 10f;

		public ParticleSystem particles;

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo = default(RaycastHit);
			if (!Physics.Raycast(ray, out hitInfo, 100f, layers))
			{
				return;
			}
			MuscleCollisionBroadcaster component = hitInfo.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
			if (component != null)
			{
				component.Hit(unpin, ray.direction * force, hitInfo.point);
				component.puppetMaster.RemoveMuscleRecursive(component.puppetMaster.muscles[component.muscleIndex].joint, true, true, removeMuscleMode);
			}
			else
			{
				ConfigurableJoint component2 = hitInfo.collider.attachedRigidbody.GetComponent<ConfigurableJoint>();
				if (component2 != null)
				{
					Object.Destroy(component2);
				}
				hitInfo.collider.attachedRigidbody.AddForceAtPosition(ray.direction * force, hitInfo.point);
			}
			particles.transform.position = hitInfo.point;
			particles.transform.rotation = Quaternion.LookRotation(-ray.direction);
			particles.Emit(5);
		}
	}
}
