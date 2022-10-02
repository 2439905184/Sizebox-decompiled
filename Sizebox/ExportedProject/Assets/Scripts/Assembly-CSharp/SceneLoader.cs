using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Assets.Scripts.DynamicClouds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[Serializable]
	public class SceneElement
	{
		public string scene;

		public Sprite thumbnail;
	}

	public class SceneInfo
	{
		public string Scene;

		public Sprite Thumbnail;

		public AssetData AssetData;
	}

	public class SceneLoading : ScriptableObject
	{
		internal AsyncOperation Load(string scenePath)
		{
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenePath);
			if (asyncOperation == null)
			{
				UnityEngine.Object.Destroy(this);
				return null;
			}
			StateManager.Mouse.Add();
			asyncOperation.completed += OnAsyncOnCompleted;
			return asyncOperation;
		}

		private void OnAsyncOnCompleted(AsyncOperation operation)
		{
			StateManager.Mouse.Remove();
			InputManager.inputPreferences.ApplyMouseSettings();
			UnityEngine.Object.Destroy(this);
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Comparison<AssetData> _003C_003E9__18_0;

		internal int _003CFindScenes_003Eb__18_0(AssetData a, AssetData b)
		{
			return a.Modified.CompareTo(b.Modified);
		}
	}

	private uint _loadFailed;

	public Material defaultSkybox;

	public GameObject playerSpawn;

	public GameObject cloudController;

	public List<SceneElement> localScenes;

	private List<SceneInfo> _allScenes;

	private SceneInfo _loadingScene;

	private List<AssetData> _assetDataList;

	private Dictionary<string, string> _loadedAssetBundles;

	private Dictionary<string, string> _shaderData;

	public static bool Initialized
	{
		get
		{
			return UnityEngine.Object.FindObjectsOfType<SceneLoader>().Length != 0;
		}
	}

	private void Awake()
	{
		if (UnityEngine.Object.FindObjectsOfType<SceneLoader>().Length > 1)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(this);
		SceneManager.sceneLoaded += OnSceneLoaded;
		_loadedAssetBundles = new Dictionary<string, string>();
		_shaderData = new Dictionary<string, string>();
	}

	public List<SceneInfo> GetSceneList()
	{
		if (_allScenes == null)
		{
			_allScenes = new List<SceneInfo>();
			foreach (AssetData item3 in FindScenes())
			{
				SceneInfo item = new SceneInfo
				{
					Scene = item3.Name,
					Thumbnail = TextureLoader.LoadNewSprite(item3.Thumbnail),
					AssetData = item3
				};
				_allScenes.Add(item);
			}
			foreach (SceneElement localScene in localScenes)
			{
				SceneInfo item2 = new SceneInfo
				{
					Scene = localScene.scene,
					Thumbnail = localScene.thumbnail
				};
				_allScenes.Add(item2);
			}
		}
		if (_loadFailed != 0)
		{
			UiMessageBox.Create(_loadFailed + " custom " + ((_loadFailed > 1) ? "scenes" : "scene") + " could not be added. Check log for more details", "Custom Scene Scan Error").Popup();
		}
		return _allScenes;
	}

	public AsyncOperation LoadScene(SceneInfo scene)
	{
		_loadingScene = scene;
		if (scene.AssetData != null)
		{
			return LoadAssetBundleScene(scene.AssetData.Path);
		}
		return LoadScene(scene.Scene);
	}

	public void LoadSceneByName(string sceneName)
	{
		foreach (SceneInfo scene in GetSceneList())
		{
			if (scene.Scene == sceneName)
			{
				LoadScene(scene);
				return;
			}
		}
		Debug.LogError("The scene " + sceneName + " couldn't be found.");
	}

	private IEnumerable<AssetData> FindScenes()
	{
		if (_assetDataList != null)
		{
			return _assetDataList;
		}
		_loadFailed = 0u;
		_assetDataList = new List<AssetData>();
		List<string> list = new List<string>();
		IOManager.AddIf(list, IOManager.GetApplicationDirectory(Path.Combine("Models", "Scenes")));
		IOManager.AddIf(list, IOManager.GetUserDirectory(Path.Combine("Models", "Scenes")));
		IOManager.AddIfExists(list, Sbox.StringPreferenceOrArgument(GlobalPreferences.PathScene, "-path-scenes", "Scenes"));
		foreach (string item in list)
		{
			RecursiveSearchOfAssets(item);
		}
		_assetDataList.Sort(_003C_003Ec._003C_003E9__18_0 ?? (_003C_003Ec._003C_003E9__18_0 = _003C_003Ec._003C_003E9._003CFindScenes_003Eb__18_0));
		_assetDataList.Reverse();
		return _assetDataList;
	}

	private static AssetData ReadAssetDataAtPath(string path, string dir)
	{
		Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(path));
		AssetData obj = new AssetData
		{
			Name = manifest.name,
			Dir = dir + "/"
		};
		obj.Thumbnail = obj.Dir + manifest.thumbnail;
		obj.Data = obj.Dir + manifest.data;
		obj.Path = obj.Dir + manifest.asset;
		obj.Modified = File.GetLastAccessTime(path).Ticks;
		return obj;
	}

	private void RecursiveSearchOfAssets(string dir)
	{
		string[] files = Directory.GetFiles(dir);
		foreach (string text in files)
		{
			if (text.EndsWith("manifest.json"))
			{
				AssetData assetData = ReadAssetDataAtPath(text, dir);
				if (assetData != null)
				{
					AddOnce(assetData);
					return;
				}
			}
		}
		files = Directory.GetDirectories(dir);
		foreach (string dir2 in files)
		{
			RecursiveSearchOfAssets(dir2);
		}
	}

	private void AddOnce(AssetData assetData)
	{
		foreach (AssetData assetData2 in _assetDataList)
		{
			if (assetData2.Name == assetData.Name)
			{
				_loadFailed++;
				Debug.LogWarning("Custom Scene '" + assetData.Dir + "' can't be loaded because '" + assetData2.Dir + "' is already loaded with the same name.");
				return;
			}
		}
		_assetDataList.Add(assetData);
	}

	private AsyncOperation LoadAssetBundleScene(string assetPath)
	{
		string text;
		if (!_loadedAssetBundles.ContainsKey(assetPath))
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(assetPath);
			if (!(assetBundle != null))
			{
				return null;
			}
			text = assetBundle.GetAllScenePaths()[0];
			_loadedAssetBundles[assetPath] = text;
		}
		else
		{
			text = _loadedAssetBundles[assetPath];
		}
		return LoadScene(text);
	}

	private AsyncOperation LoadScene(string scenePath)
	{
		return ScriptableObject.CreateInstance<SceneLoading>().Load(scenePath);
	}

	private List<T> GETAllComponentsInScene<T>(Scene scene)
	{
		List<GameObject> list = new List<GameObject>();
		scene.GetRootGameObjects(list);
		List<T> list2 = new List<T>();
		foreach (GameObject item in list)
		{
			list2.AddRange(item.GetComponentsInChildren<T>(true));
		}
		return list2;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		Debug.Log("Scene Loaded!");
		if (_loadingScene != null && _loadingScene.AssetData != null)
		{
			Debug.Log(_loadingScene.AssetData.Data);
			MaterialData[] materials = JsonUtility.FromJson<SceneData>(File.ReadAllText(_loadingScene.AssetData.Data)).materials;
			foreach (MaterialData materialData in materials)
			{
				_shaderData[materialData.gameobject + materialData.material] = materialData.shader;
			}
			foreach (Renderer item in GETAllComponentsInScene<Renderer>(scene))
			{
				Material[] materials2 = item.materials;
				foreach (Material material in materials2)
				{
					string shaderName = SearchShaderName(item.name, material.name);
					ShaderLoader.SetShaderOnMaterial(material, shaderName);
				}
			}
			if (RenderSettings.skybox != null)
			{
				string text = SearchShaderName("Scene-Skybox", RenderSettings.skybox.name);
				if (text != "")
				{
					ShaderLoader.SetShaderOnMaterial(RenderSettings.skybox, text);
				}
				else
				{
					RenderSettings.skybox = defaultSkybox;
				}
			}
			Terrain[] array = UnityEngine.Object.FindObjectsOfType<Terrain>();
			for (int i = 0; i < array.Length; i++)
			{
				TerrainData terrainData = array[i].terrainData;
				TreePrototype[] treePrototypes = terrainData.treePrototypes;
				for (int j = 0; j < treePrototypes.Length; j++)
				{
					Renderer component = treePrototypes[j].prefab.GetComponent<Renderer>();
					Material[] materials2 = component.sharedMaterials;
					foreach (Material material2 in materials2)
					{
						string shaderName2 = SearchShaderName(component.name, material2.name);
						ShaderLoader.SetShaderOnMaterial(material2, shaderName2);
					}
				}
				terrainData.RefreshPrototypes();
			}
			Camera[] array2 = UnityEngine.Object.FindObjectsOfType<Camera>();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].gameObject.SetActive(false);
			}
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			PlayerSpawnPoint[] array3 = UnityEngine.Object.FindObjectsOfType<PlayerSpawnPoint>();
			Debug.Log("Spawn Points found: " + array3.Length);
			if (array3.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, array3.Length);
				Transform obj = array3[num].transform;
				position = obj.position;
				rotation = obj.rotation;
				MapSettingInternal.startingSize = obj.localScale.y;
				PlayerSpawnPoint[] array4 = array3;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].gameObject.SetActive(false);
				}
			}
			if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
			{
				GameObject obj2 = new GameObject("Event System");
				obj2.AddComponent<EventSystem>();
				obj2.AddComponent<StandaloneInputModule>();
			}
			UnityEngine.Object.Instantiate(playerSpawn, position, rotation).AddComponent<PerformanceTuner>();
			Light[] array5 = UnityEngine.Object.FindObjectsOfType<Light>();
			Light[] array6 = array5;
			foreach (Light light in array6)
			{
				if (light.type == LightType.Directional)
				{
					SimpleSunController.Sun = light;
				}
			}
			if (cloudController != null)
			{
				array6 = array5;
				for (int i = 0; i < array6.Length; i++)
				{
					if (array6[i].type == LightType.Directional)
					{
						UnityEngine.Object.Instantiate(cloudController).GetComponent<CloudController>();
						break;
					}
				}
			}
			StartCoroutine(AfterSceneLoadedAndItemInitialized());
		}
		_loadingScene = null;
	}

	private IEnumerator AfterSceneLoadedAndItemInitialized()
	{
		yield return new WaitForEndOfFrame();
		CustomDestructible[] array = UnityEngine.Object.FindObjectsOfType<CustomDestructible>();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i].gameObject;
			AssetDescription assetDesc = new AssetDescription(gameObject);
			float y = gameObject.transform.lossyScale.y;
			gameObject.transform.localScale = Vector3.one;
			LocalClient.Instance.SpawnObject(assetDesc, gameObject.transform.position, gameObject.transform.rotation, y);
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private string SearchShaderName(string go, string material)
	{
		string key = go + material.Replace(" (Instance)", "");
		if (_shaderData.ContainsKey(key))
		{
			return _shaderData[key];
		}
		Debug.LogError("The material " + material + " of " + go + " is not on the list of scene shaders.");
		return "";
	}
}
