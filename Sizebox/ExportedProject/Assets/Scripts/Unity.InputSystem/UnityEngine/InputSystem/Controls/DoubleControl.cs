using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class DoubleControl : InputControl<double>
	{
		public DoubleControl()
		{
			m_StateBlock.format = InputStateBlock.FormatDouble;
		}

		public unsafe override double ReadUnprocessedValueFromState(void* statePtr)
		{
			return m_StateBlock.ReadDouble(statePtr);
		}

		public unsafe override void WriteValueIntoState(double value, void* statePtr)
		{
			m_StateBlock.WriteDouble(statePtr, value);
		}
	}
}
