using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 ToVirtual(this Vector3 vector)
	{
		return CenterOrigin.WorldToVirtual(vector);
	}

	public static Vector3 ToWorld(this Vector3 vector)
	{
		return CenterOrigin.VirtualToWorld(vector);
	}
}
