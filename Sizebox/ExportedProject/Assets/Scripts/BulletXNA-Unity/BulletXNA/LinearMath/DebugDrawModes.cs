using System;

namespace BulletXNA.LinearMath
{
	[Flags]
	public enum DebugDrawModes
	{
		DBG_NoDebug = 0,
		DBG_DrawWireframe = 1,
		DBG_DrawAabb = 2,
		DBG_DrawFeaturesText = 4,
		DBG_DrawContactPoints = 8,
		DBG_NoDeactivation = 0x10,
		DBG_NoHelpText = 0x20,
		DBG_DrawText = 0x40,
		DBG_ProfileTimings = 0x80,
		DBG_EnableSatComparison = 0x100,
		DBG_DisableBulletLCP = 0x200,
		DBG_EnableCCD = 0x400,
		DBG_DrawConstraints = 0x800,
		DBG_DrawConstraintLimits = 0x1000,
		DBG_DrawFastWireframe = 0x2000,
		DBG_DrawNormals = 0x4000,
		ALL = 0x584F,
		DBG_MAX_DEBUG_DRAW_MODE = 0x5850
	}
}
