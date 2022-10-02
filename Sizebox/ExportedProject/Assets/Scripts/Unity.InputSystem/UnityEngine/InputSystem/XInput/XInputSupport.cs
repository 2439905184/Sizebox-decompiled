using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XInput
{
	internal static class XInputSupport
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<XInputController>();
			InputSystem.RegisterLayout<XInputControllerWindows>(null, default(InputDeviceMatcher).WithInterface("XInput"));
		}
	}
}
