using UnityEngine;

public class StringStored : Stored
{
	private readonly string _defaultValue;

	public string value
	{
		get
		{
			return PlayerPrefs.GetString(Key, _defaultValue);
		}
		set
		{
			if (_defaultValue != value)
			{
				PlayerPrefs.SetString(Key, value);
			}
			else
			{
				Reset();
			}
		}
	}

	public StringStored(string key, string defaultValue)
	{
		Key = key;
		_defaultValue = defaultValue;
	}
}
