using RootMotion.FinalIK;
using UnityEngine;

public class HeadIK
{
	private delegate void State();

	private LookAtIK ik;

	private Giantess giantess;

	private EntityBase target;

	private Vector3 lookDirection;

	private Transform head;

	private Transform neck;

	private float speed;

	private float bodyWeight = 0.2f;

	private const float bodyWeightStand = 0.2f;

	private const float bodyWeightBend = 0.75f;

	private const float headWeight = 1f;

	private float bodyDistance = 1000f;

	private float headDistance = 400f;

	private float targetWeight;

	public bool unlockValues;

	private Vector3 virtualTargetPosition;

	private Vector3 virtualIKPosition;

	private State CurrentState;

	private Vector3 targetPosition
	{
		get
		{
			return CenterOrigin.VirtualToWorld(virtualTargetPosition);
		}
		set
		{
			virtualTargetPosition = CenterOrigin.WorldToVirtual(value);
		}
	}

	private Vector3 ikPosition
	{
		get
		{
			return CenterOrigin.VirtualToWorld(virtualIKPosition);
		}
		set
		{
			virtualIKPosition = CenterOrigin.WorldToVirtual(value);
		}
	}

	public HeadIK(LookAtIK ik, Giantess giantess)
	{
		this.ik = ik;
		this.giantess = giantess;
		head = giantess.Animator.GetBoneTransform(HumanBodyBones.Head);
		neck = giantess.Animator.GetBoneTransform(HumanBodyBones.Neck);
		SetDefaultValues();
	}

	public void SetDefaultValues()
	{
		ik.enabled = true;
		ik.fixTransforms = false;
		ik.solver.bodyWeight = bodyWeight;
		ik.solver.headWeight = 1f;
		ik.solver.eyesWeight = 0.4f;
		ik.solver.clampWeight = 0.5f;
		ik.solver.clampWeightHead = 0.6f;
		ik.solver.clampWeightEyes = 0.7f;
		ik.solver.clampSmoothing = 2;
		CurrentState = Start;
	}

	public void LookAt(EntityBase entity)
	{
		target = entity;
		CurrentState = Look;
	}

	public void DisableLookAt()
	{
		CurrentState = TotalDisable;
	}

	public void LookAtPoint(Vector3 point)
	{
		if (GlobalPreferences.LookAtPlayer.value && giantess.canLookAtPlayer)
		{
			lookDirection = point;
			CurrentState = LookPoint;
		}
	}

	public void Cancel()
	{
		target = null;
		CurrentState = Start;
	}

	public void Update()
	{
		CurrentState();
		UpdateEffector();
	}

	private void UpdateEffector()
	{
		speed = giantess.animationManager.GetCurrentSpeed();
		float magnitude = (giantess.transform.InverseTransformPoint(head.position) - giantess.transform.InverseTransformPoint(targetPosition)).magnitude;
		ik.solver.headWeight = Mathf.Clamp01(magnitude / headDistance) * 1f;
		if (neck.position.y < targetPosition.y)
		{
			if (bodyWeight > 0.2f)
			{
				bodyWeight = Mathf.Lerp(bodyWeight, 0.2f, Time.fixedDeltaTime * speed);
			}
		}
		else if (bodyWeight < 0.75f)
		{
			bodyWeight = Mathf.Lerp(bodyWeight, 0.75f, Time.fixedDeltaTime * speed);
		}
		ik.solver.bodyWeight = Mathf.Clamp01(magnitude / bodyDistance) * bodyWeight;
		ik.solver.IKPositionWeight = Mathf.Lerp(ik.solver.IKPositionWeight, targetWeight, Time.fixedDeltaTime * speed);
		ik.solver.IKPosition = Vector3.Slerp(ikPosition, targetPosition, Time.fixedDeltaTime * 4f * speed);
		ikPosition = ik.solver.IKPosition;
		ik.solver.clampWeight = GlobalPreferences.ClampWeight.value;
		ik.solver.clampWeightHead = GlobalPreferences.ClampWeightHead.value;
		ik.solver.clampWeightEyes = GlobalPreferences.ClampWeightEyes.value;
		ik.solver.clampSmoothing = GlobalPreferences.ClampSmoothing.value;
	}

	private void Start()
	{
		Humanoid humanoid = target as Humanoid;
		if (!target || ((bool)humanoid && humanoid.IsDead))
		{
			target = GameController.LocalClient.Player.Entity;
		}
		targetWeight = Mathf.Lerp(targetWeight, 0f, Time.deltaTime * speed);
		if (giantess.senses.CheckVisibility(target))
		{
			CurrentState = Look;
		}
	}

	private void CantSee()
	{
		bodyWeight = Mathf.Lerp(bodyWeight, 0.2f, Time.fixedDeltaTime * speed);
		targetWeight = Mathf.Lerp(targetWeight, 0.1f, Time.deltaTime * speed * 0.2f);
		if (target != null && giantess.senses.CheckVisibility(target))
		{
			CurrentState = Look;
		}
	}

	private void Look()
	{
		Humanoid humanoid = target as Humanoid;
		if (!target || ((bool)humanoid && humanoid.IsDead))
		{
			CurrentState = Start;
			return;
		}
		targetPosition = target.transform.position + Vector3.up * target.Height * 0.9f;
		targetWeight = Mathf.Lerp(targetWeight, 1f, Time.deltaTime * speed);
		if (!GlobalPreferences.LookAtPlayer.value || !giantess.canLookAtPlayer)
		{
			CurrentState = Disabled;
		}
		else if (!giantess.senses.CheckVisibility(target))
		{
			CurrentState = CantSee;
		}
	}

	private void LookPoint()
	{
		targetWeight = Mathf.Lerp(targetWeight, 1f, Time.deltaTime * speed);
		targetPosition = lookDirection;
		if (!GlobalPreferences.LookAtPlayer.value || !giantess.canLookAtPlayer)
		{
			CurrentState = Disabled;
		}
	}

	private void Disabled()
	{
		targetWeight = Mathf.Lerp(targetWeight, 0f, Time.deltaTime * speed);
		if (GlobalPreferences.LookAtPlayer.value && giantess.canLookAtPlayer)
		{
			CurrentState = Start;
		}
	}

	private void TotalDisable()
	{
		targetWeight = Mathf.Lerp(targetWeight, 0f, Time.deltaTime * speed);
	}
}
