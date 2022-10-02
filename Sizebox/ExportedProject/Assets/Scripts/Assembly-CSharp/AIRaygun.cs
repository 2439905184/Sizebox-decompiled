using UnityEngine;

public class AIRaygun : AIGun
{
	private Gradient shotEmitterParticlesGradient = new Gradient();

	[SerializeField]
	private ParticleSystem shotEmitterParticles;

	public override int AnimGunType
	{
		get
		{
			return 1;
		}
	}

	public override int AnimLayer
	{
		get
		{
			return 1;
		}
	}

	public override string AnimTransitionName
	{
		get
		{
			return "Raygun Aim Layer.Idle -> Raygun Aim Layer.Aiming";
		}
	}

	protected override void Start()
	{
		defaultFiringAudioClip = SoundManager.Instance.npcRaygunProjFireSound;
		base.Start();
		SetupParticlesForSingleFire();
	}

	public override void SetColor(int r, int g, int b)
	{
		base.SetColor(r, g, b);
		SetupParticlesForSingleFire();
	}

	private void UpdateGradient()
	{
		Color col = ((!(projectileColor == Color.clear)) ? projectileColor : new Color(GlobalPreferences.AIRaygunColorR.value, GlobalPreferences.AIRaygunColorG.value, GlobalPreferences.AIRaygunColorB.value));
		shotEmitterParticlesGradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(col, 0f),
			new GradientColorKey(col, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(0f, 0f),
			new GradientAlphaKey(1f, 0.5f),
			new GradientAlphaKey(0f, 1f)
		});
	}

	private void SetupParticlesForSingleFire()
	{
		UpdateGradient();
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = shotEmitterParticles.colorOverLifetime;
		colorOverLifetime.color = shotEmitterParticlesGradient;
	}

	public override void Fire()
	{
		Fire(0f);
	}

	public override void Fire(float inaccurracyFactor)
	{
		float x = owner.transform.localScale.x;
		Vector3 position = firingPoint.position + firingPoint.forward * x;
		GameObject gameObject = Object.Instantiate(projectilePrefab, position, base.transform.rotation);
		if (inaccurracyFactor > 0f)
		{
			gameObject.transform.Rotate(Random.Range(0f, inaccurracyFactor), Random.Range(0f, inaccurracyFactor), 0f);
		}
		AIRaygunProjectile component = gameObject.GetComponent<AIRaygunProjectile>();
		if (projectileColor != Color.clear)
		{
			component.Initialize(owner.GetComponent<EntityBase>(), projectileSpeedMult, x * projectileScaleMult, projectileColor);
		}
		else
		{
			component.Initialize(owner.GetComponent<EntityBase>(), projectileSpeedMult, x * projectileScaleMult);
		}
		if (customProjectileImpactAudioClip != null)
		{
			component.SetSoundClip(customProjectileImpactAudioClip);
		}
		shotEmitterParticles.transform.localScale = Vector3.one * x;
		shotEmitterParticles.Emit(20);
		firingAudioSource.Play();
	}
}
