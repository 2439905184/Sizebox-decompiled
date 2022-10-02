using UnityEngine;

public abstract class SMGProjectile : Projectile
{
	[SerializeField]
	private TrailRenderer trail;

	[SerializeField]
	private Renderer projectileModelRenderer;

	protected override void Start()
	{
		base.Start();
		trail.startWidth *= base.transform.localScale.x;
		trail.endWidth *= base.transform.localScale.x;
		trail.time *= base.transform.localScale.x;
	}

	private void Update()
	{
		if (alive)
		{
			base.transform.position += base.transform.forward * baseSpeed * Time.deltaTime;
			lifeTime -= Time.deltaTime;
			if (lifeTime < 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	protected override void Impact(GameObject hitObject)
	{
		alive = false;
		audioSource.Play();
		foreach (Transform item in base.gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
		base.gameObject.GetComponent<CapsuleCollider>().enabled = false;
		base.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
		projectileModelRenderer.enabled = false;
		Object.Destroy(base.gameObject, 3f);
	}

	protected override void SetColor(Color color, float emissionModifier = 1f)
	{
		mainColor = color;
		Vector4 vector = new Vector4(color.r, color.g, color.b, 0f) * (Mathf.Log(Mathf.Abs(emissionModifier) + 1.3f, 2.3f) * 2f);
		projectileModelRenderer.material.color = color;
		Material material = trail.material;
		material.SetColor("_Color", color);
		material.SetColor("_EmissionColor", vector);
	}
}
