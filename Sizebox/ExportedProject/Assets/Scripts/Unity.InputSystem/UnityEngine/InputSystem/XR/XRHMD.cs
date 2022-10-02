using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.XR
{
	[InputControlLayout(isGenericTypeOfDevice = true, displayName = "XR HMD")]
	[Preserve]
	public class XRHMD : TrackedDevice
	{
		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control leftEyePosition { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public QuaternionControl leftEyeRotation { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control rightEyePosition { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public QuaternionControl rightEyeRotation { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public Vector3Control centerEyePosition { get; private set; }

		[InputControl(noisy = true)]
		[Preserve]
		public QuaternionControl centerEyeRotation { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			centerEyePosition = GetChildControl<Vector3Control>("centerEyePosition");
			centerEyeRotation = GetChildControl<QuaternionControl>("centerEyeRotation");
			leftEyePosition = GetChildControl<Vector3Control>("leftEyePosition");
			leftEyeRotation = GetChildControl<QuaternionControl>("leftEyeRotation");
			rightEyePosition = GetChildControl<Vector3Control>("rightEyePosition");
			rightEyeRotation = GetChildControl<QuaternionControl>("rightEyeRotation");
		}
	}
}
