using System;

namespace BulletXNA.BulletCollision
{
	[Flags]
	public enum CollisionFilterGroups
	{
		DefaultFilter = 1,
		StaticFilter = 2,
		KinematicFilter = 4,
		DebrisFilter = 8,
		SensorTrigger = 0x10,
		CharacterFilter = 0x20,
		AllFilter = -1
	}
}
