using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.ProceduralCityGenerator;
using SaveDataStructures;
using SizeboxUI;
using UnityEngine;

public class CityBuilder : EntityBase, ISavable, IGameObject
{
	public enum TileType
	{
		NoBuild = 0,
		CanBuild = 1,
		Intersection = 2,
		Road = 3,
		Occupied = 4
	}

	private enum Direction
	{
		Horizontal = 0,
		Vertical = 1
	}

	public class Tile
	{
		public int x;

		public int y;

		public int ux;

		public int uy;

		public TileType type;

		public Vector3 localPosition;

		public Vector3[] points;

		public Tile(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass88_0
	{
		public bool visible;

		internal bool _003CMakeBuildingsVisible_003Eb__0(Renderer x)
		{
			return x.enabled = visible;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass89_0
	{
		public bool visible;

		internal bool _003CMakeRoadsVisible_003Eb__0(Renderer x)
		{
			return x.enabled = visible;
		}
	}

	private bool isBuildingsVisible = true;

	private bool isRoadsVisible = true;

	private float maxVisibleDistance = 30000f;

	private float maxVisibleDistanceRoad = 15000f;

	private Tile[,] tiles;

	private float[,] perlinValue;

	[SerializeField]
	private CityDebrisGroup debrisGroupPrefab;

	[Header("Zone Prefabs")]
	[SerializeField]
	private GameObject[] suburbZoneBuildings;

	[SerializeField]
	private GameObject[] officeZoneBuildings;

	[SerializeField]
	private GameObject[] skyscrapers;

	[Header("Street Prefabs")]
	[SerializeField]
	private GameObject intersectionPrefab;

	[SerializeField]
	private GameObject straightPrefab;

	[SerializeField]
	private GameObject endPrefab;

	[SerializeField]
	private GameObject curvePrefab;

	[SerializeField]
	private GameObject junctionPrefab;

	[Space]
	[SerializeField]
	private GameObject cityZoneCollider;

	private CityPopulationManager PopManager;

	private CityDestructionManager mDestructionManager;

	public float buildingDensity = 0.4f;

	public float tileSize = 15f;

	public int radius = 80;

	private int diameter;

	public int streetModule = 5;

	private GameObject buildingRoot;

	private GameObject roadRoot;

	private GameObject floorRoot;

	public float superiorLimit = 300f;

	public float inferiorLimit = 100f;

	public float maxSlope = 30f;

	public float scaleNoise = 1f;

	public float buildScaleModification = 1f;

	public float buildingHeightBonus;

	[SerializeField]
	protected float previewBuildingHeight = 150f;

	public static float cPerlinThreshold = 0.4f;

	public const float cMediumThreshold = 0.5f;

	public const float cTallThreshold = 0.8f;

	private float populationOffset;

	private bool seedExists;

	private int cityGenSeed;

	private float cityScale = 1f;

	private float streetOffset = 0.2f;

	private float floorOffset = 0.1f;

	public Material floorMaterial;

	public Material lowPoly;

	private CityCreationPopUp creationPopup;

	private bool AutoPopulate;

	private bool mLowEndMode;

	private AudioSource audioSource;

	public float minSoundDistance;

	public float maxSoundDistance;

	private Transform placeHolder;

	private Camera mainCamera;

	private bool isPlaced;

	private Collider parentCollider;

	private List<Tile> intersections;

	private readonly SpawningCacheHolder buildingCreationCache = new SpawningCacheHolder();

	private int cellsPerCycle = 10;

	public static CityDebrisGroup DebrisGroupPrefab { get; private set; }

	protected override void FinishInitialization()
	{
		Init();
		base.FinishInitialization();
	}

	protected override void Awake()
	{
		DebrisGroupPrefab = debrisGroupPrefab;
		base.Awake();
	}

	private void Init()
	{
		rotationEnabled = false;
		mainCamera = Camera.main;
		placeHolder = base.transform.GetChild(0);
		UpdatePreview();
		GameObject[] array = suburbZoneBuildings;
		foreach (GameObject building in array)
		{
			InitializeBuilding(building);
		}
		array = officeZoneBuildings;
		foreach (GameObject building2 in array)
		{
			InitializeBuilding(building2);
		}
		array = skyscrapers;
		foreach (GameObject building3 in array)
		{
			InitializeBuilding(building3);
		}
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.dopplerLevel = 0f;
		audioSource.spatialBlend = 0f;
		audioSource.outputAudioMixerGroup = SoundManager.AudioMixerBackground;
		minSoundDistance = (float)radius * tileSize;
		maxSoundDistance = (float)radius * tileSize * 2f;
		ResetCity();
	}

	private void ResetCity()
	{
		if (roadRoot != null)
		{
			Object.Destroy(roadRoot);
		}
		if (buildingRoot != null)
		{
			Object.Destroy(buildingRoot);
		}
		if (floorRoot != null)
		{
			Object.Destroy(floorRoot);
		}
		ColliderList = new Collider[0];
		buildingRoot = new GameObject("Buildings");
		buildingRoot.transform.SetParent(base.transform, false);
		roadRoot = new GameObject("Roads");
		roadRoot.transform.SetParent(base.transform, false);
		placeHolder.gameObject.SetActive(true);
		isPlaced = false;
	}

	private void ResetCityLod()
	{
		float magnitude = (mainCamera.transform.position - base.transform.position).magnitude;
		if (magnitude < maxVisibleDistanceRoad)
		{
			MakeRoadsVisible(true);
		}
		else
		{
			MakeRoadsVisible(false);
		}
		if (magnitude < maxVisibleDistance)
		{
			MakeBuildingsVisible(true);
		}
		else
		{
			MakeBuildingsVisible(false);
		}
	}

	public override void Move(Vector3 position)
	{
		if (isPlaced)
		{
			ResetCity();
		}
		base.Move(position);
	}

	public override void ChangeRotation(Vector3 newRotation)
	{
	}

	public override void ChangeVerticalOffset(float newOffset)
	{
	}

	public override void ChangeScale(float newScale)
	{
		if (!isPlaced)
		{
			base.ChangeScale(newScale);
		}
	}

	public override void Place()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("UI/CityGeneratorUi") as GameObject);
		gameObject.transform.SetParent(GuiManager.Instance.MainCanvasGameObject.transform, false);
		creationPopup = gameObject.AddComponent<CityCreationPopUp>();
		creationPopup.SetHandlers(_003CPlace_003Eb__73_0, Cancel, UpdatePreview);
	}

	private void UpdateCreationValues()
	{
		if (!(creationPopup == null))
		{
			populationOffset = (float)creationPopup.PopModifierValue / 250f;
			radius = creationPopup.CityRadiusValue;
			buildingHeightBonus = creationPopup.SkyScraperHeightValue;
			scaleNoise = (float)creationPopup.OpenSpaceChanceValue / 100f * 8f;
			AutoPopulate = creationPopup.AutoPoplulate;
			mLowEndMode = GlobalPreferences.LowEndCities.value;
			seedExists = !string.IsNullOrEmpty(creationPopup.SeedValue);
			if (seedExists)
			{
				cityGenSeed = creationPopup.SeedValue.GetHashCode();
			}
		}
	}

	private void UpdatePreview()
	{
		UpdateCreationValues();
		placeHolder.localScale = new Vector3((float)radius * tileSize * 1.4f, previewBuildingHeight * (1f + buildingHeightBonus / 100f), (float)radius * tileSize * 1.4f);
	}

	private void Cancel()
	{
		Object.Destroy(base.gameObject);
	}

	private GameObject CloneBuildingPrefab(GameObject building, bool cloneChunks)
	{
		GameObject gameObject = Object.Instantiate(building);
		if (!cloneChunks)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				if (child.name == "Chunks")
				{
					Object.Destroy(child.gameObject);
					break;
				}
			}
		}
		gameObject.transform.position += new Vector3(0f, -400f, 0f);
		return gameObject;
	}

	private void BuildCity(bool asyncronous = true)
	{
		placeHolder.gameObject.SetActive(false);
		UpdateCreationValues();
		if (AutoPopulate)
		{
			Vector3 position = placeHolder.position;
			Vector3 lossyScale = placeHolder.lossyScale;
			PopManager = buildingRoot.AddComponent<CityPopulationManager>();
			PopManager.SetParameters(position, lossyScale, buildingRoot, Scale);
		}
		else
		{
			PopManager = null;
		}
		if (!seedExists)
		{
			cityGenSeed = Random.Range(int.MinValue, int.MaxValue);
		}
		Random.InitState(cityGenSeed);
		if (!mLowEndMode)
		{
			mDestructionManager = buildingRoot.AddComponent<CityDestructionManager>();
			mDestructionManager.CityManager = PopManager;
		}
		for (int i = 0; i < suburbZoneBuildings.Length; i++)
		{
			suburbZoneBuildings[i] = SetBuildingPrefabs(suburbZoneBuildings[i]);
		}
		for (int j = 0; j < officeZoneBuildings.Length; j++)
		{
			officeZoneBuildings[j] = SetBuildingPrefabs(officeZoneBuildings[j]);
		}
		for (int k = 0; k < skyscrapers.Length; k++)
		{
			skyscrapers[k] = SetBuildingPrefabs(skyscrapers[k]);
		}
		creationPopup = null;
		cityScale = base.transform.lossyScale.y;
		if (base.transform.parent != null)
		{
			parentCollider = base.transform.parent.GetComponent<Collider>();
		}
		else
		{
			parentCollider = null;
		}
		diameter = radius * 2 + 1;
		perlinValue = SamplePerlinNoise(diameter);
		tiles = new Tile[diameter, diameter];
		StartCoroutine(ExecuteCityBuilding(asyncronous));
	}

	private GameObject SetBuildingPrefabs(GameObject building)
	{
		GameObject gameObject = CloneBuildingPrefab(building, !mLowEndMode);
		InitializeBuilding(gameObject);
		return gameObject;
	}

	private IEnumerator ExecuteCityBuilding(bool asyncronous = true)
	{
		DetermineBuildableArea();
		if (asyncronous)
		{
			yield return null;
		}
		CreateStreets();
		if (asyncronous)
		{
			yield return null;
		}
		CreateFloor();
		if (asyncronous)
		{
			yield return null;
		}
		while (!CreateBuildings())
		{
			if (asyncronous)
			{
				yield return null;
			}
		}
		if (asyncronous)
		{
			yield return null;
		}
		if (AutoPopulate)
		{
			GameObject gameObject = Object.Instantiate(cityZoneCollider, base.transform, true);
			Transform transform = placeHolder.transform;
			gameObject.transform.localScale = transform.localScale;
			gameObject.transform.position = transform.position;
			PopManager.SetTriggerZone(gameObject);
			PopManager.PopStart();
		}
		isPlaced = true;
		ResetCityLod();
		if (mLowEndMode)
		{
			for (int i = 0; i < suburbZoneBuildings.Length; i++)
			{
				Object.Destroy(suburbZoneBuildings[i]);
			}
			for (int j = 0; j < officeZoneBuildings.Length; j++)
			{
				Object.Destroy(officeZoneBuildings[j]);
			}
			for (int k = 0; k < skyscrapers.Length; k++)
			{
				Object.Destroy(skyscrapers[k]);
			}
		}
	}

	private void DetermineBuildableArea()
	{
		for (int i = -radius; i < radius; i++)
		{
			for (int j = -radius; j < radius; j++)
			{
				int num = i + radius;
				int num2 = j + radius;
				Tile tile = new Tile(i, j);
				tile.ux = num;
				tile.uy = num2;
				if (perlinValue[num, num2] > cPerlinThreshold)
				{
					tile.type = TileType.CanBuild;
				}
				else
				{
					tile.type = TileType.NoBuild;
				}
				tiles[num, num2] = tile;
			}
		}
	}

	private void CreateStreets()
	{
		intersections = new List<Tile>();
		for (int i = -radius; i < radius; i++)
		{
			for (int j = -radius; j < radius; j++)
			{
				int num = i + radius;
				int num2 = j + radius;
				Tile tile = tiles[num, num2];
				if (tile.type == TileType.CanBuild && (i % streetModule == 0 || j % streetModule == 0) && i % streetModule == 0 && j % streetModule == 0 && PlanIntersection(tile))
				{
					CheckCloseIntersection(num, num2);
					intersections.Add(tile);
				}
			}
		}
		foreach (Tile intersection in intersections)
		{
			SpawnIntersection(intersection);
		}
		StaticBatchingUtility.Combine(roadRoot);
	}

	private void CreateFloor()
	{
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector2> list3 = new List<Vector2>();
		foreach (Tile intersection in intersections)
		{
			int ux = intersection.ux;
			int uy = intersection.uy;
			if (uy + streetModule < diameter && ux + streetModule < diameter)
			{
				Tile[] array = new Tile[3]
				{
					tiles[ux, uy + streetModule],
					tiles[ux + streetModule, uy],
					tiles[ux + streetModule, uy + streetModule]
				};
				if (array[0].type == TileType.Intersection && array[1].type == TileType.Intersection && array[2].type == TileType.Intersection)
				{
					int count = list.Count;
					Vector3 item = intersection.localPosition + new Vector3(tileSize * 0.5f, floorOffset, tileSize * 0.5f);
					Vector3 item2 = array[0].localPosition + new Vector3(tileSize * 0.5f, floorOffset, (0f - tileSize) * 0.5f);
					Vector3 item3 = array[2].localPosition + new Vector3((0f - tileSize) * 0.5f, floorOffset, (0f - tileSize) * 0.5f);
					Vector3 item4 = array[1].localPosition + new Vector3((0f - tileSize) * 0.5f, floorOffset, tileSize * 0.5f);
					list.Add(item);
					list.Add(item2);
					list.Add(item3);
					list.Add(item4);
					list3.Add(new Vector2(0f, 0f));
					list3.Add(new Vector2(1f, 0f));
					list3.Add(new Vector2(1f, 1f));
					list3.Add(new Vector2(0f, 1f));
					list2.Add(count);
					list2.Add(count + 1);
					list2.Add(count + 2);
					list2.Add(count);
					list2.Add(count + 2);
					list2.Add(count + 3);
				}
			}
		}
		floorRoot = new GameObject("Floor");
		floorRoot.layer = Layers.mapLayer;
		floorRoot.transform.SetParent(base.transform, false);
		MeshFilter meshFilter = floorRoot.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = floorRoot.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = floorRoot.AddComponent<MeshCollider>();
		Mesh mesh = new Mesh();
		mesh.SetVertices(list);
		mesh.SetTriangles(list2, 0);
		mesh.SetUVs(0, list3);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
		meshRenderer.material = floorMaterial;
	}

	private bool CreateBuildings()
	{
		if (buildingCreationCache.NeedsReInit)
		{
			buildingCreationCache.X = -radius;
			buildingCreationCache.Y = -radius;
			buildingCreationCache.NeedsReInit = false;
		}
		int num = 0;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = buildingCreationCache.X; i < radius; i++)
		{
			buildingCreationCache.X = i;
			for (int j = buildingCreationCache.Y; j < radius; j++)
			{
				int num2 = i + radius;
				int num3 = j + radius;
				if (tiles[num2, num3].type == TileType.CanBuild)
				{
					InstantiateBuilding(i, j, GenerateBuildingHeight(num2, num3));
					num++;
				}
				buildingCreationCache.Y = j;
				if (num > cellsPerCycle)
				{
					stopwatch.Stop();
					RecalculateSreamingLoad(stopwatch.ElapsedMilliseconds);
					return false;
				}
			}
			buildingCreationCache.Y = -radius;
		}
		stopwatch.Stop();
		return true;
	}

	private void RecalculateSreamingLoad(long pastMs)
	{
		if (pastMs < 20)
		{
			cellsPerCycle++;
		}
		else if (pastMs > 12 && cellsPerCycle > 1)
		{
			cellsPerCycle--;
		}
	}

	private float GenerateBuildingHeight(int x, int y)
	{
		return perlinValue[x, y] + populationOffset;
	}

	private void Update()
	{
		float magnitude = (mainCamera.transform.position - base.transform.position).magnitude;
		magnitude /= Scale;
		if (magnitude < maxSoundDistance && !audioSource.isPlaying)
		{
			audioSource.clip = SoundManager.Instance.GetCitySoundPeaceful();
			audioSource.Play();
		}
		if (magnitude < minSoundDistance)
		{
			audioSource.volume = SoundManager.AmbientVolume;
		}
		else if (magnitude < maxSoundDistance)
		{
			audioSource.volume = SoundManager.AmbientVolume * (1f - (magnitude - minSoundDistance) / (maxSoundDistance - minSoundDistance));
		}
		else
		{
			audioSource.volume = 0f;
			audioSource.Stop();
		}
		if (!isBuildingsVisible && magnitude < maxVisibleDistance)
		{
			MakeBuildingsVisible(true);
		}
		else if (isBuildingsVisible && magnitude > maxVisibleDistance)
		{
			MakeBuildingsVisible(false);
		}
		if (!isRoadsVisible && magnitude < maxVisibleDistanceRoad)
		{
			MakeRoadsVisible(true);
		}
		else if (isRoadsVisible && magnitude > maxVisibleDistanceRoad)
		{
			MakeRoadsVisible(false);
		}
	}

	private void MakeBuildingsVisible(bool visible)
	{
		_003C_003Ec__DisplayClass88_0 _003C_003Ec__DisplayClass88_ = new _003C_003Ec__DisplayClass88_0();
		_003C_003Ec__DisplayClass88_.visible = visible;
		if ((bool)buildingRoot && (bool)floorRoot)
		{
			buildingRoot.GetComponentsInChildren<Renderer>().All(_003C_003Ec__DisplayClass88_._003CMakeBuildingsVisible_003Eb__0);
			MeshRenderer component = floorRoot.GetComponent<MeshRenderer>();
			if (_003C_003Ec__DisplayClass88_.visible)
			{
				component.material = floorMaterial;
			}
			else
			{
				component.material = lowPoly;
			}
			isBuildingsVisible = _003C_003Ec__DisplayClass88_.visible;
		}
	}

	private void MakeRoadsVisible(bool visible)
	{
		_003C_003Ec__DisplayClass89_0 _003C_003Ec__DisplayClass89_ = new _003C_003Ec__DisplayClass89_0();
		_003C_003Ec__DisplayClass89_.visible = visible;
		if ((bool)roadRoot)
		{
			roadRoot.GetComponentsInChildren<Renderer>().All(_003C_003Ec__DisplayClass89_._003CMakeRoadsVisible_003Eb__0);
			isRoadsVisible = _003C_003Ec__DisplayClass89_.visible;
		}
	}

	private void InitializeBuilding(GameObject building)
	{
		building.gameObject.layer = Layers.buildingLayer;
		if (building.GetComponent<CityBuilding>() == null)
		{
			building.AddComponent<CityBuilding>();
		}
	}

	private GameObject InstantiateBuilding(int x, int y, float buildingValue)
	{
		if (seedExists)
		{
			Random.InitState(x * 2 * (y * 3) * cityGenSeed);
		}
		if (Random.value > buildingDensity)
		{
			return null;
		}
		int num = x + radius;
		int num2 = y + radius;
		int num3 = Random.Range(0, 4);
		bool num4 = num3 == 1 || num3 == 3;
		GameObject gameObject = null;
		if (buildingValue > 0.8f)
		{
			int num5 = Random.Range(0, skyscrapers.Length);
			gameObject = skyscrapers[num5];
		}
		else if (buildingValue > 0.5f)
		{
			int num6 = Random.Range(0, officeZoneBuildings.Length);
			gameObject = officeZoneBuildings[num6];
		}
		else
		{
			int num7 = Random.Range(0, suburbZoneBuildings.Length);
			gameObject = suburbZoneBuildings[num7];
		}
		Vector3 localScale;
		if (buildingValue > 0.5f)
		{
			float num8 = 1f;
			num8 += buildScaleModification * (2f * Random.value - 1f);
			num8 *= 1f + buildingHeightBonus / 100f;
			localScale = new Vector3(1f, num8, 1f);
		}
		else
		{
			localScale = Vector3.one;
		}
		CityBuilding component = gameObject.GetComponent<CityBuilding>();
		int num9 = component.xSize;
		int num10 = component.zSize;
		if (num4)
		{
			num9 = num10;
			num10 = component.xSize;
		}
		if (num + num9 > diameter || num2 + num10 > diameter)
		{
			return null;
		}
		for (int i = num; i < num + num9; i++)
		{
			for (int j = num2; j < num2 + num10; j++)
			{
				if (tiles[i, j].type != TileType.CanBuild)
				{
					return null;
				}
			}
		}
		Vector3[] array = CalculateCorners(tiles[num, num2], num9, num10);
		Vector3 vector = array[4];
		bool flag = false;
		for (int k = 0; k < array.Length; k++)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(base.transform.TransformPoint(array[k] + Vector3.up * superiorLimit), -Vector3.up, out hitInfo, (superiorLimit + inferiorLimit) * cityScale))
			{
				continue;
			}
			Vector3 vector2 = base.transform.InverseTransformPoint(hitInfo.point);
			if (!flag || vector2.y < vector.y)
			{
				vector.y = vector2.y;
				if (Mathf.Abs(Vector3.Angle(Vector3.up, hitInfo.normal)) > maxSlope)
				{
					return null;
				}
				flag = true;
			}
		}
		if (!flag)
		{
			return null;
		}
		for (int l = num; l < num + num9; l++)
		{
			for (int m = num2; m < num2 + num10; m++)
			{
				tiles[l, m].type = TileType.Occupied;
			}
		}
		GameObject gameObject2 = Object.Instantiate(gameObject);
		gameObject2.transform.SetParent(buildingRoot.transform, false);
		gameObject2.transform.localPosition = vector;
		gameObject2.transform.localRotation = Quaternion.AngleAxis(90f * (float)num3, Vector3.up);
		gameObject2.transform.localScale = localScale;
		CityBuilding component2 = gameObject2.GetComponent<CityBuilding>();
		component2.Position = vector;
		component2.DestructionManager = mDestructionManager;
		if (parentCollider != null)
		{
			gameObject2.GetComponent<CityBuilding>().IgnoreCollision(parentCollider);
		}
		if (PopManager != null)
		{
			CityBuilding component3 = gameObject2.GetComponent<CityBuilding>();
			Vector3[] array2 = new Vector3[array.Length];
			for (int n = 0; n < array.Length; n++)
			{
				array2[n] = base.transform.TransformPoint(array[n]);
			}
			component3.CellArea = array2;
			PopManager.AddBuilding(component3);
		}
		DisableSmokeIfRequired(gameObject2);
		return gameObject2;
	}

	private void DisableSmokeIfRequired(GameObject building)
	{
		if (mLowEndMode || GlobalPreferences.SmokeEnabled.value)
		{
			return;
		}
		for (int i = 0; i < building.transform.childCount; i++)
		{
			Transform child = building.transform.GetChild(i);
			if (child.name == "Chunks")
			{
				FracturedObject component = child.GetComponent<FracturedObject>();
				component.EventExplosionPrefabsArray = new FracturedObject.PrefabInfo[0];
				component.EventDetachPrefabsArray = new FracturedObject.PrefabInfo[0];
				component.EventImpactPrefabsArray = new FracturedObject.PrefabInfo[0];
				component.EventDetachedPrefabsArray = new FracturedObject.PrefabInfo[0];
				break;
			}
		}
	}

	private Vector3[] CalculateCorners(Tile tile, int xSize = 1, int zSize = 1)
	{
		Vector3[] array = new Vector3[5];
		array[0] = new Vector3((float)tile.x * tileSize - tileSize / 2f, 0f, (float)tile.y * tileSize - tileSize / 2f);
		array[1] = array[0] + new Vector3(0f, 0f, tileSize * (float)zSize);
		array[2] = array[0] + new Vector3(tileSize * (float)xSize, 0f, 0f);
		array[3] = array[0] + new Vector3(tileSize * (float)xSize, 0f, tileSize * (float)zSize);
		array[4] = (array[0] + array[3]) / 2f;
		return array;
	}

	private bool PlanIntersection(Tile tile)
	{
		Vector3[] array = CalculateCorners(tile);
		bool flag = false;
		Vector3 localPosition = array[4];
		Vector3 up = Vector3.up;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit hitInfo;
			if (!Physics.Raycast(base.transform.TransformPoint(array[i] + Vector3.up * superiorLimit), -Vector3.up, out hitInfo, (superiorLimit + inferiorLimit) * cityScale))
			{
				continue;
			}
			Vector3 vector = base.transform.InverseTransformPoint(hitInfo.point);
			if (!flag || vector.y > localPosition.y)
			{
				localPosition.y = vector.y;
				up = hitInfo.normal;
				flag = true;
				if (Mathf.Abs(Vector3.Angle(Vector3.up, up)) > maxSlope)
				{
					return false;
				}
			}
		}
		if (flag)
		{
			tile.localPosition = localPosition;
			tile.localPosition.y += streetOffset;
			tile.type = TileType.Intersection;
			tile.points = array;
		}
		return flag;
	}

	private void SpawnIntersection(Tile tile)
	{
		bool[] array = new bool[4];
		int num = 0;
		int ux = tile.ux;
		int uy = tile.uy;
		array[0] = uy + 1 < diameter && tiles[ux, uy + 1].type == TileType.Road;
		array[1] = uy - 1 >= 0 && tiles[ux, uy - 1].type == TileType.Road;
		array[2] = ux - 1 >= 0 && tiles[ux - 1, uy].type == TileType.Road;
		array[3] = ux + 1 < diameter && tiles[ux + 1, uy].type == TileType.Road;
		for (int i = 0; i < 4; i++)
		{
			if (array[i])
			{
				num++;
			}
		}
		GameObject gameObject = null;
		float num2 = 0f;
		switch (num)
		{
		case 1:
			gameObject = Object.Instantiate(endPrefab);
			if (array[1])
			{
				num2 = 180f;
			}
			if (array[2])
			{
				num2 = -90f;
			}
			if (array[3])
			{
				num2 = 90f;
			}
			break;
		case 2:
			if ((array[0] & array[1]) || (array[2] && array[3]))
			{
				gameObject = Object.Instantiate(straightPrefab);
				if (array[2] && array[3])
				{
					num2 = 90f;
				}
				break;
			}
			gameObject = Object.Instantiate(curvePrefab);
			if (array[0] && array[2])
			{
				num2 = -90f;
			}
			if (array[1] && array[2])
			{
				num2 = 180f;
			}
			if (array[1] && array[3])
			{
				num2 = 90f;
			}
			break;
		case 3:
			gameObject = Object.Instantiate(junctionPrefab);
			if (!array[0])
			{
				num2 = 90f;
			}
			if (!array[1])
			{
				num2 = -90f;
			}
			if (!array[3])
			{
				num2 = 180f;
			}
			break;
		case 4:
			gameObject = Object.Instantiate(intersectionPrefab);
			break;
		}
		if (!(gameObject == null))
		{
			gameObject.transform.SetParent(roadRoot.transform, false);
			gameObject.transform.localPosition = tile.localPosition;
			if (num2 != 0f)
			{
				gameObject.transform.localRotation = Quaternion.Euler(0f, num2, 0f);
			}
		}
	}

	private GameObject CheckCloseIntersection(int x, int y)
	{
		int num = streetModule;
		Tile tile = tiles[x, y];
		if (x - num > 0)
		{
			Tile tile2 = tiles[x - num, y];
			if (tile2.type == TileType.Intersection)
			{
				Vector3 vector = tile.localPosition - Vector3.right * tileSize / 2f;
				Vector3 vector2 = tile2.localPosition + Vector3.right * tileSize / 2f;
				if (Mathf.Abs((vector - vector2).y) < tileSize * (float)streetModule && PlaceRoad(vector, vector2, Direction.Horizontal))
				{
					PlanRoad(tile, tile2);
				}
			}
		}
		if (y - num > 0)
		{
			Tile tile3 = tiles[x, y - num];
			if (tile3.type == TileType.Intersection)
			{
				Vector3 vector3 = tile.localPosition - Vector3.forward * tileSize / 2f;
				Vector3 vector4 = tile3.localPosition + Vector3.forward * tileSize / 2f;
				if (Mathf.Abs((vector3 - vector4).y) < tileSize * (float)streetModule && PlaceRoad(vector3, vector4, Direction.Vertical))
				{
					PlanRoad(tile, tile3);
				}
			}
		}
		return null;
	}

	private void PlanRoad(Tile start, Tile end)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (start.ux == end.ux)
		{
			num = start.ux;
			num2 = start.ux;
			num3 = Mathf.Min(start.uy, end.uy) + 1;
			num4 = Mathf.Max(start.uy, end.uy) - 1;
		}
		if (start.uy == end.uy)
		{
			num3 = start.uy;
			num4 = start.uy;
			num = Mathf.Min(start.ux, end.ux) + 1;
			num2 = Mathf.Max(start.ux, end.ux) - 1;
		}
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				tiles[i, j].type = TileType.Road;
			}
		}
	}

	private bool PlaceRoad(Vector3 startPoint, Vector3 endPoint, Direction direction)
	{
		Vector3 vector = base.transform.TransformPoint(startPoint);
		Vector3 vector2 = base.transform.TransformPoint(endPoint) - vector;
		float magnitude = vector2.magnitude;
		float magnitude2 = (startPoint - endPoint).magnitude;
		if (Physics.Raycast(vector, vector2, magnitude))
		{
			return false;
		}
		UnityEngine.Debug.DrawRay(vector, vector2, Color.yellow, 60f);
		Vector3 vector3 = ((direction != 0) ? Vector3.right : Vector3.forward);
		Vector3 vector4 = base.transform.TransformPoint(startPoint + vector3 * tileSize / 4f);
		Vector3 origin = base.transform.TransformPoint(startPoint - vector3 * tileSize / 4f);
		UnityEngine.Debug.DrawRay(vector4, vector2, Color.yellow, 60f);
		if (Physics.Raycast(vector4, vector2, magnitude))
		{
			return false;
		}
		if (Physics.Raycast(origin, vector2, magnitude))
		{
			return false;
		}
		Vector3 localPosition = (endPoint + startPoint) / 2f;
		GameObject gameObject = Object.Instantiate(straightPrefab);
		gameObject.transform.SetParent(roadRoot.transform, false);
		float x = Mathf.Asin((Vector3.up * (endPoint.y - startPoint.y)).y / magnitude2) * 57.29578f;
		gameObject.transform.localPosition = localPosition;
		if (direction == Direction.Vertical)
		{
			gameObject.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		}
		else
		{
			gameObject.transform.localRotation = Quaternion.Euler(x, 90f, 0f);
		}
		gameObject.transform.localScale = new Vector3(1f, 1f, magnitude2 / tileSize);
		return true;
	}

	private float[,] SamplePerlinNoise(int length)
	{
		float num = Random.value * 10f;
		float[,] array = new float[length, length];
		float num2 = (length - 1) / 2;
		float num3 = num2 * num2;
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				float num4 = (float)i - num2;
				float num5 = (float)j - num2;
				float num6 = num4 * num4;
				float num7 = num5 * num5;
				float value = 0f;
				if (num6 + num7 < num3)
				{
					Vector2 vector = new Vector2(num4, num5);
					if (scaleNoise < 0.3f && vector.magnitude < num2 / 2f)
					{
						value = num2 / 4f / vector.magnitude;
					}
					else
					{
						value = 0f;
						float num8 = (float)i / (float)length;
						float num9 = (float)j / (float)length;
						float num10 = Mathf.PerlinNoise(num + num8 * scaleNoise, num + num9 * scaleNoise);
						float num11 = Mathf.PerlinNoise(num + num8 * scaleNoise * 2f, num + num9 * scaleNoise * 2f);
						float num12 = 1f - vector.magnitude / num2;
						value = (0.5f * num10 + 0.5f * num11) * num12 * 2f;
					}
				}
				array[i, j] = Mathf.Clamp01(value);
			}
		}
		return array;
	}

	public override SavableData Save()
	{
		return new CitySaveData(this, cityGenSeed, isPlaced, populationOffset, radius, buildingHeightBonus, AutoPopulate);
	}

	public override void Load(SavableData inData, bool loadPosition = true)
	{
		CitySaveData citySaveData = (CitySaveData)inData;
		if (citySaveData.isPlaced)
		{
			Object.Destroy(creationPopup);
			creationPopup = null;
			cityGenSeed = citySaveData.seed;
			populationOffset = citySaveData.populationOffset;
			radius = citySaveData.radius;
			buildingHeightBonus = citySaveData.buildingHeight;
			AutoPopulate = citySaveData.populate;
			seedExists = true;
			BuildCity(false);
		}
		base.Load(inData, loadPosition);
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
	private void _003CPlace_003Eb__73_0()
	{
		BuildCity();
	}
}
