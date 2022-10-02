using UnityEngine;

public class IntStored : Stored
{
	private readonly int _defaultValue;

	public int value
	{
		get
		{
			return PlayerPrefs.GetInt(Key, _defaultValue);
		}
		set
		{
			if (_defaultValue != value)
			{
				PlayerPrefs.SetInt(Key, value);
			}
			else
			{
				Reset();
			}
		}
	}

	public static implicit operator int(IntStored intStored)
	{
		return intStored.value;
	}

	public IntStored(string key, int defaultValue)
	{
		_defaultValue = defaultValue;
		Key = key;
	}
}
