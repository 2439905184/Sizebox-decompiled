using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Pause
{
	public class ControlSettingsView : SettingsView
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static UnityAction<bool> _003C_003E9__2_0;

			public static UnityAction<bool> _003C_003E9__2_1;

			public static UnityAction<bool> _003C_003E9__2_2;

			public static UnityAction<bool> _003C_003E9__2_3;

			internal void _003CStart_003Eb__2_0(bool x)
			{
				PlayerPrefs.SetInt("EnableMouseZoom", x ? 1 : 0);
			}

			internal void _003CStart_003Eb__2_1(bool x)
			{
				PlayerPrefs.SetInt("EnableMouseWeaponRange", x ? 1 : 0);
			}

			internal void _003CStart_003Eb__2_2(bool x)
			{
				PlayerPrefs.SetInt("InvertYAxis", x ? 1 : 0);
			}

			internal void _003CStart_003Eb__2_3(bool x)
			{
				PlayerPrefs.SetInt("InvertMouseZoom", x ? 1 : 0);
			}
		}

		private UiBindingBox _uiBindingBox;

		private InputSlider _mouseSensitivity;

		private void Start()
		{
			base.Title = "Controls";
			_uiBindingBox = UiBindingBox.Create();
			InputActionMap inputActionMap = InputManager.inputs.Player.Get();
			AddHeader("Common Player");
			foreach (InputAction item in inputActionMap)
			{
				AddBinding(item, _uiBindingBox.gameObject, _uiBindingBox.message);
			}
			InputActionMap inputActionMap2 = InputManager.inputs.Macro.Get();
			AddHeader("Macro");
			foreach (InputAction item2 in inputActionMap2)
			{
				AddBinding(item2, _uiBindingBox.gameObject, _uiBindingBox.message);
			}
			InputActionMap inputActionMap3 = InputManager.inputs.Micro.Get();
			AddHeader("Micro");
			foreach (InputAction item3 in inputActionMap3)
			{
				AddBinding(item3, _uiBindingBox.gameObject, _uiBindingBox.message);
			}
			InputActionMap inputActionMap4 = InputManager.inputs.EditMode.Get();
			AddHeader("Edit Mode");
			foreach (InputAction item4 in inputActionMap4)
			{
				AddBinding(item4, _uiBindingBox.gameObject, _uiBindingBox.message);
			}
			AddHeader("Mouse");
			AddToggle("Adjust Camera Zoom with Scroll Wheel", PlayerPrefs.GetInt("EnableMouseZoom", 1) == 1, _003C_003Ec._003C_003E9__2_0 ?? (_003C_003Ec._003C_003E9__2_0 = _003C_003Ec._003C_003E9._003CStart_003Eb__2_0));
			AddToggle("Adjust Weapon Range with Scroll Wheel", PlayerPrefs.GetInt("EnableMouseWeaponRange", 1) == 1, _003C_003Ec._003C_003E9__2_1 ?? (_003C_003Ec._003C_003E9__2_1 = _003C_003Ec._003C_003E9._003CStart_003Eb__2_1));
			AddToggle("Invert Vertical Camera Axis", PlayerPrefs.GetInt("InvertYAxis", 0) == 1, _003C_003Ec._003C_003E9__2_2 ?? (_003C_003Ec._003C_003E9__2_2 = _003C_003Ec._003C_003E9._003CStart_003Eb__2_2));
			AddToggle("Invert Zoom direction", PlayerPrefs.GetInt("InvertMouseZoom", 0) == 1, _003C_003Ec._003C_003E9__2_3 ?? (_003C_003Ec._003C_003E9__2_3 = _003C_003Ec._003C_003E9._003CStart_003Eb__2_3));
			_mouseSensitivity = AddInputSlider("Sensitivity", 0.1f, 5f);
			_mouseSensitivity.slider.value = GlobalPreferences.MouseSensibility.value;
			_mouseSensitivity.slider.onValueChanged.AddListener(_003CStart_003Eb__2_4);
			MouseSensitivityChanged(GlobalPreferences.MouseSensibility.value);
			InputActionMap inputActionMap5 = InputManager.inputs.Misc.Get();
			AddHeader("Misc");
			foreach (InputAction item5 in inputActionMap5)
			{
				AddBinding(item5, _uiBindingBox.gameObject, _uiBindingBox.message);
			}
			AddButton("Reset to Defaults").onClick.AddListener(OnResetPress);
		}

		private void MouseSensitivityChanged(float value, bool write = false)
		{
			_mouseSensitivity.input.text = value.ToString("0.00", CultureInfo.CurrentCulture);
			if (write)
			{
				GlobalPreferences.MouseSensibility.value = value;
			}
		}

		private void OnResetPress()
		{
			InputManager.inputPreferences.ResetAll();
			InputManager.inputPreferences.SaveAllOverrides();
			UnityInputSystemRebind[] componentsInChildren = base.gameObject.GetComponentsInChildren<UnityInputSystemRebind>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].UpdateBindingDisplay();
			}
			GlobalPreferences.MouseSensibility.Reset();
			_mouseSensitivity.slider.value = GlobalPreferences.MouseSensibility.value;
		}

		private void OnEnable()
		{
			StateManager.Mouse.Add();
		}

		private void OnDisable()
		{
			StateManager.Mouse.Remove();
			InputManager.inputPreferences.SaveAllOverrides();
			InputManager.inputPreferences.ApplyMouseSettings();
		}

		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(_uiBindingBox.gameObject);
		}

		[CompilerGenerated]
		private void _003CStart_003Eb__2_4(float v)
		{
			MouseSensitivityChanged(v, true);
		}
	}
}
