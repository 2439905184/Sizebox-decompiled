using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGiantessController : BaseMovementController
{
	[SerializeField]
	private GameObject targetAreaPrefab;

	private Giantess _giantess;

	public float sprintMultiplier = 4.5f;

	public float turnSpeed = 10f;

	public float speedChange = 5f;

	private float _currentInput;

	private bool _isPreparingStomp;

	private bool _isStomping;

	private bool _leftFootWasCloser;

	private int _verticalHash;

	private AnimationManager _animManager;

	private GameObject _targetArea;

	private readonly LayerMask _aimStompMask = Layers.stompingMask;

	private Camera _cam;

	protected override void Awake()
	{
		_cam = Camera.main;
		_targetArea = Object.Instantiate(targetAreaPrefab, base.transform, true);
		_targetArea.SetActive(false);
		_verticalHash = Animator.StringToHash("verticalInput");
		spawnMicroAction = InputManager.inputs.Misc.SpawnMicro;
		base.Awake();
	}

	private void OnEnable()
	{
		StateManager.Keyboard.Sync();
	}

	protected override void ConnectInputActions()
	{
		if (!inputsConnected)
		{
			base.ConnectInputActions();
			Inputs.MacroActions macro = InputManager.inputs.Macro;
			macro.Stomp.performed += OnStompPerformed;
			InputManager.Instance.EnableControls(macro.Get(), GameMode.Play);
		}
	}

	protected override void DisconnectInputActions()
	{
		base.DisconnectInputActions();
		Inputs.MacroActions macro = InputManager.inputs.Macro;
		macro.Stomp.performed -= OnStompPerformed;
		InputManager.Instance.DisableControls(macro.Get(), GameMode.Play);
	}

	internal override void OnMovePerformed(InputAction.CallbackContext obj)
	{
		if (!_isPreparingStomp && !_isStomping)
		{
			base.OnMovePerformed(obj);
		}
	}

	private void OnStompPerformed(InputAction.CallbackContext obj)
	{
	}

	public override void SetTarget(Transform newTarget)
	{
		if (!newTarget || !newTarget.GetComponent<Giantess>())
		{
			DisconnectInputActions();
			target = null;
			_giantess = null;
			base.enabled = false;
		}
		else
		{
			ConnectInputActions();
			_giantess = newTarget.GetComponent<Giantess>();
			target = newTarget;
			base.enabled = true;
			_animManager = _giantess.animationManager;
		}
	}

	private void OnDisable()
	{
		_targetArea.SetActive(false);
		StateManager.Keyboard.Sync();
	}

	public override void DoUpdate()
	{
		ReadInput();
		if (horizontalInput != 0f || verticalInput != 0f)
		{
			_giantess.ai.DisableAI();
		}
	}

	public override void DoFixedUpdate()
	{
		Move();
	}

	private void ReadInput()
	{
		if (!GameController.playerInputEnabled)
		{
			horizontalInput = 0f;
			verticalInput = 0f;
			if (_isPreparingStomp)
			{
				_isPreparingStomp = false;
				_targetArea.SetActive(false);
			}
			if (_isStomping)
			{
				AimStompUpdate(false);
			}
		}
		else if (!_isStomping && !_isPreparingStomp)
		{
			if (InputManager.inputs.Macro.Stomp.triggered && _giantess.ai.IsIdle())
			{
				SetupAimStomp();
			}
		}
		else
		{
			AimStompUpdate(true);
		}
	}

	private void Move()
	{
		if (autoWalk)
		{
			verticalInput = 1f;
		}
		float currentSpeed = _animManager.GetCurrentSpeed();
		float num = Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput));
		if (sprint)
		{
			num *= sprintMultiplier;
		}
		_animManager.UpdateAnimationSpeed();
		_currentInput = Mathf.Lerp(_currentInput, num, speedChange * Time.fixedDeltaTime);
		_giantess.Animator.SetFloat(_verticalHash, _currentInput);
		if (!(horizontalInput * horizontalInput < 0.01f) || !(verticalInput * verticalInput < 0.01f))
		{
			if (_giantess.gtsMovement.doNotMoveGts)
			{
				_giantess.gtsMovement.doNotMoveGts = false;
			}
			if (_giantess.Movement.move)
			{
				_giantess.ai.behaviorController.StopMainBehavior();
			}
			if (_giantess.ik.initialized && _giantess.ik.hand.IsActive())
			{
				_giantess.ik.hand.CancelGrab();
			}
			if (_giantess.IsPosed)
			{
				_giantess.SetPoseMode(false);
			}
			if (_giantess.Animator.runtimeAnimatorController != IOManager.Instance.gtsPlayerAnimatorController)
			{
				_giantess.Animator.SetRuntimeController(IOManager.Instance.gtsPlayerAnimatorController);
				_giantess.Movement.Stop();
				_giantess.Movement.move = true;
			}
			Vector3 b = GetCameraForward() * verticalInput + GetCameraRight() * horizontalInput;
			b.Normalize();
			float num2 = _giantess.unscaledWalkSpeed * currentSpeed;
			if (sprint)
			{
				num2 *= sprintMultiplier;
			}
			b = Vector3.Slerp(_giantess.transform.forward, b, turnSpeed * Time.fixedDeltaTime * currentSpeed);
			_giantess.Rigidbody.position += b * (_giantess.AccurateScale * num2);
			_giantess.Rigidbody.rotation = Quaternion.LookRotation(b);
		}
	}

	private void SetupAimStomp()
	{
		horizontalInput = 0f;
		verticalInput = 0f;
		_leftFootWasCloser = false;
		_targetArea.SetActive(true);
		Transform transform = _giantess.transform;
		Vector3 position = transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(position + new Vector3(0f, 100f, 0f), Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
		{
			_targetArea.transform.position = new Vector3(position.x, hitInfo.point.y + 1f, position.z);
		}
		else
		{
			_targetArea.transform.position = _giantess.transform.position + new Vector3(0f, 10f, 0f);
		}
		_targetArea.transform.localEulerAngles = _giantess.transform.localEulerAngles;
		_targetArea.transform.localScale = transform.lossyScale * 160f;
		_isPreparingStomp = true;
	}

	private void AimStompUpdate(bool updateInput)
	{
		if (_isPreparingStomp)
		{
			if (InputManager.inputs.Macro.Stomp.triggered)
			{
				_isPreparingStomp = false;
				_isStomping = true;
				_targetArea.transform.localScale *= 0.6f;
				_giantess.ik.FootIk.CrushManualTarget(_targetArea.transform);
			}
			else if (InputManager.inputs.Macro.Cancel.triggered)
			{
				_isPreparingStomp = false;
				_targetArea.SetActive(false);
			}
		}
		else if (_isStomping)
		{
			if (_giantess.ik.FootIk.FootState == FootIK.FootStates.Crush && _targetArea.activeSelf)
			{
				_targetArea.transform.localScale /= 0.6f;
				_targetArea.SetActive(false);
			}
			if (_giantess.ik.FootIk.CrushEnded)
			{
				_isStomping = false;
				_targetArea.SetActive(false);
			}
		}
		if (!_targetArea.activeSelf)
		{
			return;
		}
		Vector3 vector2;
		if (updateInput)
		{
			Vector2 vector = InputManager.inputs.Player.Move.ReadValue<Vector2>();
			vector2 = GetCameraForward() * vector.y + GetCameraRight() * vector.x;
			vector2.Normalize();
		}
		else
		{
			vector2 = Vector3.zero;
		}
		Vector3 vector3 = _targetArea.transform.position + vector2 * _giantess.AccurateScale / (_isPreparingStomp ? 60f : 300f);
		if (!_giantess.ik.FootIk.IsClose(vector3))
		{
			Vector3 position = _targetArea.transform.position;
			vector2 = (_giantess.transform.position - position) * (0.1f * Time.deltaTime);
			vector3 = position + vector2;
		}
		if (_giantess.ik.FootIk.IsCloserToLeftFoot(vector3))
		{
			if (!_leftFootWasCloser)
			{
				_targetArea.transform.GetChild(1).gameObject.SetActive(false);
				_targetArea.transform.GetChild(2).gameObject.SetActive(true);
				_leftFootWasCloser = true;
			}
		}
		else if (_leftFootWasCloser)
		{
			_targetArea.transform.GetChild(2).gameObject.SetActive(false);
			_targetArea.transform.GetChild(1).gameObject.SetActive(true);
			_leftFootWasCloser = false;
		}
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(vector3, Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
		{
			_targetArea.transform.position = hitInfo.point + new Vector3(0f, 0.5f, 0f);
			Vector3 normal = hitInfo.normal;
			int num = 1;
			if (Physics.Raycast(new Ray(vector3 + new Vector3(_giantess.AccurateScale, 10f, _giantess.AccurateScale), Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal += hitInfo.normal;
				num++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(_giantess.AccurateScale, 10f, 0f - _giantess.AccurateScale), Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal += hitInfo.normal;
				num++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(0f - _giantess.AccurateScale, 10f, _giantess.AccurateScale), Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal += hitInfo.normal;
				num++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(0f - _giantess.AccurateScale, 10f, 0f - _giantess.AccurateScale), Vector3.down), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal += hitInfo.normal;
				num++;
			}
			normal = normal * 90f / num;
			_targetArea.transform.eulerAngles = new Vector3(normal.x, _giantess.transform.localEulerAngles.y, normal.z);
		}
		else if (Physics.Raycast(new Ray(vector3, Vector3.up), out hitInfo, float.PositiveInfinity, _aimStompMask))
		{
			_targetArea.transform.position = hitInfo.point + new Vector3(0f, 0.5f, 0f);
			Vector3 normal2 = hitInfo.normal;
			int num2 = 1;
			if (Physics.Raycast(new Ray(vector3 + new Vector3(_giantess.AccurateScale, -10f, _giantess.AccurateScale), Vector3.up), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal2 += hitInfo.normal;
				num2++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(_giantess.AccurateScale, -10f, 0f - _giantess.AccurateScale), Vector3.up), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal2 += hitInfo.normal;
				num2++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(0f - _giantess.AccurateScale, -10f, _giantess.AccurateScale), Vector3.up), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal2 += hitInfo.normal;
				num2++;
			}
			if (Physics.Raycast(new Ray(vector3 + new Vector3(0f - _giantess.AccurateScale, -10f, 0f - _giantess.AccurateScale), Vector3.up), out hitInfo, float.PositiveInfinity, _aimStompMask))
			{
				normal2 += hitInfo.normal;
				num2++;
			}
			normal2 = normal2 * -90f / num2;
			_targetArea.transform.eulerAngles = new Vector3(normal2.x, _giantess.transform.localEulerAngles.y, normal2.z);
		}
	}

	private Vector3 GetCameraForward()
	{
		Vector3 forward = _cam.transform.forward;
		forward.y = 0f;
		return forward.normalized;
	}

	private Vector3 GetCameraRight()
	{
		Vector3 right = _cam.transform.right;
		right.y = 0f;
		return right.normalized;
	}
}
