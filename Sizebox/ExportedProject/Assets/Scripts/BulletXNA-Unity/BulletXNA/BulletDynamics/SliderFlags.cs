namespace BulletXNA.BulletDynamics
{
	public enum SliderFlags
	{
		BT_SLIDER_FLAGS_CFM_DIRLIN = 1,
		BT_SLIDER_FLAGS_ERP_DIRLIN = 2,
		BT_SLIDER_FLAGS_CFM_DIRANG = 4,
		BT_SLIDER_FLAGS_ERP_DIRANG = 8,
		BT_SLIDER_FLAGS_CFM_ORTLIN = 0x10,
		BT_SLIDER_FLAGS_ERP_ORTLIN = 0x20,
		BT_SLIDER_FLAGS_CFM_ORTANG = 0x40,
		BT_SLIDER_FLAGS_ERP_ORTANG = 0x80,
		BT_SLIDER_FLAGS_CFM_LIMLIN = 0x100,
		BT_SLIDER_FLAGS_ERP_LIMLIN = 0x200,
		BT_SLIDER_FLAGS_CFM_LIMANG = 0x400,
		BT_SLIDER_FLAGS_ERP_LIMANG = 0x800
	}
}