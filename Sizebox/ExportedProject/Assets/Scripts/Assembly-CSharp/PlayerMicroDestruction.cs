using UnityEngine;

public class PlayerMicroDestruction : MonoBehaviour
{
	private Micro _micro;

	private BoolStored playerMicroDestruction;

	private void Awake()
	{
		_micro = GetComponentInParent<Micro>();
		if (!_micro)
		{
			Debug.LogError("PlayerMicroDestruction must be placed within a micro's hierarchy.");
			Object.Destroy(this);
		}
		playerMicroDestruction = GlobalPreferences.MicroPlayerBuildingDestruction;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!playerMicroDestruction.value)
		{
			return;
		}
		IDamagable componentInParent = collision.collider.GetComponentInParent<IDamagable>();
		if (componentInParent != null && (!(componentInParent is ICrushable) || !((ICrushable)componentInParent).TryToCrush(_micro.Rigidbody.mass, _micro.Rigidbody.velocity, collision, _micro)))
		{
			if (componentInParent is IDestructible)
			{
				((IDestructible)componentInParent).TryToDestroy(Sbox.CalculateDestructionForce(_micro), collision.GetContact(0).point, _micro);
			}
			componentInParent.Damage(Sbox.CalculateDamageDealt(_micro));
		}
	}
}
