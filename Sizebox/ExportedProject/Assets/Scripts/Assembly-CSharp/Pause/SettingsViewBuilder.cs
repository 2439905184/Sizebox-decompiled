using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using SizeboxUI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace Pause
{
	public class SettingsViewBuilder : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public GameObject newKeyBind;

			public Button btn;

			internal void _003CAddCustomKeyBind_003Eb__0()
			{
				if (newKeyBind.GetComponentInChildren<KeyBindInput>() == null)
				{
					newKeyBind.AddComponent<KeyBindInput>();
					newKeyBind.GetComponent<KeyBindInput>().oldKeyName = btn.GetComponentInChildren<Text>().text;
					btn.GetComponentInChildren<Text>().text = "Awaiting input...";
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass20_0
		{
			public SettingsViewBuilder _003C_003E4__this;

			public InputSlider newInputSlider;

			public SettingsView view;

			internal void _003CAddInputSlider_003Eb__0(string x)
			{
				_003C_003E4__this.InputSanity(newInputSlider, x, view);
			}
		}

		[SerializeField]
		private Toggle toggle;

		[SerializeField]
		private GameObject slider;

		[SerializeField]
		private GameObject readOnly;

		[SerializeField]
		private GameObject readOnlyPath;

		[SerializeField]
		private GameObject editablePath;

		[SerializeField]
		private GameObject dropdown;

		[SerializeField]
		private GameObject header;

		[SerializeField]
		private GameObject toggleSlider;

		[SerializeField]
		private GameObject button;

		[SerializeField]
		private GameObject input;

		[SerializeField]
		private GameObject inputSlider;

		[SerializeField]
		private GameObject keyBind;

		[SerializeField]
		private GameObject binding;

		private List<InputBinding> GetBindings(ReadOnlyArray<InputBinding> inputBindings)
		{
			List<InputBinding> list = new List<InputBinding>();
			list.Add(inputBindings[0]);
			if (inputBindings[0].isComposite)
			{
				int count = inputBindings.Count;
				for (int i = 1; i < count; i++)
				{
					if (inputBindings[i].isComposite)
					{
						list.Add(inputBindings[i]);
						return list;
					}
				}
			}
			list.Add(inputBindings[1]);
			return list;
		}

		public GameObject AddBinding(InputAction inputAction, Transform parent, GameObject overlay, Text overlayText)
		{
			GameObject gameObject = Object.Instantiate(binding, parent);
			Button[] componentsInChildren = gameObject.GetComponentsInChildren<Button>();
			List<InputBinding> bindings = GetBindings(inputAction.bindings);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				UnityInputSystemRebind component = componentsInChildren[i].GetComponent<UnityInputSystemRebind>();
				componentsInChildren[i].onClick.AddListener(component.StartInteractiveRebind);
				component.rebindOverlay = overlay;
				component.rebindPrompt = overlayText;
				component.actionReference = InputActionReference.Create(inputAction);
				component.bindingId = bindings[i].id.ToString();
			}
			return gameObject;
		}

		public Button AddCustomKeyBind(string text, Transform parent)
		{
			_003C_003Ec__DisplayClass15_0 _003C_003Ec__DisplayClass15_ = new _003C_003Ec__DisplayClass15_0();
			_003C_003Ec__DisplayClass15_.newKeyBind = Object.Instantiate(keyBind, parent);
			_003C_003Ec__DisplayClass15_.newKeyBind.GetComponentInChildren<Text>().text = text;
			_003C_003Ec__DisplayClass15_.btn = _003C_003Ec__DisplayClass15_.newKeyBind.GetComponentInChildren<Button>();
			_003C_003Ec__DisplayClass15_.btn.onClick.AddListener(_003C_003Ec__DisplayClass15_._003CAddCustomKeyBind_003Eb__0);
			return _003C_003Ec__DisplayClass15_.btn;
		}

		public Button AddButton(string text, Transform parent)
		{
			Button component = Object.Instantiate(button, parent).GetComponent<Button>();
			component.GetComponentInChildren<Text>().text = text;
			return component;
		}

		public ToggleSlider AddToggleSlider(string text, float min, float max, Transform parent)
		{
			ToggleSlider result = default(ToggleSlider);
			GameObject gameObject = Object.Instantiate(toggleSlider, parent);
			gameObject.GetComponentInChildren<Text>().text = text;
			result.toggle = gameObject.GetComponent<Toggle>();
			result.slider = gameObject.GetComponentInChildren<Slider>();
			result.slider.minValue = min;
			result.slider.maxValue = max;
			return result;
		}

		public Toggle AddToggle(string text, Transform parent)
		{
			Toggle obj = Object.Instantiate(toggle, parent);
			obj.GetComponentInChildren<Text>().text = text;
			return obj;
		}

		public InputField AddInput(string text, string placeHolderText, Transform parent)
		{
			GameObject obj = Object.Instantiate(input, parent);
			obj.GetComponentsInChildren<Text>()[0].text = text;
			obj.GetComponentsInChildren<Text>()[1].text = placeHolderText;
			InputField componentInChildren = obj.GetComponentInChildren<InputField>();
			Sbox.AddSBoxInputFieldEvents(componentInChildren);
			return componentInChildren;
		}

		public InputSlider AddInputSlider(string text, float min, float max, SettingsView view, Transform parent, bool wholeNumbers = false)
		{
			_003C_003Ec__DisplayClass20_0 _003C_003Ec__DisplayClass20_ = new _003C_003Ec__DisplayClass20_0();
			_003C_003Ec__DisplayClass20_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass20_.view = view;
			_003C_003Ec__DisplayClass20_.newInputSlider = default(InputSlider);
			GameObject gameObject = Object.Instantiate(inputSlider, parent);
			gameObject.GetComponentInChildren<Text>().text = text;
			_003C_003Ec__DisplayClass20_.newInputSlider.input = gameObject.GetComponentInChildren<InputField>();
			Sbox.AddSBoxInputFieldEvents(_003C_003Ec__DisplayClass20_.newInputSlider.input);
			_003C_003Ec__DisplayClass20_.newInputSlider.slider = gameObject.GetComponentInChildren<Slider>();
			_003C_003Ec__DisplayClass20_.newInputSlider.slider.minValue = min;
			_003C_003Ec__DisplayClass20_.newInputSlider.slider.maxValue = max;
			_003C_003Ec__DisplayClass20_.newInputSlider.slider.wholeNumbers = wholeNumbers;
			_003C_003Ec__DisplayClass20_.newInputSlider.input.onEndEdit.AddListener(_003C_003Ec__DisplayClass20_._003CAddInputSlider_003Eb__0);
			return _003C_003Ec__DisplayClass20_.newInputSlider;
		}

		public Slider AddSlider(string text, float min, float max, Transform parent)
		{
			GameObject obj = Object.Instantiate(slider, parent);
			obj.GetComponentInChildren<Text>().text = text;
			Slider componentInChildren = obj.GetComponentInChildren<Slider>();
			componentInChildren.minValue = min;
			componentInChildren.maxValue = max;
			return componentInChildren;
		}

		public Text AddReadOnly(string title, string value, Transform parent)
		{
			Text[] componentsInChildren = Object.Instantiate(readOnly, parent).GetComponentsInChildren<Text>();
			Text result = null;
			Text[] array = componentsInChildren;
			foreach (Text text in array)
			{
				if (text.gameObject.name == "Property")
				{
					text.text = title;
					continue;
				}
				text.text = value;
				result = text;
			}
			return result;
		}

		public PathInput AddPath(string title, string value, Transform parent, bool write = false)
		{
			PathInput pathInput = default(PathInput);
			pathInput.gameObject = Object.Instantiate(write ? editablePath : readOnlyPath, parent, false);
			PathInput result = pathInput;
			result.gameObject.GetComponentInChildren<Text>().text = title;
			result.inputField = result.gameObject.GetComponentInChildren<InputField>();
			Sbox.AddSBoxInputFieldEvents(result.inputField);
			result.inputField.text = value;
			result.viewButton = result.gameObject.transform.Find("View").GetComponent<Button>();
			if (write)
			{
				result.editButton = result.gameObject.transform.Find("Edit").GetComponent<Button>();
			}
			return result;
		}

		public Dropdown AddDropdown(string text, List<Dropdown.OptionData> options, Transform parent)
		{
			GameObject obj = Object.Instantiate(dropdown, parent);
			obj.GetComponentInChildren<Text>().text = text;
			Dropdown componentInChildren = obj.GetComponentInChildren<Dropdown>();
			componentInChildren.AddOptions(options);
			return componentInChildren;
		}

		public Text AddHeader(string text, Transform parent)
		{
			Text componentInChildren = Object.Instantiate(header, parent).GetComponentInChildren<Text>();
			componentInChildren.text = text;
			componentInChildren.alignment = TextAnchor.MiddleCenter;
			return componentInChildren;
		}

		public List<Dropdown.OptionData> CreateTextOptions(string[] strings, bool capital = true)
		{
			List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
			for (int i = 0; i < strings.Length; i++)
			{
				string text = strings[i];
				if (capital)
				{
					text = SettingsViewUtil.Capitalize(text);
				}
				Dropdown.OptionData item = new Dropdown.OptionData
				{
					text = text
				};
				list.Add(item);
			}
			return list;
		}

		private void InputSanity(InputSlider i, string s, SettingsView view)
		{
			if (view.noSet || string.IsNullOrEmpty(s))
			{
				return;
			}
			bool flag = false;
			for (int j = 0; j < s.Length; j++)
			{
				if (SettingsViewUtil.ValidateDigit(s[j]) != 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				float num = float.Parse(s, CultureInfo.CurrentCulture);
				if (num < i.slider.minValue)
				{
					num = i.slider.minValue;
				}
				else if (num > i.slider.maxValue)
				{
					num = i.slider.maxValue;
				}
				i.slider.value = num;
			}
		}
	}
}
