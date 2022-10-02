using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem
{
	[Preserve]
	public class InputDevice : InputControl
	{
		[Flags]
		internal enum DeviceFlags
		{
			UpdateBeforeRender = 1,
			HasStateCallbacks = 2,
			HasControlsWithDefaultState = 4,
			Remote = 8,
			Native = 0x10,
			Disabled = 0x20,
			DisabledStateHasBeenQueried = 0x40
		}

		public const int InvalidDeviceId = 0;

		internal const int kLocalParticipantId = 0;

		internal const int kInvalidDeviceIndex = -1;

		internal DeviceFlags m_DeviceFlags;

		internal int m_DeviceId;

		internal int m_ParticipantId;

		internal int m_DeviceIndex;

		internal InputDeviceDescription m_Description;

		internal double m_LastUpdateTimeInternal;

		internal uint m_CurrentUpdateStepCount;

		internal InternedString[] m_AliasesForEachControl;

		internal InternedString[] m_UsagesForEachControl;

		internal InputControl[] m_UsageToControl;

		internal InputControl[] m_ChildrenForEachControl;

		public InputDeviceDescription description
		{
			get
			{
				return m_Description;
			}
		}

		public bool enabled
		{
			get
			{
				if ((m_DeviceFlags & DeviceFlags.DisabledStateHasBeenQueried) == 0)
				{
					QueryEnabledStateCommand command = QueryEnabledStateCommand.Create();
					if (ExecuteCommand(ref command) >= 0)
					{
						if (command.isEnabled)
						{
							m_DeviceFlags &= ~DeviceFlags.Disabled;
						}
						else
						{
							m_DeviceFlags |= DeviceFlags.Disabled;
						}
					}
					else
					{
						m_DeviceFlags &= ~DeviceFlags.Disabled;
					}
					m_DeviceFlags |= DeviceFlags.DisabledStateHasBeenQueried;
				}
				return (m_DeviceFlags & DeviceFlags.Disabled) != DeviceFlags.Disabled;
			}
		}

		public bool canRunInBackground
		{
			get
			{
				QueryCanRunInBackground command = QueryCanRunInBackground.Create();
				if (ExecuteCommand(ref command) >= 0)
				{
					return command.canRunInBackground;
				}
				return false;
			}
		}

		public bool added
		{
			get
			{
				return m_DeviceIndex != -1;
			}
		}

		public bool remote
		{
			get
			{
				return (m_DeviceFlags & DeviceFlags.Remote) == DeviceFlags.Remote;
			}
		}

		public bool native
		{
			get
			{
				return (m_DeviceFlags & DeviceFlags.Native) == DeviceFlags.Native;
			}
		}

		public bool updateBeforeRender
		{
			get
			{
				return (m_DeviceFlags & DeviceFlags.UpdateBeforeRender) == DeviceFlags.UpdateBeforeRender;
			}
		}

		public int deviceId
		{
			get
			{
				return m_DeviceId;
			}
		}

		public double lastUpdateTime
		{
			get
			{
				return m_LastUpdateTimeInternal - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public bool wasUpdatedThisFrame
		{
			get
			{
				return m_CurrentUpdateStepCount == InputUpdate.s_UpdateStepCount;
			}
		}

		public ReadOnlyArray<InputControl> allControls
		{
			get
			{
				return new ReadOnlyArray<InputControl>(m_ChildrenForEachControl);
			}
		}

		public override Type valueType
		{
			get
			{
				return typeof(byte[]);
			}
		}

		public override int valueSizeInBytes
		{
			get
			{
				return (int)m_StateBlock.alignedSizeInBytes;
			}
		}

		[Obsolete("Use 'InputSystem.devices' instead. (UnityUpgradable) -> InputSystem.devices", false)]
		public static ReadOnlyArray<InputDevice> all
		{
			get
			{
				return InputSystem.devices;
			}
		}

		internal bool hasControlsWithDefaultState
		{
			get
			{
				return (m_DeviceFlags & DeviceFlags.HasControlsWithDefaultState) == DeviceFlags.HasControlsWithDefaultState;
			}
			set
			{
				if (value)
				{
					m_DeviceFlags |= DeviceFlags.HasControlsWithDefaultState;
				}
				else
				{
					m_DeviceFlags &= ~DeviceFlags.HasControlsWithDefaultState;
				}
			}
		}

		internal bool hasStateCallbacks
		{
			get
			{
				return (m_DeviceFlags & DeviceFlags.HasStateCallbacks) == DeviceFlags.HasStateCallbacks;
			}
			set
			{
				if (value)
				{
					m_DeviceFlags |= DeviceFlags.HasStateCallbacks;
				}
				else
				{
					m_DeviceFlags &= ~DeviceFlags.HasStateCallbacks;
				}
			}
		}

		public InputDevice()
		{
			m_DeviceId = 0;
			m_ParticipantId = 0;
			m_DeviceIndex = -1;
		}

		public unsafe override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
		{
			throw new NotImplementedException();
		}

		public unsafe override object ReadValueFromStateAsObject(void* statePtr)
		{
			if (m_DeviceIndex == -1)
			{
				return null;
			}
			uint alignedSizeInBytes = base.stateBlock.alignedSizeInBytes;
			byte[] array = new byte[alignedSizeInBytes];
			fixed (byte* destination = array)
			{
				byte* source = (byte*)statePtr + m_StateBlock.byteOffset;
				UnsafeUtility.MemCpy(destination, source, alignedSizeInBytes);
			}
			return array;
		}

		public unsafe override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
		{
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			if (bufferPtr == null)
			{
				throw new ArgumentNullException("bufferPtr");
			}
			if (bufferSize < valueSizeInBytes)
			{
				throw new ArgumentException(string.Format("Buffer too small (expected: {0}, actual: {1}", valueSizeInBytes, bufferSize));
			}
			byte* source = (byte*)statePtr + m_StateBlock.byteOffset;
			UnsafeUtility.MemCpy(bufferPtr, source, m_StateBlock.alignedSizeInBytes);
		}

		public unsafe override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
		{
			if (firstStatePtr == null)
			{
				throw new ArgumentNullException("firstStatePtr");
			}
			if (secondStatePtr == null)
			{
				throw new ArgumentNullException("secondStatePtr");
			}
			byte* ptr = (byte*)firstStatePtr + m_StateBlock.byteOffset;
			byte* ptr2 = (byte*)firstStatePtr + m_StateBlock.byteOffset;
			return UnsafeUtility.MemCmp(ptr, ptr2, m_StateBlock.alignedSizeInBytes) == 0;
		}

		internal void OnConfigurationChanged()
		{
			base.isConfigUpToDate = false;
			for (int i = 0; i < m_ChildrenForEachControl.Length; i++)
			{
				m_ChildrenForEachControl[i].isConfigUpToDate = false;
			}
			m_DeviceFlags &= ~DeviceFlags.DisabledStateHasBeenQueried;
		}

		public virtual void MakeCurrent()
		{
		}

		protected virtual void OnAdded()
		{
		}

		protected virtual void OnRemoved()
		{
		}

		public unsafe long ExecuteCommand<TCommand>(ref TCommand command) where TCommand : struct, IInputDeviceCommandInfo
		{
			InlinedArray<InputDeviceCommandDelegate> deviceCommandCallbacks = InputSystem.s_Manager.m_DeviceCommandCallbacks;
			for (int i = 0; i < deviceCommandCallbacks.length; i++)
			{
				long? num = deviceCommandCallbacks[i](this, (InputDeviceCommand*)UnsafeUtility.AddressOf(ref command));
				if (num.HasValue)
				{
					return num.Value;
				}
			}
			return InputRuntime.s_Instance.DeviceCommand(deviceId, ref command);
		}

		internal void AddDeviceUsage(InternedString usage)
		{
			int count = m_UsageToControl.LengthSafe() + m_UsageCount;
			if (m_UsageCount == 0)
			{
				m_UsageStartIndex = count;
			}
			ArrayHelpers.AppendWithCapacity(ref m_UsagesForEachControl, ref count, usage);
			m_UsageCount++;
		}

		internal void RemoveDeviceUsage(InternedString usage)
		{
			int count = m_UsageToControl.LengthSafe() + m_UsageCount;
			int num = m_UsagesForEachControl.IndexOfValue(usage, m_UsageStartIndex, count);
			if (num != -1)
			{
				ArrayHelpers.EraseAtWithCapacity(m_UsagesForEachControl, ref count, num);
				m_UsageCount--;
				if (m_UsageCount == 0)
				{
					m_UsageStartIndex = 0;
				}
			}
		}

		internal void ClearDeviceUsages()
		{
			for (int i = m_UsageStartIndex; i < m_UsageCount; i++)
			{
				m_UsagesForEachControl[i] = default(InternedString);
			}
			m_UsageCount = 0;
		}

		internal bool RequestReset()
		{
			RequestResetCommand command = RequestResetCommand.Create();
			return base.device.ExecuteCommand(ref command) >= 0;
		}

		internal void NotifyAdded()
		{
			OnAdded();
		}

		internal void NotifyRemoved()
		{
			OnRemoved();
		}

		internal static TDevice Build<TDevice>(string layoutName = null, string layoutVariants = null, InputDeviceDescription deviceDescription = default(InputDeviceDescription)) where TDevice : InputDevice
		{
			if (string.IsNullOrEmpty(layoutName))
			{
				layoutName = InputControlLayout.s_Layouts.TryFindLayoutForType(typeof(TDevice));
				if (string.IsNullOrEmpty(layoutName))
				{
					layoutName = typeof(TDevice).Name;
				}
			}
			using (InputDeviceBuilder.Ref())
			{
				InputDeviceBuilder.instance.Setup(new InternedString(layoutName), new InternedString(layoutVariants), deviceDescription);
				InputDevice inputDevice = InputDeviceBuilder.instance.Finish();
				TDevice result;
				if ((result = inputDevice as TDevice) == null)
				{
					throw new ArgumentException("Expected device of type '" + typeof(TDevice).Name + "' but got device of type '" + inputDevice.GetType().Name + "' instead", "TDevice");
				}
				return result;
			}
		}
	}
}
