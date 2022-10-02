using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[Serializable]
	[AddComponentMenu("XR/Tracked Pose Driver (New Input System)")]
	public class TrackedPoseDriver : MonoBehaviour
	{
		public enum TrackingType
		{
			RotationAndPosition = 0,
			RotationOnly = 1,
			PositionOnly = 2
		}

		public enum UpdateType
		{
			UpdateAndBeforeRender = 0,
			Update = 1,
			BeforeRender = 2
		}

		[SerializeField]
		private TrackingType m_TrackingType;

		[SerializeField]
		private UpdateType m_UpdateType;

		[SerializeField]
		private InputAction m_PositionAction;

		[SerializeField]
		private InputAction m_RotationAction;

		private Vector3 m_CurrentPosition = Vector3.zero;

		private Quaternion m_CurrentRotation = Quaternion.identity;

		private bool m_RotationBound;

		private bool m_PositionBound;

		public TrackingType trackingType
		{
			get
			{
				return m_TrackingType;
			}
			set
			{
				m_TrackingType = value;
			}
		}

		public UpdateType updateType
		{
			get
			{
				return m_UpdateType;
			}
			set
			{
				m_UpdateType = value;
			}
		}

		public InputAction positionAction
		{
			get
			{
				return m_PositionAction;
			}
			set
			{
				UnbindPosition();
				m_PositionAction = value;
				BindActions();
			}
		}

		public InputAction rotationAction
		{
			get
			{
				return m_RotationAction;
			}
			set
			{
				UnbindRotation();
				m_RotationAction = value;
				BindActions();
			}
		}

		private void BindActions()
		{
			BindPosition();
			BindRotation();
		}

		private void BindPosition()
		{
			if (!m_PositionBound && m_PositionAction != null)
			{
				m_PositionAction.Rename(base.gameObject.name + " - TPD - Position");
				m_PositionAction.performed += OnPositionUpdate;
				m_PositionBound = true;
				m_PositionAction.Enable();
			}
		}

		private void BindRotation()
		{
			if (!m_RotationBound && m_RotationAction != null)
			{
				m_RotationAction.Rename(base.gameObject.name + " - TPD - Rotation");
				m_RotationAction.performed += OnRotationUpdate;
				m_RotationBound = true;
				m_RotationAction.Enable();
			}
		}

		private void UnbindActions()
		{
			UnbindPosition();
			UnbindRotation();
		}

		private void UnbindPosition()
		{
			if (m_PositionAction != null && m_PositionBound)
			{
				m_PositionAction.Disable();
				m_PositionAction.performed -= OnPositionUpdate;
				m_PositionBound = false;
			}
		}

		private void UnbindRotation()
		{
			if (m_RotationAction != null && m_RotationBound)
			{
				m_RotationAction.Disable();
				m_RotationAction.performed -= OnRotationUpdate;
				m_RotationBound = false;
			}
		}

		private void OnPositionUpdate(InputAction.CallbackContext context)
		{
			m_CurrentPosition = context.ReadValue<Vector3>();
		}

		private void OnRotationUpdate(InputAction.CallbackContext context)
		{
			m_CurrentRotation = context.ReadValue<Quaternion>();
		}

		protected virtual void Awake()
		{
			if (HasStereoCamera())
			{
				XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), true);
			}
		}

		protected void OnEnable()
		{
			InputSystem.onAfterUpdate += UpdateCallback;
			BindActions();
		}

		private void OnDisable()
		{
			UnbindActions();
			InputSystem.onAfterUpdate -= UpdateCallback;
		}

		protected virtual void OnDestroy()
		{
			if (HasStereoCamera())
			{
				XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), false);
			}
		}

		protected void UpdateCallback()
		{
			if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
			{
				OnBeforeRender();
			}
			else
			{
				OnUpdate();
			}
		}

		protected virtual void OnUpdate()
		{
			if (m_UpdateType == UpdateType.Update || m_UpdateType == UpdateType.UpdateAndBeforeRender)
			{
				PerformUpdate();
			}
		}

		protected virtual void OnBeforeRender()
		{
			if (m_UpdateType == UpdateType.BeforeRender || m_UpdateType == UpdateType.UpdateAndBeforeRender)
			{
				PerformUpdate();
			}
		}

		protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
		{
			if (m_TrackingType == TrackingType.RotationAndPosition || m_TrackingType == TrackingType.RotationOnly)
			{
				base.transform.localRotation = newRotation;
			}
			if (m_TrackingType == TrackingType.RotationAndPosition || m_TrackingType == TrackingType.PositionOnly)
			{
				base.transform.localPosition = newPosition;
			}
		}

		private bool HasStereoCamera()
		{
			Camera component = GetComponent<Camera>();
			if (component != null)
			{
				return component.stereoEnabled;
			}
			return false;
		}

		protected virtual void PerformUpdate()
		{
			SetLocalTransform(m_CurrentPosition, m_CurrentRotation);
		}
	}
}
