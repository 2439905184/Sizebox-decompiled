using UnityEngine;

public class BoolStored : Stored
{
	private readonly bool _defaultValue;

	public bool value
	{
		get
		{
			return IntToBool(PlayerPrefs.GetInt(Key, _defaultValue ? 1 : 0));
		}
		set
		{
			if (_defaultValue != value)
			{
				PlayerPrefs.SetInt(Key, BoolToInt(value));
			}
			else
			{
				Reset();
			}
		}
	}

	public static implicit operator bool(BoolStored boolStored)
	{
		return boolStored.value;
	}

	public BoolStored(string key, bool defaultValue)
	{
		Key = key;
		_defaultValue = defaultValue;
	}

	private bool IntToBool(int integer)
	{
		return integer != 0;
	}

	private int BoolToInt(bool boolean)
	{
		int result = 0;
		if (boolean)
		{
			result = 1;
		}
		return result;
	}
}
