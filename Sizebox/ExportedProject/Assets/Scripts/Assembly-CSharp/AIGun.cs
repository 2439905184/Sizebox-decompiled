using UnityEngine;

public abstract class AIGun : AIWeapon
{
	protected Color projectileColor;

	[SerializeField]
	protected Transform firingPoint;

	[SerializeField]
	protected GameObject projectilePrefab;

	protected float projectileScaleMult = 1f;

	protected float projectileSpeedMult;

	protected AudioClip defaultFiringAudioClip;

	protected AudioClip customFiringAudioClip;

	protected AudioClip customProjectileImpactAudioClip;

	[SerializeField]
	protected AudioSource firingAudioSource;

	public virtual int AnimGunType
	{
		get
		{
			return -1;
		}
	}

	public virtual int AnimLayer
	{
		get
		{
			return -1;
		}
	}

	public virtual string AnimTransitionName
	{
		get
		{
			return "";
		}
	}

	protected virtual void Start()
	{
		firingAudioSource.clip = defaultFiringAudioClip;
		projectileSpeedMult = GlobalPreferences.AIProjectileSpeed.value;
	}

	public virtual void SetColor(int r, int g, int b)
	{
		projectileColor = new Color(Mathf.Clamp01((float)r / 255f), Mathf.Clamp01((float)g / 255f), Mathf.Clamp01((float)b / 255f));
	}

	public void SetProjectileScale(float scale)
	{
		projectileScaleMult = scale;
	}

	public void SetProjectileSpeed(float speedMult)
	{
		projectileSpeedMult = speedMult;
	}

	public virtual void Fire()
	{
	}

	public virtual void Fire(float inaccuraccyFactor)
	{
	}

	public void PointAt(Transform target)
	{
		base.transform.LookAt(target);
	}

	public void PointAt(Vector3 targetPos)
	{
		base.transform.LookAt(targetPos);
	}

	public void SetupFiringSound(string clip)
	{
		customFiringAudioClip = IOManager.Instance.LoadAudioClip(clip);
		if (customFiringAudioClip != null)
		{
			firingAudioSource.clip = customFiringAudioClip;
		}
		else
		{
			firingAudioSource.clip = defaultFiringAudioClip;
		}
	}

	public void SetupProjectileImpactSound(string clip)
	{
		customProjectileImpactAudioClip = IOManager.Instance.LoadAudioClip(clip);
	}
}
