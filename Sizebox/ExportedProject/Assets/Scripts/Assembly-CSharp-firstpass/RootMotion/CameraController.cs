using System;
using UnityEngine;

namespace RootMotion
{
	public class CameraController : MonoBehaviour
	{
		[Serializable]
		public enum UpdateMode
		{
			Update = 0,
			FixedUpdate = 1,
			LateUpdate = 2,
			FixedLateUpdate = 3
		}

		public Transform target;

		public Transform rotationSpace;

		public UpdateMode updateMode = UpdateMode.LateUpdate;

		public bool lockCursor = true;

		[Header("Position")]
		public bool smoothFollow;

		public Vector3 offset = new Vector3(0f, 1.5f, 0.5f);

		public float followSpeed = 10f;

		[Header("Rotation")]
		public float rotationSensitivity = 3.5f;

		public float yMinLimit = -20f;

		public float yMaxLimit = 80f;

		public bool rotateAlways = true;

		public bool rotateOnLeftButton;

		public bool rotateOnRightButton;

		public bool rotateOnMiddleButton;

		[Header("Distance")]
		public float distance = 10f;

		public float minDistance = 4f;

		public float maxDistance = 10f;

		public float zoomSpeed = 10f;

		public float zoomSensitivity = 1f;

		[Header("Blocking")]
		public LayerMask blockingLayers;

		public float blockingRadius = 1f;

		public float blockingSmoothTime = 0.1f;

		[Range(0f, 1f)]
		public float blockedOffset = 0.5f;

		private Vector3 targetDistance;

		private Vector3 position;

		private Quaternion rotation = Quaternion.identity;

		private Vector3 smoothPosition;

		private Camera cam;

		private bool fixedFrame;

		private float fixedDeltaTime;

		private Quaternion r = Quaternion.identity;

		private Vector3 lastUp;

		private float blockedDistance = 10f;

		private float blockedDistanceV;

		public float x { get; private set; }

		public float y { get; private set; }

		public float distanceTarget { get; private set; }

		private float zoomAdd
		{
			get
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis > 0f)
				{
					return 0f - zoomSensitivity;
				}
				if (axis < 0f)
				{
					return zoomSensitivity;
				}
				return 0f;
			}
		}

		protected virtual void Awake()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			x = eulerAngles.y;
			y = eulerAngles.x;
			distanceTarget = distance;
			smoothPosition = base.transform.position;
			cam = GetComponent<Camera>();
			lastUp = ((rotationSpace != null) ? rotationSpace.up : Vector3.up);
		}

		protected virtual void Update()
		{
			if (updateMode == UpdateMode.Update)
			{
				UpdateTransform();
			}
		}

		protected virtual void FixedUpdate()
		{
			fixedFrame = true;
			fixedDeltaTime += Time.deltaTime;
			if (updateMode == UpdateMode.FixedUpdate)
			{
				UpdateTransform();
			}
		}

		protected virtual void LateUpdate()
		{
			UpdateInput();
			if (updateMode == UpdateMode.LateUpdate)
			{
				UpdateTransform();
			}
			if (updateMode == UpdateMode.FixedLateUpdate && fixedFrame)
			{
				UpdateTransform(fixedDeltaTime);
				fixedDeltaTime = 0f;
				fixedFrame = false;
			}
		}

		public void UpdateInput()
		{
			if (cam.enabled)
			{
				Cursor.lockState = (lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
				Cursor.visible = ((!lockCursor) ? true : false);
				if (rotateAlways || (rotateOnLeftButton && Input.GetMouseButton(0)) || (rotateOnRightButton && Input.GetMouseButton(1)) || (rotateOnMiddleButton && Input.GetMouseButton(2)))
				{
					x += Input.GetAxis("Mouse X") * rotationSensitivity;
					y = ClampAngle(y - Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);
				}
				distanceTarget = Mathf.Clamp(distanceTarget + zoomAdd, minDistance, maxDistance);
			}
		}

		public void UpdateTransform()
		{
			UpdateTransform(Time.deltaTime);
		}

		public void UpdateTransform(float deltaTime)
		{
			if (!cam.enabled)
			{
				return;
			}
			rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
			if (rotationSpace != null)
			{
				r = Quaternion.FromToRotation(lastUp, rotationSpace.up) * r;
				rotation = r * rotation;
				lastUp = rotationSpace.up;
			}
			if (target != null)
			{
				distance += (distanceTarget - distance) * zoomSpeed * deltaTime;
				if (!smoothFollow)
				{
					smoothPosition = target.position;
				}
				else
				{
					smoothPosition = Vector3.Lerp(smoothPosition, target.position, deltaTime * followSpeed);
				}
				Vector3 vector = smoothPosition + rotation * offset;
				Vector3 vector2 = rotation * -Vector3.forward;
				if ((int)blockingLayers != -1)
				{
					RaycastHit hitInfo;
					if (Physics.SphereCast(vector, blockingRadius, vector2, out hitInfo, distanceTarget - blockingRadius, blockingLayers))
					{
						blockedDistance = Mathf.SmoothDamp(blockedDistance, hitInfo.distance + blockingRadius * (1f - blockedOffset), ref blockedDistanceV, blockingSmoothTime);
					}
					else
					{
						blockedDistance = distanceTarget;
					}
					distance = Mathf.Min(distance, blockedDistance);
				}
				position = vector + vector2 * distance;
				base.transform.position = position;
			}
			base.transform.rotation = rotation;
		}

		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
