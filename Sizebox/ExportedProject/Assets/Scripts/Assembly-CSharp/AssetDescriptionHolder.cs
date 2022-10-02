using UnityEngine;

[CreateAssetMenu(menuName = "Debugging/New AssetDescriptionHolder")]
public class AssetDescriptionHolder : ScriptableObject
{
	[SerializeField]
	private AssetDescription asset;

	public AssetDescription Asset
	{
		get
		{
			return asset;
		}
	}
}
