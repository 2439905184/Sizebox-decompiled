using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class QuaternionControl : InputControl<Quaternion>
	{
		[InputControl(displayName = "X")]
		public AxisControl x { get; private set; }

		[InputControl(displayName = "Y")]
		public AxisControl y { get; private set; }

		[InputControl(displayName = "Z")]
		public AxisControl z { get; private set; }

		[InputControl(displayName = "W")]
		public AxisControl w { get; private set; }

		public QuaternionControl()
		{
			m_StateBlock.sizeInBits = 128u;
			m_StateBlock.format = InputStateBlock.FormatQuaternion;
		}

		protected override void FinishSetup()
		{
			x = GetChildControl<AxisControl>("x");
			y = GetChildControl<AxisControl>("y");
			z = GetChildControl<AxisControl>("z");
			w = GetChildControl<AxisControl>("w");
			base.FinishSetup();
		}

		public unsafe override Quaternion ReadUnprocessedValueFromState(void* statePtr)
		{
			return new Quaternion(x.ReadValueFromState(statePtr), y.ReadValueFromState(statePtr), z.ReadValueFromState(statePtr), w.ReadUnprocessedValueFromState(statePtr));
		}

		public unsafe override void WriteValueIntoState(Quaternion value, void* statePtr)
		{
			x.WriteValueIntoState(value.x, statePtr);
			y.WriteValueIntoState(value.y, statePtr);
			z.WriteValueIntoState(value.z, statePtr);
			w.WriteValueIntoState(value.w, statePtr);
		}
	}
}
