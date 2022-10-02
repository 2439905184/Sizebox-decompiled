using UnityEngine;

namespace RootMotion.Demos
{
	public class PuppetBoard : MonoBehaviour
	{
		[Tooltip("Board target Rigidbody.")]
		public Rigidbody target;

		[Tooltip("Pivot Transform of the body target.")]
		public Transform bodyTargetPivot;

		[Tooltip("The body target keeps the puppet upright by a SpringJoint connected to the body.")]
		public Transform bodyTarget;

		private Rigidbody r;

		private void Start()
		{
			r = GetComponent<Rigidbody>();
			Physics.IgnoreLayerCollision(base.gameObject.layer, target.gameObject.layer, true);
		}

		private void FixedUpdate()
		{
			r.MovePosition(target.position);
			r.MoveRotation(target.rotation);
			r.velocity = target.velocity;
			r.angularVelocity = target.angularVelocity;
			Quaternion quaternion = Quaternion.FromToRotation(bodyTarget.position - base.transform.position, Vector3.up);
			bodyTargetPivot.rotation = quaternion * bodyTarget.rotation;
		}
	}
}
