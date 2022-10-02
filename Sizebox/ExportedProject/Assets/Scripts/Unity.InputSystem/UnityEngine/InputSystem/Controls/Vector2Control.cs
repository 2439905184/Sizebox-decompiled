using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class Vector2Control : InputControl<Vector2>
	{
		[InputControl(offset = 0u, displayName = "X")]
		public AxisControl x { get; private set; }

		[InputControl(offset = 4u, displayName = "Y")]
		public AxisControl y { get; private set; }

		public Vector2Control()
		{
			m_StateBlock.format = InputStateBlock.FormatVector2;
		}

		protected override void FinishSetup()
		{
			x = GetChildControl<AxisControl>("x");
			y = GetChildControl<AxisControl>("y");
			base.FinishSetup();
		}

		public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Vector2(x.ReadUnprocessedValueFromState(statePtr), y.ReadUnprocessedValueFromState(statePtr));
		}

		public unsafe override void WriteValueIntoState(Vector2 value, void* statePtr)
		{
			x.WriteValueIntoState(value.x, statePtr);
			y.WriteValueIntoState(value.y, statePtr);
		}

		public unsafe override float EvaluateMagnitude(void* statePtr)
		{
			return ReadValueFromState(statePtr).magnitude;
		}
	}
}
