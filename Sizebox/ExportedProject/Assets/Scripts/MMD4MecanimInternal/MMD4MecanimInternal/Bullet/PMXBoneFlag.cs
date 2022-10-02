using System;

namespace MMD4MecanimInternal.Bullet
{
	[Flags]
	public enum PMXBoneFlag
	{
		None = 0,
		Destination = 1,
		Rotate = 2,
		Translate = 4,
		Visible = 8,
		Controllable = 0x10,
		IK = 0x20,
		IKChild = 0x40,
		InherenceLocal = 0x80,
		InherenceRotation = 0x100,
		InherencePosition = 0x200,
		FixedAxis = 0x400,
		LocalAxis = 0x800,
		TransformAfterPhysics = 0x1000,
		TransformExternalParent = 0x2000
	}
}
