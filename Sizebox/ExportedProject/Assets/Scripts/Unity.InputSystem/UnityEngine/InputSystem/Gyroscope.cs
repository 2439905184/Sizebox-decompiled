using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(GyroscopeState))]
	[Preserve]
	public class Gyroscope : Sensor
	{
		public Vector3Control angularVelocity { get; private set; }

		public static Gyroscope current { get; private set; }

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
			angularVelocity = GetChildControl<Vector3Control>("angularVelocity");
			base.FinishSetup();
		}
	}
}
