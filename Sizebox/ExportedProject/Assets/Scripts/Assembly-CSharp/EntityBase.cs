using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lua;
using SaveDataStructures;
using Sizebox.CharacterEditor;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class EntityBase : MonoBehaviour, IEntity, IGameObject, ISavable, IMorphable
{
	public static UnityAction<EntityBase> EntityDeleted;

	private static int _entityIndex = 1;

	private GameObject _model;

	[Header("Entity - Required References")]
	[SerializeField]
	private UnityEngine.Rigidbody rBody;

	[SerializeField]
	protected MovementCharacter movement;

	[SerializeField]
	protected GameObject defaultModel;

	[Header("Entity - Data")]
	public float offset;

	public float baseHeight = 1f;

	public float meshHeight;

	public bool isPositioned = true;

	public bool isGiantess;

	public bool isHumanoid;

	public bool isMicro;

	public bool isPlayer;

	public float maxSize = 1000f;

	public float minSize = 0.001f;

	public bool rotationEnabled = true;

	public UnityAction<float, float> SizeChanging;

	private bool _visible = true;

	public bool locked;

	private readonly List<EntityMorphData> _morphs = new List<EntityMorphData>();

	protected Renderer[] MeshRenderers;

	private Entity _luaEntity;

	private readonly UnityEngine.Vector3 _normal = UnityEngine.Vector3.up;

	protected Collider[] ColliderList;

	private bool _isColliderActive = true;

	public virtual EntityType EntityType
	{
		get
		{
			return EntityType.OBJECT;
		}
	}

	public GameObject model
	{
		get
		{
			return _model;
		}
		protected set
		{
			_model = value;
			MeshRenderers = GetRenderers();
		}
	}

	public bool Gravity
	{
		get
		{
			Gravity component = GetComponent<Gravity>();
			if (!component)
			{
				return false;
			}
			return component.enabled;
		}
		set
		{
			Gravity component = GetComponent<Gravity>();
			if ((bool)component)
			{
				component.enabled = value;
			}
		}
	}

	public bool updateWhenOffScreen
	{
		get
		{
			SkinnedMeshRenderer componentInChildren = GetComponentInChildren<SkinnedMeshRenderer>();
			if ((bool)componentInChildren)
			{
				return componentInChildren.updateWhenOffscreen;
			}
			return false;
		}
		set
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].updateWhenOffscreen = value;
			}
		}
	}

	public AssetDescription asset { get; private set; }

	public UnityEngine.Rigidbody Rigidbody
	{
		get
		{
			return rBody;
		}
	}

	public EntityBase Entity
	{
		get
		{
			return this;
		}
	}

	public MovementCharacter Movement
	{
		get
		{
			return movement;
		}
	}

	public bool Initialized { get; private set; }

	public int id { get; set; }

	public bool MorphsInitialized { get; private set; }

	public List<EntityMorphData> Morphs
	{
		get
		{
			return _morphs;
		}
	}

	public float Height
	{
		get
		{
			return base.transform.lossyScale.y * baseHeight;
		}
	}

	public float MeshHeight
	{
		get
		{
			UnityEngine.Transform transform = base.transform;
			if (!transform)
			{
				return 0f;
			}
			return transform.lossyScale.y * meshHeight;
		}
		set
		{
			ChangeScale(value / meshHeight);
		}
	}

	public float AccurateScale
	{
		get
		{
			float num = base.transform.lossyScale.y;
			if (isGiantess)
			{
				num *= 1000f;
			}
			return num;
		}
		set
		{
			if (isGiantess)
			{
				value /= 1000f;
			}
			ChangeScale(value);
		}
	}

	public virtual float Scale
	{
		get
		{
			return base.transform.lossyScale.y;
		}
	}

	public float ModelScale
	{
		get
		{
			if (!model)
			{
				return 0f;
			}
			return model.transform.localScale.y;
		}
	}

	public event Action<EntityBase> OnInitialized;

	public static void ForceNextId(int id)
	{
		_entityIndex = id;
	}

	public bool CanSupportChild(EntityBase child)
	{
		float num = child.MeshHeight;
		float num2 = MeshHeight;
		if (num > num2)
		{
			return false;
		}
		return num2 / num > 15f;
	}

	public EntityBase()
	{
		id = _entityIndex++;
	}

	public void RegisterModelAsset(AssetDescription assetDescription)
	{
		asset = assetDescription;
		base.name = assetDescription.AssetFullName;
		LoadModelAssetAsync();
	}

	private void LoadModelAssetAsync()
	{
		if (asset != null)
		{
			model = defaultModel;
			if ((bool)defaultModel)
			{
				defaultModel.SetActive(true);
			}
			if (asset.AssetType == AssetType.InternalPremade)
			{
				Initialize(model);
			}
			else
			{
				LoadModelAsync();
			}
		}
	}

	private void LoadModelAsync()
	{
		AssetLoader.LoadModelAssetAsync(asset, CompleteAsyncLoad);
	}

	private void CompleteAsyncLoad()
	{
		if ((bool)this && asset.IsLoaded)
		{
			if ((bool)defaultModel)
			{
				defaultModel.SetActive(false);
			}
			Initialize(asset.Asset);
		}
	}

	private void Initialize(GameObject modelPrefab)
	{
		if (asset.AssetType != AssetType.InternalPremade)
		{
			InitializeModel(modelPrefab);
		}
		InitializeComponents();
		Initialized = true;
		FinishInitialization();
	}

	protected virtual void InitializeModel(GameObject modelPrefab)
	{
		model = ((modelPrefab != defaultModel) ? UnityEngine.Object.Instantiate(modelPrefab, base.transform) : modelPrefab);
		int num = Layers.objectLayer;
		CustomDestructible component = model.GetComponent<CustomDestructible>();
		if ((bool)component)
		{
			offset = model.transform.localPosition.y;
			model.transform.localPosition = UnityEngine.Vector3.zero;
			if (component.destructibleType == CustomDestructible.DestructibleType.Stationary)
			{
				num = Layers.buildingLayer;
			}
			else
			{
				num = Layers.vehicelsLayer;
				if ((bool)component.myRigidbody)
				{
					rBody = base.gameObject.AddComponent<UnityEngine.Rigidbody>();
					rBody.mass = component.myRigidbody.mass;
					rBody.drag = component.myRigidbody.drag;
					rBody.angularDrag = component.myRigidbody.angularDrag;
					rBody.collisionDetectionMode = component.myRigidbody.collisionDetectionMode;
					UnityEngine.Object.Destroy(component.myRigidbody);
					component.myRigidbody = null;
				}
			}
			base.gameObject.AddComponent<AssetDestructible>();
		}
		else
		{
			MeshFilter[] componentsInChildren = model.GetComponentsInChildren<MeshFilter>();
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				meshFilter.gameObject.layer = num;
				if (meshFilter.gameObject.GetComponent<MeshCollider>() == null)
				{
					meshFilter.gameObject.AddComponent<MeshCollider>();
				}
			}
		}
		base.gameObject.layer = num;
		base.gameObject.SetRenderingLayer(num);
		MeshRenderers = GetRenderers();
		meshHeight = GetCombinedRendererHeight(MeshRenderers);
		ResetColliderActive();
	}

	private void InitializeComponents()
	{
		EntityComponent[] componentsInChildren = GetComponentsInChildren<EntityComponent>();
		foreach (EntityComponent entityComponent in componentsInChildren)
		{
			try
			{
				entityComponent.Initialize(this);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	protected virtual void FinishInitialization()
	{
		InitializeMorphs();
		if (InterfaceControl.instance.selectedEntity == this && EditPlacement.Instance.IsMoving())
		{
			SetColliderActive(false);
		}
		ObjectManager.Instance.RegisterEntity(this);
		Initialized = true;
		if (this.OnInitialized != null)
		{
			this.OnInitialized(this);
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnEnable()
	{
		if (Initialized)
		{
			ObjectManager.Instance.RegisterEntity(this);
		}
	}

	protected virtual void OnDisable()
	{
		ObjectManager.Instance.UnregisterEntity(this);
	}

	protected virtual void SizeChanged(float oldSize, float newSize)
	{
	}

	public Entity GetLuaEntity()
	{
		return _luaEntity ?? (_luaEntity = new Entity(this));
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	public virtual void Lock()
	{
		if (!locked)
		{
			locked = true;
			Rigidbody.isKinematic = true;
			Gravity component = GetComponent<Gravity>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}
	}

	public virtual void Unlock()
	{
		if (!locked)
		{
			return;
		}
		locked = false;
		UnityEngine.Rigidbody component = GetComponent<UnityEngine.Rigidbody>();
		if (!(component == null))
		{
			component.isKinematic = false;
			Gravity component2 = GetComponent<Gravity>();
			if ((bool)component2)
			{
				component2.enabled = true;
			}
		}
	}

	public virtual void Move(UnityEngine.Vector3 worldPos)
	{
		base.transform.position = worldPos;
		base.transform.Translate(UnityEngine.Vector3.up * (offset * Height));
	}

	protected virtual float ClampScale(float scale)
	{
		return scale;
	}

	public virtual void ChangeScale(float newScale)
	{
		newScale = ClampScale(newScale);
		UnityEngine.Transform obj = base.transform;
		UnityEngine.Transform parent = obj.parent;
		float y = obj.lossyScale.y;
		obj.SetParent(null);
		obj.localScale = UnityEngine.Vector3.one * newScale;
		obj.SetParent(parent, true);
		SizeChanged(y, newScale);
		UnityAction<float, float> sizeChanging = SizeChanging;
		if (sizeChanging != null)
		{
			sizeChanging(y, newScale);
		}
	}

	public virtual void ChangeRotation(UnityEngine.Vector3 newRotation)
	{
		if (rotationEnabled)
		{
			base.transform.Rotate(newRotation);
		}
	}

	public virtual float GetGrounderWeight()
	{
		return 0f;
	}

	public virtual void ChangeGrounderWeight(float newWeight)
	{
	}

	public virtual void ChangeVerticalOffset(float newOffset)
	{
		float num = newOffset - offset;
		base.transform.position += _normal * (num * Height);
		offset = newOffset;
	}

	protected void ResetColliderActive()
	{
		FindColliders();
		SetColliderActive(_isColliderActive);
	}

	public virtual void SetColliderActive(bool value)
	{
		_isColliderActive = value;
		if (ColliderList == null)
		{
			FindColliders();
			if (ColliderList == null || ColliderList.Length == 0)
			{
				if ((bool)Rigidbody)
				{
					Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
					Rigidbody.isKinematic = !value;
				}
				return;
			}
		}
		Collider[] colliderList = ColliderList;
		for (int i = 0; i < colliderList.Length; i++)
		{
			colliderList[i].enabled = value;
		}
	}

	protected virtual void FindColliders()
	{
		ColliderList = GetComponentsInChildren<Collider>();
		Collider[] colliderList = ColliderList;
		for (int i = 0; i < colliderList.Length; i++)
		{
			float y = colliderList[i].bounds.size.y;
			if (y > baseHeight)
			{
				baseHeight = y;
			}
		}
	}

	public virtual void DestroyObject(bool recursive = true)
	{
		EntityBase[] componentsInChildren = GetComponentsInChildren<EntityBase>(true);
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].transform.SetParent(null);
		}
		if (recursive)
		{
			for (int j = 1; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].DestroyObject(false);
			}
		}
		ObjectManager.Instance.UnregisterEntity(this);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected float GetCombinedRendererHeight(Renderer[] a)
	{
		if (a.Length == 0)
		{
			return 0f;
		}
		UnityEngine.Transform transform = base.transform;
		UnityEngine.Transform parent = transform.parent;
		UnityEngine.Vector3 localScale = transform.localScale;
		UnityEngine.Vector3 position = transform.position;
		UnityEngine.Quaternion rotation = transform.rotation;
		transform.SetParent(null);
		transform.localScale = UnityEngine.Vector3.one;
		transform.rotation = UnityEngine.Quaternion.identity;
		transform.position = UnityEngine.Vector3.zero;
		Bounds bounds = a[0].bounds;
		if (a.Length > 1)
		{
			for (int i = 1; i < a.Length; i++)
			{
				bounds.Encapsulate(a[i].bounds);
			}
		}
		transform.parent = parent;
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = localScale;
		return bounds.max.y;
	}

	public void Render(bool doEnabled)
	{
		if (_visible != doEnabled)
		{
			_visible = doEnabled;
			Renderer[] meshRenderers = MeshRenderers;
			for (int i = 0; i < meshRenderers.Length; i++)
			{
				meshRenderers[i].enabled = doEnabled;
			}
		}
	}

	protected Renderer[] GetRenderers()
	{
		return model.GetComponentsInChildren<Renderer>();
	}

	public bool IsVisible()
	{
		return _visible;
	}

	public virtual bool IsTargetAble()
	{
		return true;
	}

	public virtual void Place()
	{
	}

	public virtual UnityEngine.Vector3 GetEyesPosition()
	{
		return UnityEngine.Vector3.zero;
	}

	public float CheckForOverHeadObjects(float gtsHeight)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position + new UnityEngine.Vector3(0f, 1f, 0f) * base.transform.localScale.y, UnityEngine.Vector3.up, out hitInfo, gtsHeight * 2f, Layers.gtsWalkableMask))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	public void SetMorphValue(string morphName, float weight)
	{
		foreach (EntityMorphData morph in _morphs)
		{
			if (morph.Name == morphName)
			{
				SetMorphValue(morph, weight);
				break;
			}
		}
	}

	public void SetMorphValue(int i, float weight)
	{
		EntityMorphData entityMorphData = _morphs[i];
		SetMorphValue(entityMorphData, weight);
	}

	protected void SetMorphValue(EntityMorphData entityMorphData, float weight)
	{
		entityMorphData.Weight = weight;
		foreach (MeshMorphData item in entityMorphData.mesh)
		{
			item.mesh.SetBlendShapeWeight(item.id, weight * 100f);
		}
	}

	public float GetMorphValue(string morphName)
	{
		foreach (EntityMorphData morph in _morphs)
		{
			if (morph.Name == morphName)
			{
				return GetMorphValue(morph);
			}
		}
		return 0f;
	}

	protected float GetMorphValue(EntityMorphData entityMorphData)
	{
		return entityMorphData.Weight;
	}

	protected EntityMorphData FindMorphByNameContains(string[] morphNames)
	{
		foreach (EntityMorphData morph in Morphs)
		{
			foreach (string value in morphNames)
			{
				if (morph.Name.Contains(value))
				{
					return morph;
				}
			}
		}
		return null;
	}

	protected EntityMorphData FindMorphByNameContains(string morphName)
	{
		foreach (EntityMorphData morph in Morphs)
		{
			if (morph.Name.Contains(morphName))
			{
				return morph;
			}
		}
		return null;
	}

	private EntityMorphData FindMorphByName(string morphName)
	{
		foreach (EntityMorphData morph in Morphs)
		{
			if (morph.Name == morphName)
			{
				return morph;
			}
		}
		return null;
	}

	public virtual void InitializeMorphs()
	{
		if (MorphsInitialized)
		{
			return;
		}
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			if (sharedMesh.blendShapeCount == 0)
			{
				continue;
			}
			for (int j = 0; j < sharedMesh.blendShapeCount; j++)
			{
				string blendShapeName = sharedMesh.GetBlendShapeName(j);
				MeshMorphData meshMorphData = default(MeshMorphData);
				meshMorphData.id = j;
				meshMorphData.mesh = skinnedMeshRenderer;
				MeshMorphData item = meshMorphData;
				EntityMorphData entityMorphData = FindMorphByName(blendShapeName);
				if (entityMorphData != null)
				{
					entityMorphData.mesh.Add(item);
					continue;
				}
				entityMorphData = new EntityMorphData
				{
					Name = sharedMesh.GetBlendShapeName(j),
					Weight = 0f
				};
				entityMorphData.mesh.Add(item);
				_morphs.Add(entityMorphData);
			}
		}
		MorphsInitialized = true;
	}

	public virtual SavableData Save()
	{
		return new EntitySaveData(this);
	}

	public virtual void Load(SavableData inData, bool loadPosition = true)
	{
		EntitySaveData entitySaveData = (EntitySaveData)inData;
		base.name = entitySaveData.name;
		offset = entitySaveData.floorOffset;
		if (entitySaveData.morphs != null && entitySaveData.morphs.Length != 0)
		{
			InitializeMorphs();
			MorphSaveData[] morphs = entitySaveData.morphs;
			foreach (MorphSaveData morphSaveData in morphs)
			{
				SetMorphValue(morphSaveData.index, morphSaveData.value);
			}
		}
		ParentData parentingData = entitySaveData.parentingData;
		int parentEntityId = parentingData.parentEntityId;
		if (loadPosition)
		{
			if (parentEntityId != -1)
			{
				UnityEngine.Transform transform = ObjectManager.Instance.GetById(parentEntityId).transform.FindRecursive(entitySaveData.parentingData.transformName);
				if ((bool)transform)
				{
					UnityEngine.Transform obj = base.transform;
					obj.parent = transform;
					obj.localPosition = parentingData.localPosition;
					obj.localRotation = parentingData.localRotation;
				}
			}
			else
			{
				UnityEngine.Transform obj2 = base.transform;
				obj2.position = entitySaveData.virtualPosition.ToWorld();
				obj2.rotation = entitySaveData.rotation;
			}
			Gravity component = GetComponent<Gravity>();
			if ((bool)component)
			{
				component.PauseForSeconds(3f);
			}
		}
		else
		{
			base.transform.rotation = ((parentEntityId == -1) ? entitySaveData.rotation : parentingData.localRotation);
		}
		if (entitySaveData.characterEditorSaveData != null)
		{
			base.gameObject.AddComponent<CharacterEditor>().Load(entitySaveData.characterEditorSaveData);
		}
	}

	protected virtual void OnDestroy()
	{
		UnityAction<EntityBase> entityDeleted = EntityDeleted;
		if (entityDeleted != null)
		{
			entityDeleted(this);
		}
	}

	[SpecialName]
	GameObject IGameObject.get_gameObject()
	{
		return base.gameObject;
	}

	[SpecialName]
	UnityEngine.Transform IGameObject.get_transform()
	{
		return base.transform;
	}
}
