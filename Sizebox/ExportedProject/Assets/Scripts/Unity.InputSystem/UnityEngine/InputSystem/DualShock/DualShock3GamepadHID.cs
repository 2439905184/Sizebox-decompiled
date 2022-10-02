using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.DualShock
{
	[InputControlLayout(stateType = typeof(DualShock3HIDInputReport), hideInUI = true)]
	[Preserve]
	public class DualShock3GamepadHID : DualShockGamepad
	{
		public ButtonControl leftTriggerButton { get; private set; }

		public ButtonControl rightTriggerButton { get; private set; }

		public ButtonControl playStationButton { get; private set; }

		protected override void FinishSetup()
		{
			leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
			rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");
			playStationButton = GetChildControl<ButtonControl>("systemButton");
			base.FinishSetup();
		}
	}
}
