using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XInput.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.XInput
{
	[InputControlLayout(stateType = typeof(XInputControllerWindowsState), hideInUI = true)]
	[Preserve]
	public class XInputControllerWindows : XInputController
	{
	}
}
