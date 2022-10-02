using UnityEngine;

namespace RootMotion.Demos
{
	public class BallShooter : MonoBehaviour
	{
		public KeyCode keyCode = KeyCode.Mouse0;

		public GameObject ball;

		public Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

		public Vector3 force = new Vector3(0f, 0f, 7f);

		public float mass = 3f;

		private void Update()
		{
			if (Input.GetKeyDown(keyCode))
			{
				Rigidbody component = Object.Instantiate(ball, base.transform.position + base.transform.rotation * spawnOffset, base.transform.rotation).GetComponent<Rigidbody>();
				if (component != null)
				{
					component.mass = mass;
					component.AddForce(Quaternion.LookRotation(Camera.main.ScreenPointToRay(Input.mousePosition).direction) * force, ForceMode.VelocityChange);
				}
			}
		}
	}
}
