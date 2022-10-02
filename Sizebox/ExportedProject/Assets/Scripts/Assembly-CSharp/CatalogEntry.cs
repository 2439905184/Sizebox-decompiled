using UnityEngine;
using UnityEngine.UI;

public class CatalogEntry : MonoBehaviour
{
	[Header("Colors")]
	[SerializeField]
	private Color activeColor = Color.white;

	[SerializeField]
	private Color inactiveColor = Color.gray;

	[SerializeField]
	private Button button;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Text nameText;

	public AssetDescription Asset { get; private set; }

	public Button Button
	{
		get
		{
			return button;
		}
	}

	public Image Image
	{
		get
		{
			return image;
		}
	}

	public bool Active
	{
		get
		{
			return image.sprite != null;
		}
	}

	public string Name
	{
		get
		{
			if ((bool)image.sprite)
			{
				return image.sprite.name;
			}
			return "";
		}
	}

	public void SetAsset(AssetDescription asset)
	{
		Asset = asset;
		if (asset != null)
		{
			image.sprite = asset.Sprite;
			image.color = activeColor;
			nameText.text = asset.AssetName;
		}
		else
		{
			image.sprite = null;
			image.color = inactiveColor;
			nameText.text = "";
		}
	}
}
