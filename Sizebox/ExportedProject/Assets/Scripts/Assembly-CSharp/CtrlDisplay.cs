using UnityEngine;
using UnityEngine.UI;

public class CtrlDisplay : MonoBehaviour
{
	[Header("Required References")]
	[SerializeField]
	private Text hoverText;

	[SerializeField]
	private Text assetText;

	[SerializeField]
	private Text typeText;

	private void Awake()
	{
		SetHoverDisplay(null);
		SetClipboardDisplay(null, EntityType.NONE);
	}

	public void SetHoverDisplay(EntityBase entity)
	{
		if ((bool)entity && entity.asset != null)
		{
			hoverText.text = entity.asset.AssetName;
		}
		else if ((bool)entity)
		{
			hoverText.text = entity.name;
		}
		else
		{
			hoverText.text = "";
		}
	}

	public void SetClipboardDisplay(AssetDescription asset, EntityType type)
	{
		if (asset != null)
		{
			assetText.text = asset.AssetName;
			typeText.text = GetTypeText(type);
		}
		else
		{
			assetText.text = "";
			typeText.text = "";
		}
	}

	private string GetTypeText(EntityType type)
	{
		switch (type)
		{
		case EntityType.MICRO:
			return "Micro";
		case EntityType.MACRO:
			return "Macro";
		case EntityType.OBJECT:
			return "Object";
		case EntityType.VEHICLE:
			return "Vehicle";
		default:
			return "";
		}
	}
}
