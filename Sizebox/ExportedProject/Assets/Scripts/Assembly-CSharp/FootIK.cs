using System;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GiantessIK))]
public class FootIK : MonoBehaviour
{
	public enum FootStates
	{
		Idle = 0,
		Prepare = 1,
		Crush = 2,
		Wait = 3,
		Grind1 = 4,
		Grind2 = 5,
		Retract = 6,
		Return = 7
	}

	private GiantessIK _giantessIk;

	private FootEffect _footEffect;

	private Transform _internalTarget;

	private Transform _targetTransform;

	private EntityBase _targetEntity;

	private GrounderFBBIK _grounder;

	private FootStates _footState;

	private float _minFootWeight = -0.3f;

	private const float WhatTheFuckMagicNumber = 1.4f;

	private float _offsetPercentage = 1f;

	private IKBone _rightFootEffector;

	private IKBone _leftFootEffector;

	private IKBone _activeFoot;

	private int _activeFootNumber;

	private MeshCollider _rightFootCollider;

	private MeshCollider _leftFootCollider;

	private MeshCollider _activeFootCollider;

	private float _footOffsetRight;

	private float _footOffsetLeft;

	private float _activeFootOffset;

	private readonly float _maxOffset = 400f;

	private readonly float _returnSpeed = 2f;

	private readonly float _maxDistanceFoot = 0.35f;

	private readonly float _retractSpeed = 0.5f;

	private float _footWaitTime;

	private readonly float _footForwardDisplacement = 40f;

	private Vector3 _footTarget;

	private Vector3 _footTargetAngles;

	private Vector3 _randomOffset;

	private Transform _rightFoot;

	private Transform _leftFoot;

	private Humanoid _giantess;

	private bool _manualStomp;

	private AnimationManager _animManager;

	private bool _initialized;

	private Vector3 _finalTargetAim;

	private Vector3 _footStartingAngles;

	private readonly Vector3 _grind1 = new Vector3(0f, 30f, 0f);

	private readonly Vector3 _grind2 = new Vector3(0f, -60f, 0f);

	private float _turnOffsetPercentage;

	public bool EnableIk
	{
		get
		{
			return _grounder.enabled;
		}
		set
		{
			_grounder.enabled = value;
		}
	}

	public float VerticalOffset
	{
		get
		{
			return _grounder.solver.heightOffset;
		}
		set
		{
			_grounder.solver.heightOffset = value;
		}
	}

	public float Grounder
	{
		get
		{
			return _grounder.weight;
		}
		set
		{
			_grounder.weight = Mathf.Clamp01(value);
		}
	}

	public bool IsActive
	{
		get
		{
			if (_footState == FootStates.Idle)
			{
				return EnableIk;
			}
			return true;
		}
	}

	public FootStates FootState
	{
		get
		{
			return _footState;
		}
	}

	public bool CrushEnded { get; private set; } = true;


	private void Awake()
	{
		_giantessIk = GetComponent<GiantessIK>();
	}

	public void Initialize(FullBodyBipedIK ik, Animator anim, Humanoid humanoid)
	{
		_giantess = humanoid;
		_animManager = _giantess.animationManager;
		_rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
		_leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
		_leftFootEffector = new IKBone(ik.solver.leftFootEffector)
		{
			position = _leftFoot.position
		};
		_rightFootEffector = new IKBone(ik.solver.rightFootEffector)
		{
			position = _rightFoot.position
		};
		_footEffect = _giantess.model.GetComponent<FootEffect>();
		FindFootOffsets();
		_grounder = base.gameObject.AddComponent<GrounderFBBIK>();
		_grounder.solver.layers = Layers.stompingMask;
		_grounder.solver.footSpeed = 10f;
		_grounder.solver.maxFootRotationAngle = 20f;
		_grounder.ik = ik;
		_grounder.solver.footRadius = _giantess.Scale * 75f;
		_grounder.solver.maxStep = _giantess.Scale * 600f;
		_grounder.enabled = GlobalPreferences.UseGrounder.value;
		_internalTarget = new GameObject("FootIkTarget").transform;
		_internalTarget.parent = _giantess.transform;
		humanoid.SizeChanging = (UnityAction<float, float>)Delegate.Combine(humanoid.SizeChanging, new UnityAction<float, float>(OnScale));
		_initialized = true;
	}

	private void OnScale(float oldScale, float newScale)
	{
		_grounder.solver.footRadius = newScale * 75f;
		_grounder.solver.maxStep = newScale * 600f;
	}

	private void FindFootOffsets()
	{
		Vector3 position = base.transform.position;
		_footOffsetRight = (_rightFoot.position.y - position.y) / _giantess.Scale;
		_footOffsetLeft = (_leftFoot.position.y - position.y) / _giantess.Scale;
	}

	private void LateUpdate()
	{
		if (_initialized && !_giantessIk.poseMode)
		{
			if ((bool)_targetTransform)
			{
				_internalTarget.position = _targetTransform.position;
			}
			UpdateBones();
			if ((_footState == FootStates.Prepare || _footState == FootStates.Crush) && !IsClose(_internalTarget))
			{
				_footState = FootStates.Return;
			}
		}
	}

	private void UpdateBones()
	{
		_leftFootEffector.Update();
		_rightFootEffector.Update();
	}

	public void CancelFootCrush()
	{
		_footState = FootStates.Return;
	}

	public void CrushTarget(EntityBase entity)
	{
		SetTarget(entity);
		StartCrush();
	}

	public void CrushTarget(Vector3 targetPoint)
	{
		SetTarget(targetPoint);
		StartCrush();
	}

	public void CrushManualTarget(Transform stompTarget)
	{
		EntityBase entityBase = FindNearbyEntity(stompTarget);
		if ((bool)entityBase)
		{
			SetTarget(entityBase);
		}
		else
		{
			SetTarget(stompTarget);
		}
		_manualStomp = true;
		StartCrush();
	}

	private EntityBase FindNearbyEntity(Transform aimT)
	{
		Collider[] array = new Collider[2048];
		int num = Physics.OverlapSphereNonAlloc(aimT.position, aimT.lossyScale.x, array, 1 << Layers.microLayer);
		for (int i = 0; i < num; i++)
		{
			EntityBase componentInParent = array[i].GetComponentInParent<EntityBase>();
			if ((bool)componentInParent)
			{
				return componentInParent;
			}
		}
		return null;
	}

	private void StartCrush()
	{
		if (_giantessIk.luaIKEnabled)
		{
			_footState = FootStates.Idle;
		}
		else if (CrushEnded)
		{
			StopAllCoroutines();
			StartCoroutine(HandleFootCrush());
		}
	}

	private void ChooseFoot()
	{
		Vector3 position = _internalTarget.position;
		float magnitude = (_leftFoot.position - position).magnitude;
		float magnitude2 = (_rightFoot.position - position).magnitude;
		if (!_leftFootCollider || !_rightFootCollider)
		{
			FindFeetColliders();
		}
		if ((!_manualStomp) ? (4f + UnityEngine.Random.value + UnityEngine.Random.value < 10f * magnitude2 / (magnitude2 + magnitude)) : (magnitude2 > magnitude))
		{
			_activeFoot = _leftFootEffector;
			_activeFootNumber = 0;
			_activeFootCollider = _leftFootCollider;
			_activeFootOffset = _footOffsetLeft;
		}
		else
		{
			_activeFoot = _rightFootEffector;
			_activeFootNumber = 1;
			_activeFootCollider = _rightFootCollider;
			_activeFootOffset = _footOffsetRight;
		}
	}

	private IEnumerator HandleFootCrush()
	{
		_footState = FootStates.Idle;
		CrushEnded = false;
		float weightSpeed = 1f;
		float weight = 1f;
		ChooseFoot();
		do
		{
			float scale = _giantess.Scale;
			float stompSpeed = Time.deltaTime * GlobalPreferences.StompSpeed.value * _animManager.GetCurrentSpeed() * 3f;
			switch (_footState)
			{
			case FootStates.Idle:
				yield return IdleState();
				break;
			case FootStates.Prepare:
				yield return PrepareState(scale);
				break;
			case FootStates.Crush:
				yield return CrushState(stompSpeed, scale);
				break;
			case FootStates.Wait:
				yield return WaitState(stompSpeed);
				break;
			case FootStates.Grind1:
				yield return Grind1State(stompSpeed);
				break;
			case FootStates.Grind2:
				yield return Grind2State(stompSpeed);
				break;
			case FootStates.Retract:
				yield return RetractState(stompSpeed, scale);
				break;
			case FootStates.Return:
				yield return ReturnState();
				break;
			}
			if (_footState == FootStates.Return)
			{
				weight = -0.2f;
				weightSpeed = _returnSpeed;
			}
			if (_footState != 0)
			{
				_activeFoot.positionWeight = Mathf.Lerp(_activeFoot.positionWeight, weight, stompSpeed * weightSpeed);
				_activeFoot.position = _footTarget;
			}
			if (_footState == FootStates.Grind1 || _footState == FootStates.Grind2)
			{
				_activeFoot.rotation.eulerAngles = _footTargetAngles;
			}
			if (_footState == FootStates.Idle || _activeFoot != _leftFootEffector)
			{
				_leftFootEffector.positionWeight = Mathf.Lerp(_leftFootEffector.positionWeight, _minFootWeight, stompSpeed);
			}
			if (_footState == FootStates.Idle || _activeFoot != _rightFootEffector)
			{
				_rightFootEffector.positionWeight = Mathf.Lerp(_rightFootEffector.positionWeight, _minFootWeight, stompSpeed);
			}
		}
		while (!CrushEnded);
	}

	private IEnumerator IdleState()
	{
		_leftFootEffector.position = _leftFoot.position;
		_rightFootEffector.position = _rightFoot.position;
		if (!_animManager.TransitionEnded())
		{
			yield break;
		}
		if (IsReachableByFeet(_internalTarget) && IsClose(_internalTarget))
		{
			if (_manualStomp)
			{
				_randomOffset = Vector3.zero;
			}
			else
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * 20f;
				_randomOffset = _activeFootCollider.transform.rotation * new Vector3(vector.x, 0f, vector.y * 4f);
			}
			_footState = FootStates.Prepare;
		}
		else
		{
			_footState = FootStates.Return;
		}
	}

	private IEnumerator PrepareState(float gtsScale)
	{
		if ((bool)_internalTarget)
		{
			_footTarget = _internalTarget.position + gtsScale * (Vector3.up * _maxOffset - _activeFootCollider.transform.forward * _footForwardDisplacement + _randomOffset);
			if ((bool)_targetEntity && _targetEntity.AccurateScale * 2.5f <= _giantess.AccurateScale)
			{
				_footTarget += Vector3.up * _targetEntity.Height;
			}
		}
		else
		{
			_footState = FootStates.Return;
		}
		if (FeetTargetIsChild(_internalTarget))
		{
			_footState = FootStates.Return;
		}
		if (_activeFoot.positionWeight > 0.75f)
		{
			_finalTargetAim = _internalTarget.position - Vector3.up * (_giantess.Height * 0.03f);
			_footState = FootStates.Crush;
			_offsetPercentage = _activeFoot.positionWeight;
		}
		yield break;
	}

	private IEnumerator CrushState(float stompSpeed, float gtsScale)
	{
		_offsetPercentage = Mathf.Lerp(_offsetPercentage, -0.1f, stompSpeed * 3f);
		_offsetPercentage = Mathf.Max(_offsetPercentage, 0f);
		float num = _maxOffset * _offsetPercentage + _activeFootOffset * 1.4f;
		_footTarget = _finalTargetAim + gtsScale * (Vector3.up * num - _activeFootCollider.transform.forward * _footForwardDisplacement + _randomOffset);
		if (_offsetPercentage == 0f)
		{
			Vector3 vector = _finalTargetAim;
			RaycastHit hitInfo;
			if (Physics.Raycast(_activeFoot.position + Vector3.up * gtsScale, Vector3.down, out hitInfo, float.PositiveInfinity, Layers.gtsWalkableMask))
			{
				vector = hitInfo.point;
			}
			_footEffect.DoStep(vector + base.transform.forward * (100f * gtsScale), 3f, _activeFootNumber);
			_footTarget = _activeFoot.position;
			_footWaitTime = 0f;
			_footState = FootStates.Wait;
		}
		yield break;
	}

	private IEnumerator WaitState(float stompSpeed)
	{
		_footWaitTime += stompSpeed;
		if ((double)_footWaitTime > 1.2)
		{
			_footState = FootStates.Retract;
		}
		yield break;
	}

	private IEnumerator Grind1State(float stompSpeed)
	{
		_turnOffsetPercentage = Mathf.Lerp(_turnOffsetPercentage, 0f, stompSpeed * 3f);
		_footTargetAngles = _footStartingAngles + _grind1 * _turnOffsetPercentage;
		if (_turnOffsetPercentage < 0.15f)
		{
			Debug.Log("here");
			_turnOffsetPercentage = 1f;
			_footStartingAngles = _activeFoot.rotation.eulerAngles;
			_footState = FootStates.Grind2;
		}
		yield break;
	}

	private IEnumerator Grind2State(float stompSpeed)
	{
		_turnOffsetPercentage = Mathf.Lerp(_turnOffsetPercentage, 0f, stompSpeed * 1.5f);
		_footTargetAngles = _footStartingAngles + _grind2 * _turnOffsetPercentage;
		if (_turnOffsetPercentage < 0.15f)
		{
			_footState = FootStates.Retract;
		}
		yield break;
	}

	private IEnumerator RetractState(float stompSpeed, float gtsScale)
	{
		_offsetPercentage = Mathf.Lerp(_offsetPercentage, 1f, stompSpeed * _retractSpeed);
		float num = _maxOffset * _offsetPercentage + _activeFootOffset * 1.4f;
		_footTarget = _finalTargetAim + gtsScale * (Vector3.up * num - _activeFootCollider.transform.forward * _footForwardDisplacement + _randomOffset);
		if (_offsetPercentage > 0.5f)
		{
			_footState = FootStates.Return;
		}
		yield break;
	}

	private IEnumerator ReturnState()
	{
		if (_activeFoot == null)
		{
			_activeFoot = _rightFootEffector;
		}
		if (_activeFoot.positionWeight < 0f)
		{
			_footState = FootStates.Idle;
			CrushEnded = true;
			_manualStomp = false;
		}
		yield break;
	}

	private bool IsClose(Transform victim)
	{
		if (!victim)
		{
			return false;
		}
		return IsClose(victim.position);
	}

	public bool IsClose(Vector3 targetPos)
	{
		float scale = _giantess.Scale;
		Transform transform = base.transform;
		Vector3 vector = transform.position - (targetPos - transform.forward * (_footForwardDisplacement * scale));
		vector.y = 0f;
		return vector.magnitude < _maxDistanceFoot * 1000f * scale * 1.6f;
	}

	private bool IsReachableByFeet(Transform victim)
	{
		if ((bool)victim && !FeetTargetIsChild(victim))
		{
			return FeetTargetIsInRange(victim);
		}
		return false;
	}

	private bool FeetTargetIsChild(Transform victim)
	{
		Transform parent = victim.transform.parent;
		if ((bool)parent)
		{
			return parent.gameObject.layer == Layers.gtsBodyLayer;
		}
		return false;
	}

	private bool FeetTargetIsInRange(Transform victim)
	{
		return Mathf.Abs(base.transform.InverseTransformPoint(victim.position).y) < _maxOffset;
	}

	private void FindFeetColliders()
	{
		for (int i = 0; i < _rightFoot.childCount; i++)
		{
			if (_rightFoot.GetChild(i).name.StartsWith("Coll"))
			{
				_rightFootCollider = _rightFoot.GetChild(i).GetComponent<MeshCollider>();
				break;
			}
		}
		for (int j = 0; j < _leftFoot.childCount; j++)
		{
			if (_leftFoot.GetChild(j).name.StartsWith("Coll"))
			{
				_leftFootCollider = _leftFoot.GetChild(j).GetComponent<MeshCollider>();
				break;
			}
		}
	}

	public bool IsCloserToLeftFoot(Vector3 pos)
	{
		float magnitude = (_leftFoot.position - pos).magnitude;
		return (_rightFoot.position - pos).magnitude > magnitude;
	}

	private void SetTarget(EntityBase entity)
	{
		_targetEntity = entity;
		if ((bool)entity)
		{
			_targetTransform = entity.transform;
			_internalTarget.position = _targetTransform.position;
		}
		else
		{
			_targetTransform = null;
		}
	}

	private void SetTarget(Transform newTarget)
	{
		_targetTransform = newTarget;
		_targetEntity = null;
		if ((bool)_targetTransform)
		{
			_internalTarget.position = _targetTransform.position;
		}
	}

	private void SetTarget(Vector3 targetPos)
	{
		_targetEntity = null;
		_targetTransform = null;
		_internalTarget.position = targetPos;
	}

	public void ClearTarget()
	{
		_targetEntity = null;
		_targetTransform = null;
	}

	private void OnDrawGizmos()
	{
		if ((bool)_internalTarget)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(_internalTarget.position, new Vector3(3f, 3f, 3f));
			Gizmos.DrawWireSphere(_footTarget - _activeFootOffset * Vector3.up * _giantess.Scale, 1f);
		}
	}
}
