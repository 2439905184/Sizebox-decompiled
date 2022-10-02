using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.WindowsMR.Input
{
	[Preserve]
	[InputControlLayout(displayName = "Windows MR Controller", commonUsages = new string[] { "LeftHand", "RightHand" })]
	public class WMRSpatialController : XRControllerWithRumble
	{
		[Preserve]
		[InputControl(aliases = new string[] { "Primary2DAxis", "thumbstickaxes" })]
		public Vector2Control joystick { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "Secondary2DAxis", "touchpadaxes" })]
		public Vector2Control touchpad { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "gripaxis" })]
		public AxisControl grip { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "gripbutton" })]
		public ButtonControl gripPressed { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "Primary", "menubutton" })]
		public ButtonControl menu { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "triggeraxis" })]
		public AxisControl trigger { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "triggerbutton" })]
		public ButtonControl triggerPressed { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "thumbstickpressed" })]
		public ButtonControl joystickClicked { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "joystickorpadpressed", "touchpadpressed" })]
		public ButtonControl touchpadClicked { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "joystickorpadtouched", "touchpadtouched" })]
		public ButtonControl touchpadTouched { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "gripVelocity" })]
		public Vector3Control deviceVelocity { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "gripAngularVelocity" })]
		public Vector3Control deviceAngularVelocity { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public AxisControl batteryLevel { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public AxisControl sourceLossRisk { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control sourceLossMitigationDirection { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control pointerPosition { get; private set; }

		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "PointerOrientation" })]
		public QuaternionControl pointerRotation { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			joystick = GetChildControl<Vector2Control>("joystick");
			trigger = GetChildControl<AxisControl>("trigger");
			touchpad = GetChildControl<Vector2Control>("touchpad");
			grip = GetChildControl<AxisControl>("grip");
			gripPressed = GetChildControl<ButtonControl>("gripPressed");
			menu = GetChildControl<ButtonControl>("menu");
			joystickClicked = GetChildControl<ButtonControl>("joystickClicked");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			touchpadClicked = GetChildControl<ButtonControl>("touchpadClicked");
			touchpadTouched = GetChildControl<ButtonControl>("touchPadTouched");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			batteryLevel = GetChildControl<AxisControl>("batteryLevel");
			sourceLossRisk = GetChildControl<AxisControl>("sourceLossRisk");
			sourceLossMitigationDirection = GetChildControl<Vector3Control>("sourceLossMitigationDirection");
			pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
			pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");
		}
	}
}
