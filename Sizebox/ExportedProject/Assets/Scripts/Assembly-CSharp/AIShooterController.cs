using System.Collections;
using SteeringBehaviors;
using UnityEngine;

[RequireComponent(typeof(EntityBase))]
public class AIShooterController : MonoBehaviour
{
	private enum DistancePositioningState
	{
		TooClose = 0,
		Okay = 1,
		SlightlyTooFar = 2,
		TooFar = 3
	}

	private enum AimAlignmentState
	{
		WholeBody = 0,
		UpperBody = 1,
		None = 2
	}

	private Humanoid ownerEntity;

	private Transform userSpine;

	private Animator anim;

	private MovementCharacter movement;

	private WalkController walk;

	private GameObject gunObj;

	private AIGun gun;

	public bool aiming;

	private bool pauseAiming;

	private float aimAngleSin;

	private bool autoReposition;

	private bool repositioning;

	public bool isFiring;

	public bool burstFire;

	public int burstFireRounds = 3;

	private int currentBurstRound = 3;

	public float burstFireInterval = 0.75f;

	private float currentBurstFireInterval;

	public float firingInterval = 4f;

	private float currentInterval = 3f;

	public bool accurateFire;

	public float inaccuracyFactor;

	public bool predictiveAiming;

	private Humanoid aimTargetEntity;

	private Transform aimTarget;

	private int aimAngleSinFloat;

	private int aimBool;

	private int gunTypeInt;

	private DistancePositioningState distanceState;

	private AimAlignmentState aimDirState;

	private const int gunAdjustmentCheckDelay = 2;

	private const int gunReadjustmentCheckDelay = 6;

	private const int gunReadjustmentAngleThreshold = 10;

	private void Awake()
	{
		ownerEntity = GetComponent<Humanoid>();
		anim = ownerEntity.Animator;
		if (!anim)
		{
			Object.Destroy(this);
			return;
		}
		userSpine = anim.GetBoneTransform(HumanBodyBones.Spine);
		movement = ownerEntity.Movement;
		walk = movement.walkController;
		walk.Initialize(movement);
		aimAngleSinFloat = Animator.StringToHash("aimAngleSin");
		aimBool = Animator.StringToHash("aiming");
		gunTypeInt = Animator.StringToHash("gunType");
		accurateFire = GlobalPreferences.AIAccurateShooting.value;
		inaccuracyFactor = GlobalPreferences.AIInaccuracyFactor.value;
		predictiveAiming = GlobalPreferences.AIPredictiveAiming.value;
		burstFire = GlobalPreferences.AIBurstFire.value;
		if (GlobalPreferences.AIRandomIntervals.value)
		{
			currentInterval += Random.Range(0f, 1.5f);
		}
		EquipRaygun();
	}

	private void Update()
	{
		if (aiming)
		{
			if (aimTargetEntity == null)
			{
				StopAiming();
			}
			else if (autoReposition)
			{
				UpdateDistancePositionState();
			}
			else
			{
				UpdateAimAlignmentState();
			}
		}
	}

	private void LateUpdate()
	{
		if (!aiming)
		{
			return;
		}
		if (aimTargetEntity != null && !pauseAiming)
		{
			UpdateAim();
			if (autoReposition)
			{
				UpdatePositioning();
			}
		}
		if (isFiring && !pauseAiming)
		{
			UpdateFiring();
		}
	}

	private void OnDestroy()
	{
		CleanupCurrentGun();
	}

	private Vector3 GetHandCenterPos()
	{
		if ((bool)anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal) && (bool)anim.GetBoneTransform(HumanBodyBones.RightLittleProximal) && (bool)anim.GetBoneTransform(HumanBodyBones.RightThumbProximal))
		{
			Vector3 vector = (anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position + anim.GetBoneTransform(HumanBodyBones.RightHand).position) * 0.5f;
			Vector3 vector2 = (anim.GetBoneTransform(HumanBodyBones.RightLittleProximal).position + anim.GetBoneTransform(HumanBodyBones.RightThumbProximal).position) * 0.5f;
			return (vector + vector2) * 0.5f;
		}
		return anim.GetBoneTransform(HumanBodyBones.RightHand).position;
	}

	public void EquipRaygun()
	{
		if (gunObj != null)
		{
			CleanupCurrentGun();
		}
		gunObj = Object.Instantiate((GameObject)Resources.Load("Raygun/raygunLitePrefab"));
		gun = gunObj.GetComponent<AIRaygun>();
		EquipGun(gun, gunObj);
	}

	public void EquipSMG()
	{
		if (gunObj != null)
		{
			CleanupCurrentGun();
		}
		gunObj = Object.Instantiate((GameObject)Resources.Load("SMG/MP-40Prefab"));
		gun = gunObj.GetComponent<AISMG>();
		EquipGun(gun, gunObj);
	}

	private void EquipGun(AIGun gun, GameObject gunObj)
	{
		if ((bool)anim)
		{
			gun.SetOwner(base.gameObject);
			anim.SetInteger(gunTypeInt, gun.AnimGunType);
			Vector3 forward = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position - anim.GetBoneTransform(HumanBodyBones.RightHand).position;
			Vector3 upwards = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal).position - anim.GetBoneTransform(HumanBodyBones.RightLittleProximal).position;
			Quaternion rotation = Quaternion.LookRotation(forward, upwards);
			gunObj.transform.SetParent(anim.GetBoneTransform(HumanBodyBones.RightHand));
			gunObj.transform.localPosition = anim.GetBoneTransform(HumanBodyBones.RightHand).InverseTransformPoint(GetHandCenterPos());
			gunObj.transform.rotation = rotation;
		}
	}

	public void UnequipGun()
	{
		if (aiming)
		{
			StopAiming();
		}
		CleanupCurrentGun();
		Object.Destroy(this);
	}

	private void CleanupCurrentGun()
	{
		if (!(gunObj == null))
		{
			if (isFiring)
			{
				StopFiring();
			}
			Object.Destroy(gunObj);
		}
	}

	public void StartFiring(bool autoSeek)
	{
		if (!aimTargetEntity || !aimTargetEntity.GetAnimator().GetBoneTransform(HumanBodyBones.Spine))
		{
			if (GlobalPreferences.ScriptAuxLogging.value)
			{
				Debug.Log("No target to start firing at. Please set a traget first.");
			}
			return;
		}
		isFiring = true;
		autoReposition = autoSeek;
		if (!aiming)
		{
			StartAim();
		}
	}

	public void StartSeekFiring(EntityBase target)
	{
		if (target != null)
		{
			SetAimTarget(target);
		}
		StartFiring(true);
	}

	public void StopFiring()
	{
		isFiring = false;
	}

	public void StopSeekFiring()
	{
		autoReposition = false;
		StopAiming();
	}

	public void Fire()
	{
		if (aiming)
		{
			gun.Fire(accurateFire ? 0f : inaccuracyFactor);
			EventManager.SendEvent(new AIWeaponFireEvent(ownerEntity));
		}
	}

	public void SetAimTarget(EntityBase target)
	{
		if (target == null)
		{
			target = null;
			if (aiming)
			{
				StopAiming();
			}
		}
		else
		{
			if (!target.isGiantess && !target.isMicro)
			{
				return;
			}
			Humanoid humanoid = target as Humanoid;
			if ((bool)humanoid)
			{
				aimTargetEntity = humanoid;
				if ((bool)humanoid.GetAnimator().GetBoneTransform(HumanBodyBones.Spine))
				{
					aimTarget = aimTargetEntity.GetAnimator().GetBoneTransform(HumanBodyBones.Spine);
				}
				else
				{
					aimTarget = aimTargetEntity.transform;
				}
				StartAim();
			}
		}
	}

	private void StartAim()
	{
		anim.SetBool(aimBool, true);
		aiming = true;
		pauseAiming = false;
		aimDirState = AimAlignmentState.WholeBody;
		distanceState = DistancePositioningState.Okay;
		StartCoroutine(OrientGun());
	}

	private IEnumerator OrientGun()
	{
		bool transitionStarted = false;
		while (!transitionStarted || !anim.GetAnimatorTransitionInfo(gun.AnimLayer).IsName(gun.AnimTransitionName))
		{
			if (!transitionStarted && anim.GetAnimatorTransitionInfo(gun.AnimLayer).IsName(gun.AnimTransitionName))
			{
				transitionStarted = true;
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		gun.PointAt(aimTarget);
		yield return new WaitForSeconds(6f);
		if (Vector3.Angle(aimTarget.position - gunObj.transform.position, gunObj.transform.forward) > 10f)
		{
			gun.PointAt(aimTarget);
		}
	}

	public void ReorientGun()
	{
		gun.PointAt(aimTarget);
	}

	private void UpdateAimAlignmentState()
	{
		if (!ownerEntity.Movement.move)
		{
			aimDirState = AimAlignmentState.WholeBody;
		}
		else
		{
			Vector3 to = aimTargetEntity.transform.position - base.transform.position;
			to.y = 0f;
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			if (Mathf.Abs(Vector3.Angle(forward, to)) < 70f)
			{
				aimDirState = AimAlignmentState.UpperBody;
			}
			else
			{
				aimDirState = AimAlignmentState.None;
			}
		}
		if (aimDirState != AimAlignmentState.None && pauseAiming)
		{
			pauseAiming = false;
		}
	}

	private void UpdateDistancePositionState()
	{
		if (aimAngleSin > 0.9f)
		{
			distanceState = DistancePositioningState.TooClose;
			return;
		}
		DistancePositioningState num = distanceState;
		float num2 = Vector3.Magnitude(base.transform.position - aimTargetEntity.transform.position);
		float meshHeight = aimTargetEntity.MeshHeight;
		if (distanceState == DistancePositioningState.SlightlyTooFar && num2 < meshHeight * 2.9f)
		{
			distanceState = DistancePositioningState.Okay;
		}
		if (num2 < meshHeight * 3f)
		{
			distanceState = DistancePositioningState.Okay;
		}
		else if (distanceState == DistancePositioningState.TooFar && num2 < meshHeight * 5.9f)
		{
			distanceState = DistancePositioningState.SlightlyTooFar;
		}
		else if (num2 < meshHeight * 6f)
		{
			distanceState = DistancePositioningState.SlightlyTooFar;
		}
		else
		{
			distanceState = DistancePositioningState.TooFar;
		}
		if (num != distanceState)
		{
			anim.Play("Idle");
			movement.Stop();
			repositioning = false;
		}
	}

	private void UpdateAim()
	{
		if (aimDirState == AimAlignmentState.None)
		{
			if (anim.GetBool(aimBool))
			{
				anim.SetBool(aimBool, false);
				pauseAiming = true;
			}
			return;
		}
		if (!anim.GetBool(aimBool))
		{
			anim.SetBool(aimBool, true);
		}
		UpdateArmAim();
		UpdateAimAlignment();
	}

	public void StopAiming()
	{
		anim.SetBool(aimBool, false);
		aiming = false;
		aimTargetEntity = null;
		if (isFiring)
		{
			StopFiring();
		}
	}

	private void UpdateArmAim()
	{
		float num = GetTargetPos().y - userSpine.position.y;
		float num2 = Vector3.Distance(userSpine.position, GetTargetEntityPos());
		aimAngleSin = num / num2;
		if (distanceState == DistancePositioningState.TooFar)
		{
			aimAngleSin += 0.3f;
		}
		anim.SetFloat(aimAngleSinFloat, aimAngleSin);
	}

	private void UpdateAimAlignment()
	{
		if (aimDirState == AimAlignmentState.WholeBody)
		{
			base.transform.LookAt(new Vector3(GetTargetPos().x, base.transform.position.y, GetTargetPos().z));
			return;
		}
		Vector3 toDirection = GetTargetEntityPos() - anim.GetBoneTransform(HumanBodyBones.Spine).position;
		toDirection.y = 0f;
		anim.GetBoneTransform(HumanBodyBones.Spine).rotation = Quaternion.FromToRotation(base.transform.forward, toDirection) * anim.GetBoneTransform(HumanBodyBones.Spine).rotation;
	}

	private void UpdateFiring()
	{
		currentInterval -= Time.deltaTime;
		if (!(currentInterval <= 0f))
		{
			return;
		}
		if (!burstFire)
		{
			Fire();
			currentInterval = firingInterval;
			if (GlobalPreferences.AIRandomIntervals.value)
			{
				currentInterval += Random.Range(0f, 1.5f);
			}
			return;
		}
		currentBurstFireInterval -= Time.deltaTime;
		if (!(currentBurstFireInterval <= 0f))
		{
			return;
		}
		Fire();
		currentBurstRound--;
		if (currentBurstRound != 0)
		{
			currentBurstFireInterval = burstFireInterval;
			return;
		}
		currentBurstRound = burstFireRounds;
		currentInterval = firingInterval;
		if (GlobalPreferences.AIRandomIntervals.value)
		{
			currentInterval += Random.Range(0f, 1.5f);
		}
	}

	private void UpdatePositioning()
	{
		if (distanceState == DistancePositioningState.TooClose)
		{
			if (!repositioning)
			{
				anim.Play("Walk Backwards");
				movement.StartMoveInDirectionBehavior(new VectorKinematic(base.transform.position - aimTargetEntity.transform.position));
				repositioning = true;
			}
		}
		else
		{
			if (distanceState == DistancePositioningState.Okay)
			{
				return;
			}
			if (distanceState == DistancePositioningState.SlightlyTooFar)
			{
				if (!repositioning)
				{
					anim.Play("Walk");
					movement.StartMoveInDirectionBehavior(new VectorKinematic(aimTargetEntity.transform.position - base.transform.position));
					repositioning = true;
				}
			}
			else if (!repositioning)
			{
				anim.Play("Run");
				movement.StartSeekBehavior(new TransformKinematic(aimTargetEntity.transform), 0f, 0f);
				repositioning = true;
			}
		}
	}

	private Vector3 GetTargetPos()
	{
		if (!predictiveAiming || !aimTargetEntity.Movement.move)
		{
			return aimTarget.position;
		}
		return aimTarget.position + aimTargetEntity.Movement.velocity * 0.5f;
	}

	private Vector3 GetTargetEntityPos()
	{
		if (!predictiveAiming || !aimTargetEntity.Movement.move)
		{
			return aimTargetEntity.transform.position;
		}
		return aimTargetEntity.transform.position + aimTargetEntity.Movement.velocity * 0.5f;
	}

	public void SetBurstFireMode(bool enable)
	{
		burstFire = enable;
		if (enable)
		{
			currentBurstFireInterval = 0f;
			currentBurstRound = burstFireRounds;
		}
	}

	public void SetProjectileColor(int r, int g, int b)
	{
		gun.SetColor(r, g, b);
	}

	public void SetProjectileSpeed(float speedMult)
	{
		gun.SetProjectileSpeed(speedMult);
	}

	public void SetProjectileScale(float scaleMult)
	{
		gun.SetProjectileScale(scaleMult);
	}

	public void SetRaygunFiringSound(string clip)
	{
		gun.SetupFiringSound(clip);
	}

	public void SetProjectileImpactSound(string clip)
	{
		gun.SetupProjectileImpactSound(clip);
	}
}
