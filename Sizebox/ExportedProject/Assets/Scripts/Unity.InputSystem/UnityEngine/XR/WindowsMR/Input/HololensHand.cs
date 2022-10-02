using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.WindowsMR.Input
{
	[Preserve]
	[InputControlLayout(displayName = "HoloLens Hand", commonUsages = new string[] { "LeftHand", "RightHand" })]
	public class HololensHand : XRController
	{
		[Preserve]
		[InputControl(noisy = true, aliases = new string[] { "gripVelocity" })]
		public Vector3Control deviceVelocity { get; private set; }

		[Preserve]
		[InputControl(aliases = new string[] { "triggerbutton" })]
		public ButtonControl airTap { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public AxisControl sourceLossRisk { get; private set; }

		[Preserve]
		[InputControl(noisy = true)]
		public Vector3Control sourceLossMitigationDirection { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			airTap = GetChildControl<ButtonControl>("airTap");
			deviceVelocity = GetChildControl<Vector3Control>("deviceVelocity");
			sourceLossRisk = GetChildControl<AxisControl>("sourceLossRisk");
			sourceLossMitigationDirection = GetChildControl<Vector3Control>("sourceLossMitigationDirection");
		}
	}
}
