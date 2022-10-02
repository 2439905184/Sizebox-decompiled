using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class StickControl : Vector2Control
	{
		[InputControl(useStateFrom = "y", processors = "axisDeadzone", parameters = "clamp=2,clampMin=0,clampMax=1", synthetic = true, displayName = "Up")]
		[InputControl(name = "x", minValue = -1f, maxValue = 1f, layout = "Axis", processors = "axisDeadzone")]
		[InputControl(name = "y", minValue = -1f, maxValue = 1f, layout = "Axis", processors = "axisDeadzone")]
		[Preserve]
		public ButtonControl up { get; private set; }

		[InputControl(useStateFrom = "y", processors = "axisDeadzone", parameters = "clamp=2,clampMin=-1,clampMax=0,invert", synthetic = true, displayName = "Down")]
		[Preserve]
		public ButtonControl down { get; private set; }

		[InputControl(useStateFrom = "x", processors = "axisDeadzone", parameters = "clamp=2,clampMin=-1,clampMax=0,invert", synthetic = true, displayName = "Left")]
		[Preserve]
		public ButtonControl left { get; private set; }

		[InputControl(useStateFrom = "x", processors = "axisDeadzone", parameters = "clamp=2,clampMin=0,clampMax=1", synthetic = true, displayName = "Right")]
		[Preserve]
		public ButtonControl right { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			up = GetChildControl<ButtonControl>("up");
			down = GetChildControl<ButtonControl>("down");
			left = GetChildControl<ButtonControl>("left");
			right = GetChildControl<ButtonControl>("right");
		}
	}
}
