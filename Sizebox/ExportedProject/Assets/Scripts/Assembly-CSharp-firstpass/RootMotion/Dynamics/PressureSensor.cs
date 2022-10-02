using UnityEngine;

namespace RootMotion.Dynamics
{
	public class PressureSensor : MonoBehaviour
	{
		public bool visualize;

		public LayerMask layers;

		private bool fixedFrame;

		private Vector3 P;

		private int count;

		public Vector3 center { get; private set; }

		public bool inContact { get; private set; }

		public Vector3 bottom { get; private set; }

		public Rigidbody r { get; private set; }

		private void Awake()
		{
			r = GetComponent<Rigidbody>();
			center = base.transform.position;
		}

		private void OnCollisionEnter(Collision c)
		{
			ProcessCollision(c);
		}

		private void OnCollisionStay(Collision c)
		{
			ProcessCollision(c);
		}

		private void OnCollisionExit(Collision c)
		{
			inContact = false;
		}

		private void FixedUpdate()
		{
			fixedFrame = true;
			if (!r.IsSleeping())
			{
				P = Vector3.zero;
				count = 0;
			}
		}

		private void LateUpdate()
		{
			if (fixedFrame)
			{
				if (count > 0)
				{
					center = P / count;
				}
				fixedFrame = false;
			}
		}

		private void ProcessCollision(Collision c)
		{
			if (LayerMaskExtensions.Contains(layers, c.gameObject.layer))
			{
				Vector3 zero = Vector3.zero;
				for (int i = 0; i < c.contacts.Length; i++)
				{
					zero += c.contacts[i].point;
				}
				zero /= (float)c.contacts.Length;
				P += zero;
				count++;
				inContact = true;
			}
		}

		private void OnDrawGizmos()
		{
			if (visualize)
			{
				Gizmos.DrawSphere(center, 0.1f);
			}
		}
	}
}
