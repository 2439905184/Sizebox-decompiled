using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Unity.XR.Oculus.Input
{
	[InputControlLayout(displayName = "Oculus Headset (w/ on-headset controls)")]
	[Preserve]
	public class OculusHMDExtended : OculusHMD
	{
		[Preserve]
		[InputControl]
		public ButtonControl back { get; private set; }

		[Preserve]
		[InputControl]
		public Vector2Control touchpad { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			back = GetChildControl<ButtonControl>("back");
			touchpad = GetChildControl<Vector2Control>("touchpad");
		}
	}
}
