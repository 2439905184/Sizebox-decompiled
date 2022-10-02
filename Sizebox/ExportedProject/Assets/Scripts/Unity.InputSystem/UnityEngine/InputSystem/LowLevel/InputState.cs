using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public static class InputState
	{
		private class StateChangeMonitorDelegate : IInputStateChangeMonitor
		{
			public Action<InputControl, double, InputEventPtr, long> valueChangeCallback;

			public Action<InputControl, double, long, int> timerExpiredCallback;

			public void NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long monitorIndex)
			{
				valueChangeCallback(control, time, eventPtr, monitorIndex);
			}

			public void NotifyTimerExpired(InputControl control, double time, long monitorIndex, int timerIndex)
			{
				Action<InputControl, double, long, int> action = timerExpiredCallback;
				if (action != null)
				{
					action(control, time, monitorIndex, timerIndex);
				}
			}
		}

		public static InputUpdateType currentUpdateType
		{
			get
			{
				return InputUpdate.s_LastUpdateType;
			}
		}

		public static uint updateCount
		{
			get
			{
				return InputUpdate.s_UpdateStepCount;
			}
		}

		public static double currentTime
		{
			get
			{
				return InputRuntime.s_Instance.currentTime + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public static event Action<InputDevice, InputEventPtr> onChange
		{
			add
			{
				InputSystem.s_Manager.onDeviceStateChange += value;
			}
			remove
			{
				InputSystem.s_Manager.onDeviceStateChange -= value;
			}
		}

		public unsafe static void Change(InputDevice device, InputEventPtr eventPtr, InputUpdateType updateType = InputUpdateType.None)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			FourCC stateFormat;
			if (eventPtr.IsA<StateEvent>())
			{
				stateFormat = StateEvent.From(eventPtr)->stateFormat;
			}
			else
			{
				if (!eventPtr.IsA<DeltaStateEvent>())
				{
					return;
				}
				stateFormat = DeltaStateEvent.From(eventPtr)->stateFormat;
			}
			if (stateFormat != device.stateBlock.format)
			{
				throw new ArgumentException(string.Format("State format {0} from event does not match state format {1} of device {2}", stateFormat, device.stateBlock.format, device), "eventPtr");
			}
			InputSystem.s_Manager.UpdateState(device, eventPtr, (updateType != 0) ? updateType : InputSystem.s_Manager.defaultUpdateType);
		}

		public unsafe static void Change<TState>(InputControl control, TState state, InputUpdateType updateType = InputUpdateType.None, InputEventPtr eventPtr = default(InputEventPtr)) where TState : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (control.stateBlock.bitOffset != 0 || control.stateBlock.sizeInBits % 8u != 0)
			{
				throw new ArgumentException(string.Format("Cannot change state of bitfield control '{0}' using this method", control), "control");
			}
			InputDevice device = control.device;
			long num = Math.Min(UnsafeUtility.SizeOf<TState>(), control.m_StateBlock.alignedSizeInBytes);
			void* statePtr = UnsafeUtility.AddressOf(ref state);
			uint stateOffsetInDevice = control.stateBlock.byteOffset - device.stateBlock.byteOffset;
			InputSystem.s_Manager.UpdateState(device, (updateType != 0) ? updateType : InputSystem.s_Manager.defaultUpdateType, statePtr, stateOffsetInDevice, (uint)num, eventPtr.valid ? eventPtr.internalTime : InputRuntime.s_Instance.currentTime, eventPtr);
		}

		public static bool IsIntegerFormat(this FourCC format)
		{
			if (!(format == InputStateBlock.FormatBit) && !(format == InputStateBlock.FormatInt) && !(format == InputStateBlock.FormatByte) && !(format == InputStateBlock.FormatShort) && !(format == InputStateBlock.FormatSBit) && !(format == InputStateBlock.FormatUInt) && !(format == InputStateBlock.FormatUShort) && !(format == InputStateBlock.FormatLong))
			{
				return format == InputStateBlock.FormatULong;
			}
			return true;
		}

		public static void AddChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex = -1L)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor");
			}
			if (control.device.m_DeviceIndex == -1)
			{
				throw new ArgumentException(string.Format("Device for control '{0}' has not been added to system"), "control");
			}
			InputSystem.s_Manager.AddStateChangeMonitor(control, monitor, monitorIndex);
		}

		public static IInputStateChangeMonitor AddChangeMonitor(InputControl control, Action<InputControl, double, InputEventPtr, long> valueChangeCallback, int monitorIndex = -1, Action<InputControl, double, long, int> timerExpiredCallback = null)
		{
			if (valueChangeCallback == null)
			{
				throw new ArgumentNullException("valueChangeCallback");
			}
			StateChangeMonitorDelegate stateChangeMonitorDelegate = new StateChangeMonitorDelegate
			{
				valueChangeCallback = valueChangeCallback,
				timerExpiredCallback = timerExpiredCallback
			};
			AddChangeMonitor(control, stateChangeMonitorDelegate, monitorIndex);
			return stateChangeMonitorDelegate;
		}

		public static void RemoveChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex = -1L)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor");
			}
			InputSystem.s_Manager.RemoveStateChangeMonitor(control, monitor, monitorIndex);
		}

		public static void AddChangeMonitorTimeout(InputControl control, IInputStateChangeMonitor monitor, double time, long monitorIndex = -1L, int timerIndex = -1)
		{
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor");
			}
			InputSystem.s_Manager.AddStateChangeMonitorTimeout(control, monitor, time, monitorIndex, timerIndex);
		}

		public static void RemoveChangeMonitorTimeout(IInputStateChangeMonitor monitor, long monitorIndex = -1L, int timerIndex = -1)
		{
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor");
			}
			InputSystem.s_Manager.RemoveStateChangeMonitorTimeout(monitor, monitorIndex, timerIndex);
		}
	}
}
