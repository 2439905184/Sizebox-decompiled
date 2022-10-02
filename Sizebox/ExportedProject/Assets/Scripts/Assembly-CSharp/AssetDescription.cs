using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AssetDescription
{
	[Header("Options")]
	[SerializeField]
	private AssetType assetType;

	[SerializeField]
	private string assetName;

	[SerializeField]
	private string bundleName;

	[SerializeField]
	private string bundlePath;

	[Space]
	[SerializeField]
	private GameObject asset;

	[SerializeField]
	private Sprite sprite;

	public AssetType AssetType
	{
		get
		{
			return assetType;
		}
	}

	public string AssetFullPath
	{
		get
		{
			return bundlePath + assetName;
		}
	}

	public string AssetFullName
	{
		get
		{
			return Path.Combine(bundleName, assetName);
		}
	}

	public string AssetName
	{
		get
		{
			return assetName;
		}
	}

	public string BundlePath
	{
		get
		{
			return bundlePath;
		}
	}

	public bool IsLoaded
	{
		get
		{
			return asset;
		}
	}

	public GameObject Asset
	{
		get
		{
			return asset;
		}
	}

	public Sprite Sprite
	{
		get
		{
			return sprite;
		}
	}

	public ModelData ModelData { get; private set; }

	public AssetDescription()
	{
	}

	public AssetDescription(AssetBundle bundle, string bundlePath, string assetName)
	{
		this.assetName = assetName;
		this.bundlePath = bundlePath;
		string[] array = bundlePath.Split(Path.DirectorySeparatorChar);
		bundleName = array[array.Length - 1];
		PrepareSprite(bundle, bundlePath, assetName);
		PrepareHash(bundle, bundlePath, assetName);
		assetType = AssetType.External;
	}

	public AssetDescription(GameObject go)
	{
		assetName = go.name;
		asset = go;
		bundleName = "DynamicFromScene";
		assetType = AssetType.Internal;
	}

	public AssetDescription(AssetDescription other)
	{
		assetType = other.assetType;
		assetName = other.assetName;
		bundleName = other.bundleName;
		bundlePath = other.bundlePath;
		asset = other.asset;
		sprite = other.sprite;
		ModelData = other.ModelData;
	}

	private void PrepareSprite(AssetBundle bundle, string bundlePath, string assetName)
	{
		Texture2D texture2D = bundle.LoadAsset(assetName + "Thumb") as Texture2D;
		if ((bool)texture2D)
		{
			sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
		}
		else
		{
			Debug.LogError("Error loading the sprite of the model: " + assetName);
		}
	}

	private void PrepareHash(AssetBundle bundle, string bundlePath, string assetName)
	{
	}

	public void _AssignLoadedData(GameObject asset, ModelData data)
	{
		this.asset = asset;
		ModelData = data;
		this.asset.name = AssetFullName;
	}

	public static List<AssetDescription> CreateAssetDescriptions(string assetBundlePath)
	{
		List<AssetDescription> list = new List<AssetDescription>();
		AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
		if ((bool)assetBundle)
		{
			try
			{
				StringHolder stringHolder = assetBundle.LoadAsset("modellist") as StringHolder;
				if (stringHolder != null)
				{
					string[] content = stringHolder.content;
					foreach (string text in content)
					{
						list.Add(new AssetDescription(assetBundle, assetBundlePath, text));
					}
				}
				assetBundle.Unload(false);
				return list;
			}
			catch (Exception message)
			{
				Debug.LogError("Error loading model in: " + assetBundlePath);
				Debug.LogError(message);
				return list;
			}
		}
		return list;
	}
}
