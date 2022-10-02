using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Windows MR Controller (OpenVR)", commonUsages = new string[] { "LeftHand", "RightHand" })]
	[Preserve]
	public class OpenVRControllerWMR : XRController
	{
		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceVelocity { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceAngularVelocity { get; private set; }

		[InputControl(aliases = new string[] { "primary2DAxisClick", "joystickOrPadPressed" })]
		[Preserve]
		public ButtonControl touchpadClick { get; private set; }

		[InputControl(aliases = new string[] { "primary2DAxisTouch", "joystickOrPadTouched" })]
		[Preserve]
		public ButtonControl touchpadTouch { get; private set; }

		[InputControl]
		[Preserve]
		public ButtonControl gripPressed { get; private set; }

		[InputControl]
		public ButtonControl triggerPressed { get; private set; }

		[InputControl(aliases = new string[] { "primary" })]
		[Preserve]
		public ButtonControl menu { get; private set; }

		[InputControl]
		[Preserve]
		public AxisControl trigger { get; private set; }

		[InputControl]
		[Preserve]
		public AxisControl grip { get; private set; }

		[InputControl(aliases = new string[] { "secondary2DAxis" })]
		[Preserve]
		public Vector2Control touchpad { get; private set; }

		[InputControl(aliases = new string[] { "primary2DAxis" })]
		[Preserve]
		public Vector2Control joystick { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			touchpadClick = GetChildControl<ButtonControl>("touchpadClick");
			touchpadTouch = GetChildControl<ButtonControl>("touchpadTouch");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			menu = GetChildControl<ButtonControl>("menu");
			trigger = GetChildControl<AxisControl>("trigger");
			grip = GetChildControl<AxisControl>("grip");
			touchpad = GetChildControl<Vector2Control>("touchpad");
			joystick = GetChildControl<Vector2Control>("joystick");
		}
	}
}
