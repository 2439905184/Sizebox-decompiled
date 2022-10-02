using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Humidity")]
	[Preserve]
	public class HumiditySensor : Sensor
	{
		[InputControl(displayName = "Relative Humidity", noisy = true)]
		public AxisControl relativeHumidity { get; private set; }

		public static HumiditySensor current { get; private set; }

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
			relativeHumidity = GetChildControl<AxisControl>("relativeHumidity");
			base.FinishSetup();
		}
	}
}
