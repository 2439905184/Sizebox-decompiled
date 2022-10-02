using UnityEngine;

public class AISMG : AIGun
{
	[SerializeField]
	private ParticleSystem muzzleFlash;

	public override int AnimGunType
	{
		get
		{
			return 2;
		}
	}

	public override int AnimLayer
	{
		get
		{
			return 2;
		}
	}

	public override string AnimTransitionName
	{
		get
		{
			return "SMG Aim Layer.Idle -> SMG Aim Layer.Aiming";
		}
	}

	protected override void Start()
	{
		defaultFiringAudioClip = SoundManager.Instance.npcSmgProjFireSound;
		base.Start();
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
		AISMGProjectile component = gameObject.GetComponent<AISMGProjectile>();
		projectileScaleMult = 2.5f;
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
		muzzleFlash.Emit(10);
		firingAudioSource.Play();
	}
}
