using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.XR
{
	[Preserve]
	public class XRControllerWithRumble : XRController
	{
		public void SendImpulse(float amplitude, float duration)
		{
			SendHapticImpulseCommand command = SendHapticImpulseCommand.Create(0, amplitude, duration);
			ExecuteCommand(ref command);
		}
	}
}
