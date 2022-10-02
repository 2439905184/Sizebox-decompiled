using UnityEngine;

public class AIRaygunProjectile : RaygunProjectile
{
	private EntityBase shooter;

	protected override void Start()
	{
		base.Start();
		audioSource.clip = SoundManager.Instance.GetNpcRaygunProjectileImpactSound();
		lifeTime = GlobalPreferences.AIProjectileLifetime.value;
		createImpactParticles = GlobalPreferences.AIProjectileImpactParticles.value;
		particlesScaleMult = GlobalPreferences.AIProjectileImpactParticlesSizeMult.value;
		SetupAILayerMask();
	}

	public void Initialize(EntityBase shooter, float projectileSpeedMultiplier, float scale)
	{
		Color color = new Color(GlobalPreferences.AIRaygunColorR.value, GlobalPreferences.AIRaygunColorG.value, GlobalPreferences.AIRaygunColorB.value);
		Initialize(shooter, projectileSpeedMultiplier, scale, color);
	}

	public void Initialize(EntityBase shooter, float projectileSpeedMultiplier, float scale, Color color)
	{
		this.shooter = shooter;
		baseSpeed = 20f * projectileSpeedMultiplier * scale;
		base.transform.localScale *= scale * 2.5f;
		SetColor(color);
	}

	protected override void Impact(GameObject hitObject)
	{
		if (hitObject != null && hitObject.layer != Layers.mapLayer)
		{
			EventManager.SendEvent(new AIRaygunHitEvent(shooter, hitObject.GetComponent<EntityBase>()));
		}
		base.Impact(hitObject);
	}
}
