using UnityEngine;

namespace Assets.Scripts.ProceduralCityGenerator
{
	[RequireComponent(typeof(Rigidbody))]
	public class DebrisHandler : MonoBehaviour
	{
		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (GlobalPreferences.DebrisCanCrush.value)
			{
				ICrushable component = collision.gameObject.GetComponent<ICrushable>();
				if (component != null && !(_rigidbody.velocity.y > 0f))
				{
					component.TryToCrush(_rigidbody.mass, _rigidbody.velocity, collision);
				}
			}
		}
	}
}
