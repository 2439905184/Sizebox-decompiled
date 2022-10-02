using UnityEngine;

namespace RootMotion.Demos
{
	public class Planet : MonoBehaviour
	{
		public float mass = 1000f;

		public Rigidbody[] rigidbodies;

		private const float G = 6.672E-11f;

		private void Start()
		{
			rigidbodies = Object.FindObjectsOfType<Rigidbody>();
			Rigidbody[] array = rigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].useGravity = false;
			}
		}

		private void FixedUpdate()
		{
			Rigidbody[] array = rigidbodies;
			foreach (Rigidbody rigidbody in array)
			{
				if (!rigidbody.isKinematic)
				{
					ApplyGravity(rigidbody);
				}
			}
		}

		private void ApplyGravity(Rigidbody r)
		{
			Vector3 vector = base.transform.position - r.position;
			float sqrMagnitude = vector.sqrMagnitude;
			float num = Mathf.Sqrt(sqrMagnitude);
			r.velocity += vector / num * 6.672E-11f * (mass / sqrMagnitude) * Time.fixedDeltaTime;
		}
	}
}
