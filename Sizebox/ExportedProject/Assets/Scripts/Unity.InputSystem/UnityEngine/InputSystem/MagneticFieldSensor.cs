using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Magnetic Field")]
	[Preserve]
	public class MagneticFieldSensor : Sensor
	{
		[InputControl(displayName = "Magnetic Field", noisy = true)]
		public Vector3Control magneticField { get; private set; }

		public static MagneticFieldSensor current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (current == this)
			{
				current = null;
			}
		}

		protected override void FinishSetup()
		{
			magneticField = GetChildControl<Vector3Control>("magneticField");
			base.FinishSetup();
		}
	}
}
