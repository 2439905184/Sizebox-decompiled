using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public class InputSettings : ScriptableObject
	{
		public enum UpdateMode
		{
			ProcessEventsInDynamicUpdate = 1,
			ProcessEventsInFixedUpdate = 2,
			ProcessEventsManually = 3
		}

		[Tooltip("Determine which type of devices are used by the application. By default, this is empty meaning that all devices recognized by Unity will be used. Restricting the set of supported devices will make only those devices appear in the input system.")]
		[SerializeField]
		private string[] m_SupportedDevices;

		[Tooltip("Determine when Unity processes events. By default, accumulated input events are flushed out before each fixed update and before each dynamic update. This setting can be used to restrict event processing to only where the application needs it.")]
		[SerializeField]
		private UpdateMode m_UpdateMode = UpdateMode.ProcessEventsInDynamicUpdate;

		[SerializeField]
		private bool m_CompensateForScreenOrientation = true;

		[SerializeField]
		private bool m_FilterNoiseOnCurrent;

		[SerializeField]
		private float m_DefaultDeadzoneMin = 0.125f;

		[SerializeField]
		private float m_DefaultDeadzoneMax = 0.925f;

		[SerializeField]
		private float m_DefaultButtonPressPoint = 0.5f;

		[SerializeField]
		private float m_DefaultTapTime = 0.2f;

		[SerializeField]
		private float m_DefaultSlowTapTime = 0.5f;

		[SerializeField]
		private float m_DefaultHoldTime = 0.4f;

		[SerializeField]
		private float m_TapRadius = 5f;

		[SerializeField]
		private float m_MultiTapDelayTime = 0.75f;

		internal const int s_OldUnsupportedFixedAndDynamicUpdateSetting = 0;

		public UpdateMode updateMode
		{
			get
			{
				return m_UpdateMode;
			}
			set
			{
				if (m_UpdateMode != value)
				{
					m_UpdateMode = value;
					OnChange();
				}
			}
		}

		public bool compensateForScreenOrientation
		{
			get
			{
				return m_CompensateForScreenOrientation;
			}
			set
			{
				if (m_CompensateForScreenOrientation != value)
				{
					m_CompensateForScreenOrientation = value;
					OnChange();
				}
			}
		}

		public bool filterNoiseOnCurrent
		{
			get
			{
				return m_FilterNoiseOnCurrent;
			}
			set
			{
				if (m_FilterNoiseOnCurrent != value)
				{
					m_FilterNoiseOnCurrent = value;
					OnChange();
				}
			}
		}

		public float defaultDeadzoneMin
		{
			get
			{
				return m_DefaultDeadzoneMin;
			}
			set
			{
				if (m_DefaultDeadzoneMin != value)
				{
					m_DefaultDeadzoneMin = value;
					OnChange();
				}
			}
		}

		public float defaultDeadzoneMax
		{
			get
			{
				return m_DefaultDeadzoneMax;
			}
			set
			{
				if (m_DefaultDeadzoneMax != value)
				{
					m_DefaultDeadzoneMax = value;
					OnChange();
				}
			}
		}

		public float defaultButtonPressPoint
		{
			get
			{
				return m_DefaultButtonPressPoint;
			}
			set
			{
				if (m_DefaultButtonPressPoint != value)
				{
					m_DefaultButtonPressPoint = value;
					OnChange();
				}
			}
		}

		public float defaultTapTime
		{
			get
			{
				return m_DefaultTapTime;
			}
			set
			{
				if (m_DefaultTapTime != value)
				{
					m_DefaultTapTime = value;
					OnChange();
				}
			}
		}

		public float defaultSlowTapTime
		{
			get
			{
				return m_DefaultSlowTapTime;
			}
			set
			{
				if (m_DefaultSlowTapTime != value)
				{
					m_DefaultSlowTapTime = value;
					OnChange();
				}
			}
		}

		public float defaultHoldTime
		{
			get
			{
				return m_DefaultHoldTime;
			}
			set
			{
				if (m_DefaultHoldTime != value)
				{
					m_DefaultHoldTime = value;
					OnChange();
				}
			}
		}

		public float tapRadius
		{
			get
			{
				return m_TapRadius;
			}
			set
			{
				if (m_TapRadius != value)
				{
					m_TapRadius = value;
					OnChange();
				}
			}
		}

		public float multiTapDelayTime
		{
			get
			{
				return m_MultiTapDelayTime;
			}
			set
			{
				if (m_MultiTapDelayTime != value)
				{
					m_MultiTapDelayTime = value;
					OnChange();
				}
			}
		}

		public ReadOnlyArray<string> supportedDevices
		{
			get
			{
				return new ReadOnlyArray<string>(m_SupportedDevices);
			}
			set
			{
				if (supportedDevices.Count == value.Count)
				{
					bool flag = false;
					for (int i = 0; i < supportedDevices.Count; i++)
					{
						if (m_SupportedDevices[i] != value[i])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
				}
				m_SupportedDevices = value.ToArray();
				OnChange();
			}
		}

		internal void OnChange()
		{
			if (InputSystem.settings == this)
			{
				InputSystem.s_Manager.ApplySettings();
			}
		}
	}
}
