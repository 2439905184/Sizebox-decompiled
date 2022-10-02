using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CanvasReferenceScalar : MonoBehaviour
{
	public Canvas canvas;

	private void OnRectTransformDimensionsChange()
	{
		CameraSettings.SetUiScale(canvas, GlobalPreferences.UIScale.value);
	}
}
