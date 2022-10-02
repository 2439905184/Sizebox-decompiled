using System;

namespace BulletXNA.BulletCollision
{
	[Flags]
	public enum EFlags
	{
		kF_None = 0,
		kF_FilterBackfaces = 1,
		kF_KeepUnflippedNormal = 2,
		kF_Terminator = 0xFFFFFFF
	}
}
