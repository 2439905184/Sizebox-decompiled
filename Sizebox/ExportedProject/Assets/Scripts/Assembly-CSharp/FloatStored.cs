using System;
using UnityEngine;

public class FloatStored : Stored
{
	private readonly float _defaultValue;

	public float value
	{
		get
		{
			return PlayerPrefs.GetFloat(Key, _defaultValue);
		}
		set
		{
			if (Math.Abs(_defaultValue - value) > float.Epsilon)
			{
				PlayerPrefs.SetFloat(Key, value);
			}
			else
			{
				Reset();
			}
		}
	}

	public static implicit operator float(FloatStored floatStored)
	{
		return floatStored.value;
	}

	public FloatStored(string key, float defaultValue)
	{
		Key = key;
		_defaultValue = defaultValue;
	}
}
