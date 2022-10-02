using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(AccelerometerState))]
	[Preserve]
	public class Accelerometer : Sensor
	{
		public Vector3Control acceleration { get; private set; }

		public static Accelerometer current { get; private set; }

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
			acceleration = GetChildControl<Vector3Control>("acceleration");
			base.FinishSetup();
		}
	}
}
