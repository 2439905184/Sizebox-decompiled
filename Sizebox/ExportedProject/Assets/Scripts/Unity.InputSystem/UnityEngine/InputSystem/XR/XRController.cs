using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[InputControlLayout(commonUsages = new string[] { "LeftHand", "RightHand" }, isGenericTypeOfDevice = true, displayName = "XR Controller")]
	[Preserve]
	public class XRController : TrackedDevice
	{
		public static XRController leftHand
		{
			get
			{
				return InputSystem.GetDevice<XRController>(CommonUsages.LeftHand);
			}
		}

		public static XRController rightHand
		{
			get
			{
				return InputSystem.GetDevice<XRController>(CommonUsages.RightHand);
			}
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			XRDeviceDescriptor xRDeviceDescriptor = XRDeviceDescriptor.FromJson(base.description.capabilities);
			if (xRDeviceDescriptor != null)
			{
				if ((xRDeviceDescriptor.characteristics & InputDeviceCharacteristics.Left) != 0)
				{
					InputSystem.SetDeviceUsage(this, CommonUsages.LeftHand);
				}
				else if ((xRDeviceDescriptor.characteristics & InputDeviceCharacteristics.Right) != 0)
				{
					InputSystem.SetDeviceUsage(this, CommonUsages.RightHand);
				}
			}
		}
	}
}
