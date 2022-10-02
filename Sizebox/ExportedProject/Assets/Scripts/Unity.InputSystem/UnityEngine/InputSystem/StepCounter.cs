using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(displayName = "Step Counter")]
	[Preserve]
	public class StepCounter : Sensor
	{
		[InputControl(displayName = "Step Counter", noisy = true)]
		public IntegerControl stepCounter { get; private set; }

		public static StepCounter current { get; private set; }

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
			stepCounter = GetChildControl<IntegerControl>("stepCounter");
			base.FinishSetup();
		}
	}
}
