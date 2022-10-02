using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.UI.Controller;
using Sizebox.CharacterEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BodyPhysics : MonoBehaviour
{
	private class ColliderPair
	{
		private readonly Transform _transform;

		private readonly Transform _parentBone;

		public readonly Collider Collider;

		public readonly Rigidbody Rigidbody;

		private readonly Transform _offsetTarget;

		public ColliderPair(Transform transform, Transform parent, Collider collider)
			: this(transform, collider)
		{
			_parentBone = parent;
		}

		public ColliderPair(Transform transform, Collider collider, Transform offsetTarget)
			: this(transform, collider)
		{
			_offsetTarget = offsetTarget;
		}

		public ColliderPair(Transform transform, Collider collider)
		{
			_transform = transform;
			Collider = collider;
			GameObject gameObject = collider.gameObject;
			gameObject.AddComponent<Destructor>();
			gameObject.layer = Layers.destroyerLayer;
			collider.gameObject.AddComponent<NotABone>();
			Rigidbody = collider.gameObject.AddComponent<Rigidbody>();
			Rigidbody.useGravity = false;
			Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}

		public void Update()
		{
			Vector3 vector = Vector3.zero;
			if ((bool)_offsetTarget)
			{
				float num = Rigidbody.position.y - _offsetTarget.position.y;
				vector = Vector3.down * (num * 0.8f);
			}
			if ((bool)_parentBone)
			{
				Vector3 position = _parentBone.position;
				Vector3 position2 = _transform.position;
				Rigidbody.MovePosition(Vector3.Lerp(position2, position, 0.5f) + vector);
				Vector3 vector2 = position - position2;
				if (vector2 != Vector3.zero)
				{
					Rigidbody.rotation = Quaternion.LookRotation(vector2);
				}
			}
			else
			{
				Rigidbody.MovePosition(_transform.position + vector);
			}
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Transform, Transform, Transform> _003C_003E9__23_0;

		public static Func<Transform, Transform, Transform> _003C_003E9__23_1;

		internal Transform _003CBreastGrowth_003Eb__23_0(Transform x, Transform y)
		{
			if (!(x.position.y > y.position.y))
			{
				return y;
			}
			return x;
		}

		internal Transform _003CBreastGrowth_003Eb__23_1(Transform x, Transform y)
		{
			if (!(x.position.y < y.position.y))
			{
				return y;
			}
			return x;
		}
	}

	[FormerlySerializedAs("BeSpeed")]
	public float beSpeed;

	private bool _mBreastExpansionEnabled;

	private DynamicBoneColliderBase _mTorsoCollider;

	private DynamicBone _breastController;

	private List<Transform> _breasts;

	private BreastPhysicsController _mBreastUiPhysicsController;

	private Animator _anim;

	private List<ColliderPair> _colliderPairs;

	private Dictionary<Transform, Transform> _boneToParentBoneMap;

	private void Awake()
	{
		_anim = GetComponent<Animator>();
		_colliderPairs = new List<ColliderPair>();
		_boneToParentBoneMap = GenerateParentMap();
		AddDestructionColliders();
		if (GlobalPreferences.HairPhysics.value)
		{
			SetHairPhysics();
		}
		PlaceTorsoCollider();
		if (GlobalPreferences.BreastPhysics.value)
		{
			SetBreastPhysics();
		}
		if (GlobalPreferences.JigglePhysics.value)
		{
			SetJigglePhysics();
		}
	}

	private void FixedUpdate()
	{
		UpdateColliderPositions();
	}

	private void UpdateColliderPositions()
	{
		foreach (ColliderPair colliderPair in _colliderPairs)
		{
			colliderPair.Update();
		}
	}

	private Dictionary<Transform, Transform> GenerateParentMap()
	{
		Dictionary<Transform, Transform> dictionary = new Dictionary<Transform, Transform>();
		AddParentMapping(dictionary, HumanBodyBones.LeftFoot, HumanBodyBones.LeftLowerLeg);
		AddParentMapping(dictionary, HumanBodyBones.RightFoot, HumanBodyBones.RightLowerLeg);
		AddParentMapping(dictionary, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftUpperLeg);
		AddParentMapping(dictionary, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightUpperLeg);
		AddParentMapping(dictionary, HumanBodyBones.LeftHand, HumanBodyBones.LeftLowerArm);
		AddParentMapping(dictionary, HumanBodyBones.RightHand, HumanBodyBones.RightLowerArm);
		AddParentMapping(dictionary, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftUpperArm);
		AddParentMapping(dictionary, HumanBodyBones.RightLowerArm, HumanBodyBones.RightUpperArm);
		AddParentMapping(dictionary, HumanBodyBones.Spine, HumanBodyBones.Hips);
		return dictionary;
	}

	private void AddParentMapping(Dictionary<Transform, Transform> map, HumanBodyBones childBone, HumanBodyBones parentBone)
	{
		Transform boneTransform = _anim.GetBoneTransform(childBone);
		Transform boneTransform2 = _anim.GetBoneTransform(parentBone);
		if ((bool)boneTransform && (bool)boneTransform2)
		{
			map.Add(boneTransform, boneTransform2);
		}
	}

	public void SetPoseMode(bool poseMode)
	{
		if ((bool)_breastController)
		{
			_breastController.enabled = !poseMode;
		}
	}

	public void HandleNewBreastPhysics(BreastPhysicsController controller)
	{
		controller.AttachActions(AcceptBreastSettings, CancelBreastSettings, EnableBreastPhysics, DisableBreastPhysics);
		if (_breastController != null)
		{
			controller.Dampening = _breastController.m_Damping;
			controller.Collider = _breastController.m_Colliders.Any();
			controller.Elasticity = _breastController.m_Elasticity;
			controller.Inert = _breastController.m_Inert;
			controller.Stiffness = _breastController.m_Stiffness;
		}
		controller.ReloadUi();
		_mBreastUiPhysicsController = controller;
	}

	private void CancelBreastSettings()
	{
		_mBreastUiPhysicsController = null;
	}

	private void AcceptBreastSettings()
	{
		if (_breastController == null)
		{
			return;
		}
		_breastController.m_Damping = _mBreastUiPhysicsController.Dampening;
		_breastController.m_Elasticity = _mBreastUiPhysicsController.Elasticity;
		_breastController.m_Inert = _mBreastUiPhysicsController.Inert;
		_breastController.m_Stiffness = _mBreastUiPhysicsController.Stiffness;
		if (_breastController.m_Colliders.Any() != _mBreastUiPhysicsController.Collider)
		{
			if (_mBreastUiPhysicsController.Collider)
			{
				_breastController.m_Colliders = new List<DynamicBoneColliderBase> { _mTorsoCollider };
			}
			else
			{
				_breastController.m_Colliders.Clear();
			}
		}
		_breastController.UpdateParameters();
	}

	private void DisableBreastPhysics()
	{
		if (!(_breastController == null))
		{
			_breastController.enabled = false;
			UnityEngine.Object.Destroy(_breastController);
			_breastController = null;
		}
	}

	private void EnableBreastPhysics()
	{
		if (!(_breastController != null))
		{
			SetBreastPhysics();
		}
	}

	private bool IsBreastSystemEnabled()
	{
		return _breastController != null;
	}

	public void StartBe()
	{
		if (!_mBreastExpansionEnabled)
		{
			if (IsBreastSystemEnabled())
			{
				StartCoroutine(BreastGrowth());
			}
			else
			{
				Debug.LogError("No Breast Bones found in this model");
			}
		}
	}

	private IEnumerator BreastGrowth()
	{
		_mBreastExpansionEnabled = true;
		Transform leftBreastTransform = _breasts.Aggregate(_003C_003Ec._003C_003E9__23_0 ?? (_003C_003Ec._003C_003E9__23_0 = _003C_003Ec._003C_003E9._003CBreastGrowth_003Eb__23_0));
		Transform rightBreastTransform = _breasts.Aggregate(_003C_003Ec._003C_003E9__23_1 ?? (_003C_003Ec._003C_003E9__23_1 = _003C_003Ec._003C_003E9._003CBreastGrowth_003Eb__23_1));
		float xOffset = Mathf.Abs(leftBreastTransform.position.y - rightBreastTransform.position.y) * 10f / base.transform.lossyScale.y;
		_mBreastExpansionEnabled = true;
		while (_mBreastExpansionEnabled)
		{
			float num = beSpeed * Time.deltaTime;
			leftBreastTransform.localScale *= 1f + num;
			Vector3 localPosition = leftBreastTransform.localPosition;
			localPosition -= Vector3.right * (xOffset * num);
			localPosition -= Vector3.up * (10f * num);
			leftBreastTransform.localPosition = localPosition;
			rightBreastTransform.localScale *= 1f + num;
			Vector3 localPosition2 = rightBreastTransform.localPosition;
			localPosition2 += Vector3.right * (xOffset * num);
			localPosition2 -= Vector3.up * (10f * num);
			rightBreastTransform.localPosition = localPosition2;
			yield return null;
		}
	}

	private void SetHairPhysics()
	{
		Transform boneTransform = _anim.GetBoneTransform(HumanBodyBones.Head);
		Transform transform = BoneUtil.FindHairRoot(_anim);
		if (!transform)
		{
			return;
		}
		DynamicBone dynamicBone = transform.gameObject.AddComponent<DynamicBone>();
		dynamicBone.m_Root = transform;
		dynamicBone.m_Damping = 0.2f;
		dynamicBone.m_Elasticity = 0.05f;
		dynamicBone.m_Stiffness = 0.7f;
		dynamicBone.m_Inert = 0f;
		dynamicBone.m_Gravity = Vector3.down;
		dynamicBone.m_EndLength = 1f;
		dynamicBone.m_Radius = 0.003f / boneTransform.lossyScale.y;
		dynamicBone.m_DistanceToObject = 100f;
		HashSet<Transform> hashSet = new HashSet<Transform>();
		foreach (Transform item in transform)
		{
			if (BoneUtil.IsHairBone(item))
			{
				Collider[] componentsInChildren = item.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					hashSet.Add(collider.transform);
				}
			}
			else
			{
				hashSet.Add(item);
			}
		}
		dynamicBone.m_Exclusions = hashSet;
		dynamicBone.m_Colliders = new List<DynamicBoneColliderBase>
		{
			_mTorsoCollider,
			PlaceHairHeadCollider(boneTransform)
		};
	}

	private void FindJiggles(ref HashSet<Transform> jiggles, Transform bone)
	{
		for (int i = 0; i < bone.childCount; i++)
		{
			Transform child = bone.GetChild(i);
			if (BoneUtil.IsJiggleBone(child) && !BoneUtil.IsJiggleBone(child.parent) && !BoneUtil.IsHumanBone(_anim, child.parent))
			{
				jiggles.Add(child);
				break;
			}
			FindJiggles(ref jiggles, child);
		}
	}

	private void SetJigglePhysics()
	{
		HashSet<Transform> hashSet = new HashSet<Transform>();
		HashSet<Transform> jiggles = new HashSet<Transform>();
		SkinnedMeshRenderer[] componentsInChildren = _anim.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform[] bones = componentsInChildren[i].bones;
			foreach (Transform item in bones)
			{
				hashSet.Add(item);
			}
		}
		foreach (Transform item2 in hashSet)
		{
			FindJiggles(ref jiggles, item2);
		}
		if (jiggles.Count == 0)
		{
			return;
		}
		foreach (Transform item3 in jiggles)
		{
			DynamicBone dynamicBone = item3.gameObject.AddComponent<DynamicBone>();
			Transform parent = item3.parent;
			while ((bool)parent)
			{
				if (BoneUtil.IsJiggleBone(parent))
				{
					dynamicBone.m_Root = parent;
					break;
				}
				parent = parent.parent;
			}
			if (parent == null)
			{
				dynamicBone.m_Root = item3.parent;
			}
			dynamicBone.m_Damping = 0.2f;
			dynamicBone.m_Elasticity = 0.05f;
			dynamicBone.m_Stiffness = 0.7f;
			dynamicBone.m_Inert = 0f;
			dynamicBone.m_Gravity = Vector3.down;
			dynamicBone.m_EndLength = 1f;
			dynamicBone.m_Radius = 0.003f / dynamicBone.m_Root.lossyScale.y;
			dynamicBone.m_DistanceToObject = 100f;
		}
	}

	private DynamicBoneColliderBase PlaceHairHeadCollider(Transform head)
	{
		DynamicBoneCollider dynamicBoneCollider = head.gameObject.AddComponent<DynamicBoneCollider>();
		Vector3 lossyScale = head.lossyScale;
		dynamicBoneCollider.m_Radius = 0.1f / lossyScale.y;
		dynamicBoneCollider.m_Center = new Vector3(0f, 0.1f / lossyScale.y, 0f);
		return dynamicBoneCollider;
	}

	private void PlaceTorsoCollider()
	{
		Transform boneTransform = _anim.GetBoneTransform(HumanBodyBones.UpperChest);
		if (!boneTransform)
		{
			boneTransform = _anim.GetBoneTransform(HumanBodyBones.Chest);
		}
		if ((bool)boneTransform)
		{
			DynamicBoneCollider dynamicBoneCollider = boneTransform.gameObject.AddComponent<DynamicBoneCollider>();
			Vector3 lossyScale = boneTransform.lossyScale;
			dynamicBoneCollider.m_Radius = 0.07f / lossyScale.y;
			dynamicBoneCollider.m_Height = 0.5f / lossyScale.y;
			_mTorsoCollider = dynamicBoneCollider;
		}
	}

	private void SetBreastPhysics()
	{
		List<Transform> breastBones;
		HashSet<Transform> exclusions;
		Transform transform = BoneUtil.FindAllBreastBones(_anim, out breastBones, out exclusions);
		if ((bool)transform)
		{
			CreateBreastJiggleScript(transform, exclusions);
			_breasts = breastBones;
		}
	}

	private void CreateBreastJiggleScript(Transform root, HashSet<Transform> exclusions)
	{
		DynamicBone dynamicBone = root.gameObject.AddComponent<DynamicBone>();
		dynamicBone.m_Root = root;
		dynamicBone.m_UpdateRate = 60f;
		dynamicBone.m_Damping = 0.2f;
		dynamicBone.m_Elasticity = 0.1f;
		dynamicBone.m_Stiffness = 0.1f;
		dynamicBone.m_Inert = 0.1f;
		dynamicBone.m_Radius = 0f;
		dynamicBone.m_EndLength = 5f;
		dynamicBone.m_EndOffset = new Vector3(-0.1f, -0.1f, 0.3f);
		dynamicBone.m_Gravity = new Vector3(0f, 0f, 0f);
		dynamicBone.m_DistanceToObject = 20f;
		dynamicBone.m_DistantDisable = false;
		dynamicBone.m_Exclusions = exclusions;
		dynamicBone.m_Colliders = new List<DynamicBoneColliderBase> { _mTorsoCollider };
		_breastController = dynamicBone;
		_breastController.UpdateParameters();
	}

	public void ColliderEnable(bool enable)
	{
		if (_colliderPairs == null)
		{
			return;
		}
		foreach (ColliderPair colliderPair in _colliderPairs)
		{
			colliderPair.Collider.enabled = enable;
		}
	}

	private void AddDestructionColliders()
	{
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.LeftFoot), true);
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.RightFoot), true);
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.Spine));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.Head));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.Hips));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.LeftLowerArm));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.RightLowerArm));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.RightLowerLeg));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.LeftHand));
		AddCollider(_anim.GetBoneTransform(HumanBodyBones.RightHand));
	}

	private void AddCollider(Transform bone, bool offset = false)
	{
		if ((bool)bone)
		{
			AddSphereCollider(bone, offset);
			Transform value;
			_boneToParentBoneMap.TryGetValue(bone, out value);
			if ((bool)value)
			{
				AddCapsuleCollider(bone, value);
			}
		}
	}

	private void AddSphereCollider(Transform bone, bool offset = false)
	{
		if ((bool)bone)
		{
			SphereCollider sphereCollider = new GameObject(bone.name + " collider").AddComponent<SphereCollider>();
			Transform transform = sphereCollider.transform;
			transform.SetParent(base.transform);
			float num = base.transform.parent.lossyScale.y * (1f / transform.parent.lossyScale.y);
			transform.localScale = Vector3.one * num;
			sphereCollider.transform.localPosition = Vector3.zero;
			sphereCollider.radius = 55f;
			_colliderPairs.Add(offset ? new ColliderPair(bone, sphereCollider, _anim.transform) : new ColliderPair(bone, sphereCollider));
		}
	}

	private void AddCapsuleCollider(Transform childBone, Transform parentBone)
	{
		CapsuleCollider capsuleCollider = new GameObject(parentBone.name + " collider").AddComponent<CapsuleCollider>();
		Transform transform = capsuleCollider.transform;
		transform.SetParent(parentBone);
		float num = base.transform.parent.lossyScale.y * (1f / transform.parent.lossyScale.y);
		transform.localScale = Vector3.one * num;
		Vector3 position = parentBone.position;
		Vector3 position2 = childBone.position;
		transform.position = Vector3.Lerp(position, position2, 0.5f);
		transform.LookAt(childBone);
		capsuleCollider.direction = 2;
		capsuleCollider.radius = 50f;
		capsuleCollider.height = Vector3.Distance(position, position2) / transform.lossyScale.y;
		_colliderPairs.Add(new ColliderPair(childBone, parentBone, capsuleCollider));
	}

	public void UpdateMass(float totalMass)
	{
		float mass = totalMass / (float)_colliderPairs.Count();
		foreach (ColliderPair colliderPair in _colliderPairs)
		{
			colliderPair.Rigidbody.mass = mass;
		}
	}
}
