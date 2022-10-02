using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Handed Vive Tracker", commonUsages = new string[] { "LeftHand", "RightHand" })]
	[Preserve]
	public class HandedViveTracker : ViveTracker
	{
		[InputControl]
		[Preserve]
		public AxisControl grip { get; private set; }

		[InputControl]
		[Preserve]
		public ButtonControl gripPressed { get; private set; }

		[InputControl]
		[Preserve]
		public ButtonControl primary { get; private set; }

		[InputControl(aliases = new string[] { "JoystickOrPadPressed" })]
		[Preserve]
		public ButtonControl trackpadPressed { get; private set; }

		[InputControl]
		[Preserve]
		public ButtonControl triggerPressed { get; private set; }

		protected override void FinishSetup()
		{
			grip = GetChildControl<AxisControl>("grip");
			primary = GetChildControl<ButtonControl>("primary");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			trackpadPressed = GetChildControl<ButtonControl>("trackpadPressed");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			base.FinishSetup();
		}
	}
}
