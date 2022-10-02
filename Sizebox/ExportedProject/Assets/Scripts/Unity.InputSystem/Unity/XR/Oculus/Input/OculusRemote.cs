using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Unity.XR.Oculus.Input
{
	[InputControlLayout(displayName = "Oculus Remote")]
	[Preserve]
	public class OculusRemote : InputDevice
	{
		[Preserve]
		[InputControl]
		public ButtonControl back { get; private set; }

		[Preserve]
		[InputControl]
		public ButtonControl start { get; private set; }

		[Preserve]
		[InputControl]
		public Vector2Control touchpad { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			back = GetChildControl<ButtonControl>("back");
			start = GetChildControl<ButtonControl>("start");
			touchpad = GetChildControl<Vector2Control>("touchpad");
		}
	}
}
