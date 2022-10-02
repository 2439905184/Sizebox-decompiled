using System;
using System.Collections.Generic;
using MMD4MecanimInternal.Bullet;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MMD4MecanimModel : MonoBehaviour, MMD4MecanimAnim.IAnimModel
{
	public class IK
	{
		public class IKLink
		{
			public MMD4MecanimData.IKLinkData ikLinkData;

			public MMD4MecanimBone bone;
		}

		private int _ikID;

		private MMD4MecanimData.IKData _ikData;

		private MMD4MecanimBone _destBone;

		private MMD4MecanimBone _targetBone;

		private IKLink[] _ikLinkList;

		public int ikID
		{
			get
			{
				return _ikID;
			}
		}

		public MMD4MecanimData.IKData ikData
		{
			get
			{
				return _ikData;
			}
		}

		public MMD4MecanimBone destBone
		{
			get
			{
				return _destBone;
			}
		}

		public MMD4MecanimBone targetBone
		{
			get
			{
				return _targetBone;
			}
		}

		public IKLink[] ikLinkList
		{
			get
			{
				return _ikLinkList;
			}
		}

		public bool ikEnabled
		{
			get
			{
				if (_destBone != null)
				{
					return _destBone.ikEnabled;
				}
				return false;
			}
			set
			{
				if (_destBone != null)
				{
					_destBone.ikEnabled = value;
				}
			}
		}

		public float ikWeight
		{
			get
			{
				if (_destBone != null)
				{
					return _destBone.ikWeight;
				}
				return 0f;
			}
			set
			{
				if (_destBone != null)
				{
					_destBone.ikWeight = value;
				}
			}
		}

		public IK(MMD4MecanimModel model, int ikID)
		{
			if (model == null || model.modelData == null || model.modelData.ikDataList == null || ikID >= model.modelData.ikDataList.Length)
			{
				Debug.LogError("model:" + (model != null).ToString() + " modelData:" + (model != null && model.modelData != null).ToString() + " ikDataList:" + (model != null && model.modelData != null && model.modelData.ikDataList != null).ToString() + " ikID:" + ikID + " Length:" + ((model != null && model.modelData != null && model.modelData.ikDataList != null) ? model.modelData.ikDataList.Length : 0));
				return;
			}
			_ikID = ikID;
			_ikData = model.modelData.ikDataList[ikID];
			if (_ikData == null)
			{
				return;
			}
			_destBone = model.GetBone(_ikData.destBoneID);
			_targetBone = model.GetBone(_ikData.targetBoneID);
			if (_ikData.ikLinkDataList != null)
			{
				_ikLinkList = new IKLink[_ikData.ikLinkDataList.Length];
				for (int i = 0; i < _ikData.ikLinkDataList.Length; i++)
				{
					_ikLinkList[i] = new IKLink();
					_ikLinkList[i].ikLinkData = _ikData.ikLinkDataList[i];
				}
			}
		}

		public void Destroy()
		{
			_ikData = null;
			_destBone = null;
			_targetBone = null;
			_ikLinkList = null;
		}
	}

	public enum NEXTEdgePass
	{
		Pass4 = 0,
		Pass8 = 1
	}

	public enum PhysicsEngine
	{
		None = 0,
		BulletPhysics = 1
	}

	public class Morph : MMD4MecanimAnim.IMorph
	{
		public float weight;

		public float weight2;

		public float _animWeight;

		public float _appendWeight;

		public float _updateWeight;

		public float _updatedWeight;

		public MMD4MecanimData.MorphData morphData;

		public MMD4MecanimData.MorphAutoLumninousType morphAutoLuminousType;

		public MMD4MecanimData.MorphType morphType
		{
			get
			{
				if (morphData != null)
				{
					return morphData.morphType;
				}
				return MMD4MecanimData.MorphType.Group;
			}
		}

		public MMD4MecanimData.MorphCategory morphCategory
		{
			get
			{
				if (morphData != null)
				{
					return morphData.morphCategory;
				}
				return MMD4MecanimData.MorphCategory.Base;
			}
		}

		public string name
		{
			get
			{
				if (morphData != null)
				{
					return morphData.nameJp;
				}
				return null;
			}
		}

		public string translatedName
		{
			get
			{
				if (morphData != null)
				{
					return morphData.translatedName;
				}
				return null;
			}
		}

		string MMD4MecanimAnim.IMorph.name
		{
			get
			{
				return name;
			}
		}

		float MMD4MecanimAnim.IMorph.weight
		{
			get
			{
				return weight;
			}
			set
			{
				weight = value;
			}
		}
	}

	public class MorphAutoLuminous
	{
		public float lightUp;

		public float lightOff;

		public float lightBlink;

		public float lightBS;

		public bool updated;
	}

	[Serializable]
	public class RigidBody
	{
		[NonSerialized]
		public MMD4MecanimData.RigidBodyData rigidBodyData;

		public bool freezed = true;

		public int _freezedCached = -1;
	}

	[Serializable]
	public class Anim : MMD4MecanimAnim.Anim
	{
	}

	[Serializable]
	public class BulletPhysics
	{
		public bool joinLocalWorld = true;

		public bool useOriginalScale = true;

		public bool useCustomResetTime;

		public float resetMorphTime = 1.8f;

		public float resetWaitTime = 1.2f;

		public WorldProperty worldProperty;

		public MMDModelProperty mmdModelProperty;
	}

	public class CloneMesh
	{
		public SkinnedMeshRenderer skinnedMeshRenderer;

		public Mesh mesh;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Matrix4x4[] bindposes;

		public BoneWeight[] boneWeights;
	}

	public class CloneMaterial
	{
		public Material[] materials;

		public MMD4MecanimData.MorphMaterialData[] materialData;

		public MMD4MecanimData.MorphMaterialData[] backupMaterialData;

		public bool[] updateMaterialData;
	}

	private struct MorphBlendShape
	{
		public int[] blendShapeIndices;
	}

	public enum PPHType
	{
		Shoulder = 0
	}

	public enum EditorViewPage
	{
		Model = 0,
		Bone = 1,
		IK = 2,
		Morph = 3,
		Anim = 4,
		Physics = 5
	}

	public enum EditorViewMorphNameType
	{
		Japanese = 0,
		English = 1,
		Translated = 2
	}

	private Animator _animator;

	private Anim _playingAnim;

	private float _prevDeltaTime;

	private float[] _animMorphCategoryWeights;

	private int _delayedAwakeFrame;

	private bool _isDelayedAwake;

	private MMD4MecanimBone _leftShoulderBone;

	private MMD4MecanimBone _rightShoulderBone;

	private MMD4MecanimBone _leftArmBone;

	private MMD4MecanimBone _rightArmBone;

	[NonSerialized]
	public MMD4MecanimIKTarget[] fullBodyIKTargetList;

	[NonSerialized]
	public Transform fullBodyIKTargets;

	[NonSerialized]
	public Transform[] fullBodyIKGroupList;

	public const float NEXTEdgeScale = 0.05f;

	public Material nextEdgeMaterial_Pass4;

	public Material nextEdgeMaterial_Pass8;

	public NEXTEdgePass nextEdgePass;

	public float nextEdgeSize = 1f;

	public Color nextEdgeColor = new Color(0.4f, 1f, 1f, 1f);

	private MeshRenderer[] _nextEdgeMeshRenderers;

	private SkinnedMeshRenderer[] _nextEdgeSkinnedMeshRenderers;

	private bool _nextEdgeVisibleCached;

	private float _nextEdgeSizeCached;

	private Color _nextEdgeColorCached;

	public bool supportNEXTEdge;

	private bool _notFoundShaderNEXTPass4;

	private bool _notFoundShaderNEXTPass8;

	public const int MaterialEdgeRenderQueue = 2001;

	public const int MaterialBaseRenderQueue = 2002;

	public const int MaterialEdgeRenderQueue_AfterSkybox = 2501;

	public const int MaterialBaseRenderQueue_AfterSkybox = 2502;

	public float importScale = 0.01f;

	public bool initializeOnAwake;

	public bool postfixRenderQueue = true;

	public bool renderQueueAfterSkybox;

	public bool updateWhenOffscreen = true;

	public bool animEnabled = true;

	public bool animSyncToAudio = true;

	public TextAsset modelFile;

	public TextAsset indexFile;

	public TextAsset vertexFile;

	public AudioSource audioSource;

	public int animDelayedAwakeFrame = 1;

	public bool boneInherenceEnabled;

	public bool boneMorphEnabled;

	public bool pphEnabled;

	public bool pphEnabledNoAnimation;

	public bool pphShoulderEnabled = true;

	public float pphShoulderFixRate = 0.7f;

	public bool ikEnabled;

	public bool fullBodyIKEnabled;

	public bool vertexMorphEnabled = true;

	public bool materialMorphEnabled = true;

	public bool blendShapesEnabled = true;

	public bool xdefEnabled;

	public bool xdefNormalEnabled;

	public bool xdefMobileEnabled;

	private bool _blendShapesEnabledCache;

	private float _pphShoulderFixRateImmediately;

	public float generatedColliderMargin = 0.01f;

	[NonSerialized]
	public MMD4MecanimBone[] boneList;

	[NonSerialized]
	public IK[] ikList;

	[NonSerialized]
	public Morph[] morphList;

	[NonSerialized]
	public MorphAutoLuminous morphAutoLuminous;

	public PhysicsEngine physicsEngine;

	private PhysicsEngine _physicsEngineCached;

	public BulletPhysics bulletPhysics;

	public RigidBody[] rigidBodyList;

	public Anim[] animList;

	private bool _initialized;

	private MMD4MecanimBone _rootBone;

	private MMD4MecanimBone[] _sortedBoneList;

	private List<MMD4MecanimBone> _processingBoneList;

	private MeshRenderer[] _meshRenderers;

	private SkinnedMeshRenderer[] _skinnedMeshRenderers;

	private CloneMesh[] _cloneMeshes;

	private MorphBlendShape[] _morphBlendShapes;

	private CloneMaterial[] _cloneMaterials;

	private bool _supportDeferred;

	private Light _deferredLight;

	public MMD4MecanimData.ModelData _modelData;

	[HideInInspector]
	public EditorViewPage editorViewPage;

	[HideInInspector]
	public byte editorViewMorphBits = 15;

	[HideInInspector]
	public bool editorViewRigidBodies;

	[HideInInspector]
	public EditorViewMorphNameType editorViewMorphNameType;

	[NonSerialized]
	public Mesh defaultMesh;

	private MMD4MecanimBulletPhysics.MMDModel _bulletPhysicsMMDModel;

	public Action onUpdating;

	public Action onUpdated;

	public Action onLateUpdating;

	public Action onLateUpdated;

	int MMD4MecanimAnim.IAnimModel.morphCount
	{
		get
		{
			if (morphList != null)
			{
				return morphList.Length;
			}
			return 0;
		}
	}

	int MMD4MecanimAnim.IAnimModel.animCount
	{
		get
		{
			if (animList != null)
			{
				return animList.Length;
			}
			return 0;
		}
	}

	Animator MMD4MecanimAnim.IAnimModel.animator
	{
		get
		{
			return _animator;
		}
	}

	AudioSource MMD4MecanimAnim.IAnimModel.audioSource
	{
		get
		{
			if (audioSource == null)
			{
				audioSource = MMD4MecanimCommon.WeakAddComponent<AudioSource>(base.gameObject);
			}
			return audioSource;
		}
	}

	bool MMD4MecanimAnim.IAnimModel.animSyncToAudio
	{
		get
		{
			return animSyncToAudio;
		}
	}

	float MMD4MecanimAnim.IAnimModel.prevDeltaTime
	{
		get
		{
			return _prevDeltaTime;
		}
		set
		{
			_prevDeltaTime = value;
		}
	}

	MMD4MecanimAnim.IAnim MMD4MecanimAnim.IAnimModel.playingAnim
	{
		get
		{
			return _playingAnim;
		}
		set
		{
			_playingAnim = (Anim)value;
		}
	}

	public int _skinnedMeshCount
	{
		get
		{
			if (_skinnedMeshRenderers != null)
			{
				return _skinnedMeshRenderers.Length;
			}
			return 0;
		}
	}

	public int _cloneMeshCount
	{
		get
		{
			if (_cloneMeshes != null)
			{
				return _cloneMeshes.Length;
			}
			return 0;
		}
	}

	public float pphShoulderFixRateImmediately
	{
		get
		{
			return _pphShoulderFixRateImmediately;
		}
	}

	public MMD4MecanimData.ModelData modelData
	{
		get
		{
			return _modelData;
		}
	}

	public byte[] modelFileBytes
	{
		get
		{
			if (!(modelFile != null))
			{
				return null;
			}
			return modelFile.bytes;
		}
	}

	public byte[] indexFileBytes
	{
		get
		{
			if (!(indexFile != null))
			{
				return null;
			}
			return indexFile.bytes;
		}
	}

	public byte[] vertexFileBytes
	{
		get
		{
			if (!(vertexFile != null))
			{
				return null;
			}
			return vertexFile.bytes;
		}
	}

	public bool skinningEnabled
	{
		get
		{
			if (_skinnedMeshRenderers != null)
			{
				return _skinnedMeshRenderers.Length != 0;
			}
			return false;
		}
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorph(string name)
	{
		return GetMorph(name);
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorph(string name, bool startsWith)
	{
		return GetMorph(name, startsWith);
	}

	MMD4MecanimAnim.IMorph MMD4MecanimAnim.IAnimModel.GetMorphAt(int index)
	{
		if (morphList != null && index >= 0 && index < morphList.Length)
		{
			return morphList[index];
		}
		return null;
	}

	MMD4MecanimAnim.IAnim MMD4MecanimAnim.IAnimModel.GetAnimAt(int index)
	{
		if (animList != null && index >= 0 && index < animList.Length)
		{
			return animList[index];
		}
		return null;
	}

	void MMD4MecanimAnim.IAnimModel._SetAnimMorphWeight(MMD4MecanimAnim.IMorph morph, float weight)
	{
		((Morph)morph)._animWeight = weight;
	}

	private void _InitializeAnimatoion()
	{
		_animator = GetComponent<Animator>();
		if (Application.isPlaying && animEnabled && animDelayedAwakeFrame > 0 && _animator != null && _animator.enabled)
		{
			_animator.speed = 0f;
			_isDelayedAwake = true;
		}
		_animMorphCategoryWeights = new float[5];
		if (Application.isPlaying)
		{
			MMD4MecanimAnim.InitializeAnimModel(this);
		}
	}

	private void _UpdateAnim()
	{
		if (!Application.isPlaying || _bulletPhysicsMMDModel == null)
		{
			return;
		}
		if (_isDelayedAwake)
		{
			if (animEnabled && animDelayedAwakeFrame > 0 && ++_delayedAwakeFrame <= animDelayedAwakeFrame)
			{
				return;
			}
			_isDelayedAwake = false;
			_animator.speed = 1f;
		}
		MMD4MecanimAnim.PreUpdateAnimModel(this);
		if (!animEnabled)
		{
			MMD4MecanimAnim.StopAnimModel(this);
		}
		else
		{
			MMD4MecanimAnim.UpdateAnimModel(this);
		}
		MMD4MecanimAnim.PostUpdateAnimModel(this);
	}

	private void _BindBone()
	{
		foreach (Transform item in base.gameObject.transform)
		{
			_BindBone(item);
		}
	}

	private void _BindBone(Transform trn)
	{
		if (!string.IsNullOrEmpty(trn.gameObject.name))
		{
			int value = 0;
			if (_modelData.boneDataDictionary.TryGetValue(trn.gameObject.name, out value))
			{
				MMD4MecanimBone mMD4MecanimBone = trn.gameObject.GetComponent<MMD4MecanimBone>();
				if (mMD4MecanimBone == null)
				{
					mMD4MecanimBone = trn.gameObject.AddComponent<MMD4MecanimBone>();
				}
				mMD4MecanimBone.model = this;
				mMD4MecanimBone.boneID = value;
				mMD4MecanimBone.Setup();
				boneList[value] = mMD4MecanimBone;
				if (boneList[value].boneData != null && boneList[value].boneData.isRootBone)
				{
					_rootBone = boneList[value];
				}
			}
		}
		foreach (Transform item in trn)
		{
			_BindBone(item);
		}
	}

	private void _UpdateBone()
	{
	}

	private void _LateUpdateBone()
	{
		_UpdatePPHBones();
	}

	private void _InitializePPHBones()
	{
		if (_animator == null || _animator.avatar == null || !_animator.avatar.isValid || !_animator.avatar.isHuman)
		{
			return;
		}
		Transform boneTransform = _animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
		Transform boneTransform2 = _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
		if (boneTransform != null && boneTransform2 != null)
		{
			_leftShoulderBone = boneTransform.gameObject.GetComponent<MMD4MecanimBone>();
			_leftArmBone = boneTransform2.gameObject.GetComponent<MMD4MecanimBone>();
			if (_leftShoulderBone != null)
			{
				_leftShoulderBone.humanBodyBones = 11;
			}
			if (_leftArmBone != null)
			{
				_leftArmBone.humanBodyBones = 13;
			}
		}
		Transform boneTransform3 = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);
		Transform boneTransform4 = _animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
		if (boneTransform3 != null && boneTransform4 != null)
		{
			_rightShoulderBone = boneTransform3.gameObject.GetComponent<MMD4MecanimBone>();
			_rightArmBone = boneTransform4.gameObject.GetComponent<MMD4MecanimBone>();
			if (_rightShoulderBone != null)
			{
				_rightShoulderBone.humanBodyBones = 12;
			}
			if (_rightArmBone != null)
			{
				_rightArmBone.humanBodyBones = 14;
			}
		}
	}

	public void ForceUpdatePPHBones()
	{
		_UpdatePPHBones();
	}

	private void _UpdatePPHBones()
	{
		_pphShoulderFixRateImmediately = 0f;
	}

	private void _InitializeDeferredMaterial()
	{
	}

	private void _DestroyDeferredMaterial()
	{
	}

	private void _InitializeDeferredMesh()
	{
	}

	private static GameObject _CreateDeferrecClearGameObject(GameObject parentGameObject)
	{
		return null;
	}

	private Material[] _CloneDeferredClearMaterials(Material[] materials)
	{
		return null;
	}

	private void _UpdatedDeffered()
	{
	}

	private void _SetDeferredShaderSettings(Light directionalLight)
	{
	}

	private void _InitializeIK()
	{
		_CheckTransformIK();
	}

	private void _CheckTransformIK()
	{
		if (!Application.isPlaying || !fullBodyIKEnabled)
		{
			return;
		}
		if (fullBodyIKTargetList != null)
		{
			if (fullBodyIKTargetList.Length != 36)
			{
				fullBodyIKTargetList = null;
			}
			else
			{
				for (int i = 0; i != fullBodyIKTargetList.Length; i++)
				{
					if (fullBodyIKTargetList[i] == null)
					{
						fullBodyIKTargetList = null;
						break;
					}
				}
			}
		}
		if (fullBodyIKTargetList != null || _bulletPhysicsMMDModel == null)
		{
			return;
		}
		Vector3[] fullBodyIKPositionList = _bulletPhysicsMMDModel.fullBodyIKPositionList;
		Quaternion[] fullBodyIKRotationList = _bulletPhysicsMMDModel.fullBodyIKRotationList;
		if (fullBodyIKPositionList == null || fullBodyIKRotationList == null || fullBodyIKPositionList.Length != 36 || fullBodyIKRotationList.Length != 36)
		{
			return;
		}
		if (fullBodyIKTargets == null)
		{
			GameObject gameObject = new GameObject("FullBodyIKTargets");
			fullBodyIKTargets = gameObject.transform;
			fullBodyIKTargets.parent = base.transform;
			fullBodyIKTargets.localPosition = Vector3.zero;
			fullBodyIKTargets.localRotation = Quaternion.identity;
			fullBodyIKTargets.localScale = Vector3.one;
		}
		if (fullBodyIKGroupList == null || fullBodyIKGroupList.Length != 8)
		{
			fullBodyIKGroupList = new Transform[8];
			for (int j = 0; j != 8; j++)
			{
				MMD4MecanimData.FullBodyIKGroup fullBodyIKGroup = (MMD4MecanimData.FullBodyIKGroup)j;
				GameObject gameObject2 = new GameObject(fullBodyIKGroup.ToString());
				fullBodyIKGroupList[j] = gameObject2.transform;
				fullBodyIKGroupList[j].parent = fullBodyIKTargets;
				fullBodyIKGroupList[j].localPosition = Vector3.zero;
				fullBodyIKGroupList[j].localRotation = Quaternion.identity;
				fullBodyIKGroupList[j].localScale = Vector3.one;
			}
		}
		MMD4MecanimBone rootBone = GetRootBone();
		Transform transform = null;
		if (rootBone != null)
		{
			transform = rootBone.transform;
		}
		fullBodyIKTargetList = new MMD4MecanimIKTarget[36];
		for (int k = 0; k != 36; k++)
		{
			MMD4MecanimData.FullBodyIKGroup fullBodyIKGroup2;
			string fullBodyIKTargetName = MMD4MecanimData.GetFullBodyIKTargetName(out fullBodyIKGroup2, k);
			if (fullBodyIKGroup2 != MMD4MecanimData.FullBodyIKGroup.Unknown)
			{
				GameObject gameObject3 = new GameObject(fullBodyIKTargetName);
				Transform obj = gameObject3.transform;
				Vector3 position = fullBodyIKPositionList[k];
				Quaternion quaternion = fullBodyIKRotationList[k];
				if (transform != null)
				{
					position = transform.TransformPoint(position);
					quaternion = transform.rotation * quaternion;
				}
				obj.parent = fullBodyIKGroupList[(int)fullBodyIKGroup2];
				obj.position = position;
				obj.rotation = quaternion;
				obj.localScale = Vector3.one;
				fullBodyIKTargetList[k] = gameObject3.AddComponent<MMD4MecanimIKTarget>();
				fullBodyIKTargetList[k].model = this;
			}
		}
	}

	private void _UpdateIK()
	{
		_CheckTransformIK();
	}

	private void _LateUpdateIK()
	{
		if (!Application.isPlaying || _bulletPhysicsMMDModel == null)
		{
			return;
		}
		int[] updateFullBodyIKFlagsList = _bulletPhysicsMMDModel.updateFullBodyIKFlagsList;
		float[] updateFullBodyIKTransformList = _bulletPhysicsMMDModel.updateFullBodyIKTransformList;
		float[] updateFullBodyIKWeightList = _bulletPhysicsMMDModel.updateFullBodyIKWeightList;
		if (updateFullBodyIKFlagsList == null || updateFullBodyIKTransformList == null || updateFullBodyIKWeightList == null || updateFullBodyIKFlagsList.Length != 36 || updateFullBodyIKTransformList.Length != 144 || updateFullBodyIKWeightList.Length != 36 || fullBodyIKTargetList == null || fullBodyIKTargetList.Length != 36)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		while (num != 36)
		{
			MMD4MecanimIKTarget mMD4MecanimIKTarget = fullBodyIKTargetList[num];
			if (mMD4MecanimIKTarget != null)
			{
				Vector3 position = mMD4MecanimIKTarget.transform.position;
				updateFullBodyIKTransformList[num2] = position.x;
				updateFullBodyIKTransformList[num2 + 1] = position.y;
				updateFullBodyIKTransformList[num2 + 2] = position.z;
				updateFullBodyIKTransformList[num2 + 3] = 1f;
				updateFullBodyIKWeightList[num] = mMD4MecanimIKTarget.ikWeight;
			}
			num++;
			num2 += 4;
		}
	}

	private void _UpdateAmbientPreview()
	{
	}

	private void _UpdateAmbientPreviewInternal(Material[] materials)
	{
	}

	private void _InitializeGlobalAmbient()
	{
	}

	private void _UpdateGlobalAmbient()
	{
	}

	private void _CleanupGlobalAmbient()
	{
	}

	private void _UpdateAutoLuminous()
	{
		if (morphAutoLuminous == null || !morphAutoLuminous.updated || _cloneMaterials == null)
		{
			return;
		}
		for (int i = 0; i != _cloneMaterials.Length; i++)
		{
			CloneMaterial cloneMaterial = _cloneMaterials[i];
			if (cloneMaterial.updateMaterialData == null || cloneMaterial.materialData == null || cloneMaterial.materials == null)
			{
				continue;
			}
			for (int j = 0; j < cloneMaterial.updateMaterialData.Length; j++)
			{
				if (!cloneMaterial.updateMaterialData[j] && cloneMaterial.materialData[j].shininess > 100f)
				{
					cloneMaterial.updateMaterialData[j] = true;
				}
			}
		}
	}

	private void _CleanupAutoLuminous()
	{
		if (morphAutoLuminous != null)
		{
			morphAutoLuminous.updated = false;
		}
	}

	public void _UploadMeshVertex(int meshID, Vector3[] vertices, Vector3[] normals)
	{
		if (_cloneMeshes == null || meshID < 0 || meshID >= _cloneMeshes.Length)
		{
			return;
		}
		CloneMesh cloneMesh = _cloneMeshes[meshID];
		if (cloneMesh != null)
		{
			if (vertices != null)
			{
				cloneMesh.mesh.vertices = vertices;
			}
			if (normals != null)
			{
				cloneMesh.mesh.normals = normals;
			}
		}
	}

	private void _UploadMeshMaterial()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		_UpdateGlobalAmbient();
		_UpdateAutoLuminous();
		if (_cloneMaterials != null)
		{
			for (int i = 0; i != _cloneMaterials.Length; i++)
			{
				CloneMaterial cloneMaterial = _cloneMaterials[i];
				if (cloneMaterial.updateMaterialData == null || cloneMaterial.materialData == null || cloneMaterial.materials == null)
				{
					continue;
				}
				for (int j = 0; j < cloneMaterial.updateMaterialData.Length; j++)
				{
					if (cloneMaterial.updateMaterialData[j])
					{
						cloneMaterial.updateMaterialData[j] = false;
						MMD4MecanimCommon.FeedbackMaterial(ref cloneMaterial.materialData[j], cloneMaterial.materials[j], morphAutoLuminous);
					}
				}
			}
		}
		_CleanupGlobalAmbient();
		_CleanupAutoLuminous();
	}

	public int[] _PrepareMeshFlags()
	{
		if (_skinnedMeshRenderers != null)
		{
			int num = _skinnedMeshRenderers.Length;
			if (num > 0)
			{
				return new int[num];
			}
		}
		return null;
	}

	public int[] _PrepareMeshFlags(out bool blendShapesAnything)
	{
		blendShapesAnything = false;
		if (_skinnedMeshRenderers != null)
		{
			int num = _skinnedMeshRenderers.Length;
			if (num > 0)
			{
				int[] array = new int[num];
				if (vertexMorphEnabled && blendShapesEnabled)
				{
					for (int i = 0; i != num; i++)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[i];
						if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null && skinnedMeshRenderer.sharedMesh.blendShapeCount != 0)
						{
							array[i] |= 1;
							blendShapesAnything = true;
						}
					}
				}
				return array;
			}
		}
		return null;
	}

	public void _InitializeCloneMesh(int[] meshFlags)
	{
		int num = 0;
		int num2 = 0;
		if (_meshRenderers != null)
		{
			num2 += _meshRenderers.Length;
		}
		if (_skinnedMeshRenderers != null)
		{
			num2 += _skinnedMeshRenderers.Length;
		}
		if (num2 > 0)
		{
			_cloneMaterials = new CloneMaterial[num2];
		}
		if (_meshRenderers != null)
		{
			for (int i = 0; i != _meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = _meshRenderers[i];
				Material[] array = null;
				if (Application.isPlaying)
				{
					array = meshRenderer.materials;
				}
				if (array == null)
				{
					array = meshRenderer.sharedMaterials;
				}
				_PostfixRenderQueue(array, postfixRenderQueue, renderQueueAfterSkybox);
				_cloneMaterials[num] = new CloneMaterial();
				_SetupCloneMaterial(_cloneMaterials[num], array);
				num++;
			}
		}
		bool flag = false;
		if (meshFlags != null && _skinnedMeshRenderers != null && meshFlags.Length == _skinnedMeshRenderers.Length && Application.isPlaying)
		{
			for (int j = 0; j != meshFlags.Length; j++)
			{
				if (flag)
				{
					break;
				}
				flag = (meshFlags[j] & 6) != 0;
			}
		}
		if (_skinnedMeshRenderers != null)
		{
			if (flag)
			{
				_cloneMeshes = new CloneMesh[_skinnedMeshRenderers.Length];
			}
			for (int k = 0; k != _skinnedMeshRenderers.Length; k++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[k];
				if (((uint)meshFlags[k] & 6u) != 0)
				{
					_cloneMeshes[k] = new CloneMesh();
					MMD4MecanimCommon.CloneMeshWork cloneMeshWork = MMD4MecanimCommon.CloneMesh(skinnedMeshRenderer.sharedMesh);
					if (cloneMeshWork != null && cloneMeshWork.mesh != null)
					{
						_cloneMeshes[k].skinnedMeshRenderer = skinnedMeshRenderer;
						_cloneMeshes[k].mesh = cloneMeshWork.mesh;
						if (_cloneMeshes[k].mesh != null)
						{
							_cloneMeshes[k].mesh.MarkDynamic();
						}
						_cloneMeshes[k].vertices = cloneMeshWork.vertices;
						if (((uint)meshFlags[k] & 4u) != 0)
						{
							if (xdefNormalEnabled)
							{
								_cloneMeshes[k].normals = cloneMeshWork.normals;
							}
							_cloneMeshes[k].bindposes = cloneMeshWork.bindposes;
							_cloneMeshes[k].boneWeights = cloneMeshWork.boneWeights;
						}
						skinnedMeshRenderer.sharedMesh = _cloneMeshes[k].mesh;
					}
					else
					{
						Debug.LogError("CloneMesh() Failed. : " + base.gameObject.name);
					}
				}
				Material[] array2 = null;
				if (Application.isPlaying)
				{
					array2 = skinnedMeshRenderer.materials;
				}
				if (array2 == null)
				{
					array2 = skinnedMeshRenderer.sharedMaterials;
				}
				_PostfixRenderQueue(array2, postfixRenderQueue, renderQueueAfterSkybox);
				_cloneMaterials[num] = new CloneMaterial();
				_SetupCloneMaterial(_cloneMaterials[num], array2);
				num++;
			}
		}
		if (_cloneMaterials == null)
		{
			return;
		}
		for (int l = 0; l != _cloneMaterials.Length; l++)
		{
			Material[] materials = _cloneMaterials[l].materials;
			if (materials == null)
			{
				continue;
			}
			for (int m = 0; m != materials.Length; m++)
			{
				if (MMD4MecanimCommon.IsDeferredShader(materials[m]))
				{
					_supportDeferred = true;
					break;
				}
			}
			if (_supportDeferred)
			{
				break;
			}
		}
	}

	public CloneMesh _GetCloneMesh(int meshIndex)
	{
		if (_cloneMeshes != null && meshIndex >= 0 && meshIndex < _cloneMeshes.Length)
		{
			return _cloneMeshes[meshIndex];
		}
		return null;
	}

	public void _CleanupCloneMesh()
	{
		if (_cloneMeshes == null)
		{
			return;
		}
		for (int i = 0; i != _cloneMeshes.Length; i++)
		{
			if (_cloneMeshes[i] != null)
			{
				if (!xdefNormalEnabled)
				{
					_cloneMeshes[i].normals = null;
				}
				_cloneMeshes[i].bindposes = null;
				_cloneMeshes[i].boneWeights = null;
			}
		}
	}

	public Morph GetMorph(string morphName)
	{
		return GetMorph(morphName, false);
	}

	public Morph GetMorph(string morphName, bool isStartsWith)
	{
		if (modelData != null)
		{
			int morphDataIndexJp = modelData.GetMorphDataIndexJp(morphName, isStartsWith);
			if (morphDataIndexJp != -1)
			{
				return morphList[morphDataIndexJp];
			}
			if (!isStartsWith)
			{
				morphDataIndexJp = modelData.GetMorphDataIndexEn(morphName, false);
				if (morphDataIndexJp != -1)
				{
					return morphList[morphDataIndexJp];
				}
				morphDataIndexJp = modelData.GetTranslatedMorphDataIndex(morphName, false);
				if (morphDataIndexJp != -1)
				{
					return morphList[morphDataIndexJp];
				}
			}
		}
		return null;
	}

	public void ForceUpdateMorph()
	{
		_UpdateMorph();
	}

	private void _UpdateMorph()
	{
		if (morphList == null || _animMorphCategoryWeights == null)
		{
			return;
		}
		for (int i = 0; i != _animMorphCategoryWeights.Length; i++)
		{
			_animMorphCategoryWeights[i] = 1f;
		}
		for (int j = 0; j != morphList.Length; j++)
		{
			Morph morph = morphList[j];
			MMD4MecanimData.MorphCategory morphCategory = morph.morphCategory;
			if ((uint)(morphCategory - 1) <= 2u && morph.weight2 != 0f)
			{
				if (morph.weight2 == 1f)
				{
					_animMorphCategoryWeights[(int)morph.morphCategory] = 0f;
				}
				else
				{
					_animMorphCategoryWeights[(int)morph.morphCategory] = Mathf.Min(_animMorphCategoryWeights[(int)morph.morphCategory], 1f - morph.weight2);
				}
			}
		}
		bool flag = false;
		for (int k = 0; k != morphList.Length; k++)
		{
			morphList[k]._updateWeight = _GetMorphUpdateWeight(morphList[k], _animMorphCategoryWeights);
			flag |= morphList[k]._updateWeight != morphList[k]._updatedWeight;
		}
		if (!flag)
		{
			return;
		}
		for (int l = 0; l != morphList.Length; l++)
		{
			morphList[l]._appendWeight = 0f;
		}
		if (_modelData == null || _modelData.morphDataList == null)
		{
			return;
		}
		bool flag2 = false;
		for (int m = 0; m != _modelData.morphDataList.Length; m++)
		{
			if (_modelData.morphDataList[m].morphType == MMD4MecanimData.MorphType.Group)
			{
				flag2 = true;
				_ApplyMorph(m);
			}
		}
		for (int n = 0; n != morphList.Length; n++)
		{
			if (_modelData.morphDataList[n].morphType != 0)
			{
				if (flag2)
				{
					morphList[n]._updateWeight = _GetMorphUpdateWeight(morphList[n], _animMorphCategoryWeights);
				}
				_ApplyMorph(n);
			}
		}
	}

	private void _ApplyMorph(int morphIndex)
	{
		if ((_morphBlendShapes == null && _cloneMeshes == null) || morphList == null || (uint)morphIndex >= (uint)morphList.Length)
		{
			return;
		}
		Morph morph = morphList[morphIndex];
		if (morph == null)
		{
			return;
		}
		float num = (morph._updatedWeight = morph._updateWeight);
		if (_modelData == null || _modelData.morphDataList == null || (uint)morphIndex >= (uint)_modelData.morphDataList.Length)
		{
			return;
		}
		MMD4MecanimData.MorphData morphData = _modelData.morphDataList[morphIndex];
		if (morphAutoLuminous != null && morph.morphAutoLuminousType != 0)
		{
			switch (morph.morphAutoLuminousType)
			{
			case MMD4MecanimData.MorphAutoLumninousType.LightUp:
				if (morphAutoLuminous.lightUp != num)
				{
					morphAutoLuminous.lightUp = num;
					morphAutoLuminous.updated = true;
				}
				break;
			case MMD4MecanimData.MorphAutoLumninousType.LightOff:
				if (morphAutoLuminous.lightOff != num)
				{
					morphAutoLuminous.lightOff = num;
					morphAutoLuminous.updated = true;
				}
				break;
			case MMD4MecanimData.MorphAutoLumninousType.LightBlink:
				if (morphAutoLuminous.lightBlink != num)
				{
					morphAutoLuminous.lightBlink = num;
					morphAutoLuminous.updated = true;
				}
				break;
			case MMD4MecanimData.MorphAutoLumninousType.LightBS:
				if (morphAutoLuminous.lightBS != num)
				{
					morphAutoLuminous.lightBS = num;
					morphAutoLuminous.updated = true;
				}
				break;
			}
		}
		if (morphData.morphType == MMD4MecanimData.MorphType.Group)
		{
			if (morphData.indices == null)
			{
				return;
			}
			for (int i = 0; i < morphData.indices.Length; i++)
			{
				int num2 = morphData.indices[i];
				if (num2 < morphList.Length)
				{
					morphList[num2]._appendWeight += num;
				}
			}
		}
		else if (morphData.morphType == MMD4MecanimData.MorphType.Vertex)
		{
			if (!_blendShapesEnabledCache || _morphBlendShapes == null)
			{
				return;
			}
			num *= 100f;
			if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Length != _morphBlendShapes.Length)
			{
				return;
			}
			for (int j = 0; j != _morphBlendShapes.Length; j++)
			{
				if (_morphBlendShapes[j].blendShapeIndices != null && morphIndex < _morphBlendShapes[j].blendShapeIndices.Length)
				{
					int num3 = _morphBlendShapes[j].blendShapeIndices[morphIndex];
					if (num3 != -1 && _skinnedMeshRenderers[j] != null)
					{
						_skinnedMeshRenderers[j].SetBlendShapeWeight(num3, num);
					}
				}
			}
		}
		else if (morphData.morphType == MMD4MecanimData.MorphType.Material && morphData.materialData != null && materialMorphEnabled)
		{
			for (int k = 0; k != morphData.materialData.Length; k++)
			{
				_ApplyMaterialData(ref morphData.materialData[k], num);
			}
		}
	}

	private void _ApplyMaterialData(ref MMD4MecanimData.MorphMaterialData morphMaterialData, float weight)
	{
		if (_cloneMaterials == null)
		{
			return;
		}
		for (int i = 0; i != _cloneMaterials.Length; i++)
		{
			CloneMaterial cloneMaterial = _cloneMaterials[i];
			if (cloneMaterial.backupMaterialData == null || cloneMaterial.updateMaterialData == null || cloneMaterial.materialData == null || cloneMaterial.materials == null)
			{
				continue;
			}
			for (int j = 0; j < cloneMaterial.updateMaterialData.Length; j++)
			{
				if (cloneMaterial.backupMaterialData[j].materialID == morphMaterialData.materialID)
				{
					if (!cloneMaterial.updateMaterialData[j])
					{
						cloneMaterial.updateMaterialData[j] = true;
						cloneMaterial.materialData[j] = cloneMaterial.backupMaterialData[j];
					}
					MMD4MecanimCommon.OperationMaterial(ref cloneMaterial.materialData[j], ref morphMaterialData, weight);
				}
			}
		}
	}

	private static float _FastScl(float weight, float weight2)
	{
		if (weight2 == 0f)
		{
			return 0f;
		}
		if (weight2 == 1f)
		{
			return weight;
		}
		return weight * weight2;
	}

	private static float _GetMorphUpdateWeight(Morph morph, float[] animMorphCategoryWeights)
	{
		float b = animMorphCategoryWeights[(int)morph.morphCategory];
		float weight = Mathf.Min(1f - morph.weight2, b);
		return Mathf.Min(1f, Mathf.Max(morph.weight, _FastScl(morph._animWeight + morph._appendWeight, weight)));
	}

	private void _InitializeNEXTMaterial()
	{
		if ((nextEdgeMaterial_Pass4 == null || nextEdgeMaterial_Pass4.shader == null) && !_notFoundShaderNEXTPass4)
		{
			Shader shader = Shader.Find("MMD4Mecanim/MMDLit-NEXTEdge-Pass4");
			if (shader != null)
			{
				nextEdgeMaterial_Pass4 = new Material(shader);
			}
			else
			{
				_notFoundShaderNEXTPass4 = true;
			}
		}
		if ((nextEdgeMaterial_Pass8 == null || nextEdgeMaterial_Pass8.shader == null) && !_notFoundShaderNEXTPass8)
		{
			Shader shader2 = Shader.Find("MMD4Mecanim/MMDLit-NEXTEdge-Pass8");
			if (shader2 != null)
			{
				nextEdgeMaterial_Pass8 = new Material(shader2);
			}
			else
			{
				_notFoundShaderNEXTPass8 = true;
			}
		}
		_PostfixNEXTMaterial();
	}

	private void _DestroyNEXTMaterial()
	{
		if (nextEdgeMaterial_Pass4 != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(nextEdgeMaterial_Pass4);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(nextEdgeMaterial_Pass4);
			}
			nextEdgeMaterial_Pass4 = null;
		}
		if (nextEdgeMaterial_Pass8 != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(nextEdgeMaterial_Pass8);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(nextEdgeMaterial_Pass8);
			}
			nextEdgeMaterial_Pass8 = null;
		}
	}

	private void _PostfixNEXTMaterial()
	{
		if (postfixRenderQueue)
		{
			int renderQueue = (renderQueueAfterSkybox ? 2501 : 2001);
			if (nextEdgeMaterial_Pass4 != null && nextEdgeMaterial_Pass4.GetFloat("_PostfixRenderQueue") <= 0.5f)
			{
				nextEdgeMaterial_Pass4.SetFloat("_PostfixRenderQueue", 1f);
				nextEdgeMaterial_Pass4.renderQueue = renderQueue;
			}
			if (nextEdgeMaterial_Pass8 != null && nextEdgeMaterial_Pass8.GetFloat("_PostfixRenderQueue") <= 0.5f)
			{
				nextEdgeMaterial_Pass8.SetFloat("_PostfixRenderQueue", 1f);
				nextEdgeMaterial_Pass8.renderQueue = renderQueue;
			}
		}
		else
		{
			if (nextEdgeMaterial_Pass4 != null && nextEdgeMaterial_Pass4.GetFloat("_PostfixRenderQueue") > 0.5f)
			{
				nextEdgeMaterial_Pass4.SetFloat("_PostfixRenderQueue", 0f);
				nextEdgeMaterial_Pass4.renderQueue = -1;
			}
			if (nextEdgeMaterial_Pass8 != null && nextEdgeMaterial_Pass8.GetFloat("_PostfixRenderQueue") > 0.5f)
			{
				nextEdgeMaterial_Pass8.SetFloat("_PostfixRenderQueue", 0f);
				nextEdgeMaterial_Pass8.renderQueue = -1;
			}
		}
	}

	private void _InitializeNEXTEdgeMesh()
	{
		if (!supportNEXTEdge)
		{
			return;
		}
		_InitializeNEXTMaterial();
		if (nextEdgeMaterial_Pass4 == null)
		{
			Debug.LogWarning("nextEdgeMaterial_Pass4 is null. Skipped _InitializenextEdgeMesh().");
			return;
		}
		if (nextEdgeMaterial_Pass8 == null)
		{
			Debug.LogWarning("nextEdgeMaterial_Pass8 is null. Skipped _InitializenextEdgeMesh().");
			return;
		}
		bool flag = (_nextEdgeVisibleCached = nextEdgeSize > 0f);
		if (_meshRenderers != null)
		{
			_nextEdgeMeshRenderers = new MeshRenderer[_meshRenderers.Length];
			for (int i = 0; i < _meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = _meshRenderers[i];
				if (!(meshRenderer != null))
				{
					continue;
				}
				Material[] array = meshRenderer.sharedMaterials;
				if (array == null || array.Length == 0)
				{
					array = meshRenderer.materials;
				}
				if (array != null)
				{
					array = _CloneNEXTEdgeMaterials(array);
				}
				if (array != null)
				{
					GameObject gameObject = _CreateNEXTEdgeGameObject(meshRenderer.gameObject);
					MeshRenderer meshRenderer2 = gameObject.AddComponent<MeshRenderer>();
					meshRenderer2.enabled = flag;
					meshRenderer2.shadowCastingMode = ShadowCastingMode.Off;
					meshRenderer2.receiveShadows = false;
					meshRenderer2.materials = array;
					MeshFilter component = _meshRenderers[i].gameObject.GetComponent<MeshFilter>();
					if (component != null)
					{
						gameObject.AddComponent<MeshFilter>().sharedMesh = component.sharedMesh;
					}
					_nextEdgeMeshRenderers[i] = meshRenderer2;
				}
			}
		}
		if (_skinnedMeshRenderers != null)
		{
			_nextEdgeSkinnedMeshRenderers = new SkinnedMeshRenderer[_skinnedMeshRenderers.Length];
			for (int j = 0; j < _skinnedMeshRenderers.Length; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[j];
				if (skinnedMeshRenderer != null)
				{
					Material[] array2 = skinnedMeshRenderer.sharedMaterials;
					if (array2 == null || array2.Length == 0)
					{
						array2 = skinnedMeshRenderer.materials;
					}
					if (array2 != null)
					{
						array2 = _CloneNEXTEdgeMaterials(array2);
					}
					if (array2 != null)
					{
						SkinnedMeshRenderer skinnedMeshRenderer2 = _CreateNEXTEdgeGameObject(skinnedMeshRenderer.gameObject).AddComponent<SkinnedMeshRenderer>();
						skinnedMeshRenderer2.sharedMesh = skinnedMeshRenderer.sharedMesh;
						skinnedMeshRenderer2.bones = skinnedMeshRenderer.bones;
						skinnedMeshRenderer2.rootBone = skinnedMeshRenderer.rootBone;
						skinnedMeshRenderer2.shadowCastingMode = ShadowCastingMode.Off;
						skinnedMeshRenderer2.receiveShadows = false;
						skinnedMeshRenderer2.materials = array2;
						skinnedMeshRenderer2.enabled = flag;
						_nextEdgeSkinnedMeshRenderers[j] = skinnedMeshRenderer2;
					}
				}
			}
		}
		_UpdatedNEXTEdge();
	}

	private static GameObject _CreateNEXTEdgeGameObject(GameObject parentGameObject)
	{
		if (parentGameObject != null)
		{
			GameObject obj = new GameObject(parentGameObject.name + "(NEXTEdge)");
			obj.transform.parent = parentGameObject.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			return obj;
		}
		return null;
	}

	private Material[] _CloneNEXTEdgeMaterials(Material[] materials)
	{
		if (materials != null)
		{
			Material[] array = new Material[materials.Length];
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null && materials[i].shader != null)
				{
					if (nextEdgePass == NEXTEdgePass.Pass4)
					{
						array[i] = nextEdgeMaterial_Pass4;
					}
					else
					{
						array[i] = nextEdgeMaterial_Pass8;
					}
				}
			}
			return array;
		}
		return null;
	}

	private void _UpdatedNEXTEdge()
	{
		if (!supportNEXTEdge)
		{
			return;
		}
		bool flag = nextEdgeSize > 0f;
		if (_nextEdgeVisibleCached != flag)
		{
			_nextEdgeVisibleCached = flag;
			if (_nextEdgeMeshRenderers != null)
			{
				for (int i = 0; i != _nextEdgeMeshRenderers.Length; i++)
				{
					_nextEdgeMeshRenderers[i].enabled = flag;
				}
			}
			if (_nextEdgeSkinnedMeshRenderers != null)
			{
				for (int j = 0; j != _nextEdgeSkinnedMeshRenderers.Length; j++)
				{
					_nextEdgeSkinnedMeshRenderers[j].enabled = flag;
				}
			}
		}
		if (_nextEdgeSizeCached != nextEdgeSize || _nextEdgeColorCached != nextEdgeColor)
		{
			_nextEdgeSizeCached = nextEdgeSize;
			_nextEdgeColorCached = nextEdgeColor;
			if (nextEdgeMaterial_Pass4 != null)
			{
				nextEdgeMaterial_Pass4.SetFloat("_EdgeSize", _nextEdgeSizeCached * 0.05f);
				nextEdgeMaterial_Pass4.SetColor("_EdgeColor", _nextEdgeColorCached);
			}
			if (nextEdgeMaterial_Pass8 != null)
			{
				nextEdgeMaterial_Pass8.SetFloat("_EdgeSize", _nextEdgeSizeCached * 0.05f);
				nextEdgeMaterial_Pass8.SetColor("_EdgeColor", _nextEdgeColorCached);
			}
		}
	}

	private void _InitializeRigidBody()
	{
		if (_modelData == null)
		{
			rigidBodyList = null;
			return;
		}
		if (_modelData.rigidBodyDataList == null)
		{
			rigidBodyList = null;
			return;
		}
		if (rigidBodyList != null)
		{
			for (int i = 0; i != rigidBodyList.Length; i++)
			{
				if (rigidBodyList[i] == null)
				{
					rigidBodyList = null;
					break;
				}
			}
		}
		if (rigidBodyList == null || rigidBodyList.Length != _modelData.rigidBodyDataList.Length)
		{
			rigidBodyList = new RigidBody[_modelData.rigidBodyDataList.Length];
			for (int j = 0; j != rigidBodyList.Length; j++)
			{
				rigidBodyList[j] = new RigidBody();
				rigidBodyList[j].rigidBodyData = _modelData.rigidBodyDataList[j];
				rigidBodyList[j].freezed = rigidBodyList[j].rigidBodyData.isFreezed;
			}
		}
		else
		{
			for (int k = 0; k != rigidBodyList.Length; k++)
			{
				rigidBodyList[k].rigidBodyData = _modelData.rigidBodyDataList[k];
			}
		}
	}

	private void _InitializePhysicsEngine()
	{
		if (modelFile == null)
		{
			Debug.LogWarning(base.gameObject.name + ":modelFile is nothing.");
		}
		else if (physicsEngine == PhysicsEngine.None || physicsEngine == PhysicsEngine.BulletPhysics)
		{
			MMD4MecanimBulletPhysics instance = MMD4MecanimBulletPhysics.instance;
			if (instance != null)
			{
				_bulletPhysicsMMDModel = instance.CreateMMDModel(this);
			}
		}
	}

	public void SetGravity(float gravityScale, float gravityNoise, Vector3 gravityDirection)
	{
		if (bulletPhysics != null && bulletPhysics.worldProperty != null)
		{
			bulletPhysics.worldProperty.gravityScale = gravityScale;
			bulletPhysics.worldProperty.gravityNoise = gravityNoise;
			bulletPhysics.worldProperty.gravityDirection = gravityDirection;
		}
	}

	public MMD4MecanimBone GetRootBone()
	{
		return _rootBone;
	}

	public MMD4MecanimBone GetBone(int boneID)
	{
		if (boneList != null && boneID >= 0 && boneID < boneList.Length)
		{
			return boneList[boneID];
		}
		return null;
	}

	private void Awake()
	{
		if (initializeOnAwake)
		{
			Initialize();
		}
	}

	private void Start()
	{
		Initialize();
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			_Salvage();
			if (onUpdating != null)
			{
				onUpdating();
			}
			_UpdateAnim();
			_UpdateMorph();
			_UpdateBone();
			_UpdateIK();
			if (onUpdated != null)
			{
				onUpdated();
			}
		}
	}

	private void _Salvage()
	{
		if (_meshRenderers == null || _skinnedMeshRenderers == null)
		{
			_InitializeMesh();
		}
		if (!(modelFile != null))
		{
			return;
		}
		if (_modelData == null)
		{
			_InitializeModelData();
		}
		if (_modelData != null)
		{
			int num = ((boneList != null) ? boneList.Length : 0);
			int num2 = ((_modelData.boneDataList != null) ? _modelData.boneDataList.Length : 0);
			if (num != num2)
			{
				_InitializeModel();
			}
			int num3 = ((rigidBodyList != null) ? rigidBodyList.Length : 0);
			int num4 = ((_modelData.rigidBodyDataList != null) ? _modelData.rigidBodyDataList.Length : 0);
			if (num3 != num4)
			{
				_InitializeRigidBody();
			}
			if (_morphBlendShapes == null && _modelData.morphDataList != null && _modelData.morphDataList.Length != 0 && blendShapesEnabled && _IsBlendShapesAnything())
			{
				_blendShapesEnabledCache = blendShapesEnabled;
				_PrepareBlendShapes();
				_InitializeBlendShapes();
			}
			if (_animator == null || _animMorphCategoryWeights == null)
			{
				_InitializeAnimatoion();
			}
			if (_animator != null && _animator.avatar != null && _animator.avatar.isValid && _animator.avatar.isHuman && (_leftArmBone == null || _rightArmBone == null))
			{
				_InitializePPHBones();
			}
			if (nextEdgeMaterial_Pass4 == null || nextEdgeMaterial_Pass4.shader == null || nextEdgeMaterial_Pass8 == null || nextEdgeMaterial_Pass8.shader == null)
			{
				_InitializeNEXTMaterial();
			}
		}
	}

	private void LateUpdate()
	{
		_UpdatedDeffered();
		_UpdatedNEXTEdge();
		if (!Application.isPlaying)
		{
			_UpdateAmbientPreview();
		}
		if (Application.isPlaying)
		{
			if (onLateUpdating != null)
			{
				onLateUpdating();
			}
			_LateUpdateBone();
			_LateUpdateIK();
			_UploadMeshMaterial();
			if (onLateUpdated != null)
			{
				onLateUpdated();
			}
		}
	}

	private void OnRenderObject()
	{
	}

	private void OnDestroy()
	{
		_DestroyDeferredMaterial();
		_DestroyNEXTMaterial();
		if (ikList != null)
		{
			for (int i = 0; i < ikList.Length; i++)
			{
				if (ikList[i] != null)
				{
					ikList[i].Destroy();
				}
			}
			ikList = null;
		}
		_sortedBoneList = null;
		if (boneList != null)
		{
			for (int j = 0; j < boneList.Length; j++)
			{
				if (boneList[j] != null)
				{
					boneList[j].Destroy();
				}
			}
			boneList = null;
		}
		if (_bulletPhysicsMMDModel != null && !_bulletPhysicsMMDModel.isExpired)
		{
			MMD4MecanimBulletPhysics instance = MMD4MecanimBulletPhysics.instance;
			if (instance != null)
			{
				instance.DestroyMMDModel(_bulletPhysicsMMDModel);
			}
		}
		_bulletPhysicsMMDModel = null;
	}

	public void Initialize()
	{
		if (!Application.isPlaying)
		{
			InitializeOnEditor();
		}
		else if (!(this == null) && !_initialized)
		{
			_initialized = true;
			_blendShapesEnabledCache = blendShapesEnabled;
			_InitializeMesh();
			_InitializeModel();
			_InitializeRigidBody();
			_PrepareBlendShapes();
			_InitializeBlendShapes();
			_InitializeAnimatoion();
			_InitializePhysicsEngine();
			_InitializePPHBones();
			_InitializeDeferredMesh();
			_InitializeNEXTEdgeMesh();
			_InitializeGlobalAmbient();
			_InitializeIK();
		}
	}

	public AudioSource GetAudioSource()
	{
		if (audioSource == null)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
		}
		return audioSource;
	}

	public void InitializeOnEditor()
	{
		if (this == null)
		{
			return;
		}
		if (_modelData == null)
		{
			_initialized = false;
		}
		if ((_modelData != null || !(modelFile == null)) && (_modelData != null || _InitializeModelData()))
		{
			if (_modelData != null && _modelData.boneDataList != null && (boneList == null || boneList.Length != _modelData.boneDataList.Length))
			{
				_initialized = false;
			}
			if (!_initialized)
			{
				_initialized = true;
				_blendShapesEnabledCache = blendShapesEnabled;
				_InitializeMesh();
				_InitializeModel();
				_InitializeRigidBody();
				_PrepareBlendShapes();
				_InitializeBlendShapes();
				_InitializeAnimatoion();
				_InitializeDeferredMaterial();
				_InitializeNEXTMaterial();
				_InitializeIK();
			}
		}
	}

	private void _InitializeMesh()
	{
		if (_meshRenderers != null)
		{
			for (int i = 0; i != _meshRenderers.Length; i++)
			{
				if (_meshRenderers[i] == null)
				{
					_meshRenderers = null;
					break;
				}
			}
		}
		if (_skinnedMeshRenderers != null)
		{
			for (int j = 0; j != _skinnedMeshRenderers.Length; j++)
			{
				if (_skinnedMeshRenderers[j] == null)
				{
					_skinnedMeshRenderers = null;
					break;
				}
			}
		}
		if (_meshRenderers == null || _meshRenderers.Length == 0)
		{
			_meshRenderers = MMD4MecanimCommon.GetMeshRenderers(base.gameObject);
			if (_meshRenderers == null)
			{
				_meshRenderers = new MeshRenderer[0];
			}
		}
		if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Length == 0)
		{
			_skinnedMeshRenderers = MMD4MecanimCommon.GetSkinnedMeshRenderers(base.gameObject);
			if (_skinnedMeshRenderers == null)
			{
				_skinnedMeshRenderers = new SkinnedMeshRenderer[0];
			}
			if (_skinnedMeshRenderers != null)
			{
				for (int k = 0; k != _skinnedMeshRenderers.Length; k++)
				{
					if (_skinnedMeshRenderers[k].updateWhenOffscreen != updateWhenOffscreen)
					{
						_skinnedMeshRenderers[k].updateWhenOffscreen = updateWhenOffscreen;
					}
				}
			}
		}
		if (defaultMesh == null && _skinnedMeshRenderers != null && _skinnedMeshRenderers.Length != 0)
		{
			defaultMesh = _skinnedMeshRenderers[0].sharedMesh;
		}
		if (defaultMesh == null && _meshRenderers != null && _meshRenderers.Length != 0)
		{
			MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
			if (component != null)
			{
				defaultMesh = component.sharedMesh;
			}
		}
	}

	private void _PrepareBlendShapes()
	{
		if (_skinnedMeshRenderers == null)
		{
			return;
		}
		for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[i];
			if (!(skinnedMeshRenderer != null))
			{
				continue;
			}
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			if (!(sharedMesh != null))
			{
				continue;
			}
			for (int j = 0; j < sharedMesh.blendShapeCount; j++)
			{
				if (Application.isPlaying)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(j, 0f);
				}
				else if (skinnedMeshRenderer.GetBlendShapeWeight(j) != 0f)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(j, 0f);
				}
			}
		}
	}

	private bool _IsBlendShapesAnything()
	{
		if (_skinnedMeshRenderers != null)
		{
			for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[i];
				if (skinnedMeshRenderer != null)
				{
					Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
					if (sharedMesh != null && sharedMesh.blendShapeCount > 0)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void _InitializeBlendShapes()
	{
		if (!_blendShapesEnabledCache || _skinnedMeshRenderers == null || _modelData == null || _modelData.morphDataList == null || _modelData.morphDataList.Length == 0 || (_morphBlendShapes != null && _morphBlendShapes.Length == _skinnedMeshRenderers.Length))
		{
			return;
		}
		_morphBlendShapes = null;
		if (!_IsBlendShapesAnything())
		{
			return;
		}
		_morphBlendShapes = new MorphBlendShape[_skinnedMeshRenderers.Length];
		for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
		{
			_morphBlendShapes[i] = default(MorphBlendShape);
			_morphBlendShapes[i].blendShapeIndices = new int[_modelData.morphDataList.Length];
			for (int j = 0; j < _modelData.morphDataList.Length; j++)
			{
				_morphBlendShapes[i].blendShapeIndices[j] = -1;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = _skinnedMeshRenderers[i];
			if (!(skinnedMeshRenderer.sharedMesh != null) || skinnedMeshRenderer.sharedMesh.blendShapeCount <= 0)
			{
				continue;
			}
			for (int k = 0; k < skinnedMeshRenderer.sharedMesh.blendShapeCount; k++)
			{
				string blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(k);
				int num = -1;
				num = ((!MMD4MecanimCommon.IsID(blendShapeName)) ? _modelData.GetMorphDataIndex(blendShapeName, false) : MMD4MecanimCommon.ToInt(blendShapeName));
				if ((uint)num < (uint)_modelData.morphDataList.Length)
				{
					_morphBlendShapes[i].blendShapeIndices[num] = k;
				}
			}
		}
	}

	private static void _PostfixRenderQueue(Material[] materials, bool postfixRenderQueue, bool renderQueueAfterSkybox)
	{
		if (!Application.isPlaying || materials == null)
		{
			return;
		}
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i].shader != null && materials[i].shader.name != null && materials[i].shader.name.StartsWith("MMD4Mecanim") && postfixRenderQueue)
			{
				int num = (renderQueueAfterSkybox ? 2502 : 2002);
				materials[i].renderQueue = num + MMD4MecanimCommon.ToInt(materials[i].name);
			}
		}
	}

	private static void _SetupCloneMaterial(CloneMaterial cloneMaterial, Material[] materials)
	{
		cloneMaterial.materials = materials;
		if (materials == null)
		{
			return;
		}
		int num = materials.Length;
		cloneMaterial.materialData = new MMD4MecanimData.MorphMaterialData[num];
		cloneMaterial.backupMaterialData = new MMD4MecanimData.MorphMaterialData[num];
		cloneMaterial.updateMaterialData = new bool[num];
		for (int i = 0; i < num; i++)
		{
			if (materials[i] != null)
			{
				MMD4MecanimCommon.BackupMaterial(ref cloneMaterial.backupMaterialData[i], materials[i]);
				cloneMaterial.materialData[i] = cloneMaterial.backupMaterialData[i];
			}
		}
	}

	public bool IsEnableMorphBlendShapes(int meshIndex)
	{
		if (_morphBlendShapes == null)
		{
			return false;
		}
		if (meshIndex < 0 || meshIndex >= _morphBlendShapes.Length)
		{
			return false;
		}
		if (_morphBlendShapes[meshIndex].blendShapeIndices == null)
		{
			return false;
		}
		for (int i = 0; i != _morphBlendShapes[meshIndex].blendShapeIndices.Length; i++)
		{
			if (_morphBlendShapes[meshIndex].blendShapeIndices[i] != -1)
			{
				return true;
			}
		}
		return false;
	}

	private bool _InitializeModelData()
	{
		if (modelFile == null)
		{
			Debug.LogWarning(base.gameObject.name + ":modelFile is nothing.");
			return false;
		}
		_modelData = MMD4MecanimData.BuildModelData(modelFile);
		if (_modelData == null)
		{
			Debug.LogError(base.gameObject.name + ":modelFile is unsupported format.");
			return false;
		}
		return true;
	}

	private void _InitializeModel()
	{
		if (_modelData == null && !_InitializeModelData())
		{
			return;
		}
		if (_modelData.boneDataList != null && _modelData.boneDataDictionary != null && (boneList == null || boneList.Length != _modelData.boneDataList.Length))
		{
			boneList = new MMD4MecanimBone[_modelData.boneDataList.Length];
			_BindBone();
			for (int i = 0; i < boneList.Length; i++)
			{
				if (boneList[i] != null)
				{
					boneList[i].Bind();
				}
			}
			_sortedBoneList = new MMD4MecanimBone[boneList.Length];
			for (int j = 0; j < boneList.Length; j++)
			{
				if (!(boneList[j] != null))
				{
					continue;
				}
				MMD4MecanimData.BoneData boneData = boneList[j].boneData;
				if (boneData != null)
				{
					int sortedBoneID = boneData.sortedBoneID;
					if (sortedBoneID >= 0 && sortedBoneID < boneList.Length)
					{
						_sortedBoneList[sortedBoneID] = boneList[j];
					}
				}
			}
		}
		if (_modelData.ikDataList != null)
		{
			int num = _modelData.ikDataList.Length;
			ikList = new IK[num];
			for (int k = 0; k < num; k++)
			{
				ikList[k] = new IK(this, k);
			}
		}
		if (_modelData.morphDataList != null)
		{
			morphList = new Morph[_modelData.morphDataList.Length];
			for (int l = 0; l < _modelData.morphDataList.Length; l++)
			{
				morphList[l] = new Morph();
				morphList[l].morphData = _modelData.morphDataList[l];
				string text = morphList[l].name;
				if (!string.IsNullOrEmpty(text))
				{
					switch (text)
					{
					case "LightUp":
						morphList[l].morphAutoLuminousType = MMD4MecanimData.MorphAutoLumninousType.LightUp;
						break;
					case "LightOff":
						morphList[l].morphAutoLuminousType = MMD4MecanimData.MorphAutoLumninousType.LightOff;
						break;
					case "LightBlink":
						morphList[l].morphAutoLuminousType = MMD4MecanimData.MorphAutoLumninousType.LightBlink;
						break;
					case "LightBS":
						morphList[l].morphAutoLuminousType = MMD4MecanimData.MorphAutoLumninousType.LightBS;
						break;
					}
				}
			}
		}
		morphAutoLuminous = new MorphAutoLuminous();
	}
}
