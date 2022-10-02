using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SimCam : MonoBehaviour
{
	private enum SimCamMode
	{
		Basic = 0,
		Advanced = 1
	}

	private bool _enable;

	private Camera _mainCamera;

	private EventSystem _eventSystem;

	private float _zoomSpeed = 0.3f;

	private float _zoomPower;

	public float zoomSmooth = 5f;

	public float rotationSmooth = 0.125f;

	public float orbitSmooth = 16f;

	public float zoomShift = 1.2f;

	private float _shiftMultiplier = 5f;

	private float _movementSpeed = 1f;

	private float _currentSpeed;

	private float _baseSpeed;

	private Vector3 _movementDirection;

	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private float cameraDistanceSmoothing = 5f;

	private GameController _gameController;

	private SimCamMode _mode;

	private float _cameraDistance = 1f;

	private bool _edgeScroll;

	private bool _shift;

	private float _zoomInput;

	private float _forwardMovement;

	private float _rightMovement;

	private float _upMovement;

	private Vector2 _mousePositionInScreen;

	private Vector3 _centerPoint;

	private bool _rightClick;

	private bool _middleClick;

	private int borderThickness = 3;

	private Quaternion _myRotation;

	private Quaternion _targetRotation;

	private Vector2 _mouseDelta;

	private Vector2 _pointerPos;

	private Inputs.EditModeActions _editModeActions;

	private void OnGameModeChanged(GameMode mode)
	{
		if (mode == GameMode.Edit || mode == GameMode.FreeCam)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}

	private void Enable()
	{
		_enable = true;
		_zoomPower = 0f;
		_movementDirection = Vector3.zero;
		if ((bool)cameraTransform)
		{
			_cameraDistance = cameraTransform.localScale.y;
		}
		GetCameraDistance();
		_baseSpeed = _movementSpeed * _cameraDistance;
		_edgeScroll = GlobalPreferences.CameraEdgeScroll.value;
	}

	private void Disable()
	{
		_enable = false;
	}

	private void Awake()
	{
		_mode = ((!GlobalPreferences.CameraRtsMode.value) ? SimCamMode.Advanced : SimCamMode.Basic);
		_gameController = GameController.Instance;
		GameController gameController = _gameController;
		gameController.onModeChange = (UnityAction<GameMode>)Delegate.Combine(gameController.onModeChange, new UnityAction<GameMode>(OnGameModeChanged));
		_mainCamera = Camera.main;
		_edgeScroll = GlobalPreferences.CameraEdgeScroll.value;
		_eventSystem = EventSystem.current;
		_editModeActions = InputManager.inputs.EditMode;
		_editModeActions.Zoom.performed += OnSpeedChange;
	}

	private void OnSpeedChange(InputAction.CallbackContext obj)
	{
		float num = obj.ReadValue<float>() * 2.5f;
		_baseSpeed *= 1f * num * Time.deltaTime;
	}

	private void OnDestroy()
	{
		_editModeActions.Zoom.performed -= OnSpeedChange;
	}

	private void Update()
	{
		if (_enable && !GameController.Instance.paused && !StateManager.Keyboard.userIsTyping)
		{
			GetInput();
			UpdateCenterPoint();
			GetCameraDistance();
			UpdateCameraScale();
		}
	}

	private void LateUpdate()
	{
		if (_enable && !GameController.Instance.paused && !(Mathf.Abs(Time.timeScale) < Mathf.Epsilon))
		{
			if (_mode == SimCamMode.Basic)
			{
				BasicMode();
			}
			else
			{
				AdvancedMode();
			}
			Zoom();
			MouseLook();
		}
	}

	private void BasicMode()
	{
		if (!OrbitPoint(_centerPoint))
		{
			MoveBasic();
		}
	}

	private void AdvancedMode()
	{
		if (!OrbitPoint(_centerPoint))
		{
			MoveAdvanced();
		}
	}

	private void GetInput()
	{
		_mousePositionInScreen = Pointer.current.position.ReadValue();
		_rightClick = Mouse.current.rightButton.isPressed;
		_middleClick = Mouse.current.middleButton.isPressed;
		_mouseDelta = _editModeActions.Look.ReadValue<Vector2>();
		_shift = Mathf.Abs(_editModeActions.MoveFaster.ReadValue<float>()) > Mathf.Epsilon;
		_zoomInput = _editModeActions.Zoom.ReadValue<float>();
		Vector2 vector = _editModeActions.MoveCamera.ReadValue<Vector2>();
		_forwardMovement = vector.y;
		_rightMovement = vector.x;
		if (_eventSystem.IsPointerOverGameObject() || MouseOutOfScreen())
		{
			_zoomInput = 0f;
		}
		if (_edgeScroll && !_rightClick && _mode == SimCamMode.Basic && Screen.fullScreen)
		{
			if (Math.Abs(_forwardMovement) < Mathf.Epsilon)
			{
				if (_mousePositionInScreen.y < (float)borderThickness)
				{
					_forwardMovement = -1f;
				}
				if (_mousePositionInScreen.y > (float)(Screen.height - borderThickness))
				{
					_forwardMovement = 1f;
				}
			}
			if (Math.Abs(_rightMovement) < Mathf.Epsilon)
			{
				if (_mousePositionInScreen.x < (float)borderThickness)
				{
					_rightMovement = -1f;
				}
				if (_mousePositionInScreen.x > (float)(Screen.width - borderThickness))
				{
					_rightMovement = 1f;
				}
			}
		}
		_upMovement = InputManager.inputs.EditMode.MoveCameraUpDown.ReadValue<float>();
		if (_editModeActions.ChangeCameraMode.triggered)
		{
			_mode = ((_mode == SimCamMode.Basic) ? SimCamMode.Advanced : SimCamMode.Basic);
			GlobalPreferences.CameraRtsMode.value = _mode == SimCamMode.Basic;
		}
	}

	private bool MouseOutOfScreen()
	{
		Vector2 vector = Input.mousePosition;
		if (vector.x > (float)Screen.width || vector.x < 0f || vector.y > (float)Screen.height || vector.y < 0f)
		{
			return true;
		}
		return false;
	}

	private void MoveCamera(Vector3 forward, Vector3 up, Vector3 right)
	{
		if (Mathf.Abs(_forwardMovement) > Mathf.Epsilon || Mathf.Abs(_rightMovement) > Mathf.Epsilon || Mathf.Abs(_upMovement) > Mathf.Epsilon)
		{
			_movementDirection = _forwardMovement * forward + _rightMovement * right + _upMovement * up;
			_movementDirection.Normalize();
			_movementDirection *= _currentSpeed;
			Vector3 vector = _movementDirection * Time.deltaTime;
			cameraTransform.position += vector;
		}
	}

	private bool OrbitPoint(Vector3 point)
	{
		Vector3 forward = cameraTransform.position - point;
		Vector3 eulerAngles = Quaternion.LookRotation(forward).eulerAngles;
		if (!_middleClick)
		{
			_myRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
			_targetRotation = _myRotation;
			return false;
		}
		float x = _targetRotation.eulerAngles.x + _mouseDelta.y;
		float y = _targetRotation.eulerAngles.y + _mouseDelta.x;
		x = ClampAngle(x);
		_targetRotation = Quaternion.Euler(x, y, 0f);
		_myRotation = Quaternion.Slerp(_myRotation, _targetRotation, orbitSmooth * Time.deltaTime);
		forward = _myRotation * (Vector3.forward * forward.magnitude);
		cameraTransform.position = forward + point;
		LookAt(point);
		return true;
	}

	private void LookAt(Vector3 point)
	{
		Vector3 forward = point - cameraTransform.position;
		cameraTransform.rotation = Quaternion.LookRotation(forward);
		if (GameController.VrMode)
		{
			Vector3 eulerAngles = cameraTransform.eulerAngles;
			eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
			cameraTransform.eulerAngles = eulerAngles;
		}
	}

	private void MoveBasic()
	{
		Vector3 forward = cameraTransform.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 right = cameraTransform.right;
		right.y = 0f;
		right.Normalize();
		_baseSpeed = _movementSpeed * _cameraDistance;
		_currentSpeed = _baseSpeed;
		if (_shift)
		{
			_currentSpeed *= _shiftMultiplier;
		}
		MoveCamera(forward, Vector3.up, right);
	}

	private void MoveAdvanced()
	{
		_currentSpeed = _baseSpeed;
		if (_shift)
		{
			_currentSpeed *= _shiftMultiplier;
		}
		MoveCamera(cameraTransform.forward, cameraTransform.up, cameraTransform.right);
	}

	private void UpdateCenterPoint()
	{
		if (!_middleClick)
		{
			Vector3 pos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
			RaycastHit hitInfo;
			if (Physics.Raycast(_mainCamera.ScreenPointToRay(pos), out hitInfo, float.PositiveInfinity))
			{
				_centerPoint = hitInfo.point;
			}
		}
	}

	private void GetCameraDistance()
	{
		if (!cameraTransform)
		{
			_cameraDistance = 2f;
			return;
		}
		RaycastHit hitInfo;
		float num = (Physics.Raycast(cameraTransform.position, Vector3.down, out hitInfo, 10000f) ? hitInfo.distance : ((!(_cameraDistance < 2f)) ? _cameraDistance : 2f));
		if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hitInfo, _cameraDistance) && hitInfo.distance < num)
		{
			num = hitInfo.distance;
		}
		num = Mathf.Clamp(num, 0.001f, 5000f);
		_cameraDistance = Mathf.Lerp(_cameraDistance, num, Time.deltaTime * cameraDistanceSmoothing);
		_baseSpeed = _movementSpeed * _cameraDistance;
	}

	private void UpdateCameraScale()
	{
		cameraTransform.localScale = Vector3.one * (_cameraDistance * 0.5f);
	}

	private void Zoom()
	{
		if (Math.Abs(_zoomInput) > Mathf.Epsilon)
		{
			_zoomPower = Mathf.Clamp(_zoomInput, -0.1f, 0.1f);
			if (_shift)
			{
				_zoomPower *= zoomShift;
			}
		}
		if (Math.Abs(_zoomPower) < Mathf.Epsilon)
		{
			return;
		}
		Vector3 vector = _centerPoint - cameraTransform.position;
		float magnitude = vector.magnitude;
		float num = _zoomPower * _zoomSpeed * magnitude;
		_zoomPower = Mathf.Lerp(_zoomPower, 0f, zoomSmooth * Time.deltaTime);
		if (magnitude - num > 0f)
		{
			vector = vector.normalized * num;
			if (_shift)
			{
				vector *= _shiftMultiplier;
			}
			cameraTransform.position += vector;
		}
	}

	private void MouseLook()
	{
		if ((_rightClick || _gameController.mode == GameMode.FreeCam) && !_middleClick)
		{
			Vector3 localEulerAngles = cameraTransform.localEulerAngles;
			float x = localEulerAngles.x - _mouseDelta.y * rotationSmooth;
			x = ClampAngle(x);
			float y = localEulerAngles.y + _mouseDelta.x * rotationSmooth;
			cameraTransform.rotation = Quaternion.Euler(x, y, 0f);
		}
	}

	private float ClampAngle(float x)
	{
		if (x > 180f)
		{
			x -= 360f;
		}
		x = Mathf.Clamp(x, -89f, 89f);
		return x;
	}
}
