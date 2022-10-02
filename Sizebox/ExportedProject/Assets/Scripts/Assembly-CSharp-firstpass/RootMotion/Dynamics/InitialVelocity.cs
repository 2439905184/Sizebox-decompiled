using UnityEngine;

namespace RootMotion.Dynamics
{
	public class InitialVelocity : MonoBehaviour
	{
		public Vector3 initialVelocity;

		private void Start()
		{
			GetComponent<Rigidbody>().velocity = initialVelocity;
		}
	}
}
