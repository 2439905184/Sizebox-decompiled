using System;
using UnityEngine;

[Serializable]
public class EntityValue
{
	[SerializeField]
	private float value = 25f;

	[SerializeField]
	private float maxValue = 25f;

	[SerializeField]
	private float minValue;

	public float Value
	{
		get
		{
			return value;
		}
	}

	public float MaxValue
	{
		get
		{
			return maxValue;
		}
	}

	public float MinValue
	{
		get
		{
			return minValue;
		}
	}

	public bool IsAtMax
	{
		get
		{
			return value >= maxValue;
		}
	}

	public bool IsAtMin
	{
		get
		{
			return value <= minValue;
		}
	}

	public void Increase(float amount)
	{
		value = Mathf.Clamp(value + amount, value, maxValue);
	}

	public void Decrease(float amount)
	{
		value = Mathf.Clamp(value - amount, minValue, value);
	}
}
