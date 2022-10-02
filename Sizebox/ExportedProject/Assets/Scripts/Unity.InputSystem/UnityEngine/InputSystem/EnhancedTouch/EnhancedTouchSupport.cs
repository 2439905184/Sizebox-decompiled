using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public static class EnhancedTouchSupport
	{
		private static int s_Enabled;

		private static InputSettings.UpdateMode s_UpdateMode;

		public static bool enabled
		{
			get
			{
				return s_Enabled > 0;
			}
		}

		public static void Enable()
		{
			s_Enabled++;
			if (s_Enabled <= 1)
			{
				InputSystem.onDeviceChange += OnDeviceChange;
				InputSystem.onBeforeUpdate += Touch.BeginUpdate;
				InputSystem.onSettingsChange += OnSettingsChange;
				SetUpState();
			}
		}

		public static void Disable()
		{
			if (enabled)
			{
				s_Enabled--;
				if (s_Enabled <= 0)
				{
					InputSystem.onDeviceChange -= OnDeviceChange;
					InputSystem.onBeforeUpdate -= Touch.BeginUpdate;
					InputSystem.onSettingsChange -= OnSettingsChange;
					TearDownState();
				}
			}
		}

		private static void SetUpState()
		{
			Touch.s_PlayerState.updateMask = InputUpdateType.Dynamic | InputUpdateType.Manual;
			s_UpdateMode = InputSystem.settings.updateMode;
			foreach (InputDevice device in InputSystem.devices)
			{
				OnDeviceChange(device, InputDeviceChange.Added);
			}
		}

		private static void TearDownState()
		{
			foreach (InputDevice device in InputSystem.devices)
			{
				OnDeviceChange(device, InputDeviceChange.Removed);
			}
			Touch.s_PlayerState.Destroy();
			Touch.s_PlayerState = default(Touch.FingerAndTouchState);
		}

		private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			switch (change)
			{
			case InputDeviceChange.Added:
			{
				Touchscreen screen2;
				if ((screen2 = device as Touchscreen) != null)
				{
					Touch.AddTouchscreen(screen2);
				}
				break;
			}
			case InputDeviceChange.Removed:
			{
				Touchscreen screen;
				if ((screen = device as Touchscreen) != null)
				{
					Touch.RemoveTouchscreen(screen);
				}
				break;
			}
			}
		}

		private static void OnSettingsChange()
		{
			InputSettings.UpdateMode updateMode = InputSystem.settings.updateMode;
			if (s_UpdateMode != updateMode)
			{
				TearDownState();
				SetUpState();
			}
		}
	}
}
