using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.WindowsMR.Input
{
	[Preserve]
	[InputControlLayout(displayName = "Windows MR Headset")]
	public class WMRHMD : XRHMD
	{
		[Preserve]
		[InputControl]
		[InputControl(name = "devicePosition", layout = "Vector3", aliases = new string[] { "HeadPosition" })]
		[InputControl(name = "deviceRotation", layout = "Quaternion", aliases = new string[] { "HeadRotation" })]
		public ButtonControl userPresence { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			userPresence = GetChildControl<ButtonControl>("userPresence");
		}
	}
}
