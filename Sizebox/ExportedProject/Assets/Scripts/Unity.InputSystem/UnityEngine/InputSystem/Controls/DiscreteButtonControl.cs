using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class DiscreteButtonControl : ButtonControl
	{
		public int minValue;

		public int maxValue;

		public int wrapAtValue;

		public int nullValue;

		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.stateBlock.format.IsIntegerFormat())
			{
				throw new NotSupportedException(string.Format("Non-integer format '{0}' is not supported for DiscreteButtonControl '{1}'", base.stateBlock.format, this));
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			int num = MemoryHelpers.ReadIntFromMultipleBits((byte*)statePtr + (int)m_StateBlock.byteOffset, m_StateBlock.bitOffset, m_StateBlock.sizeInBits);
			float value = 0f;
			if (minValue > maxValue)
			{
				if (wrapAtValue == nullValue)
				{
					wrapAtValue = minValue;
				}
				if ((num >= minValue && num <= wrapAtValue) || (num != nullValue && num <= maxValue))
				{
					value = 1f;
				}
			}
			else
			{
				value = ((num >= minValue && num <= maxValue) ? 1f : 0f);
			}
			return Preprocess(value);
		}

		public unsafe override void WriteValueIntoState(float value, void* statePtr)
		{
			throw new NotImplementedException();
		}
	}
}
