using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AI;
using SaveDataStructures;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Humanoid : EntityBase, IPosable, IAnimated, IEntity, IGameObject
{
	private const float BaseHumanoidWeight = 12.142858f;

	private const float MaxHumanoidWeight = 5000000f;

	private const float BlinkDurationTime = 0.05f;

	private static readonly WaitForSeconds BlinkShort = new WaitForSeconds(2.9f);

	private static readonly WaitForSeconds BlinkLong = new WaitForSeconds(5f);

	private static readonly WaitForSeconds BlinkDuration = new WaitForSeconds(0.05f);

	[Header("Humanoid - Required References")]
	public AIController ai;

	public ActionController ActionManager;

	public AnimationManager animationManager;

	public SenseController senses;

	[FormerlySerializedAs("_defaultAnimatorController")]
	[SerializeField]
	private RuntimeAnimatorController defaultAnimatorController;

	[SerializeField]
	protected EntityValue health;

	public GiantessIK ik;

	private bool _isPosed;

	private FingerPoser _fingerPoser;

	private Transform _head;

	private Transform _leftEye;

	private Transform _rightEye;

	public Cloth[] cloths;

	protected bool Dead;

	public float animationSpeed;

	private EntityMorphData _blinkingMorph;

	private float _blinkMorphUserState = float.NegativeInfinity;

	public Animator Animator { get; private set; }

	public GiantessIK Ik
	{
		get
		{
			return ik;
		}
	}

	public RuntimeAnimatorController DefaultAnimatorController
	{
		get
		{
			return defaultAnimatorController;
		}
	}

	public bool IsPosed
	{
		get
		{
			return _isPosed;
		}
		private set
		{
			_isPosed = value;
			if ((int)GlobalPreferences.OffScreenUpdate == 1)
			{
				base.updateWhenOffScreen = value;
			}
		}
	}

	public bool IsDead
	{
		get
		{
			return Dead;
		}
	}

	public float unscaledWalkSpeed { get; private set; }

	public event Action<Humanoid> OnRevive;

	public event Action<Humanoid> OnDeath;

	protected override void Awake()
	{
		ActionManager = new ActionController(this);
		isHumanoid = true;
		IsPosed = false;
		base.Awake();
		SavedScenesManager.PreSave = (UnityAction)Delegate.Combine(SavedScenesManager.PreSave, new UnityAction(PreSave));
	}

	protected override void InitializeModel(GameObject modelPrefab)
	{
		base.model = ((modelPrefab != defaultModel) ? UnityEngine.Object.Instantiate(modelPrefab, base.transform) : modelPrefab);
		Animator = base.model.GetComponent<Animator>();
		_head = Animator.GetBoneTransform(HumanBodyBones.Head);
		_leftEye = Animator.GetBoneTransform(HumanBodyBones.LeftEye);
		_rightEye = Animator.GetBoneTransform(HumanBodyBones.RightEye);
		MeshRenderers = GetRenderers();
		meshHeight = GetCombinedRendererHeight(MeshRenderers);
		ResetColliderActive();
		base.model.gameObject.AddComponent<RootMotionTransfer>();
		if (GlobalPreferences.ClothPhysics.value)
		{
			cloths = GetComponentsInChildren<Cloth>();
			SizeChanging = (UnityAction<float, float>)Delegate.Combine(SizeChanging, new UnityAction<float, float>(_003CInitializeModel_003Eb__45_0));
			return;
		}
		Cloth[] componentsInChildren = GetComponentsInChildren<Cloth>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
	}

	protected override void FinishInitialization()
	{
		base.FinishInitialization();
		Transform boneTransform = Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
		Transform boneTransform2 = Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
		Transform boneTransform3 = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
		if (!boneTransform || !boneTransform2 || !boneTransform3)
		{
			unscaledWalkSpeed = 0.01f;
		}
		else
		{
			Vector3 position = boneTransform2.position;
			float num = ((boneTransform3.position - position).magnitude + (boneTransform.position - position).magnitude) / base.AccurateScale;
			unscaledWalkSpeed = num * 1.2f / 1.333f / 60f;
		}
		ChangeScale(base.transform.lossyScale.y);
	}

	protected override void OnDestroy()
	{
		SavedScenesManager.PreSave = (UnityAction)Delegate.Remove(SavedScenesManager.PreSave, new UnityAction(PreSave));
		if (ai != null && ai.behaviorController != null)
		{
			ai.behaviorController.StopAllBehaviors();
		}
		UnityEngine.Object.Destroy(GetComponent<AIController>());
		UnityEngine.Object.Destroy(GetComponent<AnimationManager>());
		base.OnDestroy();
	}

	public override Vector3 GetEyesPosition()
	{
		if (_leftEye != null && _rightEye != null)
		{
			return (_leftEye.position + _rightEye.position) * 0.5f;
		}
		return _head.position;
	}

	protected void StayAtLockedScale(Transform selfTransform, float intendedScale)
	{
		Transform parent = selfTransform.parent;
		selfTransform.parent = null;
		selfTransform.localScale = Vector3.one * intendedScale;
		selfTransform.parent = parent;
	}

	public override void ChangeScale(float newScale)
	{
		base.ChangeScale(newScale);
		UpdateMass();
	}

	protected virtual void UpdateMass()
	{
		base.Rigidbody.mass = CalculateMass();
	}

	protected float CalculateMass()
	{
		return Mathf.Clamp(12.142858f * Mathf.Pow(7f, Mathf.Log(base.MeshHeight, 2f)), 0.1f, 5000000f);
	}

	protected virtual void Kill()
	{
		if (!Dead)
		{
			Dead = true;
			if (this.OnDeath != null)
			{
				this.OnDeath(this);
			}
		}
	}

	protected void Revive()
	{
		if (Dead)
		{
			Dead = false;
			if (this.OnRevive != null)
			{
				this.OnRevive(this);
			}
		}
	}

	public virtual bool CanDecide()
	{
		return !Dead;
	}

	public Animator GetAnimator()
	{
		return Animator;
	}

	public override void Lock()
	{
		if (!locked)
		{
			ActionController actionManager = ActionManager;
			if (actionManager != null)
			{
				actionManager.ClearAll();
			}
			base.Lock();
		}
	}

	public virtual void SetPoseMode(bool value)
	{
		if (IsPosed != value)
		{
			IsPosed = value;
			Vector3 position = base.transform.position;
			Animator.SetRuntimeController(IsPosed ? IOManager.Instance.poseAnimatorController : DefaultAnimatorController);
			base.transform.position = position;
			if ((bool)ik)
			{
				ik.SetPoseMode(IsPosed);
			}
			if (!value)
			{
				DestroyFingerPosers();
			}
		}
	}

	public void SetPose(string namePose)
	{
		ai.DisableAI();
		ai.behaviorController.StopMainBehavior();
		ActionManager.ClearAll();
		animationManager.PlayAnimation(namePose, true);
	}

	public override void ChangeVerticalOffset(float newOffset)
	{
		if ((bool)ik && (bool)ik.FootIk)
		{
			ik.FootIk.VerticalOffset = newOffset;
		}
		base.ChangeVerticalOffset(newOffset);
	}

	public override float GetGrounderWeight()
	{
		if ((bool)ik && (bool)ik.FootIk)
		{
			return ik.FootIk.Grounder;
		}
		return 1f;
	}

	public override void ChangeGrounderWeight(float newWeight)
	{
		if ((bool)ik && (bool)ik.FootIk)
		{
			float grounder = Mathf.Clamp01(newWeight);
			ik.FootIk.Grounder = grounder;
			base.Gravity = !(newWeight < 1f);
		}
	}

	public void SetPoseIk(bool enable)
	{
		if ((bool)ik)
		{
			ik.SetPoseIK(enable);
		}
	}

	public void LoadPose(CustomPose pose)
	{
		if ((bool)ik)
		{
			SetPoseMode(true);
			ik.SetPoseIK(true);
			ik.poseIK.LoadPose(pose);
		}
	}

	public void CreateFingerPosers()
	{
		if (_fingerPoser != null)
		{
			UnityEngine.Object.Destroy(_fingerPoser);
		}
		_fingerPoser = base.gameObject.AddComponent<FingerPoser>();
		_fingerPoser.Init(this);
	}

	public void DestroyFingerPosers()
	{
		if (_fingerPoser != null)
		{
			UnityEngine.Object.Destroy(_fingerPoser);
		}
	}

	public void ShowFingerPosers(bool visible)
	{
		if (_fingerPoser != null)
		{
			_fingerPoser.Show(visible);
		}
	}

	public override void InitializeMorphs()
	{
		base.InitializeMorphs();
		_blinkingMorph = FindMorphByNameContains("笑い");
		if (_blinkingMorph != null)
		{
			StartCoroutine(BlinkRoutine());
		}
	}

	public static IEnumerator StartBlinkingRoutines()
	{
		IList<Humanoid> humanoids = UnityEngine.Object.FindObjectsOfType<Humanoid>();
		yield return null;
		foreach (Humanoid item in humanoids)
		{
			item.StartBlinkingRoutine();
			yield return null;
		}
	}

	private void StartBlinkingRoutine()
	{
		if (_blinkingMorph != null)
		{
			StartCoroutine(BlinkRoutine());
		}
	}

	private IEnumerator BlinkTransition(float begin, float end, float duration)
	{
		float time = 0f;
		while (time < duration)
		{
			float weight = Mathf.Lerp(begin, end, time / duration);
			time += Time.deltaTime;
			SetMorphValue(_blinkingMorph, weight);
			yield return null;
		}
		SetMorphValue(_blinkingMorph, end);
	}

	private IEnumerator BlinkRoutine()
	{
		while (GlobalPreferences.BlinkEnabled.value)
		{
			yield return (UnityEngine.Random.value < 0.4f) ? BlinkShort : BlinkLong;
			_blinkMorphUserState = GetMorphValue(_blinkingMorph);
			yield return BlinkTransition(_blinkMorphUserState, 1f, 0.1f);
			yield return BlinkDuration;
			yield return BlinkTransition(1f, _blinkMorphUserState, 0.1f);
			_blinkMorphUserState = float.NegativeInfinity;
		}
	}

	public override void Load(SavableData inData, bool loadPosition = true)
	{
		HumanoidSaveData humanoidSaveData = (HumanoidSaveData)inData;
		if (isGiantess)
		{
			GTSMovement gtsMovement = base.gameObject.GetComponentInChildren<Giantess>().gtsMovement;
			if (humanoidSaveData.Animation != null)
			{
				gtsMovement.doNotMoveGts = humanoidSaveData.Animation.doNotMoveGts;
			}
		}
		if (humanoidSaveData.Animation != null && !string.IsNullOrEmpty(humanoidSaveData.Animation.name))
		{
			animationManager.PlayAnimation(humanoidSaveData.Animation.name, humanoidSaveData.Animation.isPose, true);
			animationManager.ChangeSpeed(humanoidSaveData.Animation.speed);
		}
		if (humanoidSaveData.AIData != null)
		{
			ai.LoadSaveData(humanoidSaveData.AIData);
		}
		if (humanoidSaveData.IsPosed)
		{
			if (humanoidSaveData.Animation != null)
			{
				SetPose(humanoidSaveData.Animation.name);
			}
			if (humanoidSaveData.IsCustomPosed)
			{
				LoadPose(humanoidSaveData.CustomPose);
			}
		}
		base.Load(inData, loadPosition);
	}

	private void PreSave()
	{
		if (!float.IsNegativeInfinity(_blinkMorphUserState))
		{
			SetMorphValue(_blinkingMorph, _blinkMorphUserState);
		}
	}

	[SpecialName]
	GameObject IGameObject.get_gameObject()
	{
		return base.gameObject;
	}

	[SpecialName]
	Transform IGameObject.get_transform()
	{
		return base.transform;
	}

	[CompilerGenerated]
	private void _003CInitializeModel_003Eb__45_0(float os, float ns)
	{
		Cloth[] array = cloths;
		foreach (Cloth obj in array)
		{
			obj.enabled = false;
			obj.enabled = true;
		}
	}
}
