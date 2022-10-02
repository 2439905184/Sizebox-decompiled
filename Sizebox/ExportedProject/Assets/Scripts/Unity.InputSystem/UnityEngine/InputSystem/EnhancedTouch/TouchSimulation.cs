using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	[AddComponentMenu("Input/Debug/Touch Simulation")]
	[ExecuteInEditMode]
	public class TouchSimulation : MonoBehaviour, IInputStateChangeMonitor
	{
		private struct SimulatedTouch
		{
			public int sourceIndex;

			public int buttonIndex;

			public int touchId;
		}

		[NonSerialized]
		private int m_NumSources;

		[NonSerialized]
		private Pointer[] m_Sources;

		[NonSerialized]
		private Vector2[] m_CurrentPositions;

		[NonSerialized]
		private SimulatedTouch[] m_Touches;

		[NonSerialized]
		private int m_LastTouchId;

		[NonSerialized]
		private int m_PrimaryTouchIndex = -1;

		internal static TouchSimulation s_Instance;

		public Touchscreen simulatedTouchscreen { get; private set; }

		public static TouchSimulation instance
		{
			get
			{
				return s_Instance;
			}
		}

		public static void Enable()
		{
			if (instance == null)
			{
				GameObject obj = new GameObject();
				obj.SetActive(false);
				obj.hideFlags = HideFlags.HideAndDontSave;
				s_Instance = obj.AddComponent<TouchSimulation>();
				instance.gameObject.SetActive(true);
			}
			instance.enabled = true;
		}

		public static void Disable()
		{
			if (instance != null)
			{
				instance.enabled = false;
			}
		}

		public static void Destroy()
		{
			Disable();
			if (s_Instance != null)
			{
				Object.Destroy(s_Instance.gameObject);
				s_Instance = null;
			}
		}

		protected void AddPointer(Pointer pointer)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			if (!ArrayHelpers.ContainsReference(m_Sources, m_NumSources, pointer))
			{
				int count = m_NumSources;
				ArrayHelpers.AppendWithCapacity(ref m_CurrentPositions, ref count, Vector2.zero);
				int startIndex = ArrayHelpers.AppendWithCapacity(ref m_Sources, ref m_NumSources, pointer);
				InstallStateChangeMonitors(startIndex);
			}
		}

		protected void RemovePointer(Pointer pointer)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			int num = m_Sources.IndexOfReference(pointer, m_NumSources);
			if (num == -1)
			{
				return;
			}
			UninstallStateChangeMonitors(num);
			for (int i = 0; i < m_Touches.Length; i++)
			{
				if (m_Touches[i].touchId != 0 && m_Touches[i].sourceIndex == num)
				{
					bool num2 = m_PrimaryTouchIndex == i;
					TouchState state = new TouchState
					{
						phase = TouchPhase.Canceled,
						position = m_CurrentPositions[num],
						touchId = m_Touches[i].touchId
					};
					if (num2)
					{
						InputState.Change(simulatedTouchscreen.primaryTouch, state);
						m_PrimaryTouchIndex = -1;
					}
					InputState.Change(simulatedTouchscreen.touches[i], state);
					m_Touches[i].touchId = 0;
					m_Touches[i].sourceIndex = 0;
				}
			}
			int count = m_NumSources;
			ArrayHelpers.EraseAtWithCapacity(m_CurrentPositions, ref count, num);
			ArrayHelpers.EraseAtWithCapacity(m_Sources, ref m_NumSources, num);
			if (num != m_NumSources)
			{
				InstallStateChangeMonitors(num);
			}
		}

		protected void InstallStateChangeMonitors(int startIndex = 0)
		{
			for (int i = startIndex; i < m_NumSources; i++)
			{
				Pointer obj = m_Sources[i];
				InputState.AddChangeMonitor(obj.position, this, i);
				int num = 0;
				foreach (InputControl allControl in obj.allControls)
				{
					ButtonControl buttonControl;
					if ((buttonControl = allControl as ButtonControl) != null && !buttonControl.synthetic)
					{
						InputState.AddChangeMonitor(buttonControl, this, (long)(((ulong)(uint)num << 32) | (uint)i));
						num++;
					}
				}
			}
		}

		protected void UninstallStateChangeMonitors(int startIndex = 0)
		{
			for (int i = startIndex; i < m_NumSources; i++)
			{
				Pointer obj = m_Sources[i];
				InputState.RemoveChangeMonitor(obj.position, this, i);
				int num = 0;
				foreach (InputControl allControl in obj.allControls)
				{
					ButtonControl buttonControl;
					if ((buttonControl = allControl as ButtonControl) != null && !buttonControl.synthetic)
					{
						InputState.RemoveChangeMonitor(buttonControl, this, (long)(((ulong)(uint)num << 32) | (uint)i));
						num++;
					}
				}
			}
		}

		protected void OnSourceControlChangedValue(InputControl control, double time, InputEventPtr eventPtr, long sourceDeviceAndButtonIndex)
		{
			long num = sourceDeviceAndButtonIndex & 0xFFFFFFFFu;
			if (num < 0 && num >= m_NumSources)
			{
				throw new ArgumentOutOfRangeException("sourceDeviceIndex", string.Format("Index {0} out of range; have {1} sources", num, m_NumSources));
			}
			ButtonControl buttonControl;
			if ((buttonControl = control as ButtonControl) != null)
			{
				int num2 = (int)(sourceDeviceAndButtonIndex >> 32);
				if (buttonControl.isPressed)
				{
					for (int i = 0; i < m_Touches.Length; i++)
					{
						if (m_Touches[i].touchId == 0)
						{
							int touchId = ++m_LastTouchId;
							m_Touches[i] = new SimulatedTouch
							{
								touchId = touchId,
								buttonIndex = num2,
								sourceIndex = (int)num
							};
							bool flag = m_PrimaryTouchIndex == -1;
							Vector2 vector = m_CurrentPositions[num];
							TouchState touchState = simulatedTouchscreen.touches[i].ReadValue();
							TouchState touchState2 = default(TouchState);
							touchState2.touchId = touchId;
							touchState2.position = vector;
							touchState2.phase = TouchPhase.Began;
							touchState2.startTime = time;
							touchState2.startPosition = vector;
							touchState2.isPrimaryTouch = flag;
							touchState2.tapCount = touchState.tapCount;
							TouchState state = touchState2;
							if (flag)
							{
								InputState.Change(simulatedTouchscreen.primaryTouch, state, InputUpdateType.None, eventPtr);
								m_PrimaryTouchIndex = i;
							}
							InputState.Change(simulatedTouchscreen.touches[i], state, InputUpdateType.None, eventPtr);
							break;
						}
					}
					return;
				}
				for (int j = 0; j < m_Touches.Length; j++)
				{
					if (m_Touches[j].buttonIndex == num2 && m_Touches[j].sourceIndex == num && m_Touches[j].touchId != 0)
					{
						Vector2 vector2 = m_CurrentPositions[num];
						TouchState touchState3 = simulatedTouchscreen.touches[j].ReadValue();
						bool flag2 = time - touchState3.startTime <= (double)Touchscreen.s_TapTime && (vector2 - touchState3.startPosition).sqrMagnitude <= Touchscreen.s_TapRadiusSquared;
						TouchState touchState2 = default(TouchState);
						touchState2.touchId = m_Touches[j].touchId;
						touchState2.phase = TouchPhase.Ended;
						touchState2.position = vector2;
						touchState2.tapCount = (byte)(touchState3.tapCount + (flag2 ? 1 : 0));
						touchState2.isTap = flag2;
						touchState2.startPosition = touchState3.startPosition;
						touchState2.startTime = touchState3.startTime;
						TouchState state2 = touchState2;
						if (m_PrimaryTouchIndex == j)
						{
							InputState.Change(simulatedTouchscreen.primaryTouch, state2, InputUpdateType.None, eventPtr);
							m_PrimaryTouchIndex = -1;
						}
						InputState.Change(simulatedTouchscreen.touches[j], state2, InputUpdateType.None, eventPtr);
						m_Touches[j].touchId = 0;
						break;
					}
				}
				return;
			}
			Vector2 vector3 = ((InputControl<Vector2>)control).ReadValue();
			Vector2 delta = vector3 - m_CurrentPositions[num];
			m_CurrentPositions[num] = vector3;
			for (int k = 0; k < m_Touches.Length; k++)
			{
				if (m_Touches[k].sourceIndex == num && m_Touches[k].touchId != 0)
				{
					TouchState touchState4 = simulatedTouchscreen.touches[k].ReadValue();
					bool flag3 = m_PrimaryTouchIndex == k;
					TouchState touchState2 = default(TouchState);
					touchState2.touchId = m_Touches[k].touchId;
					touchState2.phase = TouchPhase.Moved;
					touchState2.position = vector3;
					touchState2.delta = delta;
					touchState2.isPrimaryTouch = flag3;
					touchState2.tapCount = touchState4.tapCount;
					touchState2.isTap = false;
					touchState2.startPosition = touchState4.startPosition;
					touchState2.startTime = touchState4.startTime;
					TouchState state3 = touchState2;
					if (flag3)
					{
						InputState.Change(simulatedTouchscreen.primaryTouch, state3, InputUpdateType.None, eventPtr);
					}
					InputState.Change(simulatedTouchscreen.touches[k], state3, InputUpdateType.None, eventPtr);
				}
			}
		}

		void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long monitorIndex)
		{
			OnSourceControlChangedValue(control, time, eventPtr, monitorIndex);
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long monitorIndex, int timerIndex)
		{
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (device == simulatedTouchscreen && change == InputDeviceChange.Removed)
			{
				Disable();
				return;
			}
			switch (change)
			{
			case InputDeviceChange.Added:
			{
				Pointer pointer2;
				if ((pointer2 = device as Pointer) != null && !(device is Touchscreen))
				{
					AddPointer(pointer2);
				}
				break;
			}
			case InputDeviceChange.Removed:
			{
				Pointer pointer;
				if ((pointer = device as Pointer) != null)
				{
					RemovePointer(pointer);
				}
				break;
			}
			}
		}

		protected void OnEnable()
		{
			if (simulatedTouchscreen != null)
			{
				if (!simulatedTouchscreen.added)
				{
					InputSystem.AddDevice(simulatedTouchscreen);
				}
			}
			else
			{
				simulatedTouchscreen = InputSystem.GetDevice("Simulated Touchscreen") as Touchscreen;
				if (simulatedTouchscreen == null)
				{
					simulatedTouchscreen = InputSystem.AddDevice<Touchscreen>("Simulated Touchscreen");
				}
			}
			if (m_Touches == null)
			{
				m_Touches = new SimulatedTouch[simulatedTouchscreen.touches.Count];
			}
			foreach (InputDevice device in InputSystem.devices)
			{
				OnDeviceChange(device, InputDeviceChange.Added);
			}
			InputSystem.onDeviceChange += OnDeviceChange;
		}

		protected void OnDisable()
		{
			if (simulatedTouchscreen != null && simulatedTouchscreen.added)
			{
				InputSystem.RemoveDevice(simulatedTouchscreen);
			}
			UninstallStateChangeMonitors();
			m_Sources.Clear(m_NumSources);
			m_CurrentPositions.Clear(m_NumSources);
			m_Touches.Clear();
			m_NumSources = 0;
			m_LastTouchId = 0;
			m_PrimaryTouchIndex = -1;
			InputSystem.onDeviceChange -= OnDeviceChange;
		}
	}
}
