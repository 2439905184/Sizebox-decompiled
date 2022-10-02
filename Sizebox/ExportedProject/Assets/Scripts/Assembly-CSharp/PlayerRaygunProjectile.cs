using UnityEngine;

public class PlayerRaygunProjectile : RaygunProjectile
{
	private float chargeVal;

	private float magnitude;

	private ResizeManager.Resizer resizer;

	protected override void Start()
	{
		base.Start();
		audioSource.clip = SoundManager.Instance.GetPlayerRaygunProjectileImpactSound();
		lifeTime = GlobalPreferences.PlayerProjectileLifetime.value;
		createImpactParticles = GlobalPreferences.PlayerProjectileImpactParticles.value;
		particlesScaleMult = GlobalPreferences.PlayerProjectileImpactParticlesSizeMult.value;
	}

	public void Initialize(float polarityMagnitude, float chargeMultiplier, Color color, LayerMask mask, float playerScale)
	{
		base.mask = mask;
		magnitude = polarityMagnitude;
		chargeVal = chargeMultiplier;
		baseSpeed = 20f * GlobalPreferences.PlayerProjectileSpeed.value * playerScale;
		mainColor = color;
		float duration = ((GlobalPreferences.ProjectileEffectMode.value == 0) ? GlobalPreferences.ProjectileSpurtDuration.value : 0.01f);
		float factor = 0.02f * polarityMagnitude * chargeMultiplier * GlobalPreferences.ProjectileEffectMultiplier.value;
		resizer = new ResizeManager.Resizer(duration, factor, false);
		SetColor(color, polarityMagnitude);
		base.transform.localScale *= chargeMultiplier * playerScale;
	}

	protected override void Impact(GameObject hitObject)
	{
		if (hitObject.layer != Layers.mapLayer)
		{
			if (!GlobalPreferences.RaygunScriptMode.value)
			{
				(hitObject.GetComponent<ResizeManager>() ?? hitObject.AddComponent<ResizeManager>()).AddResizer(resizer);
			}
			else
			{
				EventManager.SendEvent(new PlayerRaygunHitEvent(hitObject.GetComponent<EntityBase>(), magnitude, 0, chargeVal / 24f));
			}
		}
		base.Impact(hitObject);
	}
}
