using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public struct Touch : IEquatable<Touch>
	{
		internal struct FingerAndTouchState
		{
			public InputUpdateType updateMask;

			public uint updateStepCount;

			public Finger[] fingers;

			public Finger[] activeFingers;

			public Touch[] activeTouches;

			public int activeFingerCount;

			public int activeTouchCount;

			public int totalFingerCount;

			public uint lastId;

			public bool haveBuiltActiveTouches;

			public InputStateHistory<TouchState> activeTouchState;

			public void AddFingers(Touchscreen screen)
			{
				int count = screen.touches.Count;
				ArrayHelpers.EnsureCapacity(ref fingers, totalFingerCount, count);
				for (int i = 0; i < count; i++)
				{
					Finger value = new Finger(screen, i, updateMask);
					ArrayHelpers.AppendWithCapacity(ref fingers, ref totalFingerCount, value);
				}
			}

			public void RemoveFingers(Touchscreen screen)
			{
				int count = screen.touches.Count;
				for (int i = 0; i < fingers.Length; i++)
				{
					if (fingers[i].screen == screen)
					{
						for (int j = 0; j < count; j++)
						{
							fingers[i + j].m_StateHistory.Dispose();
						}
						ArrayHelpers.EraseSliceWithCapacity(ref fingers, ref totalFingerCount, i, count);
						break;
					}
				}
				haveBuiltActiveTouches = false;
			}

			public void Destroy()
			{
				for (int i = 0; i < totalFingerCount; i++)
				{
					fingers[i].m_StateHistory.Dispose();
				}
				InputStateHistory<TouchState> inputStateHistory = activeTouchState;
				if (inputStateHistory != null)
				{
					inputStateHistory.Dispose();
				}
				activeTouchState = null;
			}

			public void UpdateActiveFingers()
			{
				activeFingerCount = 0;
				for (int i = 0; i < totalFingerCount; i++)
				{
					Finger finger = fingers[i];
					if (finger.currentTouch.valid)
					{
						ArrayHelpers.AppendWithCapacity(ref activeFingers, ref activeFingerCount, finger);
					}
				}
			}

			public unsafe void UpdateActiveTouches()
			{
				if (haveBuiltActiveTouches)
				{
					return;
				}
				if (activeTouchState == null)
				{
					activeTouchState = new InputStateHistory<TouchState>();
					activeTouchState.extraMemoryPerRecord = UnsafeUtility.SizeOf<ExtraDataPerTouchState>();
				}
				else
				{
					activeTouchState.Clear();
				}
				activeTouchCount = 0;
				for (int i = 0; i < totalFingerCount; i++)
				{
					Finger finger = fingers[i];
					if (!finger.currentTouch.valid)
					{
						continue;
					}
					int index = activeTouchCount;
					InputStateHistory<TouchState> stateHistory = finger.m_StateHistory;
					int count = stateHistory.Count;
					int num = 0;
					for (int num2 = count - 1; num2 >= 0; num2--)
					{
						InputStateHistory<TouchState>.Record record = stateHistory[num2];
						TouchState unsafeMemoryPtr = *(TouchState*)record.GetUnsafeMemoryPtr();
						ExtraDataPerTouchState* unsafeExtraMemoryPtr = (ExtraDataPerTouchState*)record.GetUnsafeExtraMemoryPtr();
						if (unsafeMemoryPtr.touchId != num || unsafeMemoryPtr.phase.IsEndedOrCanceled())
						{
							bool flag = unsafeExtraMemoryPtr->updateStepCount == updateStepCount;
							if (!flag && unsafeMemoryPtr.phase.IsEndedOrCanceled())
							{
								break;
							}
							InputStateHistory<TouchState>.Record touchRecord = activeTouchState.AddRecord(record);
							Touch value = new Touch(finger, touchRecord);
							if ((unsafeMemoryPtr.phase == TouchPhase.Moved || unsafeMemoryPtr.phase == TouchPhase.Began) && !flag)
							{
								((TouchState*)touchRecord.GetUnsafeMemoryPtr())->phase = TouchPhase.Stationary;
							}
							if (!((unsafeMemoryPtr.phase == TouchPhase.Moved || unsafeMemoryPtr.phase == TouchPhase.Ended) && flag))
							{
								((TouchState*)touchRecord.GetUnsafeMemoryPtr())->delta = default(Vector2);
							}
							else
							{
								((TouchState*)touchRecord.GetUnsafeMemoryPtr())->delta = ((ExtraDataPerTouchState*)touchRecord.GetUnsafeExtraMemoryPtr())->accumulatedDelta;
							}
							ArrayHelpers.InsertAtWithCapacity(ref activeTouches, ref activeTouchCount, index, value);
							num = unsafeMemoryPtr.touchId;
						}
					}
				}
				haveBuiltActiveTouches = true;
			}
		}

		internal struct ExtraDataPerTouchState
		{
			public Vector2 accumulatedDelta;

			public uint updateStepCount;

			public uint uniqueId;
		}

		private readonly Finger m_Finger;

		internal InputStateHistory<TouchState>.Record m_TouchRecord;

		internal static InlinedArray<Touchscreen> s_Touchscreens;

		internal static int s_HistoryLengthPerFinger = 64;

		internal static InlinedArray<Action<Finger>> s_OnFingerDown;

		internal static InlinedArray<Action<Finger>> s_OnFingerMove;

		internal static InlinedArray<Action<Finger>> s_OnFingerUp;

		internal static FingerAndTouchState s_PlayerState;

		public bool valid
		{
			get
			{
				return m_TouchRecord.valid;
			}
		}

		public Finger finger
		{
			get
			{
				return m_Finger;
			}
		}

		public TouchPhase phase
		{
			get
			{
				return state.phase;
			}
		}

		public int touchId
		{
			get
			{
				return state.touchId;
			}
		}

		public float pressure
		{
			get
			{
				return state.pressure;
			}
		}

		public Vector2 radius
		{
			get
			{
				return state.radius;
			}
		}

		public double startTime
		{
			get
			{
				return state.startTime;
			}
		}

		public double time
		{
			get
			{
				return m_TouchRecord.time;
			}
		}

		public Touchscreen screen
		{
			get
			{
				return finger.screen;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				return state.position;
			}
		}

		public Vector2 startScreenPosition
		{
			get
			{
				return state.startPosition;
			}
		}

		public Vector2 delta
		{
			get
			{
				return state.delta;
			}
		}

		public int tapCount
		{
			get
			{
				return state.tapCount;
			}
		}

		public bool isTap
		{
			get
			{
				return state.isTap;
			}
		}

		public bool isInProgress
		{
			get
			{
				TouchPhase touchPhase = phase;
				if ((uint)(touchPhase - 1) <= 1u || touchPhase == TouchPhase.Stationary)
				{
					return true;
				}
				return false;
			}
		}

		internal uint updateStepCount
		{
			get
			{
				return extraData.updateStepCount;
			}
		}

		internal uint uniqueId
		{
			get
			{
				return extraData.uniqueId;
			}
		}

		private unsafe ref TouchState state
		{
			get
			{
				return ref *(TouchState*)m_TouchRecord.GetUnsafeMemoryPtr();
			}
		}

		private unsafe ref ExtraDataPerTouchState extraData
		{
			get
			{
				return ref *(ExtraDataPerTouchState*)m_TouchRecord.GetUnsafeExtraMemoryPtr();
			}
		}

		public TouchHistory history
		{
			get
			{
				if (!valid)
				{
					throw new InvalidOperationException("Touch is invalid");
				}
				return finger.GetTouchHistory(this);
			}
		}

		public static ReadOnlyArray<Touch> activeTouches
		{
			get
			{
				s_PlayerState.UpdateActiveTouches();
				return new ReadOnlyArray<Touch>(s_PlayerState.activeTouches, 0, s_PlayerState.activeTouchCount);
			}
		}

		public static ReadOnlyArray<Finger> fingers
		{
			get
			{
				return new ReadOnlyArray<Finger>(s_PlayerState.fingers, 0, s_PlayerState.totalFingerCount);
			}
		}

		public static ReadOnlyArray<Finger> activeFingers
		{
			get
			{
				s_PlayerState.UpdateActiveFingers();
				return new ReadOnlyArray<Finger>(s_PlayerState.activeFingers, 0, s_PlayerState.activeFingerCount);
			}
		}

		public static IEnumerable<Touchscreen> screens
		{
			get
			{
				return s_Touchscreens;
			}
		}

		public static int maxHistoryLengthPerFinger
		{
			get
			{
				return s_HistoryLengthPerFinger;
			}
		}

		public static event Action<Finger> onFingerDown
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				s_OnFingerDown.AppendWithCapacity(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = s_OnFingerDown.IndexOf(value);
				if (num != -1)
				{
					s_OnFingerDown.RemoveAtWithCapacity(num);
				}
			}
		}

		public static event Action<Finger> onFingerUp
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				s_OnFingerUp.AppendWithCapacity(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = s_OnFingerUp.IndexOf(value);
				if (num != -1)
				{
					s_OnFingerUp.RemoveAtWithCapacity(num);
				}
			}
		}

		public static event Action<Finger> onFingerMove
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				s_OnFingerMove.AppendWithCapacity(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				int num = s_OnFingerMove.IndexOf(value);
				if (num != -1)
				{
					s_OnFingerMove.RemoveAtWithCapacity(num);
				}
			}
		}

		internal Touch(Finger finger, InputStateHistory<TouchState>.Record touchRecord)
		{
			m_Finger = finger;
			m_TouchRecord = touchRecord;
		}

		public override string ToString()
		{
			if (!valid)
			{
				return "<None>";
			}
			return string.Format("{{finger={0} touchId={1} phase={2} position={3} time={4}}}", finger.index, touchId, phase, screenPosition, time);
		}

		public bool Equals(Touch other)
		{
			if (object.Equals(m_Finger, other.m_Finger))
			{
				return m_TouchRecord.Equals(other.m_TouchRecord);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			object obj2;
			if ((obj2 = obj) is Touch)
			{
				Touch other = (Touch)obj2;
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((m_Finger != null) ? m_Finger.GetHashCode() : 0) * 397) ^ m_TouchRecord.GetHashCode();
		}

		internal static void AddTouchscreen(Touchscreen screen)
		{
			s_Touchscreens.AppendWithCapacity(screen, 5);
			s_PlayerState.AddFingers(screen);
		}

		internal static void RemoveTouchscreen(Touchscreen screen)
		{
			int index = s_Touchscreens.IndexOfReference(screen);
			s_Touchscreens.RemoveAtWithCapacity(index);
			s_PlayerState.RemoveFingers(screen);
		}

		internal static void BeginUpdate()
		{
			s_PlayerState.updateStepCount++;
			s_PlayerState.haveBuiltActiveTouches = false;
		}
	}
}
