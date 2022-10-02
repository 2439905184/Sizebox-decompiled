using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[InputControlLayout(hideInUI = true)]
	[Preserve]
	public class TouchPressControl : ButtonControl
	{
		protected override void FinishSetup()
		{
			base.FinishSetup();
			if (!base.stateBlock.format.IsIntegerFormat())
			{
				throw new NotSupportedException(string.Format("Non-integer format '{0}' is not supported for TouchButtonControl '{1}'", base.stateBlock.format, this));
			}
		}

		public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
		{
			TouchPhase touchPhase = (TouchPhase)MemoryHelpers.ReadIntFromMultipleBits((byte*)statePtr + (int)m_StateBlock.byteOffset, m_StateBlock.bitOffset, m_StateBlock.sizeInBits);
			float value = 0f;
			if (touchPhase == TouchPhase.Began || touchPhase == TouchPhase.Stationary || touchPhase == TouchPhase.Moved)
			{
				value = 1f;
			}
			return Preprocess(value);
		}
	}
}
