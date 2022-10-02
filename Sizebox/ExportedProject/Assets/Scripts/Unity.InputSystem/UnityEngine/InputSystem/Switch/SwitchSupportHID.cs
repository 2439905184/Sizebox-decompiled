using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.Switch
{
	internal static class SwitchSupportHID
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<SwitchProControllerHID>(null, default(InputDeviceMatcher).WithInterface("HID").WithCapability("vendorId", 1406).WithCapability("productId", 8201));
		}
	}
}
