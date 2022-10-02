using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaygun : PlayerGun
{
	public enum FiringMode
	{
		SingleFire = 0,
		Continuous = 1,
		Sonic = 2
	}

	public FiringMode currentFiringMode;

	public static PlayerRaygun instance;

	[SerializeField]
	private GameObject model;

	[SerializeField]
	private MeshRenderer modelMeshRenderer;

	private GameObject reticuleUIObj;

	private RaygunReticule reticuleUI;

	[SerializeField]
	private Rigidbody rb;

	private LayerMask projectileMask;

	private LayerMask laserMask;

	private LayerMask sonicMask;

	private AnimationCurve expandingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private AnimationCurve retractingCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	private Color shrinkColor;

	private Color growColor;

	private Color activeColor;

	private Gradient activeEmitterParticlesGradient;

	private Gradient singleFireEmitterParticlesGradient = new Gradient();

	private Gradient laserLineGradient = new Gradient();

	private Gradient laserParticlesGradient = new Gradient();

	private Gradient sonicParticlesGradient = new Gradient();

	[SerializeField]
	private LineRenderer line;

	private float baseLineStartWidth = 0.15f;

	private float baseLineEndWidth = 0.3f;

	[SerializeField]
	private ParticleSystem emitterParticles;

	[SerializeField]
	private ParticleSystem shotEmitterParticles;

	[SerializeField]
	private ParticleSystem sonicParticles;

	[SerializeField]
	private ParticleSystem chargeParticles;

	[SerializeField]
	private ParticleSystem impactParticles;

	[SerializeField]
	private GameObject impactParticlesObj;

	[SerializeField]
	private CapsuleCollider sonicCollider;

	private float baseSonicColliderRadius = 60f;

	private float playerScale = 1f;

	private bool firstPersonMode;

	private float sonicLengthMult = 0.5f;

	private bool isFiring;

	private float polarityMagnitude = 0.5f;

	private float currentCharge;

	private List<GameObject> sonicAffectedObjects = new List<GameObject>();

	private Dictionary<Transform, int> sonicAffectedRootTransforms = new Dictionary<Transform, int>();

	private bool checkCurrentObjectsInFrustrum;

	private Color scriptShrinkColor;

	private Color scriptGrowColor;

	private bool scriptEnableRaygun = true;

	[SerializeField]
	private AudioSource firingAudioSource1;

	[SerializeField]
	private AudioSource firingAudioSource2;

	[SerializeField]
	private AudioSource auxiliaryAudioSource;

	private Player player;

	private void Start()
	{
		player = GameController.LocalClient.Player;
		currentFiringMode = FiringMode.SingleFire;
		rb.detectCollisions = false;
		SetupMasks();
		growColor = new Color(GlobalPreferences.GrowColorR.value, GlobalPreferences.GrowColorG.value, GlobalPreferences.GrowColorB.value);
		shrinkColor = new Color(GlobalPreferences.ShrinkColorR.value, GlobalPreferences.ShrinkColorG.value, GlobalPreferences.ShrinkColorB.value);
		activeColor = growColor;
		line.positionCount = 3;
		line.enabled = false;
		line.startWidth = baseLineStartWidth;
		line.endWidth = baseLineEndWidth;
		RefreshColors();
		instance = this;
	}

	private void OnEnable()
	{
		if (reticuleUIObj != null)
		{
			reticuleUIObj.SetActive(true);
			return;
		}
		reticuleUIObj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Raygun/UI/raygun_reticule"));
		reticuleUI = reticuleUIObj.GetComponent<RaygunReticule>();
	}

	private void OnDisable()
	{
		StopFiring();
		if (reticuleUIObj != null)
		{
			reticuleUIObj.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (reticuleUI != null)
		{
			GameObject gameObject = reticuleUI.gameObject;
			if ((bool)gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	public void SetupMasks()
	{
		SetupProjectileMask();
		SetupLaserMask();
		SetupSonicMask();
	}

	public void SetupProjectileMask()
	{
		List<string> list = new List<string>();
		list.Add(LayerMask.LayerToName(Layers.mapLayer));
		if (GlobalPreferences.PlayerProjectileGtsMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.gtsBodyLayer));
		}
		if (GlobalPreferences.PlayerProjectileMicroMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.microLayer));
		}
		if (GlobalPreferences.PlayerProjectileObjectMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.objectLayer));
		}
		projectileMask = LayerMask.GetMask(list.ToArray());
	}

	public void SetupLaserMask()
	{
		List<string> list = new List<string>();
		list.Add(LayerMask.LayerToName(Layers.mapLayer));
		if (GlobalPreferences.LaserGtsMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.gtsBodyLayer));
		}
		if (GlobalPreferences.LaserMicroMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.microLayer));
		}
		if (GlobalPreferences.LaserObjectMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.objectLayer));
		}
		laserMask = LayerMask.GetMask(list.ToArray());
	}

	public void SetupSonicMask()
	{
		List<string> list = new List<string>();
		if (GlobalPreferences.SonicGtsMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.gtsBodyLayer));
		}
		if (GlobalPreferences.SonicMicroMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.microLayer));
		}
		if (GlobalPreferences.SonicObjectMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.objectLayer));
		}
		sonicMask = LayerMask.GetMask(list.ToArray());
	}

	private void LateUpdate()
	{
		if (GameController.Instance.paused)
		{
			return;
		}
		if ((bool)player.Entity)
		{
			playerScale = player.Entity.Scale;
		}
		if (isFiring)
		{
			switch (currentFiringMode)
			{
			case FiringMode.SingleFire:
				ChargeProjectile();
				break;
			case FiringMode.Continuous:
				FireContinuousLaser();
				break;
			case FiringMode.Sonic:
				FireSonic();
				break;
			}
		}
		if (StateManager.Keyboard.Shift)
		{
			UpdateUtilityInput();
		}
		else
		{
			UpdatePolarityInput();
		}
	}

	private void FixedUpdate()
	{
		if (currentFiringMode != FiringMode.Sonic || !isFiring)
		{
			return;
		}
		foreach (KeyValuePair<Transform, int> sonicAffectedRootTransform in sonicAffectedRootTransforms)
		{
			AffectSonicObject(sonicAffectedRootTransform.Key);
		}
	}

	private void UpdatePolarityInput()
	{
		float num = InputManager.inputs.Micro.WeaponRange.ReadValue<float>() / 10f;
		if (Math.Abs(num) > float.Epsilon)
		{
			polarityMagnitude = Mathf.Clamp(polarityMagnitude + num, -1f, 1f);
			reticuleUI.ChangePolarityValue(polarityMagnitude);
			activeColor = ((polarityMagnitude < 0f) ? shrinkColor : growColor);
			UpdateGradient();
			UpdateChamberColor();
			if (currentFiringMode == FiringMode.Continuous)
			{
				UpdateLaserAlpha();
			}
			PlayAuxiliarySound(SoundManager.Instance.playerRaygunPolaritySound);
		}
	}

	private void UpdateUtilityInput()
	{
		float num = InputManager.inputs.Micro.WeaponRange.ReadValue<float>() * 5f;
		if (Math.Abs(num) > float.Epsilon)
		{
			switch (currentFiringMode)
			{
			case FiringMode.SingleFire:
				GlobalPreferences.PlayerProjectileSpeed.value = Mathf.Clamp(GlobalPreferences.PlayerProjectileSpeed.value + num / 2f, 0.25f, 3f);
				break;
			case FiringMode.Continuous:
				GlobalPreferences.LaserWidth.value = Mathf.Clamp(GlobalPreferences.LaserWidth.value + num, 0.5f, 15f);
				break;
			case FiringMode.Sonic:
				GlobalPreferences.SonicWidth.value = Mathf.Clamp(GlobalPreferences.SonicWidth.value + num, 0.5f, 3f);
				SetupParticlesForSonicFire();
				break;
			}
			PlayAuxiliarySound(SoundManager.Instance.playerRaygunModeSwitchSound);
		}
	}

	public void RefreshColors()
	{
		if (!scriptGrowColor.Equals(Color.clear) && GlobalPreferences.RaygunScriptMode.value)
		{
			growColor = scriptGrowColor;
		}
		else
		{
			growColor = new Color(GlobalPreferences.GrowColorR.value, GlobalPreferences.GrowColorG.value, GlobalPreferences.GrowColorB.value);
		}
		if (!scriptShrinkColor.Equals(Color.clear) && GlobalPreferences.RaygunScriptMode.value)
		{
			shrinkColor = scriptShrinkColor;
		}
		else
		{
			shrinkColor = new Color(GlobalPreferences.ShrinkColorR.value, GlobalPreferences.ShrinkColorG.value, GlobalPreferences.ShrinkColorB.value);
		}
		activeColor = ((polarityMagnitude < 0f) ? shrinkColor : growColor);
		UpdateGradient();
		UpdateChamberColor();
	}

	public void RefreshUI()
	{
		reticuleUI.SetupUI();
	}

	public void ResetUIFade()
	{
		reticuleUI.ResetFade();
	}

	public void ChangeAuxiliaryUIColor(Color color)
	{
		reticuleUI.ChangeAuxiliaryColor(color);
	}

	public void StartFiring()
	{
		if (scriptEnableRaygun || !GlobalPreferences.RaygunScriptMode.value)
		{
			switch (currentFiringMode)
			{
			case FiringMode.SingleFire:
				currentCharge = 8f;
				SetupParticlesForSingleFire();
				SetupSoundsForProjectile();
				break;
			case FiringMode.Continuous:
				SetupParticlesForLaser();
				SetupSoundsForLaser();
				break;
			case FiringMode.Sonic:
				sonicAffectedObjects.Clear();
				sonicAffectedRootTransforms.Clear();
				SetupParticlesForSonicFire();
				SetupSoundsForSonic();
				break;
			}
			isFiring = true;
			EventManager.SendEvent(new PlayerTriggerPressEvent());
		}
	}

	private void ChargeProjectile()
	{
		emitterParticles.transform.localScale = Vector3.one * playerScale;
		if (emitterParticles.isStopped)
		{
			emitterParticles.Play();
			chargeParticles.Play();
		}
		currentCharge = Mathf.Lerp(currentCharge, 1f, Time.deltaTime / 2f * GlobalPreferences.ProjectileChargeRate.value);
		ParticleSystem.MainModule main = emitterParticles.main;
		main.startLifetime = new ParticleSystem.MinMaxCurve(currentCharge);
	}

	private void FireProjectile()
	{
		float num = 6f / currentCharge * 4f;
		Vector3 position = firingPoint.position + firingPoint.forward * num / 5f * playerScale;
		UnityEngine.Object.Instantiate(projectilePrefab, position, base.transform.rotation).GetComponent<PlayerRaygunProjectile>().Initialize(polarityMagnitude, num, activeColor, projectileMask, playerScale);
		emitterParticles.Stop();
		chargeParticles.Stop();
		chargeParticles.Clear();
		if (!firstPersonMode)
		{
			shotEmitterParticles.transform.localScale = Vector3.one * playerScale;
			shotEmitterParticles.Emit(20);
		}
		firingAudioSource1.volume = 1f;
		firingAudioSource1.Play();
	}

	private void FireContinuousLaser()
	{
		emitterParticles.transform.localScale = Vector3.one * playerScale;
		if (emitterParticles.isStopped && !firstPersonMode)
		{
			emitterParticles.Play();
		}
		firingAudioSource2.volume = Mathf.Abs(polarityMagnitude * 0.67f) + 0.33f;
		if (!line.enabled)
		{
			firingAudioSource2.Play();
		}
		EntityBase entityBase = null;
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 1000000f, laserMask))
		{
			vector = hitInfo.point;
			entityBase = hitInfo.transform.GetComponentInParent<EntityBase>();
			if ((bool)entityBase)
			{
				float num = 0.25f;
				if (entityBase.isGiantess)
				{
					num = 10f * Mathf.Log(entityBase.Scale + 1f, 2f);
				}
				if (!GlobalPreferences.RaygunScriptMode.value)
				{
					ResizeManager obj = entityBase.GetComponent<ResizeManager>() ?? entityBase.gameObject.AddComponent<ResizeManager>();
					ResizeManager.Resizer resizer = new ResizeManager.Resizer(0.1f, 0.01f * polarityMagnitude * GlobalPreferences.LaserEffectMultiplier.value, false);
					obj.AddResizer(resizer);
				}
				else
				{
					EventManager.SendEvent(new PlayerRaygunHitEvent(entityBase.GetComponent<EntityBase>(), polarityMagnitude, 1, 0f));
				}
				if (GlobalPreferences.LaserImpactParticles.value)
				{
					impactParticlesObj.transform.position = vector;
					float num2 = 10f * Mathf.Log(GlobalPreferences.LaserWidth.value + 1f, 6f) * num * GlobalPreferences.LaserImpactParticlesSizeMult.value;
					impactParticlesObj.transform.localScale = new Vector3(num2, num2, num2);
					if (impactParticles.isStopped)
					{
						impactParticles.Play();
					}
				}
			}
		}
		if (!entityBase)
		{
			vector = transform.position + transform.forward * 1000f * playerScale;
			if (impactParticles.isPlaying)
			{
				impactParticles.Stop();
				impactParticles.Clear();
			}
		}
		line.enabled = true;
		line.startWidth = baseLineStartWidth * playerScale * GlobalPreferences.LaserWidth.value;
		line.endWidth = baseLineEndWidth * playerScale * GlobalPreferences.LaserWidth.value;
		line.SetPosition(0, firingPoint.position);
		float num3 = Vector3.Magnitude(vector - firingPoint.position);
		Vector3 vector2 = ((num3 < 5f) ? (Vector3.Normalize(vector - firingPoint.position) * 0.96f * playerScale) : ((!(num3 < 100f)) ? (Vector3.Normalize(vector - firingPoint.position) * (5f + GlobalPreferences.LaserWidth.value) * playerScale) : ((vector - firingPoint.position) * (4f + GlobalPreferences.LaserWidth.value) / 100f * playerScale)));
		Vector3 position = vector2 + firingPoint.position;
		line.SetPosition(1, position);
		line.SetPosition(2, vector);
	}

	private void ClearLaser()
	{
		impactParticles.Stop();
		emitterParticles.Stop();
		line.enabled = false;
		firingAudioSource2.Stop();
	}

	private void FireSonic()
	{
		sonicParticles.transform.localScale = Vector3.one * playerScale;
		emitterParticles.transform.localScale = Vector3.one * playerScale;
		if (emitterParticles.isStopped && !firstPersonMode)
		{
			emitterParticles.Play();
		}
		firingAudioSource2.volume = Mathf.Abs(polarityMagnitude * 0.67f) + 0.33f;
		if (sonicParticles.isStopped)
		{
			sonicParticles.Play();
			firingAudioSource1.volume = Mathf.Abs(polarityMagnitude * 0.67f) + 0.33f;
			firingAudioSource1.Play();
			firingAudioSource2.Play();
		}
		if (!rb.detectCollisions)
		{
			StartCoroutine(ActivateSonicFrustrum());
		}
	}

	private IEnumerator ActivateSonicFrustrum()
	{
		checkCurrentObjectsInFrustrum = true;
		if (currentFiringMode == FiringMode.Sonic && isFiring)
		{
			rb.detectCollisions = true;
		}
		yield return null;
		yield return null;
		yield return null;
		checkCurrentObjectsInFrustrum = false;
	}

	private void ClearSonic()
	{
		sonicParticles.Stop();
		sonicParticles.Clear();
		emitterParticles.Stop();
		rb.detectCollisions = false;
		checkCurrentObjectsInFrustrum = false;
		firingAudioSource2.Stop();
	}

	private void OnTriggerStay(Collider collider)
	{
		if (checkCurrentObjectsInFrustrum)
		{
			OnTriggerEnter(collider);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		Transform root = collider.transform.root;
		if ((sonicMask.value & (1 << gameObject.layer)) != 1 << gameObject.layer || gameObject.layer == Layers.playerLayer || sonicAffectedObjects.Contains(gameObject))
		{
			return;
		}
		sonicAffectedObjects.Add(gameObject);
		if (sonicAffectedRootTransforms.ContainsKey(root))
		{
			if (!GlobalPreferences.SonicTagging.value)
			{
				sonicAffectedRootTransforms[root] += 1;
			}
		}
		else
		{
			sonicAffectedRootTransforms.Add(root, 1);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		Transform root = collider.transform.root;
		if ((sonicMask.value & (1 << gameObject.layer)) != 1 << gameObject.layer || gameObject.layer == Layers.playerLayer || !sonicAffectedObjects.Contains(gameObject))
		{
			return;
		}
		sonicAffectedObjects.Remove(gameObject);
		if (!GlobalPreferences.SonicTagging.value)
		{
			if (sonicAffectedRootTransforms.ContainsKey(root) && sonicAffectedRootTransforms[root] > 1)
			{
				sonicAffectedRootTransforms[root] -= 1;
			}
			else
			{
				sonicAffectedRootTransforms.Remove(root);
			}
		}
	}

	private void AffectSonicObject(Transform target)
	{
		float num = 0.25f;
		GameObject gameObject = target.gameObject;
		if (!GlobalPreferences.RaygunScriptMode.value)
		{
			ResizeManager obj = gameObject.GetComponent<ResizeManager>() ?? gameObject.AddComponent<ResizeManager>();
			ResizeManager.Resizer resizer = new ResizeManager.Resizer(0.1f, 0.01f * polarityMagnitude * GlobalPreferences.SonicEffectMultiplier.value * num, false);
			obj.AddResizer(resizer);
		}
		else
		{
			EventManager.SendEvent(new PlayerRaygunHitEvent(gameObject.GetComponent<EntityBase>(), polarityMagnitude, 2, 0f));
		}
	}

	public void StopFiring()
	{
		if (isFiring)
		{
			switch (currentFiringMode)
			{
			case FiringMode.SingleFire:
				FireProjectile();
				break;
			case FiringMode.Continuous:
				ClearLaser();
				break;
			case FiringMode.Sonic:
				sonicAffectedObjects.Clear();
				sonicAffectedRootTransforms.Clear();
				ClearSonic();
				break;
			}
		}
		isFiring = false;
		EventManager.SendEvent(new PlayerTriggerReleaseEvent());
	}

	public void SwitchFiringMode()
	{
		StopFiring();
		if (currentFiringMode != FiringMode.Sonic)
		{
			currentFiringMode++;
		}
		else
		{
			currentFiringMode = FiringMode.SingleFire;
		}
		reticuleUI.ChangeFiringMode((int)currentFiringMode);
		PlayAuxiliarySound(SoundManager.Instance.playerRaygunModeSwitchSound);
	}

	private void UpdateChamberColor()
	{
		Material[] materials = modelMeshRenderer.materials;
		Material material = UnityEngine.Object.Instantiate(materials[6]);
		material.SetColor("_Color", activeColor);
		Vector4 vector = new Vector4(activeColor.r, activeColor.g, activeColor.b, 0f) * (Mathf.Abs(polarityMagnitude) * 3f);
		material.SetColor("_EmissionColor", vector);
		materials[6] = material;
		modelMeshRenderer.materials = materials;
	}

	private void UpdateGradient()
	{
		float num = Mathf.Abs(polarityMagnitude);
		laserLineGradient.SetKeys(new GradientColorKey[4]
		{
			new GradientColorKey(Color.clear, 0f),
			new GradientColorKey(activeColor, 0.1f),
			new GradientColorKey(activeColor, 0.96f),
			new GradientColorKey(Color.clear, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(0f, 0f),
			new GradientAlphaKey(1f, 1f)
		});
		line.colorGradient = laserLineGradient;
		laserParticlesGradient.SetKeys(new GradientColorKey[3]
		{
			new GradientColorKey(Color.clear, 0f),
			new GradientColorKey(activeColor, 0.5f),
			new GradientColorKey(Color.clear, 1f)
		}, new GradientAlphaKey[4]
		{
			new GradientAlphaKey(0f, 0f),
			new GradientAlphaKey(1f * num, 0.4f),
			new GradientAlphaKey(1f * num, 0.7f),
			new GradientAlphaKey(0f, 1f)
		});
		singleFireEmitterParticlesGradient.SetKeys(new GradientColorKey[1]
		{
			new GradientColorKey(activeColor, 0f)
		}, new GradientAlphaKey[1]
		{
			new GradientAlphaKey(1f, 0f)
		});
		sonicParticlesGradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(activeColor, 0f),
			new GradientColorKey(activeColor, 1f)
		}, new GradientAlphaKey[4]
		{
			new GradientAlphaKey(0f, 0f),
			new GradientAlphaKey(Mathf.Clamp(0.7f * num, 0.005f, 2f), 0.1f),
			new GradientAlphaKey(Mathf.Clamp(0.3f * num, 0.001f, 2f), 0.85f),
			new GradientAlphaKey(0f, 1f)
		});
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = emitterParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		colorOverLifetime = chargeParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		colorOverLifetime = impactParticles.colorOverLifetime;
		colorOverLifetime.color = laserParticlesGradient;
		colorOverLifetime = sonicParticles.colorOverLifetime;
		colorOverLifetime.color = sonicParticlesGradient;
	}

	private void UpdateLaserAlpha()
	{
		Material material = UnityEngine.Object.Instantiate(line.material);
		material.SetFloat("_alpha", Mathf.Abs(polarityMagnitude));
		line.material = material;
	}

	private void SetupParticlesForLaser()
	{
		activeEmitterParticlesGradient = laserParticlesGradient;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = emitterParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = emitterParticles.sizeOverLifetime;
		sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, expandingCurve);
		ParticleSystem.EmissionModule emission = emitterParticles.emission;
		emission.rateOverTime = 2f;
		ParticleSystem.MainModule main = emitterParticles.main;
		main.maxParticles = 50;
		main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 1.5f);
		main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 1f);
	}

	private void SetupParticlesForSingleFire()
	{
		activeEmitterParticlesGradient = singleFireEmitterParticlesGradient;
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = emitterParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		colorOverLifetime = shotEmitterParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		if (chargeParticles.isPlaying)
		{
			chargeParticles.Stop();
		}
		colorOverLifetime = chargeParticles.colorOverLifetime;
		colorOverLifetime.color = activeEmitterParticlesGradient;
		ParticleSystem.MainModule main = chargeParticles.main;
		main.duration = 4f / GlobalPreferences.ProjectileChargeRate.value;
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = emitterParticles.sizeOverLifetime;
		sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, retractingCurve);
		ParticleSystem.EmissionModule emission = emitterParticles.emission;
		emission.rateOverTime = 0.5f;
		main = emitterParticles.main;
		main.maxParticles = 1;
		main.startLifetime = new ParticleSystem.MinMaxCurve(8f);
		main.startSize = new ParticleSystem.MinMaxCurve(1f);
	}

	private void SetupParticlesForSonicFire()
	{
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = sonicParticles.colorOverLifetime;
		colorOverLifetime.color = sonicParticlesGradient;
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(0f, 0.25f);
		animationCurve.AddKey(1f, 7f);
		ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = sonicParticles.sizeOverLifetime;
		sizeOverLifetime.x = new ParticleSystem.MinMaxCurve(GlobalPreferences.SonicWidth.value, animationCurve);
		sizeOverLifetime.y = new ParticleSystem.MinMaxCurve(GlobalPreferences.SonicWidth.value, animationCurve);
		sizeOverLifetime.z = new ParticleSystem.MinMaxCurve(5f);
		ParticleSystem.EmissionModule emission = sonicParticles.emission;
		emission.rateOverTime = 12f;
		ParticleSystem.MainModule main = sonicParticles.main;
		main.maxParticles = (int)(2000f * sonicLengthMult);
		main.startLifetime = new ParticleSystem.MinMaxCurve(3f * sonicLengthMult);
		main.startSize = new ParticleSystem.MinMaxCurve(50f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(12f);
		if (GlobalPreferences.SonicWidth.value <= 1f)
		{
			sonicCollider.radius = 5f + GlobalPreferences.SonicWidth.value * 30f;
			sonicCollider.height = 240f;
			sonicCollider.center = new Vector3(0f, 1f, 135f);
		}
		else
		{
			sonicCollider.radius = baseSonicColliderRadius * (GlobalPreferences.SonicWidth.value - 0.75f);
			if (GlobalPreferences.SonicWidth.value > 2f)
			{
				sonicCollider.height = 290f;
				sonicCollider.center = new Vector3(0f, 1f, 160f);
			}
			else
			{
				sonicCollider.center = new Vector3(0f, 1f, 145f);
			}
		}
		SetupParticlesForLaser();
	}

	public void SetFirstPersonMode(bool isFpsMode)
	{
		firstPersonMode = isFpsMode;
		EnableModelRender(!isFpsMode);
	}

	private void EnableModelRender(bool render)
	{
		if (model == null)
		{
			model = base.transform.Find("raygun").gameObject;
		}
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = render;
		}
	}

	private void SetupSoundsForProjectile()
	{
		firingAudioSource1.clip = SoundManager.Instance.playerRaygunProjFireSound;
	}

	private void SetupSoundsForLaser()
	{
		firingAudioSource2.clip = SoundManager.Instance.playerRaygunLaserSustainSound;
	}

	private void SetupSoundsForSonic()
	{
		firingAudioSource1.clip = SoundManager.Instance.playerRaygunSonicFireSound;
		firingAudioSource2.clip = SoundManager.Instance.playerRaygunSonicSustainSound;
	}

	private void PlayAuxiliarySound(AudioClip clip)
	{
		auxiliaryAudioSource.clip = clip;
		auxiliaryAudioSource.Play();
	}

	public void UpdateSounds()
	{
		switch (currentFiringMode)
		{
		case FiringMode.SingleFire:
			SetupSoundsForProjectile();
			break;
		case FiringMode.Continuous:
			SetupSoundsForLaser();
			break;
		case FiringMode.Sonic:
			SetupSoundsForSonic();
			break;
		}
	}

	public void SetScriptGrowColor(int r, int g, int b)
	{
		scriptGrowColor = new Color(Mathf.Clamp01((float)r / 255f), Mathf.Clamp01((float)g / 255f), Mathf.Clamp01((float)b / 255f));
		RefreshColors();
	}

	public void ClearScriptGrowColor()
	{
		scriptGrowColor = Color.clear;
		RefreshColors();
	}

	public void SetScriptShrinkColor(int r, int g, int b)
	{
		scriptShrinkColor = new Color(Mathf.Clamp01((float)r / 255f), Mathf.Clamp01((float)g / 255f), Mathf.Clamp01((float)b / 255f));
		RefreshColors();
	}

	public void ClearScriptShrinkColor()
	{
		scriptShrinkColor = Color.clear;
		RefreshColors();
	}

	public void SetScriptEnableRaygun(bool enable)
	{
		scriptEnableRaygun = enable;
	}

	public bool GetScriptEnableRaygun()
	{
		return scriptEnableRaygun;
	}
}
