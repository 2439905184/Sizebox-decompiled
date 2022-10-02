using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetManager
{
	private static AssetManager _instance;

	private List<AssetDescription> giantessAssets;

	private List<AssetDescription> maleMicroAssets;

	private List<AssetDescription> femaleMicroAssets;

	private List<AssetDescription> objectAssets;

	private List<string> gtsAssetNames = new List<string>();

	private List<string> maleMicroAssetNames = new List<string>();

	private List<string> femaleMicroAssetNames = new List<string>();

	private List<string> objectAssetNames = new List<string>();

	private Dictionary<string, AssetDescription> nameToAssetMap;

	private static string[] ASSET_EXTENSIONS = new string[3] { ".gts", ".micro", ".object" };

	public static AssetManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AssetManager();
			}
			return _instance;
		}
	}

	private AssetManager()
	{
		Initialize();
	}

	private void Initialize()
	{
		IOManager instance = IOManager.Instance;
		AssetSet assetSet = Resources.Load<AssetSet>("DefaultAssets");
		giantessAssets = new List<AssetDescription>();
		maleMicroAssets = new List<AssetDescription>();
		femaleMicroAssets = new List<AssetDescription>();
		objectAssets = new List<AssetDescription>();
		giantessAssets.AddRange(assetSet.GiantessAssets);
		maleMicroAssets.AddRange(assetSet.MaleMicroAssets);
		femaleMicroAssets.AddRange(assetSet.FemaleMicroAssets);
		objectAssets.AddRange(assetSet.ObjectAssets);
		giantessAssets.AddRange(GetAssetDescriptions(instance.GtsAssetFolder));
		maleMicroAssets.AddRange(GetAssetDescriptions(instance.MaleMicroAssetFolder));
		femaleMicroAssets.AddRange(GetAssetDescriptions(instance.FemaleMicroAssetFolder));
		objectAssets.AddRange(GetAssetDescriptions(instance.ObjectAssetFolder));
		PrepareAssetDescriptionMaps();
		PrepareAssetNames();
		FindAnimations(instance);
	}

	private void PrepareAssetDescriptionMaps()
	{
		nameToAssetMap = new Dictionary<string, AssetDescription>();
		List<AssetDescription>[] array = new List<AssetDescription>[4] { giantessAssets, femaleMicroAssets, maleMicroAssets, objectAssets };
		for (int i = 0; i < array.Length; i++)
		{
			foreach (AssetDescription item in array[i])
			{
				if (!nameToAssetMap.ContainsKey(item.AssetFullName))
				{
					nameToAssetMap.Add(item.AssetFullName, item);
				}
			}
		}
	}

	private void PrepareAssetNames()
	{
		List<string>[] array = new List<string>[4] { gtsAssetNames, maleMicroAssetNames, femaleMicroAssetNames, objectAssetNames };
		List<AssetDescription>[] array2 = new List<AssetDescription>[4] { giantessAssets, maleMicroAssets, femaleMicroAssets, objectAssets };
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < array2[i].Count; j++)
			{
				array[i].Add(array2[i][j].AssetFullName);
			}
		}
	}

	public AssetDescription GetAssetDescriptionByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		AssetDescription value;
		if (nameToAssetMap.TryGetValue(name, out value))
		{
			return value;
		}
		return null;
	}

	private void FindAnimations(IOManager ioManager)
	{
		HashSet<string> hashSet = new HashSet<string>();
		List<AssetDescription>[] array = new List<AssetDescription>[4] { giantessAssets, femaleMicroAssets, maleMicroAssets, objectAssets };
		for (int i = 0; i < array.Length; i++)
		{
			foreach (AssetDescription item in array[i])
			{
				if (item.AssetType == AssetType.External)
				{
					string bundlePath = item.BundlePath;
					if (!hashSet.Contains(bundlePath))
					{
						hashSet.Add(bundlePath);
					}
				}
			}
		}
		foreach (string item2 in hashSet)
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(item2);
			if (!assetBundle)
			{
				continue;
			}
			RuntimeAnimatorController runtimeAnimatorController = assetBundle.LoadAsset<RuntimeAnimatorController>("animationController");
			if ((bool)runtimeAnimatorController)
			{
				Debug.Log("Animator controller found in: " + item2);
				AnimationClip[] animationClips = runtimeAnimatorController.animationClips;
				foreach (AnimationClip animationClip in animationClips)
				{
					ioManager.AnimationControllers[animationClip.name] = runtimeAnimatorController;
				}
			}
			assetBundle.Unload(false);
		}
	}

	private static List<AssetDescription> GetAssetDescriptions(IList<string> folders)
	{
		List<AssetDescription> list = new List<AssetDescription>();
		foreach (string folder in folders)
		{
			if (Directory.Exists(folder.Trim()))
			{
				list.AddRange(CreateAssetDescriptions(folder.Trim()));
			}
			else
			{
				Debug.LogError("Directory '" + folder.Trim() + "' dosen't exist");
			}
		}
		return list;
	}

	private static List<AssetDescription> CreateAssetDescriptions(string folder)
	{
		List<AssetDescription> list = new List<AssetDescription>();
		string[] directories = Directory.GetDirectories(folder);
		for (int i = 0; i < directories.Length; i++)
		{
			foreach (AssetDescription item in CreateAssetDescriptions(directories[i]))
			{
				list.Add(item);
			}
		}
		directories = Directory.GetFiles(folder);
		foreach (string text in directories)
		{
			if (HasAssetExtension(text))
			{
				list.AddRange(AssetDescription.CreateAssetDescriptions(text));
			}
		}
		return list;
	}

	private static bool HasAssetExtension(string file)
	{
		string extension = Path.GetExtension(file);
		string[] aSSET_EXTENSIONS = ASSET_EXTENSIONS;
		foreach (string value in aSSET_EXTENSIONS)
		{
			if (extension.Equals(value))
			{
				return true;
			}
		}
		return false;
	}

	public IList<AssetDescription> GetModelAssets()
	{
		List<AssetDescription> list = new List<AssetDescription>();
		list.AddRange(giantessAssets);
		list.AddRange(maleMicroAssets);
		list.AddRange(femaleMicroAssets);
		return list;
	}

	public AssetDescription GetRandomMicroAsset(MicroGender gender = MicroGender.None)
	{
		switch (gender)
		{
		case MicroGender.Female:
			if (femaleMicroAssets.Count == 0)
			{
				return null;
			}
			break;
		case MicroGender.Male:
			if (maleMicroAssets.Count == 0)
			{
				return null;
			}
			break;
		case MicroGender.None:
			if (femaleMicroAssets.Count == 0 || maleMicroAssets.Count == 0)
			{
				if (maleMicroAssets.Count == femaleMicroAssets.Count)
				{
					return null;
				}
				gender = ((femaleMicroAssets.Count == 0) ? MicroGender.Male : MicroGender.Female);
			}
			else
			{
				gender = ((Random.Range(0, 2) == 1) ? MicroGender.Male : MicroGender.Female);
			}
			break;
		}
		if (gender == MicroGender.Male)
		{
			int index = Random.Range(0, maleMicroAssets.Count - 1);
			return maleMicroAssets[index];
		}
		int index2 = Random.Range(0, femaleMicroAssets.Count - 1);
		return femaleMicroAssets[index2];
	}

	public IList<AssetDescription> GetObjectAssets()
	{
		return objectAssets.AsReadOnly();
	}

	public IList<string> GetGtsAssetNames()
	{
		return gtsAssetNames.AsReadOnly();
	}

	public IList<string> GetMaleMicroAssetNames()
	{
		return maleMicroAssetNames.AsReadOnly();
	}

	public IList<string> GetFemaleMicroAssetNames()
	{
		return femaleMicroAssetNames.AsReadOnly();
	}

	public IList<string> GetObjectAssetNames()
	{
		return objectAssetNames.AsReadOnly();
	}
}
