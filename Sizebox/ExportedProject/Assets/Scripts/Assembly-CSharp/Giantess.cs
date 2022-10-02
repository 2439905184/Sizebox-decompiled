using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.UI.Controller;
using SaveDataStructures;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(GiantessIK))]
public class Giantess : Humanoid, IPlayable, IEntity, IGameObject
{
	private class BloodScale : MonoBehaviour
	{
		public float scale;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<GameObject> _003C_003E9__23_0;

		internal bool _003CSizeChanged_003Eb__23_0(GameObject blood)
		{
			return blood == null;
		}
	}

	[Header("Giantess - Required References")]
	[FormerlySerializedAs("GtsCollider")]
	public GameObject gtsCollider;

	[FormerlySerializedAs("GtsMovement")]
	public GTSMovement gtsMovement;

	private Voice _voiceAudio;

	private Wind _windAudio;

	private FootEffect _footEffect;

	private List<SkinnedMeshCollider> _skinnedMeshColliderList;

	private BodyPhysics _mBodyPhysics;

	private readonly List<GameObject> _bloodPatches = new List<GameObject>();

	public bool canLookAtPlayer;

	public override EntityType EntityType
	{
		get
		{
			return EntityType.MACRO;
		}
	}

	public Player Player { get; private set; }

	public bool IsPlayerControlled { get; private set; }

	private void DeleteOldColliders()
	{
		SABoneColliderChild[] componentsInChildren = GetComponentsInChildren<SABoneColliderChild>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	protected override void InitializeModel(GameObject modelPrefab)
	{
		base.InitializeModel(modelPrefab);
		base.model.SetLayerRecursively(Layers.gtsBodyLayer);
		if (base.model != defaultModel)
		{
			_skinnedMeshColliderList = AddSkinnedMeshColliders(base.model);
		}
		base.model.transform.localScale = Vector3.one * (2000f / meshHeight);
		meshHeight = 2000f;
		_mBodyPhysics = base.model.GetComponent<BodyPhysics>();
		if (!_mBodyPhysics)
		{
			_mBodyPhysics = base.model.AddComponent<BodyPhysics>();
		}
		_footEffect = base.model.GetComponent<FootEffect>();
		if (!_footEffect)
		{
			_footEffect = base.model.AddComponent<FootEffect>();
		}
		_voiceAudio = base.model.GetComponent<Voice>();
		if (!_voiceAudio)
		{
			_voiceAudio = base.model.AddComponent<Voice>();
		}
		_windAudio = base.model.GetComponent<Wind>();
		if (!_windAudio)
		{
			_windAudio = base.model.AddComponent<Wind>();
		}
		base.Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		base.Animator.SetRuntimeController(IOManager.Instance.gtsAnimatorController);
		base.Animator.SetFloat(Animator.StringToHash("animationSpeed"), GameController.macroSpeed);
		if (GlobalPreferences.GtsAI.value)
		{
			ai.EnableAI();
		}
	}

	protected override void FinishInitialization()
	{
		base.FinishInitialization();
		base.updateWhenOffScreen = (int)GlobalPreferences.OffScreenUpdate > 1;
		GiantessManager.Instance.AddGiantess(this);
		ForceColliderUpdate();
		if ((bool)Player)
		{
			Player.PlayAs(this);
		}
	}

	protected override void Awake()
	{
		isGiantess = true;
		ObjectManager.UpdateGiantessSpeed(this);
		DeleteOldColliders();
		base.Awake();
		Transform transform = base.transform;
		gtsCollider.transform.position = transform.position;
		gtsCollider.transform.rotation = transform.rotation;
		gtsCollider.transform.localScale = transform.localScale;
		baseHeight = 2000f;
		UpdateCapsuleColliderTransform();
		canLookAtPlayer = true;
	}

	protected override void SizeChanged(float oldSize, float newSize)
	{
		_bloodPatches.RemoveAll(_003C_003Ec._003C_003E9__23_0 ?? (_003C_003Ec._003C_003E9__23_0 = _003C_003Ec._003C_003E9._003CSizeChanged_003Eb__23_0));
		foreach (GameObject bloodPatch in _bloodPatches)
		{
			BloodScale component = bloodPatch.GetComponent<BloodScale>();
			if ((!GlobalPreferences.BloodGrows.value && component.scale < bloodPatch.gameObject.transform.lossyScale.y) || (!GlobalPreferences.BloodShrinks.value && component.scale > bloodPatch.gameObject.transform.lossyScale.y))
			{
				StayAtLockedScale(bloodPatch.gameObject.transform, component.scale);
			}
			else
			{
				component.scale = bloodPatch.gameObject.transform.lossyScale.y;
			}
		}
	}

	private List<SkinnedMeshCollider> AddSkinnedMeshColliders(GameObject modelMesh)
	{
		List<SkinnedMeshCollider> list = new List<SkinnedMeshCollider>();
		SkinnedMeshRenderer[] componentsInChildren = modelMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SkinnedMeshCollider skinnedMeshCollider = componentsInChildren[i].gameObject.AddComponent<SkinnedMeshCollider>();
			skinnedMeshCollider.Init(this, SaveData.IsLoading);
			list.Add(skinnedMeshCollider);
		}
		return list;
	}

	public override void Move(Vector3 worldPos)
	{
		base.Move(worldPos);
		UpdateCapsuleColliderTransform();
	}

	public void _MoveMesh(Vector3 worldPos)
	{
		base.Move(worldPos);
	}

	public void InvokeColliderUpdate()
	{
		Invoke("ForceColliderUpdate", 1f);
	}

	public void ForceColliderUpdate()
	{
		if (!base.Initialized)
		{
			return;
		}
		foreach (SkinnedMeshCollider skinnedMeshCollider in _skinnedMeshColliderList)
		{
			skinnedMeshCollider.UpdateCollider();
		}
	}

	public void ManualBreastPhysics()
	{
		BreastPhysicsController controller = UnityEngine.Object.Instantiate(Resources.Load("UI/BreastControlsUi") as GameObject, GuiManager.Instance.MainCanvasGameObject.transform, false).AddComponent<BreastPhysicsController>();
		_mBodyPhysics.HandleNewBreastPhysics(controller);
	}

	public override void SetColliderActive(bool enable)
	{
		gtsCollider.gameObject.SetActive(enable);
		if (enable)
		{
			UpdateCapsuleColliderTransform();
		}
		SkinnedCollidersEnable(enable);
		PhysicsCollidersEnable(enable);
	}

	public override void SetPoseMode(bool pose)
	{
		if (pose != base.IsPosed)
		{
			base.SetPoseMode(pose);
			_mBodyPhysics.SetPoseMode(pose);
		}
	}

	private void PhysicsCollidersEnable(bool option)
	{
		if ((bool)_mBodyPhysics)
		{
			_mBodyPhysics.ColliderEnable(option && !base.IsPosed);
		}
		gtsMovement.EnableCollider(option && !base.IsPosed);
	}

	private void SkinnedCollidersEnable(bool option)
	{
		if (_skinnedMeshColliderList == null)
		{
			return;
		}
		foreach (SkinnedMeshCollider skinnedMeshCollider in _skinnedMeshColliderList)
		{
			skinnedMeshCollider.EnableCollision(option);
		}
	}

	private void UpdateCapsuleColliderTransform()
	{
		Transform transform = base.transform;
		gtsCollider.transform.position = transform.position;
		gtsCollider.transform.rotation = transform.rotation;
		ScaleCapsuleCollider();
	}

	private void ScaleCapsuleCollider()
	{
		gtsCollider.transform.localScale = base.transform.lossyScale;
	}

	private void ScaleAudioSources(float scale)
	{
		if (base.Initialized)
		{
			_voiceAudio.UpdateAudioScale(scale);
			_windAudio.UpdateAudioScale(scale);
			_footEffect.UpdateScale(scale);
		}
	}

	protected override void OnDestroy()
	{
		UnityEngine.Object.Destroy(gtsCollider);
		GiantessManager.Instance.RemoveGiantess(base.id);
		base.OnDestroy();
	}

	public override void ChangeRotation(Vector3 newRotation)
	{
		base.ChangeRotation(newRotation);
		UpdateCapsuleColliderTransform();
	}

	protected override float ClampScale(float scale)
	{
		return Mathf.Clamp(scale, MapSettingInternal.minGtsSize, MapSettingInternal.maxGtsSize);
	}

	public override void ChangeScale(float newScale)
	{
		base.ChangeScale(newScale);
		ScaleCapsuleCollider();
		ScaleAudioSources(newScale);
		animationManager.UpdateAnimationSpeed();
	}

	protected override void UpdateMass()
	{
		base.UpdateMass();
		if ((bool)_mBodyPhysics)
		{
			_mBodyPhysics.UpdateMass(base.Rigidbody.mass);
		}
	}

	public override void DestroyObject(bool recursive = true)
	{
		UnityEngine.Object.Destroy(gtsCollider);
		base.DestroyObject(recursive);
	}

	public void StartBreastExpansion()
	{
		_mBodyPhysics.StartBe();
	}

	public void SetBeSpeed(float speed)
	{
		_mBodyPhysics.beSpeed = speed;
	}

	public void AddBlood(GameObject blood)
	{
		blood.AddComponent<BloodScale>().scale = blood.transform.lossyScale.x;
		_bloodPatches.Add(blood);
	}

	public bool StartPlayerControl(Player player)
	{
		if (IsPlayerControlled || !player)
		{
			return false;
		}
		Player = player;
		if (!base.Initialized)
		{
			return false;
		}
		ik.DisableLookAt();
		base.Animator.logWarnings = false;
		base.Animator.SetRuntimeController(IOManager.Instance.gtsPlayerAnimatorController);
		ai.behaviorController.StopMainBehavior();
		gtsMovement.moveState = GTSMovement.MacroMoveState.OnlyMoveWithPhysics;
		IsPlayerControlled = true;
		isPlayer = true;
		return true;
	}

	public void OnPlayerControlEnd(Player player)
	{
		Player = null;
		IsPlayerControlled = false;
		isPlayer = false;
		ik.EnableLookAt();
		base.Animator.logWarnings = true;
		base.Animator.SetRuntimeController(IOManager.Instance.gtsAnimatorController);
		gtsMovement.moveState = GTSMovement.MacroMoveState.DoNotMove;
	}

	public override SavableData Save()
	{
		return new MacroSaveData(this);
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
}
