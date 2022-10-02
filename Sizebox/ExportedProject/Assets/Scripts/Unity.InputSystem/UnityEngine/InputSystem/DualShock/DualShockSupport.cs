using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.DualShock
{
	internal static class DualShockSupport
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<DualShockGamepad>();
			InputSystem.RegisterLayout<DualShock4GamepadHID>(null, default(InputDeviceMatcher).WithInterface("HID").WithCapability("vendorId", 1356).WithCapability("productId", 2508));
			InputSystem.RegisterLayoutMatcher<DualShock4GamepadHID>(default(InputDeviceMatcher).WithInterface("HID").WithManufacturer("Sony.+Entertainment").WithProduct("Wireless Controller"));
			InputSystem.RegisterLayout<DualShock3GamepadHID>(null, default(InputDeviceMatcher).WithInterface("HID").WithCapability("vendorId", 1356).WithCapability("productId", 616));
			InputSystem.RegisterLayoutMatcher<DualShock3GamepadHID>(default(InputDeviceMatcher).WithInterface("HID").WithManufacturer("Sony.+Entertainment").WithProduct("PLAYSTATION(R)3 Controller"));
		}
	}
}
