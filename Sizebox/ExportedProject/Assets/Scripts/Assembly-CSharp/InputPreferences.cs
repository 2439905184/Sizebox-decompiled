using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputPreferences
{
	public static class Preferences
	{
		public const string MouseInvertYAxis = "InvertYAxis";

		public const string MouseScrollWheelEnableZoom = "EnableMouseZoom";

		public const string MouseScrollWheelEnableRange = "EnableMouseWeaponRange";

		public const string MouseScrollInvertZoom = "InvertMouseZoom";
	}

	public static class Id
	{
		public const string MouseLookDelta = "8540e8e9-f26e-4ee0-820a-3cd06ecd7673";

		public const string MouseZoomScroll = "e354db56-0a11-4c81-bf27-4d702448aedb";

		public const string MouseRangeScroll = "213b689d-ddd0-465e-86e3-2802bd27657b";
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		public Guid id;

		internal bool _003CApplyMouseSettings_003Eb__0(InputBinding x)
		{
			return x.id == id;
		}

		internal bool _003CApplyMouseSettings_003Eb__1(InputBinding x)
		{
			return x.id == id;
		}

		internal bool _003CApplyMouseSettings_003Eb__2(InputBinding x)
		{
			return x.id == id;
		}
	}

	private readonly Dictionary<Guid, string> _overrides = new Dictionary<Guid, string>();

	private readonly List<InputActionMap> _maps;

	public InputPreferences(Inputs input)
	{
		InputActionMap item = input.EditMode.Get();
		InputActionMap item2 = input.EditMode.Get();
		InputActionMap item3 = input.Player.Get();
		InputActionMap item4 = input.Macro.Get();
		InputActionMap item5 = input.Micro.Get();
		InputActionMap item6 = input.Misc.Get();
		_maps = new List<InputActionMap> { item, item2, item3, item4, item5, item6 };
	}

	public void SaveAllOverrides()
	{
		foreach (InputActionMap map in _maps)
		{
			foreach (InputBinding binding in map.bindings)
			{
				if (!binding.isComposite)
				{
					if (!string.IsNullOrEmpty(binding.overridePath))
					{
						_overrides[binding.id] = binding.overridePath;
					}
					else
					{
						ClearOverrideSave(binding.id);
					}
				}
			}
			foreach (KeyValuePair<Guid, string> @override in _overrides)
			{
				if (!string.IsNullOrEmpty(@override.Value))
				{
					PlayerPrefs.SetString(@override.Key.ToString(), @override.Value);
				}
			}
		}
	}

	public void LoadAllOverrides()
	{
		foreach (InputActionMap map in _maps)
		{
			ReadOnlyArray<InputBinding> bindings = map.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				if (bindings[i].isComposite)
				{
					continue;
				}
				string value = bindings[i].id.ToString();
				if (PlayerPrefs.HasKey(value))
				{
					_overrides.Add(bindings[i].id, PlayerPrefs.GetString(value));
					if (_overrides.TryGetValue(bindings[i].id, out value))
					{
						map.ApplyBindingOverride(i, new InputBinding
						{
							overridePath = value
						});
					}
				}
			}
		}
	}

	public void ResetAll()
	{
		foreach (InputActionMap map in _maps)
		{
			foreach (InputBinding binding in map.bindings)
			{
				if (!binding.isComposite)
				{
					ResetBind(binding, map);
				}
			}
		}
	}

	public void ResetBind(InputBinding binding, InputActionMap actionMap)
	{
		binding.overridePath = null;
		actionMap.ApplyBindingOverride(binding);
		ClearOverrideSave(binding.id);
	}

	private void ClearOverrideSave(Guid bindingId)
	{
		string key = bindingId.ToString();
		if (PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.DeleteKey(key);
		}
		_overrides.Remove(bindingId);
	}

	public void ApplyMouseSettings()
	{
		_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = new _003C_003Ec__DisplayClass10_0();
		InputActionMap inputActionMap = InputManager.inputs.Player.Get();
		_003C_003Ec__DisplayClass10_.id = new Guid("8540e8e9-f26e-4ee0-820a-3cd06ecd7673");
		InputBinding bindingOverride = inputActionMap.bindings.First(_003C_003Ec__DisplayClass10_._003CApplyMouseSettings_003Eb__0);
		List<string> list = new List<string>();
		float value = GlobalPreferences.MouseSensibility.value;
		list.Add("ScaleVector2(x=" + value.ToString(CultureInfo.InvariantCulture) + ",y=" + value.ToString(CultureInfo.InvariantCulture) + ")");
		if (PlayerPrefs.GetInt("InvertYAxis", 0) == 1)
		{
			list.Add("InvertVector2(invertX=false)");
		}
		string overrideProcessors = string.Join(",", list);
		bindingOverride.overrideProcessors = overrideProcessors;
		inputActionMap.ApplyBindingOverride(bindingOverride);
		_003C_003Ec__DisplayClass10_.id = new Guid("e354db56-0a11-4c81-bf27-4d702448aedb");
		bindingOverride = inputActionMap.bindings.First(_003C_003Ec__DisplayClass10_._003CApplyMouseSettings_003Eb__1);
		bindingOverride.overridePath = ((PlayerPrefs.GetInt("EnableMouseZoom", 1) == 0) ? string.Empty : null);
		bindingOverride.overrideProcessors = ((PlayerPrefs.GetInt("InvertMouseZoom", 0) == 1) ? "Invert" : null);
		inputActionMap.ApplyBindingOverride(bindingOverride);
		inputActionMap = InputManager.inputs.Micro.Get();
		_003C_003Ec__DisplayClass10_.id = new Guid("213b689d-ddd0-465e-86e3-2802bd27657b");
		bindingOverride = inputActionMap.bindings.First(_003C_003Ec__DisplayClass10_._003CApplyMouseSettings_003Eb__2);
		bindingOverride.overridePath = ((PlayerPrefs.GetInt("EnableMouseWeaponRange", 1) == 0) ? string.Empty : null);
		inputActionMap.ApplyBindingOverride(bindingOverride);
	}
}
