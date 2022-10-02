using UnityEngine;

public static class MapSettingInternal
{
	private static MapSettings _instance;

	private static MapSettings Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = Object.FindObjectOfType<MapSettings>();
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = new GameObject("Map Settings [Default]").AddComponent<MapSettings>();
			return _instance;
		}
	}

	public static float maxGtsSize
	{
		get
		{
			return Instance.maxGtsSize;
		}
		set
		{
			Instance.maxGtsSize = value;
		}
	}

	public static float minGtsSize
	{
		get
		{
			return Instance.minGtsSize;
		}
		set
		{
			Instance.minGtsSize = value;
		}
	}

	public static float startingSize
	{
		get
		{
			return Instance.startingSize;
		}
		set
		{
			Instance.startingSize = value;
		}
	}

	public static bool enableFog
	{
		get
		{
			return Instance.enableFog;
		}
	}

	public static float gtsStartingScale
	{
		get
		{
			return Instance.gtsStartingScale;
		}
	}

	public static float maxPlayerSize
	{
		get
		{
			return Instance.maxPlayerSize;
		}
	}

	public static float minPlayerSize
	{
		get
		{
			return Instance.minPlayerSize;
		}
	}

	public static float scale
	{
		get
		{
			return Instance.scale;
		}
	}
}
