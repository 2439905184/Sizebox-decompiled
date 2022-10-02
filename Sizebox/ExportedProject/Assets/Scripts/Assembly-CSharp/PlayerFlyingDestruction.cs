using UnityEngine;

public class PlayerFlyingDestruction : MonoBehaviour
{
	[SerializeField]
	private float multiplier = 1f;

	[SerializeField]
	private float damageOffset = 25f;

	[SerializeField]
	private float punchRadius = 0.25f;

	[SerializeField]
	private float punchForwardLength = 0.2f;

	private Micro micro;

	private NewPlayerMicroController microController;

	private RaycastHit[] punchBuffer = new RaycastHit[200];

	private void Awake()
	{
		micro = GetComponent<Micro>();
	}

	public void AssignController(NewPlayerMicroController microController)
	{
		this.microController = microController;
	}

	public void SetDestructionForceMultiplier(float multiplier)
	{
		this.multiplier = multiplier;
	}

	public void Punch(float power, Vector3 handPosition, Vector3 forward)
	{
		Vector3 vector = handPosition - forward;
		IDamagable damagable = micro;
		int num = Physics.SphereCastNonAlloc(handPosition + forward * punchRadius * 0.75f, punchRadius, forward, punchBuffer, punchForwardLength);
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = punchBuffer[i];
			IDamagable componentInParent = raycastHit.collider.GetComponentInParent<IDamagable>();
			IDestructible destructible = componentInParent as IDestructible;
			if (destructible != null)
			{
				float num2 = microController.Velocity.magnitude + 1f;
				destructible.TryToDestroy(micro.AccurateScale * num2 * multiplier, raycastHit.point, micro);
				continue;
			}
			if (componentInParent != null && componentInParent != damagable)
			{
				float num3 = microController.Velocity.magnitude + 1f;
				componentInParent.Damage(micro.AccurateScale * num3 * multiplier - damageOffset);
			}
			Rigidbody componentInParent2 = raycastHit.collider.GetComponentInParent<Rigidbody>();
			if ((bool)componentInParent2 && componentInParent2 != micro.Rigidbody)
			{
				componentInParent2.AddForceAtPosition(power * (componentInParent2.position - vector), raycastHit.point, ForceMode.Impulse);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		IDamagable componentInParent = collision.collider.GetComponentInParent<IDamagable>();
		ICrushable crushable = componentInParent as ICrushable;
		IDestructible destructible = componentInParent as IDestructible;
		if (destructible != null)
		{
			ContactPoint contact = collision.GetContact(0);
			float magnitude = microController.Velocity.magnitude;
			float num = 1f;
			if (magnitude > micro.AccurateScale * 20f)
			{
				float num2 = Vector3.Angle(contact.normal, -microController.Velocity);
				num = Mathf.Clamp01((90f - num2) / 90f);
			}
			destructible.TryToDestroy(micro.AccurateScale * magnitude * num * multiplier, contact.point, micro);
		}
		else if (crushable != null)
		{
			crushable.TryToCrush(micro.Rigidbody.mass, microController.Velocity, collision, micro);
		}
		else if (componentInParent != null)
		{
			float magnitude2 = microController.Velocity.magnitude;
			float num3 = micro.AccurateScale * magnitude2 * multiplier - damageOffset;
			if (num3 > 0f)
			{
				destructible.Damage(num3);
			}
		}
	}
}
