using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public class Finger
	{
		[CompilerGenerated]
		private readonly Touchscreen _003Cscreen_003Ek__BackingField;

		[CompilerGenerated]
		private readonly int _003Cindex_003Ek__BackingField;

		internal readonly InputStateHistory<TouchState> m_StateHistory;

		public Touchscreen screen
		{
			[CompilerGenerated]
			get
			{
				return _003Cscreen_003Ek__BackingField;
			}
		}

		public int index
		{
			[CompilerGenerated]
			get
			{
				return _003Cindex_003Ek__BackingField;
			}
		}

		public bool isActive
		{
			get
			{
				return currentTouch.valid;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				Touch touch = lastTouch;
				if (!touch.valid)
				{
					return default(Vector2);
				}
				return touch.screenPosition;
			}
		}

		public Touch lastTouch
		{
			get
			{
				int count = m_StateHistory.Count;
				if (count == 0)
				{
					return default(Touch);
				}
				return new Touch(this, m_StateHistory[count - 1]);
			}
		}

		public Touch currentTouch
		{
			get
			{
				Touch result = lastTouch;
				if (!result.valid)
				{
					return default(Touch);
				}
				if (result.isInProgress)
				{
					return result;
				}
				if (result.updateStepCount == Touch.s_PlayerState.updateStepCount)
				{
					return result;
				}
				return default(Touch);
			}
		}

		public TouchHistory touchHistory
		{
			get
			{
				return new TouchHistory(this, m_StateHistory);
			}
		}

		internal Finger(Touchscreen screen, int index, InputUpdateType updateMask)
		{
			_003Cscreen_003Ek__BackingField = screen;
			_003Cindex_003Ek__BackingField = index;
			m_StateHistory = new InputStateHistory<TouchState>(screen.touches[index])
			{
				historyDepth = Touch.maxHistoryLengthPerFinger,
				extraMemoryPerRecord = UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>(),
				onRecordAdded = OnTouchRecorded,
				onShouldRecordStateChange = ShouldRecordTouch,
				updateMask = updateMask
			};
			m_StateHistory.StartRecording();
		}

		private static bool ShouldRecordTouch(InputControl control, double time, InputEventPtr eventPtr)
		{
			if (!eventPtr.valid)
			{
				return false;
			}
			TouchState touchState = ((TouchControl)control).ReadValue();
			if (touchState.phase == TouchPhase.Ended && !touchState.isTap && touchState.tapCount > 0)
			{
				return false;
			}
			return true;
		}

		private unsafe void OnTouchRecorded(InputStateHistory.Record record)
		{
			TouchState* unsafeMemoryPtr = (TouchState*)record.GetUnsafeMemoryPtr();
			Touch.s_PlayerState.haveBuiltActiveTouches = false;
			Touch.ExtraDataPerTouchState* unsafeExtraMemoryPtr = (Touch.ExtraDataPerTouchState*)record.GetUnsafeExtraMemoryPtr();
			unsafeExtraMemoryPtr->updateStepCount = Touch.s_PlayerState.updateStepCount;
			unsafeExtraMemoryPtr->uniqueId = ++Touch.s_PlayerState.lastId;
			unsafeExtraMemoryPtr->accumulatedDelta = unsafeMemoryPtr->delta;
			if (unsafeMemoryPtr->phase != TouchPhase.Began)
			{
				InputStateHistory.Record previous = record.previous;
				if (previous.valid)
				{
					unsafeMemoryPtr->delta -= ((TouchState*)previous.GetUnsafeMemoryPtr())->delta;
				}
			}
			TouchState* unsafeMemoryPtr2 = (TouchState*)record.GetUnsafeMemoryPtr();
			switch (unsafeMemoryPtr2->phase)
			{
			case TouchPhase.Began:
				DelegateHelpers.InvokeCallbacksSafe(ref Touch.s_OnFingerDown, this, "Touch.onFingerDown");
				break;
			case TouchPhase.Moved:
				DelegateHelpers.InvokeCallbacksSafe(ref Touch.s_OnFingerMove, this, "Touch.onFingerMove");
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				DelegateHelpers.InvokeCallbacksSafe(ref Touch.s_OnFingerUp, this, "Touch.onFingerUp");
				break;
			}
		}

		private unsafe Touch FindTouch(uint uniqueId)
		{
			foreach (InputStateHistory<TouchState>.Record item in m_StateHistory)
			{
				if (((Touch.ExtraDataPerTouchState*)item.GetUnsafeExtraMemoryPtr())->uniqueId == uniqueId)
				{
					return new Touch(this, item);
				}
			}
			return default(Touch);
		}

		internal unsafe TouchHistory GetTouchHistory(Touch touch)
		{
			InputStateHistory<TouchState>.Record touchRecord = touch.m_TouchRecord;
			if (touchRecord.owner != m_StateHistory)
			{
				touch = FindTouch(touch.uniqueId);
				if (!touch.valid)
				{
					return default(TouchHistory);
				}
			}
			int touchId = touch.touchId;
			int num = touch.m_TouchRecord.index;
			int num2 = 0;
			if (touch.phase != TouchPhase.Began)
			{
				InputStateHistory<TouchState>.Record previous = touch.m_TouchRecord.previous;
				while (previous.valid)
				{
					TouchState* unsafeMemoryPtr = (TouchState*)previous.GetUnsafeMemoryPtr();
					if (unsafeMemoryPtr->touchId != touchId)
					{
						break;
					}
					num2++;
					if (unsafeMemoryPtr->phase == TouchPhase.Began)
					{
						break;
					}
					previous = previous.previous;
				}
			}
			if (num2 == 0)
			{
				return default(TouchHistory);
			}
			num--;
			return new TouchHistory(this, m_StateHistory, num, num2);
		}
	}
}
