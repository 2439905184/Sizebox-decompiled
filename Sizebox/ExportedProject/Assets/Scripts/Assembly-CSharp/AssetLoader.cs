using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public static class AssetLoader
{
	private class AssetBundleManager
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public AssetBundleManager _003C_003E4__this;

			public AssetDescription asset;

			internal void _003CRegister_003Eb__0(AsyncOperation async)
			{
				_003C_003E4__this.CompleteBundleLoad(async, asset);
			}
		}

		private Dictionary<AssetDescription, List<UnityAction>> assetToCallbackMap = new Dictionary<AssetDescription, List<UnityAction>>();

		private Dictionary<string, HashSet<AssetDescription>> waitingOnBundleLoadMap = new Dictionary<string, HashSet<AssetDescription>>();

		private Dictionary<string, HashSet<AssetDescription>> bundlesInUseMap = new Dictionary<string, HashSet<AssetDescription>>();

		private Dictionary<string, AssetBundle> pathToBundleMap = new Dictionary<string, AssetBundle>();

		public bool Contains(AssetDescription asset)
		{
			return assetToCallbackMap.ContainsKey(asset);
		}

		public void Register(AssetDescription asset, UnityAction callback)
		{
			_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
			_003C_003Ec__DisplayClass5_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass5_.asset = asset;
			if (Contains(_003C_003Ec__DisplayClass5_.asset))
			{
				List<UnityAction> value;
				assetToCallbackMap.TryGetValue(_003C_003Ec__DisplayClass5_.asset, out value);
				value.Add(callback);
				return;
			}
			assetToCallbackMap.Add(_003C_003Ec__DisplayClass5_.asset, new List<UnityAction> { callback });
			string bundlePath = _003C_003Ec__DisplayClass5_.asset.BundlePath;
			if (!bundlesInUseMap.ContainsKey(_003C_003Ec__DisplayClass5_.asset.BundlePath))
			{
				if (waitingOnBundleLoadMap.ContainsKey(bundlePath))
				{
					HashSet<AssetDescription> value2;
					waitingOnBundleLoadMap.TryGetValue(bundlePath, out value2);
					value2.Add(_003C_003Ec__DisplayClass5_.asset);
				}
				else
				{
					waitingOnBundleLoadMap.Add(bundlePath, new HashSet<AssetDescription> { _003C_003Ec__DisplayClass5_.asset });
					AssetBundle.LoadFromFileAsync(bundlePath).completed += _003C_003Ec__DisplayClass5_._003CRegister_003Eb__0;
				}
			}
			else
			{
				AssetBundle value3;
				pathToBundleMap.TryGetValue(bundlePath, out value3);
				if ((bool)value3)
				{
					HashSet<AssetDescription> value4;
					bundlesInUseMap.TryGetValue(bundlePath, out value4);
					value4.Add(_003C_003Ec__DisplayClass5_.asset);
					OpenBundle(value3, _003C_003Ec__DisplayClass5_.asset);
				}
			}
		}

		private void CompleteBundleLoad(AsyncOperation async, AssetDescription asset)
		{
			AssetBundle assetBundle = ((AssetBundleCreateRequest)async).assetBundle;
			string bundlePath = asset.BundlePath;
			if (!(assetBundle != null))
			{
				return;
			}
			pathToBundleMap.Add(bundlePath, assetBundle);
			HashSet<AssetDescription> value;
			waitingOnBundleLoadMap.TryGetValue(bundlePath, out value);
			bundlesInUseMap.Add(bundlePath, value);
			waitingOnBundleLoadMap.Remove(bundlePath);
			foreach (AssetDescription item in value)
			{
				OpenBundle(assetBundle, item);
			}
		}

		public void Complete(AssetDescription asset)
		{
			if (!Contains(asset))
			{
				return;
			}
			List<UnityAction> value;
			assetToCallbackMap.TryGetValue(asset, out value);
			foreach (UnityAction item in value)
			{
				item();
			}
			assetToCallbackMap.Remove(asset);
			string bundlePath = asset.BundlePath;
			HashSet<AssetDescription> value2;
			bundlesInUseMap.TryGetValue(bundlePath, out value2);
			if (value2.Contains(asset))
			{
				value2.Remove(asset);
			}
			if (value2.Count == 0)
			{
				AssetBundle value3;
				pathToBundleMap.TryGetValue(bundlePath, out value3);
				value3.Unload(false);
				pathToBundleMap.Remove(bundlePath);
				bundlesInUseMap.Remove(bundlePath);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public AssetDescription assetDesc;

		public AssetBundle bundle;

		internal void _003COpenBundle_003Eb__0(AsyncOperation modelAsync)
		{
			LoadAsset(modelAsync, assetDesc, bundle);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public AssetDescription assetDesc;

		public AssetBundle bundle;

		internal void _003CLoadAsset_003Eb__0(AsyncOperation fallbackAsync)
		{
			LoadAssetFallback(fallbackAsync, assetDesc, bundle);
		}
	}

	private static AssetBundleManager bundleManager = new AssetBundleManager();

	public static void LoadModelAssetAsync(AssetDescription assetDescription, UnityAction completionCallback)
	{
		if (assetDescription.IsLoaded)
		{
			completionCallback();
		}
		else
		{
			bundleManager.Register(assetDescription, completionCallback);
		}
	}

	private static void OpenBundle(AssetBundle bundle, AssetDescription assetDesc)
	{
		_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = new _003C_003Ec__DisplayClass3_0();
		_003C_003Ec__DisplayClass3_.assetDesc = assetDesc;
		_003C_003Ec__DisplayClass3_.bundle = bundle;
		_003C_003Ec__DisplayClass3_.bundle.LoadAssetAsync(_003C_003Ec__DisplayClass3_.assetDesc.AssetName + "Prefab").completed += _003C_003Ec__DisplayClass3_._003COpenBundle_003Eb__0;
	}

	private static void LoadAsset(AsyncOperation op, AssetDescription assetDesc, AssetBundle bundle)
	{
		_003C_003Ec__DisplayClass4_0 _003C_003Ec__DisplayClass4_ = new _003C_003Ec__DisplayClass4_0();
		_003C_003Ec__DisplayClass4_.assetDesc = assetDesc;
		_003C_003Ec__DisplayClass4_.bundle = bundle;
		GameObject gameObject = ((AssetBundleRequest)op).asset as GameObject;
		string bundlePath = _003C_003Ec__DisplayClass4_.assetDesc.BundlePath;
		string assetName = _003C_003Ec__DisplayClass4_.assetDesc.AssetName;
		if (gameObject == null || gameObject.GetComponent<UsePrefab>() == null)
		{
			_003C_003Ec__DisplayClass4_.bundle.LoadAssetAsync(assetName).completed += _003C_003Ec__DisplayClass4_._003CLoadAsset_003Eb__0;
		}
		else
		{
			FinishLoading(gameObject, _003C_003Ec__DisplayClass4_.assetDesc, _003C_003Ec__DisplayClass4_.bundle);
		}
	}

	private static void LoadAssetFallback(AsyncOperation op, AssetDescription assetDesc, AssetBundle bundle)
	{
		FinishLoading(((AssetBundleRequest)op).asset as GameObject, assetDesc, bundle);
	}

	private static void FinishLoading(GameObject model, AssetDescription assetDesc, AssetBundle bundle)
	{
		string bundlePath = assetDesc.BundlePath;
		string assetName = assetDesc.AssetName;
		if (model == null)
		{
			Debug.LogError(assetName + " could not be loaded in " + bundlePath);
			bundle.Unload(false);
			bundleManager.Complete(assetDesc);
			return;
		}
		StringHolder stringHolder = bundle.LoadAsset(assetName + "_shaders") as StringHolder;
		if (stringHolder != null)
		{
			PrepareAssetShaders(model, stringHolder.content);
		}
		else
		{
			stringHolder = bundle.LoadAsset("shaders") as StringHolder;
			if (stringHolder != null)
			{
				PrepareAssetShaders(model, stringHolder.content);
			}
			else
			{
				ShaderLoader.ApplyShader(model);
			}
		}
		ModelData modelData = new ModelData();
		modelData.modelBytes = bundle.LoadAsset<TextAsset>(assetName + ".model.bytes");
		modelData.indexBytes = bundle.LoadAsset<TextAsset>(assetName + ".index.bytes");
		modelData.vertexBytes = bundle.LoadAsset<TextAsset>(assetName + ".vertex.bytes");
		assetDesc._AssignLoadedData(model, modelData);
		bundleManager.Complete(assetDesc);
	}

	private static void PrepareAssetShaders(GameObject model, string[] shaderList)
	{
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>();
		bool flag = false;
		int num = 0;
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Material[] sharedMaterials = array[i].sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (num >= shaderList.Length)
				{
					break;
				}
				if (!flag && shaderList[num].StartsWith("MMD"))
				{
					Debug.LogWarning("Model: '" + model.name + "' is using deprecated shader(s) '" + shaderList[num] + "' consider re-exporting this model with another shader");
					flag = true;
				}
				ShaderLoader.SetShaderOnMaterial(material, shaderList[num], model.name);
				num++;
			}
		}
	}

	public static bool LoadModelAssetSyncronous(AssetDescription assetDescription)
	{
		if (assetDescription.IsLoaded)
		{
			return true;
		}
		string bundlePath = assetDescription.BundlePath;
		string assetName = assetDescription.AssetName;
		AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
		if (assetBundle != null)
		{
			ModelData modelData = new ModelData();
			GameObject gameObject = assetBundle.LoadAsset(assetName + "Prefab") as GameObject;
			if (gameObject == null || gameObject.GetComponent<UsePrefab>() == null)
			{
				gameObject = assetBundle.LoadAsset(assetName) as GameObject;
			}
			if (gameObject == null)
			{
				Debug.LogError(assetName + " could not be loaded in " + bundlePath);
				return false;
			}
			StringHolder stringHolder = assetBundle.LoadAsset(assetName + "_shaders") as StringHolder;
			if (stringHolder != null)
			{
				PrepareAssetShaders(gameObject, stringHolder.content);
			}
			else
			{
				stringHolder = assetBundle.LoadAsset("shaders") as StringHolder;
				if (stringHolder != null)
				{
					PrepareAssetShaders(gameObject, stringHolder.content);
				}
				else
				{
					ShaderLoader.ApplyShader(gameObject);
				}
			}
			modelData.modelBytes = assetBundle.LoadAsset<TextAsset>(assetName + ".model.bytes");
			modelData.indexBytes = assetBundle.LoadAsset<TextAsset>(assetName + ".index.bytes");
			modelData.vertexBytes = assetBundle.LoadAsset<TextAsset>(assetName + ".vertex.bytes");
			assetDescription._AssignLoadedData(gameObject, modelData);
			assetBundle.Unload(false);
			return true;
		}
		return false;
	}
}
