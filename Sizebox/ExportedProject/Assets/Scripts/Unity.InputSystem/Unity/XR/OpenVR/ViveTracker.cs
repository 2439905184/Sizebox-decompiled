using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Vive Tracker")]
	[Preserve]
	public class ViveTracker : TrackedDevice
	{
		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceVelocity { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control deviceAngularVelocity { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			deviceAngularVelocity = GetChildControl<Vector3Control>("deviceAngularVelocity");
		}
	}
}
