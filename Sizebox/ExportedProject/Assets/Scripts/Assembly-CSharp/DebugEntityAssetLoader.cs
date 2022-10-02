using UnityEngine;

[RequireComponent(typeof(EntityBase))]
public class DebugEntityAssetLoader : MonoBehaviour
{
	[SerializeField]
	private AssetDescriptionHolder assetHolder;

	private AssetDescriptionHolder previous;

	private void Update()
	{
		if (previous != assetHolder)
		{
			GetComponent<EntityBase>().RegisterModelAsset(assetHolder.Asset);
		}
		previous = assetHolder;
	}
}
