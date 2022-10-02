using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.InputSystem
{
	public static class InputSystem
	{
		private struct StateEventBuffer
		{
			public StateEvent stateEvent;

			public const int kMaxSize = 512;

			public unsafe fixed byte data[511];
		}

		private struct DeltaStateEventBuffer
		{
			public DeltaStateEvent stateEvent;

			public const int kMaxSize = 512;

			public unsafe fixed byte data[511];
		}

		internal static InputManager s_Manager;

		internal static InputRemoting s_Remote;

		public static ReadOnlyArray<InputDevice> devices
		{
			get
			{
				return s_Manager.devices;
			}
		}

		public static ReadOnlyArray<InputDevice> disconnectedDevices
		{
			get
			{
				return new ReadOnlyArray<InputDevice>(s_Manager.m_DisconnectedDevices, 0, s_Manager.m_DisconnectedDevicesCount);
			}
		}

		public static float pollingFrequency
		{
			get
			{
				return s_Manager.pollingFrequency;
			}
			set
			{
				s_Manager.pollingFrequency = value;
			}
		}

		public static InputSettings settings
		{
			get
			{
				return s_Manager.settings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!(s_Manager.m_Settings == value))
				{
					s_Manager.settings = value;
				}
			}
		}

		public static InputRemoting remoting
		{
			get
			{
				return s_Remote;
			}
		}

		public static Version version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		public static InputMetrics metrics
		{
			get
			{
				return s_Manager.metrics;
			}
		}

		public static event Action<string, InputControlLayoutChange> onLayoutChange
		{
			add
			{
				lock (s_Manager)
				{
					s_Manager.onLayoutChange += value;
				}
			}
			remove
			{
				lock (s_Manager)
				{
					s_Manager.onLayoutChange -= value;
				}
			}
		}

		public static event Action<InputDevice, InputDeviceChange> onDeviceChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onDeviceChange += value;
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onDeviceChange -= value;
				}
			}
		}

		public unsafe static event InputDeviceCommandDelegate onDeviceCommand
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onDeviceCommand += value;
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onDeviceCommand -= value;
				}
			}
		}

		public static event InputDeviceFindControlLayoutDelegate onFindLayoutForDevice
		{
			add
			{
				lock (s_Manager)
				{
					s_Manager.onFindControlLayoutForDevice += value;
				}
			}
			remove
			{
				lock (s_Manager)
				{
					s_Manager.onFindControlLayoutForDevice -= value;
				}
			}
		}

		public static event Action<InputEventPtr, InputDevice> onEvent
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onEvent += value;
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					s_Manager.onEvent -= value;
				}
			}
		}

		public static event Action onBeforeUpdate
		{
			add
			{
				lock (s_Manager)
				{
					s_Manager.onBeforeUpdate += value;
				}
			}
			remove
			{
				lock (s_Manager)
				{
					s_Manager.onBeforeUpdate -= value;
				}
			}
		}

		public static event Action onAfterUpdate
		{
			add
			{
				lock (s_Manager)
				{
					s_Manager.onAfterUpdate += value;
				}
			}
			remove
			{
				lock (s_Manager)
				{
					s_Manager.onAfterUpdate -= value;
				}
			}
		}

		public static event Action onSettingsChange
		{
			add
			{
				s_Manager.onSettingsChange += value;
			}
			remove
			{
				s_Manager.onSettingsChange -= value;
			}
		}

		public static event Action<object, InputActionChange> onActionChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					if (!InputActionState.s_OnActionChange.Contains(value))
					{
						InputActionState.s_OnActionChange.Append(value);
					}
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (s_Manager)
				{
					InputActionState.s_OnActionChange.Remove(value);
				}
			}
		}

		public static void RegisterLayout(Type type, string name = null, InputDeviceMatcher? matches = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
			}
			s_Manager.RegisterControlLayout(name, type);
			if (matches.HasValue)
			{
				s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RegisterLayout<T>(string name = null, InputDeviceMatcher? matches = null) where T : InputControl
		{
			RegisterLayout(typeof(T), name, matches);
		}

		public static void RegisterLayout(string json, string name = null, InputDeviceMatcher? matches = null)
		{
			s_Manager.RegisterControlLayout(json, name);
			if (matches.HasValue)
			{
				s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RegisterLayoutOverride(string json, string name = null)
		{
			s_Manager.RegisterControlLayout(json, name, true);
		}

		public static void RegisterLayoutMatcher(string layoutName, InputDeviceMatcher matcher)
		{
			s_Manager.RegisterControlLayoutMatcher(layoutName, matcher);
		}

		public static void RegisterLayoutMatcher<TDevice>(InputDeviceMatcher matcher) where TDevice : InputDevice
		{
			s_Manager.RegisterControlLayoutMatcher(typeof(TDevice), matcher);
		}

		public static void RegisterLayoutBuilder(Func<InputControlLayout> buildMethod, string name, string baseLayout = null, InputDeviceMatcher? matches = null)
		{
			if (buildMethod == null)
			{
				throw new ArgumentNullException("buildMethod");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			s_Manager.RegisterControlLayoutBuilder(buildMethod, name, baseLayout);
			if (matches.HasValue)
			{
				s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RemoveLayout(string name)
		{
			s_Manager.RemoveControlLayout(name);
		}

		public static string TryFindMatchingLayout(InputDeviceDescription deviceDescription)
		{
			return s_Manager.TryFindMatchingControlLayout(ref deviceDescription);
		}

		public static IEnumerable<string> ListLayouts()
		{
			return s_Manager.ListControlLayouts();
		}

		public static IEnumerable<string> ListLayoutsBasedOn(string baseLayout)
		{
			if (string.IsNullOrEmpty(baseLayout))
			{
				throw new ArgumentNullException("baseLayout");
			}
			return s_Manager.ListControlLayouts(baseLayout);
		}

		public static InputControlLayout LoadLayout(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return s_Manager.TryLoadControlLayout(new InternedString(name));
		}

		public static InputControlLayout LoadLayout<TControl>() where TControl : InputControl
		{
			return s_Manager.TryLoadControlLayout(typeof(TControl));
		}

		public static string GetNameOfBaseLayout(string layoutName)
		{
			if (string.IsNullOrEmpty(layoutName))
			{
				throw new ArgumentNullException("layoutName");
			}
			InternedString key = new InternedString(layoutName);
			InternedString value;
			if (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		public static bool IsFirstLayoutBasedOnSecond(string firstLayoutName, string secondLayoutName)
		{
			if (string.IsNullOrEmpty(firstLayoutName))
			{
				throw new ArgumentNullException("firstLayoutName");
			}
			if (string.IsNullOrEmpty(secondLayoutName))
			{
				throw new ArgumentNullException("secondLayoutName");
			}
			InternedString internedString = new InternedString(firstLayoutName);
			InternedString internedString2 = new InternedString(secondLayoutName);
			if (internedString == internedString2)
			{
				return true;
			}
			return InputControlLayout.s_Layouts.IsBasedOn(internedString2, internedString);
		}

		public static void RegisterProcessor(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Processor"))
				{
					name = name.Substring(0, name.Length - "Processor".Length);
				}
			}
			s_Manager.processors.AddTypeRegistration(name, type);
		}

		public static void RegisterProcessor<T>(string name = null)
		{
			RegisterProcessor(typeof(T), name);
		}

		public static Type TryGetProcessor(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return s_Manager.processors.LookupTypeRegistration(name);
		}

		public static IEnumerable<string> ListProcessors()
		{
			return s_Manager.processors.names;
		}

		public static InputDevice AddDevice(string layout, string name = null, string variants = null)
		{
			if (string.IsNullOrEmpty(layout))
			{
				throw new ArgumentNullException("layout");
			}
			return s_Manager.AddDevice(layout, name, new InternedString(variants));
		}

		public static TDevice AddDevice<TDevice>(string name = null) where TDevice : InputDevice
		{
			InputDevice inputDevice = s_Manager.AddDevice(typeof(TDevice), name);
			TDevice obj = inputDevice as TDevice;
			if (obj == null)
			{
				if (inputDevice != null)
				{
					RemoveDevice(inputDevice);
				}
				throw new InvalidOperationException("Layout registered for type '" + typeof(TDevice).Name + "' did not produce a device of that type; layout probably has been overridden");
			}
			return obj;
		}

		public static InputDevice AddDevice(InputDeviceDescription description)
		{
			if (description.empty)
			{
				throw new ArgumentException("Description must not be empty", "description");
			}
			return s_Manager.AddDevice(description);
		}

		public static void AddDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			s_Manager.AddDevice(device);
		}

		public static void RemoveDevice(InputDevice device)
		{
			s_Manager.RemoveDevice(device);
		}

		public static void FlushDisconnectedDevices()
		{
			s_Manager.FlushDisconnectedDevices();
		}

		public static InputDevice GetDevice(string nameOrLayout)
		{
			return s_Manager.TryGetDevice(nameOrLayout);
		}

		public static TDevice GetDevice<TDevice>() where TDevice : InputDevice
		{
			TDevice val = null;
			double num = -1.0;
			foreach (InputDevice device in devices)
			{
				TDevice val2 = device as TDevice;
				if (val2 != null && (val == null || val2.m_LastUpdateTimeInternal > num))
				{
					val = val2;
					num = val.m_LastUpdateTimeInternal;
				}
			}
			return val;
		}

		public static TDevice GetDevice<TDevice>(InternedString usage) where TDevice : InputDevice
		{
			TDevice val = null;
			double num = -1.0;
			foreach (InputDevice device in devices)
			{
				TDevice val2 = device as TDevice;
				if (val2 != null && val2.usages.Contains(usage) && (val == null || val2.m_LastUpdateTimeInternal > num))
				{
					val = val2;
					num = val.m_LastUpdateTimeInternal;
				}
			}
			return val;
		}

		public static TDevice GetDevice<TDevice>(string usage) where TDevice : InputDevice
		{
			return GetDevice<TDevice>(new InternedString(usage));
		}

		public static InputDevice GetDeviceById(int deviceId)
		{
			return s_Manager.TryGetDeviceById(deviceId);
		}

		public static List<InputDeviceDescription> GetUnsupportedDevices()
		{
			List<InputDeviceDescription> list = new List<InputDeviceDescription>();
			GetUnsupportedDevices(list);
			return list;
		}

		public static int GetUnsupportedDevices(List<InputDeviceDescription> descriptions)
		{
			return s_Manager.GetUnsupportedDevices(descriptions);
		}

		public static void EnableDevice(InputDevice device)
		{
			s_Manager.EnableOrDisableDevice(device, true);
		}

		public static void DisableDevice(InputDevice device)
		{
			s_Manager.EnableOrDisableDevice(device, false);
		}

		public static bool TrySyncDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			RequestSyncCommand command = RequestSyncCommand.Create();
			return device.ExecuteCommand(ref command) >= 0;
		}

		public static bool TryResetDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			return device.RequestReset();
		}

		public static void PauseHaptics()
		{
			ReadOnlyArray<InputDevice> readOnlyArray = devices;
			int count = readOnlyArray.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics;
				if ((haptics = readOnlyArray[i] as IHaptics) != null)
				{
					haptics.PauseHaptics();
				}
			}
		}

		public static void ResumeHaptics()
		{
			ReadOnlyArray<InputDevice> readOnlyArray = devices;
			int count = readOnlyArray.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics;
				if ((haptics = readOnlyArray[i] as IHaptics) != null)
				{
					haptics.ResumeHaptics();
				}
			}
		}

		public static void ResetHaptics()
		{
			ReadOnlyArray<InputDevice> readOnlyArray = devices;
			int count = readOnlyArray.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics;
				if ((haptics = readOnlyArray[i] as IHaptics) != null)
				{
					haptics.ResetHaptics();
				}
			}
		}

		public static void SetDeviceUsage(InputDevice device, string usage)
		{
			SetDeviceUsage(device, new InternedString(usage));
		}

		public static void SetDeviceUsage(InputDevice device, InternedString usage)
		{
			s_Manager.SetDeviceUsage(device, usage);
		}

		public static void AddDeviceUsage(InputDevice device, string usage)
		{
			s_Manager.AddDeviceUsage(device, new InternedString(usage));
		}

		public static void AddDeviceUsage(InputDevice device, InternedString usage)
		{
			s_Manager.AddDeviceUsage(device, usage);
		}

		public static void RemoveDeviceUsage(InputDevice device, string usage)
		{
			s_Manager.RemoveDeviceUsage(device, new InternedString(usage));
		}

		public static void RemoveDeviceUsage(InputDevice device, InternedString usage)
		{
			s_Manager.RemoveDeviceUsage(device, usage);
		}

		public static InputControl FindControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			ReadOnlyArray<InputDevice> readOnlyArray = s_Manager.devices;
			int count = readOnlyArray.Count;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = InputControlPath.TryFindControl(readOnlyArray[i], path);
				if (inputControl != null)
				{
					return inputControl;
				}
			}
			return null;
		}

		public static InputControlList<InputControl> FindControls(string path)
		{
			return FindControls<InputControl>(path);
		}

		public static InputControlList<TControl> FindControls<TControl>(string path) where TControl : InputControl
		{
			InputControlList<TControl> controls = default(InputControlList<TControl>);
			FindControls(path, ref controls);
			return controls;
		}

		public static int FindControls<TControl>(string path, ref InputControlList<TControl> controls) where TControl : InputControl
		{
			return s_Manager.GetControls(path, ref controls);
		}

		public static void QueueEvent(InputEventPtr eventPtr)
		{
			if (!eventPtr.valid)
			{
				throw new ArgumentException("Received a null event pointer", "eventPtr");
			}
			s_Manager.QueueEvent(eventPtr);
		}

		public static void QueueEvent<TEvent>(ref TEvent inputEvent) where TEvent : struct, IInputEventTypeInfo
		{
			s_Manager.QueueEvent(ref inputEvent);
		}

		public unsafe static void QueueStateEvent<TState>(InputDevice device, TState state, double time = -1.0) where TState : struct, IInputStateTypeInfo
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.m_DeviceIndex == -1)
			{
				throw new InvalidOperationException(string.Format("Cannot queue state event for device '{0}' because device has not been added to system", device));
			}
			uint num = (uint)UnsafeUtility.SizeOf<TState>();
			if (num > 512)
			{
				throw new ArgumentException(string.Format("Size of '{0}' exceeds maximum supported state size of {1}", typeof(TState).Name, 512), "state");
			}
			long num2 = UnsafeUtility.SizeOf<StateEvent>() + num - 1;
			time = ((!(time < 0.0)) ? (time + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup) : InputRuntime.s_Instance.currentTime);
			StateEventBuffer stateEventBuffer = default(StateEventBuffer);
			stateEventBuffer.stateEvent = new StateEvent
			{
				baseEvent = new InputEvent(1398030676, (int)num2, device.deviceId, time),
				stateFormat = state.format
			};
			UnsafeUtility.MemCpy(stateEventBuffer.stateEvent.stateData, UnsafeUtility.AddressOf(ref state), num);
			s_Manager.QueueEvent(ref stateEventBuffer.stateEvent);
		}

		public unsafe static void QueueDeltaStateEvent<TDelta>(InputControl control, TDelta delta, double time = -1.0) where TDelta : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (control.stateBlock.bitOffset != 0)
			{
				throw new InvalidOperationException(string.Format("Cannot send delta state events against bitfield controls: {0}", control));
			}
			InputDevice device = control.device;
			if (device.m_DeviceIndex == -1)
			{
				throw new InvalidOperationException(string.Format("Cannot queue state event for control '{0}' on device '{1}' because device has not been added to system", control, device));
			}
			time = ((!(time < 0.0)) ? (time + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup) : InputRuntime.s_Instance.currentTime);
			uint num = (uint)UnsafeUtility.SizeOf<TDelta>();
			if (num > 512)
			{
				throw new ArgumentException(string.Format("Size of state delta '{0}' exceeds maximum supported state size of {1}", typeof(TDelta).Name, 512), "delta");
			}
			if (num != control.stateBlock.alignedSizeInBytes)
			{
				throw new ArgumentException(string.Format("Size {0} of delta state of type {1} provided for control '{2}' does not match size {3} of control", num, typeof(TDelta).Name, control, control.stateBlock.alignedSizeInBytes), "delta");
			}
			long num2 = UnsafeUtility.SizeOf<DeltaStateEvent>() + num - 1;
			DeltaStateEventBuffer deltaStateEventBuffer = default(DeltaStateEventBuffer);
			deltaStateEventBuffer.stateEvent = new DeltaStateEvent
			{
				baseEvent = new InputEvent(1145852993, (int)num2, device.deviceId, time),
				stateFormat = device.stateBlock.format,
				stateOffset = control.m_StateBlock.byteOffset - device.m_StateBlock.byteOffset
			};
			UnsafeUtility.MemCpy(deltaStateEventBuffer.stateEvent.stateData, UnsafeUtility.AddressOf(ref delta), num);
			s_Manager.QueueEvent(ref deltaStateEventBuffer.stateEvent);
		}

		public static void QueueConfigChangeEvent(InputDevice device, double time = -1.0)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.deviceId == 0)
			{
				throw new InvalidOperationException("Device has not been added");
			}
			time = ((!(time < 0.0)) ? (time + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup) : InputRuntime.s_Instance.currentTime);
			DeviceConfigurationEvent inputEvent = DeviceConfigurationEvent.Create(device.deviceId, time);
			s_Manager.QueueEvent(ref inputEvent);
		}

		public static void QueueTextEvent(InputDevice device, char character, double time = -1.0)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.deviceId == 0)
			{
				throw new InvalidOperationException("Device has not been added");
			}
			time = ((!(time < 0.0)) ? (time + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup) : InputRuntime.s_Instance.currentTime);
			TextEvent inputEvent = TextEvent.Create(device.deviceId, character, time);
			s_Manager.QueueEvent(ref inputEvent);
		}

		public static void Update()
		{
			s_Manager.Update();
		}

		internal static void Update(InputUpdateType updateType)
		{
			if (updateType != 0 && (s_Manager.updateMask & updateType) == 0)
			{
				throw new InvalidOperationException(string.Format("'{0}' updates are not enabled; InputSystem.settings.updateMode is set to '{1}'", updateType, settings.updateMode));
			}
			s_Manager.Update(updateType);
		}

		public static void RegisterInteraction(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Interaction"))
				{
					name = name.Substring(0, name.Length - "Interaction".Length);
				}
			}
			s_Manager.interactions.AddTypeRegistration(name, type);
		}

		public static void RegisterInteraction<T>(string name = null)
		{
			RegisterInteraction(typeof(T), name);
		}

		public static Type TryGetInteraction(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return s_Manager.interactions.LookupTypeRegistration(name);
		}

		public static IEnumerable<string> ListInteractions()
		{
			return s_Manager.interactions.names;
		}

		public static void RegisterBindingComposite(Type type, string name)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Composite"))
				{
					name = name.Substring(0, name.Length - "Composite".Length);
				}
			}
			s_Manager.composites.AddTypeRegistration(name, type);
		}

		public static void RegisterBindingComposite<T>(string name = null)
		{
			RegisterBindingComposite(typeof(T), name);
		}

		public static Type TryGetBindingComposite(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return s_Manager.composites.LookupTypeRegistration(name);
		}

		public static void DisableAllEnabledActions()
		{
			InputActionState.DisableAllActions();
		}

		public static List<InputAction> ListEnabledActions()
		{
			List<InputAction> list = new List<InputAction>();
			ListEnabledActions(list);
			return list;
		}

		public static int ListEnabledActions(List<InputAction> actions)
		{
			if (actions == null)
			{
				throw new ArgumentNullException("actions");
			}
			return InputActionState.FindAllEnabledActions(actions);
		}

		static InputSystem()
		{
			InitializeInPlayer();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RunInitializeInPlayer()
		{
			if (s_Manager == null)
			{
				InitializeInPlayer();
			}
		}

		private static void InitializeInPlayer(IInputRuntime runtime = null, InputSettings settings = null)
		{
			if (settings == null)
			{
				settings = Resources.FindObjectsOfTypeAll<InputSettings>().FirstOrDefault() ?? ScriptableObject.CreateInstance<InputSettings>();
			}
			s_Manager = new InputManager();
			s_Manager.Initialize(runtime ?? NativeInputRuntime.instance, settings);
			PerformDefaultPluginInitialization();
			RunInitialUpdate();
		}

		private static void RunInitialUpdate()
		{
			Update(InputUpdateType.None);
		}

		private static void PerformDefaultPluginInitialization()
		{
			UISupport.Initialize();
			XInputSupport.Initialize();
			DualShockSupport.Initialize();
			HIDSupport.Initialize();
			SwitchSupportHID.Initialize();
			XRSupport.Initialize();
		}
	}
}
