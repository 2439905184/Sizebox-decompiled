using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/New Asset Set")]
public class AssetSet : ScriptableObject
{
	[SerializeField]
	private List<AssetDescription> giantessAssets = new List<AssetDescription>();

	[SerializeField]
	private List<AssetDescription> maleMicroAssets = new List<AssetDescription>();

	[SerializeField]
	private List<AssetDescription> femaleMicroAssets = new List<AssetDescription>();

	[SerializeField]
	private List<AssetDescription> objectAssets = new List<AssetDescription>();

	public IList<AssetDescription> GiantessAssets
	{
		get
		{
			return giantessAssets.AsReadOnly();
		}
	}

	public IList<AssetDescription> MaleMicroAssets
	{
		get
		{
			return maleMicroAssets.AsReadOnly();
		}
	}

	public IList<AssetDescription> FemaleMicroAssets
	{
		get
		{
			return femaleMicroAssets.AsReadOnly();
		}
	}

	public IList<AssetDescription> ObjectAssets
	{
		get
		{
			return objectAssets.AsReadOnly();
		}
	}
}
