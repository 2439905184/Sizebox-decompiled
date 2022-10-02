using UnityEngine;

public abstract class RaygunProjectile : Projectile
{
	[SerializeField]
	private GameObject impactParticlesTemplate;

	[SerializeField]
	private TrailRenderer trail;

	[SerializeField]
	private Renderer projectileModelRenderer;

	protected bool createImpactParticles;

	protected float particlesScaleMult;

	private const int impactParticleCount = 40;

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
				Explode(0.25f);
			}
		}
	}

	protected override void Impact(GameObject hitObject)
	{
		float scaleMult = 0.25f;
		if (hitObject.GetComponent<Giantess>() != null)
		{
			scaleMult = 10f * Mathf.Log(hitObject.transform.localScale.x + 1f, 2f);
		}
		Explode(scaleMult);
	}

	protected void Explode(float scaleMult)
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
		if (createImpactParticles)
		{
			float scale = scaleMult * 5f * particlesScaleMult;
			CreateImpactParticles(scale);
		}
		Object.Destroy(base.gameObject, 3f);
	}

	private void CreateImpactParticles(float scale)
	{
		GameObject gameObject = Object.Instantiate(impactParticlesTemplate, base.transform.position, Quaternion.identity);
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(mainColor, 0f),
			new GradientColorKey(mainColor, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(0f, 0f),
			new GradientAlphaKey(0.9f, 0.5f),
			new GradientAlphaKey(0f, 1f)
		});
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = component.colorOverLifetime;
		colorOverLifetime.color = gradient;
		ParticleSystem.MinMaxCurve size = component.sizeOverLifetime.size;
		size.constantMin *= scale;
		size.constantMax *= scale;
		component.Emit(40);
		Object.Destroy(gameObject, component.main.duration);
	}

	protected override void SetColor(Color color, float emissionModifier = 1f)
	{
		mainColor = color;
		Vector4 vector = new Vector4(color.r, color.g, color.b, 0f) * (Mathf.Log(Mathf.Abs(emissionModifier) + 1.3f, 2.3f) * 2f);
		Material material = projectileModelRenderer.material;
		material.color = color;
		material.SetColor("_EmissionColor", vector);
		Material material2 = trail.material;
		material2.SetColor("_Color", color);
		material2.SetColor("_EmissionColor", vector);
	}
}
