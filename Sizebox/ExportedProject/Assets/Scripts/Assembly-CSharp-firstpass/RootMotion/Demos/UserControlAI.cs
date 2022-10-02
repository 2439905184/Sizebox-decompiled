using UnityEngine;

namespace RootMotion.Demos
{
	public class UserControlAI : UserControlThirdPerson
	{
		public Transform moveTarget;

		public float stoppingDistance = 0.5f;

		public float stoppingThreshold = 1.5f;

		protected override void Update()
		{
			float num = (walkByDefault ? 0.5f : 1f);
			Vector3 tangent = moveTarget.position - base.transform.position;
			float magnitude = tangent.magnitude;
			Vector3 normal = base.transform.up;
			Vector3.OrthoNormalize(ref normal, ref tangent);
			float num2 = ((state.move != Vector3.zero) ? stoppingDistance : (stoppingDistance * stoppingThreshold));
			state.move = ((magnitude > num2) ? (tangent * num) : Vector3.zero);
		}
	}
}
