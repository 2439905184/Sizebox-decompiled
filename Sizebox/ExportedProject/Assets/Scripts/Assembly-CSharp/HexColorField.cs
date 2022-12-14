using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class HexColorField : MonoBehaviour
{
	public ColorPicker hsvpicker;

	public bool displayAlpha;

	private InputField hexInputField;

	private void Awake()
	{
		hexInputField = GetComponent<InputField>();
		hexInputField.onEndEdit.AddListener(UpdateColor);
		hsvpicker.onValueChanged.AddListener(UpdateHex);
	}

	private void OnDestroy()
	{
		hexInputField.onValueChanged.RemoveListener(UpdateColor);
		hsvpicker.onValueChanged.RemoveListener(UpdateHex);
	}

	private void UpdateHex(Color newColor)
	{
		hexInputField.text = ColorToHex(newColor);
	}

	private void UpdateColor(string newHex)
	{
		if (!newHex.StartsWith("#"))
		{
			newHex = "#" + newHex;
		}
		Color color;
		if (ColorUtility.TryParseHtmlString(newHex, out color))
		{
			hsvpicker.CurrentColor = color;
		}
		else
		{
			Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
		}
	}

	private string ColorToHex(Color32 color)
	{
		if (!displayAlpha)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
		}
		return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
	}
}
