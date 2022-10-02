using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngineInternal.Input;

namespace UnityEngine.InputSystem.LowLevel
{
	internal class NativeInputRuntime : IInputRuntime
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{
			public InputUpdateDelegate value;

			internal unsafe void _003Cset_onUpdate_003Eb__0(NativeInputUpdateType updateType, NativeInputEventBuffer* eventBufferPtr)
			{
				InputEventBuffer eventBuffer = new InputEventBuffer((InputEvent*)eventBufferPtr->eventBuffer, eventBufferPtr->eventCount, eventBufferPtr->sizeInBytes, eventBufferPtr->capacityInBytes);
				try
				{
					value((InputUpdateType)updateType, ref eventBuffer);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("{0} during event processing of {1} update; resetting event buffer", ex.GetType().Name, updateType));
					Debug.LogException(ex);
					eventBuffer.Reset();
				}
				if (eventBuffer.eventCount > 0)
				{
					eventBufferPtr->eventCount = eventBuffer.eventCount;
					eventBufferPtr->sizeInBytes = (int)eventBuffer.sizeInBytes;
					eventBufferPtr->capacityInBytes = (int)eventBuffer.capacityInBytes;
					eventBufferPtr->eventBuffer = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(eventBuffer.data);
				}
				else
				{
					eventBufferPtr->eventCount = 0;
					eventBufferPtr->sizeInBytes = 0;
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public Action<InputUpdateType> value;

			internal void _003Cset_onBeforeUpdate_003Eb__0(NativeInputUpdateType updateType)
			{
				value((InputUpdateType)updateType);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass13_0
		{
			public Func<InputUpdateType, bool> value;

			internal bool _003Cset_onShouldRunUpdate_003Eb__0(NativeInputUpdateType updateType)
			{
				return value((InputUpdateType)updateType);
			}
		}

		public static readonly NativeInputRuntime instance = new NativeInputRuntime();

		private Action m_ShutdownMethod;

		private InputUpdateDelegate m_OnUpdate;

		private Action<InputUpdateType> m_OnBeforeUpdate;

		private Func<InputUpdateType, bool> m_OnShouldRunUpdate;

		private float m_PollingFrequency = 60f;

		private bool m_DidCallOnShutdown;

		private Action<bool> m_FocusChangedMethod;

		public unsafe InputUpdateDelegate onUpdate
		{
			get
			{
				return m_OnUpdate;
			}
			set
			{
				_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
				_003C_003Ec__DisplayClass7_.value = value;
				if (_003C_003Ec__DisplayClass7_.value != null)
				{
					NativeInputSystem.onUpdate = _003C_003Ec__DisplayClass7_._003Cset_onUpdate_003Eb__0;
				}
				else
				{
					NativeInputSystem.onUpdate = null;
				}
				m_OnUpdate = _003C_003Ec__DisplayClass7_.value;
			}
		}

		public Action<InputUpdateType> onBeforeUpdate
		{
			get
			{
				return m_OnBeforeUpdate;
			}
			set
			{
				_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = new _003C_003Ec__DisplayClass10_0();
				_003C_003Ec__DisplayClass10_.value = value;
				if (_003C_003Ec__DisplayClass10_.value != null)
				{
					NativeInputSystem.onBeforeUpdate = _003C_003Ec__DisplayClass10_._003Cset_onBeforeUpdate_003Eb__0;
				}
				else
				{
					NativeInputSystem.onBeforeUpdate = null;
				}
				m_OnBeforeUpdate = _003C_003Ec__DisplayClass10_.value;
			}
		}

		public Func<InputUpdateType, bool> onShouldRunUpdate
		{
			get
			{
				return m_OnShouldRunUpdate;
			}
			set
			{
				_003C_003Ec__DisplayClass13_0 _003C_003Ec__DisplayClass13_ = new _003C_003Ec__DisplayClass13_0();
				_003C_003Ec__DisplayClass13_.value = value;
				if (_003C_003Ec__DisplayClass13_.value != null)
				{
					NativeInputSystem.onShouldRunUpdate = _003C_003Ec__DisplayClass13_._003Cset_onShouldRunUpdate_003Eb__0;
				}
				else
				{
					NativeInputSystem.onShouldRunUpdate = null;
				}
				m_OnShouldRunUpdate = _003C_003Ec__DisplayClass13_.value;
			}
		}

		public Action<int, string> onDeviceDiscovered
		{
			get
			{
				return NativeInputSystem.onDeviceDiscovered;
			}
			set
			{
				NativeInputSystem.onDeviceDiscovered = value;
			}
		}

		public Action onShutdown
		{
			get
			{
				return m_ShutdownMethod;
			}
			set
			{
				if (value == null)
				{
					Application.quitting -= OnShutdown;
				}
				else if (m_ShutdownMethod == null)
				{
					Application.quitting += OnShutdown;
				}
				m_ShutdownMethod = value;
			}
		}

		public Action<bool> onPlayerFocusChanged
		{
			get
			{
				return m_FocusChangedMethod;
			}
			set
			{
				if (value == null)
				{
					Application.focusChanged -= OnFocusChanged;
				}
				else if (m_FocusChangedMethod == null)
				{
					Application.focusChanged += OnFocusChanged;
				}
				m_FocusChangedMethod = value;
			}
		}

		public float pollingFrequency
		{
			get
			{
				return m_PollingFrequency;
			}
			set
			{
				m_PollingFrequency = value;
				NativeInputSystem.SetPollingFrequency(value);
			}
		}

		public double currentTime
		{
			get
			{
				return NativeInputSystem.currentTime;
			}
		}

		public double currentTimeForFixedUpdate
		{
			get
			{
				return (double)Time.fixedUnscaledTime + currentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public double currentTimeOffsetToRealtimeSinceStartup
		{
			get
			{
				return NativeInputSystem.currentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public float unscaledGameTime
		{
			get
			{
				return Time.unscaledTime;
			}
		}

		public bool runInBackground
		{
			get
			{
				return Application.runInBackground;
			}
		}

		public ScreenOrientation screenOrientation
		{
			get
			{
				return Screen.orientation;
			}
		}

		public bool isInBatchMode
		{
			get
			{
				return Application.isBatchMode;
			}
		}

		public int AllocateDeviceId()
		{
			return NativeInputSystem.AllocateDeviceId();
		}

		public void Update(InputUpdateType updateType)
		{
			NativeInputSystem.Update((NativeInputUpdateType)updateType);
		}

		public unsafe void QueueEvent(InputEvent* ptr)
		{
			NativeInputSystem.QueueInputEvent((IntPtr)ptr);
		}

		public unsafe long DeviceCommand(int deviceId, InputDeviceCommand* commandPtr)
		{
			if (commandPtr == null)
			{
				throw new ArgumentNullException("commandPtr");
			}
			return NativeInputSystem.IOCTL(deviceId, commandPtr->type, new IntPtr(commandPtr->payloadPtr), commandPtr->payloadSizeInBytes);
		}

		private void OnShutdown()
		{
			m_ShutdownMethod();
		}

		private bool OnWantsToShutdown()
		{
			if (!m_DidCallOnShutdown)
			{
				OnShutdown();
				m_DidCallOnShutdown = true;
			}
			return true;
		}

		private void OnFocusChanged(bool focus)
		{
			m_FocusChangedMethod(focus);
		}

		public void RegisterAnalyticsEvent(string name, int maxPerHour, int maxPropertiesPerEvent)
		{
		}

		public void SendAnalyticsEvent(string name, object data)
		{
		}
	}
}
