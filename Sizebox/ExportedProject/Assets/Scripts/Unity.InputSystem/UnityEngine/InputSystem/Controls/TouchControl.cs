using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[InputControlLayout(stateType = typeof(TouchState))]
	[Preserve]
	public class TouchControl : InputControl<TouchState>
	{
		public TouchPressControl press { get; private set; }

		public IntegerControl touchId { get; private set; }

		public Vector2Control position { get; private set; }

		public Vector2Control delta { get; private set; }

		public AxisControl pressure { get; private set; }

		public Vector2Control radius { get; private set; }

		public TouchPhaseControl phase { get; private set; }

		public ButtonControl indirectTouch { get; private set; }

		public ButtonControl tap { get; private set; }

		public IntegerControl tapCount { get; private set; }

		public DoubleControl startTime { get; private set; }

		public Vector2Control startPosition { get; private set; }

		public bool isInProgress
		{
			get
			{
				TouchPhase touchPhase = phase.ReadValue();
				if ((uint)(touchPhase - 1) <= 1u || touchPhase == TouchPhase.Stationary)
				{
					return true;
				}
				return false;
			}
		}

		public TouchControl()
		{
			m_StateBlock.format = new FourCC('T', 'O', 'U', 'C');
		}

		protected override void FinishSetup()
		{
			press = GetChildControl<TouchPressControl>("press");
			touchId = GetChildControl<IntegerControl>("touchId");
			position = GetChildControl<Vector2Control>("position");
			delta = GetChildControl<Vector2Control>("delta");
			pressure = GetChildControl<AxisControl>("pressure");
			radius = GetChildControl<Vector2Control>("radius");
			phase = GetChildControl<TouchPhaseControl>("phase");
			indirectTouch = GetChildControl<ButtonControl>("indirectTouch");
			tap = GetChildControl<ButtonControl>("tap");
			tapCount = GetChildControl<IntegerControl>("tapCount");
			startTime = GetChildControl<DoubleControl>("startTime");
			startPosition = GetChildControl<Vector2Control>("startPosition");
			base.FinishSetup();
		}

		public unsafe override TouchState ReadUnprocessedValueFromState(void* statePtr)
		{
			TouchState* ptr = (TouchState*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
			return *ptr;
		}

		public unsafe override void WriteValueIntoState(TouchState value, void* statePtr)
		{
			TouchState* destination = (TouchState*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
			UnsafeUtility.MemCpy(destination, UnsafeUtility.AddressOf(ref value), UnsafeUtility.SizeOf<TouchState>());
		}
	}
}
