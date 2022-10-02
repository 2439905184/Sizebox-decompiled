using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorPickerTester : MonoBehaviour
{
	[FormerlySerializedAs("renderer")]
	public Renderer myRenderer;

	public ColorPicker picker;

	[FormerlySerializedAs("Color")]
	public Color color = Color.red;

	private void Start()
	{
		picker.onValueChanged.AddListener(_003CStart_003Eb__3_0);
		myRenderer.material.color = picker.CurrentColor;
		picker.CurrentColor = color;
	}

	[CompilerGenerated]
	private void _003CStart_003Eb__3_0(Color color)
	{
		myRenderer.material.color = color;
		this.color = color;
	}
}
