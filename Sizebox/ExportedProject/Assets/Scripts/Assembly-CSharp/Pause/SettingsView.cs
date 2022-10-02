using System.Collections.Generic;
using SizeboxUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Pause
{
	public class SettingsView : MonoBehaviour
	{
		protected Camera mainCamera;

		protected GridLayoutGroup gridGroup;

		private SettingsViewBuilder _builder;

		protected bool initialized;

		public GuiManager main;

		public bool noSet;

		protected static bool IsMainMenu
		{
			get
			{
				return Object.FindObjectOfType<MenuController>();
			}
		}

		protected string Title
		{
			get
			{
				return GetComponentInChildren<Text>().text;
			}
			set
			{
				GetComponentInChildren<Text>().text = value;
			}
		}

		private static void SetLabelSensitivity(GameObject go, bool sensitivity)
		{
			Text[] componentsInChildren = go.GetComponentsInChildren<Text>();
			foreach (Text text in componentsInChildren)
			{
				if (text.name == "Label")
				{
					text.color = (sensitivity ? Color.white : Color.gray);
					break;
				}
			}
		}

		protected static void SetLabelSensitivity(Toggle t, bool sensitivity)
		{
			SetLabelSensitivity(t.gameObject, sensitivity);
		}

		protected static void SetLabelSensitivity(Dropdown d, bool sensitivity)
		{
			SetLabelSensitivity(d.transform.parent.gameObject, sensitivity);
		}

		protected static void SetLabelSensitivity(Slider s, bool sensitivity)
		{
			SetLabelSensitivity(s.transform.parent.gameObject, sensitivity);
		}

		private void Awake()
		{
			mainCamera = Camera.main;
			gridGroup = GetComponentInChildren<GridLayoutGroup>();
			_builder = GetComponentInParent<SettingsViewBuilder>();
			GetComponentInChildren<Button>().onClick.AddListener(ClosePanel);
		}

		public virtual void ClosePanel()
		{
			Object.Destroy(base.gameObject);
		}

		protected void SetInputSliderInput(InputSlider i, string style = "####")
		{
			noSet = true;
			i.input.text = i.slider.value.ToString(style);
			noSet = false;
		}

		protected void SetInputSliderInput(InputSlider i, float f, string style = "####")
		{
			noSet = true;
			i.input.text = f.ToString(style);
			noSet = false;
		}

		protected void SetInputSliderInput(InputSlider i, int f)
		{
			noSet = true;
			i.input.text = f.ToString();
			noSet = false;
		}

		protected GameObject AddBinding(InputAction inputAction, GameObject overlay, Text overlayText)
		{
			return _builder.AddBinding(inputAction, gridGroup.transform, overlay, overlayText);
		}

		protected Button AddCustomKeyBind(string text)
		{
			return _builder.AddCustomKeyBind(text, gridGroup.transform);
		}

		protected Button AddButton(string text)
		{
			return _builder.AddButton(text, gridGroup.transform);
		}

		protected ToggleSlider AddToggleSlider(string text, float min, float max)
		{
			return _builder.AddToggleSlider(text, min, max, gridGroup.transform);
		}

		protected Toggle AddToggle(string text, bool state, UnityAction<bool> action)
		{
			Toggle toggle = AddToggle(text, state);
			toggle.onValueChanged.AddListener(action);
			return toggle;
		}

		protected Toggle AddToggle(string text, bool state)
		{
			Toggle toggle = AddToggle(text);
			toggle.isOn = state;
			return toggle;
		}

		protected Toggle AddToggle(string text)
		{
			return _builder.AddToggle(text, gridGroup.transform);
		}

		protected InputField AddInput(string text, string placeHolderText)
		{
			return _builder.AddInput(text, placeHolderText, gridGroup.transform);
		}

		public InputSlider AddInputSlider(string text, float min, float max, float state, string style = "####", bool wholeNumbers = false)
		{
			InputSlider inputSlider = AddInputSlider(text, min, max, wholeNumbers);
			SetInputSliderInput(inputSlider, state, style);
			return inputSlider;
		}

		protected InputSlider AddInputSlider(string text, float min, float max, bool wholeNumbers = false)
		{
			return _builder.AddInputSlider(text, min, max, this, gridGroup.transform, wholeNumbers);
		}

		protected Slider AddSlider(string text, float min, float max, float state)
		{
			Slider slider = AddSlider(text, min, max);
			slider.value = state;
			return slider;
		}

		protected Slider AddSlider(string text, float min, float max)
		{
			return _builder.AddSlider(text, min, max, gridGroup.transform);
		}

		protected Text AddReadOnly(string title, string value)
		{
			return _builder.AddReadOnly(title, value, gridGroup.transform);
		}

		protected PathInput AddPath(string title, string value, bool write = false)
		{
			return _builder.AddPath(title, value, gridGroup.transform, write);
		}

		protected Dropdown AddDropdown(string text, List<Dropdown.OptionData> options)
		{
			return _builder.AddDropdown(text, options, gridGroup.transform);
		}

		protected Text AddHeader(string text)
		{
			return _builder.AddHeader(text, gridGroup.transform);
		}

		protected List<Dropdown.OptionData> CreateTextOptions(string[] strs, bool capital = true)
		{
			return _builder.CreateTextOptions(strs, capital);
		}

		private void OnEnable()
		{
			if (initialized)
			{
				UpdateValues();
			}
		}

		protected virtual void UpdateValues()
		{
		}
	}
}
