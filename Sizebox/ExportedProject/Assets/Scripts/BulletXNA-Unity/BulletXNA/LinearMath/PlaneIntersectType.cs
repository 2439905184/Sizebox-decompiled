using System;

namespace BulletXNA.LinearMath
{
	[Flags]
	public enum PlaneIntersectType
	{
		COPLANAR = 0,
		UNDER = 1,
		OVER = 2,
		SPLIT = 3
	}
}
