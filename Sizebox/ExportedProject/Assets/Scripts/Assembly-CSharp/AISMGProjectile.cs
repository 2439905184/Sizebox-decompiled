using UnityEngine;

public class AISMGProjectile : SMGProjectile
{
	private EntityBase shooter;

	protected override void Start()
	{
		base.Start();
		audioSource.clip = SoundManager.Instance.GetNpcSmgProjectileImpactSound();
		lifeTime = GlobalPreferences.AIProjectileLifetime.value;
		SetupAILayerMask();
		Color color = new Color(GlobalPreferences.AiSmgColorR.value, GlobalPreferences.AiSmgColorG.value, GlobalPreferences.AiSmgColorB.value);
		SetColor(color);
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
		base.transform.localScale *= scale;
		SetColor(color);
	}

	protected override void Impact(GameObject hitObject)
	{
		if (hitObject != null && hitObject.layer != Layers.mapLayer)
		{
			EventManager.SendEvent(new AISMGHitEvent(shooter, hitObject.GetComponent<EntityBase>()));
		}
		base.Impact(hitObject);
	}
}
