using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class BaseCameraController : MonoBehaviour
{
	[Serializable]
	public class PositionSettings
	{
		public Vector3 up = Vector3.up;

		public Vector3 forward = Vector3.forward;

		public float verticalOffset = 1.5f;

		public float targetZoom = -2.5f;

		public float distanceFromTarget = -2.5f;

		public float zoomSmooth = 0.1f;

		public float minZoom = -2f;

		public float maxZoom = -200f;

		public float smooth = 2f;

		[HideInInspector]
		public float adjustementDistance = 9999f;
	}

	[Serializable]
	public class OrbitSettings
	{
		public float xRotation = -20f;

		public float yRotation = -180f;

		public float maxXRotation = 85f;

		public float minXRotation = -85f;
	}

	protected Transform target;

	protected Camera nearCamera;

	protected Camera farCamera;

	protected CameraEffectsSettings camEffectSettingsScript;

	public float smoothReferenceRotation = 5f;

	public float forwardView = 0.14f;

	[FormerlySerializedAs("_colliding")]
	public bool colliding;

	public PositionSettings position = new PositionSettings();

	public OrbitSettings orbit = new OrbitSettings();

	protected Vector3 targetPos = Vector3.zero;

	protected Vector3 destination = Vector3.zero;

	protected Vector3 adjustedDestination = Vector3.zero;

	protected float hOrbitInput;

	protected float vOrbitInput;

	protected float zoomInput;

	[FormerlySerializedAs("_heightModifier")]
	public float heightModifier = 1f;

	public float targetScale = 1f;

	[FormerlySerializedAs("lookDownFOV")]
	public float lookDownFieldOfView = 15f;

	[FormerlySerializedAs("sprintFOV")]
	public float sprintFieldOfView = 25f;

	protected bool middleMouseClick;

	protected float targetFov;

	protected bool inputsConnected;

	public float shakeStrength;

	public ResizeCharacter resizeChar;

	protected LayerMask collisionLayer;

	protected Transform parentTransform;

	protected Quaternion appliedReferenceRotation = Quaternion.identity;

	public bool FirstPersonMode { get; protected set; }

	protected static float DefaultFieldOfView
	{
		get
		{
			return GlobalPreferences.Fov.value;
		}
	}

	protected virtual void Awake()
	{
		nearCamera = Camera.main;
		farCamera = nearCamera.GetComponentsInChildren<Camera>()[1];
		base.enabled = false;
		parentTransform = nearCamera.transform.parent;
		camEffectSettingsScript = nearCamera.GetComponent<CameraEffectsSettings>();
	}

	protected void ConnectInputActions()
	{
		if (!inputsConnected)
		{
			Inputs.PlayerActions player = InputManager.inputs.Player;
			player.ChangeCamera.performed += OnCameraPress;
			player.Zoom.canceled += OnZoomCancel;
			player.Zoom.performed += OnZoomPerformed;
			player.Look.canceled += OnLookCancel;
			player.Look.performed += OnLookPerformed;
			player.LookBack.canceled += OnLookBackCancel;
			player.LookBack.started += OnLookBackStart;
			inputsConnected = true;
		}
	}

	protected void DisconnectInputActions()
	{
		Inputs.PlayerActions player = InputManager.inputs.Player;
		player.ChangeCamera.performed -= OnCameraPress;
		player.Zoom.canceled -= OnZoomCancel;
		player.Zoom.performed -= OnZoomPerformed;
		player.Look.canceled -= OnLookCancel;
		player.Look.performed -= OnLookPerformed;
		player.LookBack.canceled -= OnLookBackCancel;
		player.LookBack.started -= OnLookBackStart;
		inputsConnected = false;
	}

	private void OnDestroy()
	{
		DisconnectInputActions();
	}

	protected virtual void OnLookBackCancel(InputAction.CallbackContext obj)
	{
		middleMouseClick = false;
	}

	protected virtual void OnLookBackStart(InputAction.CallbackContext obj)
	{
		middleMouseClick = true;
	}

	private void OnLookCancel(InputAction.CallbackContext obj)
	{
		hOrbitInput = (vOrbitInput = 0f);
	}

	private void OnLookPerformed(InputAction.CallbackContext obj)
	{
		float timeScale = Time.timeScale;
		Vector2 vector = obj.ReadValue<Vector2>();
		hOrbitInput = vector.x * timeScale;
		vOrbitInput = vector.y * timeScale;
	}

	internal virtual void OnZoomCancel(InputAction.CallbackContext obj)
	{
		zoomInput = 0f;
	}

	internal virtual void OnZoomPerformed(InputAction.CallbackContext obj)
	{
		float value = obj.ReadValue<float>();
		value = Mathf.Clamp(value, -0.1f, 0.1f);
		if (!StateManager.Keyboard.Shift)
		{
			zoomInput += value;
			return;
		}
		heightModifier += value;
		heightModifier = Mathf.Clamp(heightModifier, 0.1f, 1.9f);
	}

	internal virtual void OnCameraPress(InputAction.CallbackContext obj)
	{
		FirstPersonMode = !FirstPersonMode;
	}

	public void OnNotify(IEvent @event)
	{
		StepEvent stepEvent = (StepEvent)@event;
		if (!stepEvent.entity.isMicro && (bool)GlobalPreferences.CameraShakeEnabled)
		{
			float num = Mathf.Log10(stepEvent.entity.Height / parentTransform.localScale.y * stepEvent.magnitude);
			float num2 = stepEvent.entity.Height * num;
			if (!(num < 0f))
			{
				float magnitude = (stepEvent.position - base.transform.position).magnitude;
				magnitude = 1f - magnitude / num2;
				magnitude = Mathf.Clamp01(magnitude);
				CameraShake(num * magnitude);
			}
		}
	}

	protected void CameraShakeEffect()
	{
		float num = GlobalPreferences.CameraShakeMultiplier;
		if (shakeStrength > 0f)
		{
			Vector3 vector = UnityEngine.Random.onUnitSphere * shakeStrength;
			vector.z = 0f;
			vector *= targetScale;
			Vector3 vector2 = parentTransform.position;
			vector2 = Vector3.Lerp(vector2, vector2 + vector, 0.5f * Time.deltaTime);
			parentTransform.position = vector2;
			shakeStrength -= num * Time.deltaTime;
		}
	}

	private void CameraShake(float intensity)
	{
		float num = intensity * (float)GlobalPreferences.CameraShakeMultiplier;
		if (num > shakeStrength)
		{
			shakeStrength = num;
		}
	}

	internal void UpdateFieldOfView()
	{
		targetFov = DefaultFieldOfView;
		float num = 2f;
		if (!FirstPersonMode && orbit.xRotation > 0f)
		{
			num = orbit.xRotation / orbit.maxXRotation * 20f;
			if (num < 2f)
			{
				num = 2f;
			}
			targetFov += orbit.xRotation / orbit.maxXRotation * lookDownFieldOfView;
		}
		if (targetFov > 110f)
		{
			targetFov = 110f;
		}
		float fieldOfView = Mathf.Lerp(nearCamera.fieldOfView, targetFov, Time.deltaTime * num);
		nearCamera.fieldOfView = fieldOfView;
		farCamera.fieldOfView = fieldOfView;
	}

	internal void LookAtTarget()
	{
		Quaternion rotation = Quaternion.LookRotation(targetPos - parentTransform.position);
		parentTransform.rotation = rotation;
		if (GameController.VrMode)
		{
			Vector3 eulerAngles = parentTransform.eulerAngles;
			eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
			parentTransform.eulerAngles = eulerAngles;
		}
	}

	internal void OrbitTarget()
	{
		if (GameController.VrMode)
		{
			orbit.xRotation = 0f;
		}
		else
		{
			orbit.xRotation += vOrbitInput;
		}
		orbit.yRotation += hOrbitInput;
		orbit.xRotation = Mathf.Clamp(orbit.xRotation, orbit.minXRotation, orbit.maxXRotation);
	}

	internal void ZoomInOnTarget()
	{
		position.targetZoom *= 1f - zoomInput * position.zoomSmooth * Time.deltaTime;
		position.targetZoom = Mathf.Clamp(position.targetZoom, position.maxZoom, position.minZoom);
		float distanceFromTarget = position.distanceFromTarget;
		position.distanceFromTarget = Mathf.Lerp(distanceFromTarget, position.targetZoom * targetScale, position.smooth * Time.deltaTime);
		position.adjustementDistance *= position.distanceFromTarget / distanceFromTarget;
	}

	protected float WhiskerCollisionCheck(Vector3 direction, float xRotation, float yRotation)
	{
		direction = Quaternion.LookRotation(direction) * Quaternion.Euler(xRotation, yRotation, 0f) * Vector3.forward * direction.magnitude;
		RaycastHit hitInfo;
		if (Physics.Raycast(targetPos, direction, out hitInfo, direction.magnitude * 1.5f, collisionLayer))
		{
			Debug.DrawLine(targetPos, targetPos + direction, Color.red);
			return hitInfo.distance;
		}
		Debug.DrawLine(targetPos, targetPos + direction, Color.green);
		return 0f - position.distanceFromTarget;
	}

	protected void CollisionDetection()
	{
		Vector3 vector = destination - targetPos;
		float num = 0f - position.distanceFromTarget;
		Debug.DrawLine(targetPos, targetPos + vector, Color.green);
		RaycastHit hitInfo;
		colliding = Physics.Raycast(targetPos, vector, out hitInfo, vector.magnitude * 1.5f, collisionLayer);
		float num2 = hitInfo.distance * 0.9f;
		float num3 = 15f;
		float num4 = WhiskerCollisionCheck(vector, 0f - num3, 0f);
		float num5 = WhiskerCollisionCheck(vector, num3, 0f);
		float num6 = WhiskerCollisionCheck(vector, 0f, 0f - num3);
		float num7 = WhiskerCollisionCheck(vector, 0f, num3);
		float num8;
		float num9 = (num8 = Mathf.Min(num4, num5, num6, num7, num));
		float num10 = 0.5f;
		if (colliding && num2 < num8)
		{
			num8 = num2;
			num10 = 16f;
		}
		if (num9 < position.adjustementDistance)
		{
			num10 = 8f;
		}
		num8 *= 0.9f;
		position.adjustementDistance = Mathf.Lerp(position.adjustementDistance, num8, num10 * Time.deltaTime);
	}

	public abstract void SetTarget(Transform target);

	public abstract void DoLateUpdate();
}
