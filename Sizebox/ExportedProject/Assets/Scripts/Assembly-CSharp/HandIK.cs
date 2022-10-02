using RootMotion.FinalIK;
using UnityEngine;

public class HandIK
{
	private delegate void State();

	private FullBodyBipedIK ik;

	private Giantess giantess;

	private EntityBase target;

	private Transform hand;

	private IKBone handEffector;

	private float targetWeight;

	private Vector3 targetPos;

	private State CurrentState;

	private float grabTime;

	private float holdTime = 20f;

	public HandIK(FullBodyBipedIK ik, Giantess giantess)
	{
		this.ik = ik;
		this.giantess = giantess;
		hand = ik.references.rightHand;
		if (giantess.ik == null)
		{
			Debug.LogError("No IK");
		}
		if (giantess.ik.rightHandEffector == null)
		{
			Debug.LogError("No hand ik effector");
		}
		handEffector = giantess.ik.rightHandEffector;
		targetWeight = 0f;
		ik.solver.leftArmMapping.weight = 0f;
		CurrentState = Idle;
	}

	public void GrabTarget(EntityBase entity)
	{
		if (giantess.Height < entity.Height * 5f)
		{
			giantess.animationManager.PlayAnimation("No 2");
			if (GlobalPreferences.ScriptAuxLogging.value)
			{
				Debug.Log("Target is too large to grab");
			}
		}
		else
		{
			target = entity;
			CurrentState = Grab;
		}
	}

	public void CancelGrab()
	{
		target = null;
		CurrentState = Return;
	}

	public void ReleaseTarget()
	{
		if (target != null)
		{
			target.transform.parent = null;
			target.Unlock();
		}
	}

	public bool GrabCompleted()
	{
		return !IsActive();
	}

	public void Update()
	{
		CurrentState();
		UpdateEffector();
	}

	public bool IsActive()
	{
		return CurrentState != new State(Idle);
	}

	private void UpdateEffector()
	{
		Vector3 position = Vector3.MoveTowards(handEffector.position, targetPos, Time.fixedDeltaTime * 25f);
		float num = Mathf.Lerp(handEffector.positionWeight, targetWeight, Time.fixedDeltaTime);
		handEffector.position = position;
		handEffector.positionWeight = num;
		ik.solver.rightArmMapping.weight = num;
	}

	private void Idle()
	{
		targetWeight = 0f;
		handEffector.position = hand.position;
	}

	private void Grab()
	{
		Humanoid humanoid = target as Humanoid;
		if (!target || ((bool)humanoid && humanoid.IsDead))
		{
			CurrentState = Return;
			return;
		}
		float num = 800f * giantess.Scale;
		targetWeight = 1f;
		targetPos = target.transform.position;
		float sqrMagnitude = (hand.position - targetPos).sqrMagnitude;
		float num2 = giantess.AccurateScale * 0.1f;
		float num3 = num2 * num2;
		if (sqrMagnitude < num3)
		{
			target.transform.SetParent(hand);
			target.Lock();
			CurrentState = MoveUp;
			grabTime = Time.time;
		}
		else if ((new Vector3(giantess.transform.position.x, Mathf.Clamp(targetPos.y, giantess.transform.position.y, giantess.transform.position.y + giantess.meshHeight * (giantess.Scale / 2f)), giantess.transform.position.z) - targetPos).sqrMagnitude > num * num)
		{
			CurrentState = Return;
		}
	}

	private void MoveUp()
	{
		Humanoid humanoid = target as Humanoid;
		if (!target || ((bool)humanoid && humanoid.IsDead))
		{
			CurrentState = Return;
			return;
		}
		targetPos = giantess.transform.TransformPoint(new Vector3(100f, 1400f, 300f));
		if (Time.time > grabTime + holdTime || !target.locked)
		{
			CurrentState = Return;
		}
	}

	private void Return()
	{
		targetWeight = 0f;
		targetPos = hand.transform.position;
		if (handEffector.positionWeight < 0.05f)
		{
			CurrentState = Idle;
		}
	}
}
