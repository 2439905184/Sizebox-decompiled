using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public static ObjectManager Instance;

	[Header("Prefabs")]
	[SerializeField]
	private Giantess giantessPrefab;

	[SerializeField]
	private MicroNpc microPrefab;

	[SerializeField]
	private EntityBase objectPrefab;

	[Space]
	public Vector3 spawnPoint;

	private Camera _camera;

	private GiantessManager _gtsManager;

	private MicroManager _microManager;

	private Dictionary<int, EntityBase> _entityDictionary;

	public Dictionary<int, Giantess> GiantessDictionary;

	public Dictionary<int, EntityBase> ObjectDictionary;

	public List<Vehicle> vehicles;

	public ICollection<Giantess> giantessList
	{
		get
		{
			return GiantessDictionary.Values;
		}
	}

	public ICollection<Micro> microList
	{
		get
		{
			return _microManager.microDictionary.Values;
		}
	}

	public ICollection<EntityBase> objectList
	{
		get
		{
			return ObjectDictionary.Values;
		}
	}

	public event EntityEvent OnEntityAdd;

	public event EntityEvent OnEntityRemove;

	public event GiantessEvent OnGiantessAdd;

	public event GiantessEvent OnGiantessRemove;

	public event MicroEvent OnMicroAdd;

	public event EntityEvent OnObjectAdd;

	public void RegisterEntity(EntityBase entity)
	{
		EntityEvent onEntityAdd = this.OnEntityAdd;
		if (onEntityAdd != null)
		{
			onEntityAdd(entity);
		}
		_entityDictionary.Add(entity.id, entity);
		if ((bool)entity.GetComponent<Micro>())
		{
			_OnMicroSpawned(entity.GetComponent<Micro>());
		}
		else if ((bool)entity.GetComponent<Giantess>())
		{
			_OnGiantessSpawned(entity.GetComponent<Giantess>());
		}
		else if ((bool)entity.GetComponent<Vehicle>())
		{
			_OnVehicleSpawned(entity.GetComponent<Vehicle>());
		}
		else
		{
			_OnObjectSpawned(entity);
		}
	}

	public void UnregisterEntity(EntityBase entity)
	{
		EntityEvent onEntityRemove = this.OnEntityRemove;
		if (onEntityRemove != null)
		{
			onEntityRemove(entity);
		}
		_entityDictionary.Remove(entity.id);
		if ((bool)(entity as Micro))
		{
			_OnMicroRemoved(entity as Micro);
		}
		else if ((bool)(entity as Giantess))
		{
			_OnGiantessRemoved(entity as Giantess);
		}
		else
		{
			_OnObjectRemoved(entity);
		}
	}

	private void _OnObjectSpawned(EntityBase entity)
	{
		ObjectDictionary[entity.id] = entity;
		EntityEvent onObjectAdd = this.OnObjectAdd;
		if (onObjectAdd != null)
		{
			onObjectAdd(entity);
		}
	}

	private void _OnMicroSpawned(Micro micro)
	{
		MicroEvent onMicroAdd = this.OnMicroAdd;
		if (onMicroAdd != null)
		{
			onMicroAdd(micro);
		}
	}

	private void _OnGiantessSpawned(Giantess giantess)
	{
		GiantessDictionary[giantess.id] = giantess;
		GiantessEvent onGiantessAdd = this.OnGiantessAdd;
		if (onGiantessAdd != null)
		{
			onGiantessAdd(giantess);
		}
	}

	private void _OnVehicleSpawned(Vehicle vehicle)
	{
		vehicles.Add(vehicle);
	}

	private void _OnObjectRemoved(EntityBase entity)
	{
		ObjectDictionary[entity.id] = entity;
		EntityEvent onObjectAdd = this.OnObjectAdd;
		if (onObjectAdd != null)
		{
			onObjectAdd(entity);
		}
	}

	private void _OnMicroRemoved(Micro micro)
	{
		MicroEvent onMicroAdd = this.OnMicroAdd;
		if (onMicroAdd != null)
		{
			onMicroAdd(micro);
		}
	}

	private void _OnGiantessRemoved(Giantess giantess)
	{
		if (GiantessDictionary.ContainsKey(giantess.id))
		{
			GiantessDictionary.Remove(giantess.id);
		}
		_gtsManager.RemoveGiantess(giantess.id);
		GiantessEvent onGiantessRemove = this.OnGiantessRemove;
		if (onGiantessRemove != null)
		{
			onGiantessRemove(giantess);
		}
	}

	public static void UpdatePlayerSpeed()
	{
		Humanoid humanoid = LocalClient.Instance.Player.Entity as Humanoid;
		if ((bool)humanoid)
		{
			UpdateHumanoidSpeed(humanoid, GameController.macroSpeed);
		}
	}

	public static void UpdateHumanoidSpeed(Humanoid humanoid)
	{
		float num = GameController.macroSpeed;
		if (GameController.bulletTimeActive && !humanoid.isPlayer)
		{
			num *= GameController.bulletTimeFactor;
		}
		UpdateHumanoidSpeed(humanoid, num);
	}

	public static void UpdateHumanoidSpeed(Humanoid humanoid, float speed)
	{
		humanoid.animationSpeed = speed;
		humanoid.animationManager.UpdateAnimationSpeed();
	}

	public static void UpdateGiantessSpeed(Giantess giantess)
	{
		float num = GameController.macroSpeed;
		if (GameController.bulletTimeActive && !giantess.isPlayer)
		{
			num *= GameController.bulletTimeFactor;
		}
		UpdateHumanoidSpeed(giantess, num);
	}

	public void UpdateGiantessSpeeds()
	{
		float num = GameController.macroSpeed;
		if (GameController.bulletTimeActive)
		{
			num *= GameController.bulletTimeFactor;
		}
		foreach (KeyValuePair<int, Giantess> item in GiantessDictionary)
		{
			UpdateHumanoidSpeed(item.Value, num);
		}
	}

	public static void UpdateMicroSpeed(Micro micro)
	{
		float num = GameController.microSpeed;
		if (GameController.bulletTimeActive && GameController.microsAffectedByBulletTime)
		{
			num *= GameController.bulletTimeFactor;
		}
		UpdateHumanoidSpeed(micro, num);
	}

	public void UpdateMicroSpeeds()
	{
		float num = GameController.microSpeed;
		if (GameController.bulletTimeActive && GameController.microsAffectedByBulletTime)
		{
			num *= GameController.bulletTimeFactor;
		}
		foreach (Micro micro in microList)
		{
			UpdateHumanoidSpeed(micro, num);
		}
	}

	public Giantess InstantiateGiantess(AssetDescription assetDescription, Vector3 position, Quaternion rotation, float scale = 1f, int id = -1)
	{
		Giantess giantess = UnityEngine.Object.Instantiate(giantessPrefab, position, rotation);
		if (id != -1)
		{
			giantess.id = id;
		}
		giantess.RegisterModelAsset(assetDescription);
		giantess.ChangeScale(scale);
		return giantess;
	}

	public Giantess InstantiateGiantess(AssetDescription assetDescription, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		Giantess giantess = UnityEngine.Object.Instantiate(giantessPrefab, position, rotation);
		giantess.OnInitialized += callback;
		giantess.RegisterModelAsset(assetDescription);
		giantess.ChangeScale(scale);
		return giantess;
	}

	public MicroNpc InstantiateMicro(AssetDescription assetDescription, Vector3 position, Quaternion rotation, float scale = 1f, int id = -1)
	{
		MicroNpc microNpc = UnityEngine.Object.Instantiate(microPrefab, position, rotation);
		if (id != -1)
		{
			microNpc.id = id;
		}
		microNpc.RegisterModelAsset(assetDescription);
		microNpc.ChangeScale(scale);
		return microNpc;
	}

	public MicroNpc InstantiateMicro(AssetDescription assetDescription, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		MicroNpc microNpc = UnityEngine.Object.Instantiate(microPrefab, position, rotation);
		microNpc.OnInitialized += callback;
		microNpc.RegisterModelAsset(assetDescription);
		microNpc.ChangeScale(scale);
		return microNpc;
	}

	public EntityBase InstantiateObject(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale = 1f, int id = -1)
	{
		EntityBase entityBase = ((assetDesc.AssetType != AssetType.InternalPremade) ? UnityEngine.Object.Instantiate(objectPrefab, position, rotation) : UnityEngine.Object.Instantiate(assetDesc.Asset, position, rotation).GetComponent<EntityBase>());
		if (id != -1)
		{
			entityBase.id = id;
		}
		entityBase.RegisterModelAsset(assetDesc);
		entityBase.ChangeScale(scale);
		return entityBase;
	}

	public EntityBase InstantiateObject(AssetDescription assetDesc, Vector3 position, Quaternion rotation, float scale, Action<EntityBase> callback)
	{
		EntityBase entityBase = ((assetDesc.AssetType != AssetType.InternalPremade) ? UnityEngine.Object.Instantiate(objectPrefab, position, rotation) : UnityEngine.Object.Instantiate(assetDesc.Asset, position, rotation).GetComponent<EntityBase>());
		entityBase.OnInitialized += callback;
		entityBase.RegisterModelAsset(assetDesc);
		entityBase.ChangeScale(scale);
		return entityBase;
	}

	public EntityBase GetById(int id)
	{
		EntityBase value;
		_entityDictionary.TryGetValue(id, out value);
		return value;
	}

	private void Awake()
	{
		Instance = this;
		_gtsManager = new GiantessManager();
		_microManager = new MicroManager();
		_camera = Camera.main;
		if ((bool)_camera)
		{
			spawnPoint = _camera.transform.position;
		}
		else
		{
			spawnPoint = Vector3.zero;
		}
		_entityDictionary = new Dictionary<int, EntityBase>();
		GiantessDictionary = new Dictionary<int, Giantess>();
		ObjectDictionary = new Dictionary<int, EntityBase>();
		vehicles = new List<Vehicle>();
	}

	private void Update()
	{
		_microManager.FrameUpdate();
	}
}
