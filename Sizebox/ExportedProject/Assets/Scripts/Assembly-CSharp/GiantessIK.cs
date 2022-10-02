using System;
using System.Collections.Generic;
using RootMotion;
using RootMotion.FinalIK;
using SteeringBehaviors;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FootIK))]
public class GiantessIK : EntityComponent
{
	private enum ButtState
	{
		Idle = 0,
		Sitting = 1,
		Standing = 2
	}

	public bool luaIKEnabled;

	private FullBodyBipedIK _ik;

	private LookAtIK _lookAtIK;

	private Player _player;

	private Animator _anim;

	private Giantess _giantess;

	private Kinematic _target;

	private List<IKEffector> _effectors = new List<IKEffector>();

	private Transform _headT;

	private Transform _rightShoulder;

	private Transform _hips;

	private IKBone _bodyEffector;

	private ButtState _buttState;

	private Vector3 _sitPosition;

	private bool _sit;

	private float ButtOffset = 180f;

	private bool _cancelButt;

	private GiantessBone[] _handBones;

	private float _currentGtsSpeed = 1f;

	private AnimationManager _animManager;

	private Vector3 _previousPosition;

	public FootIK FootIk { get; private set; }

	private IKBone leftHandEffector { get; set; }

	public IKBone rightHandEffector { get; private set; }

	public HandIK hand { get; private set; }

	public HeadIK head { get; private set; }

	public PoseIK poseIK { get; private set; }

	public bool PoseIkEnabled { get; private set; }

	public bool poseMode { get; private set; }

	private void Awake()
	{
		_player = GameController.LocalClient.Player;
	}

	public override void Initialize(EntityBase entity)
	{
		_giantess = entity as Giantess;
		if (!_giantess)
		{
			Debug.LogError("GiantessIK requires a Giantess entity");
			return;
		}
		_anim = _giantess.Animator;
		_headT = _anim.GetBoneTransform(HumanBodyBones.Head);
		Transform boneTransform = _anim.GetBoneTransform(HumanBodyBones.Spine);
		Transform boneTransform2 = _anim.GetBoneTransform(HumanBodyBones.LeftEye);
		Transform boneTransform3 = _anim.GetBoneTransform(HumanBodyBones.RightEye);
		_rightShoulder = _anim.GetBoneTransform(HumanBodyBones.RightShoulder);
		_hips = _anim.GetBoneTransform(HumanBodyBones.Hips);
		BipedReferences references = new BipedReferences();
		BipedReferences.AutoDetectReferences(ref references, _anim.transform, BipedReferences.AutoDetectParams.Default);
		if (!references.isFilled)
		{
			return;
		}
		_ik = _giantess.model.GetComponent<FullBodyBipedIK>();
		if (!_ik)
		{
			_ik = _giantess.model.AddComponent<FullBodyBipedIK>();
		}
		_ik.solver.iterations = 3;
		_ik.SetReferences(references, null);
		_ik.solver.SetLimbOrientations(BipedLimbOrientations.UMA);
		SetDefaultIkValues();
		_bodyEffector = new IKBone(_ik.solver.bodyEffector);
		leftHandEffector = new IKBone(_ik.solver.leftHandEffector);
		rightHandEffector = new IKBone(_ik.solver.rightHandEffector);
		_lookAtIK = _giantess.model.GetComponent<LookAtIK>();
		if (!_lookAtIK)
		{
			_lookAtIK = base.gameObject.AddComponent<LookAtIK>();
		}
		_lookAtIK.solver.SetChain(new Transform[1] { boneTransform }, _headT, new Transform[2] { boneTransform2, boneTransform3 }, _anim.transform.GetChild(0));
		head = new HeadIK(_lookAtIK, _giantess);
		hand = new HandIK(_ik, _giantess);
		poseIK = new PoseIK(this, _ik, _lookAtIK, _giantess);
		_bodyEffector.position = _hips.position;
		_animManager = _giantess.animationManager;
		if ((bool)_rightShoulder)
		{
			_handBones = _rightShoulder.GetComponentsInChildren<GiantessBone>();
			GiantessBone[] handBones = _handBones;
			for (int i = 0; i < handBones.Length; i++)
			{
				handBones[i].canCrush = false;
			}
		}
		FootIk = GetComponent<FootIK>();
		FootIk.Initialize(_ik, _anim, _giantess);
		CenterOrigin.onCentering = (UnityAction<Vector3>)Delegate.Combine(CenterOrigin.onCentering, new UnityAction<Vector3>(OnOriginOffset));
		GetEffectors();
		_previousPosition = _giantess.transform.position;
		Giantess giantess = _giantess;
		giantess.SizeChanging = (UnityAction<float, float>)Delegate.Combine(giantess.SizeChanging, new UnityAction<float, float>(ScaleEffectors));
		base.initialized = true;
	}

	private void GetEffectors()
	{
		_effectors = new List<IKEffector>
		{
			_ik.solver.bodyEffector,
			_ik.solver.leftHandEffector,
			_ik.solver.rightHandEffector,
			_ik.solver.rightFootEffector,
			_ik.solver.leftFootEffector,
			_ik.solver.leftShoulderEffector,
			_ik.solver.rightShoulderEffector,
			_ik.solver.rightThighEffector,
			_ik.solver.leftThighEffector
		};
	}

	private void OnOriginOffset(Vector3 offset)
	{
		if (poseMode)
		{
			poseIK.OffsetEffectors(offset);
		}
		_previousPosition = _giantess.transform.position;
	}

	public void LookAtPoint(Vector3 point)
	{
		if (head != null)
		{
			head.LookAtPoint(point);
		}
	}

	public void DisableLookAt()
	{
		if (head != null)
		{
			head.DisableLookAt();
		}
	}

	public void EnableLookAt()
	{
		if (head != null)
		{
			head.Cancel();
		}
	}

	private void OnDestroy()
	{
		CenterOrigin.onCentering = (UnityAction<Vector3>)Delegate.Remove(CenterOrigin.onCentering, new UnityAction<Vector3>(OnOriginOffset));
		if (hand != null)
		{
			hand.ReleaseTarget();
		}
	}

	public void SetPoseMode(bool pose)
	{
		if (poseMode != pose)
		{
			poseMode = pose;
			if (pose)
			{
				EnableIK(false);
				return;
			}
			SetPoseIK(false);
			SetDefaultIkValues();
			EnableIK(true);
		}
	}

	public void SetPoseIK(bool value)
	{
		if (value && !poseMode)
		{
			SetPoseMode(true);
		}
		if (poseIK != null)
		{
			if (value)
			{
				poseIK.EnablePoseIk();
			}
			else
			{
				poseIK.DisablePoseIk();
			}
			PoseIkEnabled = value;
		}
	}

	public void EnableIK(bool value)
	{
		EnableBodyIK(value);
		_lookAtIK.enabled = value;
	}

	private void EnableBodyIK(bool value)
	{
		_ik.enabled = value;
	}

	private void SetDefaultIkValues()
	{
		_ik.fixTransforms = false;
		_ik.solver.headMapping.maintainRotationWeight = 0.6f;
		_ik.solver.leftArmChain.reach = 0f;
		_ik.solver.rightArmChain.reach = 0f;
		_ik.solver.spineMapping.twistWeight = 0f;
		_ik.solver.spineStiffness = 0f;
		_ik.solver.pullBodyHorizontal = 0f;
		_ik.solver.pullBodyVertical = 0f;
		_ik.solver.rightLegMapping.maintainRotationWeight = 0.6f;
		_ik.solver.rightLegChain.reach = 0.2f;
		_ik.solver.leftLegMapping.maintainRotationWeight = 0.6f;
		_ik.solver.leftLegChain.reach = 0.2f;
		if (head != null)
		{
			head.SetDefaultValues();
		}
	}

	private void FixedUpdate()
	{
		if (base.initialized && !poseMode)
		{
			if (!luaIKEnabled && hand != null)
			{
				hand.Update();
			}
			if (head != null)
			{
				head.Update();
			}
		}
	}

	private void Update()
	{
		if (!base.initialized || poseMode)
		{
			return;
		}
		_currentGtsSpeed = _animManager.GetCurrentSpeed();
		EnableBodyIK(FootIk.IsActive || luaIKEnabled || (hand != null && hand.IsActive()));
		if (!_target)
		{
			SetTarget(_player.Entity);
			if (!_target)
			{
				return;
			}
		}
		UpdateButt();
	}

	private void LateUpdate()
	{
		if (base.initialized)
		{
			UpdateEffectors();
			if (!poseMode)
			{
				UpdateBones();
			}
		}
	}

	private void UpdateBones()
	{
		leftHandEffector.Update();
		rightHandEffector.Update();
		_bodyEffector.Update();
	}

	private void UpdateEffectors()
	{
		Vector3 vector = _giantess.transform.position - _previousPosition;
		foreach (IKEffector effector in _effectors)
		{
			effector.position += vector;
		}
		_lookAtIK.solver.IKPosition += vector;
		_previousPosition = _giantess.transform.position;
	}

	private void ScaleEffectors(float oldScale, float newScale)
	{
		Vector3 position = _giantess.transform.position;
		float num = (newScale - oldScale) / oldScale;
		foreach (IKEffector effector in _effectors)
		{
			Vector3 vector = effector.position - position;
			vector += vector * num;
			effector.position = vector + position;
		}
		Vector3 vector2 = _lookAtIK.solver.IKPosition - position;
		vector2 += vector2 * num;
		_lookAtIK.solver.IKPosition = vector2 + position;
	}

	private void SetTarget(EntityBase entity)
	{
		if ((bool)entity)
		{
			SetTarget(entity.transform);
		}
	}

	private void SetTarget(Transform target)
	{
		_target = new TransformKinematic(target);
	}

	public void SetTarget(Vector3 target)
	{
		_target = new VectorKinematic(target);
	}

	public void SetButtTarget(Vector3 target)
	{
		_sitPosition = target;
		_sit = true;
	}

	public void CancelButtTarget()
	{
		_cancelButt = true;
	}

	private void UpdateButt()
	{
		ButtState buttState = _buttState;
		float b = 0f;
		Vector3 b2 = _bodyEffector.position;
		float num = 1f;
		switch (_buttState)
		{
		case ButtState.Idle:
			b = 0f;
			if (_sit)
			{
				buttState = ButtState.Sitting;
				Vector3 position = _hips.position;
				_bodyEffector.position = position;
				b2 = position;
				Debug.Log("Starting Rotation: " + base.transform.localRotation);
			}
			break;
		case ButtState.Sitting:
			b = 1f;
			num = 0.3f;
			b2 = _sitPosition + Vector3.up * (ButtOffset * _giantess.Scale);
			if (_cancelButt)
			{
				_cancelButt = false;
				buttState = ButtState.Standing;
				_sit = false;
			}
			break;
		case ButtState.Standing:
			b = 0f;
			if (_bodyEffector.positionWeight < 0.01f)
			{
				buttState = ButtState.Idle;
			}
			else if (_sit)
			{
				buttState = ButtState.Sitting;
			}
			break;
		}
		_bodyEffector.rotationWeight = Mathf.Lerp(_bodyEffector.rotationWeight, b, Time.deltaTime * num * _currentGtsSpeed);
		_bodyEffector.positionWeight = Mathf.Lerp(_bodyEffector.positionWeight, b, Time.deltaTime * num * _currentGtsSpeed);
		_bodyEffector.position = Vector3.Lerp(_bodyEffector.position, b2, Time.deltaTime * num * _currentGtsSpeed);
		if (_buttState != buttState)
		{
			Debug.Log("Next Butt State: " + buttState);
			_buttState = buttState;
		}
	}

	public bool IsSit()
	{
		return _buttState == ButtState.Sitting;
	}
}
