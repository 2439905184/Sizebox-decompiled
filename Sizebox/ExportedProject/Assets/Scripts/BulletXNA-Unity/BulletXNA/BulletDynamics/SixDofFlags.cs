using System;

namespace BulletXNA.BulletDynamics
{
	[Flags]
	public enum SixDofFlags
	{
		BT_6DOF_FLAGS_CFM_NORM = 1,
		BT_6DOF_FLAGS_CFM_STOP = 2,
		BT_6DOF_FLAGS_ERP_STOP = 4
	}
}
