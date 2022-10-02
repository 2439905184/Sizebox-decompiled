using System;
using System.Collections;
using System.Runtime.CompilerServices;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NewPlayerMicroController : BaseMovementController
{
	[IsReadOnly]
	private struct ClimbingRaycastInfo
	{
		public readonly bool isValid;

		public readonly Vector3 averagePosition;

		public readonly Vector3 averageNormal;

		public ClimbingRaycastInfo(Vector3 pos, Vector3 norm)
		{
			isValid = true;
			averagePosition = pos;
			averageNormal = norm;
		}
	}

	private const float DefaultRaygunScale = 0.08f;

	private const float BaseFlyingMass = 25f;

	[Header("Required References")]
	[SerializeField]
	private PlayerMicroSettings settings;

	[SerializeField]
	private PhysicMaterial playerPhysicsMaterial;

	public PlayerIK playerIk;

	public PlayerMicroCameraController cameraController;

	public PlayerRaygun raygun;

	[SerializeField]
	private GameObject bloodSpritePrefab;

	public Transform aimPoint;

	public LayerMask aimingMask;

	private Vector3 _previousVelocity = Vector3.zero;

	private PlayerMicroControllerState _state;

	private AnimatedMicroNPC _micro;

	private PlayerMicroDestruction _microDestruction;

	private PlayerFlyingDestruction _flyingDestruction;

	private Camera _camera;

	private AudioSource _audioSource;

	private AudioSource _flySource;

	private AudioSource _raygunArmingSource;

	private Coroutine _aimSetup;

	private PhysicMaterial _cachedMaterial;

	private Vector3 _cameraForward;

	private Vector3 _cameraRight;

	private Vector3 _cameraForwardOnPlane;

	private Quaternion _cameraRotationOnPlane;

	private float _turnAngle;

	private bool _jump;

	private bool _isHoldingJump;

	private bool _isHoldingFlyDown;

	private bool _isSuperFlying;

	private bool _isHoldingFlyingPunch;

	private float _timeToRecover;

	private float _timeToNextJump;

	private float _timeToAnimatorReset;

	private float _timeToSplatReset;

	private int _speedFloat;

	private int _velocityFloat;

	private int _jumpBool;

	private int _hFloat;

	private int _vFloat;

	private int _turnFloat;

	private int _flyBool;

	private int _startFlyingBool;

	private int _groundedBool;

	private int _climbingBool;

	private int _startClimbingBool;

	private int _aimBool;

	private int _splatBool;

	private int _rollBool;

	private int _fallingFloat;

	private int _fallBackwardsBool;

	private int _fallForwardsBool;

	private int _hardLandingBool;

	private int _flyingPunchBool;

	private bool _isImmobilized;

	private bool _isVelocityLocked;

	private bool _isGravityLocked;

	private Vector3 _lockedVelocity;

	private bool _isAimingLocked;

	private bool _inAir;

	private float _previousYPosition;

	private float _currentFlySpeed;

	private bool _preparingPunch;

	private readonly Vector3 _previousGravity = Vector3.zero;

	private Vector3 _previousClimbDirection = Vector3.forward;

	[Range(0.05f, 2.5f)]
	[SerializeField]
	private float rayForwardOffset = 0.3f;

	[Range(0.05f, 2.5f)]
	[SerializeField]
	private float rayUpOffset = 0.3f;

	[SerializeField]
	private float rayLength = 1.25f;

	[SerializeField]
	private float rayArcRangeUpper = 0.35f;

	[SerializeField]
	private float rayArcRangeLower = -0.35f;

	private readonly RaycastHit[] _climbingRaycastBuffer = new RaycastHit[16];

	private bool _isOnLeft;

	public PlayerMicroSettings Settings
	{
		get
		{
			return settings;
		}
	}

	public Transform ReferenceTransform { get; private set; }

	public bool IsMoving { get; private set; }

	public bool IsSprinting { get; private set; }

	public bool IsClimbing
	{
		get
		{
			return _state == PlayerMicroControllerState.Climbing;
		}
	}

	public bool IsFlying
	{
		get
		{
			return _state == PlayerMicroControllerState.Flying;
		}
	}

	public bool IsAiming { get; private set; }

	private bool SlowWalk { get; set; }

	public Vector3 Velocity
	{
		get
		{
			return _previousVelocity;
		}
	}

	protected override void Awake()
	{
		_speedFloat = Animator.StringToHash("Speed");
		_velocityFloat = Animator.StringToHash("Velocity");
		_jumpBool = Animator.StringToHash("Jump");
		_hFloat = Animator.StringToHash("H");
		_vFloat = Animator.StringToHash("V");
		_climbingBool = Animator.StringToHash("Climbing");
		_startClimbingBool = Animator.StringToHash("StartClimbing");
		_turnFloat = Animator.StringToHash("Turn");
		_aimBool = Animator.StringToHash("Aiming");
		_flyBool = Animator.StringToHash("Fly");
		_startFlyingBool = Animator.StringToHash("StartFlying");
		_groundedBool = Animator.StringToHash("Grounded");
		_splatBool = Animator.StringToHash("Splat");
		_rollBool = Animator.StringToHash("Roll");
		_fallingFloat = Animator.StringToHash("Falling");
		_fallBackwardsBool = Animator.StringToHash("FallBackwards");
		_fallForwardsBool = Animator.StringToHash("FallForwards");
		_hardLandingBool = Animator.StringToHash("HardLanding");
		_flyingPunchBool = Animator.StringToHash("FlyingPunch");
		aimPoint.parent = null;
		_audioSource = base.gameObject.AddComponent<AudioSource>();
		_audioSource.outputAudioMixerGroup = SoundManager.AudioMixerMicro;
		_flySource = base.gameObject.AddComponent<AudioSource>();
		_flySource.outputAudioMixerGroup = SoundManager.AudioMixerWindAmbient;
		_flySource.loop = true;
		_flySource.spatialBlend = 0f;
		_flySource.dopplerLevel = 0f;
		_flySource.volume = 0f;
		SoundManager.SetAndPlaySoundClip(_flySource, SoundManager.Instance.flySound);
		_raygunArmingSource = base.gameObject.AddComponent<AudioSource>();
		_raygunArmingSource.outputAudioMixerGroup = SoundManager.AudioMixerRaygun;
		_raygunArmingSource.loop = false;
		_raygunArmingSource.spatialBlend = 0f;
		_raygunArmingSource.dopplerLevel = 0f;
		_raygunArmingSource.volume = 0f;
		SoundManager.SetAndPlaySoundClip(_raygunArmingSource, SoundManager.Instance.playerRaygunArmingSound);
		_camera = Camera.main;
		ReferenceTransform = new GameObject("Reference Transform", typeof(CameraReference)).transform;
	}

	public override void SetTarget(Transform newTarget)
	{
		if (_aimSetup != null)
		{
			StopCoroutine(_aimSetup);
		}
		if ((bool)_micro)
		{
			_micro.gameObject.SetLayerRecursively(Layers.microLayer);
			UnityEngine.Object.Destroy(_micro.model.GetComponent<PlayerMicroFootsteps>());
			UnityEngine.Object.Destroy(_micro.model.GetComponent<AimIK>());
			_micro.GetComponentInChildren<RootMotionTransfer>().enabled = true;
			_micro.GetComponent<Collider>().sharedMaterial = _cachedMaterial;
			UnityEngine.Object.Destroy(_microDestruction);
			UnityEngine.Object.Destroy(_flyingDestruction);
		}
		if (!newTarget || !newTarget.GetComponent<AnimatedMicroNPC>())
		{
			DisconnectInputActions();
			DisableAim();
			StopAllCoroutines();
			target = null;
			_micro = null;
			playerIk.enabled = false;
			base.enabled = false;
			return;
		}
		ConnectInputActions();
		target = newTarget;
		_micro = newTarget.GetComponent<AnimatedMicroNPC>();
		_micro.gameObject.SetLayerRecursively(Layers.playerLayer);
		_micro.model.AddComponent<PlayerMicroFootsteps>();
		Collider component = _micro.GetComponent<Collider>();
		_cachedMaterial = component.sharedMaterial;
		component.sharedMaterial = playerPhysicsMaterial;
		_microDestruction = _micro.gameObject.AddComponent<PlayerMicroDestruction>();
		_flyingDestruction = _micro.gameObject.AddComponent<PlayerFlyingDestruction>();
		_flyingDestruction.AssignController(this);
		_flyingDestruction.SetDestructionForceMultiplier(0f);
		_micro.GetComponentInChildren<RootMotionTransfer>().enabled = false;
		playerIk.Initialize(_micro, this);
		playerIk.enabled = true;
		SetState(PlayerMicroControllerState.Standing);
		base.enabled = true;
	}

	private void OnEnable()
	{
		StateManager.Keyboard.Sync();
	}

	private void OnDisable()
	{
		StateManager.Keyboard.Sync();
	}

	protected override void ConnectInputActions()
	{
		if (!inputsConnected)
		{
			base.ConnectInputActions();
			Inputs.MicroActions micro = InputManager.inputs.Micro;
			micro.Interact.performed += OnInteract;
			micro.Walk.performed += OnWalkPerformed;
			micro.Jump.performed += OnJumpStart;
			micro.Jump.canceled += OnJumpCancel;
			micro.Crouch.performed += OnCrouchPerformed;
			micro.Fly.performed += OnFlyPerformed;
			micro.FlyDown.started += OnFlyDownStart;
			micro.FlyDown.canceled += OnFlyDownCancel;
			micro.SuperFly.performed += OnSuperFly;
			micro.FlyingPunch.started += OnFlyingPunchStart;
			micro.FlyingPunch.canceled += OnFlyingPunchEnd;
			micro.WeaponAim.performed += OnAimPerformed;
			micro.WeaponFire.canceled += OnFireCancel;
			micro.WeaponFire.started += OnFireStart;
			InputManager.Instance.EnableControls(micro, GameMode.Play);
		}
	}

	protected override void DisconnectInputActions()
	{
		base.DisconnectInputActions();
		Inputs.MicroActions micro = InputManager.inputs.Micro;
		micro.Interact.performed -= OnInteract;
		micro.Walk.performed -= OnWalkPerformed;
		micro.Jump.performed -= OnJumpStart;
		micro.Jump.canceled -= OnJumpCancel;
		micro.Crouch.performed -= OnCrouchPerformed;
		micro.Fly.performed -= OnFlyPerformed;
		micro.FlyDown.started -= OnFlyDownStart;
		micro.FlyDown.canceled -= OnFlyDownCancel;
		micro.SuperFly.performed -= OnSuperFly;
		micro.FlyingPunch.started -= OnFlyingPunchStart;
		micro.FlyingPunch.canceled -= OnFlyingPunchEnd;
		micro.WeaponAim.performed -= OnAimPerformed;
		micro.WeaponFire.canceled -= OnFireCancel;
		micro.WeaponFire.started -= OnFireStart;
		InputManager.Instance.DisableControls(micro, GameMode.Play);
	}

	private void OnAimPerformed(InputAction.CallbackContext obj)
	{
		if (!_isImmobilized)
		{
			if (IsAiming)
			{
				DisableAim();
			}
			else
			{
				SetupAim();
			}
		}
	}

	private void OnDestroy()
	{
		DisconnectInputActions();
	}

	private void OnFireStart(InputAction.CallbackContext obj)
	{
		if (_micro.isActiveAndEnabled && GameController.playerInputEnabled && IsAiming)
		{
			raygun.StartFiring();
		}
	}

	private void OnFireCancel(InputAction.CallbackContext obj)
	{
		if (_micro.isActiveAndEnabled && GameController.playerInputEnabled && IsAiming)
		{
			raygun.StopFiring();
		}
	}

	private void OnWalkPerformed(InputAction.CallbackContext obj)
	{
		SlowWalk = !SlowWalk;
	}

	protected override void OnSprintCancelled(InputAction.CallbackContext obj)
	{
		IsSprinting = false;
	}

	protected override void OnSprintPerformed(InputAction.CallbackContext obj)
	{
		IsSprinting = true;
	}

	private void OnJumpCancel(InputAction.CallbackContext obj)
	{
		_isHoldingJump = false;
	}

	private void OnJumpStart(InputAction.CallbackContext obj)
	{
		if (_micro.RagDollEnabled)
		{
			_micro.DisableRagDoll();
			return;
		}
		_isHoldingJump = true;
		if (_micro.isCrushed)
		{
			_micro.StandUp();
		}
		else
		{
			_jump = true;
		}
	}

	private void OnCrouchPerformed(InputAction.CallbackContext obj)
	{
		if (!IsFlying)
		{
			SetState((_state != PlayerMicroControllerState.Climbing) ? PlayerMicroControllerState.Climbing : PlayerMicroControllerState.Standing);
		}
	}

	private void OnFlyPerformed(InputAction.CallbackContext obj)
	{
		if (_micro.RagDollEnabled)
		{
			_micro.DisableRagDoll();
		}
		SetState((_state != PlayerMicroControllerState.Flying) ? PlayerMicroControllerState.Flying : PlayerMicroControllerState.Standing);
	}

	private void OnFlyDownStart(InputAction.CallbackContext obj)
	{
		_isHoldingFlyDown = true;
	}

	private void OnFlyDownCancel(InputAction.CallbackContext obj)
	{
		_isHoldingFlyDown = false;
	}

	private void OnSuperFly(InputAction.CallbackContext obj)
	{
		_isSuperFlying = !_isSuperFlying;
	}

	private void OnFlyingPunchStart(InputAction.CallbackContext obj)
	{
		_isHoldingFlyingPunch = true;
	}

	private void OnFlyingPunchEnd(InputAction.CallbackContext obj)
	{
		_isHoldingFlyingPunch = false;
	}

	private void OnInteract(InputAction.CallbackContext obj)
	{
		if (_micro.gameObject.activeInHierarchy)
		{
			Interact();
		}
	}

	protected override IEnumerator SpawnMicro()
	{
		while ((bool)target)
		{
			bool female;
			bool male;
			GetMicroSpawnButtonState(out female, out male);
			if (female || male)
			{
				float num = target.lossyScale.x;
				if (!StateManager.Keyboard.Shift)
				{
					num *= GlobalPreferences.MicroSpawnSize.value;
				}
				num = Mathf.Clamp(num, MapSettingInternal.minPlayerSize, MapSettingInternal.maxPlayerSize);
				MicroGender gender = ((!female) ? MicroGender.Male : MicroGender.Female);
				AssetDescription randomMicroAsset = AssetManager.Instance.GetRandomMicroAsset(gender);
				if (randomMicroAsset != null)
				{
					GameController.LocalClient.SpawnMicro(randomMicroAsset, num);
				}
				yield return BaseMovementController.MicroSpawnInterval;
				continue;
			}
			break;
		}
	}

	public override void DoUpdate()
	{
		if (!_micro || _micro.RagDollEnabled)
		{
			DisableAim();
			return;
		}
		TakeInput();
		UpdateReference();
		UpdateRigidbodySettings();
		UpdateAiming();
	}

	private void UpdateAiming()
	{
		if (IsAiming && GameController.playerInputEnabled)
		{
			if (cameraController.FirstPersonMode)
			{
				HandleFirstPersonAiming();
			}
			else
			{
				HandleThirdPersonAiming();
			}
		}
	}

	private void TakeInput()
	{
		if ((bool)_micro && (_micro.isCrushed || _micro.IsDead))
		{
			IsMoving = false;
			horizontalInput = (verticalInput = 0f);
			return;
		}
		_cameraForward = _camera.transform.TransformDirection(Vector3.forward);
		_cameraRight = _camera.transform.TransformDirection(Vector3.right);
		_cameraForwardOnPlane = _cameraForward.Flatten().normalized;
		_cameraRotationOnPlane = Quaternion.LookRotation(_cameraForwardOnPlane);
		if (autoWalk)
		{
			verticalInput = 1f;
		}
		IsMoving = (double)Mathf.Abs(horizontalInput) > 0.1 || (double)Mathf.Abs(verticalInput) > 0.1;
	}

	private void UpdateReference()
	{
		if (ReferenceTransform.parent != base.transform.parent)
		{
			ReferenceTransform.SetParent(base.transform.parent);
		}
		Vector3 vector = (IsClimbing ? _micro.transform.up : Vector3.up);
		Quaternion b = Quaternion.LookRotation(Vector3.ProjectOnPlane(ReferenceTransform.forward, vector), vector);
		ReferenceTransform.rotation = Quaternion.Slerp(ReferenceTransform.rotation, b, Time.deltaTime * 4f);
	}

	private void UpdateRigidbodySettings()
	{
		if ((bool)_micro.transform.parent && _micro.Rigidbody.interpolation == RigidbodyInterpolation.Interpolate)
		{
			_micro.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}
		else if (_micro.transform.parent == null && _micro.Rigidbody.interpolation == RigidbodyInterpolation.None)
		{
			_micro.Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		}
		if (IsFlying)
		{
			_micro.Rigidbody.mass = 25f * _micro.Scale * Velocity.magnitude;
		}
	}

	public override void DoLateUpdate()
	{
		if ((bool)_micro && !_micro.RagDollEnabled)
		{
			TakeEquipmentInput();
		}
	}

	private void TakeEquipmentInput()
	{
		if (_micro.isActiveAndEnabled && GameController.playerInputEnabled && IsAiming)
		{
			if (Input.GetButtonDown(ButtonInput.Fire))
			{
				raygun.StartFiring();
			}
			else if (Input.GetButtonUp(ButtonInput.Fire))
			{
				raygun.StopFiring();
			}
			if (Input.GetButtonUp(ButtonInput.SwitchFiringMode))
			{
				raygun.SwitchFiringMode();
			}
		}
	}

	public override void DoFixedUpdate()
	{
		FixedUpdateState();
		if (_isVelocityLocked)
		{
			float y = _micro.Rigidbody.velocity.y;
			Vector3 lockedVelocity = _lockedVelocity;
			if (!_isGravityLocked)
			{
				lockedVelocity.y = y;
			}
			_micro.Rigidbody.velocity = lockedVelocity;
		}
	}

	private void FixedUpdateState()
	{
		if ((bool)_micro && !_micro.RagDollEnabled)
		{
			if ((_state != PlayerMicroControllerState.AIState && _micro.Animator.runtimeAnimatorController != IOManager.Instance.playerAnimatorController) || _micro.ai.IsEnabled())
			{
				SetState(PlayerMicroControllerState.AIState);
			}
			switch (_state)
			{
			case PlayerMicroControllerState.Standing:
				StandingState();
				break;
			case PlayerMicroControllerState.Flying:
				FlyingState();
				break;
			case PlayerMicroControllerState.Climbing:
				ClimbingState();
				break;
			case PlayerMicroControllerState.AIState:
				AiState();
				break;
			}
		}
	}

	private void SetState(PlayerMicroControllerState newState)
	{
		if (!_isImmobilized && _state != newState)
		{
			CleanupState();
			_state = newState;
			PrepareState(newState);
		}
	}

	private void CleanupState()
	{
		switch (_state)
		{
		case PlayerMicroControllerState.Standing:
			_inAir = false;
			_jump = false;
			break;
		case PlayerMicroControllerState.Flying:
			_micro.Gravity.enabled = true;
			_micro.Animator.SetBool(_flyBool, false);
			_micro.Animator.SetBool(_startFlyingBool, false);
			DisableFlySound();
			ReOrientateImmediately();
			_micro.ChangeScale(_micro.Scale);
			break;
		case PlayerMicroControllerState.Climbing:
			_micro.Animator.SetBool(_climbingBool, false);
			_micro.Animator.SetFloat(_speedFloat, 0.1f);
			_micro.Capsule.height = 2f;
			_micro.Capsule.center = Vector3.up;
			_micro.Gravity.enabled = true;
			break;
		case PlayerMicroControllerState.AIState:
			_micro.ai.behaviorController.StopMainBehavior();
			_micro.ai.actionController.ClearAll();
			_micro.ai.DisableAI();
			_micro.Animator.SetRuntimeController(IOManager.Instance.playerAnimatorController);
			_micro.GetComponentInChildren<RootMotionTransfer>().enabled = false;
			break;
		}
	}

	private void PrepareState(PlayerMicroControllerState state)
	{
		switch (state)
		{
		case PlayerMicroControllerState.Flying:
			_micro.Gravity.enabled = false;
			_isSuperFlying = false;
			_currentFlySpeed = _previousVelocity.magnitude;
			_micro.Animator.SetBool(_groundedBool, false);
			_micro.Animator.SetBool(_flyBool, true);
			_micro.Animator.SetBool(_startFlyingBool, true);
			DelayedExecute(0.5f, _003CPrepareState_003Eb__117_0);
			break;
		case PlayerMicroControllerState.Climbing:
			_micro.Capsule.height = 0.6f;
			_micro.Capsule.center = Vector3.up * 0.3f;
			_micro.Animator.SetBool(_climbingBool, true);
			_micro.Animator.SetBool(_startClimbingBool, true);
			DelayedExecute(0.5f, _003CPrepareState_003Eb__117_1);
			break;
		case PlayerMicroControllerState.AIState:
			_micro.GetComponentInChildren<RootMotionTransfer>().enabled = true;
			break;
		case PlayerMicroControllerState.Standing:
			break;
		}
	}

	public void ResetState()
	{
		SetState(PlayerMicroControllerState.Standing);
	}

	private void DelayedExecute(float delay, UnityAction action)
	{
		StartCoroutine(_DelayedExecute(delay, action));
	}

	private IEnumerator _DelayedExecute(float delay, UnityAction action)
	{
		yield return new WaitForSeconds(delay);
		action();
	}

	private void Immobilize(float length, UnityAction endCallback)
	{
		StartCoroutine(_ImmobilizeCoroutine(length, endCallback));
	}

	private IEnumerator _ImmobilizeCoroutine(float length, UnityAction callback)
	{
		_isImmobilized = true;
		yield return new WaitForSeconds(length);
		_isImmobilized = false;
		callback();
	}

	private void LockVelocity(Vector3 velocity, bool lockGravity)
	{
		_lockedVelocity = velocity;
		_isVelocityLocked = true;
		_isGravityLocked = lockGravity;
	}

	private void UnlockVelocity()
	{
		_isVelocityLocked = false;
		_isGravityLocked = false;
	}

	private void KillVelocity()
	{
		_micro.Rigidbody.velocity = Vector3.down * _micro.AccurateScale;
		_previousVelocity = Vector3.zero;
	}

	private void LockAiming(bool unEquipGun = false)
	{
		if (unEquipGun)
		{
			DisableAim();
		}
		_isAimingLocked = true;
	}

	private void UnlockAiming()
	{
		_isAimingLocked = false;
	}

	private void Interact()
	{
		if (_state == PlayerMicroControllerState.Standing)
		{
			TryEnterVehicle();
		}
	}

	private void TryEnterVehicle()
	{
		if (ObjectManager.Instance.vehicles.Count <= 0)
		{
			return;
		}
		Vehicle vehicle = null;
		float num = 7.5f * _micro.Scale;
		num *= num;
		foreach (Vehicle vehicle2 in ObjectManager.Instance.vehicles)
		{
			if ((bool)vehicle2)
			{
				float sqrMagnitude = (_micro.transform.position - vehicle2.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					vehicle = vehicle2;
					num = sqrMagnitude;
				}
			}
		}
		if (vehicle != null)
		{
			vehicle.EnterVehicle();
		}
	}

	private void StandingState()
	{
		bool flag = false;
		if (_timeToNextJump <= Time.time)
		{
			flag = _micro.IsOnSurface(Layers.walkableMask);
		}
		_micro.Animator.SetBool(_groundedBool, flag);
		if (_micro.Animator.runtimeAnimatorController != IOManager.Instance.playerAnimatorController || _micro.ai.IsEnabled())
		{
			SetState(PlayerMicroControllerState.AIState);
		}
		else if (!_isImmobilized)
		{
			MovementManagement(horizontalInput, verticalInput, IsSprinting, flag);
			HitGround(flag);
			JumpManagement(flag);
			_micro.Animator.SetFloat(_hFloat, horizontalInput);
			_micro.Animator.SetFloat(_vFloat, verticalInput);
			_micro.Animator.SetFloat(_turnFloat, 0f);
			_micro.Animator.SetFloat(_velocityFloat, _micro.Rigidbody.velocity.magnitude / _micro.AccurateScale);
		}
	}

	private void MovementManagement(float horizontal, float vertical, bool sprinting, bool isGrounded)
	{
		float num;
		if (IsMoving)
		{
			if (sprinting)
			{
				num = settings.sprintSpeed;
				_micro.Animator.SetFloat(_speedFloat, settings.sprintAnimationSpeed, settings.speedDampTime, Time.fixedDeltaTime);
			}
			else if (!SlowWalk)
			{
				num = settings.runSpeed;
				_micro.Animator.SetFloat(_speedFloat, settings.runAnimationSpeed, settings.speedDampTime, Time.fixedDeltaTime);
			}
			else
			{
				num = settings.walkSpeed;
				_micro.Animator.SetFloat(_speedFloat, settings.walkAnimationSpeed, settings.speedDampTime, Time.fixedDeltaTime);
			}
		}
		else
		{
			num = 0f;
			_micro.Animator.SetFloat(_speedFloat, 0f, settings.speedDampTime / 4f, Time.fixedDeltaTime);
		}
		if (cameraController.FirstPersonMode && IsMoving)
		{
			Vector3 forward = _cameraForwardOnPlane * vertical + _cameraRight * horizontal;
			if (!IsAiming)
			{
				_micro.Rigidbody.MoveRotation(Quaternion.LookRotation(forward));
			}
		}
		else
		{
			DoRotation(horizontal, vertical);
		}
		Rigidbody rigidbody = _micro.Rigidbody;
		Vector3 vector = new Vector3(horizontal, 0f, vertical);
		if (vector.sqrMagnitude > 1f)
		{
			vector = vector.normalized;
		}
		Vector3 velocity;
		if (isGrounded)
		{
			float num2 = vector.magnitude * num * settings.baseSpeed * _micro.Scale;
			Vector3 vector2 = _cameraRotationOnPlane * vector.normalized;
			float num3 = Vector3.Angle(vector2, _micro.transform.forward);
			num2 *= (180f - num3) / 180f * 0.8f + 0.2f;
			velocity = Vector3.Lerp(_previousVelocity, vector2 * num2, settings.movementSmoothing * Time.fixedDeltaTime);
			velocity = HandleGrounding(velocity);
		}
		else
		{
			velocity = rigidbody.velocity;
			float num4 = vector.magnitude * num * settings.baseSpeed * settings.airControlMult;
			Vector3 vector3 = _cameraRotationOnPlane * (vector.normalized * num4);
			velocity += vector3 * (_micro.Scale * Time.fixedDeltaTime);
		}
		_previousVelocity = velocity;
		_micro.Rigidbody.velocity = velocity;
	}

	private Vector3 HandleGrounding(Vector3 velocity)
	{
		if (_timeToNextJump > Time.time)
		{
			return velocity;
		}
		if (Physics.CheckSphere(base.transform.position, _micro.Scale * 0.15f, Layers.walkableMask))
		{
			Vector3 vector = _micro.Rigidbody.position + _micro.Rigidbody.rotation * (Vector3.up * (0.4f * _micro.Scale) + Vector3.forward * (0.4f * _micro.Scale));
			Debug.DrawLine(vector, vector + Vector3.down * _micro.Scale, Color.red);
			RaycastHit hitInfo;
			if (Physics.Raycast(layerMask: (_micro.AccurateScale <= 5.5f) ? Layers.walkableMask : Layers.gtsWalkableMask, origin: vector, direction: Vector3.down, hitInfo: out hitInfo, maxDistance: _micro.Scale * 0.75f))
			{
				float time = (hitInfo.point.y - base.transform.position.y) / _micro.Scale;
				velocity.y = settings.groundingCurve.Evaluate(time) * _micro.Scale;
			}
		}
		return velocity;
	}

	private void DoRotation(float horizontal, float vertical)
	{
		if (!IsAiming)
		{
			Vector3 cameraForwardOnPlane = _cameraForwardOnPlane;
			Vector3 vector = Vector3.Cross(Vector3.up, cameraForwardOnPlane);
			Vector3 normalized = (cameraForwardOnPlane * vertical + vector * horizontal).normalized;
			Vector3 vector2 = _micro.transform.InverseTransformDirection(normalized);
			_turnAngle = Mathf.Lerp(_turnAngle, Mathf.Atan2(vector2.x, vector2.z), Time.deltaTime * 2f);
			_micro.Animator.SetFloat(_turnFloat, _turnAngle);
			float turnSmoothing = settings.turnSmoothing;
			if (IsMoving && normalized != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(normalized, Vector3.up);
				Quaternion rot = Quaternion.Slerp(_micro.Rigidbody.rotation, b, turnSmoothing * Time.fixedDeltaTime);
				_micro.Rigidbody.MoveRotation(rot);
			}
			else
			{
				KeepUpright();
			}
		}
	}

	private void KeepUpright()
	{
		Transform transform = _micro.transform;
		Vector3 vector = (IsClimbing ? transform.up : Vector3.up);
		Quaternion b = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, vector), vector);
		_micro.Rigidbody.MoveRotation(Quaternion.Slerp(_micro.transform.rotation, b, Time.fixedDeltaTime * 4f));
	}

	private void HitGround(bool isGrounded)
	{
		float y = _micro.Rigidbody.position.ToVirtual().y;
		float num = _previousYPosition - y;
		if (num < 0f)
		{
			_previousYPosition = y;
		}
		if (_inAir)
		{
			_micro.Animator.SetFloat(_fallingFloat, num / (settings.splatFallDistanceThreshold * _micro.AccurateScale * 2f));
		}
		else
		{
			_micro.Animator.SetFloat(_fallingFloat, 0f);
		}
		if (_inAir && isGrounded)
		{
			_inAir = false;
			float num2 = _previousVelocity.magnitude / _micro.AccurateScale;
			if (num > settings.splatFallDistanceThreshold * _micro.AccurateScale || num2 > settings.splatVelocityThreshold)
			{
				if (settings.isSplatEnabled)
				{
					DoSplat();
				}
				else
				{
					DoHardLanding();
				}
			}
			else
			{
				if (!(num > settings.rollFallDistanceThreshold * _micro.AccurateScale) && !(num2 > settings.rollVelocityThreshold))
				{
					return;
				}
				if (_micro.Rigidbody.velocity.Flatten().magnitude > settings.fallOverVelocityThreshold * _micro.AccurateScale)
				{
					Vector3 to = _previousVelocity.Flatten();
					if (Mathf.Abs(Vector3.SignedAngle(_micro.transform.forward, to, Vector3.up)) > settings.fallOverAngleThreshold)
					{
						DoFallBackwards();
					}
					else if (IsAiming)
					{
						DoFallForwards();
					}
					else
					{
						DoRoll();
					}
				}
				else
				{
					DoHardLanding();
				}
			}
		}
		else if (!_inAir && !isGrounded)
		{
			_previousYPosition = _micro.Rigidbody.position.ToVirtual().y;
			_inAir = true;
		}
	}

	private void PlaceBloodOnGround()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(_micro.transform.position + Vector3.up * 0.2f, Vector3.down, out hitInfo, 3f, Layers.pathfindingMask))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(bloodSpritePrefab, hitInfo.point + hitInfo.normal * 0.02f, Quaternion.identity);
			UnityEngine.Object.Destroy(gameObject, 35f);
			gameObject.transform.LookAt(gameObject.transform.position + hitInfo.normal);
		}
	}

	private void DoSplat()
	{
		_micro.Animator.SetBool(_splatBool, true);
		PlaceBloodOnGround();
		KillVelocity();
		LockAiming(true);
		Immobilize(settings.splatLength, _003CDoSplat_003Eb__144_0);
	}

	private void DoHardLanding()
	{
		_micro.Animator.SetBool(_hardLandingBool, true);
		KillVelocity();
		Immobilize(settings.hardLandingLength, _003CDoHardLanding_003Eb__145_0);
	}

	private void DoRoll()
	{
		_micro.Animator.SetBool(_rollBool, true);
		Vector3 vector = _micro.Rigidbody.velocity.Flatten();
		_micro.Rigidbody.rotation = Quaternion.LookRotation(vector);
		LockAiming();
		Vector3 vector2 = ((!(settings.rollingSpeed < vector.magnitude)) ? (vector.normalized * settings.rollingSpeed) : vector);
		LockVelocity(vector2 * _micro.AccurateScale, false);
		Immobilize(settings.rollLength, _003CDoRoll_003Eb__146_0);
	}

	private void DoFallForwards()
	{
		_micro.Animator.SetBool(_fallForwardsBool, true);
		playerIk.LockHips();
		Immobilize(settings.fallDownLength, _003CDoFallForwards_003Eb__147_0);
	}

	private void DoFallBackwards()
	{
		_micro.Animator.SetBool(_fallBackwardsBool, true);
		playerIk.LockHips();
		Immobilize(settings.fallDownLength, _003CDoFallBackwards_003Eb__148_0);
	}

	private void JumpManagement(bool isGrounded)
	{
		if (_timeToAnimatorReset <= Time.time)
		{
			_micro.Animator.SetBool(_jumpBool, false);
		}
		if (_jump && !_isImmobilized)
		{
			_jump = false;
			if (isGrounded && _timeToNextJump <= Time.time)
			{
				_micro.Animator.SetBool(_jumpBool, true);
				Vector3 velocity = _micro.Rigidbody.velocity;
				velocity.y = settings.jumpHeight * _micro.Scale;
				_micro.Rigidbody.velocity = velocity;
				_timeToNextJump = Time.time + settings.jumpCooldown;
				_timeToAnimatorReset = Time.time + settings.jumpBoolTime;
			}
		}
	}

	private void FlyingState()
	{
		if (!_isImmobilized)
		{
			_micro.Animator.SetFloat(_hFloat, horizontalInput);
			_micro.Animator.SetFloat(_vFloat, verticalInput);
			_micro.Animator.SetFloat(_turnFloat, 0f);
			FlySound();
			FlyManagement();
			TryFlyingPunch();
			_jump = false;
			_micro.ChangeParent(null);
		}
	}

	private void FlyManagement()
	{
		float num = settings.flySpeed;
		if (_isSuperFlying)
		{
			num = settings.superSpeed;
		}
		if (IsSprinting)
		{
			num *= settings.flySprintMultiplier;
		}
		num *= _micro.Scale;
		float num2 = 0f;
		if (_isHoldingJump)
		{
			num2 += 1f;
		}
		if (_isHoldingFlyDown)
		{
			num2 -= 1f;
		}
		Vector3 vector = _cameraForward * verticalInput + _cameraRight * horizontalInput + Vector3.up * num2;
		if (vector.sqrMagnitude > 1f)
		{
			vector = vector.normalized;
		}
		DoFlyingRotation(vector);
		if (IsMoving || _isHoldingJump || _isHoldingFlyDown)
		{
			_currentFlySpeed = Mathf.Lerp(_currentFlySpeed, num, Time.fixedDeltaTime * settings.flyingSpeedSmoothingCurve.Evaluate(_currentFlySpeed / settings.superSpeed));
			_micro.Rigidbody.velocity = vector * _currentFlySpeed;
		}
		else
		{
			Vector3 velocity = _micro.Rigidbody.velocity;
			velocity *= 0.9f;
			_micro.Rigidbody.velocity = velocity;
			_currentFlySpeed = velocity.magnitude;
		}
		_micro.Animator.SetFloat(_speedFloat, _currentFlySpeed, settings.speedDampTime, Time.fixedDeltaTime * 3f);
		_previousVelocity = _micro.Rigidbody.velocity;
	}

	private void ReOrientate()
	{
		Transform transform = _micro.transform;
		float angle = Vector3.SignedAngle(transform.up, Vector3.up, transform.right);
		Quaternion quaternion = _micro.Rigidbody.rotation * Quaternion.AngleAxis(angle, Vector3.right);
		quaternion = Quaternion.LookRotation((quaternion * Vector3.forward).Flatten());
		_micro.Rigidbody.MoveRotation(Quaternion.Slerp(_micro.transform.rotation, quaternion, Time.fixedDeltaTime * 4f));
	}

	private void ReOrientateImmediately()
	{
		Transform transform = _micro.transform;
		float angle = Vector3.SignedAngle(transform.up, Vector3.up, transform.right);
		Quaternion quaternion = _micro.Rigidbody.rotation * Quaternion.AngleAxis(angle, Vector3.right);
		quaternion = Quaternion.LookRotation((quaternion * Vector3.forward).Flatten());
		_micro.Rigidbody.MoveRotation(quaternion);
	}

	private void DoFlyingRotation(Vector3 targetDirection)
	{
		if (!IsAiming)
		{
			if (IsMoving && targetDirection != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(targetDirection, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
				Quaternion rot = Quaternion.Slerp(_micro.Rigidbody.rotation, b, settings.flyingRotationSmoothing * Time.fixedDeltaTime);
				_micro.Rigidbody.MoveRotation(rot);
			}
			else
			{
				ReOrientate();
			}
		}
	}

	private void FlySound()
	{
		float b = 0.2f;
		float b2 = 1f;
		if (!IsFlying)
		{
			b = 0f;
		}
		else if (!IsMoving)
		{
			b = 0.05f;
		}
		else if (_isSuperFlying && IsSprinting)
		{
			b = 1f;
			b2 = 3f;
		}
		else if (IsSprinting || (_isSuperFlying && !IsSprinting))
		{
			b = 0.5f;
			b2 = 1.5f;
		}
		_flySource.volume = Mathf.Lerp(_flySource.volume, b, 2f * Time.deltaTime);
		_flySource.pitch = Mathf.Lerp(_flySource.pitch, b2, Time.deltaTime);
	}

	private void DisableFlySound()
	{
		_flySource.volume = 0f;
	}

	private void TryFlyingPunch()
	{
		if (!IsAiming)
		{
			if (_preparingPunch && !_isHoldingFlyingPunch)
			{
				DoFlyingPunch();
				_preparingPunch = false;
			}
			else if (_isHoldingFlyingPunch)
			{
				_micro.Animator.SetBool(_flyingPunchBool, true);
				_preparingPunch = true;
			}
		}
	}

	private void DoFlyingPunch()
	{
		_micro.Animator.SetBool(_flyingPunchBool, false);
		LockVelocity(_micro.Rigidbody.velocity, false);
		_flyingDestruction.SetDestructionForceMultiplier(settings.punchingPower);
		Immobilize(settings.flyingPunchStartLength, _003CDoFlyingPunch_003Eb__160_0);
	}

	private void ClimbingState()
	{
		if (!_isImmobilized)
		{
			ClimbManagement(horizontalInput, verticalInput);
		}
	}

	private void ClimbManagement(float hInput, float vInput)
	{
		float num = Physics.gravity.y * _micro.Scale * settings.climbingGravityScale;
		ClimbingRaycastInfo climbingRaycastInfo = PerformBaseClimbingRaycast();
		ClimbingRaycastInfo climbingRaycastInfo2 = default(ClimbingRaycastInfo);
		if (vInput != 0f)
		{
			climbingRaycastInfo2 = PerformFrontClimbingRaycast();
		}
		Transform transform = _micro.transform;
		Vector3 vector = ((climbingRaycastInfo.isValid && climbingRaycastInfo2.isValid && vInput != 0f) ? Vector3.Lerp(climbingRaycastInfo.averageNormal, climbingRaycastInfo2.averageNormal, 0.85f) : (climbingRaycastInfo.isValid ? climbingRaycastInfo.averageNormal : ((!climbingRaycastInfo2.isValid) ? Vector3.up : climbingRaycastInfo2.averageNormal)));
		bool flag = climbingRaycastInfo2.isValid || climbingRaycastInfo.isValid;
		_micro.Gravity.enabled = !flag;
		Vector3 vector2 = ((!IsMoving || !cameraController.FirstPersonMode) ? _micro.transform.forward : _cameraForward);
		Vector3 b = Vector3.Cross(Vector3.Cross(vector, vector2), vector);
		vector2 = Vector3.Lerp(vector2, b, settings.alignWithSurfaceRotationSmooth * Time.fixedDeltaTime).normalized;
		vector2 = Vector3.Lerp(_previousClimbDirection, vector2, settings.alignWithSurfaceRotationSmooth * Time.fixedDeltaTime).normalized;
		Quaternion b2 = Quaternion.LookRotation(vector2, vector);
		b2 = Quaternion.Slerp(_micro.Rigidbody.rotation, b2, settings.climbRotationSmooth * Time.fixedDeltaTime);
		b2 *= Quaternion.Euler(0f, hInput * settings.climbTurnSpeed, 0f);
		_micro.Rigidbody.MoveRotation(b2);
		if (!flag)
		{
			_previousClimbDirection = vector2;
			_micro.Animator.SetFloat(_speedFloat, 0f);
			return;
		}
		float num2 = settings.climbSpeed * _micro.Scale;
		Vector3 vector3 = Vector3.Slerp(_previousGravity, vector * num, Time.fixedDeltaTime * settings.climbingGravitySmoothing);
		Vector3 vector4 = Vector3.zero;
		if (IsMoving)
		{
			_micro.Animator.SetFloat(_speedFloat, vInput * settings.climbingAnimationSpeed);
			if (IsSprinting)
			{
				num2 *= settings.climbSprintMultiplier;
			}
			float time = 0f;
			if (vInput != 0f && climbingRaycastInfo2.isValid)
			{
				time = (Quaternion.Inverse(transform.rotation) * (climbingRaycastInfo2.averagePosition - transform.position)).y / _micro.Scale;
			}
			vector4 = vector2 * (vInput * num2) + transform.up * (_micro.Scale * settings.climbingGroundingCurve.Evaluate(time));
		}
		else
		{
			_micro.Animator.SetFloat(_speedFloat, 0f);
		}
		_micro.Rigidbody.velocity = vector4 + vector3;
		_previousVelocity = _micro.Rigidbody.velocity;
		_previousClimbDirection = vector2;
	}

	private ClimbingRaycastInfo PerformFrontClimbingRaycast()
	{
		Transform obj = _micro.transform;
		Vector3 forward = obj.forward;
		Vector3 up = obj.up;
		Vector3 a = Vector3.SlerpUnclamped(forward, up, rayArcRangeUpper);
		Vector3 b = Vector3.SlerpUnclamped(forward, up, rayArcRangeLower);
		Vector3 vector = _micro.transform.up * (rayUpOffset * _micro.Scale) + forward * (rayForwardOffset * _micro.Scale);
		float maxDistance = rayLength * _micro.Scale;
		int num = 0;
		float num2 = settings.numberOfForwardClimbingRayCasts;
		for (int i = 0; (float)i < num2; i++)
		{
			Vector3 vector2 = Vector3.Lerp(a, b, (float)i / num2);
			Debug.DrawRay(_micro.transform.position + vector, vector2, Color.red, 0.05f);
			RaycastHit hitInfo;
			if (Physics.Raycast(_micro.transform.position + vector, vector2, out hitInfo, maxDistance, Layers.walkableMask))
			{
				_climbingRaycastBuffer[num] = hitInfo;
				num++;
			}
		}
		if (num == 0)
		{
			return default(ClimbingRaycastInfo);
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int j = 0; j < num; j++)
		{
			zero += _climbingRaycastBuffer[j].point;
			zero2 += _climbingRaycastBuffer[j].normal;
		}
		float num3 = num;
		zero /= num3;
		zero2 /= num3;
		return new ClimbingRaycastInfo(zero, zero2);
	}

	private ClimbingRaycastInfo PerformBaseClimbingRaycast()
	{
		Transform obj = _micro.transform;
		Vector3 position = obj.position;
		Vector3 up = obj.up;
		Vector3 forward = obj.forward;
		Vector3 a = position + (up * (0.1f * _micro.Scale) + forward * (settings.baseClimbingRaycastRadius * _micro.Scale));
		Vector3 b = position + (up * (0.1f * _micro.Scale) + forward * ((0f - settings.baseClimbingRaycastRadius) * _micro.Scale));
		float num = settings.numberOfBaseClimbingRayCasts;
		float maxDistance = settings.baseClimbingRaycastLength * _micro.Scale;
		Vector3 vector = -up;
		int num2 = 0;
		for (int i = 0; (float)i < num; i++)
		{
			Vector3 vector2 = Vector3.Lerp(a, b, (float)i / num);
			Debug.DrawRay(vector2, vector, Color.red, 0.05f);
			RaycastHit hitInfo;
			if (Physics.Raycast(vector2, vector, out hitInfo, maxDistance, Layers.walkableMask))
			{
				_climbingRaycastBuffer[num2] = hitInfo;
				num2++;
			}
		}
		if (num2 == 0)
		{
			return default(ClimbingRaycastInfo);
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int j = 0; j < num2; j++)
		{
			zero += _climbingRaycastBuffer[j].point;
			zero2 += _climbingRaycastBuffer[j].normal;
		}
		float num3 = num2;
		zero /= num3;
		zero2 /= num3;
		return new ClimbingRaycastInfo(zero, zero2);
	}

	private void AiState()
	{
		if (IsMoving || _jump || IsFlying || IsClimbing)
		{
			SetState(PlayerMicroControllerState.Standing);
		}
	}

	private void SetupAim()
	{
		if (!_isAimingLocked && _state != PlayerMicroControllerState.Climbing)
		{
			Transform boneTransform = _micro.Animator.GetBoneTransform(HumanBodyBones.RightHand);
			Transform boneTransform2 = _micro.Animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
			if (!IsAiming && !_micro.isCrushed && (bool)boneTransform && (bool)boneTransform2)
			{
				IsAiming = true;
				_micro.Rigidbody.MoveRotation((_state != PlayerMicroControllerState.Flying) ? Quaternion.LookRotation(_cameraForwardOnPlane) : Quaternion.LookRotation(_cameraForward));
				raygun.transform.localScale = Vector3.one * (0.08f * _micro.Scale);
				raygun.transform.SetParent(boneTransform, true);
				raygun.gameObject.SetActive(true);
				raygun.transform.localPosition = Vector3.zero;
				raygun.transform.rotation = Quaternion.LookRotation(boneTransform.position - boneTransform2.position);
				_micro.Animator.SetBool(_aimBool, IsAiming);
				_micro.Animator.SetFloat(_speedFloat, 0f);
				_raygunArmingSource.volume = 1f;
				SoundManager.SetAndPlaySoundClip(_raygunArmingSource, SoundManager.Instance.playerRaygunArmingSound);
				_aimSetup = StartCoroutine(SetRaygunRotation());
				cameraController.StartAiming();
				InputManager.inputs.Player.Zoom.Disable();
			}
		}
	}

	private void DisableAim()
	{
		if (IsAiming)
		{
			IsAiming = false;
			if (_aimSetup != null)
			{
				StopCoroutine(_aimSetup);
			}
			cameraController.StopAiming();
			raygun.gameObject.SetActive(false);
			raygun.transform.SetParent(base.transform, false);
			_micro.Animator.SetBool(_aimBool, IsAiming);
			playerIk.GetAimIk().enabled = false;
			SoundManager.SetAndPlaySoundClip(_raygunArmingSource, SoundManager.Instance.playerRaygunDisarmingSound);
			_raygunArmingSource.Play();
			InputManager.inputs.Player.Zoom.Enable();
		}
	}

	private void HandleThirdPersonAiming()
	{
		if (_state == PlayerMicroControllerState.Standing)
		{
			HandleStandingThirdPersonAiming();
		}
		else if (_state == PlayerMicroControllerState.Flying)
		{
			HandleFlyingThirdPersonAiming();
		}
	}

	private void HandleStandingThirdPersonAiming()
	{
		float num = Vector3.Angle(_camera.transform.forward.Flatten(), _micro.transform.forward.Flatten());
		bool triggered = InputManager.inputs.Micro.WeaponFire.triggered;
		if (IsMoving || (triggered && num > settings.maxAimAngle))
		{
			StandingAimingRotation();
		}
		if (num <= settings.maxAimAngle || triggered)
		{
			aimPoint.position = GetAimPoint();
			playerIk.GetAimIk().solver.target = aimPoint;
		}
		else
		{
			Vector3 forward = _camera.transform.forward;
			Vector3 obj = forward;
			float num2 = Vector3.Angle(obj, obj.Flatten());
			float num3 = settings.maxAimAngle;
			if (!_isOnLeft)
			{
				num3 *= -1f;
			}
			if (forward.y > 0f)
			{
				num2 *= -1f;
			}
			Quaternion quaternion = Quaternion.Euler(0f, num3, 0f) * Quaternion.Euler(num2, 0f, 0f);
			Transform transform = _micro.transform;
			aimPoint.position = transform.position + Vector3.up * (_micro.Height * 0.75f) + transform.rotation * (quaternion * Vector3.forward) * (settings.baseZeroing * _micro.Scale);
			playerIk.GetAimIk().solver.target = aimPoint;
		}
		bool flag = (Quaternion.Inverse(_micro.transform.rotation) * _camera.transform.forward).x >= 0f;
		if (flag != _isOnLeft && num <= settings.responseAngle)
		{
			_isOnLeft = flag;
			if (num > settings.maxAimAngle)
			{
				Vector3 position = _micro.transform.position;
				Vector3 position2 = aimPoint.transform.position;
				Vector3 fromDirection = position2.Flatten() - position.Flatten();
				Quaternion quaternion2 = Quaternion.FromToRotation(fromDirection, _micro.transform.forward.Flatten());
				fromDirection = position2 - position;
				fromDirection = quaternion2 * (quaternion2 * fromDirection);
				position2 = position + fromDirection;
				aimPoint.transform.position = position2;
			}
		}
	}

	private void HandleFlyingThirdPersonAiming()
	{
		FlyingAimingRotation();
		aimPoint.position = GetAimPoint();
		playerIk.GetAimIk().solver.target = aimPoint;
	}

	private void StandingAimingRotation()
	{
		_micro.Rigidbody.MoveRotation(Quaternion.LookRotation(_cameraForwardOnPlane));
	}

	private void FlyingAimingRotation()
	{
		_micro.Rigidbody.MoveRotation(Quaternion.LookRotation(_cameraForward));
	}

	public PlayerRaygun GetRaygun()
	{
		return raygun;
	}

	public void SetRaygunFirstPersonPosition(Vector3 pos)
	{
		if (_aimSetup != null)
		{
			StopCoroutine(_aimSetup);
		}
		Transform obj = raygun.transform;
		Transform transform2 = (obj.parent = _camera.transform);
		obj.position = transform2.position + transform2.rotation * pos;
		obj.localRotation = Quaternion.identity;
	}

	public void ResetRaygunPosition()
	{
		Transform obj = raygun.transform;
		obj.parent = _micro.Animator.GetBoneTransform(HumanBodyBones.RightHand);
		obj.localPosition = Vector3.zero;
		_aimSetup = StartCoroutine(SetRaygunRotation());
	}

	private void HandleFirstPersonAiming()
	{
		StandingAimingRotation();
		Vector3 worldPosition = GetAimPoint();
		raygun.transform.LookAt(worldPosition);
	}

	private Vector3 GetAimPoint()
	{
		Transform transform = _camera.transform;
		Vector3 position = transform.position;
		float num = Vector3.Distance(_micro.transform.position, position);
		RaycastHit hitInfo;
		if (Physics.Raycast(position + transform.forward * num, transform.forward, out hitInfo, 100f * _micro.Scale, aimingMask))
		{
			return hitInfo.point;
		}
		return _camera.transform.position + transform.forward * (settings.baseZeroing * _micro.Scale + num);
	}

	private IEnumerator SetRaygunRotation()
	{
		yield return new WaitForSeconds(0.2f);
		yield return null;
		if (IsAiming)
		{
			raygun.transform.rotation = _micro.transform.rotation;
			AimIK aimIk = playerIk.GetAimIk();
			aimIk.enabled = true;
			try
			{
				IKSolver.Bone[] bones = aimIk.solver.bones;
				bones[0].weight = 0.6f;
				bones[1].weight = 0.8f;
				bones[2].weight = 1f;
			}
			catch (Exception ex)
			{
				Debug.LogError("A bone was missing while initializing the raygun's player IK.");
				Debug.LogError(ex.ToString());
			}
			playerIk.GetAimIk().solver.transform = raygun.GetFiringPoint();
		}
	}

	[CompilerGenerated]
	private void _003CPrepareState_003Eb__117_0()
	{
		_micro.Animator.SetBool(_startFlyingBool, false);
	}

	[CompilerGenerated]
	private void _003CPrepareState_003Eb__117_1()
	{
		_micro.Animator.SetBool(_startClimbingBool, false);
	}

	[CompilerGenerated]
	private void _003CDoSplat_003Eb__144_0()
	{
		_micro.Animator.SetBool(_splatBool, false);
		Immobilize(settings.splatStandUpLength, UnlockAiming);
	}

	[CompilerGenerated]
	private void _003CDoHardLanding_003Eb__145_0()
	{
		_micro.Animator.SetBool(_hardLandingBool, false);
	}

	[CompilerGenerated]
	private void _003CDoRoll_003Eb__146_0()
	{
		_micro.Animator.SetBool(_rollBool, false);
		UnlockVelocity();
		UnlockAiming();
	}

	[CompilerGenerated]
	private void _003CDoFallForwards_003Eb__147_0()
	{
		_micro.Animator.SetBool(_fallForwardsBool, false);
		if (_micro.IsOnSurface(Layers.walkableMask))
		{
			Immobilize(settings.standUpLength, _003CDoFallForwards_003Eb__147_1);
		}
		else
		{
			playerIk.UnlockHips();
		}
	}

	[CompilerGenerated]
	private void _003CDoFallForwards_003Eb__147_1()
	{
		playerIk.UnlockHips();
		KillVelocity();
	}

	[CompilerGenerated]
	private void _003CDoFallBackwards_003Eb__148_0()
	{
		_micro.Animator.SetBool(_fallBackwardsBool, false);
		if (_micro.IsOnSurface(Layers.walkableMask))
		{
			Immobilize(settings.standUpLength, _003CDoFallBackwards_003Eb__148_1);
		}
		else
		{
			playerIk.UnlockHips();
		}
	}

	[CompilerGenerated]
	private void _003CDoFallBackwards_003Eb__148_1()
	{
		playerIk.UnlockHips();
		KillVelocity();
	}

	[CompilerGenerated]
	private void _003CDoFlyingPunch_003Eb__160_0()
	{
		Vector3 position = _micro.Animator.GetBoneTransform(HumanBodyBones.RightHand).position;
		_flyingDestruction.Punch(settings.punchingPower * Velocity.magnitude, position, Velocity.normalized);
		Immobilize(settings.flyingPunchEndLength, _003CDoFlyingPunch_003Eb__160_1);
	}

	[CompilerGenerated]
	private void _003CDoFlyingPunch_003Eb__160_1()
	{
		UnlockVelocity();
		_flyingDestruction.SetDestructionForceMultiplier(0f);
	}
}
