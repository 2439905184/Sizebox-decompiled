using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.HSVPicker
{
	[Serializable]
	public class ColorPickerSetup
	{
		public enum ColorHeaderShowing
		{
			Hide = 0,
			ShowColor = 1,
			ShowColorCode = 2,
			ShowAll = 3
		}

		[Serializable]
		public class UiElements
		{
			public RectTransform[] Elements;

			public void Toggle(bool active)
			{
				for (int i = 0; i < Elements.Length; i++)
				{
					Elements[i].gameObject.SetActive(active);
				}
			}
		}

		public bool ShowRgb = true;

		public bool ShowHsv;

		public bool ShowAlpha = true;

		public bool ShowColorBox = true;

		public bool ShowColorSliderToggle = true;

		public ColorHeaderShowing ShowHeader = ColorHeaderShowing.ShowAll;

		public UiElements RgbSliders;

		public UiElements HsvSliders;

		public UiElements ColorToggleElement;

		public UiElements AlphaSlidiers;

		public UiElements ColorHeader;

		public UiElements ColorCode;

		public UiElements ColorPreview;

		public UiElements ColorBox;

		public Text SliderToggleButtonText;

		public string PresetColorsId = "default";

		public Color[] DefaultPresetColors;
	}
}
