using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Unity.XR.Oculus.Input
{
	[Preserve]
	[InputControlLayout(displayName = "GearVR Controller", commonUsages = new string[] { "LeftHand", "RightHand" })]
	public class GearVRTrackedController : XRController
	{
		[Preserve]
		[InputControl]
		public Vector2Control touchpad { get; private set; }

		[Preserve]
		[InputControl]
		public AxisControl trigger { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl back { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl triggerPressed { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl touchpadClicked { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl touchpadTouched { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control deviceAcceleration { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control deviceAngularAcceleration { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			touchpad = GetChildControl<Vector2Control>("touchpad");
			trigger = GetChildControl<AxisControl>("trigger");
			back = GetChildControl<ButtonControl>("back");
			triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
			touchpadClicked = GetChildControl<ButtonControl>("touchpadClicked");
			touchpadTouched = GetChildControl<ButtonControl>("touchpadTouched");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
			deviceAcceleration = GetChildControl<Vector3Control>("deviceAcceleration");
			deviceAngularAcceleration = GetChildControl<Vector3Control>("deviceAngularAcceleration");
		}
	}
}
