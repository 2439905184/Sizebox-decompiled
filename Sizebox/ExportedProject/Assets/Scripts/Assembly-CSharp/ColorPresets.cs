using System.Collections.Generic;
using Assets.HSVPicker;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresets : MonoBehaviour
{
	public ColorPicker picker;

	public GameObject[] presets;

	public Image createPresetImage;

	private ColorPresetList _colors;

	private void Awake()
	{
		picker.onValueChanged.AddListener(ColorChanged);
	}

	private void Start()
	{
		_colors = ColorPresetManager.Get(picker.Setup.PresetColorsId);
		if (_colors.Colors.Count < picker.Setup.DefaultPresetColors.Length)
		{
			_colors.UpdateList(picker.Setup.DefaultPresetColors);
		}
		_colors.OnColorsUpdated += OnColorsUpdate;
		OnColorsUpdate(_colors.Colors);
	}

	private void OnColorsUpdate(List<Color> colors)
	{
		for (int i = 0; i < presets.Length; i++)
		{
			if (colors.Count <= i)
			{
				presets[i].SetActive(false);
				continue;
			}
			presets[i].SetActive(true);
			presets[i].GetComponent<Image>().color = colors[i];
		}
		createPresetImage.gameObject.SetActive(colors.Count < presets.Length);
	}

	public void CreatePresetButton()
	{
		_colors.AddColor(picker.CurrentColor);
	}

	public void PresetSelect(Image sender)
	{
		picker.CurrentColor = sender.color;
	}

	private void ColorChanged(Color color)
	{
		createPresetImage.color = color;
	}
}
