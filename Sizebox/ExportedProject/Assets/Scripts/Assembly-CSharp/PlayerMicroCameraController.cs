using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerMicroCameraController : BaseCameraController, IListener
{
	[SerializeField]
	private NewPlayerMicroController movementController;

	private const float DefaultHeight = 1.35f;

	public AnimatedMicroNPC targetMicro;

	[SerializeField]
	private Vector3 aimingOffset = Vector3.zero;

	[SerializeField]
	private Vector3 firstPersonRaygunOffset = Vector3.zero;

	private bool _isAiming;

	protected override void Awake()
	{
		base.Awake();
		collisionLayer = Layers.microCameraCollisionMask;
	}

	private void Start()
	{
		EventManager.Register(this, EventCode.OnStep);
	}

	protected override void OnLookBackCancel(InputAction.CallbackContext obj)
	{
		if (!_isAiming)
		{
			base.OnLookBackCancel(obj);
		}
	}

	protected override void OnLookBackStart(InputAction.CallbackContext obj)
	{
		if (!_isAiming)
		{
			base.OnLookBackStart(obj);
		}
	}

	internal override void OnZoomCancel(InputAction.CallbackContext obj)
	{
		if (!_isAiming)
		{
			base.OnZoomCancel(obj);
		}
	}

	internal override void OnZoomPerformed(InputAction.CallbackContext obj)
	{
		if (!_isAiming)
		{
			base.OnZoomPerformed(obj);
		}
	}

	internal override void OnCameraPress(InputAction.CallbackContext obj)
	{
		base.OnCameraPress(obj);
		int num = LayerMask.NameToLayer("Player");
		PositionRaygun();
		if (base.FirstPersonMode)
		{
			nearCamera.cullingMask &= ~(1 << num);
			heightModifier = 1.25f;
		}
		else
		{
			nearCamera.cullingMask |= 1 << num;
		}
	}

	public override void SetTarget(Transform newTarget)
	{
		if (!newTarget || !newTarget.GetComponent<AnimatedMicroNPC>())
		{
			DisconnectInputActions();
			target = null;
			targetMicro = null;
			base.enabled = false;
		}
		else
		{
			ConnectInputActions();
			target = newTarget;
			targetMicro = newTarget.GetComponent<AnimatedMicroNPC>();
			base.enabled = true;
			base.FirstPersonMode = false;
			collisionLayer = Layers.microCameraCollisionMask;
		}
	}

	public override void DoLateUpdate()
	{
		if (GameController.playerInputEnabled && !GameController.Instance.paused && !(target == null))
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
		if (targetMicro != null)
		{
			targetScale = targetMicro.AccurateScale;
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
			Quaternion quaternion = Quaternion.LookRotation(movementController.ReferenceTransform.forward, movementController.ReferenceTransform.transform.up);
			float num2 = 1.35f;
			Vector3 b = Vector3.up;
			if (targetMicro != null && targetMicro.isGiantess)
			{
				num2 = 0.9f;
			}
			else if (movementController.IsClimbing)
			{
				Transform transform = target.transform;
				b = ((!base.FirstPersonMode) ? (0.35f * transform.up + 0.05f * transform.forward) : (0.35f * transform.up + 0.4f * transform.forward));
			}
			else if (targetMicro.isCrushed)
			{
				Transform transform2 = target.transform;
				b = (base.FirstPersonMode ? (0.1f * transform2.up - 0.45f * transform2.forward) : ((!targetMicro.IsStuck()) ? (0.1f * transform2.up - 0f * transform2.forward) : (0.2f * transform2.up - 0f * transform2.forward)));
			}
			num2 *= heightModifier;
			position.up = Vector3.Lerp(position.up, b, Time.deltaTime * 4f);
			position.verticalOffset = Mathf.Lerp(position.verticalOffset, num2, Time.deltaTime * 4f);
			targetPos = target.position + position.up * (position.verticalOffset * targetScale);
			if (base.FirstPersonMode && targetMicro != null && targetMicro.isGiantess)
			{
				Vector3 forward = base.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				targetPos = targetMicro.GetEyesPosition() + forward * (forwardView * targetScale);
			}
			Quaternion quaternion2 = quaternion;
			Quaternion quaternion3 = Quaternion.Euler(xRotation, num, 0f);
			float num3 = 0f - position.distanceFromTarget;
			if (base.FirstPersonMode)
			{
				num3 = 0.02f * targetScale;
			}
			destination = quaternion2 * (quaternion3 * Vector3.forward) * num3;
			destination += targetPos;
			parentTransform.position = destination;
			float adjustementDistance = position.adjustementDistance;
			if (!base.FirstPersonMode && adjustementDistance < num3)
			{
				num3 = adjustementDistance;
				adjustedDestination = quaternion2 * (quaternion3 * Vector3.forward) * num3;
				adjustedDestination += targetPos;
				parentTransform.position = adjustedDestination;
			}
			if (_isAiming && !base.FirstPersonMode)
			{
				nearCamera.transform.localPosition = aimingOffset;
			}
			else
			{
				nearCamera.transform.localPosition = Vector3.zero;
			}
		}
	}

	private new void LookAtTarget()
	{
		Quaternion rotation = Quaternion.LookRotation(targetPos - parentTransform.position, movementController.ReferenceTransform.up);
		parentTransform.rotation = rotation;
		if (GameController.VrMode)
		{
			Vector3 eulerAngles = parentTransform.eulerAngles;
			eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
			parentTransform.eulerAngles = eulerAngles;
		}
	}

	private new void ZoomInOnTarget()
	{
		position.targetZoom *= 1f - zoomInput * position.zoomSmooth * Time.deltaTime;
		position.targetZoom = Mathf.Clamp(position.targetZoom, position.maxZoom, position.minZoom);
		float distanceFromTarget = position.distanceFromTarget;
		position.distanceFromTarget = Mathf.Lerp(distanceFromTarget, position.targetZoom * targetScale, position.smooth * Time.deltaTime);
		position.adjustementDistance *= position.distanceFromTarget / distanceFromTarget;
	}

	private new void UpdateFieldOfView()
	{
		targetFov = BaseCameraController.DefaultFieldOfView;
		float num = 2f;
		if (!base.FirstPersonMode)
		{
			if (movementController.IsSprinting)
			{
				targetFov += sprintFieldOfView;
			}
			else if (orbit.xRotation > 0f)
			{
				num = orbit.xRotation / orbit.maxXRotation * 20f;
				if (num < 2f)
				{
					num = 2f;
				}
				targetFov += orbit.xRotation / orbit.maxXRotation * lookDownFieldOfView;
			}
		}
		if (targetFov > 110f)
		{
			targetFov = 110f;
		}
		float fieldOfView = Mathf.Lerp(nearCamera.fieldOfView, targetFov, Time.deltaTime * num);
		if (!XRSettings.enabled)
		{
			nearCamera.fieldOfView = fieldOfView;
			farCamera.fieldOfView = fieldOfView;
		}
	}

	public void StartAiming()
	{
		_isAiming = true;
		PositionRaygun();
	}

	public void StopAiming()
	{
		_isAiming = false;
	}

	private void PositionRaygun()
	{
		if (_isAiming)
		{
			if (base.FirstPersonMode)
			{
				movementController.SetRaygunFirstPersonPosition(firstPersonRaygunOffset * targetMicro.Scale);
			}
			else
			{
				movementController.ResetRaygunPosition();
			}
		}
	}
}
