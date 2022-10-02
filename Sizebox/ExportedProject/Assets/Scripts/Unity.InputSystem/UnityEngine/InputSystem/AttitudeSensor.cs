using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(AttitudeState), displayName = "Attitude")]
	[Preserve]
	public class AttitudeSensor : Sensor
	{
		public QuaternionControl attitude { get; private set; }

		public static AttitudeSensor current { get; private set; }

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
			attitude = GetChildControl<QuaternionControl>("attitude");
			base.FinishSetup();
		}
	}
}
