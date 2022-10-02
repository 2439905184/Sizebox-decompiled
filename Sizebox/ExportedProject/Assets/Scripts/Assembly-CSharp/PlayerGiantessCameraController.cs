using UnityEngine;

public class PlayerGiantessCameraController : BaseCameraController, IListener
{
	public Giantess targetGiantess;

	protected override void Awake()
	{
		base.Awake();
		collisionLayer = Layers.giantessCameraCollisionMask;
	}

	private void Start()
	{
		EventManager.Register(this, EventCode.OnStep);
	}

	public override void SetTarget(Transform newTarget)
	{
		if (!newTarget || !newTarget.GetComponent<Giantess>())
		{
			DisconnectInputActions();
			target = null;
			targetGiantess = null;
			base.enabled = false;
		}
		else
		{
			ConnectInputActions();
			target = newTarget;
			targetGiantess = newTarget.GetComponent<Giantess>();
			base.enabled = true;
		}
	}

	public override void DoLateUpdate()
	{
		if (GameController.playerInputEnabled && !GameController.Instance.paused && (bool)target)
		{
			AdjustToPlayerSize();
			UpdateFieldOfView();
			OrbitTarget();
			MoveToTarget();
			LookAtTarget();
			ZoomInOnTarget();
			CollisionDetection();
			CameraShakeEffect();
		}
	}

	private void AdjustToPlayerSize()
	{
		if ((bool)targetGiantess)
		{
			targetScale = targetGiantess.AccurateScale;
			parentTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
			nearCamera.nearClipPlane = camEffectSettingsScript.defaultNearPlane * targetScale;
		}
	}

	private void MoveToTarget()
	{
		if (!(target == null))
		{
			float xRotation = orbit.xRotation;
			float num = orbit.yRotation;
			if (middleMouseClick)
			{
				num += 180f;
			}
			Quaternion identity = Quaternion.identity;
			Vector3 up = Vector3.up;
			float b = 0.9f * heightModifier;
			position.up = Vector3.Lerp(position.up, up, Time.deltaTime * 4f);
			position.verticalOffset = Mathf.Lerp(position.verticalOffset, b, Time.deltaTime * 4f);
			targetPos = target.position + position.up * (position.verticalOffset * targetScale);
			if (base.FirstPersonMode)
			{
				Vector3 forward = nearCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				targetPos = targetGiantess.GetEyesPosition() + forward * (forwardView * targetScale);
			}
			appliedReferenceRotation = Quaternion.Slerp(appliedReferenceRotation, identity, Time.deltaTime * smoothReferenceRotation);
			Quaternion quaternion = appliedReferenceRotation;
			Quaternion quaternion2 = Quaternion.Euler(xRotation, num, 0f);
			float num2 = 0f - position.distanceFromTarget;
			if (base.FirstPersonMode)
			{
				num2 = 0.02f * targetScale;
			}
			destination = quaternion * (quaternion2 * Vector3.forward) * num2;
			destination += targetPos;
			parentTransform.position = destination;
			float adjustementDistance = position.adjustementDistance;
			if (!base.FirstPersonMode && adjustementDistance < num2)
			{
				num2 = adjustementDistance;
				adjustedDestination = quaternion * (quaternion2 * Vector3.forward) * num2;
				adjustedDestination += targetPos;
				parentTransform.position = adjustedDestination;
			}
		}
	}
}
