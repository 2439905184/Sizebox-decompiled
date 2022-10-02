using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Unity.XR.Oculus.Input
{
	[Preserve]
	public class OculusTrackingReference : TrackedDevice
	{
		[Preserve]
		[InputControl(aliases = new string[] { "trackingReferenceTrackingState" })]
		public new IntegerControl trackingState { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "trackingReferenceIsTracked" })]
		public new ButtonControl isTracked { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			trackingState = GetChildControl<IntegerControl>("trackingState");
			isTracked = GetChildControl<ButtonControl>("isTracked");
		}
	}
}
