using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal class InputManager
	{
		[Serializable]
		internal struct AvailableDevice
		{
			public InputDeviceDescription description;

			public int deviceId;

			public bool isNative;

			public bool isRemoved;
		}

		internal struct StateChangeMonitorListener
		{
			public InputControl control;

			public IInputStateChangeMonitor monitor;

			public long monitorIndex;
		}

		internal struct StateChangeMonitorsForDevice
		{
			public MemoryHelpers.BitRegion[] memoryRegions;

			public StateChangeMonitorListener[] listeners;

			public DynamicBitfield signalled;

			public int count
			{
				get
				{
					return signalled.length;
				}
			}

			public void Add(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex)
			{
				int length = signalled.length;
				ArrayHelpers.AppendWithCapacity(ref listeners, ref length, new StateChangeMonitorListener
				{
					monitor = monitor,
					monitorIndex = monitorIndex,
					control = control
				});
				ref InputStateBlock stateBlock = ref control.m_StateBlock;
				int length2 = signalled.length;
				ArrayHelpers.AppendWithCapacity(ref memoryRegions, ref length2, new MemoryHelpers.BitRegion(stateBlock.byteOffset - control.device.stateBlock.byteOffset, stateBlock.bitOffset, stateBlock.sizeInBits));
				signalled.SetLength(signalled.length + 1);
			}

			public void Remove(IInputStateChangeMonitor monitor, long monitorIndex)
			{
				if (listeners == null)
				{
					return;
				}
				for (int i = 0; i < signalled.length; i++)
				{
					if (listeners[i].monitor == monitor && listeners[i].monitorIndex == monitorIndex)
					{
						listeners[i] = default(StateChangeMonitorListener);
						memoryRegions[i] = default(MemoryHelpers.BitRegion);
						signalled.ClearBit(i);
						break;
					}
				}
			}

			public void Clear()
			{
				listeners.Clear(count);
				signalled.SetLength(0);
			}
		}

		private struct StateChangeMonitorTimeout
		{
			public InputControl control;

			public double time;

			public IInputStateChangeMonitor monitor;

			public long monitorIndex;

			public int timerIndex;
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<InputDevice, string> _003C_003E9__141_0;

			internal string _003CMakeDeviceNameUnique_003Eb__141_0(InputDevice x)
			{
				if (x == null)
				{
					return string.Empty;
				}
				return x.name;
			}
		}

		internal int m_LayoutRegistrationVersion;

		private float m_PollingFrequency;

		internal InputControlLayout.Collection m_Layouts;

		private TypeTable m_Processors;

		private TypeTable m_Interactions;

		private TypeTable m_Composites;

		private int m_DevicesCount;

		private InputDevice[] m_Devices;

		private Dictionary<int, InputDevice> m_DevicesById;

		internal int m_AvailableDeviceCount;

		internal AvailableDevice[] m_AvailableDevices;

		internal int m_DisconnectedDevicesCount;

		internal InputDevice[] m_DisconnectedDevices;

		private InputUpdateType m_UpdateMask;

		internal InputStateBuffers m_StateBuffers;

		private InlinedArray<Action<InputDevice, InputDeviceChange>> m_DeviceChangeListeners;

		private InlinedArray<Action<InputDevice, InputEventPtr>> m_DeviceStateChangeListeners;

		private InlinedArray<InputDeviceFindControlLayoutDelegate> m_DeviceFindLayoutCallbacks;

		internal InlinedArray<InputDeviceCommandDelegate> m_DeviceCommandCallbacks;

		private InlinedArray<Action<string, InputControlLayoutChange>> m_LayoutChangeListeners;

		private InlinedArray<Action<InputEventPtr, InputDevice>> m_EventListeners;

		private InlinedArray<Action> m_BeforeUpdateListeners;

		private InlinedArray<Action> m_AfterUpdateListeners;

		private InlinedArray<Action> m_SettingsChangedListeners;

		private bool m_NativeBeforeUpdateHooked;

		private bool m_HaveDevicesWithStateCallbackReceivers;

		private bool m_HasFocus;

		private InputDeviceExecuteCommandDelegate m_DeviceFindExecuteCommandDelegate;

		private int m_DeviceFindExecuteCommandDeviceId;

		internal IInputRuntime m_Runtime;

		internal InputMetrics m_Metrics;

		internal InputSettings m_Settings;

		internal StateChangeMonitorsForDevice[] m_StateChangeMonitors;

		private InlinedArray<StateChangeMonitorTimeout> m_StateChangeMonitorTimeouts;

		public ReadOnlyArray<InputDevice> devices
		{
			get
			{
				return new ReadOnlyArray<InputDevice>(m_Devices, 0, m_DevicesCount);
			}
		}

		public TypeTable processors
		{
			get
			{
				return m_Processors;
			}
		}

		public TypeTable interactions
		{
			get
			{
				return m_Interactions;
			}
		}

		public TypeTable composites
		{
			get
			{
				return m_Composites;
			}
		}

		public InputMetrics metrics
		{
			get
			{
				InputMetrics result = m_Metrics;
				result.currentNumDevices = m_DevicesCount;
				result.currentStateSizeInBytes = (int)m_StateBuffers.totalSize;
				result.currentControlCount = m_DevicesCount;
				for (int i = 0; i < m_DevicesCount; i++)
				{
					result.currentControlCount += m_Devices[i].allControls.Count;
				}
				result.currentLayoutCount = m_Layouts.layoutTypes.Count;
				result.currentLayoutCount += m_Layouts.layoutStrings.Count;
				result.currentLayoutCount += m_Layouts.layoutBuilders.Count;
				result.currentLayoutCount += m_Layouts.layoutOverrides.Count;
				return result;
			}
		}

		public InputSettings settings
		{
			get
			{
				return m_Settings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!(m_Settings == value))
				{
					m_Settings = value;
					ApplySettings();
				}
			}
		}

		public InputUpdateType updateMask
		{
			get
			{
				return m_UpdateMask;
			}
			set
			{
				if (m_UpdateMask != value)
				{
					m_UpdateMask = value;
					if (m_DevicesCount > 0)
					{
						ReallocateStateBuffers();
					}
				}
			}
		}

		public InputUpdateType defaultUpdateType
		{
			get
			{
				if ((m_UpdateMask & InputUpdateType.Manual) != 0)
				{
					return InputUpdateType.Manual;
				}
				if ((m_UpdateMask & InputUpdateType.Dynamic) != 0)
				{
					return InputUpdateType.Dynamic;
				}
				if ((m_UpdateMask & InputUpdateType.Fixed) != 0)
				{
					return InputUpdateType.Fixed;
				}
				return InputUpdateType.None;
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
				if (value <= 0f)
				{
					throw new ArgumentException("Polling frequency must be greater than zero", "value");
				}
				m_PollingFrequency = value;
				if (m_Runtime != null)
				{
					m_Runtime.pollingFrequency = value;
				}
			}
		}

		private bool gameIsPlayingAndHasFocus
		{
			get
			{
				return true;
			}
		}

		public event Action<InputDevice, InputDeviceChange> onDeviceChange
		{
			add
			{
				m_DeviceChangeListeners.AppendWithCapacity(value);
			}
			remove
			{
				int num = m_DeviceChangeListeners.IndexOf(value);
				if (num >= 0)
				{
					m_DeviceChangeListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action<InputDevice, InputEventPtr> onDeviceStateChange
		{
			add
			{
				m_DeviceStateChangeListeners.AppendWithCapacity(value);
			}
			remove
			{
				int num = m_DeviceStateChangeListeners.IndexOf(value);
				if (num >= 0)
				{
					m_DeviceStateChangeListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public unsafe event InputDeviceCommandDelegate onDeviceCommand
		{
			add
			{
				m_DeviceCommandCallbacks.Append(value);
			}
			remove
			{
				int num = m_DeviceCommandCallbacks.IndexOf(value);
				if (num >= 0)
				{
					m_DeviceCommandCallbacks.RemoveAtWithCapacity(num);
				}
			}
		}

		public event InputDeviceFindControlLayoutDelegate onFindControlLayoutForDevice
		{
			add
			{
				m_DeviceFindLayoutCallbacks.AppendWithCapacity(value);
				AddAvailableDevicesThatAreNowRecognized();
			}
			remove
			{
				int num = m_DeviceFindLayoutCallbacks.IndexOf(value);
				if (num >= 0)
				{
					m_DeviceFindLayoutCallbacks.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action<string, InputControlLayoutChange> onLayoutChange
		{
			add
			{
				m_LayoutChangeListeners.AppendWithCapacity(value);
			}
			remove
			{
				int num = m_LayoutChangeListeners.IndexOf(value);
				if (num >= 0)
				{
					m_LayoutChangeListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action<InputEventPtr, InputDevice> onEvent
		{
			add
			{
				if (!m_EventListeners.Contains(value))
				{
					m_EventListeners.AppendWithCapacity(value);
				}
			}
			remove
			{
				int num = m_EventListeners.IndexOf(value);
				if (num >= 0)
				{
					m_EventListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action onBeforeUpdate
		{
			add
			{
				InstallBeforeUpdateHookIfNecessary();
				if (!m_BeforeUpdateListeners.Contains(value))
				{
					m_BeforeUpdateListeners.AppendWithCapacity(value);
				}
			}
			remove
			{
				int num = m_BeforeUpdateListeners.IndexOf(value);
				if (num >= 0)
				{
					m_BeforeUpdateListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action onAfterUpdate
		{
			add
			{
				if (!m_AfterUpdateListeners.Contains(value))
				{
					m_AfterUpdateListeners.AppendWithCapacity(value);
				}
			}
			remove
			{
				int num = m_AfterUpdateListeners.IndexOf(value);
				if (num >= 0)
				{
					m_AfterUpdateListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public event Action onSettingsChange
		{
			add
			{
				if (!m_SettingsChangedListeners.Contains(value))
				{
					m_SettingsChangedListeners.AppendWithCapacity(value);
				}
			}
			remove
			{
				int num = m_SettingsChangedListeners.IndexOf(value);
				if (num >= 0)
				{
					m_SettingsChangedListeners.RemoveAtWithCapacity(num);
				}
			}
		}

		public void RegisterControlLayout(string name, Type type)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = typeof(InputDevice).IsAssignableFrom(type);
			bool flag2 = typeof(InputControl).IsAssignableFrom(type);
			if (!flag && !flag2)
			{
				throw new ArgumentException("Types used as layouts have to be InputControls or InputDevices; '" + type.Name + "' is a '" + type.BaseType.Name + "'", "type");
			}
			InternedString internedString = new InternedString(name);
			bool isReplacement = DoesLayoutExist(internedString);
			m_Layouts.layoutTypes[internedString] = type;
			string text = null;
			Type baseType = type.BaseType;
			while (text == null && baseType != typeof(InputControl))
			{
				foreach (KeyValuePair<InternedString, Type> layoutType in m_Layouts.layoutTypes)
				{
					if (layoutType.Value == baseType)
					{
						text = layoutType.Key;
						break;
					}
				}
				baseType = baseType.BaseType;
			}
			PerformLayoutPostRegistration(internedString, new InlinedArray<InternedString>(new InternedString(text)), isReplacement, flag);
		}

		public void RegisterControlLayout(string json, string name = null, bool isOverride = false)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			InternedString name2;
			InlinedArray<InternedString> baseLayouts;
			InputDeviceMatcher deviceMatcher;
			InputControlLayout.ParseHeaderFieldsFromJson(json, out name2, out baseLayouts, out deviceMatcher);
			InternedString internedString = new InternedString(name);
			if (internedString.IsEmpty())
			{
				internedString = name2;
				if (internedString.IsEmpty())
				{
					throw new ArgumentException("Layout name has not been given and is not set in JSON layout", "name");
				}
			}
			if (isOverride && baseLayouts.length == 0)
			{
				throw new ArgumentException(string.Format("Layout override '{0}' must have 'extend' property mentioning layout to which to apply the overrides", internedString), "json");
			}
			bool isReplacement = DoesLayoutExist(internedString);
			m_Layouts.layoutStrings[internedString] = json;
			if (isOverride)
			{
				m_Layouts.layoutOverrideNames.Add(internedString);
				for (int i = 0; i < baseLayouts.length; i++)
				{
					InternedString key = baseLayouts[i];
					InternedString[] value;
					m_Layouts.layoutOverrides.TryGetValue(key, out value);
					ArrayHelpers.Append(ref value, internedString);
					m_Layouts.layoutOverrides[key] = value;
				}
			}
			PerformLayoutPostRegistration(internedString, baseLayouts, isReplacement, false, isOverride);
			if (!deviceMatcher.empty)
			{
				RegisterControlLayoutMatcher(internedString, deviceMatcher);
			}
		}

		public void RegisterControlLayoutBuilder(Func<InputControlLayout> method, string name, string baseLayout = null)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			InternedString internedString = new InternedString(name);
			InternedString value = new InternedString(baseLayout);
			bool isReplacement = DoesLayoutExist(internedString);
			m_Layouts.layoutBuilders[internedString] = method;
			PerformLayoutPostRegistration(internedString, new InlinedArray<InternedString>(value), isReplacement);
		}

		private void PerformLayoutPostRegistration(InternedString layoutName, InlinedArray<InternedString> baseLayouts, bool isReplacement, bool isKnownToBeDeviceLayout = false, bool isOverride = false)
		{
			m_LayoutRegistrationVersion++;
			InputControlLayout.s_CacheInstance.Clear();
			if (!isOverride && baseLayouts.length > 0)
			{
				if (baseLayouts.length > 1)
				{
					throw new NotSupportedException(string.Format("Layout '{0}' has multiple base layouts; this is only supported on layout overrides", layoutName));
				}
				InternedString value = baseLayouts[0];
				if (!value.IsEmpty())
				{
					m_Layouts.baseLayoutTable[layoutName] = value;
				}
			}
			if (isOverride)
			{
				for (int i = 0; i < baseLayouts.length; i++)
				{
					RecreateDevicesUsingLayout(baseLayouts[i], isKnownToBeDeviceLayout);
				}
			}
			else
			{
				RecreateDevicesUsingLayout(layoutName, isKnownToBeDeviceLayout);
			}
			InputControlLayoutChange arg = (isReplacement ? InputControlLayoutChange.Replaced : InputControlLayoutChange.Added);
			for (int j = 0; j < m_LayoutChangeListeners.length; j++)
			{
				m_LayoutChangeListeners[j](layoutName.ToString(), arg);
			}
		}

		private void RecreateDevicesUsingLayout(InternedString layout, bool isKnownToBeDeviceLayout = false)
		{
			if (m_DevicesCount == 0)
			{
				return;
			}
			List<InputDevice> list = null;
			for (int i = 0; i < m_DevicesCount; i++)
			{
				InputDevice inputDevice = m_Devices[i];
				if ((!isKnownToBeDeviceLayout) ? IsControlOrChildUsingLayoutRecursive(inputDevice, layout) : IsControlUsingLayout(inputDevice, layout))
				{
					if (list == null)
					{
						list = new List<InputDevice>();
					}
					list.Add(inputDevice);
				}
			}
			if (list == null)
			{
				return;
			}
			using (InputDeviceBuilder.Ref())
			{
				for (int j = 0; j < list.Count; j++)
				{
					InputDevice inputDevice2 = list[j];
					RecreateDevice(inputDevice2, inputDevice2.m_Layout);
				}
			}
		}

		private bool IsControlOrChildUsingLayoutRecursive(InputControl control, InternedString layout)
		{
			if (IsControlUsingLayout(control, layout))
			{
				return true;
			}
			ReadOnlyArray<InputControl> children = control.children;
			for (int i = 0; i < children.Count; i++)
			{
				if (IsControlOrChildUsingLayoutRecursive(children[i], layout))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsControlUsingLayout(InputControl control, InternedString layout)
		{
			if (control.layout == layout)
			{
				return true;
			}
			InternedString value = control.m_Layout;
			while (m_Layouts.baseLayoutTable.TryGetValue(value, out value))
			{
				if (value == layout)
				{
					return true;
				}
			}
			return false;
		}

		public void RegisterControlLayoutMatcher(string layoutName, InputDeviceMatcher matcher)
		{
			if (string.IsNullOrEmpty(layoutName))
			{
				throw new ArgumentNullException("layoutName");
			}
			if (matcher.empty)
			{
				throw new ArgumentException("Matcher cannot be empty", "matcher");
			}
			InternedString layout = new InternedString(layoutName);
			m_Layouts.AddMatcher(layout, matcher);
			RecreateDevicesUsingLayoutWithInferiorMatch(matcher);
			AddAvailableDevicesMatchingDescription(matcher, layout);
		}

		public void RegisterControlLayoutMatcher(Type type, InputDeviceMatcher matcher)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (matcher.empty)
			{
				throw new ArgumentException("Matcher cannot be empty", "matcher");
			}
			InternedString internedString = m_Layouts.TryFindLayoutForType(type);
			if (internedString.IsEmpty())
			{
				throw new ArgumentException("Type '" + type.Name + "' has not been registered as a control layout", "type");
			}
			RegisterControlLayoutMatcher(internedString, matcher);
		}

		private void RecreateDevicesUsingLayoutWithInferiorMatch(InputDeviceMatcher deviceMatcher)
		{
			if (m_DevicesCount == 0)
			{
				return;
			}
			using (InputDeviceBuilder.Ref())
			{
				int num = m_DevicesCount;
				for (int i = 0; i < num; i++)
				{
					InputDevice inputDevice = m_Devices[i];
					InputDeviceDescription deviceDescription = inputDevice.description;
					if (!deviceDescription.empty && deviceMatcher.MatchPercentage(deviceDescription) > 0f)
					{
						InternedString internedString = TryFindMatchingControlLayout(ref deviceDescription, inputDevice.deviceId);
						if (internedString != inputDevice.m_Layout)
						{
							inputDevice.m_Description = deviceDescription;
							RecreateDevice(inputDevice, internedString);
							i--;
							num--;
						}
					}
				}
			}
		}

		private void RecreateDevice(InputDevice oldDevice, InternedString newLayout)
		{
			RemoveDevice(oldDevice, true);
			InputDevice inputDevice = InputDevice.Build<InputDevice>(newLayout, oldDevice.m_Variants, oldDevice.m_Description);
			inputDevice.m_DeviceId = oldDevice.m_DeviceId;
			inputDevice.m_Description = oldDevice.m_Description;
			if (oldDevice.native)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Native;
			}
			if (oldDevice.remote)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Remote;
			}
			if (!oldDevice.enabled)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledStateHasBeenQueried;
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Disabled;
			}
			AddDevice(inputDevice);
		}

		private void AddAvailableDevicesMatchingDescription(InputDeviceMatcher matcher, InternedString layout)
		{
			for (int i = 0; i < m_AvailableDeviceCount; i++)
			{
				if (m_AvailableDevices[i].isRemoved)
				{
					continue;
				}
				int deviceId = m_AvailableDevices[i].deviceId;
				if (TryGetDeviceById(deviceId) == null && matcher.MatchPercentage(m_AvailableDevices[i].description) > 0f)
				{
					try
					{
						AddDevice(layout, deviceId, null, m_AvailableDevices[i].description, m_AvailableDevices[i].isNative ? InputDevice.DeviceFlags.Native : ((InputDevice.DeviceFlags)0));
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("Layout '{0}' matches existing device '{1}' but failed to instantiate: {2}", layout, m_AvailableDevices[i].description, ex));
						Debug.LogException(ex);
						continue;
					}
					EnableDeviceCommand command = EnableDeviceCommand.Create();
					m_Runtime.DeviceCommand(deviceId, ref command);
				}
			}
		}

		public void RemoveControlLayout(string name, string @namespace = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (@namespace != null)
			{
				name = @namespace + "::" + name;
			}
			InternedString internedString = new InternedString(name);
			int num = 0;
			while (num < m_DevicesCount)
			{
				InputDevice inputDevice = m_Devices[num];
				if (IsControlOrChildUsingLayoutRecursive(inputDevice, internedString))
				{
					RemoveDevice(inputDevice, true);
				}
				else
				{
					num++;
				}
			}
			m_Layouts.layoutTypes.Remove(internedString);
			m_Layouts.layoutStrings.Remove(internedString);
			m_Layouts.layoutBuilders.Remove(internedString);
			m_Layouts.baseLayoutTable.Remove(internedString);
			for (int i = 0; i < m_LayoutChangeListeners.length; i++)
			{
				m_LayoutChangeListeners[i](name, InputControlLayoutChange.Removed);
			}
		}

		public InputControlLayout TryLoadControlLayout(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!typeof(InputControl).IsAssignableFrom(type))
			{
				throw new ArgumentException("Type '" + type.Name + "' is not an InputControl", "type");
			}
			InternedString name = m_Layouts.TryFindLayoutForType(type);
			if (name.IsEmpty())
			{
				throw new ArgumentException("Type '" + type.Name + "' has not been registered as a control layout", "type");
			}
			return m_Layouts.TryLoadLayout(name);
		}

		public InputControlLayout TryLoadControlLayout(InternedString name)
		{
			return m_Layouts.TryLoadLayout(name);
		}

		public InternedString TryFindMatchingControlLayout(ref InputDeviceDescription deviceDescription, int deviceId = 0)
		{
			InternedString internedString = m_Layouts.TryFindMatchingLayout(deviceDescription);
			if (internedString.IsEmpty() && !string.IsNullOrEmpty(deviceDescription.deviceClass))
			{
				InternedString layoutName = new InternedString(deviceDescription.deviceClass);
				Type controlTypeForLayout = m_Layouts.GetControlTypeForLayout(layoutName);
				if (controlTypeForLayout != null && typeof(InputDevice).IsAssignableFrom(controlTypeForLayout))
				{
					internedString = new InternedString(deviceDescription.deviceClass);
				}
			}
			if (m_DeviceFindLayoutCallbacks.length > 0)
			{
				if (m_DeviceFindExecuteCommandDelegate == null)
				{
					m_DeviceFindExecuteCommandDelegate = _003CTryFindMatchingControlLayout_003Eb__65_0;
				}
				m_DeviceFindExecuteCommandDeviceId = deviceId;
				bool flag = false;
				for (int i = 0; i < m_DeviceFindLayoutCallbacks.length; i++)
				{
					string text = m_DeviceFindLayoutCallbacks[i](ref deviceDescription, internedString, m_DeviceFindExecuteCommandDelegate);
					if (!string.IsNullOrEmpty(text) && !flag)
					{
						internedString = new InternedString(text);
						flag = true;
					}
				}
			}
			return internedString;
		}

		private bool IsDeviceLayoutMarkedAsSupportedInSettings(InternedString layoutName)
		{
			ReadOnlyArray<string> supportedDevices = m_Settings.supportedDevices;
			if (supportedDevices.Count == 0)
			{
				return true;
			}
			for (int i = 0; i < supportedDevices.Count; i++)
			{
				InternedString internedString = new InternedString(supportedDevices[i]);
				if (layoutName == internedString || m_Layouts.IsBasedOn(internedString, layoutName))
				{
					return true;
				}
			}
			return false;
		}

		private bool DoesLayoutExist(InternedString name)
		{
			if (!m_Layouts.layoutTypes.ContainsKey(name) && !m_Layouts.layoutStrings.ContainsKey(name))
			{
				return m_Layouts.layoutBuilders.ContainsKey(name);
			}
			return true;
		}

		public IEnumerable<string> ListControlLayouts(string basedOn = null)
		{
			if (!string.IsNullOrEmpty(basedOn))
			{
				InternedString internedBasedOn = new InternedString(basedOn);
				foreach (KeyValuePair<InternedString, Type> layoutType in m_Layouts.layoutTypes)
				{
					if (m_Layouts.IsBasedOn(internedBasedOn, layoutType.Key))
					{
						yield return layoutType.Key;
					}
				}
				foreach (KeyValuePair<InternedString, string> layoutString in m_Layouts.layoutStrings)
				{
					if (m_Layouts.IsBasedOn(internedBasedOn, layoutString.Key))
					{
						yield return layoutString.Key;
					}
				}
				foreach (KeyValuePair<InternedString, Func<InputControlLayout>> layoutBuilder in m_Layouts.layoutBuilders)
				{
					if (m_Layouts.IsBasedOn(internedBasedOn, layoutBuilder.Key))
					{
						yield return layoutBuilder.Key;
					}
				}
				yield break;
			}
			foreach (KeyValuePair<InternedString, Type> layoutType2 in m_Layouts.layoutTypes)
			{
				yield return layoutType2.Key;
			}
			foreach (KeyValuePair<InternedString, string> layoutString2 in m_Layouts.layoutStrings)
			{
				yield return layoutString2.Key;
			}
			foreach (KeyValuePair<InternedString, Func<InputControlLayout>> layoutBuilder2 in m_Layouts.layoutBuilders)
			{
				yield return layoutBuilder2.Key;
			}
		}

		public int GetControls<TControl>(string path, ref InputControlList<TControl> controls) where TControl : InputControl
		{
			if (string.IsNullOrEmpty(path))
			{
				return 0;
			}
			if (m_DevicesCount == 0)
			{
				return 0;
			}
			int devicesCount = m_DevicesCount;
			int num = 0;
			for (int i = 0; i < devicesCount; i++)
			{
				InputDevice control = m_Devices[i];
				num += InputControlPath.TryFindControls(control, path, 0, ref controls);
			}
			return num;
		}

		public void SetDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if ((device.usages.Count != 1 || !(device.usages[0] == usage)) && (device.usages.Count != 0 || !usage.IsEmpty()))
			{
				device.ClearDeviceUsages();
				if (!usage.IsEmpty())
				{
					device.AddDeviceUsage(usage);
				}
				NotifyUsageChanged(device);
			}
		}

		public void AddDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (usage.IsEmpty())
			{
				throw new ArgumentException("Usage string cannot be empty", "usage");
			}
			if (!device.usages.Contains(usage))
			{
				device.AddDeviceUsage(usage);
				NotifyUsageChanged(device);
			}
		}

		public void RemoveDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (usage.IsEmpty())
			{
				throw new ArgumentException("Usage string cannot be empty", "usage");
			}
			if (device.usages.Contains(usage))
			{
				device.RemoveDeviceUsage(usage);
				NotifyUsageChanged(device);
			}
		}

		private void NotifyUsageChanged(InputDevice device)
		{
			InputActionState.OnDeviceChange(device, InputDeviceChange.UsageChanged);
			for (int i = 0; i < m_DeviceChangeListeners.length; i++)
			{
				m_DeviceChangeListeners[i](device, InputDeviceChange.UsageChanged);
			}
			device.MakeCurrent();
		}

		public InputDevice AddDevice(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			InternedString internedString = m_Layouts.TryFindLayoutForType(type);
			if (internedString.IsEmpty() && internedString.IsEmpty())
			{
				internedString = new InternedString(type.Name);
				RegisterControlLayout(type.Name, type);
			}
			return AddDevice(internedString, name);
		}

		public InputDevice AddDevice(string layout, string name = null, InternedString variants = default(InternedString))
		{
			if (string.IsNullOrEmpty(layout))
			{
				throw new ArgumentNullException("layout");
			}
			InputDevice inputDevice = InputDevice.Build<InputDevice>(layout, variants);
			if (!string.IsNullOrEmpty(name))
			{
				inputDevice.m_Name = new InternedString(name);
			}
			AddDevice(inputDevice);
			return inputDevice;
		}

		private InputDevice AddDevice(InternedString layout, int deviceId, string deviceName = null, InputDeviceDescription deviceDescription = default(InputDeviceDescription), InputDevice.DeviceFlags deviceFlags = (InputDevice.DeviceFlags)0, InternedString variants = default(InternedString))
		{
			InputDevice inputDevice = InputDevice.Build<InputDevice>(new InternedString(layout), deviceDescription: deviceDescription, layoutVariants: variants);
			inputDevice.m_DeviceId = deviceId;
			inputDevice.m_Description = deviceDescription;
			inputDevice.m_DeviceFlags |= deviceFlags;
			if (!string.IsNullOrEmpty(deviceName))
			{
				inputDevice.m_Name = new InternedString(deviceName);
			}
			if (!string.IsNullOrEmpty(deviceDescription.product))
			{
				inputDevice.m_DisplayName = deviceDescription.product;
			}
			AddDevice(inputDevice);
			return inputDevice;
		}

		public void AddDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (string.IsNullOrEmpty(device.layout))
			{
				throw new InvalidOperationException("Device has no associated layout");
			}
			if (ArrayHelpers.Contains(m_Devices, device))
			{
				return;
			}
			MakeDeviceNameUnique(device);
			AssignUniqueDeviceId(device);
			device.m_DeviceIndex = ArrayHelpers.AppendWithCapacity(ref m_Devices, ref m_DevicesCount, device);
			m_DevicesById[device.deviceId] = device;
			device.m_StateBlock.byteOffset = uint.MaxValue;
			ReallocateStateBuffers();
			InitializeDefaultState(device);
			InitializeNoiseMask(device);
			m_Metrics.maxNumDevices = Mathf.Max(m_DevicesCount, m_Metrics.maxNumDevices);
			m_Metrics.maxStateSizeInBytes = Mathf.Max((int)m_StateBuffers.totalSize, m_Metrics.maxStateSizeInBytes);
			for (int i = 0; i < m_AvailableDeviceCount; i++)
			{
				if (m_AvailableDevices[i].deviceId == device.deviceId)
				{
					m_AvailableDevices[i].isRemoved = false;
				}
			}
			InputActionState.OnDeviceChange(device, InputDeviceChange.Added);
			IInputUpdateCallbackReceiver inputUpdateCallbackReceiver;
			if ((inputUpdateCallbackReceiver = device as IInputUpdateCallbackReceiver) != null)
			{
				onBeforeUpdate += inputUpdateCallbackReceiver.OnUpdate;
			}
			if (device is IInputStateCallbackReceiver)
			{
				InstallBeforeUpdateHookIfNecessary();
				device.m_DeviceFlags |= InputDevice.DeviceFlags.HasStateCallbacks;
				m_HaveDevicesWithStateCallbackReceivers = true;
			}
			if (device.updateBeforeRender)
			{
				updateMask |= InputUpdateType.BeforeRender;
			}
			device.NotifyAdded();
			device.MakeCurrent();
			for (int j = 0; j < m_DeviceChangeListeners.length; j++)
			{
				m_DeviceChangeListeners[j](device, InputDeviceChange.Added);
			}
		}

		public InputDevice AddDevice(InputDeviceDescription description)
		{
			return AddDevice(description, true);
		}

		public InputDevice AddDevice(InputDeviceDescription description, bool throwIfNoLayoutFound, string deviceName = null, int deviceId = 0, InputDevice.DeviceFlags deviceFlags = (InputDevice.DeviceFlags)0)
		{
			InternedString layout = TryFindMatchingControlLayout(ref description, deviceId);
			if (layout.IsEmpty())
			{
				if (throwIfNoLayoutFound)
				{
					throw new ArgumentException(string.Format("Cannot find layout matching device description '{0}'", description), "description");
				}
				if (deviceId != 0)
				{
					DisableDeviceCommand command = DisableDeviceCommand.Create();
					m_Runtime.DeviceCommand(deviceId, ref command);
				}
				return null;
			}
			InputDevice inputDevice = AddDevice(layout, deviceId, deviceName, description, deviceFlags);
			inputDevice.m_Description = description;
			return inputDevice;
		}

		public void RemoveDevice(InputDevice device, bool keepOnListOfAvailableDevices = false)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.m_DeviceIndex == -1)
			{
				return;
			}
			RemoveStateChangeMonitors(device);
			int deviceIndex = device.m_DeviceIndex;
			int deviceId = device.deviceId;
			if (deviceIndex < m_StateChangeMonitors.LengthSafe())
			{
				int count = m_StateChangeMonitors.Length;
				ArrayHelpers.EraseAtWithCapacity(m_StateChangeMonitors, ref count, deviceIndex);
			}
			ArrayHelpers.EraseAtWithCapacity(m_Devices, ref m_DevicesCount, deviceIndex);
			m_DevicesById.Remove(deviceId);
			if (m_Devices != null)
			{
				ReallocateStateBuffers();
			}
			else
			{
				m_StateBuffers.FreeAll();
			}
			for (int i = deviceIndex; i < m_DevicesCount; i++)
			{
				m_Devices[i].m_DeviceIndex--;
			}
			device.m_DeviceIndex = -1;
			for (int j = 0; j < m_AvailableDeviceCount; j++)
			{
				if (m_AvailableDevices[j].deviceId == deviceId)
				{
					if (keepOnListOfAvailableDevices)
					{
						m_AvailableDevices[j].isRemoved = true;
					}
					else
					{
						ArrayHelpers.EraseAtWithCapacity(m_AvailableDevices, ref m_AvailableDeviceCount, j);
					}
					break;
				}
			}
			device.BakeOffsetIntoStateBlockRecursive((uint)(0uL - (ulong)device.m_StateBlock.byteOffset));
			InputActionState.OnDeviceChange(device, InputDeviceChange.Removed);
			IInputUpdateCallbackReceiver inputUpdateCallbackReceiver;
			if ((inputUpdateCallbackReceiver = device as IInputUpdateCallbackReceiver) != null)
			{
				onBeforeUpdate -= inputUpdateCallbackReceiver.OnUpdate;
			}
			if (device.updateBeforeRender)
			{
				bool flag = false;
				for (int k = 0; k < m_DevicesCount; k++)
				{
					if (m_Devices[k].updateBeforeRender)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					updateMask &= ~InputUpdateType.BeforeRender;
				}
			}
			device.NotifyRemoved();
			for (int l = 0; l < m_DeviceChangeListeners.length; l++)
			{
				m_DeviceChangeListeners[l](device, InputDeviceChange.Removed);
			}
		}

		public void FlushDisconnectedDevices()
		{
			m_DisconnectedDevices.Clear(m_DisconnectedDevicesCount);
			m_DisconnectedDevicesCount = 0;
		}

		public InputDevice TryGetDevice(string nameOrLayout)
		{
			if (string.IsNullOrEmpty(nameOrLayout))
			{
				throw new ArgumentException("Name is null or empty.", "nameOrLayout");
			}
			if (m_DevicesCount == 0)
			{
				return null;
			}
			string text = nameOrLayout.ToLower();
			for (int i = 0; i < m_DevicesCount; i++)
			{
				InputDevice inputDevice = m_Devices[i];
				if (inputDevice.m_Name.ToLower() == text || inputDevice.m_Layout.ToLower() == text)
				{
					return inputDevice;
				}
			}
			return null;
		}

		public InputDevice GetDevice(string nameOrLayout)
		{
			InputDevice inputDevice = TryGetDevice(nameOrLayout);
			if (inputDevice == null)
			{
				throw new ArgumentException("Cannot find device with name or layout '" + nameOrLayout + "'", "nameOrLayout");
			}
			return inputDevice;
		}

		public InputDevice TryGetDevice(Type layoutType)
		{
			InternedString internedString = m_Layouts.TryFindLayoutForType(layoutType);
			if (internedString.IsEmpty())
			{
				return null;
			}
			return TryGetDevice(internedString);
		}

		public InputDevice TryGetDeviceById(int id)
		{
			InputDevice value;
			if (m_DevicesById.TryGetValue(id, out value))
			{
				return value;
			}
			return null;
		}

		public int GetUnsupportedDevices(List<InputDeviceDescription> descriptions)
		{
			if (descriptions == null)
			{
				throw new ArgumentNullException("descriptions");
			}
			int num = 0;
			for (int i = 0; i < m_AvailableDeviceCount; i++)
			{
				if (TryGetDeviceById(m_AvailableDevices[i].deviceId) == null)
				{
					descriptions.Add(m_AvailableDevices[i].description);
					num++;
				}
			}
			return num;
		}

		public void EnableOrDisableDevice(InputDevice device, bool enable)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.enabled != enable)
			{
				if (!enable)
				{
					device.m_DeviceFlags |= InputDevice.DeviceFlags.Disabled;
				}
				else
				{
					device.m_DeviceFlags &= ~InputDevice.DeviceFlags.Disabled;
				}
				if (enable)
				{
					EnableDeviceCommand command = EnableDeviceCommand.Create();
					device.ExecuteCommand(ref command);
				}
				else
				{
					DisableDeviceCommand command2 = DisableDeviceCommand.Create();
					device.ExecuteCommand(ref command2);
				}
				InputDeviceChange arg = (enable ? InputDeviceChange.Enabled : InputDeviceChange.Disabled);
				for (int i = 0; i < m_DeviceChangeListeners.length; i++)
				{
					m_DeviceChangeListeners[i](device, arg);
				}
			}
		}

		public void AddStateChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex)
		{
			int deviceIndex = control.device.m_DeviceIndex;
			if (m_StateChangeMonitors == null)
			{
				m_StateChangeMonitors = new StateChangeMonitorsForDevice[m_DevicesCount];
			}
			else if (m_StateChangeMonitors.Length <= deviceIndex)
			{
				Array.Resize(ref m_StateChangeMonitors, m_DevicesCount);
			}
			m_StateChangeMonitors[deviceIndex].Add(control, monitor, monitorIndex);
		}

		private void RemoveStateChangeMonitors(InputDevice device)
		{
			if (m_StateChangeMonitors == null)
			{
				return;
			}
			int deviceIndex = device.m_DeviceIndex;
			if (deviceIndex >= m_StateChangeMonitors.Length)
			{
				return;
			}
			m_StateChangeMonitors[deviceIndex].Clear();
			for (int i = 0; i < m_StateChangeMonitorTimeouts.length; i++)
			{
				InputControl control = m_StateChangeMonitorTimeouts[i].control;
				if (((control != null) ? control.device : null) == device)
				{
					m_StateChangeMonitorTimeouts[i] = default(StateChangeMonitorTimeout);
				}
			}
		}

		public void RemoveStateChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex)
		{
			if (m_StateChangeMonitors == null)
			{
				return;
			}
			int deviceIndex = control.device.m_DeviceIndex;
			if (deviceIndex == -1 || deviceIndex >= m_StateChangeMonitors.Length)
			{
				return;
			}
			m_StateChangeMonitors[deviceIndex].Remove(monitor, monitorIndex);
			for (int i = 0; i < m_StateChangeMonitorTimeouts.length; i++)
			{
				if (m_StateChangeMonitorTimeouts[i].monitor == monitor && m_StateChangeMonitorTimeouts[i].monitorIndex == monitorIndex)
				{
					m_StateChangeMonitorTimeouts[i] = default(StateChangeMonitorTimeout);
				}
			}
		}

		public void AddStateChangeMonitorTimeout(InputControl control, IInputStateChangeMonitor monitor, double time, long monitorIndex, int timerIndex)
		{
			m_StateChangeMonitorTimeouts.Append(new StateChangeMonitorTimeout
			{
				control = control,
				time = time,
				monitor = monitor,
				monitorIndex = monitorIndex,
				timerIndex = timerIndex
			});
		}

		public void RemoveStateChangeMonitorTimeout(IInputStateChangeMonitor monitor, long monitorIndex, int timerIndex)
		{
			int length = m_StateChangeMonitorTimeouts.length;
			for (int i = 0; i < length; i++)
			{
				if (m_StateChangeMonitorTimeouts[i].monitor == monitor && m_StateChangeMonitorTimeouts[i].monitorIndex == monitorIndex && m_StateChangeMonitorTimeouts[i].timerIndex == timerIndex)
				{
					m_StateChangeMonitorTimeouts[i] = default(StateChangeMonitorTimeout);
					break;
				}
			}
		}

		public unsafe void QueueEvent(InputEventPtr ptr)
		{
			m_Runtime.QueueEvent(ptr.data);
		}

		public unsafe void QueueEvent<TEvent>(ref TEvent inputEvent) where TEvent : struct, IInputEventTypeInfo
		{
			m_Runtime.QueueEvent((InputEvent*)UnsafeUtility.AddressOf(ref inputEvent));
		}

		public void Update()
		{
			Update(defaultUpdateType);
		}

		public void Update(InputUpdateType updateType)
		{
			m_Runtime.Update(updateType);
		}

		internal void Initialize(IInputRuntime runtime, InputSettings settings)
		{
			m_Settings = settings;
			InitializeData();
			InstallRuntime(runtime);
			InstallGlobals();
			ApplySettings();
		}

		internal void Destroy()
		{
			for (int i = 0; i < m_DevicesCount; i++)
			{
				m_Devices[i].NotifyRemoved();
			}
			m_StateBuffers.FreeAll();
			UninstallGlobals();
			if (m_Settings != null && m_Settings.hideFlags == HideFlags.HideAndDontSave)
			{
				Object.DestroyImmediate(m_Settings);
			}
		}

		internal void InitializeData()
		{
			m_Layouts.Allocate();
			m_Processors.Initialize();
			m_Interactions.Initialize();
			m_Composites.Initialize();
			m_DevicesById = new Dictionary<int, InputDevice>();
			m_UpdateMask = InputUpdateType.Dynamic | InputUpdateType.Fixed;
			m_HasFocus = Application.isFocused;
			m_PollingFrequency = 60f;
			RegisterControlLayout("Axis", typeof(AxisControl));
			RegisterControlLayout("Button", typeof(ButtonControl));
			RegisterControlLayout("DiscreteButton", typeof(DiscreteButtonControl));
			RegisterControlLayout("Key", typeof(KeyControl));
			RegisterControlLayout("Analog", typeof(AxisControl));
			RegisterControlLayout("Integer", typeof(IntegerControl));
			RegisterControlLayout("Digital", typeof(IntegerControl));
			RegisterControlLayout("Double", typeof(DoubleControl));
			RegisterControlLayout("Vector2", typeof(Vector2Control));
			RegisterControlLayout("Vector3", typeof(Vector3Control));
			RegisterControlLayout("Quaternion", typeof(QuaternionControl));
			RegisterControlLayout("Stick", typeof(StickControl));
			RegisterControlLayout("Dpad", typeof(DpadControl));
			RegisterControlLayout("DpadAxis", typeof(DpadControl.DpadAxisControl));
			RegisterControlLayout("AnyKey", typeof(AnyKeyControl));
			RegisterControlLayout("Touch", typeof(TouchControl));
			RegisterControlLayout("TouchPhase", typeof(TouchPhaseControl));
			RegisterControlLayout("TouchPress", typeof(TouchPressControl));
			RegisterControlLayout("Gamepad", typeof(Gamepad));
			RegisterControlLayout("Joystick", typeof(Joystick));
			RegisterControlLayout("Keyboard", typeof(Keyboard));
			RegisterControlLayout("Pointer", typeof(Pointer));
			RegisterControlLayout("Mouse", typeof(Mouse));
			RegisterControlLayout("Pen", typeof(Pen));
			RegisterControlLayout("Touchscreen", typeof(Touchscreen));
			RegisterControlLayout("Sensor", typeof(Sensor));
			RegisterControlLayout("Accelerometer", typeof(Accelerometer));
			RegisterControlLayout("Gyroscope", typeof(Gyroscope));
			RegisterControlLayout("GravitySensor", typeof(GravitySensor));
			RegisterControlLayout("AttitudeSensor", typeof(AttitudeSensor));
			RegisterControlLayout("LinearAccelerationSensor", typeof(LinearAccelerationSensor));
			RegisterControlLayout("MagneticFieldSensor", typeof(MagneticFieldSensor));
			RegisterControlLayout("LightSensor", typeof(LightSensor));
			RegisterControlLayout("PressureSensor", typeof(PressureSensor));
			RegisterControlLayout("HumiditySensor", typeof(HumiditySensor));
			RegisterControlLayout("AmbientTemperatureSensor", typeof(AmbientTemperatureSensor));
			RegisterControlLayout("StepCounter", typeof(StepCounter));
			RegisterControlLayout("TrackedDevice", typeof(TrackedDevice));
			processors.AddTypeRegistration("Invert", typeof(InvertProcessor));
			processors.AddTypeRegistration("InvertVector2", typeof(InvertVector2Processor));
			processors.AddTypeRegistration("InvertVector3", typeof(InvertVector3Processor));
			processors.AddTypeRegistration("Clamp", typeof(ClampProcessor));
			processors.AddTypeRegistration("Normalize", typeof(NormalizeProcessor));
			processors.AddTypeRegistration("NormalizeVector2", typeof(NormalizeVector2Processor));
			processors.AddTypeRegistration("NormalizeVector3", typeof(NormalizeVector3Processor));
			processors.AddTypeRegistration("Scale", typeof(ScaleProcessor));
			processors.AddTypeRegistration("ScaleVector2", typeof(ScaleVector2Processor));
			processors.AddTypeRegistration("ScaleVector3", typeof(ScaleVector3Processor));
			processors.AddTypeRegistration("StickDeadzone", typeof(StickDeadzoneProcessor));
			processors.AddTypeRegistration("AxisDeadzone", typeof(AxisDeadzoneProcessor));
			processors.AddTypeRegistration("CompensateDirection", typeof(CompensateDirectionProcessor));
			processors.AddTypeRegistration("CompensateRotation", typeof(CompensateRotationProcessor));
			interactions.AddTypeRegistration("Hold", typeof(HoldInteraction));
			interactions.AddTypeRegistration("Tap", typeof(TapInteraction));
			interactions.AddTypeRegistration("SlowTap", typeof(SlowTapInteraction));
			interactions.AddTypeRegistration("MultiTap", typeof(MultiTapInteraction));
			interactions.AddTypeRegistration("Press", typeof(PressInteraction));
			composites.AddTypeRegistration("1DAxis", typeof(AxisComposite));
			composites.AddTypeRegistration("2DVector", typeof(Vector2Composite));
			composites.AddTypeRegistration("Axis", typeof(AxisComposite));
			composites.AddTypeRegistration("Dpad", typeof(Vector2Composite));
			composites.AddTypeRegistration("ButtonWithOneModifier", typeof(ButtonWithOneModifier));
			composites.AddTypeRegistration("ButtonWithTwoModifiers", typeof(ButtonWithTwoModifiers));
		}

		internal void InstallRuntime(IInputRuntime runtime)
		{
			if (m_Runtime != null)
			{
				m_Runtime.onUpdate = null;
				m_Runtime.onBeforeUpdate = null;
				m_Runtime.onDeviceDiscovered = null;
				m_Runtime.onPlayerFocusChanged = null;
				m_Runtime.onShouldRunUpdate = null;
			}
			m_Runtime = runtime;
			m_Runtime.onUpdate = OnUpdate;
			m_Runtime.onDeviceDiscovered = OnNativeDeviceDiscovered;
			m_Runtime.onPlayerFocusChanged = OnFocusChanged;
			m_Runtime.onShouldRunUpdate = ShouldRunUpdate;
			m_Runtime.pollingFrequency = pollingFrequency;
			if (m_BeforeUpdateListeners.length > 0 || m_HaveDevicesWithStateCallbackReceivers)
			{
				m_Runtime.onBeforeUpdate = OnBeforeUpdate;
				m_NativeBeforeUpdateHooked = true;
			}
		}

		internal unsafe void InstallGlobals()
		{
			InputControlLayout.s_Layouts = m_Layouts;
			InputProcessor.s_Processors = m_Processors;
			InputInteraction.s_Interactions = m_Interactions;
			InputBindingComposite.s_Composites = m_Composites;
			InputRuntime.s_Instance = m_Runtime;
			InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup = m_Runtime.currentTimeOffsetToRealtimeSinceStartup;
			InputUpdate.Restore(default(InputUpdate.SerializedState));
			InputStateBuffers.SwitchTo(m_StateBuffers, InputUpdateType.Dynamic);
			InputStateBuffers.s_DefaultStateBuffer = m_StateBuffers.defaultStateBuffer;
			InputStateBuffers.s_NoiseMaskBuffer = m_StateBuffers.noiseMaskBuffer;
		}

		internal void UninstallGlobals()
		{
			if (InputControlLayout.s_Layouts.baseLayoutTable == m_Layouts.baseLayoutTable)
			{
				InputControlLayout.s_Layouts = default(InputControlLayout.Collection);
			}
			if (InputProcessor.s_Processors.table == m_Processors.table)
			{
				InputProcessor.s_Processors = default(TypeTable);
			}
			if (InputInteraction.s_Interactions.table == m_Interactions.table)
			{
				InputInteraction.s_Interactions = default(TypeTable);
			}
			if (InputBindingComposite.s_Composites.table == m_Composites.table)
			{
				InputBindingComposite.s_Composites = default(TypeTable);
			}
			InputControlLayout.s_CacheInstance = default(InputControlLayout.Cache);
			InputControlLayout.s_CacheInstanceRef = 0;
			if (m_Runtime != null)
			{
				m_Runtime.onUpdate = null;
				m_Runtime.onDeviceDiscovered = null;
				m_Runtime.onBeforeUpdate = null;
				m_Runtime.onPlayerFocusChanged = null;
				m_Runtime.onShouldRunUpdate = null;
				if (InputRuntime.s_Instance == m_Runtime)
				{
					InputRuntime.s_Instance = null;
				}
			}
		}

		private void MakeDeviceNameUnique(InputDevice device)
		{
			if (m_DevicesCount != 0)
			{
				string text = StringHelpers.MakeUniqueName(device.name, m_Devices, _003C_003Ec._003C_003E9__141_0 ?? (_003C_003Ec._003C_003E9__141_0 = _003C_003Ec._003C_003E9._003CMakeDeviceNameUnique_003Eb__141_0));
				if (text != device.name)
				{
					ResetControlPathsRecursive(device);
					device.m_Name = new InternedString(text);
				}
			}
		}

		private static void ResetControlPathsRecursive(InputControl control)
		{
			control.m_Path = null;
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				ResetControlPathsRecursive(children[i]);
			}
		}

		private void AssignUniqueDeviceId(InputDevice device)
		{
			if (device.deviceId != 0)
			{
				InputDevice inputDevice = TryGetDeviceById(device.deviceId);
				if (inputDevice != null)
				{
					throw new InvalidOperationException(string.Format("Duplicate device ID {0} detected for devices '{1}' and '{2}'", device.deviceId, device.name, inputDevice.name));
				}
			}
			else
			{
				device.m_DeviceId = m_Runtime.AllocateDeviceId();
			}
		}

		private unsafe void ReallocateStateBuffers()
		{
			InputStateBuffers stateBuffers = m_StateBuffers;
			InputStateBuffers stateBuffers2 = default(InputStateBuffers);
			stateBuffers2.AllocateAll(m_Devices, m_DevicesCount);
			stateBuffers2.MigrateAll(m_Devices, m_DevicesCount, stateBuffers);
			stateBuffers.FreeAll();
			m_StateBuffers = stateBuffers2;
			InputStateBuffers.s_DefaultStateBuffer = stateBuffers2.defaultStateBuffer;
			InputStateBuffers.s_NoiseMaskBuffer = stateBuffers2.noiseMaskBuffer;
			InputStateBuffers.SwitchTo(m_StateBuffers, (InputUpdate.s_LastUpdateType != 0) ? InputUpdate.s_LastUpdateType : defaultUpdateType);
		}

		private unsafe void InitializeDefaultState(InputDevice device)
		{
			if (!device.hasControlsWithDefaultState)
			{
				return;
			}
			ReadOnlyArray<InputControl> allControls = device.allControls;
			int count = allControls.Count;
			void* defaultStateBuffer = m_StateBuffers.defaultStateBuffer;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = allControls[i];
				if (inputControl.hasDefaultState)
				{
					inputControl.m_StateBlock.Write(defaultStateBuffer, inputControl.m_DefaultState);
				}
			}
			InputStateBlock stateBlock = device.m_StateBlock;
			int deviceIndex = device.m_DeviceIndex;
			if (m_StateBuffers.m_PlayerStateBuffers.valid)
			{
				stateBlock.CopyToFrom(m_StateBuffers.m_PlayerStateBuffers.GetFrontBuffer(deviceIndex), defaultStateBuffer);
				stateBlock.CopyToFrom(m_StateBuffers.m_PlayerStateBuffers.GetBackBuffer(deviceIndex), defaultStateBuffer);
			}
		}

		private unsafe void InitializeNoiseMask(InputDevice device)
		{
			ReadOnlyArray<InputControl> allControls = device.allControls;
			int count = allControls.Count;
			void* noiseMaskBuffer = m_StateBuffers.noiseMaskBuffer;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = allControls[i];
				if (!inputControl.noisy)
				{
					ref InputStateBlock stateBlock = ref inputControl.m_StateBlock;
					MemoryHelpers.SetBitsInBuffer(noiseMaskBuffer, (int)stateBlock.byteOffset, (int)stateBlock.bitOffset, (int)stateBlock.sizeInBits, true);
				}
			}
		}

		private void OnNativeDeviceDiscovered(int deviceId, string deviceDescriptor)
		{
			RestoreDevicesAfterDomainReloadIfNecessary();
			InputDevice inputDevice = TryMatchDisconnectedDevice(deviceDescriptor);
			InputDeviceDescription deviceDescription = ((inputDevice != null) ? inputDevice.description : InputDeviceDescription.FromJson(deviceDescriptor));
			bool isRemoved = false;
			try
			{
				if (m_Settings.supportedDevices.Count > 0)
				{
					InternedString layoutName = ((inputDevice != null) ? inputDevice.m_Layout : TryFindMatchingControlLayout(ref deviceDescription, deviceId));
					if (!IsDeviceLayoutMarkedAsSupportedInSettings(layoutName))
					{
						isRemoved = true;
						return;
					}
				}
				if (inputDevice != null)
				{
					inputDevice.m_DeviceId = deviceId;
					AddDevice(inputDevice);
					for (int i = 0; i < m_DeviceChangeListeners.length; i++)
					{
						m_DeviceChangeListeners[i](inputDevice, InputDeviceChange.Reconnected);
					}
				}
				else
				{
					AddDevice(deviceDescription, false, null, deviceId, InputDevice.DeviceFlags.Native);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Could not create a device for '{0}' (exception: {1})", deviceDescription, arg));
			}
			finally
			{
				ArrayHelpers.AppendWithCapacity(ref m_AvailableDevices, ref m_AvailableDeviceCount, new AvailableDevice
				{
					description = deviceDescription,
					deviceId = deviceId,
					isNative = true,
					isRemoved = isRemoved
				});
			}
		}

		private InputDevice TryMatchDisconnectedDevice(string deviceDescriptor)
		{
			for (int i = 0; i < m_DisconnectedDevicesCount; i++)
			{
				InputDevice inputDevice = m_DisconnectedDevices[i];
				InputDeviceDescription description = inputDevice.description;
				if ((string.IsNullOrEmpty(description.interfaceName) || InputDeviceDescription.ComparePropertyToDeviceDescriptor("interface", description.interfaceName, deviceDescriptor)) && (string.IsNullOrEmpty(description.product) || InputDeviceDescription.ComparePropertyToDeviceDescriptor("product", description.product, deviceDescriptor)) && (string.IsNullOrEmpty(description.manufacturer) || InputDeviceDescription.ComparePropertyToDeviceDescriptor("manufacturer", description.manufacturer, deviceDescriptor)) && (string.IsNullOrEmpty(description.deviceClass) || InputDeviceDescription.ComparePropertyToDeviceDescriptor("type", description.deviceClass, deviceDescriptor)))
				{
					ArrayHelpers.EraseAtWithCapacity(m_DisconnectedDevices, ref m_DisconnectedDevicesCount, i);
					return inputDevice;
				}
			}
			return null;
		}

		private void InstallBeforeUpdateHookIfNecessary()
		{
			if (!m_NativeBeforeUpdateHooked && m_Runtime != null)
			{
				m_Runtime.onBeforeUpdate = OnBeforeUpdate;
				m_NativeBeforeUpdateHooked = true;
			}
		}

		private void RestoreDevicesAfterDomainReloadIfNecessary()
		{
		}

		private void WarnAboutDevicesFailingToRecreateAfterDomainReload()
		{
		}

		private void OnBeforeUpdate(InputUpdateType updateType)
		{
			RestoreDevicesAfterDomainReloadIfNecessary();
			if ((updateType & m_UpdateMask) == 0)
			{
				return;
			}
			InputStateBuffers.SwitchTo(m_StateBuffers, updateType);
			if (m_HaveDevicesWithStateCallbackReceivers && updateType != InputUpdateType.BeforeRender)
			{
				InputUpdate.s_LastUpdateType = updateType;
				for (int i = 0; i < m_DevicesCount; i++)
				{
					InputDevice inputDevice = m_Devices[i];
					if ((inputDevice.m_DeviceFlags & InputDevice.DeviceFlags.HasStateCallbacks) != 0)
					{
						((IInputStateCallbackReceiver)inputDevice).OnNextUpdate();
					}
				}
			}
			DelegateHelpers.InvokeCallbacksSafe(ref m_BeforeUpdateListeners, "onBeforeUpdate");
		}

		internal void ApplySettings()
		{
			InputUpdateType inputUpdateType = InputUpdateType.Editor;
			if ((m_UpdateMask & InputUpdateType.BeforeRender) != 0)
			{
				inputUpdateType |= InputUpdateType.BeforeRender;
			}
			if (m_Settings.updateMode == (InputSettings.UpdateMode)0)
			{
				m_Settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
			}
			switch (m_Settings.updateMode)
			{
			case InputSettings.UpdateMode.ProcessEventsInDynamicUpdate:
				inputUpdateType |= InputUpdateType.Dynamic;
				break;
			case InputSettings.UpdateMode.ProcessEventsInFixedUpdate:
				inputUpdateType |= InputUpdateType.Fixed;
				break;
			case InputSettings.UpdateMode.ProcessEventsManually:
				inputUpdateType |= InputUpdateType.Manual;
				break;
			default:
				throw new NotSupportedException("Invalid input update mode: " + m_Settings.updateMode);
			}
			updateMask = inputUpdateType;
			AddAvailableDevicesThatAreNowRecognized();
			if (settings.supportedDevices.Count > 0)
			{
				for (int i = 0; i < m_DevicesCount; i++)
				{
					InputDevice inputDevice = m_Devices[i];
					InternedString layout = inputDevice.m_Layout;
					bool flag = false;
					for (int j = 0; j < m_AvailableDeviceCount; j++)
					{
						if (m_AvailableDevices[j].deviceId == inputDevice.deviceId)
						{
							flag = true;
							break;
						}
					}
					if (flag && !IsDeviceLayoutMarkedAsSupportedInSettings(layout))
					{
						RemoveDevice(inputDevice, true);
						i--;
					}
				}
			}
			Touchscreen.s_TapTime = settings.defaultTapTime;
			Touchscreen.s_TapDelayTime = settings.multiTapDelayTime;
			Touchscreen.s_TapRadiusSquared = settings.tapRadius * settings.tapRadius;
			ButtonControl.s_GlobalDefaultButtonPressPoint = settings.defaultButtonPressPoint;
			for (int k = 0; k < m_SettingsChangedListeners.length; k++)
			{
				m_SettingsChangedListeners[k]();
			}
		}

		internal void AddAvailableDevicesThatAreNowRecognized()
		{
			for (int i = 0; i < m_AvailableDeviceCount; i++)
			{
				int deviceId = m_AvailableDevices[i].deviceId;
				if (TryGetDeviceById(deviceId) != null)
				{
					continue;
				}
				InternedString layoutName = TryFindMatchingControlLayout(ref m_AvailableDevices[i].description, deviceId);
				if (IsDeviceLayoutMarkedAsSupportedInSettings(layoutName))
				{
					try
					{
						AddDevice(m_AvailableDevices[i].description, false, null, deviceId, m_AvailableDevices[i].isNative ? InputDevice.DeviceFlags.Native : ((InputDevice.DeviceFlags)0));
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private unsafe void OnFocusChanged(bool focus)
		{
			if (!focus)
			{
				bool runInBackground = m_Runtime.runInBackground;
				int num = 0;
				int devicesCount = m_DevicesCount;
				for (int i = 0; i < devicesCount; i++)
				{
					num = Math.Max(num, (int)m_Devices[i].m_StateBlock.alignedSizeInBytes);
				}
				using (NativeArray<byte> nativeArray = new NativeArray<byte>(24 + num, Allocator.Temp))
				{
					StateEvent* unsafePtr = (StateEvent*)nativeArray.GetUnsafePtr();
					void* state = unsafePtr->state;
					double currentTime = m_Runtime.currentTime;
					InputUpdateType updateType = defaultUpdateType;
					for (int j = 0; j < devicesCount; j++)
					{
						InputDevice inputDevice = m_Devices[j];
						if (inputDevice.enabled && (!runInBackground || !inputDevice.canRunInBackground))
						{
							ref InputStateBlock stateBlock = ref inputDevice.m_StateBlock;
							uint alignedSizeInBytes = stateBlock.alignedSizeInBytes;
							unsafePtr->baseEvent.type = 1398030676;
							unsafePtr->baseEvent.sizeInBytes = 24 + alignedSizeInBytes;
							unsafePtr->baseEvent.time = currentTime;
							unsafePtr->baseEvent.deviceId = inputDevice.deviceId;
							unsafePtr->baseEvent.eventId = -1;
							unsafePtr->stateFormat = inputDevice.m_StateBlock.format;
							void* defaultStatePtr = inputDevice.defaultStatePtr;
							if (inputDevice.noisy)
							{
								void* currentStatePtr = inputDevice.currentStatePtr;
								void* noiseMaskPtr = inputDevice.noiseMaskPtr;
								UnsafeUtility.MemCpy(state, (byte*)currentStatePtr + stateBlock.byteOffset, alignedSizeInBytes);
								MemoryHelpers.MemCpyMasked(state, (byte*)defaultStatePtr + stateBlock.byteOffset, (int)alignedSizeInBytes, (byte*)noiseMaskPtr + stateBlock.byteOffset);
							}
							else
							{
								UnsafeUtility.MemCpy(state, (byte*)defaultStatePtr + stateBlock.byteOffset, alignedSizeInBytes);
							}
							UpdateState(inputDevice, updateType, state, 0u, alignedSizeInBytes, currentTime, new InputEventPtr((InputEvent*)unsafePtr));
							inputDevice.RequestReset();
						}
					}
				}
			}
			m_HasFocus = focus;
		}

		private bool ShouldRunUpdate(InputUpdateType updateType)
		{
			if (updateType == InputUpdateType.None)
			{
				return true;
			}
			InputUpdateType inputUpdateType = m_UpdateMask;
			return (updateType & inputUpdateType) != 0;
		}

		private unsafe void OnUpdate(InputUpdateType updateType, ref InputEventBuffer eventBuffer)
		{
			RestoreDevicesAfterDomainReloadIfNecessary();
			if ((updateType & m_UpdateMask) == 0)
			{
				return;
			}
			WarnAboutDevicesFailingToRecreateAfterDomainReload();
			m_Metrics.totalEventCount += eventBuffer.eventCount - (int)InputUpdate.s_LastUpdateRetainedEventCount;
			m_Metrics.totalEventBytes += (int)eventBuffer.sizeInBytes - (int)InputUpdate.s_LastUpdateRetainedEventBytes;
			ref InputMetrics reference = ref m_Metrics;
			int totalUpdateCount = reference.totalUpdateCount + 1;
			reference.totalUpdateCount = totalUpdateCount;
			InputUpdate.s_LastUpdateRetainedEventCount = 0u;
			InputUpdate.s_LastUpdateRetainedEventBytes = 0u;
			InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup = m_Runtime.currentTimeOffsetToRealtimeSinceStartup;
			InputUpdate.s_LastUpdateType = updateType;
			InputStateBuffers.SwitchTo(m_StateBuffers, updateType);
			bool flag = false;
			switch (updateType)
			{
			case InputUpdateType.Dynamic:
			case InputUpdateType.Fixed:
			case InputUpdateType.Manual:
				InputUpdate.s_UpdateStepCount++;
				break;
			case InputUpdateType.BeforeRender:
				flag = true;
				break;
			}
			double num = ((updateType == InputUpdateType.Fixed) ? m_Runtime.currentTimeForFixedUpdate : m_Runtime.currentTime);
			bool flag2 = gameIsPlayingAndHasFocus && InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
			if (eventBuffer.eventCount <= 0)
			{
				if (gameIsPlayingAndHasFocus)
				{
					ProcessStateChangeMonitorTimeouts();
				}
				InvokeAfterUpdateCallback();
				eventBuffer.Reset();
				return;
			}
			InputEvent* currentReadPos = (InputEvent*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(eventBuffer.data);
			int numRemainingEvents = eventBuffer.eventCount;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			InputEvent* currentWritePos = currentReadPos;
			int numEventsRetainedInBuffer = 0;
			double num2 = 0.0;
			while (numRemainingEvents > 0)
			{
				InputDevice inputDevice = null;
				if (flag)
				{
					while (numRemainingEvents > 0)
					{
						inputDevice = TryGetDeviceById(currentReadPos->deviceId);
						if (inputDevice != null && inputDevice.updateBeforeRender && (currentReadPos->type == 1398030676 || currentReadPos->type == 1145852993))
						{
							break;
						}
						eventBuffer.AdvanceToNextEvent(ref currentReadPos, ref currentWritePos, ref numEventsRetainedInBuffer, ref numRemainingEvents, true);
					}
				}
				if (numRemainingEvents == 0)
				{
					break;
				}
				double internalTime = currentReadPos->internalTime;
				if (flag2 && internalTime >= num)
				{
					eventBuffer.AdvanceToNextEvent(ref currentReadPos, ref currentWritePos, ref numEventsRetainedInBuffer, ref numRemainingEvents, true);
					continue;
				}
				if (internalTime <= num)
				{
					num2 += num - internalTime;
				}
				if (inputDevice == null)
				{
					inputDevice = TryGetDeviceById(currentReadPos->deviceId);
				}
				if (inputDevice == null)
				{
					eventBuffer.AdvanceToNextEvent(ref currentReadPos, ref currentWritePos, ref numEventsRetainedInBuffer, ref numRemainingEvents, false);
					continue;
				}
				if (m_EventListeners.length > 0)
				{
					for (int i = 0; i < m_EventListeners.length; i++)
					{
						m_EventListeners[i](new InputEventPtr(currentReadPos), inputDevice);
					}
					if (currentReadPos->handled)
					{
						eventBuffer.AdvanceToNextEvent(ref currentReadPos, ref currentWritePos, ref numEventsRetainedInBuffer, ref numRemainingEvents, false);
						continue;
					}
				}
				switch ((int)currentReadPos->type)
				{
				case 1145852993:
				case 1398030676:
				{
					InputEventPtr inputEventPtr = new InputEventPtr(currentReadPos);
					if (!inputDevice.enabled)
					{
						break;
					}
					bool flag3 = (inputDevice.m_DeviceFlags & InputDevice.DeviceFlags.HasStateCallbacks) == InputDevice.DeviceFlags.HasStateCallbacks;
					if (internalTime < inputDevice.m_LastUpdateTimeInternal && (!flag3 || !(inputDevice.stateBlock.format != inputEventPtr.stateFormat)))
					{
						break;
					}
					bool flag4 = true;
					if (flag3)
					{
						((IInputStateCallbackReceiver)inputDevice).OnStateEvent(inputEventPtr);
					}
					else
					{
						if (inputDevice.stateBlock.format != inputEventPtr.stateFormat)
						{
							break;
						}
						flag4 = UpdateState(inputDevice, inputEventPtr, updateType);
					}
					if (inputDevice.m_LastUpdateTimeInternal <= inputEventPtr.internalTime)
					{
						inputDevice.m_LastUpdateTimeInternal = inputEventPtr.internalTime;
					}
					if (flag4)
					{
						inputDevice.MakeCurrent();
					}
					break;
				}
				case 1413830740:
				{
					TextEvent* ptr = (TextEvent*)currentReadPos;
					ITextInputReceiver textInputReceiver;
					if ((textInputReceiver = inputDevice as ITextInputReceiver) != null)
					{
						int character = ptr->character;
						if (character >= 65536)
						{
							character -= 65536;
							int num3 = 55296 + ((character >> 10) & 0x3FF);
							int num4 = 56320 + (character & 0x3FF);
							textInputReceiver.OnTextInput((char)num3);
							textInputReceiver.OnTextInput((char)num4);
						}
						else
						{
							textInputReceiver.OnTextInput((char)character);
						}
					}
					break;
				}
				case 1229800787:
				{
					IMECompositionEvent* ptr2 = (IMECompositionEvent*)currentReadPos;
					ITextInputReceiver obj = inputDevice as ITextInputReceiver;
					if (obj != null)
					{
						obj.OnIMECompositionChanged(ptr2->compositionString);
					}
					break;
				}
				case 1146242381:
					RemoveDevice(inputDevice);
					if (inputDevice.native && !inputDevice.description.empty)
					{
						ArrayHelpers.AppendWithCapacity(ref m_DisconnectedDevices, ref m_DisconnectedDevicesCount, inputDevice);
						for (int k = 0; k < m_DeviceChangeListeners.length; k++)
						{
							m_DeviceChangeListeners[k](inputDevice, InputDeviceChange.Disconnected);
						}
					}
					break;
				case 1145259591:
				{
					inputDevice.OnConfigurationChanged();
					InputActionState.OnDeviceChange(inputDevice, InputDeviceChange.ConfigurationChanged);
					for (int j = 0; j < m_DeviceChangeListeners.length; j++)
					{
						m_DeviceChangeListeners[j](inputDevice, InputDeviceChange.ConfigurationChanged);
					}
					break;
				}
				}
				eventBuffer.AdvanceToNextEvent(ref currentReadPos, ref currentWritePos, ref numEventsRetainedInBuffer, ref numRemainingEvents, false);
			}
			m_Metrics.totalEventProcessingTime += Time.realtimeSinceStartup - realtimeSinceStartup;
			m_Metrics.totalEventLagTime += num2;
			InputUpdate.s_LastUpdateRetainedEventCount = (uint)numEventsRetainedInBuffer;
			InputUpdate.s_LastUpdateRetainedEventBytes = (uint)((byte*)currentWritePos - (byte*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(eventBuffer.data));
			if (numEventsRetainedInBuffer > 0)
			{
				void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(eventBuffer.data);
				long num5 = (byte*)currentWritePos - (byte*)unsafeBufferPointerWithoutChecks;
				eventBuffer = new InputEventBuffer((InputEvent*)unsafeBufferPointerWithoutChecks, numEventsRetainedInBuffer, (int)num5, (int)eventBuffer.capacityInBytes);
			}
			else
			{
				eventBuffer.Reset();
			}
			if (gameIsPlayingAndHasFocus)
			{
				ProcessStateChangeMonitorTimeouts();
			}
			InvokeAfterUpdateCallback();
		}

		private void InvokeAfterUpdateCallback()
		{
			for (int i = 0; i < m_AfterUpdateListeners.length; i++)
			{
				m_AfterUpdateListeners[i]();
			}
		}

		private unsafe bool ProcessStateChangeMonitors(int deviceIndex, void* newStateFromEvent, void* oldStateOfDevice, uint newStateSizeInBytes, uint newStateOffsetInBytes)
		{
			if (m_StateChangeMonitors == null)
			{
				return false;
			}
			if (deviceIndex >= m_StateChangeMonitors.Length)
			{
				return false;
			}
			MemoryHelpers.BitRegion[] memoryRegions = m_StateChangeMonitors[deviceIndex].memoryRegions;
			if (memoryRegions == null)
			{
				return false;
			}
			int num = m_StateChangeMonitors[deviceIndex].count;
			bool result = false;
			DynamicBitfield signalled = m_StateChangeMonitors[deviceIndex].signalled;
			bool flag = false;
			MemoryHelpers.BitRegion bitRegion = new MemoryHelpers.BitRegion(newStateOffsetInBytes, 0u, newStateSizeInBytes * 8);
			for (int i = 0; i < num; i++)
			{
				MemoryHelpers.BitRegion other = memoryRegions[i];
				if (other.sizeInBits == 0)
				{
					int count = num;
					int count2 = num;
					ArrayHelpers.EraseAtWithCapacity(m_StateChangeMonitors[deviceIndex].listeners, ref count, i);
					ArrayHelpers.EraseAtWithCapacity(memoryRegions, ref count2, i);
					signalled.SetLength(num - 1);
					flag = true;
					num--;
					i--;
				}
				else
				{
					MemoryHelpers.BitRegion region = bitRegion.Overlap(other);
					if (!region.isEmpty && !MemoryHelpers.Compare(oldStateOfDevice, (byte*)newStateFromEvent - newStateOffsetInBytes, region))
					{
						signalled.SetBit(i);
						flag = true;
						result = true;
					}
				}
			}
			if (flag)
			{
				m_StateChangeMonitors[deviceIndex].signalled = signalled;
			}
			return result;
		}

		private unsafe void FireStateChangeNotifications(int deviceIndex, double internalTime, InputEvent* eventPtr)
		{
			ref DynamicBitfield signalled = ref m_StateChangeMonitors[deviceIndex].signalled;
			ref StateChangeMonitorListener[] listeners = ref m_StateChangeMonitors[deviceIndex].listeners;
			double time = internalTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			for (int i = 0; i < signalled.length; i++)
			{
				if (signalled.TestBit(i))
				{
					StateChangeMonitorListener stateChangeMonitorListener = listeners[i];
					try
					{
						stateChangeMonitorListener.monitor.NotifyControlStateChanged(stateChangeMonitorListener.control, time, eventPtr, stateChangeMonitorListener.monitorIndex);
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("Exception '{0}' thrown from state change monitor '{1}' on '{2}'", ex.GetType().Name, stateChangeMonitorListener.monitor.GetType().Name, stateChangeMonitorListener.control));
						Debug.LogException(ex);
					}
					signalled.ClearBit(i);
				}
			}
		}

		private void ProcessStateChangeMonitorTimeouts()
		{
			if (m_StateChangeMonitorTimeouts.length == 0)
			{
				return;
			}
			double num = m_Runtime.currentTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			int num2 = 0;
			for (int i = 0; i < m_StateChangeMonitorTimeouts.length; i++)
			{
				if (m_StateChangeMonitorTimeouts[i].control == null)
				{
					continue;
				}
				if (m_StateChangeMonitorTimeouts[i].time <= num)
				{
					StateChangeMonitorTimeout stateChangeMonitorTimeout = m_StateChangeMonitorTimeouts[i];
					stateChangeMonitorTimeout.monitor.NotifyTimerExpired(stateChangeMonitorTimeout.control, num, stateChangeMonitorTimeout.monitorIndex, stateChangeMonitorTimeout.timerIndex);
					continue;
				}
				if (i != num2)
				{
					m_StateChangeMonitorTimeouts[num2] = m_StateChangeMonitorTimeouts[i];
				}
				num2++;
			}
			m_StateChangeMonitorTimeouts.SetLength(num2);
		}

		internal unsafe bool UpdateState(InputDevice device, InputEvent* eventPtr, InputUpdateType updateType)
		{
			InputStateBlock stateBlock = device.m_StateBlock;
			uint num = stateBlock.sizeInBits / 8u;
			uint num2 = 0u;
			byte* statePtr;
			uint num3;
			if (eventPtr->type == 1398030676)
			{
				StateEvent stateEvent = *(StateEvent*)eventPtr;
				uint stateSizeInBytes = ((StateEvent*)eventPtr)->stateSizeInBytes;
				statePtr = (byte*)((StateEvent*)eventPtr)->state;
				num3 = stateSizeInBytes;
				if (num3 > num)
				{
					num3 = num;
				}
			}
			else
			{
				DeltaStateEvent deltaStateEvent = *(DeltaStateEvent*)eventPtr;
				uint deltaStateSizeInBytes = ((DeltaStateEvent*)eventPtr)->deltaStateSizeInBytes;
				statePtr = (byte*)((DeltaStateEvent*)eventPtr)->deltaState;
				num2 = ((DeltaStateEvent*)eventPtr)->stateOffset;
				num3 = deltaStateSizeInBytes;
				if (num2 + num3 > num)
				{
					if (num2 >= num)
					{
						return false;
					}
					num3 = num - num2;
				}
			}
			return UpdateState(device, updateType, statePtr, num2, num3, eventPtr->internalTime, eventPtr);
		}

		internal unsafe bool UpdateState(InputDevice device, InputUpdateType updateType, void* statePtr, uint stateOffsetInDevice, uint stateSize, double internalTime, InputEventPtr eventPtr = default(InputEventPtr))
		{
			int deviceIndex = device.m_DeviceIndex;
			ref InputStateBlock stateBlock = ref device.m_StateBlock;
			byte* frontBufferForDevice = (byte*)InputStateBuffers.GetFrontBufferForDevice(deviceIndex);
			bool flag = ProcessStateChangeMonitors(deviceIndex, statePtr, frontBufferForDevice + stateBlock.byteOffset, stateSize, stateOffsetInDevice);
			uint num = device.m_StateBlock.byteOffset + stateOffsetInDevice;
			byte* ptr = frontBufferForDevice + num;
			bool result = true;
			if (device.noisy && m_Settings.filterNoiseOnCurrent)
			{
				byte* mask = (byte*)InputStateBuffers.s_NoiseMaskBuffer + num;
				result = !MemoryHelpers.MemCmpBitRegion(ptr, statePtr, 0u, stateSize * 8, mask);
			}
			bool flippedBuffers = FlipBuffersForDeviceIfNecessary(device, updateType);
			WriteStateChange(m_StateBuffers.m_PlayerStateBuffers, deviceIndex, ref stateBlock, stateOffsetInDevice, statePtr, stateSize, flippedBuffers);
			for (int i = 0; i < m_DeviceStateChangeListeners.length; i++)
			{
				m_DeviceStateChangeListeners[i](device, eventPtr);
			}
			if (flag)
			{
				FireStateChangeNotifications(deviceIndex, internalTime, eventPtr);
			}
			return result;
		}

		private unsafe static void WriteStateChange(InputStateBuffers.DoubleBuffers buffers, int deviceIndex, ref InputStateBlock deviceStateBlock, uint stateOffsetInDevice, void* statePtr, uint stateSizeInBytes, bool flippedBuffers)
		{
			void* frontBuffer = buffers.GetFrontBuffer(deviceIndex);
			uint num = deviceStateBlock.sizeInBits / 8u;
			if (flippedBuffers && num != stateSizeInBytes)
			{
				void* backBuffer = buffers.GetBackBuffer(deviceIndex);
				UnsafeUtility.MemCpy((byte*)frontBuffer + deviceStateBlock.byteOffset, (byte*)backBuffer + deviceStateBlock.byteOffset, num);
			}
			UnsafeUtility.MemCpy((byte*)frontBuffer + deviceStateBlock.byteOffset + stateOffsetInDevice, statePtr, stateSizeInBytes);
		}

		private bool FlipBuffersForDeviceIfNecessary(InputDevice device, InputUpdateType updateType)
		{
			if (updateType == InputUpdateType.BeforeRender)
			{
				return false;
			}
			if (device.m_CurrentUpdateStepCount != InputUpdate.s_UpdateStepCount)
			{
				m_StateBuffers.m_PlayerStateBuffers.SwapBuffers(device.m_DeviceIndex);
				device.m_CurrentUpdateStepCount = InputUpdate.s_UpdateStepCount;
				return true;
			}
			return false;
		}

		[CompilerGenerated]
		private long _003CTryFindMatchingControlLayout_003Eb__65_0(ref InputDeviceCommand commandRef)
		{
			if (m_DeviceFindExecuteCommandDeviceId == 0)
			{
				return -1L;
			}
			return m_Runtime.DeviceCommand(m_DeviceFindExecuteCommandDeviceId, ref commandRef);
		}
	}
}
