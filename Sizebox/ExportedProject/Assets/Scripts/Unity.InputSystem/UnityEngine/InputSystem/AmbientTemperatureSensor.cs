using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Ambient Temperature")]
	[Preserve]
	public class AmbientTemperatureSensor : Sensor
	{
		[InputControl(displayName = "Ambient Temperature", noisy = true)]
		public AxisControl ambientTemperature { get; private set; }

		public static AmbientTemperatureSensor current { get; private set; }

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
			ambientTemperature = GetChildControl<AxisControl>("ambientTemperature");
			base.FinishSetup();
		}
	}
}
