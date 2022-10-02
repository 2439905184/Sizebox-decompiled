using System;

namespace UnityEngine.InputSystem.LowLevel
{
	[Flags]
	internal enum TouchFlags : byte
	{
		IndirectTouch = 1,
		PrimaryTouch = 0x10,
		Tap = 0x20,
		OrphanedPrimaryTouch = 0x40
	}
}
