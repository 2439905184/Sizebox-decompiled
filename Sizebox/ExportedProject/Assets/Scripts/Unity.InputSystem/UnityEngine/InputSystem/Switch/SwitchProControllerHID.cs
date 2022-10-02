using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Switch
{
	[InputControlLayout(stateType = typeof(SwitchProControllerHIDInputState), displayName = "Switch Pro Controller")]
	[Preserve]
	public class SwitchProControllerHID : Gamepad
	{
	}
}
