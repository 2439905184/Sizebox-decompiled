using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Unity.XR.Oculus.Input
{
	[Preserve]
	[InputControlLayout(displayName = "Oculus Touch Controller", commonUsages = new string[] { "LeftHand", "RightHand" })]
	public class OculusTouchController : XRControllerWithRumble
	{
		[Preserve]
		[InputControl(aliases = new string[] { "Primary2DAxis", "Joystick" })]
		public Vector2Control thumbstick { get; private set; }

		[Preserve]
		[InputControl]
		public AxisControl trigger { get; private set; }

		[Preserve]
		[InputControl]
		public AxisControl grip { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "A", "X", "Alternate" })]
		public ButtonControl primaryButton { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "B", "Y", "Primary" })]
		public ButtonControl secondaryButton { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "GripButton" })]
		public ButtonControl gripPressed { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl start { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "JoystickOrPadPressed", "thumbstickClick" })]
		public ButtonControl thumbstickClicked { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "ATouched", "XTouched", "ATouch", "XTouch" })]
		public ButtonControl primaryTouched { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "BTouched", "YTouched", "BTouch", "YTouch" })]
		public ButtonControl secondaryTouched { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "indexTouch", "indexNearTouched" })]
		public AxisControl triggerTouched { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "indexButton", "indexTouched" })]
		public ButtonControl triggerPressed { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "JoystickOrPadTouched", "thumbstickTouch" })]
		[InputControl(name = "trackingState", layout = "Integer", aliases = new string[] { "controllerTrackingState" })]
		[InputControl(name = "isTracked", layout = "Button", aliases = new string[] { "ControllerIsTracked" })]
		[InputControl(name = "devicePosition", layout = "Vector3", aliases = new string[] { "controllerPosition" })]
		[InputControl(name = "deviceRotation", layout = "Quaternion", aliases = new string[] { "controllerRotation" })]
		public ButtonControl thumbstickTouched { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "controllerVelocity" })]
		public Vector3Control deviceVelocity { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "controllerAngularVelocity" })]
		public Vector3Control deviceAngularVelocity { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "controllerAcceleration" })]
		public Vector3Control deviceAcceleration { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "controllerAngularAcceleration" })]
		public Vector3Control deviceAngularAcceleration { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			thumbstick = GetChildControl<Vector2Control>("thumbstick");
			trigger = GetChildControl<AxisControl>("trigger");
			triggerTouched = GetChildControl<AxisControl>("triggerTouched");
			grip = GetChildControl<AxisControl>("grip");
			primaryButton = GetChildControl<ButtonControl>("primaryButton");
			secondaryButton = GetChildControl<ButtonControl>("secondaryButton");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			start = GetChildControl<ButtonControl>("start");
			thumbstickClicked = GetChildControl<ButtonControl>("thumbstickClicked");
			primaryTouched = GetChildControl<ButtonControl>("primaryTouched");
			secondaryTouched = GetChildControl<ButtonControl>("secondaryTouched");
			thumbstickTouched = GetChildControl<ButtonControl>("thumbstickTouched");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			deviceAcceleration = GetChildControl<Vector3Control>("deviceAcceleration");
			deviceAngularAcceleration = GetChildControl<Vector3Control>("deviceAngularAcceleration");
		}
	}
}
