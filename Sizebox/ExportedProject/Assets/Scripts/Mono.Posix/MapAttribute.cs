using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Delegate)]
internal class MapAttribute : Attribute
{
	private string nativeType;

	private string suppressFlags;

	public string NativeType
	{
		get
		{
			return nativeType;
		}
	}

	public string SuppressFlags
	{
		get
		{
			return suppressFlags;
		}
		set
		{
			suppressFlags = value;
		}
	}

	public MapAttribute()
	{
	}

	public MapAttribute(string nativeType)
	{
		this.nativeType = nativeType;
	}
}
