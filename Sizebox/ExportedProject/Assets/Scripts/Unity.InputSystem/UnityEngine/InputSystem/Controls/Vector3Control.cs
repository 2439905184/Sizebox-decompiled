using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class Vector3Control : InputControl<Vector3>
	{
		[InputControl(offset = 0u, displayName = "X")]
		public AxisControl x { get; private set; }

		[InputControl(offset = 4u, displayName = "Y")]
		public AxisControl y { get; private set; }

		[InputControl(offset = 8u, displayName = "Z")]
		public AxisControl z { get; private set; }

		public Vector3Control()
		{
			m_StateBlock.format = InputStateBlock.FormatVector3;
		}

		protected override void FinishSetup()
		{
			x = GetChildControl<AxisControl>("x");
			y = GetChildControl<AxisControl>("y");
			z = GetChildControl<AxisControl>("z");
			base.FinishSetup();
		}

		public unsafe override Vector3 ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Vector3(x.ReadUnprocessedValueFromState(statePtr), y.ReadUnprocessedValueFromState(statePtr), z.ReadUnprocessedValueFromState(statePtr));
		}

		public unsafe override void WriteValueIntoState(Vector3 value, void* statePtr)
		{
			x.WriteValueIntoState(value.x, statePtr);
			y.WriteValueIntoState(value.y, statePtr);
			z.WriteValueIntoState(value.z, statePtr);
		}

		public unsafe override float EvaluateMagnitude(void* statePtr)
		{
			return ReadValueFromState(statePtr).magnitude;
		}
	}
}
