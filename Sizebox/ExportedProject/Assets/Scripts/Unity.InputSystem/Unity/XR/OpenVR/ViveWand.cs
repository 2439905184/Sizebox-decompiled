using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Vive Wand", commonUsages = new string[] { "LeftHand", "RightHand" })]
	[Preserve]
	public class ViveWand : XRControllerWithRumble
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

		[InputControl(aliases = new string[] { "primary2DAxisClick", "joystickOrPadPressed" })]
		[Preserve]
		public ButtonControl trackpadPressed { get; private set; }

		[InputControl(aliases = new string[] { "primary2DAxisTouch", "joystickOrPadTouched" })]
		[Preserve]
		public ButtonControl trackpadTouched { get; private set; }

		[InputControl(aliases = new string[] { "Primary2DAxis" })]
		[Preserve]
		public Vector2Control trackpad { get; private set; }

		[InputControl]
		[Preserve]
		public AxisControl trigger { get; private set; }

		[InputControl]
		[Preserve]
		public ButtonControl triggerPressed { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceVelocity { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceAngularVelocity { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			grip = GetChildControl<AxisControl>("grip");
			primary = GetChildControl<ButtonControl>("primary");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			trackpadPressed = GetChildControl<ButtonControl>("trackpadPressed");
			trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");
			trackpad = GetChildControl<Vector2Control>("trackpad");
			trigger = GetChildControl<AxisControl>("trigger");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
		}
	}
}
